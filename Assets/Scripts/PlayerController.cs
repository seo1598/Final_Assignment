using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private HashSet<string> keys = new HashSet<string>();
    public Image keyUIImage; // ���� UI �̹���
    public TMP_Text messageText; // �޽��� �ؽ�Ʈ
    private bool isInSafeZone = false; // SafeZone ���θ� ��Ÿ���� ����

    void Start()
    {
        keyUIImage.enabled = false; // ���� �� ���� UI ����
        messageText.enabled = false; // ���� �� �޽��� �ؽ�Ʈ ����
    }

    public void AddKey(string keyId)
    {
        keys.Add(keyId);
        keyUIImage.enabled = true; // ���� UI ǥ��
    }

    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }

    public void SetInSafeZone(bool inSafeZone)
    {
        isInSafeZone = inSafeZone;
    }

    public bool IsInSafeZone()
    {
        return isInSafeZone;
    }

    public void ShowMessage(string message)
    {
        StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        messageText.text = message;
        messageText.enabled = true;
        yield return new WaitForSeconds(2f);
        messageText.enabled = false;
    }
}
