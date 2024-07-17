using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public string keyId; // ���� ID

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().AddKey(keyId);
            Destroy(gameObject); // ���� ������Ʈ �ı�
        }
    }
}
