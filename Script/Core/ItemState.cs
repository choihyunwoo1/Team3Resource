using UnityEngine;

namespace Choi
{
    public interface IGameStateProvider
    {
        GameState CurrentState { get; }
    }
}