using UnityEngine;

namespace Choi
{
    /// <summary>
    /// 하늘에서 펀치 공격하는 클래스
    /// </summary>
    public class Punch : MonoBehaviour
    {
        #region Variables
        //참조
        private Rigidbody2D rb2D;
        //하강 속도
        [SerializeField] private float fallSpeed = 15f;
        //
        [SerializeField] private float destroyTime = 4f;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            rb2D = GetComponent<Rigidbody2D>();
            // 중력 없이 일정한 속도로 떨어지게 설정
            rb2D.gravityScale = 0;
        }

        private void Start()
        {
            // 생성되자마자 아래로 발사
            rb2D.linearVelocity = Vector2.down * fallSpeed;

            // 화면 밖으로 나갈 때를 대비해 자동 삭제
            Destroy(gameObject, destroyTime);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Die(DeathCause.Enemy);
            }
            Destroy(gameObject); // 충돌 후 소멸
        }
        #endregion

        #region Custom Method

        #endregion
    }
}
