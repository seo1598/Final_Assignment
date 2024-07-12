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
            // �÷��̾� ����
            agent.SetDestination(player.position);
            if (Vector3.Distance(transform.position, player.position) > detectionRange)
            {
                // �÷��̾ ���� ������ ��� ���
                if (Time.time - lastPlayerSightingTime > chaseDuration)
                {
                    // ���� �ð� ���� �÷��̾ ������ ��� ��� ��Ʈ�ѷ� ���ư���
                    isChasing = false;
                    GoToNextPatrolPoint();
                }
            }
            else
            {
                // �÷��̾ ���� ���� ���� �ִ� ���
                lastPlayerSightingTime = Time.time;
            }
        }
        else
        {
            // ��Ʈ�� ��
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextPatrolPoint();
            }

            // �÷��̾� ����
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
