using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public string keyId; // ���� ID

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddKey(keyId);
                playerController.ShowMessage(keyId + " key collected!");
                Destroy(gameObject); // Ű ������Ʈ �ı�
            }
        }
    }
}
