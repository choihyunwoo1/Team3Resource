using UnityEngine;
using UnityEngine.InputSystem;

namespace My2DGame
{
    /// <summary>
    /// 플레이어를 제어하는 클래스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        //참조
        private Rigidbody2D rd2D;
        //이동
        [SerializeField]
        private float walkSpeed = 3f;       //걷는 속도
        [SerializeField]
        private float runSpeed = 8f;
        [SerializeField]
        private float airSpeed = 2f;

        public float jumpForce = 2f;

        #endregion

        #region Property

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            rd2D = GetComponent<Rigidbody2D>();
        }
        private void Update()
        {

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                this.gameObject.transform.position += Vector3.left * walkSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                this.gameObject.transform.position += Vector3.right * walkSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                this.gameObject.transform.position += Vector3.up * jumpForce * Time.deltaTime;
            }

        }

        #endregion

        #region Custom Method

        #endregion
    }
}