using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace midi2event
{
    internal class MidiReader
    {
        //midi format documentation: http://www.music.mcgill.ca/~ich/classes/mumt306/StandardMIDIfileformat.html

        private readonly uint MTHD_LENGTH = 6;
        private byte lastStatusByte;

        //read the midi file into a queue of MTrkEvents
        public (ushort, Queue<MTrkEvent>) Read(string fileName)
        {
            Stream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            ushort result1 = ReadHeader(fileStream);
            Queue<MTrkEvent> result2 = ReadTrack(fileStream);
            fileStream.Close();
            return (result1, result2);
        }

        //read the header of the midi file to ensure it is a supported format
        private ushort ReadHeader(Stream fileStream)
        {
            byte[] buffer = new byte[4];
            //MThd header
            fileStream.Read(buffer, 0, buffer.Length);
            if (BinaryPrimitives.ReadUInt32BigEndian(buffer) != (int)ChunkTypes.MThd)
            {
                throw new InvalidDataException("MThd chunk type expected!");
            }
            //MThd length
            fileStream.Read(buffer, 0, buffer.Length);
            if (BinaryPrimitives.ReadUInt32BigEndian(buffer) != MTHD_LENGTH)
            {
                throw new InvalidDataException("MThd needs length of " + MTHD_LENGTH + " !");
            }
            //MIDI file format
            //Currently only supports 0, which is fine for this system, but 1 + 2 should be impemented if expanding code for general MIDI functionality.
            fileStream.Read(buffer, 2, 2);
            if (BinaryPrimitives.ReadUInt32BigEndian(buffer) != 0)
            {
                throw new InvalidDataException(
                    "Only MIDI format 0 currently supported, sorry :<\nTry using a file with a single multi-channel track!"
                );
            }
            //Number of tracks (should be 1)
            fileStream.Read(buffer, 2, 2);
            if (BinaryPrimitives.ReadUInt32BigEndian(buffer) != 1)
            {
                throw new InvalidDataException("Only one track should be present!");
            }
            //delta time meaning (only supports ticks per quarter-note)
            fileStream.Read(buffer, 2, 2);
            ushort division = (ushort)BinaryPrimitives.ReadUInt32BigEndian(buffer);
            if ((division & 0x8000) != 0)
            {
                throw new InvalidDataException("Only supports ticks per quarter note for now :<");
            }
            ushort ticksPerQuarter = division;
            Debug.WriteLine(division);
            Debug.WriteLine(ticksPerQuarter);

            Debug.WriteLine("header successfully read!");
            return ticksPerQuarter;
        }

        private Queue<MTrkEvent> ReadTrack(Stream fileStream)
        {
            Queue<MTrkEvent> result = new();

            //track header parsing
            byte[] buffer = new byte[4];
            fileStream.Read(buffer, 0, buffer.Length);
            if (BinaryPrimitives.ReadUInt32BigEndian(buffer) != (int)ChunkTypes.MTrk)
            {
                throw new InvalidDataException("MTrk chunk type expected!");
            }
            fileStream.Read(buffer, 0, buffer.Length);
            uint trackLength = BinaryPrimitives.ReadUInt32BigEndian(buffer);

            //parse every event in the track
            bool trackOver = false;
            while (!trackOver)
            {
                MTrkEvent nextEvent = ReadNextMTrkEvent(fileStream);
                result.Enqueue(nextEvent);
                if (nextEvent is EndTrackMeta)
                {
                    trackOver = true;
                }
            }
            return result;
        }

        private MTrkEvent ReadNextMTrkEvent(Stream fileStream)
        {
            uint delta = ParseVarLen(fileStream);
            byte status = (byte)fileStream.ReadByte();

            //account for running status
            if ((status & 0x80) > 0)
            {
                lastStatusByte = status;
            }
            else
            {
                status = lastStatusByte;
                fileStream.Seek(fileStream.Position - 1, SeekOrigin.Begin);
            }

            //create the correct MTrkEvent type and advance the file
            switch (status)
            {
                case (byte)StatusTypes.NoteOn:
                {
                    byte noteNum = (byte)fileStream.ReadByte();
                    byte vel = (byte)fileStream.ReadByte();
                    return new NoteOnEvent(delta, noteNum, vel);
                }
                case (byte)StatusTypes.NoteOff:
                {
                    byte noteNum = (byte)fileStream.ReadByte();
                    byte vel = (byte)fileStream.ReadByte();
                    return new NoteOffEvent(delta, noteNum, vel);
                }
                case (byte)StatusTypes.PolyKeyPres:
                case (byte)StatusTypes.CtrlChange:
                case (byte)StatusTypes.PitchWheel:
                    fileStream.ReadByte();
                    fileStream.ReadByte();
                    return new MTrkEvent(delta);
                case (byte)StatusTypes.ProgChange:
                case (byte)StatusTypes.ChannelPres:
                    fileStream.ReadByte();
                    return new MTrkEvent(delta);
                case (byte)StatusTypes.MetaEvent:
                {
                    return ReadMetaEvent(fileStream, delta);
                }
                default:
                    throw new Exception(
                        "Unsupported chunk "
                            + status.ToString("X")
                            + " detected at byte "
                            + (fileStream.Position - 1).ToString("X")
                            + "!"
                    );
            }
        }

        private MTrkEvent ReadMetaEvent(Stream fileStream, uint delta)
        {
            byte status = (byte)fileStream.ReadByte();

            switch (status)
            {
                case (byte)MetaTypes.SetTempo:
                {
                    fileStream.ReadByte(); //length is always 3
                    byte[] buffer = new byte[4];
                    fileStream.Read(buffer, 1, 3);
                    uint usPerQuarter = BinaryPrimitives.ReadUInt32BigEndian(buffer);
                    return new SetTempoMeta(delta, usPerQuarter);
                }
                case (byte)MetaTypes.EndOfTrack:
                {
                    return new EndTrackMeta(delta);
                }
                default:
                    Debug.WriteLine(
                        "Unsupported meta chunk detected at "
                            + (fileStream.Position - 1).ToString("X")
                            + "! skipping..."
                    );
                    //skip over metadata
                    byte length = (byte)fileStream.ReadByte();
                    for (int i = 0; i < length; i++)
                    {
                        fileStream.ReadByte();
                    }
                    return new MTrkEvent(delta);
            }
        }

        private uint ParseVarLen(Stream fileStream)
        {
            //Parse variable length quantity value from stream
            uint result = 0;
            byte next;
            do
            {
                next = (byte)fileStream.ReadByte();
                result = (result << 7) | (next & 0x7Fu);
            } while ((next & 0x80) != 0);
            return result;
        }

        private enum ChunkTypes
        {
            MThd = 1297377380,
            MTrk = 1297379947
        }

        private enum StatusTypes
        {
            NoteOn = 0x90,
            NoteOff = 0x80,
            PolyKeyPres = 0xA0,
            CtrlChange = 0xB0,
            ProgChange = 0xC0,
            ChannelPres = 0xD0,
            PitchWheel = 0xE0,
            MetaEvent = 0xFF,
        }

        private enum MetaTypes
        {
            SetTempo = 0x51,
            EndOfTrack = 0x2F
        }
    }
}
