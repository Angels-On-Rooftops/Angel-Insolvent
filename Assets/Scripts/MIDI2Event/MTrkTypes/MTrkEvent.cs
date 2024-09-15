using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MIDI2EventSystem
{
    internal class MTrkEvent
    {
        public uint Delta { get; set; }

        public MTrkEvent(uint delta)
        {
            Delta = delta;
        }
    }
}
