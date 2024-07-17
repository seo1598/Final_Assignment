using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Patrol : MonoBehaviour
{
    // 순찰 지점 배열
    public Transform[] patrolPoints;
    // 벽 회피 거리
    public float wallAvoidanceDistance = 1.0f;
    // 벽 레이어 마스크
    public LayerMask wallLayer;
    // 플레이어 참조
    public Transform player;
    // 추적 거리
    public float chaseDistance = 10.0f;
    // 플레이어를 잃었을 때까지의 시간
    public float losePlayerTime = 5.0f;
    // 시야각
    public float fieldOfViewAngle = 45.0f;
    // 상태 텍스트
    public TextMeshPro statusText;
    // 경고 상태 지속 시간
    public float alertDuration = 10.0f;
    // 게임오버 캔버스 참조
    public GameObject gameOverCanvas;

    // 현재 순찰 지점 인덱스
    private int currentPointIndex;
    // NavMeshAgent 컴포넌트
    private NavMeshAgent agent;
    // 마지막으로 플레이어를 본 이후의 시간
    private float timeSinceLastSeenPlayer;
    // 경고 상태 여부
    private bool isAlerted;
    // 경고 위치
    private Vector3 alertPosition;

    // 시작 시 호출되는 메서드
    void Start()
    {
        // NavMeshAgent 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
        // 순찰 지점이 있는 경우
        if (patrolPoints.Length > 0)
        {
            // 초기 순찰 지점 인덱스 설정
            currentPointIndex = 0;
            // 다음 목적지 설정
            SetNextDestination();
            // 상태 텍스트 업데이트
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    // 매 프레임마다 호출되는 메서드
    void Update()
    {
        // 경고 상태인 경우
        if (isAlerted)
        {
            AlertedBehavior();
        }
        else
        {
            PatrolBehavior();
        }
    }

    // 순찰 상태에서의 동작
    void PatrolBehavior()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // 플레이어 방향 계산
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // 시야각 내에 있는지 확인
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // 플레이어가 추적 거리 내에 있고 시야각 내에 있는 경우
        if (distanceToPlayer <= chaseDistance && angleToPlayer <= fieldOfViewAngle / 2)
        {
            if (CanSeePlayer())
            {
                // 플레이어를 추적
                agent.SetDestination(player.position);
                UpdateStatusText("Player Spotted", Color.red);
            }
        }
        // 순찰 지점에 도착한 경우
        else if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            // 다음 순찰 지점으로 이동
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            SetNextDestination();
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    // 경고 상태에서의 동작
    void AlertedBehavior()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // 플레이어 방향 계산
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        // 시야각 내에 있는지 확인
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // 플레이어가 추적 거리 내에 있고 시야각 내에 있는 경우
        if (distanceToPlayer <= chaseDistance && angleToPlayer <= fieldOfViewAngle / 2)
        {
            if (CanSeePlayer())
            {
                // 경고 상태 해제
                isAlerted = false;
                // 플레이어를 추적
                agent.SetDestination(player.position);
                UpdateStatusText("Player Spotted", Color.red);
                return;
            }
        }

        // 경고 위치에 도착한 경우
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            StartCoroutine(AlertWait());
            isAlerted = false;
        }

        // 벽에 막혀서 경로를 찾을 수 없는 경우 패트롤 모드로 복귀
        if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            isAlerted = false;
            SetNextDestination();
            UpdateStatusText("Patrolling", Color.blue);
        }
    }

    // 다음 순찰 지점 설정
    void SetNextDestination()
    {
        Vector3 destination = patrolPoints[currentPointIndex].position;
        Vector3 adjustedDestination = AdjustDestinationForWall(destination);
        agent.SetDestination(adjustedDestination);
    }

    // 목적지를 벽 회피 거리만큼 조정
    Vector3 AdjustDestinationForWall(Vector3 destination)
    {
        Vector3 directionToDestination = (destination - transform.position).normalized;
        RaycastHit hit;

        // 목적지로 가는 경로에 벽이 있는지 확인
        if (Physics.Raycast(transform.position, directionToDestination, out hit, wallAvoidanceDistance, wallLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;
            destination = hitPoint + hitNormal * wallAvoidanceDistance;
        }

        return destination;
    }

    // 플레이어를 볼 수 있는지 확인
    bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // 플레이어 방향으로 레이캐스트
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
        {
            if (hit.transform == player)
            {
                return true; // 플레이어가 보임
            }
        }

        return false; // 플레이어가 벽 뒤에 있음
    }

    // 상태 텍스트 업데이트
    void UpdateStatusText(string status, Color color)
    {
        if (statusText != null)
        {
            statusText.text = status;
            statusText.color = color;
        }
    }

    // 경고 위치 설정
    public void SetAlertPosition(Vector3 position)
    {
        alertPosition = position;
        isAlerted = true;
        agent.SetDestination(alertPosition);
        UpdateStatusText("Alerted", Color.yellow);
    }

    // 경고 상태에서 일정 시간 동안 대기
    IEnumerator AlertWait()
    {
        UpdateStatusText("Searching", Color.yellow);
        yield return new WaitForSeconds(alertDuration);
        SetNextDestination();
        UpdateStatusText("Patrolling", Color.blue);
    }

    // 플레이어와 충돌한 경우 게임오버
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameOver();
        }
    }

    // 게임오버 처리
    void GameOver()
    {
        Time.timeScale = 0f; // 게임 일시정지
        gameOverCanvas.SetActive(true); // 게임오버 캔버스 활성화
    }
}

