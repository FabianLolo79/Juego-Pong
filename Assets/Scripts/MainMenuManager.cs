using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        StartGame();
    }

    public void SelectRightRed()
    {
        MenuConfig.PlayerIsLeft = false;
        StartGame();
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