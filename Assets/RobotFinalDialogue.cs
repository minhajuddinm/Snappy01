using UnityEngine;

public class RobotFinalDialogue : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip finalClip;

    [Header("Animator")]
    public Animator animator;
    public string finalTrigger = "Final";

    [Header("Text")]
    [TextArea]
    public string finalText = "Congratulations… you saved me. You are my true hero… help me defeat more germs… please.";

    [Header("Positioning")]
    public Transform playerCam;
    public float distanceInFront = 1.5f;
    public float heightOffset = -0.2f;

    private bool hasPlayed = false;

    public void PlayFinalDialogue()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        MoveInFrontOfPlayer();

        if (animator != null)
        {
            animator.SetTrigger(finalTrigger);
        }

        if (audioSource != null && finalClip != null)
        {
            audioSource.PlayOneShot(finalClip);
        }

        Debug.Log("Robot Final Dialogue: " + finalText);
    }

    void MoveInFrontOfPlayer()
    {
        if (playerCam == null) return;

        Vector3 forward = playerCam.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 targetPos = playerCam.position + forward * distanceInFront;
        targetPos.y = playerCam.position.y + heightOffset;

        transform.position = targetPos;

        // face the player
        transform.LookAt(playerCam);
    }
}