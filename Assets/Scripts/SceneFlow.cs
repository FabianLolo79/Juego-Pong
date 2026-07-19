using UnityEngine;
using UnityEngine.SceneManagement;

// Logica compartida de "salir al menu", usada tanto por PauseManager como por GameManager.
// Son features distintas (pausa vs fin de partido), pero ambas necesitan hacer
// exactamente lo mismo para volver al menu, asi que esa parte vive en un solo lugar.
public static class SceneFlow
{
    public static void ExitToMenu(string menuSceneName)
    {
        Time.timeScale = 1f; // importante: si no, el Menu queda congelado
        SceneManager.LoadScene(menuSceneName);
    }
}
