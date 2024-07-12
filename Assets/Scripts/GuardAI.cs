using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public Transform player;
    public float detectionRange = 10.0f;
    public float chaseDuration = 5.0f;
    public float moveSpeed = 3.5f;

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private float lastPlayerSightingTime;
    private bool isChasing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (isChasing)
        {
            // 플레이어 추적
            agent.SetDestination(player.position);
            if (Vector3.Distance(transform.position, player.position) > detectionRange)
            {
                // 플레이어가 추적 범위를 벗어난 경우
                if (Time.time - lastPlayerSightingTime > chaseDuration)
                {
                    // 일정 시간 동안 플레이어가 범위를 벗어난 경우 패트롤로 돌아가기
                    isChasing = false;
                    GoToNextPatrolPoint();
                }
            }
            else
            {
                // 플레이어가 추적 범위 내에 있는 경우
                lastPlayerSightingTime = Time.time;
            }
        }
        else
        {
            // 패트롤 중
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextPatrolPoint();
            }

            // 플레이어 감지
            if (Vector3.Distance(transform.position, player.position) <= detectionRange)
            {
                isChasing = true;
                lastPlayerSightingTime = Time.time;
            }
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }
}
