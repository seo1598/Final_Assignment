using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // ���� ���� ���� �ʿ��� ���� ID
    public string requiredKeyId;
    // �� ������Ʈ ����
    public GameObject door;

    // Collider�� �ٸ� Collider�� �浹�� �� ȣ��Ǵ� Unity �̺�Ʈ �޼���
    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �浹�� ��ü���� PlayerController ������Ʈ�� ������
            PlayerController playerController = other.GetComponent<PlayerController>();
            // PlayerController�� �ش� ���踦 ������ �ִ��� Ȯ��
            if (playerController.HasKey(requiredKeyId))
            {
                // ���谡 ������ ���� ��
                OpenDoor();
            }
            else
            {
                // ���谡 ������ "Wrong KEY." �޽����� ǥ��
                playerController.ShowMessage("Wrong KEY.");
            }
        }
    }

    // ���� ���� �޼���
    private void OpenDoor()
    {
        // ���� �� ������Ʈ�� ��Ȱ��ȭ
        gameObject.SetActive(false);
        // door ������Ʈ�� ��Ȱ��ȭ
        door.SetActive(false);
        // �ֿܼ� ���� ���ȴٴ� �޽��� ���
        Debug.Log("Door is opened!");
    }
}
