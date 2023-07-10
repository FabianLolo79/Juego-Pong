using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Ball : MonoBehaviour
{
    [SerializeField] private float initialVelicity = 4f;
    [SerializeField] private float velocityMultiplier = 1.1f;
    private Rigidbody2D ballRb;

    // Start is called before the first frame update
    void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
        Launch();
    }

    private void Launch()
    {
        float xVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        float yVelocity = Random.Range(0, 2) == 0 ? 1 : -1;
        ballRb.velocity =  new Vector2(xVelocity, yVelocity) * initialVelicity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            ballRb.velocity *= velocityMultiplier;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Goal1"))
        {
            GameManager.Instance.PaddleScored2();
            GameManager.Instance.Restart();
            Launch();
        }
        else
        {
            GameManager.Instance.PaddleScored1();
            GameManager.Instance.Restart();
            Launch();
        }
    }

}
