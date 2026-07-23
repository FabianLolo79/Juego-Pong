using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panel de pausa")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;

    [Header("Botones del panel de pausa")]
    [SerializeField] private Button _btnContinue;
    [SerializeField] private Button _btnExitToMenu;

    [Tooltip("Debe coincidir EXACTO con el nombre de tu escena de menu.")]
    [SerializeField] private string _menuSceneName = "Menu";

    private bool _isPaused;

    private void Awake()
    {
        if (_btnContinue != null) _btnContinue.onClick.AddListener(Resume);
        if (_btnExitToMenu != null) _btnExitToMenu.onClick.AddListener(ExitToMainMenu);
    }

    private void Start()
    {
        if (_pausePanel != null)
            _pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_pauseKey))
            TogglePause();
    }

    public void TogglePause()
    {
        if (_isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        if (_pausePanel != null)
            _pausePanel.SetActive(true);
    }

    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        if (_pausePanel != null)
            _pausePanel.SetActive(false);
    }

    public void ExitToMainMenu()
    {
        SceneFlow.ExitToMenu(_menuSceneName);
    }
}
