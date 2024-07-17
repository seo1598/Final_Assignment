using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // 문을 열기 위해 필요한 열쇠 ID
    public string requiredKeyId;
    // 문 오브젝트 참조
    public GameObject door;

    // Collider가 다른 Collider와 충돌할 때 호출되는 Unity 이벤트 메서드
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 충돌한 객체에서 PlayerController 컴포넌트를 가져옴
            PlayerController playerController = other.GetComponent<PlayerController>();
            // PlayerController가 해당 열쇠를 가지고 있는지 확인
            if (playerController.HasKey(requiredKeyId))
            {
                // 열쇠가 있으면 문을 엶
                OpenDoor();
            }
            else
            {
                // 열쇠가 없으면 "Wrong KEY." 메시지를 표시
                playerController.ShowMessage("Wrong KEY.");
            }
        }
    }

    // 문을 여는 메서드
    private void OpenDoor()
    {
        // 현재 문 오브젝트를 비활성화
        gameObject.SetActive(false);
        // door 오브젝트도 비활성화
        door.SetActive(false);
        // 콘솔에 문이 열렸다는 메시지 출력
        Debug.Log("Door is opened!");
    }
}
