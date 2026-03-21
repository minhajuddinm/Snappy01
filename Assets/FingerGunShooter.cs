using UnityEngine;
using System.Collections.Generic;

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
    
    [Range(0, 1)]
    public float triggerThreshold = 0.8f; // How much you have to bend to fire

    private bool hasFiredLeft = false;
    private bool hasFiredRight = false;

    void Update()
    {
        if (leftHand.IsTracked) CheckTrigger(leftHand, leftSkeleton, ref hasFiredLeft);
        if (rightHand.IsTracked) CheckTrigger(rightHand, rightSkeleton, ref hasFiredRight);
    }

    void CheckTrigger(OVRHand hand, OVRSkeleton skeleton, ref bool hasFired)
    {
        // Detect the "bend" of the index finger
        float indexPinch = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        // Logic: If the finger is curled tightly (like a trigger pull)
        if (indexPinch >= triggerThreshold)
        {
            if (!hasFired)
            {
                ShootFromFist(hand, skeleton);
                hasFired = true; 
            }
        }
        else if (indexPinch < 0.2f) // Must release the finger to shoot again
        {
            hasFired = false;
        }
    }

    void ShootFromFist(OVRHand hand, OVRSkeleton skeleton)
    {
        Transform knuckle = null;
        Transform wrist = null;

        foreach (var bone in skeleton.Bones)
        {
            // We use the Index Knuckle as the barrel start
            if (bone.Id == OVRSkeleton.BoneId.Hand_Index1) knuckle = bone.Transform;
            if (bone.Id == OVRSkeleton.BoneId.Hand_WristRoot) wrist = bone.Transform;
        }

        if (knuckle == null || wrist == null) return;

        // Direction: From the wrist through the knuckle (the "forward" of your fist)
        Vector3 shootDirection = (knuckle.position - wrist.position).normalized;
        
        // Tilt it slightly down as requested before
        shootDirection = Vector3.Lerp(shootDirection, Vector3.down, 0.1f).normalized;

        // Spawn right at the knuckle, pushed out slightly
        Vector3 spawnPos = knuckle.position + (shootDirection * 0.05f); 

        GameObject ball = Instantiate(paintballPrefab, spawnPos, Quaternion.identity);
        
        if (ball.GetComponent<PaintballSticker>() == null)
            ball.AddComponent<PaintballSticker>();

        ball.GetComponent<Renderer>().material.color = new Color(1.0f, 0.5f, 0.0f);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(-shootDirection * shootForce, ForceMode.Impulse);
        }

        Destroy(ball, 10f);
    }
}