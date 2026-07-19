public static class MenuConfig
{
    public static bool IsSinglePlayer;
    public static bool PlayerIsLeft; // true = azul izq, false = rojo der
    public static bool IsPvP => !IsSinglePlayer;

    // Nombres de los jugadores. Se completan en el menu (MainMenuManager)
    // y los consume GameManager para el texto de "gano X".
    public static string LeftPlayerName = "Player Azul";
    public static string RightPlayerName = "Player Rojo";
}
