using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string requiredKeyId; // 필요한 열쇠 ID

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController.HasKey(requiredKeyId))
            {
                OpenDoor();
            }
            else
            {
                playerController.ShowMessage("Wrong KEY.");
            }
        }
    }

    private void OpenDoor()
    {
        gameObject.SetActive(false); // 문 오브젝트 비활성화
        Debug.Log("Door is opened!");
    }
}
