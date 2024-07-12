using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WallDetection : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float searchRadius = 10.0f;
    public float moveSpeed = 3.0f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // ���� �ν��� ���
                Debug.Log("�� �ν�!");

                // �ֺ� �� ���� Ž��
                Vector3 escapeDirection = FindEscapeDirection();
                if (escapeDirection != Vector3.zero)
                {
                    // �� �������� �̵�
                    agent.SetDestination(transform.position + escapeDirection);
                }
                else
                {
                    Debug.Log("�� ������ ã�� ���߽��ϴ�.");
                }
            }
        }
    }

    Vector3 FindEscapeDirection()
    {
        // Ž�� �ݰ� ������ ���� �������� ������ �����Ͽ� �� ���� Ž��
        float angleIncrement = 10f; // 10���� ȸ���ϸ� Ž��
        for (float angle = 0; angle < 360; angle += angleIncrement)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            RaycastHit hit;
            if (!Physics.SphereCast(transform.position, 0.5f, direction, out hit, searchRadius))
            {
                // �� ������ ã��
                Debug.Log("�� ���� ã��! ����: " + direction);
                return direction * moveSpeed;
            }
        }

        // �� ������ ã�� ����
        return Vector3.zero;
    }
}
