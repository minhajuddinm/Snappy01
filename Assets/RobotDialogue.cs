using UnityEngine;

public class RobotDialogue : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip introClip;
    public AudioClip endClip;

    [Header("Animator")]
    public Animator animator;
    public string introTrigger = "Intro";
    public string endTrigger = "End";

    [Header("Text")]
    [TextArea]
    public string introText = "Hi… can you help me? The germs are coming… I don’t like them.";

    [TextArea]
    public string endText = "I’ll stay behind you… okay?";

    private bool introPlayed = false;
    private bool endPlayed = false;

    public bool IsIntroFinished
    {
        get
        {
            if (introClip == null) return introPlayed;
            if (audioSource == null) return introPlayed;
            return introPlayed && !audioSource.isPlaying;
        }
    }

    public void PlayIntro()
    {
        if (introPlayed) return;

        introPlayed = true;

        Debug.Log("Snappy says: " + introText);

        if (animator != null && !string.IsNullOrEmpty(introTrigger))
        {
            animator.SetTrigger(introTrigger);
        }

        if (audioSource != null && introClip != null)
        {
            audioSource.clip = introClip;
            audioSource.Play();
        }
    }

    public void PlayEnd()
    {
        if (endPlayed) return;

        endPlayed = true;

        Debug.Log("Snappy says: " + endText);

        if (animator != null && !string.IsNullOrEmpty(endTrigger))
        {
            animator.SetTrigger(endTrigger);
        }

        if (audioSource != null && endClip != null)
        {
            audioSource.PlayOneShot(endClip);
        }
    }
}