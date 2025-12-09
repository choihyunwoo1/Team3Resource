using UnityEngine;

namespace MyTeam
{
    /// <summary>
    /// 적이 플레이를 쫓아가는 스크립트
    /// </summary>
    public class EnumyController : MonoBehaviour
    {
        #region Variables
        public float moveSpeed = 5f;
        private Transform playerTransform;  //플레이어의 Transform
        #endregion

        #region Unity Event Methods
        private void Start()
        {
            //씬에서 "Player" 태그를 가진 게임 오브젝트를 찾아 플레이어의 Transform을 가져옴
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        private void Update()
        {
            if (playerTransform != null)
            { 
                // 1. 플레이어 위치를 향하는 방향 백터 계산
                Vector3 direction = playerTransform.position - transform.position;

                // 2. 방향 백터를 정규화
                Vector3 normalizedDirection = direction.normalized;

                //3. 현재 위치를 업데이트
                // Time.deltaTime을 이전 프레임 이후 경과된 시간
                transform.position += normalizedDirection * moveSpeed * Time.deltaTime;
            }
        }
        #endregion
    }
}
