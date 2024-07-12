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
                // 벽을 인식한 경우
                Debug.Log("벽 인식!");

                // 주변 빈 공간 탐색
                Vector3 escapeDirection = FindEscapeDirection();
                if (escapeDirection != Vector3.zero)
                {
                    // 빈 공간으로 이동
                    agent.SetDestination(transform.position + escapeDirection);
                }
                else
                {
                    Debug.Log("빈 공간을 찾지 못했습니다.");
                }
            }
        }
    }

    Vector3 FindEscapeDirection()
    {
        // 탐색 반경 내에서 일정 간격으로 방향을 설정하여 빈 공간 탐색
        float angleIncrement = 10f; // 10도씩 회전하며 탐색
        for (float angle = 0; angle < 360; angle += angleIncrement)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            RaycastHit hit;
            if (!Physics.SphereCast(transform.position, 0.5f, direction, out hit, searchRadius))
            {
                // 빈 공간을 찾음
                Debug.Log("빈 공간 찾음! 방향: " + direction);
                return direction * moveSpeed;
            }
        }

        // 빈 공간을 찾지 못함
        return Vector3.zero;
    }
}
