using UnityEngine;
using UnityEngine.UI;

// Le agrega el SFX de click a TODOS los botones de la escena (incluidos los que estan
// dentro de paneles ocultos) sin tener que conectar nada boton por boton.
// Poner este script UNA vez por escena (Menu y Game), en cualquier GameObject
// (por ejemplo el mismo Canvas), y asignarle el clip sfx_button.
[RequireComponent(typeof(AudioSource))]
public class ButtonClickSfxManager : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSfx;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.clip = _clickSfx;

        // Include: true -> encuentra tambien los botones que estan dentro de paneles
        // desactivados (Play, Character, Credits, Pause, etc), no solo los visibles.
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button button in allButtons)
        {
            button.onClick.AddListener(PlayClickSfx);
        }

        Debug.Log($"[ButtonClickSfxManager] SFX de click conectado a {allButtons.Length} botones.");
    }

    private void PlayClickSfx()
    {
        if (_clickSfx == null) return;
        _audioSource.PlayOneShot(_clickSfx);
    }
}