using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float _initialVelicity = 5f;
    [SerializeField] private float _velocityMultiplier = 1.1f;
    [SerializeField] private float _goalWaitTime = 3f; // antes 9f - tiempo muerto tras el gol
    private Rigidbody2D _ballRb;
    private AudioSource _ballAudioSource;
    private bool _isWaiting; // bloquea multiples triggers

    void Start()
    {
        _ballAudioSource = GetComponent<AudioSource>();
        _ballRb = GetComponent<Rigidbody2D>();
        Launch();
    }

    private void Launch()
    {
        float xVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        float yVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        _ballRb.linearVelocity = new Vector2(xVelocity, yVelocity) * _initialVelicity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            _ballRb.linearVelocity *= _velocityMultiplier;
            if (_ballAudioSource != null)
                _ballAudioSource.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isWaiting) return; // evita doble gol

        if (collision.gameObject.CompareTag("Goal1"))
        {
            GameManager.Instance.PaddleScored2();
            StartCoroutine(GoalSequence());
        }
        else if (collision.gameObject.CompareTag("Goal2"))
        {
            GameManager.Instance.PaddleScored1();
            StartCoroutine(GoalSequence());
        }
    }

    private IEnumerator GoalSequence()
    {
        _isWaiting = true;

        // 1. Para la pelota
        _ballRb.linearVelocity = Vector2.zero;
        _ballRb.isKinematic = true; // la congela

        // 2. Resetea posiciones (paddles al centro, pelota al medio)
        GameManager.Instance.Restart();

        // 3. Espera el tiempo configurado tras el gol (SFX + feedback visual)
        yield return new WaitForSeconds(_goalWaitTime);

        // Si alguien ya gano el partido, la pelota se queda quieta (no relanza)
        if (GameManager.Instance.IsMatchOver)
            yield break;

        // 4. Descongela y lanza
        _ballRb.isKinematic = false;
        Launch();

        _isWaiting = false;
    }
}