using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace midi2event
{
    internal class NoteOffEvent : MTrkEvent
    {
        public byte Note { get; set; }
        public byte Velocity { get; set; }

        public NoteOffEvent(uint delta, byte note, byte velocity)
            : base(delta)
        {
            this.Delta = delta;
            this.Note = note;
            this.Velocity = velocity;
        }
    }
}
