using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryBtn : MonoBehaviour
{
    // ���� �޴� UI�� �����ϴ� ����
    public GameObject startmenu;

    // ������ �ٽ� �����ϴ� �޼���
    public void RetryGame()
    {
        // ������ �ٽ� ����
        Time.timeScale = 1f;
        // ���� ���� �ٽ� �ε�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ������ �����ϴ� �޼���
    public void StartGame()
    {
        // ���� �ð��� �ٽ� ���� �ӵ��� ����
        Time.timeScale = 1f;
        // ���� �޴� UI ��Ȱ��ȭ
        startmenu.SetActive(false);
        // Ŀ�� ��� ���·� ����
        Cursor.lockState = CursorLockMode.Locked;
        // Ŀ���� ������ �ʰ� ����
        Cursor.visible = false;
    }

    // ���� �� ȣ��Ǵ� �޼���
    void Start()
    {
        // ������ �Ͻ����� ���·� ����
        Time.timeScale = 0f;
        // Ŀ�� ��� ����
        Cursor.lockState = CursorLockMode.None;
        // Ŀ���� ���̰� ����
        Cursor.visible = true;
    }
}
