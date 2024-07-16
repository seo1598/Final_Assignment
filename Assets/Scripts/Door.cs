using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string requiredKeyId; // �ʿ��� ���� ID

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
        gameObject.SetActive(false); // �� ������Ʈ ��Ȱ��ȭ
        Debug.Log("Door is opened!");
    }
}
