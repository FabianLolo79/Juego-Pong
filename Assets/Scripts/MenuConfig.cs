public static class MenuConfig
{
    public static bool IsSinglePlayer;
    public static bool PlayerIsLeft; // true = azul izq, false = rojo der
    public static bool IsPvP => !IsSinglePlayer;
}
