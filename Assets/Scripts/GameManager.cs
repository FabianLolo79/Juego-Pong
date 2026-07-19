using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _paddleScore1Text;
    [SerializeField] private TMP_Text _paddleScore2Text;
    [SerializeField] private Transform _paddle1Transform;
    [SerializeField] private Transform _paddle2Transform;
    [SerializeField] private Transform _ballTransform;

    [Header("Nombres de jugadores (HUD)")]
    [Tooltip("Muestra MenuConfig.LeftPlayerName (tu nombre si jugaste Azul, o CPU).")]
    [SerializeField] private TMP_Text _leftPlayerNameText;
    [Tooltip("Muestra MenuConfig.RightPlayerName (tu nombre si jugaste Rojo, o CPU).")]
    [SerializeField] private TMP_Text _rightPlayerNameText;

    [Header("Reglas del partido")]
    [SerializeField] private int _scoreToWin = 5;
    [Tooltip("Texto SIEMPRE visible en pantalla: avisa empate/gol de oro y el ganador final.")]
    [SerializeField] private TMP_Text _matchMessageText;

    [Header("Panel de fin de partido")]
    [Tooltip("Panel oculto por defecto. Se activa solo cuando alguien gana.")]
    [SerializeField] private GameObject _matchEndPanel;
    [SerializeField] private Button _btnRestartMatch;
    [SerializeField] private Button _btnExitToMenuFromEnd;
    [Tooltip("Debe coincidir EXACTO con el nombre de tu escena de menu.")]
    [SerializeField] private string _menuSceneName = "Menu";

    [Header("Audio")]
    [SerializeField] private AudioSource _crowdGameAudio;
    [SerializeField] private AudioSource _inStaduimAudio;
    [SerializeField] private AudioSource _goalAudio;

    [Header("VFX - Feedback de gol (marcador)")]
    [SerializeField] private float _punchScale = 1.3f;
    [SerializeField] private float _punchDuration = 0.12f;

    [Header("VFX - Confetti (estilo fuegos artificiales)")]
    [SerializeField] private ParticleSystem _confettiVFX;
    [SerializeField] private int _confettiBurstsPerGoal = 3;
    [SerializeField] private int _confettiBurstsOnWin = 6;
    [SerializeField] private float _confettiSpreadDelay = 0.12f;
    [SerializeField] private Vector2 _confettiAreaMin = new Vector2(-6f, -3.5f);
    [SerializeField] private Vector2 _confettiAreaMax = new Vector2(6f, 3.5f);

    private int _paddleScore1;
    private int _paddleScore2;
    private bool _matchEnded;
    private Coroutine _hideMessageRoutine;
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

    // Ball.cs consulta esto antes de relanzar: si el partido termino, la pelota queda quieta
    public bool IsMatchOver => _matchEnded;

    private void Awake()
    {
        if (_btnRestartMatch != null) _btnRestartMatch.onClick.AddListener(RestartMatch);
        if (_btnExitToMenuFromEnd != null) _btnExitToMenuFromEnd.onClick.AddListener(ExitToMenu);

        // Avisos de configuracion para detectar en el Editor lo que falta conectar,
        // en vez de que "no pase nada" en silencio.
        if (_matchMessageText == null)
            Debug.LogWarning("[GameManager] Falta asignar _matchMessageText en el Inspector: el aviso de empate/ganador no se va a ver.");
        if (_leftPlayerNameText == null)
            Debug.LogWarning("[GameManager] Falta asignar _leftPlayerNameText en el Inspector: no se va a ver el nombre del jugador Azul.");
        if (_rightPlayerNameText == null)
            Debug.LogWarning("[GameManager] Falta asignar _rightPlayerNameText en el Inspector: no se va a ver el nombre del jugador Rojo.");
        if (_matchEndPanel == null)
            Debug.LogWarning("[GameManager] Falta asignar _matchEndPanel en el Inspector: no vas a tener botones de Reiniciar/Salir al terminar el partido.");
    }

    void Start()
    {
        _crowdGameAudio?.Play();
        _inStaduimAudio?.Play();
        if (_matchMessageText != null)
            _matchMessageText.text = string.Empty;

        if (_leftPlayerNameText != null)
            _leftPlayerNameText.text = MenuConfig.LeftPlayerName;
        if (_rightPlayerNameText != null)
            _rightPlayerNameText.text = MenuConfig.RightPlayerName;

        if (_matchEndPanel != null)
            _matchEndPanel.SetActive(false);
    }

    public void PaddleScored1()
    {
        if (_matchEnded) return;

        _paddleScore1++;
        _paddleScore1Text.text = _paddleScore1.ToString();
        Debug.Log($"[GameManager] PaddleScored1() -> _paddleScore1={_paddleScore1} (confirmado: DERECHA/ROJO)");
        StartCoroutine(ScorePunch(_paddleScore1Text));
        PlayGoal();
        CheckMatchState();
    }

    public void PaddleScored2()
    {
        if (_matchEnded) return;

        _paddleScore2++;
        _paddleScore2Text.text = _paddleScore2.ToString();
        Debug.Log($"[GameManager] PaddleScored2() -> _paddleScore2={_paddleScore2} (confirmado: IZQUIERDA/AZUL)");
        StartCoroutine(ScorePunch(_paddleScore2Text));
        PlayGoal();
        CheckMatchState();
    }

    private void PlayGoal()
    {
        _goalAudio?.Play();
        StartCoroutine(ConfettiFireworks(_confettiBurstsPerGoal));
    }

    // Revisa si hay ganador o si toca avisar "gol de oro" (empate a un gol del limite)
    private void CheckMatchState()
    {
        bool isTiedBeforeWin = _paddleScore1 == _paddleScore2 && _paddleScore1 == _scoreToWin - 1;

        if (_paddleScore1 >= _scoreToWin)
        {
            // Confirmado por test: _paddleScore1 corresponde a DERECHA/ROJO en este proyecto
            EndMatch(MenuConfig.RightPlayerName);
        }
        else if (_paddleScore2 >= _scoreToWin)
        {
            // Confirmado por test: _paddleScore2 corresponde a IZQUIERDA/AZUL en este proyecto
            EndMatch(MenuConfig.LeftPlayerName);
        }
        else if (isTiedBeforeWin)
        {
            return;
            //ShowTemporaryMessage($"EMPATE {_paddleScore1}-{_paddleScore2}. GOL DE ORO: el proximo gol define el partido!", 2f);
        }
    }

    private void EndMatch(string winnerName)
    {
        _matchEnded = true;

        // Si habia un "esconder mensaje" programado por un empate previo, lo cancelamos
        // para que no borre el mensaje de "Gano X" que estamos por mostrar.
        if (_hideMessageRoutine != null)
        {
            StopCoroutine(_hideMessageRoutine);
            _hideMessageRoutine = null;
        }

        ShowMessage($"GANO {winnerName}!");
        StartCoroutine(ConfettiFireworks(_confettiBurstsOnWin));

        if (_matchEndPanel != null)
            _matchEndPanel.SetActive(true);
    }

    private void ShowMessage(string msg)
    {
        if (_matchMessageText != null)
            _matchMessageText.text = msg;
    }

    // Muestra un mensaje que se borra solo despues de "seconds" (usado para el aviso de empate,
    // que es informativo pero el partido sigue jugandose).
    private void ShowTemporaryMessage(string msg, float seconds)
    {
        ShowMessage(msg);

        if (_hideMessageRoutine != null)
            StopCoroutine(_hideMessageRoutine);

        _hideMessageRoutine = StartCoroutine(HideMessageAfter(seconds));
    }

    private IEnumerator HideMessageAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Si mientras tanto el partido termino, no pisamos el mensaje de "Gano X"
        if (!_matchEnded)
            ShowMessage(string.Empty);

        _hideMessageRoutine = null;
    }

    // Recarga la escena actual (Game) para volver a jugar desde 0 - forma mas simple y
    // confiable de resetear marcador, pelota, paddles y estado interno todo junto.
    public void RestartMatch()
    {
        SceneFlow.ExitToMenu(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        SceneFlow.ExitToMenu(_menuSceneName);
    }

    // "Punch" de escala en el texto del marcador: crece y vuelve a su tamano normal.
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

    // Dispara el mismo prefab de confetti en varias posiciones aleatorias, con un pequeno
    // delay entre cada uno para que se sienta como una tanda de fuegos artificiales.
    private IEnumerator ConfettiFireworks(int burstCount)
    {
        if (_confettiVFX == null) yield break;

        for (int i = 0; i < burstCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(_confettiAreaMin.x, _confettiAreaMax.x),
                Random.Range(_confettiAreaMin.y, _confettiAreaMax.y)
            );

            Instantiate(_confettiVFX, pos, Quaternion.identity);

            yield return new WaitForSeconds(_confettiSpreadDelay);
        }
    }

    public void Restart()
    {
        _paddle1Transform.position = new Vector2(_paddle1Transform.position.x, 0);
        _paddle2Transform.position = new Vector2(_paddle2Transform.position.x, 0);
        _ballTransform.position = Vector2.zero;
        // NO llama Launch() aca - lo maneja la coroutine de Ball
    }
}