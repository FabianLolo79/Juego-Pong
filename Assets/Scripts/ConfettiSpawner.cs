using System.Collections;
using UnityEngine;

// Dispara el confetti. No conoce a GameManager mas alla de sus dos eventos.
public class ConfettiSpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem _confettiVFX;
    [SerializeField] private int _burstsPerGoal = 3;
    [SerializeField] private int _burstsOnWin = 6;
    [SerializeField] private float _spreadDelay = 0.12f;
    [SerializeField] private Vector2 _areaMin = new Vector2(-6f, -3.5f);
    [SerializeField] private Vector2 _areaMax = new Vector2(6f, 3.5f);

    private void Awake()
    {
        if (_confettiVFX == null)
            Debug.LogWarning("[ConfettiSpawner] Falta asignar _confettiVFX en el Inspector.");
    }

    private void OnEnable()
    {
        GameManager.OnGoalScored += HandleGoalScored;
        GameManager.OnMatchWon += HandleMatchWon;
    }

    private void OnDisable()
    {
        GameManager.OnGoalScored -= HandleGoalScored;
        GameManager.OnMatchWon -= HandleMatchWon;
    }

    private void HandleGoalScored()
    {
        StartCoroutine(Fireworks(_burstsPerGoal));
    }

    private void HandleMatchWon()
    {
        StartCoroutine(Fireworks(_burstsOnWin));
    }

    // Dispara el mismo prefab de confetti en varias posiciones aleatorias, con un pequeno
    // delay entre cada uno para que se sienta como una tanda de fuegos artificiales.
    private IEnumerator Fireworks(int burstCount)
    {
        if (_confettiVFX == null) yield break;

        for (int i = 0; i < burstCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(_areaMin.x, _areaMax.x),
                Random.Range(_areaMin.y, _areaMax.y)
            );

            ParticleSystem instance = Instantiate(_confettiVFX, pos, Quaternion.identity);

            // Resguardo extra: si el prefab no tiene ParticleAutoDestroy o el Stop Action
            // no esta en Callback, esto lo destruye igual en base a su duracion real.
            float lifetime = instance.main.duration + instance.main.startLifetime.constantMax + 0.5f;
            Destroy(instance.gameObject, lifetime);

            yield return new WaitForSeconds(_spreadDelay);
        }
    }
}
