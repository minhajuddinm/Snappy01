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

    [Header("Gameplay References")]
    public FingerGunShooter fingerGunShooter;
    public GermSpawnerMR germSpawner;

    private int killCount = 0;
    private bool gameEnded = false;

    public void AddKill()
    {
        if (gameEnded) return;

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
                gameEnded = true;

                Debug.Log("Reached 15 kills. Ending gameplay.");

                if (fingerGunShooter != null)
                {
                    fingerGunShooter.canShoot = false;
                    Debug.Log("RobotPraiseSystem: Shooting disabled.");
                }
                else
                {
                    Debug.LogWarning("RobotPraiseSystem: FingerGunShooter reference is missing.");
                }

                if (germSpawner != null)
                {
                    germSpawner.StopSpawning();
                    germSpawner.DisableAllGerms();
                    Debug.Log("RobotPraiseSystem: Germ spawning stopped and remaining germs cleared.");
                }
                else
                {
                    Debug.LogWarning("RobotPraiseSystem: GermSpawnerMR reference is missing.");
                }

                if (finalDialogue != null)
                {
                    finalDialogue.PlayFinalDialogue();
                }
                else
                {
                    Debug.LogWarning("RobotPraiseSystem: FinalDialogue reference is missing.");
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