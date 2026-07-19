using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _playPanel;
    [SerializeField] private GameObject _characterPanel;
    [SerializeField] private GameObject _creditsPanel;

    [Header("Botones - Main Panel")]
    [SerializeField] private Button _btnPlay;
    [SerializeField] private Button _btnCredits;
    [SerializeField] private Button _btnExit;

    [Header("Botones - Play Panel")]
    [SerializeField] private Button _btnSingle;
    [SerializeField] private Button _btnPvP;
    [SerializeField] private Button _btnBackFromPlay;

    [Header("Botones - Character Panel")]
    [SerializeField] private Button _btnLeftBlue;
    [SerializeField] private Button _btnRightRed;
    [SerializeField] private Button _btnBackFromCharacter;

    [Header("Nombres - Character Panel")]
    [Tooltip("Nombre del jugador Azul/Izquierda (Player 1). Siempre visible.")]
    [SerializeField] private TMP_InputField _inputNameLeft;
    [Tooltip("Nombre del jugador Rojo/Derecha (Player 2). Siempre visible.")]
    [SerializeField] private TMP_InputField _inputNameRight;

    [Header("Botones - Credits Panel")]
    [SerializeField] private Button _btnBackFromCredits;

    [Header("Audio")]
    [SerializeField] private AudioSource _crowdMenuAudio;

    private void Awake()
    {
        // Conecta TODOS los botones automaticamente
        if (_btnPlay != null) _btnPlay.onClick.AddListener(ShowPlay);
        if (_btnCredits != null) _btnCredits.onClick.AddListener(ShowCredits);
        if (_btnExit != null) _btnExit.onClick.AddListener(ExitGame);

        if (_btnSingle != null) _btnSingle.onClick.AddListener(SelectSinglePlayer);
        if (_btnPvP != null) _btnPvP.onClick.AddListener(SelectPvP);
        if (_btnBackFromPlay != null) _btnBackFromPlay.onClick.AddListener(ShowMain);

        if (_btnLeftBlue != null) _btnLeftBlue.onClick.AddListener(SelectLeftBlue);
        if (_btnRightRed != null) _btnRightRed.onClick.AddListener(SelectRightRed);
        if (_btnBackFromCharacter != null) _btnBackFromCharacter.onClick.AddListener(ShowPlay);

        if (_btnBackFromCredits != null) _btnBackFromCredits.onClick.AddListener(ShowMain);
    }

    private void Start()
    {
        if (_playPanel != null) _playPanel.SetActive(false);
        if (_characterPanel != null) _characterPanel.SetActive(false);
        if (_creditsPanel != null) _creditsPanel.SetActive(false);

        ShowMain();

        if (_crowdMenuAudio != null)
            _crowdMenuAudio.Play();
    }

    public void ShowMain() => SetPanel(_mainPanel);
    public void ShowPlay() => SetPanel(_playPanel);
    public void ShowCharacter() => SetPanel(_characterPanel);
    public void ShowCredits() => SetPanel(_creditsPanel);

    private void SetPanel(GameObject activePanel)
    {
        if (_mainPanel != null) _mainPanel.SetActive(false);
        if (_playPanel != null) _playPanel.SetActive(false);
        if (_characterPanel != null) _characterPanel.SetActive(false);
        if (_creditsPanel != null) _creditsPanel.SetActive(false);

        if (activePanel != null)
            activePanel.SetActive(true);
    }

    public void SelectSinglePlayer()
    {
        MenuConfig.IsSinglePlayer = true;
        ShowCharacter();
    }

    public void SelectPvP()
    {
        MenuConfig.IsSinglePlayer = false;
        ShowCharacter();
    }

    public void SelectLeftBlue()
    {
        MenuConfig.PlayerIsLeft = true;
        AssignNames();
        StartGame();
    }

    public void SelectRightRed()
    {
        MenuConfig.PlayerIsLeft = false;
        AssignNames();
        StartGame();
    }

    // Vuelca lo escrito en los inputs a MenuConfig antes de cargar la escena Game
    private void AssignNames()
    {
        string leftFieldText = GetTrimmedOrNull(_inputNameLeft);
        string rightFieldText = GetTrimmedOrNull(_inputNameRight);

        if (MenuConfig.IsSinglePlayer)
        {
            // Cada input pertenece SIEMPRE a su lado. El lado que elijas jugar usa
            // lo que escribiste en SU campo; el otro lado pasa a ser CPU (se ignora
            // lo que hayas escrito ahi, si escribiste algo).
            if (MenuConfig.PlayerIsLeft)
            {
                MenuConfig.LeftPlayerName = leftFieldText ?? "Player Azul";
                MenuConfig.RightPlayerName = "CPU";
            }
            else
            {
                MenuConfig.LeftPlayerName = "CPU";
                MenuConfig.RightPlayerName = rightFieldText ?? "Player Rojo";
            }
        }
        else
        {
            MenuConfig.LeftPlayerName = leftFieldText ?? "Player Azul";
            MenuConfig.RightPlayerName = rightFieldText ?? "Player Rojo";
        }

        Debug.Log($"[MainMenuManager] PlayerIsLeft={MenuConfig.PlayerIsLeft} | Izquierda(Azul)={MenuConfig.LeftPlayerName} | Derecha(Rojo)={MenuConfig.RightPlayerName}");
    }

    private string GetTrimmedOrNull(TMP_InputField field)
    {
        return (field != null && !string.IsNullOrWhiteSpace(field.text))
            ? field.text.Trim()
            : null;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}