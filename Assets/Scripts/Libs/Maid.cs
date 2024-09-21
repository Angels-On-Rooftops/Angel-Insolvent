using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    //public void GiveEvent<T>(T funcToBind, Action<T> subscription, Action<T> unsubscription)
    //{
    //    subscription(funcToBind);
    //    ToRunOnCleanup.Add(() => unsubscription(funcToBind));
    //}
    private void GiveEventDelegate<T>(T eventHolder, string eventName, Delegate funcToBind)
    {
        Assert.IsTrue(
            eventHolder.GetType().GetEvent(eventName) is not null,
            $"{eventName} is not an event in {eventHolder.GetType()}"
        );

        Assert.IsTrue(
            eventHolder.GetType().GetEvent(eventName).EventHandlerType.Equals(funcToBind.GetType()), 
            $"Event {eventName} can not be bound to action of type {funcToBind.GetType()}"
        );

        eventHolder.GetType().GetEvent(eventName).AddEventHandler(eventHolder, funcToBind);
    }

    public void GiveEvent<T>(T eventHolder, string eventName, Action funcToBind)
    {
        GiveEventDelegate(eventHolder, eventName, funcToBind);
    }

    public void GiveEvent<T, A>(T eventHolder, string eventName, Action<A> funcToBind)
    {
        GiveEventDelegate(eventHolder, eventName, funcToBind);
    }

    public void GiveEvent<T, A, B>(T eventHolder, string eventName, Action<A, B> funcToBind)
    {
        GiveEventDelegate(eventHolder, eventName, funcToBind);
    }

    public void GiveEvent<T, A, B, C>(T eventHolder, string eventName, Action<A, B, C> funcToBind)
    {
        GiveEventDelegate(eventHolder, eventName, funcToBind);
    }

    public void GiveEvent<T, A, B, C, D>(T eventHolder, string eventName, Action<A, B, C, D> funcToBind)
    {
        GiveEventDelegate(eventHolder, eventName, funcToBind);
    }

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