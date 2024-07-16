using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public string keyId; // ¿­¼è ID

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().AddKey(keyId);
            Destroy(gameObject); // ¿­¼è ¿ÀºêÁ§Æ® ÆÄ±«
        }
    }
}
