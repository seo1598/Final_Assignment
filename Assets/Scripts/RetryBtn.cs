using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryBtn : MonoBehaviour
{
    public GameObject startmenu;

    public void RetryGame()
    {
        Time.timeScale = 1f; // 게임 다시 시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        startmenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
