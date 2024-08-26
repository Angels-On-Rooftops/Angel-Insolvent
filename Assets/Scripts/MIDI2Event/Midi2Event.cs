using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace midi2event
{
    public class Midi2Event
    {
        private Dictionary<int, Action> _startEvents;
        private Dictionary<int, Action> _stopEvents;
        private Action _endEvent;
        private Queue<MTrkEvent> _messages;
        private Queue<MTrkEvent> _bin;
        private MidiReader _reader;
        private uint _ticksPerQuarter;

        private double _deltaTimeSinceLastUpdate = 0;

        private double _deltaTimeToNextUpdate = 0;

        private uint _usPerQuarter = 500000;
        private bool _isPlaying = false;
        private readonly int TET = 12;
        private readonly double US_TO_S = 1e-6;

        public Midi2Event(string filePath, int lowestOctave = -1)
        {
            _startEvents = new Dictionary<int, Action>();
            _stopEvents = new Dictionary<int, Action>();
            _endEvent = () => { };
            _reader = new MidiReader();
            (_ticksPerQuarter, _messages) = _reader.Read(filePath);
            _bin = new();

            if (_messages.Count <= 0)
            {
                Debug.WriteLine("Provided midi generated no events, was this intended?");
            }
        }

        public void Update(double deltaTime)
        {
            if (_isPlaying && _messages.Count > 0)
            {
                _deltaTimeSinceLastUpdate += deltaTime;
                while (_deltaTimeToNextUpdate <= _deltaTimeSinceLastUpdate)
                {
                    double makeup = _deltaTimeSinceLastUpdate - _deltaTimeToNextUpdate;
                    MTrkEvent toProcess = _messages.Dequeue();
                    GetEvent(toProcess).Invoke();
                    _bin.Enqueue(toProcess);

                    _deltaTimeSinceLastUpdate = 0;
                    if (_messages.Count <= 0)
                    {
                        _isPlaying = false;
                        return;
                    }
                    _deltaTimeToNextUpdate = DeltaToDeltaTime(_messages.Peek().Delta) - makeup;
                }
            }
        }

        private Action GetEvent(MTrkEvent e)
        {
            return e switch
            {
                NoteOnEvent on => TryGetEvent(_startEvents, on.Note),
                NoteOffEvent off => TryGetEvent(_stopEvents, off.Note),
                EndTrackMeta => _endEvent,
                SetTempoMeta st
                    => () =>
                    {
                        _usPerQuarter = st.USPerQuarter;
                    },
                _ => () => { }
            };
        }

        private Action TryGetEvent(Dictionary<int, Action> dict, int index)
        {
            if (dict.ContainsKey(index))
            {
                return dict[index];
            }
            return () => { };
        }

        private double DeltaToDeltaTime(uint delta)
        {
            return delta * (_usPerQuarter / _ticksPerQuarter) * US_TO_S;
        }

        public void Back()
        {
            _deltaTimeSinceLastUpdate = 0;
            _deltaTimeToNextUpdate = 0;
            _usPerQuarter = 500000;
            while (_messages.Count > 0)
            {
                MTrkEvent transfer = _messages.Dequeue();
                _bin.Enqueue(transfer);
            }
            _messages = new(_bin);
            _bin = new();
        }

        public void Reset()
        {
            Back();
            _isPlaying = false;
        }

        public void Play()
        {
            if (_messages.Count <= 0)
            {
                return;
            }
            _isPlaying = true;
            _deltaTimeToNextUpdate = DeltaToDeltaTime(_messages.Peek().Delta);
        }

        public void Pause()
        {
            _isPlaying = false;
        }

        private int ToNoteId(Notes note, int octave)
        {
            return TET * (octave + 2) + (int)note;
        }

        public Action Subscribe(
            Action action,
            Notes note = 0,
            int octave = 0,
            SubType type = SubType.Start
        )
        {
            if (type == SubType.End)
            {
                _endEvent += action;
                return () =>
                {
                    _endEvent -= action;
                };
            }
            Dictionary<int, Action> events = ToNoteMap(type);
            int noteId = ToNoteId(note, octave);
            if (!events.ContainsKey(noteId))
            {
                events.Add(noteId, () => { });
            }
            events[noteId] += action;
            return () =>
            {
                events[noteId] -= action;
            };
        }

        private Dictionary<int, Action> ToNoteMap(SubType type) =>
            type switch
            {
                SubType.Start => _startEvents,
                SubType.Stop => _stopEvents,
                _ => throw new NotImplementedException("No note map for specified SubType!")
            };

        public enum SubType
        {
            Start,
            Stop,
            End
        }

        public enum Notes
        {
            C = 0,
            Cs = 1,
            D = 2,
            Ds = 3,
            E = 4,
            F = 5,
            Fs = 6,
            G = 7,
            Gs = 8,
            A = 9,
            As = 10,
            B = 11
        }
    }
}
