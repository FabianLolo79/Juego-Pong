using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _playPanel;
    [SerializeField] private GameObject _characterPanel;
    [SerializeField] private GameObject _creditsPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource _crowdMenuAudio; // hinchada en menú

    private void Start()
    {
        //ShowMain();
        _crowdMenuAudio.Play();
    }

    //Navegación entre paneles
    private void SetPanel(GameObject activePanel)
    { 
        _mainPanel.SetActive(false);
        _playPanel.SetActive(false);
        _characterPanel.SetActive(false);
        _creditsPanel.SetActive(false);
        activePanel.SetActive(false);
    }

    // Botones de color / posición
    public void SelectLeftBlue() // Juega izquierda (azul)
    {
        MenuConfig.PlayerIsLeft = true;
        StartGame();
    }

    public void SelectRightRed() // Juega derecha (rojo)
    {
        MenuConfig.PlayerIsLeft = false;
        StartGame();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Game"); // tu escena de juego
    }

    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
