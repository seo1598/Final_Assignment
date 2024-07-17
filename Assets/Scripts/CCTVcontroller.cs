using System.Collections;
using UnityEngine;

public class CCTVcontroller : MonoBehaviour
{
    // �÷��̾��� Transform ����
    public Transform player;
    // CCTV�� Ž�� ����
    public float detectionAngle = 120f;
    // CCTV�� Ž�� �Ÿ�
    public float detectionDistance = 15f;
    // CCTV�� ȸ�� �ӵ�
    public float rotationSpeed = 30f;
    // �÷��̾� ���� �ֱ�
    public float detectionInterval = 1f;
    // Ž���� ���� ���̾� ����ũ
    public LayerMask detectionLayer;
    // ���� �迭
    public Patrol[] guards;

    // �ּ� ȸ�� ����
    public float minRotationAngle = 66f;
    // �ִ� ȸ�� ����
    public float maxRotationAngle = 112f;

    // ���� ȸ�� ����
    private float currentAngle;
    // ȸ�� ���� (true�� ������, false�� ����)
    private bool rotatingRight = true;
    // CCTV�� Renderer ������Ʈ ����
    private Renderer cctvRenderer;
    // �þ� �� Mesh
    private Mesh visionConeMesh;
    // MeshFilter ������Ʈ ����
    private MeshFilter meshFilter;

    // ���� �� ȣ��Ǵ� �޼���
    void Start()
    {
        // �ڽ� ��ü���� Renderer ������Ʈ�� ã��
        cctvRenderer = GetComponentInChildren<Renderer>();
        if (cctvRenderer == null)
        {
            Debug.LogError("Renderer not found on CCTV or its children");
            return;
        }

        // ���� ������ �ּ� ȸ�� ������ ����
        currentAngle = minRotationAngle;
        // �÷��̾� ���� �ڷ�ƾ ����
        StartCoroutine(DetectPlayer());

        // MeshFilter ������Ʈ �߰�
        meshFilter = gameObject.AddComponent<MeshFilter>();
        // MeshRenderer ������Ʈ �߰�
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        // �⺻ ���� ����
        meshRenderer.material = new Material(Shader.Find("Standard"));
        // �þ� �� Mesh ����
        CreateVisionConeMesh();
    }

    // �� �����Ӹ��� ȣ��Ǵ� �޼���
    void Update()
    {
        // CCTV ȸ��
        RotateCCTV();
    }

    // CCTV�� ȸ����Ű�� �޼���
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

        // y�� �������� ȸ��
        transform.localRotation = Quaternion.Euler(0, currentAngle, 0);
    }

    // �÷��̾ �����ϴ� �ڷ�ƾ
    IEnumerator DetectPlayer()
    {
        while (true)
        {
            // ���� �ֱ⸶�� ���
            yield return new WaitForSeconds(detectionInterval);

            // �÷��̾���� ���� �� �Ÿ� ���
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // �÷��̾ Ž�� ������ �Ÿ� ���� �ִ��� Ȯ��
            if (angleToPlayer <= detectionAngle / 2 && distanceToPlayer <= detectionDistance)
            {
                RaycastHit hit;
                // �÷��̾� �������� ����ĳ��Ʈ
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionDistance, detectionLayer))
                {
                    if (hit.transform == player)
                    {
                        // �����鿡�� ���
                        AlertGuards();
                        // �÷��̾�� ��� ǥ��
                        player.GetComponent<PlayerController>().ShowAlert();
                        // �÷��̾ ������ ��� ���������� ����
                        cctvRenderer.material.color = Color.red;
                    }
                    else
                    {
                        // �÷��̾ �������� ���� ��� �ʷϻ����� ����
                        cctvRenderer.material.color = Color.green;
                    }
                }
            }
            else
            {
                // �÷��̾ �������� ���� ��� �ʷϻ����� ����
                cctvRenderer.material.color = Color.green;
            }
        }
    }

    // �����鿡�� ����ϴ� �޼���
    void AlertGuards()
    {
        foreach (Patrol guard in guards)
        {
            guard.SetAlertPosition(player.position);
        }
    }

    // �þ� �� Mesh�� �����ϴ� �޼���
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

    // ���õ� ���¿��� ����� �׸��� �޼���
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
