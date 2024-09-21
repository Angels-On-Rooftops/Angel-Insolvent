using System;
using System.Collections.Generic;

public class Maid
{
    readonly List<Action> ToRunOnCleanup = new();

    // add an event to the maid's list, the event will be
    // unsubscribed when Cleanup() is called
    //
    // C# events are not first class so this is the best we can do for now
    //
    // example usage:
    // GiveEvent<Action>(
    //   () => Debug.Log("Event Ran!"),
    //   func => event += func,
    //   func => event -= func
    // );
    public void GiveEvent<T>(T funcToBind, Action<T> subscription, Action<T> unsubscription)
    {
        subscription(funcToBind);
        ToRunOnCleanup.Add(() => unsubscription(funcToBind));
    }

    // Add a task to the maid's list, the task
    // will be completed when Cleanup() is called
    public Action GiveTask(Action task)
    {
        ToRunOnCleanup.Add(task);

        return task;
    }

    // Completes each task the maid has, call in destructors
    public void Cleanup()
    {
        foreach (Action task in ToRunOnCleanup)
        {
            task();
        }
    }
}