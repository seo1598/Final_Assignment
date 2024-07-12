using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Patrol : MonoBehaviour
{
    public Transform[] patrolPoints; // 적이 순서대로 이동할 패트롤 포인트의 배열
    public float wallAvoidanceDistance = 1.0f; // 벽을 감지할 거리
    public LayerMask wallLayer; // 벽을 감지할 레이어
    public Transform player; // 플레이어 추격 지정
    public float chaseDistance = 10.0f; //플레이어를 추격할 최대 거리
    public float losePlayerTime = 5.0f; // 플레이어를 잃어버린 후 패트롤로 돌아가기까지의 시간
    public float fieldOfViewAngle = 45.0f; // 적의 시야각
    public TextMeshPro statusText; // TextMeshPro를 사용한 텍스트

    private int currentPointIndex; // 현재 패트롤 포인트의 인덱스
    private NavMeshAgent agent; 
    private bool isChasingPlayer; // 플레이어를 추격 중인지 여부
    private float timeSinceLastSeenPlayer; // 마지막으로 플레이어를 본 후 경과 시간

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0) // 패트롤 포인트에 따라서 이동, 패트롤 중 파란색 글씨체로 표시
        {
            currentPointIndex = 0; 
            SetNextDestination();
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    void Update()
    {
        //플레이어와의 거리를 계산하고, 플레이어가 시야각 내에 있는지 확인
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (distanceToPlayer <= chaseDistance && angleToPlayer <= fieldOfViewAngle / 2)
        {
            if (CanSeePlayer()) // 플레이어가 벽 뒤에 있는지 확인
            {
                isChasingPlayer = true; // 플레이어가 감지되면 추격 모드로 전환
                timeSinceLastSeenPlayer = 0f;
                agent.SetDestination(player.position);
                UpdateStatusText("Player Spotted", Color.red); // 플레이어 발견 메세지와 빨간색으로 표시
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
        Vector3 destination = patrolPoints[currentPointIndex].position; // 현재 패트롤 포인트의 위치를 가져옴
        Vector3 adjustedDestination = AdjustDestinationForWall(destination);  // 벽을 회피한 목적지를 설정
        agent.SetDestination(adjustedDestination);
       
    }

    Vector3 AdjustDestinationForWall(Vector3 destination)
    {
        Vector3 directionToDestination = (destination - transform.position).normalized;
        RaycastHit hit;
        // 목적지로 가는 방향을 계산하고, Raycast를 사용하여 벽을 감지

        if (Physics.Raycast(transform.position, directionToDestination, out hit, wallAvoidanceDistance, wallLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;
            destination = hitPoint + hitNormal * wallAvoidanceDistance;
            // 벽이 감지되면 벽과의 거리를 유지하도록 목적지를 조정
        }

        return destination;
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // Raycast를 사용하여 적이 플레이어를 볼 수 있는지 확인

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
        { 
            if (hit.transform == player)
            {
                return true; //  적의 위치에서 플레이어 방향으로 Raycast를 쏘고, Raycast가 플레이어에 맞으면 true를 반환
            }
        }

        return false; // Raycast가 플레이어에 맞지 않으면 false를 반환
    }

    void UpdateStatusText(string status, Color color) // 상태 텍스트를 업데이트
    {
        if (statusText != null)
        {
            statusText.text = status;
            statusText.color = color;
        }
    }
}

