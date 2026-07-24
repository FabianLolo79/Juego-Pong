using UnityEngine;

// Poner este script en el MISMO GameObject que el Particle System del confetti.
// IMPORTANTE: en el Particle System, modulo "Main" -> "Stop Action" tiene que estar
// en "Callback" (no en "None"), si no OnParticleSystemStopped() nunca se llama.
[RequireComponent(typeof(ParticleSystem))]
public class ParticleAutoDestroy : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        Destroy(gameObject);
    }
}
