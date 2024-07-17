using System.Collections;
using UnityEngine;

public class CCTVcontroller : MonoBehaviour
{
    public Transform player;
    public float detectionAngle = 120f;
    public float detectionDistance = 15f;
    public float rotationSpeed = 30f;
    public float detectionInterval = 1f; // �÷��̾� ���� �ֱ�
    public LayerMask detectionLayer;
    public Patrol[] guards; // ���� �迭

    public float minRotationAngle = 66f; // �ּ� ȸ�� ����
    public float maxRotationAngle = 112f; // �ִ� ȸ�� ����

    private float currentAngle;
    private bool rotatingRight = true;
    private Renderer cctvRenderer;
    private Mesh visionConeMesh;
    private MeshFilter meshFilter;

    void Start()
    {
        cctvRenderer = GetComponentInChildren<Renderer>();
        if (cctvRenderer == null)
        {
            Debug.LogError("Renderer not found on CCTV or its children");
            return;
        }

        currentAngle = minRotationAngle; // ���� ������ �ּ� ȸ�� ������ ����
        StartCoroutine(DetectPlayer());

        meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        CreateVisionConeMesh();
    }

    void Update()
    {
        RotateCCTV();
    }

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

        transform.localRotation = Quaternion.Euler(0, currentAngle, 0); // y�� �������� ȸ��
    }

    IEnumerator DetectPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionInterval);

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (angleToPlayer <= detectionAngle / 2 && distanceToPlayer <= detectionDistance)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionDistance, detectionLayer))
                {
                    if (hit.transform == player)
                    {
                        AlertGuards();
                        cctvRenderer.material.color = Color.red; // �÷��̾ ������ ��� ���������� ����
                    }
                    else
                    {
                        cctvRenderer.material.color = Color.green; // �÷��̾ �������� ���� ��� �ʷϻ����� ����
                    }
                }
            }
            else
            {
                cctvRenderer.material.color = Color.green; // �÷��̾ �������� ���� ��� �ʷϻ����� ����
            }
        }
    }

    void AlertGuards()
    {
        foreach (Patrol guard in guards)
        {
            guard.SetAlertPosition(player.position);
        }
    }

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
