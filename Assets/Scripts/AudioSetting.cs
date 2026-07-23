using UnityEngine;

// Controla el mute de TODO el audio del juego (crowd, gol, ball, paddle, botones, etc)
// en un solo lugar, sin necesitar una referencia a cada AudioSource individual.
// AudioListener.volume es un interruptor global de Unity: afecta a cualquier AudioSource
// que exista en la escena actual y en las que se carguen despues.
public static class AudioSettings
{
    private static bool _isMuted;
    public static bool IsMuted => _isMuted;

    public static void SetMuted(bool muted)
    {
        _isMuted = muted;
        AudioListener.volume = muted ? 0f : 1f;
    }

    public static void ToggleMute()
    {
        SetMuted(!_isMuted);
    }
}
