using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    [Header("Panel de pausa")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;

    [Header("Botones del panel de pausa")]
    [SerializeField] private Button _btnResume;
    [SerializeField] private Button _btnExitToMenu;
    [Tooltip("Boton que se selecciona automaticamente al abrir el panel, para navegar con teclado/gamepad.")]
    [SerializeField] private Button _firstSelectedPause;

    [Tooltip("Debe coincidir EXACTO con el nombre de tu escena de menu.")]
    [SerializeField] private string _menuSceneName = "Menu";

    private bool _isPaused;

    private void Awake()
    {
        if (_btnResume != null) _btnResume.onClick.AddListener(Resume);
        if (_btnExitToMenu != null) _btnExitToMenu.onClick.AddListener(ExitToMainMenu);
    }

    private void Start()
    {
        if (_pausePanel != null)
            _pausePanel.SetActive(false);

        // Deteccion automatica: si el panel de pausa es el MISMO GameObject que el panel
        // de fin de partido, los botones de ambos sistemas van a estar mezclados en un
        // solo Button, y clickear uno dispara los listeners de los dos.
        GameManager gm = GameManager.Instance;
        if (gm != null && _pausePanel != null && _pausePanel == gm.MatchEndPanelRef)
        {
            Debug.LogError("[PauseManager] _pausePanel y el _matchEndPanel del GameManager son EL MISMO GameObject. " +
                "Tienen que ser paneles distintos, si no los botones se pisan entre si (por eso 'Continuar' reinicia el marcador).");
        }
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
        Debug.Log("[PauseManager] Pause()");
        if (_pausePanel != null)
            _pausePanel.SetActive(true);

        // Sin esto, el teclado/gamepad no tiene de donde arrancar a navegar el panel
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            if (_firstSelectedPause != null)
                EventSystem.current.SetSelectedGameObject(_firstSelectedPause.gameObject);
        }
    }

    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("[PauseManager] Resume()");
        if (_pausePanel != null)
            _pausePanel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void ExitToMainMenu()
    {
        SceneFlow.ExitToMenu(_menuSceneName);
    }
}