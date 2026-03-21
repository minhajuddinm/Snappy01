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

    [Range(0, 1)] public float thumbThreshold = 0.7f; // How curled the thumb needs to be to fire

    private bool hasFiredLeft  = false;
    private bool hasFiredRight = false;

    void Update()
    {
        if (leftHand.IsTracked)  CheckThumb(leftHand,  leftSkeleton,  ref hasFiredLeft);
        if (rightHand.IsTracked) CheckThumb(rightHand, rightSkeleton, ref hasFiredRight);
    }

    void CheckThumb(OVRHand hand, OVRSkeleton skeleton, ref bool hasFired)
    {
        float thumbCurl = hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);

        if (thumbCurl >= thumbThreshold)
        {
            if (!hasFired)
            {
                ShootFromIndex(skeleton);
                hasFired = true;
            }
        }
        else if (thumbCurl < 0.3f)
        {
            hasFired = false;
        }
    }

    void ShootFromIndex(OVRSkeleton skeleton)
    {
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

        if (indexTip == null || indexKnuckle == null) return;

        Vector3 fingerDir = (indexTip.position - indexKnuckle.position).normalized;
        Vector3 spawnPos  = indexTip.position + fingerDir * 0.05f;

        GameObject ball = Instantiate(paintballPrefab, spawnPos, Quaternion.LookRotation(fingerDir));

        PaintballSticker sticker = ball.AddComponent<PaintballSticker>();
        sticker.damageValue = damagePerShot;
        ball.GetComponent<Renderer>().material.color = new Color(1.0f, 0.5f, 0.0f);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity  = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(fingerDir * shootForce, ForceMode.Impulse);
        }

        Destroy(ball, 5f);
    }
}