using UnityEngine;

// Reproduce el SFX de gol. No conoce a GameManager mas alla del evento OnGoalScored.
public class GoalAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _goalAudio;

    private void Awake()
    {
        if (_goalAudio == null)
            Debug.LogWarning("[GoalAudioPlayer] Falta asignar _goalAudio en el Inspector.");
    }

    private void OnEnable()
    {
        GameManager.OnGoalScored += PlayGoalSfx;
    }

    private void OnDisable()
    {
        GameManager.OnGoalScored -= PlayGoalSfx;
    }

    private void PlayGoalSfx()
    {
        _goalAudio?.Play();
    }
}
