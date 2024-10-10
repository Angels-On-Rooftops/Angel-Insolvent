using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Dialogue_System
{
    public static class DialogueEvents
    {
        public static event Action RancidVibes;

        public static void FireEvent(string eventName)
        {
            // TODO
            //this.GetType().GetEvent(eventName).
        }
    }
}
