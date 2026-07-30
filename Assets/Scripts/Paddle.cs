using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private bool isPaddle1; // true = izquierda, false = derecha

    private AudioSource _paddleAudio;
    private float yBound = 3.75f;
    private bool _isAI;

    void Start()
    {
        _paddleAudio = GetComponent<AudioSource>();
        if (_paddleAudio != null)
            _paddleAudio.panStereo = isPaddle1 ? -1f : 1f;

        // Aplicar color según configuración del menú
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (isPaddle1)
            sr.color = Color.blue; // izquierda siempre azul
        else
            sr.color = Color.red;  // derecha siempre rojo

        // Determinar si este paddle es IA
        if (MenuConfig.IsSinglePlayer)
        {
            // Si single player y el jugador eligió izquierda (azul), la derecha es IA
            // Si eligió derecha (rojo), la izquierda es IA
            bool playerIsLeft = MenuConfig.PlayerIsLeft;
            _isAI = (isPaddle1 && !playerIsLeft) || (!isPaddle1 && playerIsLeft);

            if (_isAI)
                gameObject.AddComponent<AIController>();
        }
    }

    void Update()
    {
        if (_isAI) return; // La IA maneja el movimiento

        float movement;
        if (isPaddle1)
            movement = Input.GetAxisRaw("Vertical2");
        else
            movement = Input.GetAxisRaw("Vertical");

        Vector2 paddlePosition = transform.position;
        paddlePosition.y = Mathf.Clamp(
            paddlePosition.y + movement * speed * Time.deltaTime,
            -yBound,
            yBound
        );
        transform.position = paddlePosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (_paddleAudio != null)
                _paddleAudio.PlayOneShot(_paddleAudio.clip);
        }
    }
}
