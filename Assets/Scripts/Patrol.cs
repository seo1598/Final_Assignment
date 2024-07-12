using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Patrol : MonoBehaviour
{
    public Transform[] patrolPoints; // ���� ������� �̵��� ��Ʈ�� ����Ʈ�� �迭
    public float wallAvoidanceDistance = 1.0f; // ���� ������ �Ÿ�
    public LayerMask wallLayer; // ���� ������ ���̾�
    public Transform player; // �÷��̾� �߰� ����
    public float chaseDistance = 10.0f; //�÷��̾ �߰��� �ִ� �Ÿ�
    public float losePlayerTime = 5.0f; // �÷��̾ �Ҿ���� �� ��Ʈ�ѷ� ���ư�������� �ð�
    public float fieldOfViewAngle = 45.0f; // ���� �þ߰�
    public TextMeshPro statusText; // TextMeshPro�� ����� �ؽ�Ʈ

    private int currentPointIndex; // ���� ��Ʈ�� ����Ʈ�� �ε���
    private NavMeshAgent agent; 
    private bool isChasingPlayer; // �÷��̾ �߰� ������ ����
    private float timeSinceLastSeenPlayer; // ���������� �÷��̾ �� �� ��� �ð�

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0) // ��Ʈ�� ����Ʈ�� ���� �̵�, ��Ʈ�� �� �Ķ��� �۾�ü�� ǥ��
        {
            currentPointIndex = 0; 
            SetNextDestination();
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    void Update()
    {
        //�÷��̾���� �Ÿ��� ����ϰ�, �÷��̾ �þ߰� ���� �ִ��� Ȯ��
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (distanceToPlayer <= chaseDistance && angleToPlayer <= fieldOfViewAngle / 2)
        {
            if (CanSeePlayer()) // �÷��̾ �� �ڿ� �ִ��� Ȯ��
            {
                isChasingPlayer = true; // �÷��̾ �����Ǹ� �߰� ���� ��ȯ
                timeSinceLastSeenPlayer = 0f;
                agent.SetDestination(player.position);
                UpdateStatusText("Player Spotted", Color.red); // �÷��̾� �߰� �޼����� ���������� ǥ��
            }
        }
        else if (isChasingPlayer)
        {
            timeSinceLastSeenPlayer += Time.deltaTime;

            if (timeSinceLastSeenPlayer >= losePlayerTime)
            {
                isChasingPlayer = false;
                SetNextDestination();
                UpdateStatusText("Patrolling", Color.blue);
            }
        }

        if (!isChasingPlayer && agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            SetNextDestination();
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    void SetNextDestination()
    {
        Vector3 destination = patrolPoints[currentPointIndex].position; // ���� ��Ʈ�� ����Ʈ�� ��ġ�� ������
        Vector3 adjustedDestination = AdjustDestinationForWall(destination);  // ���� ȸ���� �������� ����
        agent.SetDestination(adjustedDestination);
       
    }

    Vector3 AdjustDestinationForWall(Vector3 destination)
    {
        Vector3 directionToDestination = (destination - transform.position).normalized;
        RaycastHit hit;
        // �������� ���� ������ ����ϰ�, Raycast�� ����Ͽ� ���� ����

        if (Physics.Raycast(transform.position, directionToDestination, out hit, wallAvoidanceDistance, wallLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;
            destination = hitPoint + hitNormal * wallAvoidanceDistance;
            // ���� �����Ǹ� ������ �Ÿ��� �����ϵ��� �������� ����
        }

        return destination;
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // Raycast�� ����Ͽ� ���� �÷��̾ �� �� �ִ��� Ȯ��

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
        { 
            if (hit.transform == player)
            {
                return true; //  ���� ��ġ���� �÷��̾� �������� Raycast�� ���, Raycast�� �÷��̾ ������ true�� ��ȯ
            }
        }

        return false; // Raycast�� �÷��̾ ���� ������ false�� ��ȯ
    }

    void UpdateStatusText(string status, Color color) // ���� �ؽ�Ʈ�� ������Ʈ
    {
        if (statusText != null)
        {
            statusText.text = status;
            statusText.color = color;
        }
    }
}

