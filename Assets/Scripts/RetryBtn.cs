using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryBtn : MonoBehaviour
{
    // 시작 메뉴 UI를 참조하는 변수
    public GameObject startmenu;

    // 게임을 다시 시작하는 메서드
    public void RetryGame()
    {
        // 게임을 다시 시작
        Time.timeScale = 1f;
        // 현재 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 게임을 시작하는 메서드
    public void StartGame()
    {
        // 게임 시간을 다시 정상 속도로 설정
        Time.timeScale = 1f;
        // 시작 메뉴 UI 비활성화
        startmenu.SetActive(false);
        // 커서 잠금 상태로 설정
        Cursor.lockState = CursorLockMode.Locked;
        // 커서를 보이지 않게 설정
        Cursor.visible = false;
    }

    // 시작 시 호출되는 메서드
    void Start()
    {
        // 게임을 일시정지 상태로 설정
        Time.timeScale = 0f;
        // 커서 잠금 해제
        Cursor.lockState = CursorLockMode.None;
        // 커서를 보이게 설정
        Cursor.visible = true;
    }
}
