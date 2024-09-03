using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace midi2event
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
