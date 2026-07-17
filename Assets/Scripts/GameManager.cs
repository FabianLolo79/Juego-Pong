using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _paddleScore1Text;
    [SerializeField] private TMP_Text _paddleScore2Text;
    [SerializeField] private Transform _paddle1Transform;
    [SerializeField] private Transform _paddle2Transform;
    [SerializeField] private Transform _ballTransform;

    [Header("Audio")]
    [SerializeField] private AudioSource _crowdGameAudio;
    [SerializeField] private AudioSource _goalAudio;
    [SerializeField] private AudioSource _ambient;

    [Header("VFX - Feedback de gol")]
    [SerializeField] private ParticleSystem _scoreVFX; // opcional: arrastra un Particle System si tenes uno
    [SerializeField] private float _punchScale = 1.8f;
    [SerializeField] private float _punchDuration = 0.5f;

    private int _paddleScore1;
    private int _paddleScore2;
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    void Start()
    {
        _crowdGameAudio?.Play();
        _ambient?.Play();
    }

    public void PaddleScored1()
    {
        _paddleScore1++;
        _paddleScore1Text.text = _paddleScore1.ToString();
        StartCoroutine(ScorePunch(_paddleScore1Text));
        PlayGoal();
    }

    public void PaddleScored2()
    {
        _paddleScore2++;
        _paddleScore2Text.text = _paddleScore2.ToString();
        StartCoroutine(ScorePunch(_paddleScore2Text));
        PlayGoal();
    }

    private void PlayGoal()
    {
        _goalAudio?.Play();

        // VFX opcional en la posicion de la pelota (donde se hizo el gol)
        if (_scoreVFX != null && _ballTransform != null)
            Instantiate(_scoreVFX, _ballTransform.position, Quaternion.identity);
    }

    // "Punch" de escala en el texto del marcador: crece y vuelve a su tamano normal.
    // No depende de ningun asset externo, funciona apenas se llama.
    private IEnumerator ScorePunch(TMP_Text text)
    {
        Vector3 originalScale = text.transform.localScale;
        Vector3 punchTarget = originalScale * _punchScale;

        float t = 0f;
        while (t < _punchDuration)
        {
            t += Time.deltaTime;
            text.transform.localScale = Vector3.Lerp(originalScale, punchTarget, t / _punchDuration);
            yield return null;
        }

        t = 0f;
        while (t < _punchDuration)
        {
            t += Time.deltaTime;
            text.transform.localScale = Vector3.Lerp(punchTarget, originalScale, t / _punchDuration);
            yield return null;
        }

        text.transform.localScale = originalScale;
    }

    public void Restart()
    {
        _paddle1Transform.position = new Vector2(_paddle1Transform.position.x, 0);
        _paddle2Transform.position = new Vector2(_paddle2Transform.position.x, 0);
        _ballTransform.position = Vector2.zero;
        // NO llama Launch() aca - lo maneja la coroutine de Ball
    }
}
