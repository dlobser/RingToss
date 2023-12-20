using UnityEngine;

public class EmitParticlesOnTrigger : MonoBehaviour
{
    public ParticleSystem particleSystem; // Reference to the Particle System
    public int emitCount = 10; // Number of particles to emit

    void Start()
    {
        // If particleSystem is not assigned, try to get it from the GameObject
        if (particleSystem == null)
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Emit particles when a collider enters the trigger
        if(other.GetComponent<Ring>()!=null){
            // if(other.GetComponent<Item>().customTag==CustomTag.Projectile){
                if (particleSystem != null)
                {
                    particleSystem.Emit(emitCount);
                }
            // }
        }
    }
}