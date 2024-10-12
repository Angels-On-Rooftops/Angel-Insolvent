using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Dialogue_System
{
    class DialogueFlags : IPersistableData
    {
        HashSet<string> Flags = new();

        Maid maid = new();

        public void Enable()
        {
            maid.GiveEvent(DataPersistenceManager.Instance, "onSaveTriggered", SaveData);
            maid.GiveEvent(DataPersistenceManager.Instance, "onLoadTriggered", LoadData);
        }

        public void Disable()
        {
            maid.Cleanup();
        }

        public bool FlagIsSet(string flag)
        {
            return Flags.Contains(flag);
        }

        public void SetFlag(string flag, bool value)
        {
            Action setFlag = (Flags.Contains(flag), value) switch
            {
                (true, false) => () => Flags.Remove(flag),
                (false, true) => () => Flags.Add(flag),
                _ => () => { },
            };

            setFlag();
        }

        public void LoadData()
        {
            var flags = DataPersistenceManager.LoadData<SerializableDialogueFlags>("DialogueFlags");
            Flags = flags.DialogueFlags.ToHashSet();
        }

        public void SaveData()
        {
            List<string> flags = Flags.ToList();
            DataPersistenceManager.SaveData(new SerializableDialogueFlags(flags));
        }

        [Serializable]
        internal class SerializableDialogueFlags
        {
            public List<string> DialogueFlags = new();

            public SerializableDialogueFlags(List<string> flags)
            {
                DialogueFlags = flags;
            }
        }
    }
}
