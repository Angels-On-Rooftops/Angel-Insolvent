using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Maid is a library that makes coding destruction of objects easier
// You don't need to worry about it for Design 4104

public class Maid
{
    List<Action> ToRunOnCleanup = new();

    // add an event to the maid's list, the event will be
    // unsubscribed when Cleanup() is called
    //
    // C# events are not first class so this is the best we can do for now
    //
    // example usage:
    // maid.GiveEvent(MyObject, "EventInMyObject", () => Debug.Log("Event in MyObject ran!"));
    //
    // Which is binds it like MyObject.EventInMyObject += () => Debug.Log("Event in MyObject ran!);
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

    // events are not first class, so we have to index it through the Reflection api
    public void GiveEvent<T>(T eventHolder, string eventName, Action funcToBind)
    {
        GiveEventDelegate(eventHolder, eventName, funcToBind);
    }

    public Coroutine GiveCoroutine(MonoBehaviour hasCoroutine, Coroutine coroutineRunning)
    {
        GiveTask(() =>
        {
            if (coroutineRunning is not null)
            {
                hasCoroutine.StopCoroutine(coroutineRunning);
            }
        });

        return coroutineRunning;
    }

    private Action GetFinalizer<T>(T task)
    {
        return task switch
        {
            Action action => action,
            GameObject gameObject => () => UnityEngine.Object.Destroy(gameObject),
            Maid maid => () => maid.Cleanup(),
            Coroutine coroutine => throw new NotSupportedException(
                "Use Maid.GiveCouroutine to clean up a coroutine. "
                    + "StopCoroutine needs a reference to the MonoBehaviour."
            ),
            _ => throw new NotImplementedException(
                $"Maid is unable to cleanup object of type {task.GetType()}"
            ),
        };
    }

    private T ProcessTask<T>(T task)
    {
        ToRunOnCleanup.Add(GetFinalizer(task));
        return task;
    }

    public T GiveTask<T>(T task)
    {
        return ProcessTask(task);
    }

    // just here so that c# can infer that () => {} is an action
    public Action GiveTask(Action task)
    {
        return ProcessTask(task);
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

    #region extra handlers for events with parameters
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

    public void GiveEvent<T, A, B, C, D>(
        T eventHolder,
        string eventName,
        Action<A, B, C, D> funcToBind
    )
    {
        GiveEventDelegate(eventHolder, eventName, funcToBind);
    }
    #endregion
}
