using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVcontroller : MonoBehaviour
{
    public Transform player;
    public float detectionAngle = 120f;
    public float detectionDistance = 15f;
    public float rotationSpeed = 30f;
    public float detectionInterval = 1f; // 플레이어 감지 주기
    public LayerMask detectionLayer;
    public Patrol[] guards; // 경비원 배열

    public float minRotationAngle = 66f; // 최소 회전 각도
    public float maxRotationAngle = 112f; // 최대 회전 각도

    private float currentAngle;
    private bool rotatingRight = true;
    private Renderer cctvRenderer;

    void Start()
    {
        cctvRenderer = GetComponent<Renderer>();
        currentAngle = minRotationAngle; // 시작 각도를 최소 회전 각도로 설정
        StartCoroutine(DetectPlayer());
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

        transform.localRotation = Quaternion.Euler(0, currentAngle, 0); // y축 기준으로 회전
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
                        cctvRenderer.material.color = Color.red; // 플레이어가 감지된 경우 빨간색으로 변경
                    }
                    else
                    {
                        cctvRenderer.material.color = Color.green; // 플레이어가 감지되지 않은 경우 초록색으로 변경
                    }
                }
            }
            else
            {
                cctvRenderer.material.color = Color.green; // 플레이어가 감지되지 않은 경우 초록색으로 변경
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
