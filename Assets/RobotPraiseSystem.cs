using UnityEngine;

public class RobotPraiseSystem : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Praise Clips")]
    public AudioClip clip5;
    public AudioClip clip10;
    public AudioClip clip15;
    public AudioClip clip20;

    [Header("Final Dialogue")]
    public RobotFinalDialogue finalDialogue;

    private int killCount = 0;

    public void AddKill()
    {
        killCount++;
        Debug.Log("Kill count = " + killCount);

        switch (killCount)
        {
            case 5:
                PlayLine(clip5, "Good job!");
                break;

            case 10:
                PlayLine(clip10, "Wow... keep going!");
                break;

            case 15:
                PlayLine(clip15, "You are protecting us... you are a hero.");

                if (finalDialogue != null)
                {
                    finalDialogue.PlayFinalDialogue();
                }
                break;
        }
    }

    void PlayLine(AudioClip clip, string text)
    {
        Debug.Log("Robot says: " + text);

        if (audioSource == null)
        {
            Debug.LogWarning("RobotPraiseSystem: No AudioSource assigned.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("RobotPraiseSystem: Missing audio clip for line: " + text);
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}