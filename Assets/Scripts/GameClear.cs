using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClear : MonoBehaviour
{
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
                // PlayerController의 GameClear 메서드 호출
                playerController.GameClear();
            }
        }
    }
}
