using UnityEngine;

public class FingerGunShooter : MonoBehaviour
{
    [Header("Hand References")]
    public OVRHand leftHand;
    public OVRHand rightHand;
    public OVRSkeleton leftSkeleton;
    public OVRSkeleton rightSkeleton;

    [Header("Paintball Settings")]
    public GameObject paintballPrefab;
    public float shootForce = 35f;
    public float damagePerShot = 1.0f;

    [Header("Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Range(0, 1)] public float indexThreshold = 0.7f;

    private bool hasFiredLeft  = false;
    private bool hasFiredRight = false;

    void Update()
    {
        if (leftHand.IsTracked)  CheckIndex(leftHand,  leftSkeleton,  ref hasFiredLeft,  leftSpawnPoint,  "Left");
        if (rightHand.IsTracked) CheckIndex(rightHand, rightSkeleton, ref hasFiredRight, rightSpawnPoint, "Right");
    }

    void CheckIndex(OVRHand hand, OVRSkeleton skeleton, ref bool hasFired, Transform spawnPoint, string handLabel)
    {
        float indexCurl = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        Debug.Log($"[FingerGun] {handLabel} index curl: {indexCurl:F2} | hasFired: {hasFired}");

        if (indexCurl >= indexThreshold)
        {
            if (!hasFired)
            {
                Debug.Log($"[FingerGun] {handLabel} hand FIRED! Curl: {indexCurl:F2}");
                ShootFromIndex(skeleton, spawnPoint, handLabel);
                hasFired = true;
            }
        }
        else if (indexCurl < 0.3f)
        {
            if (hasFired)
                Debug.Log($"[FingerGun] {handLabel} hand reset — ready to fire again.");

            hasFired = false;
        }
    }

    void ShootFromIndex(OVRSkeleton skeleton, Transform spawnPoint, string handLabel)
    {
        Vector3 spawnPos;
        Vector3 fingerDir;

        if (spawnPoint != null)
        {
            spawnPos  = spawnPoint.position;
            fingerDir = spawnPoint.forward;
            Debug.Log($"[FingerGun] {handLabel} spawning from spawnPoint at {spawnPos}");
        }
        else
        {
            Debug.LogWarning($"[FingerGun] {handLabel} has no spawnPoint assigned — falling back to index tip.");

            Transform indexTip     = null;
            Transform indexKnuckle = null;

            foreach (var bone in skeleton.Bones)
            {
                switch (bone.Id)
                {
                    case OVRSkeleton.BoneId.Hand_IndexTip: indexTip     = bone.Transform; break;
                    case OVRSkeleton.BoneId.Hand_Index1:   indexKnuckle = bone.Transform; break;
                }
            }

            if (indexTip == null || indexKnuckle == null)
            {
                Debug.LogError($"[FingerGun] {handLabel} could not find index finger bones. Aborting shot.");
                return;
            }

            fingerDir = (indexTip.position - indexKnuckle.position).normalized;
            spawnPos  = indexTip.position + fingerDir * 0.05f;
        }

        // --- Spawn paintball ---
        GameObject ball = Instantiate(paintballPrefab, spawnPos, Quaternion.LookRotation(fingerDir));

        ball.tag = "Paintball";

        PaintballSticker sticker = ball.AddComponent<PaintballSticker>();
        sticker.damageValue = damagePerShot;

        Renderer rend = ball.GetComponent<Renderer>();
if (rend != null)
{
    rend.material.color = new Color(1.0f, 0.5f, 0.0f);
}
        ball.GetComponent<Renderer>().material.color = new Color(1.0f, 0.5f, 0.0f);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity  = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(fingerDir * shootForce, ForceMode.Impulse);

            Debug.Log($"[FingerGun] {handLabel} ball launched. Direction: {fingerDir}, Force: {shootForce}");
        }
        else
        {
            Debug.LogWarning($"[FingerGun] {handLabel} paintball prefab has no Rigidbody — it won't move!");
        }

        Destroy(ball, 5f);
    }
}