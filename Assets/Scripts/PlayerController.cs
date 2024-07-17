using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private HashSet<string> keys = new HashSet<string>();
    public Image bluekeyUIImage; // 파랑 키 UI 이미지
    public Image redkeyUIImage; // 레드 키 UI 이미지
    public TMP_Text messageText; // 메시지 텍스트
    private bool isInSafeZone = false; // SafeZone 여부를 나타내는 변수

    void Start()
    {
        bluekeyUIImage.enabled = false; // 시작 시 파랑 키 UI 숨김
        redkeyUIImage.enabled = false; // 시작 시 레드 키 UI 숨김
        messageText.enabled = false; // 시작 시 메시지 텍스트 숨김
    }

    public void AddKey(string keyId)
    {
        keys.Add(keyId);
        UpdateKeyUI(keyId);
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

    private void UpdateKeyUI(string keyId)
    {
        if (keyId == "blue")
        {
            bluekeyUIImage.enabled = true;
        }
        else if (keyId == "red")
        {
            redkeyUIImage.enabled = true;
        }
    }
}
