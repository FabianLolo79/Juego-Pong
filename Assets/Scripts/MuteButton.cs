using UnityEngine;
using UnityEngine.UI;

// Poner este script en el boton de parlante. Sirve igual en la escena Menu y en Game:
// como el mute es global (AudioSettings/AudioListener.volume), no importa en que
// escena lo toques, va a aplicar a toda la app.
public class MuteButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [Tooltip("El Image del icono de parlante (el que cambia entre sonido on/off).")]
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _iconSoundOn;
    [SerializeField] private Sprite _iconSoundOff;

    private void Awake()
    {
        if (_button != null)
            _button.onClick.AddListener(ToggleMute);
        else
            Debug.LogWarning("[MuteButton] Falta asignar _button en el Inspector.");
    }

    private void Start()
    {
        // Si venis de la otra escena con el mute ya activado, el icono se sincroniza solo
        RefreshIcon();
    }

    private void ToggleMute()
    {
        AudioSettings.ToggleMute();
        RefreshIcon();
    }

    private void RefreshIcon()
    {
        if (_icon == null || _iconSoundOn == null || _iconSoundOff == null) return;
        _icon.sprite = AudioSettings.IsMuted ? _iconSoundOff : _iconSoundOn;
    }
}
