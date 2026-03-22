using UnityEngine;
using System.Collections;
using Meta.XR.MRUtilityKit;

public class GermMove : MonoBehaviour
{
    public float moveSpeed = 0.4f;
    public float rotationSpeed = 4f;
    public float waitTime = 1f;
    public float moveRange = 2f;
    public float floorOffsetY = 0.05f;

    private MRUKRoom room;
    private MRUKAnchor floorAnchor;

    private Vector3 targetPosition;
    private bool hasTarget = false;
    private bool isWaiting = false;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        yield return new WaitUntil(() => MRUK.Instance != null && MRUK.Instance.GetCurrentRoom() != null);

        room = MRUK.Instance.GetCurrentRoom();
        floorAnchor = room.GetFloorAnchor();

        if (floorAnchor == null)
        {
            Debug.LogError("No floor anchor found for germ movement.");
            yield break;
        }

        PickNewTarget();
    }

    void Update()
    {
        if (floorAnchor == null || !hasTarget || isWaiting) return;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 0.1f)
        {
            StartCoroutine(WaitAndPickNextTarget());
            return;
        }

        Vector3 moveDir = direction.normalized;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // keep on floor
        Vector3 pos = transform.position;
        pos.y = floorAnchor.GetAnchorCenter().y + floorOffsetY;
        transform.position = pos;
    }

    IEnumerator WaitAndPickNextTarget()
    {
        hasTarget = false;
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        PickNewTarget();
        isWaiting = false;
    }

    void PickNewTarget()
    {
        Vector3 center = floorAnchor.GetAnchorCenter();

        float randomX = Random.Range(-moveRange, moveRange);
        float randomZ = Random.Range(-moveRange, moveRange);

        targetPosition = new Vector3(
            center.x + randomX,
            center.y + floorOffsetY,
            center.z + randomZ
        );

        hasTarget = true;
    }
}