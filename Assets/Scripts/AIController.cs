using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIController : MonoBehaviour
{
    // Si queres que la IA sea mas facil o mas dificil,
    // ajusta _errorMargin (mas alto = mas tonta) o
    // _speed (mas bajo = mas lenta).
    [SerializeField] private float _speed = 6f;
    [SerializeField] private float _reactionDelay = 0.15f;
    [SerializeField] private float _errorMargin = 0.1f;

    private Rigidbody2D _rb;
    private Transform _ballTransform;
    private float _reactionTimer;
    private float _targetY;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        _rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        _ballTransform = GameObject.FindWithTag("Ball")?.transform;

        // Arranca apuntando a su propia posicion para no "saltar" en el primer frame
        _targetY = _rb.position.y;
    }

    void FixedUpdate()
    {
        if (_ballTransform == null) return;

        // La IA "reacciona" con delay: SOLO actualiza el objetivo cada _reactionDelay segundos
        // (esto simula que no ve la pelota en tiempo real, sin frenar el movimiento)
        _reactionTimer += Time.fixedDeltaTime;
        if (_reactionTimer >= _reactionDelay)
        {
            _reactionTimer = 0f;

            float ballY = _ballTransform.position.y;
            float error = Random.Range(-_errorMargin, _errorMargin);
            _targetY = ballY + error;
        }

        // El movimiento se ejecuta en TODOS los FixedUpdate, sin cortes,
        // usando el ultimo objetivo calculado. Esto elimina el efecto "entrecortado".
        Vector2 newPos = _rb.position;
        newPos.y = Mathf.MoveTowards(newPos.y, _targetY, _speed * Time.fixedDeltaTime);
        _rb.MovePosition(newPos);
    }
}
