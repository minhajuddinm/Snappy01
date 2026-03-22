using UnityEngine;

public class RobotFollowFrontOfPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0.8f;
    public float stopDistance = 0.3f;
    public float followDistance = 1.2f;
    public float floorY = 0f;

    [Header("Visual Facing")]
    public Transform robotVisual;
    public float visualTurnSpeed = 5f;
    public float visualYawOffset = 0f;

    private Transform playerCam;

    void Start()
    {
        if (Camera.main != null)
        {
            playerCam = Camera.main.transform;
        }
    }

    void Update()
    {
        if (playerCam == null) return;

        FollowToFrontOfPlayer();
        FacePlayer();
    }

    void FollowToFrontOfPlayer()
    {
        Vector3 forwardFlat = playerCam.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        Vector3 targetPos = playerCam.position + forwardFlat * followDistance;
        targetPos.y = floorY;

        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Vector3 moveDir = direction.normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        // Keep parent locked to floor
        Vector3 pos = transform.position;
        pos.y = floorY;
        transform.position = pos;
    }

    void FacePlayer()
    {
        if (robotVisual == null) return;

        Vector3 lookDir = playerCam.position - robotVisual.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
        targetRot *= Quaternion.Euler(0f, visualYawOffset, 0f);

        robotVisual.rotation = Quaternion.Slerp(
            robotVisual.rotation,
            targetRot,
            visualTurnSpeed * Time.deltaTime
        );
    }
}