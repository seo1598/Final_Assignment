using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    // �þ� ���� ����
    public Material VisionConeMaterial;
    // �þ� ���� �Ÿ�
    public float VisionRange;
    // �þ� ���� ����
    public float VisionAngle;
    // �þ߸� ���� ���̾� ����ũ
    public LayerMask VisionObstructingLayer;
    // �þ� ���� �ػ� (�ﰢ�� ��)
    public int VisionConeResolution = 120;
    // �þ� ���� Mesh
    Mesh VisionConeMesh;
    // MeshFilter ������Ʈ ����
    MeshFilter MeshFilter_;

    // ���� �� ȣ��Ǵ� �޼���
    void Start()
    {
        // MeshRenderer ������Ʈ�� �߰��ϰ� ���� ����
        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        // MeshFilter ������Ʈ �߰�
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        // ���ο� Mesh ����
        VisionConeMesh = new Mesh();
        // �þ� ������ �������� ��ȯ
        VisionAngle *= Mathf.Deg2Rad;
    }

    // �� �����Ӹ��� ȣ��Ǵ� �޼���
    void Update()
    {
        // �þ� ���� �׸�
        DrawVisionCone();
    }

    // �þ� ���� �׸��� �޼���
    void DrawVisionCone()
    {
        // �ﰢ�� �ε��� �迭 ����
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        // ���� �迭 ����
        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        // ���� ����
        Vertices[0] = Vector3.zero;
        // ���� ���� �ʱ�ȭ
        float Currentangle = -VisionAngle / 2;
        // ���� ������ ���
        float angleIcrement = VisionAngle / (VisionConeResolution - 1);
        float Sine;
        float Cosine;

        // �� ���� ���
        for (int i = 0; i < VisionConeResolution; i++)
        {
            // ������ ���� ���� �� �ڻ��� ���
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            // ����ĳ��Ʈ ���� ���
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            // ������ ���� ���� ���
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            // ����ĳ��Ʈ�� ����Ͽ� �þ߸� ���� ��ü�� �ִ��� Ȯ��
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                // �þ߸� ���� ��ü�� �ִ� ��� �ش� �Ÿ����� ���� ����
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                // �þ߸� ���� ��ü�� ���� ��� �ִ� �Ÿ����� ���� ����
                Vertices[i + 1] = VertForward * VisionRange;
            }

            // ���� ���� ����
            Currentangle += angleIcrement;
        }

        // �ﰢ�� �ε��� ����
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        // Mesh �ʱ�ȭ �� ������ �ﰢ�� ����
        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        // MeshFilter�� Mesh ����
        MeshFilter_.mesh = VisionConeMesh;
    }
}
