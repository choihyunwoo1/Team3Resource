using UnityEngine;

namespace JS
{
    public interface IGameStateProvider
    {
        GameState CurrentState { get; }
    }
}