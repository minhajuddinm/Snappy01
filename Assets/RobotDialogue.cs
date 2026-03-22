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

    [Header("Gameplay References")]
    public FingerGunShooter fingerGunShooter;
    public GermSpawnerMR germSpawner;

    private bool introPlayed = false;
    private bool endPlayed = false;
    private bool gameplayStarted = false;

    public bool IsIntroFinished
    {
        get
        {
            if (!introPlayed) return false;
            if (introClip == null) return true;
            if (audioSource == null) return true;
            return !audioSource.isPlaying;
        }
    }

    void Update()
    {
        if (!gameplayStarted && IsIntroFinished)
        {
            gameplayStarted = true;
            StartGameplay();
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
        else
        {
            Debug.LogWarning("RobotDialogue: Intro audioSource or introClip is missing. Gameplay will still start.");
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

    void StartGameplay()
    {
        Debug.Log("RobotDialogue: Intro finished. Starting gameplay.");

        if (fingerGunShooter != null)
        {
            fingerGunShooter.canShoot = true;
            Debug.Log("RobotDialogue: Shooting enabled.");
        }
        else
        {
            Debug.LogWarning("RobotDialogue: FingerGunShooter reference is missing.");
        }

        if (germSpawner != null)
        {
            germSpawner.StartSpawning();
            Debug.Log("RobotDialogue: Germ spawning started.");
        }
        else
        {
            Debug.LogWarning("RobotDialogue: GermSpawnerMR reference is missing.");
        }

        PlayEnd();
    }
}