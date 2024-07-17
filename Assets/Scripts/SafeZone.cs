using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    // Collider가 다른 Collider와 충돌할 때 호출되는 Unity 이벤트 메서드
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 충돌한 객체에서 PlayerController 컴포넌트를 가져와 SafeZone에 들어왔다고 설정
            other.GetComponent<PlayerController>().SetInSafeZone(true);
        }
    }

    // Collider가 다른 Collider와 충돌이 끝날 때 호출되는 Unity 이벤트 메서드
    private void OnTriggerExit(Collider other)
    {
        // 충돌이 끝난 객체가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 충돌한 객체에서 PlayerController 컴포넌트를 가져와 SafeZone에서 나갔다고 설정
            other.GetComponent<PlayerController>().SetInSafeZone(false);
        }
    }
}
