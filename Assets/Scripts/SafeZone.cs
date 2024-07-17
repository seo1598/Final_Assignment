using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    // Collider�� �ٸ� Collider�� �浹�� �� ȣ��Ǵ� Unity �̺�Ʈ �޼���
    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �浹�� ��ü���� PlayerController ������Ʈ�� ������ SafeZone�� ���Դٰ� ����
            other.GetComponent<PlayerController>().SetInSafeZone(true);
        }
    }

    // Collider�� �ٸ� Collider�� �浹�� ���� �� ȣ��Ǵ� Unity �̺�Ʈ �޼���
    private void OnTriggerExit(Collider other)
    {
        // �浹�� ���� ��ü�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �浹�� ��ü���� PlayerController ������Ʈ�� ������ SafeZone���� �����ٰ� ����
            other.GetComponent<PlayerController>().SetInSafeZone(false);
        }
    }
}
