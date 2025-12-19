using UnityEngine;

namespace Choi
{
    public abstract class PickupItem : MonoBehaviour
    {
        #region Variables
        [SerializeField] private MonoBehaviour stateProvider;
        [SerializeField] private GameManager gameManager;
        protected GameManager GameManager => gameManager;

        private IGameStateProvider gameState;
        private bool isConsumed;
        #endregion

        #region Unity Event Method
        protected virtual void Awake()
        {
            gameState = stateProvider as IGameStateProvider;

            if (gameState == null)
            {
                Debug.LogError(
                    $"{name} : stateProvider does not implement IGameStateProvider",
                    this
                );
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isConsumed)
                return;

            if (gameState != null &&
                gameState.CurrentState != GameState.Playing)
                return;

            Player player = collision.GetComponent<Player>();
            if (player == null)
                return;

            if (PickUp(player))
            {
                isConsumed = true;
                Destroy(gameObject);
            }
        }
        #endregion

        #region custom Method
        protected abstract bool PickUp(Player player);
        #endregion
    }
}