using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    // 시야 콘의 재질
    public Material VisionConeMaterial;
    // 시야 콘의 거리
    public float VisionRange;
    // 시야 콘의 각도
    public float VisionAngle;
    // 시야를 막는 레이어 마스크
    public LayerMask VisionObstructingLayer;
    // 시야 콘의 해상도 (삼각형 수)
    public int VisionConeResolution = 120;
    // 시야 콘의 Mesh
    Mesh VisionConeMesh;
    // MeshFilter 컴포넌트 참조
    MeshFilter MeshFilter_;

    // 시작 시 호출되는 메서드
    void Start()
    {
        // MeshRenderer 컴포넌트를 추가하고 재질 설정
        transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        // MeshFilter 컴포넌트 추가
        MeshFilter_ = transform.AddComponent<MeshFilter>();
        // 새로운 Mesh 생성
        VisionConeMesh = new Mesh();
        // 시야 각도를 라디안으로 변환
        VisionAngle *= Mathf.Deg2Rad;
    }

    // 매 프레임마다 호출되는 메서드
    void Update()
    {
        // 시야 콘을 그림
        DrawVisionCone();
    }

    // 시야 콘을 그리는 메서드
    void DrawVisionCone()
    {
        // 삼각형 인덱스 배열 생성
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        // 정점 배열 생성
        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        // 원점 설정
        Vertices[0] = Vector3.zero;
        // 현재 각도 초기화
        float Currentangle = -VisionAngle / 2;
        // 각도 증가량 계산
        float angleIcrement = VisionAngle / (VisionConeResolution - 1);
        float Sine;
        float Cosine;

        // 각 정점 계산
        for (int i = 0; i < VisionConeResolution; i++)
        {
            // 각도에 따른 사인 및 코사인 계산
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            // 레이캐스트 방향 계산
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            // 정점의 전방 방향 계산
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            // 레이캐스트를 사용하여 시야를 막는 객체가 있는지 확인
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                // 시야를 막는 객체가 있는 경우 해당 거리까지 정점 설정
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                // 시야를 막는 객체가 없는 경우 최대 거리까지 정점 설정
                Vertices[i + 1] = VertForward * VisionRange;
            }

            // 현재 각도 증가
            Currentangle += angleIcrement;
        }

        // 삼각형 인덱스 설정
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        // Mesh 초기화 및 정점과 삼각형 설정
        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        // MeshFilter에 Mesh 설정
        MeshFilter_.mesh = VisionConeMesh;
    }
}
