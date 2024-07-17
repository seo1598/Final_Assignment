using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Patrol : MonoBehaviour
{
    // ���� ���� �迭
    public Transform[] patrolPoints;
    // �� ȸ�� �Ÿ�
    public float wallAvoidanceDistance = 1.0f;
    // �� ���̾� ����ũ
    public LayerMask wallLayer;
    // �÷��̾� ����
    public Transform player;
    // ���� �Ÿ�
    public float chaseDistance = 10.0f;
    // �÷��̾ �Ҿ��� �������� �ð�
    public float losePlayerTime = 5.0f;
    // �þ߰�
    public float fieldOfViewAngle = 45.0f;
    // ���� �ؽ�Ʈ
    public TextMeshPro statusText;
    // ��� ���� ���� �ð�
    public float alertDuration = 10.0f;
    // ���ӿ��� ĵ���� ����
    public GameObject gameOverCanvas;

    // ���� ���� ���� �ε���
    private int currentPointIndex;
    // NavMeshAgent ������Ʈ
    private NavMeshAgent agent;
    // ���������� �÷��̾ �� ������ �ð�
    private float timeSinceLastSeenPlayer;
    // ��� ���� ����
    private bool isAlerted;
    // ��� ��ġ
    private Vector3 alertPosition;

    // ���� �� ȣ��Ǵ� �޼���
    void Start()
    {
        // NavMeshAgent ������Ʈ ��������
        agent = GetComponent<NavMeshAgent>();
        // ���� ������ �ִ� ���
        if (patrolPoints.Length > 0)
        {
            // �ʱ� ���� ���� �ε��� ����
            currentPointIndex = 0;
            // ���� ������ ����
            SetNextDestination();
            // ���� �ؽ�Ʈ ������Ʈ
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    // �� �����Ӹ��� ȣ��Ǵ� �޼���
    void Update()
    {
        // ��� ������ ���
        if (isAlerted)
        {
            AlertedBehavior();
        }
        else
        {
            PatrolBehavior();
        }
    }

    // ���� ���¿����� ����
    void PatrolBehavior()
    {
        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // �÷��̾� ���� ���
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // �þ߰� ���� �ִ��� Ȯ��
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // �÷��̾ ���� �Ÿ� ���� �ְ� �þ߰� ���� �ִ� ���
        if (distanceToPlayer <= chaseDistance && angleToPlayer <= fieldOfViewAngle / 2)
        {
            if (CanSeePlayer())
            {
                // �÷��̾ ����
                agent.SetDestination(player.position);
                UpdateStatusText("Player Spotted", Color.red);
            }
        }
        // ���� ������ ������ ���
        else if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            // ���� ���� �������� �̵�
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            SetNextDestination();
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    // ��� ���¿����� ����
    void AlertedBehavior()
    {
        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // �÷��̾� ���� ���
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // �þ߰� ���� �ִ��� Ȯ��
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // �÷��̾ ���� �Ÿ� ���� �ְ� �þ߰� ���� �ִ� ���
        if (distanceToPlayer <= chaseDistance && angleToPlayer <= fieldOfViewAngle / 2)
        {
            if (CanSeePlayer())
            {
                // ��� ���� ����
                isAlerted = false;
                // �÷��̾ ����
                agent.SetDestination(player.position);
                UpdateStatusText("Player Spotted", Color.red);
                return;
            }
        }

        // ��� ��ġ�� ������ ���
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            StartCoroutine(AlertWait());
            isAlerted = false;
        }

        // ���� ������ ��θ� ã�� �� ���� ��� ��Ʈ�� ���� ����
        if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            isAlerted = false;
            SetNextDestination();
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    // ���� ���� ���� ����
    void SetNextDestination()
    {
        Vector3 destination = patrolPoints[currentPointIndex].position;
        Vector3 adjustedDestination = AdjustDestinationForWall(destination);
        agent.SetDestination(adjustedDestination);
    }

    // �������� �� ȸ�� �Ÿ���ŭ ����
    Vector3 AdjustDestinationForWall(Vector3 destination)
    {
        Vector3 directionToDestination = (destination - transform.position).normalized;
        RaycastHit hit;

        // �������� ���� ��ο� ���� �ִ��� Ȯ��
        if (Physics.Raycast(transform.position, directionToDestination, out hit, wallAvoidanceDistance, wallLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;
            destination = hitPoint + hitNormal * wallAvoidanceDistance;
        }

        return destination;
    }

    // �÷��̾ �� �� �ִ��� Ȯ��
    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // �÷��̾� �������� ����ĳ��Ʈ
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
        {
            if (hit.transform == player)
            {
                return true; // �÷��̾ ����
            }
        }

        return false; // �÷��̾ �� �ڿ� ����
    }

    // ���� �ؽ�Ʈ ������Ʈ
    void UpdateStatusText(string status, Color color)
    {
        if (statusText != null)
        {
            statusText.text = status;
            statusText.color = color;
        }
    }

    // ��� ��ġ ����
    public void SetAlertPosition(Vector3 position)
    {
        alertPosition = position;
        isAlerted = true;
        agent.SetDestination(alertPosition);
        UpdateStatusText("Alerted", Color.yellow);
    }

    // ��� ���¿��� ���� �ð� ���� ���
    IEnumerator AlertWait()
    {
        UpdateStatusText("Searching", Color.yellow);
        yield return new WaitForSeconds(alertDuration);
        SetNextDestination();
        UpdateStatusText("Patrolling", Color.blue);
    }

    // �÷��̾�� �浹�� ��� ���ӿ���
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameOver();
        }
    }

    // ���ӿ��� ó��
    void GameOver()
    {
        Time.timeScale = 0f; // ���� �Ͻ�����
        gameOverCanvas.SetActive(true); // ���ӿ��� ĵ���� Ȱ��ȭ
    }
}

