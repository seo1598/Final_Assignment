using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Patrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float wallAvoidanceDistance = 1.0f;
    public LayerMask wallLayer;
    public Transform player;
    public float chaseDistance = 10.0f;
    public float losePlayerTime = 5.0f;
    public float fieldOfViewAngle = 45.0f;
    public TextMeshPro statusText;
    public float alertDuration = 10.0f;

    private int currentPointIndex;
    private NavMeshAgent agent;
    private bool isChasingPlayer;
    private float timeSinceLastSeenPlayer;
    private bool isAlerted;
    private Vector3 alertPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            currentPointIndex = 0;
            SetNextDestination();
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    void Update()
    {
        if (isAlerted)
        {
            if (agent.remainingDistance < 0.5f && !agent.pathPending)
            {
                StartCoroutine(AlertWait());
                isAlerted = false;
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (distanceToPlayer <= chaseDistance && angleToPlayer <= fieldOfViewAngle / 2 && !player.GetComponent<PlayerController>().IsInSafeZone())
        {
            if (CanSeePlayer())
            {
                isChasingPlayer = true;
                timeSinceLastSeenPlayer = 0f;
                agent.SetDestination(player.position);
                UpdateStatusText("Player Spotted", Color.red);
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
        Vector3 destination = patrolPoints[currentPointIndex].position;
        Vector3 adjustedDestination = AdjustDestinationForWall(destination);
        agent.SetDestination(adjustedDestination);
    }

    Vector3 AdjustDestinationForWall(Vector3 destination)
    {
        Vector3 directionToDestination = (destination - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, directionToDestination, out hit, wallAvoidanceDistance, wallLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;
            destination = hitPoint + hitNormal * wallAvoidanceDistance;
        }

        return destination;
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
        {
            if (hit.transform == player)
            {
                return true; // 플레이어가 보임
            }
        }

        return false; // 플레이어가 벽 뒤에 있음
    }

    void UpdateStatusText(string status, Color color)
    {
        if (statusText != null)
        {
            statusText.text = status;
            statusText.color = color;
        }
    }

    public void SetAlertPosition(Vector3 position)
    {
        alertPosition = position;
        isAlerted = true;
        agent.SetDestination(alertPosition);
        UpdateStatusText("Alerted", Color.yellow);
    }

    IEnumerator AlertWait()
    {
        UpdateStatusText("Searching", Color.yellow);
        yield return new WaitForSeconds(alertDuration);
        SetNextDestination();
        UpdateStatusText("Patrolling", Color.blue);
    }
}

