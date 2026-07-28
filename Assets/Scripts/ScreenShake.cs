using System.Collections;
using UnityEngine;

// Sacude la camara cuando hay un gol. Se suscribe a GameManager.OnGoalScored -
// mismo mecanismo de Observer que ya usamos en GoalAudioPlayer y ConfettiSpawner.
// Poner este script en la Main Camera.
public class ScreenShake : MonoBehaviour
{
    [Tooltip("Duracion del shake. Por defecto 3s, para que coincida con el sfx_gol.")]
    [SerializeField] private float _duration = 3f;
    [SerializeField] private float _magnitude = 0.15f;

    private Vector3 _originalLocalPos;
    private Coroutine _shakeRoutine;

    private void Awake()
    {
        _originalLocalPos = transform.localPosition;
    }

    private void OnEnable()
    {
        GameManager.OnGoalScored += HandleGoalScored;
    }

    private void OnDisable()
    {
        GameManager.OnGoalScored -= HandleGoalScored;
    }

    private void HandleGoalScored()
    {
        // Si ya habia un shake en curso (ej. gol muy seguido de otro), lo cortamos
        // y arrancamos de nuevo desde la posicion original, para no acumular offset.
        if (_shakeRoutine != null)
        {
            StopCoroutine(_shakeRoutine);
            transform.localPosition = _originalLocalPos;
        }

        _shakeRoutine = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;

            // El shake se va atenuando con el tiempo, para que termine suave y no de un frenazo
            float damper = 1f - (elapsed / _duration);
            float offsetX = Random.Range(-1f, 1f) * _magnitude * damper;
            float offsetY = Random.Range(-1f, 1f) * _magnitude * damper;

            transform.localPosition = _originalLocalPos + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        transform.localPosition = _originalLocalPos;
        _shakeRoutine = null;
    }
}
