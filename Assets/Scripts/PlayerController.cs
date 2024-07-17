using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // 플레이어가 수집한 열쇠들을 저장하는 HashSet
    private HashSet<string> keys = new HashSet<string>();
    // 파랑 키 UI 이미지
    public Image bluekeyUIImage;
    // 레드 키 UI 이미지
    public Image redkeyUIImage;
    // 메시지 텍스트
    public TMP_Text messageText;
    // 들켰다는 문구 텍스트
    public TMP_Text alertText;
    // 게임 클리어 캔버스 참조
    public GameObject gameClearCanvas;
    // SafeZone 여부를 나타내는 변수
    private bool isInSafeZone = false;

    // 시작 시 호출되는 메서드
    void Start()
    {
        // 시작 시 파랑 키 UI 숨김
        bluekeyUIImage.enabled = false;
        // 시작 시 레드 키 UI 숨김
        redkeyUIImage.enabled = false;
        // 시작 시 메시지 텍스트 숨김
        messageText.enabled = false;
        // 시작 시 들켰다는 문구 텍스트 숨김
        alertText.enabled = false;
        // 시작 시 게임 클리어 UI 숨김
        gameClearCanvas.SetActive(false);
    }

    // 열쇠를 추가하는 메서드
    public void AddKey(string keyId)
    {
        // 열쇠 ID를 HashSet에 추가
        keys.Add(keyId);
        // 키 UI 업데이트
        UpdateKeyUI(keyId);
    }

    // 특정 열쇠를 가지고 있는지 확인하는 메서드
    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }

    // SafeZone 여부를 설정하는 메서드
    public void SetInSafeZone(bool inSafeZone)
    {
        isInSafeZone = inSafeZone;
    }

    // SafeZone 여부를 반환하는 메서드
    public bool IsInSafeZone()
    {
        return isInSafeZone;
    }

    // 메시지를 표시하는 메서드
    public void ShowMessage(string message)
    {
        // 코루틴을 시작하여 메시지를 표시
        StartCoroutine(ShowMessageCoroutine(message));
    }

    // 메시지를 일정 시간 동안 표시하는 코루틴
    private IEnumerator ShowMessageCoroutine(string message)
    {
        // 메시지 텍스트 설정
        messageText.text = message;
        // 메시지 텍스트 표시
        messageText.enabled = true;
        // 2초 동안 대기
        yield return new WaitForSeconds(2f);
        // 메시지 텍스트 숨김
        messageText.enabled = false;
    }

    // 키 UI를 업데이트하는 메서드
    private void UpdateKeyUI(string keyId)
    {
        // 파랑 키를 수집한 경우 파랑 키 UI 표시
        if (keyId == "blue")
        {
            bluekeyUIImage.enabled = true;
        }
        // 레드 키를 수집한 경우 레드 키 UI 표시
        else if (keyId == "red")
        {
            redkeyUIImage.enabled = true;
        }
    }

    // 경고 메시지를 표시하는 메서드
    public void ShowAlert()
    {
        // 코루틴을 시작하여 경고 메시지를 표시
        StartCoroutine(ShowAlertCoroutine());
    }

    // 경고 메시지를 일정 시간 동안 표시하는 코루틴
    private IEnumerator ShowAlertCoroutine()
    {
        // 경고 텍스트 표시
        alertText.enabled = true;
        // 2초 동안 대기
        yield return new WaitForSeconds(2f);
        // 경고 텍스트 숨김
        alertText.enabled = false;
    }

    // 게임 클리어 시 호출되는 메서드
    public void GameClear()
    {
        // 게임 일시정지
        Time.timeScale = 0f;
        // 게임 클리어 UI 활성화
        gameClearCanvas.SetActive(true);
        // 커서 잠금 해제
        Cursor.lockState = CursorLockMode.None;
        // 커서 보이기
        Cursor.visible = true;
    }
}
