using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private bool isPaddle1;

    private AudioSource _paddleAudio;

    private float yBound = 3.75f;

    private void Start()
    {
        _paddleAudio = GetComponent<AudioSource>();

        // Defensa: si no hay AudioSource, avisa en consola pero no crashea
        if (_paddleAudio == null)
        {
            Debug.LogError($"[Paddle] Falta AudioSource en {gameObject.name}. Agregalo desde el Inspector.");
            enabled = false; // Desactiva este script para evitar mas errores
            return;
        }

        // Configura el pan estereo
        _paddleAudio.panStereo = isPaddle1 ? 1f : -1f;
    }

    // Update is called once per frame
    void Update()
    {
        float movement;

        if (isPaddle1)
            movement = Input.GetAxisRaw("Vertical2");
        else
            movement = Input.GetAxisRaw("Vertical");


        Vector2 paddlePosition = transform.position;
        paddlePosition.y = Mathf.Clamp(paddlePosition.y + movement * speed *Time.deltaTime, -yBound, yBound);
        transform.position = paddlePosition;
    }

    // NUEVO: suena solo cuando la pelota golpea este paddle
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _paddleAudio.PlayOneShot(_paddleAudio.clip);
        }
    }
}
