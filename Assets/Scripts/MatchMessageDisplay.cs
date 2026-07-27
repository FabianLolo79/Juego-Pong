using System.Collections;
using UnityEngine;
using TMPro;

// Este script NO conoce a GameManager mas alla de sus eventos. GameManager, a su vez,
// no sabe que este script existe. Esa desconexion es el punto entero del patron Observer.
public class MatchMessageDisplay : MonoBehaviour
{
    [Tooltip("Texto SIEMPRE visible en pantalla: avisa empate/gol de oro y el ganador final.")]
    [SerializeField] private TMP_Text _messageText;

    private Coroutine _hideRoutine;

    private void Awake()
    {
        if (_messageText == null)
            Debug.LogWarning("[MatchMessageDisplay] Falta asignar _messageText en el Inspector.");
    }

    // Aca pasa la magia de Observer: nos suscribimos cuando este objeto se activa...
    private void OnEnable()
    {
        GameManager.OnPermanentMessage += ShowPermanentMessage;
        GameManager.OnTemporaryMessage += ShowTemporaryMessage;
    }

    // ...y nos desuscribimos cuando se desactiva. Si te olvidas de esto, el objeto queda
    // "escuchando" un evento aunque ya no exista mas en la escena (fuga de memoria / bugs raros).
    private void OnDisable()
    {
        GameManager.OnPermanentMessage -= ShowPermanentMessage;
        GameManager.OnTemporaryMessage -= ShowTemporaryMessage;
    }

    private void Start()
    {
        SetText(string.Empty);
    }

    private void ShowPermanentMessage(string msg)
    {
        // Si habia un "esconder mensaje" pendiente de un empate previo, lo cancelamos
        // para que no borre este mensaje final.
        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }

        SetText(msg);
    }

    private void ShowTemporaryMessage(string msg, float seconds)
    {
        SetText(msg);

        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(HideAfter(seconds));
    }

    private IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetText(string.Empty);
        _hideRoutine = null;
    }

    private void SetText(string msg)
    {
        if (_messageText != null)
            _messageText.text = msg;
    }
}
