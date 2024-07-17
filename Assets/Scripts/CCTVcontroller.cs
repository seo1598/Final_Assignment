using System.Collections;
using UnityEngine;

public class CCTVcontroller : MonoBehaviour
{
    // 플레이어의 Transform 참조
    public Transform player;
    // CCTV의 탐지 각도
    public float detectionAngle = 120f;
    // CCTV의 탐지 거리
    public float detectionDistance = 15f;
    // CCTV의 회전 속도
    public float rotationSpeed = 30f;
    // 플레이어 감지 주기
    public float detectionInterval = 1f;
    // 탐지에 사용될 레이어 마스크
    public LayerMask detectionLayer;
    // 경비원 배열
    public Patrol[] guards;

    // 최소 회전 각도
    public float minRotationAngle = 66f;
    // 최대 회전 각도
    public float maxRotationAngle = 112f;

    // 현재 회전 각도
    private float currentAngle;
    // 회전 방향 (true면 오른쪽, false면 왼쪽)
    private bool rotatingRight = true;
    // CCTV의 Renderer 컴포넌트 참조
    private Renderer cctvRenderer;
    // 시야 콘 Mesh
    private Mesh visionConeMesh;
    // MeshFilter 컴포넌트 참조
    private MeshFilter meshFilter;

    // 시작 시 호출되는 메서드
    void Start()
    {
        // 자식 객체에서 Renderer 컴포넌트를 찾음
        cctvRenderer = GetComponentInChildren<Renderer>();
        if (cctvRenderer == null)
        {
            Debug.LogError("Renderer not found on CCTV or its children");
            return;
        }

        // 시작 각도를 최소 회전 각도로 설정
        currentAngle = minRotationAngle;
        // 플레이어 감지 코루틴 시작
        StartCoroutine(DetectPlayer());

        // MeshFilter 컴포넌트 추가
        meshFilter = gameObject.AddComponent<MeshFilter>();
        // MeshRenderer 컴포넌트 추가
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        // 기본 재질 설정
        meshRenderer.material = new Material(Shader.Find("Standard"));
        // 시야 콘 Mesh 생성
        CreateVisionConeMesh();
    }

    // 매 프레임마다 호출되는 메서드
    void Update()
    {
        // CCTV 회전
        RotateCCTV();
    }

    // CCTV를 회전시키는 메서드
    void RotateCCTV()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;

        if (rotatingRight)
        {
            currentAngle += rotationStep;
            if (currentAngle >= maxRotationAngle)
            {
                rotatingRight = false;
            }
        }
        else
        {
            currentAngle -= rotationStep;
            if (currentAngle <= minRotationAngle)
            {
                rotatingRight = true;
            }
        }

        // y축 기준으로 회전
        transform.localRotation = Quaternion.Euler(0, currentAngle, 0);
    }

    // 플레이어를 감지하는 코루틴
    IEnumerator DetectPlayer()
    {
        while (true)
        {
            // 감지 주기마다 대기
            yield return new WaitForSeconds(detectionInterval);

            // 플레이어와의 방향 및 거리 계산
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 플레이어가 탐지 각도와 거리 내에 있는지 확인
            if (angleToPlayer <= detectionAngle / 2 && distanceToPlayer <= detectionDistance)
            {
                RaycastHit hit;
                // 플레이어 방향으로 레이캐스트
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionDistance, detectionLayer))
                {
                    if (hit.transform == player)
                    {
                        // 경비원들에게 경고
                        AlertGuards();
                        // 플레이어에게 경고 표시
                        player.GetComponent<PlayerController>().ShowAlert();
                        // 플레이어가 감지된 경우 빨간색으로 변경
                        cctvRenderer.material.color = Color.red;
                    }
                    else
                    {
                        // 플레이어가 감지되지 않은 경우 초록색으로 변경
                        cctvRenderer.material.color = Color.green;
                    }
                }
            }
            else
            {
                // 플레이어가 감지되지 않은 경우 초록색으로 변경
                cctvRenderer.material.color = Color.green;
            }
        }
    }

    // 경비원들에게 경고하는 메서드
    void AlertGuards()
    {
        foreach (Patrol guard in guards)
        {
            guard.SetAlertPosition(player.position);
        }
    }

    // 시야 콘 Mesh를 생성하는 메서드
    void CreateVisionConeMesh()
    {
        visionConeMesh = new Mesh();

        int segments = 100;
        float angle = detectionAngle;
        float radius = detectionDistance;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -angle / 2 + (angle / segments) * i;
            float rad = Mathf.Deg2Rad * currentAngle;
            vertices[i + 1] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        visionConeMesh.vertices = vertices;
        visionConeMesh.triangles = triangles;
        visionConeMesh.RecalculateNormals();

        meshFilter.mesh = visionConeMesh;
    }

    // 선택된 상태에서 기즈모를 그리는 메서드
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionDistance;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}
