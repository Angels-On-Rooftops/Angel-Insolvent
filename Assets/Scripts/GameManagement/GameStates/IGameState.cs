using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStateManagement
{
    public interface IGameState
    {
        void EnterState(IGameState previousState);
        void ExitState();
    }
}
