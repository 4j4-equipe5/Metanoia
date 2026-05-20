using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    void Awake()
    {
        animator = GetComponent<Animator>();
        
        // Fetch all rigidbodies and colliders in the children (the skeleton bones)
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Start with the ragdoll deactivated so the character can walk normally
        DeactivateRagdoll();
    }

    // Call this when the character dies or gets hit by a shotgun (Stun)
    public void ActivateRagdoll(Vector3 forceDirection, float forceMagnitude)
    {
        animator.enabled = false; // Turn off animations so physics can take over

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = false; // Allow physics to move the bone
            
            // Optional: Push the ragdoll back based on the weapon's force
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = true; // Enable bone collisions with the ground/walls
        }
    }

    public void DeactivateRagdoll()
    {
        animator.enabled = true; // Give control back to the Animator

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = true; // Stop physics from moving the bones
        }

        foreach (var col in ragdollColliders)
        {
            // We disable child colliders so they don't fight with the main capsule collider
            if (col.gameObject != this.gameObject) 
            {
                col.enabled = false;
            }
        }
    }
}