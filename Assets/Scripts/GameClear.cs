using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    // Collider�� �ٸ� Collider�� �浹�� �� ȣ��Ǵ� Unity �̺�Ʈ �޼���
    void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �浹�� ��ü���� PlayerController ������Ʈ�� ������
            PlayerController playerController = other.GetComponent<PlayerController>();

            // PlayerController ������Ʈ�� �����ϴ��� Ȯ��
            if (playerController != null)
            {
                // PlayerController�� GameClear �޼��� ȣ��
                playerController.GameClear();
            }
        }
    }
}
