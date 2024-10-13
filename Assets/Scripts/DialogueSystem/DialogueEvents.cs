using System;
using System.Reflection;
using UnityEngine.Assertions;

namespace Assets.Scripts.DialogueSystem
{
    public class DialogueEvents
    {
        public static event Action RancidVibes;

        #region Singleton BS
        private static DialogueEvents instance = null;
        private static readonly object instanceLock = new object(); //thread-safe for co-routines

        public static DialogueEvents Instance
        {
            get
            {
                lock (instanceLock)
                {
                    instance ??= new DialogueEvents();
                    return instance;
                }
            }
        }
        #endregion

        public static void FireEvent(string eventName)
        {
            Assert.IsTrue(
                Instance.GetType().GetEvent(eventName, BindingFlags.Static) is not null,
                $"{eventName} is not a static dialogue event."
            );

            // TODO check that this works
            Instance
                .GetType()
                .GetEvent(eventName, BindingFlags.Static)
                .GetRaiseMethod()
                ?.Invoke(null, null);
        }
    }
}
