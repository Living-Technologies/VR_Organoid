using UnityEngine;

public class TriggerParticle : MonoBehaviour
{
    public ParticleSystem particleSystem; // Reference to the Particle System

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the one you want
        if (other.CompareTag("organoid")) // Replace "Player" with the tag of the object
        {
            particleSystem.Play(); // Start the Particle System
            Debug.Log("Particle system started!");
        }
    }
}
