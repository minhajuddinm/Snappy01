using UnityEngine;

public class SnappyFollowFlow : MonoBehaviour
{
    private enum SnappyState
    {
        ApproachFront,
        WaitForDialogueToFinish,
        MoveBehind,
        FollowBehind
    }

    [Header("References")]
    public Transform robotVisual;
    public RobotDialogue robotDialogue;
    public GermSpawnerMR germSpawner;

    [Header("Movement")]
    public float moveSpeed = 0.8f;
    public float stopDistance = 0.25f;
    public float floorY = 0f;

    [Header("Front Position")]
    public float frontDistance = 1.2f;

    [Header("Behind Position")]
    public float behindDistance = 0.9f;
    public float sideOffset = 0.0f;

    [Header("Facing")]
    public float visualTurnSpeed = 5f;
    public float visualYawOffset = 0f;

    private Transform playerCam;
    private SnappyState currentState = SnappyState.ApproachFront;
    private bool introTriggered = false;
    private bool endTriggered = false;

    void Start()
    {
        if (Camera.main != null)
        {
            playerCam = Camera.main.transform;
        }

        Vector3 pos = transform.position;
        pos.y = floorY;
        transform.position = pos;
    }

    void Update()
    {
        if (playerCam == null) return;

        switch (currentState)
        {
            case SnappyState.ApproachFront:
                UpdateApproachFront();
                break;

            case SnappyState.WaitForDialogueToFinish:
                UpdateWaitForDialogue();
                break;

            case SnappyState.MoveBehind:
                UpdateMoveBehind();
                break;

            case SnappyState.FollowBehind:
                UpdateFollowBehind();
                break;
        }

        FacePlayer();
        KeepOnFloor();
    }

    void UpdateApproachFront()
    {
        Vector3 targetPos = GetFrontPosition();
        float distance = MoveTowardsTarget(targetPos);

        if (distance <= stopDistance)
        {
            if (!introTriggered && robotDialogue != null)
            {
                robotDialogue.PlayIntro();
                introTriggered = true;
            }

            currentState = SnappyState.WaitForDialogueToFinish;
        }
    }

    void UpdateWaitForDialogue()
    {
        Vector3 targetPos = GetFrontPosition();
        MoveTowardsTarget(targetPos);

        if (robotDialogue != null && robotDialogue.IsIntroFinished)
        {
            if (!endTriggered)
            {
                robotDialogue.PlayEnd();
                endTriggered = true;
            }

            currentState = SnappyState.MoveBehind;
        }
    }

   void UpdateMoveBehind()
{
    Vector3 targetPos = GetBehindPosition();
    float distance = MoveTowardsTarget(targetPos);

    if (distance <= stopDistance)
    {
        currentState = SnappyState.FollowBehind;

        if (germSpawner != null)
        {
            germSpawner.StartSpawning();
        }
    }
}

    void UpdateFollowBehind()
    {
        Vector3 targetPos = GetBehindPosition();
        MoveTowardsTarget(targetPos);
    }

    Vector3 GetFrontPosition()
    {
        Vector3 forwardFlat = playerCam.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        Vector3 targetPos = playerCam.position + forwardFlat * frontDistance;
        targetPos.y = floorY;
        return targetPos;
    }

    Vector3 GetBehindPosition()
    {
        Vector3 forwardFlat = playerCam.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        Vector3 rightFlat = playerCam.right;
        rightFlat.y = 0f;
        rightFlat.Normalize();

        Vector3 targetPos = playerCam.position
                          - forwardFlat * behindDistance
                          + rightFlat * sideOffset;

        targetPos.y = floorY;
        return targetPos;
    }

    float MoveTowardsTarget(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Vector3 moveDir = direction.normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        return distance;
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

    void KeepOnFloor()
    {
        Vector3 pos = transform.position;
        pos.y = floorY;
        transform.position = pos;
    }
}