using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDI2EventSystem
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

        //microseconds per quarter note
        private uint _usPerQuarter = 500000;

        //conversion factor from microseconds to seconds
        private readonly double US_TO_S = 1e-6;
        private bool _isPlaying = false;
        private readonly int TET = 12;
        int lowestOctave;

        public double SecPerBeat
        {
            get => _usPerQuarter * US_TO_S;
        }

        public double BeatPerSec
        {
            get => 1 / SecPerBeat;
        }

        //returns true if this system is currently playing
        public bool IsPlaying
        {
            get => _isPlaying;
        }

        public Midi2Event(string filePath, int lowestOctave = -1)
        {
            _startEvents = new Dictionary<int, Action>();
            _stopEvents = new Dictionary<int, Action>();
            _endEvent = () => { };
            _reader = new MidiReader();
            _bin = new();
            this.lowestOctave = lowestOctave;
            //load chart
            (_ticksPerQuarter, _messages) = _reader.Read(filePath);
            if (_messages.Count <= 0)
            {
                Debug.WriteLine("Provided midi generated no events, was this intended?");
            }
        }

        //call this every update frame in whatever engine/tool you're using
        public void Update(double deltaTime)
        {
            //do nothing if the system isn't playing or there are no more events
            if (!(_isPlaying && _messages.Count > 0))
            {
                return;
            }

            //trigger every event that is relevant at this deltaTime
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

        //return the event in the system related to MTrkEvent e
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

        //convert MIDI format 0 delta-time to a delta-time in seconds
        private double DeltaToDeltaTime(uint delta)
        {
            return delta * (_usPerQuarter / _ticksPerQuarter) * US_TO_S;
        }

        //rewind the system to the beginning of the track
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

        //rewind the system and stop it from playing
        public void Stop()
        {
            Back();
            _isPlaying = false;
        }

        //start playing the event system chart
        public void Play()
        {
            if (_messages.Count <= 0)
            {
                return;
            }
            _isPlaying = true;
            _deltaTimeToNextUpdate = DeltaToDeltaTime(_messages.Peek().Delta);
        }

        //pause the event system chart
        public void Pause()
        {
            _isPlaying = false;
        }

        //convert a note to its MIDI byte ID
        private int ToNoteId(Notes note, int octave)
        {
            return TET * (octave - lowestOctave) + (int)note;
        }

        /*
         *  subscribes an action to trigger when the specific event occurs in the midi file
         *  returns a function which unsubscribes the action from the event
         *
         *  params:
         *  action - the action to subscribe to the event system
         *  note - the midi note of the event to subscribe to
         *  octave - the octave of the note in the midi chart
         *  type - the type of subscription to use (start of note, end of note, end of chart)
         */
        public Action Subscribe(
            Action action,
            Notes note = 0,
            int octave = 0,
            SubType type = SubType.Start
        )
        {
            if (type == SubType.ChartEnd)
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

        //get the event map for the specified subscription type
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
            ChartEnd
        }
    }
}
