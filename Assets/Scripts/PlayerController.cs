using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // �÷��̾ ������ ������� �����ϴ� HashSet
    private HashSet<string> keys = new HashSet<string>();
    // �Ķ� Ű UI �̹���
    public Image bluekeyUIImage;
    // ���� Ű UI �̹���
    public Image redkeyUIImage;
    // �޽��� �ؽ�Ʈ
    public TMP_Text messageText;
    // ���״ٴ� ���� �ؽ�Ʈ
    public TMP_Text alertText;
    // ���� Ŭ���� ĵ���� ����
    public GameObject gameClearCanvas;
    // SafeZone ���θ� ��Ÿ���� ����
    private bool isInSafeZone = false;

    // ���� �� ȣ��Ǵ� �޼���
    void Start()
    {
        // ���� �� �Ķ� Ű UI ����
        bluekeyUIImage.enabled = false;
        // ���� �� ���� Ű UI ����
        redkeyUIImage.enabled = false;
        // ���� �� �޽��� �ؽ�Ʈ ����
        messageText.enabled = false;
        // ���� �� ���״ٴ� ���� �ؽ�Ʈ ����
        alertText.enabled = false;
        // ���� �� ���� Ŭ���� UI ����
        gameClearCanvas.SetActive(false);
    }

    // ���踦 �߰��ϴ� �޼���
    public void AddKey(string keyId)
    {
        // ���� ID�� HashSet�� �߰�
        keys.Add(keyId);
        // Ű UI ������Ʈ
        UpdateKeyUI(keyId);
    }

    // Ư�� ���踦 ������ �ִ��� Ȯ���ϴ� �޼���
    public bool HasKey(string keyId)
    {
        return keys.Contains(keyId);
    }

    // SafeZone ���θ� �����ϴ� �޼���
    public void SetInSafeZone(bool inSafeZone)
    {
        isInSafeZone = inSafeZone;
    }

    // SafeZone ���θ� ��ȯ�ϴ� �޼���
    public bool IsInSafeZone()
    {
        return isInSafeZone;
    }

    // �޽����� ǥ���ϴ� �޼���
    public void ShowMessage(string message)
    {
        // �ڷ�ƾ�� �����Ͽ� �޽����� ǥ��
        StartCoroutine(ShowMessageCoroutine(message));
    }

    // �޽����� ���� �ð� ���� ǥ���ϴ� �ڷ�ƾ
    private IEnumerator ShowMessageCoroutine(string message)
    {
        // �޽��� �ؽ�Ʈ ����
        messageText.text = message;
        // �޽��� �ؽ�Ʈ ǥ��
        messageText.enabled = true;
        // 2�� ���� ���
        yield return new WaitForSeconds(2f);
        // �޽��� �ؽ�Ʈ ����
        messageText.enabled = false;
    }

    // Ű UI�� ������Ʈ�ϴ� �޼���
    private void UpdateKeyUI(string keyId)
    {
        // �Ķ� Ű�� ������ ��� �Ķ� Ű UI ǥ��
        if (keyId == "blue")
        {
            bluekeyUIImage.enabled = true;
        }
        // ���� Ű�� ������ ��� ���� Ű UI ǥ��
        else if (keyId == "red")
        {
            redkeyUIImage.enabled = true;
        }
    }

    // ��� �޽����� ǥ���ϴ� �޼���
    public void ShowAlert()
    {
        // �ڷ�ƾ�� �����Ͽ� ��� �޽����� ǥ��
        StartCoroutine(ShowAlertCoroutine());
    }

    // ��� �޽����� ���� �ð� ���� ǥ���ϴ� �ڷ�ƾ
    private IEnumerator ShowAlertCoroutine()
    {
        // ��� �ؽ�Ʈ ǥ��
        alertText.enabled = true;
        // 2�� ���� ���
        yield return new WaitForSeconds(2f);
        // ��� �ؽ�Ʈ ����
        alertText.enabled = false;
    }

    // ���� Ŭ���� �� ȣ��Ǵ� �޼���
    public void GameClear()
    {
        // ���� �Ͻ�����
        Time.timeScale = 0f;
        // ���� Ŭ���� UI Ȱ��ȭ
        gameClearCanvas.SetActive(true);
        // Ŀ�� ��� ����
        Cursor.lockState = CursorLockMode.None;
        // Ŀ�� ���̱�
        Cursor.visible = true;
    }
}
