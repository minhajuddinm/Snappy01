using UnityEngine;

public class RobotComeToPlayer : MonoBehaviour
{
    public float moveSpeed = 0.8f;
    public float rotationSpeed = 5f;
    public float stopDistance = 0.4f;
    public float frontOffset = 1.2f;
    public float floorY = 0f;

    private Transform playerCam;
    private bool hasReachedTarget = false;

    void Start()
    {
        if (Camera.main != null)
        {
            playerCam = Camera.main.transform;
        }

        // Keep robot on floor
        Vector3 pos = transform.position;
        pos.y = floorY;
        transform.position = pos;
    }

    void Update()
    {
        if (playerCam == null || hasReachedTarget) return;

        // Get point in front of player
        Vector3 forwardFlat = playerCam.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        Vector3 targetPos = playerCam.position + forwardFlat * frontOffset;
        targetPos.y = floorY;

        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= stopDistance)
        {
            hasReachedTarget = true;
            return;
        }

        Vector3 moveDir = direction.normalized;

        // Rotate robot toward movement direction
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Move robot on floor
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}