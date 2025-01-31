using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    // 열쇠의 ID를 나타내는 변수
    public string keyId;

    // Collider가 다른 Collider와 충돌할 때 호출되는 Unity 이벤트 메서드
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 충돌한 객체에서 PlayerController 컴포넌트를 가져옴
            PlayerController playerController = other.GetComponent<PlayerController>();

            // PlayerController 컴포넌트가 존재하는지 확인
            if (playerController != null)
            {
                // 플레이어에게 열쇠를 추가
                playerController.AddKey(keyId);

                // 플레이어에게 열쇠를 획득했다는 메시지를 표시
                playerController.ShowMessage(keyId + " key collected!");

                // 현재 키 오브젝트를 파괴
                Destroy(gameObject);
            }
        }
    }
}
