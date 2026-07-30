using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    // Los "megafonos": cualquier script se puede suscribir sin que GameManager sepa que existen.
    // string = el mensaje a mostrar. En el temporal, el float es cuantos segundos dura.
    public static event Action<string> OnPermanentMessage;
    public static event Action<string, float> OnTemporaryMessage;
    public static event Action OnGoalScored; // cada vez que alguien anota (incluido el gol que gana el partido)
    public static event Action OnMatchWon;    // ademas del gol, el "extra" de festejo por ganar

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

    [Header("Panel de fin de partido")]
    [Tooltip("Panel oculto por defecto. Se activa solo cuando alguien gana.")]
    [SerializeField] private GameObject _matchEndPanel;
    [SerializeField] private Button _btnRestartMatch;
    [SerializeField] private Button _btnExitToMenuFromEnd;
    [Tooltip("Boton que se selecciona automaticamente al abrir el panel, para navegar con teclado/gamepad.")]
    [SerializeField] private Button _firstSelectedMatchEnd;
    [Tooltip("Debe coincidir EXACTO con el nombre de tu escena de menu.")]
    [SerializeField] private string _menuSceneName = "Menu";

    [Header("Audio")]
    [SerializeField] private AudioSource _crowdGameAudio;
    [SerializeField] private AudioSource _ambientGameAudio;

    [Header("VFX - Feedback de gol (marcador)")]
    [SerializeField] private float _punchScale = 1.3f;
    [SerializeField] private float _punchDuration = 0.12f;

    private int _paddleScore1;
    private int _paddleScore2;
    private bool _matchEnded;
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<GameManager>();
            return instance;
        }
    }

    // Ball.cs consulta esto antes de relanzar: si el partido termino, la pelota queda quieta
    public bool IsMatchOver => _matchEnded;

    // Expuesto solo para que PauseManager pueda detectar si por error comparten el mismo panel
    public GameObject MatchEndPanelRef => _matchEndPanel;

    private void Awake()
    {
        if (_btnRestartMatch != null) _btnRestartMatch.onClick.AddListener(RestartMatch);
        if (_btnExitToMenuFromEnd != null) _btnExitToMenuFromEnd.onClick.AddListener(ExitToMenu);

        // Avisos de configuracion para detectar en el Editor lo que falta conectar,
        // en vez de que "no pase nada" en silencio.
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
        _ambientGameAudio?.Play();

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
        OnGoalScored?.Invoke();
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
            OnTemporaryMessage?.Invoke($"EMPATE {_paddleScore1}-{_paddleScore2}. GOL DE ORO: el proximo gol define el partido!", 2f);
        }
    }

    private void EndMatch(string winnerName)
    {
        _matchEnded = true;

        OnPermanentMessage?.Invoke($"GANO {winnerName}!");
        OnMatchWon?.Invoke();

        if (_matchEndPanel != null)
            _matchEndPanel.SetActive(true);

        // Sin esto, el teclado/gamepad no tiene de donde arrancar a navegar el panel
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            if (_firstSelectedMatchEnd != null)
                EventSystem.current.SetSelectedGameObject(_firstSelectedMatchEnd.gameObject);
        }
    }

    // Recarga la escena actual (Game) para volver a jugar desde 0 - forma mas simple y
    // confiable de resetear marcador, pelota, paddles y estado interno todo junto.
    public void RestartMatch()
    {
        Debug.Log("[GameManager] RestartMatch() -> recargando escena Game (esto borra el marcador)");
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

    public void Restart()
    {
        _paddle1Transform.position = new Vector2(_paddle1Transform.position.x, 0);
        _paddle2Transform.position = new Vector2(_paddle2Transform.position.x, 0);
        _ballTransform.position = Vector2.zero;
        // NO llama Launch() aca - lo maneja la coroutine de Ball
    }
}