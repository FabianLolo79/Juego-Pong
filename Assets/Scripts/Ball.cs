using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float _initialVelicity = 4f;
    [SerializeField] private float _velocityMultiplier = 1.1f;
    private Rigidbody2D _ballRb;
    private AudioSource _ballAudioSource;

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
            _ballAudioSource.Play(); // - esto es lo único nuevo
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Goal1"))
        {
            GameManager.Instance.PaddleScored2();
            GameManager.Instance.Restart();
            Launch();
        }
        else if (collision.gameObject.CompareTag("Goal2"))
        {
            GameManager.Instance.PaddleScored1();
            GameManager.Instance.Restart();
            Launch();
        }
    }
}