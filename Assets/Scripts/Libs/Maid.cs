using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Maid
{
    List<Action> ToRunOnCleanup = new();

    // add an event to the maid's list, the event will be
    // unsubscribed when Cleanup() is called
    //
    // C# events are not first class so this is the best we can do for now
    //
    // example usage:
    // maid.GiveEvent(MyClass, "EventInMyClass", () => Debug.Log("Event in MyClass ran!")); 
    private void GiveEventDelegate<T>(T eventHolder, string eventName, Delegate funcToBind)
    {
        Assert.IsTrue(
            eventHolder.GetType().GetEvent(eventName) is not null,
            $"{eventName} is not an event in {eventHolder.GetType()}"
        );

        Assert.IsTrue(
            eventHolder.GetType().GetEvent(eventName).EventHandlerType.Equals(funcToBind.GetType()), 
            $"Event {eventName} is not compatible with type {funcToBind.GetType()}"
        );

        eventHolder.GetType().GetEvent(eventName).AddEventHandler(eventHolder, funcToBind);

        // pass to the maid to unbind the event when cleaning
        ToRunOnCleanup.Add(() =>
        {
            eventHolder.GetType().GetEvent(eventName).RemoveEventHandler(eventHolder, funcToBind);
        });
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

        ToRunOnCleanup = new();
    }
}