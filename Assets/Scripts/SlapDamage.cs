using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float damage = 1f;
    public float knockbackStrength = 5f;  // Increase for stronger effect
    public float verticalBoost = 3f;  // Controls how much the player jumps
    private Renderer playerRenderer;
    private Color originalColor;
    public float flashDuration = 0.2f;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // Apply damage
            player.hitCount += damage;

            // Apply knockback
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                knockbackDirection.y = 0; // Ensure knockback is mostly horizontal
                Vector3 finalForce = (knockbackDirection * knockbackStrength) + (Vector3.up * verticalBoost);
                rb.AddForce(finalForce, ForceMode.Impulse);
                if (player.hitCount == player.maxHitCount)
                {



                    playerRenderer = other.GetComponent<Renderer>();
                    originalColor = playerRenderer.material.color;


                    TakeDamage();
                    StopAllCoroutines();
                }
            }

            Debug.Log("Player Hit: " + other.name + " | Hits Taken: " + player.hitCount);
        }
        
    }
    public void TakeDamage()
    {
        StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        playerRenderer.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        playerRenderer.material.color = originalColor;
        playerRenderer.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        playerRenderer.material.color = originalColor;
        playerRenderer.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        playerRenderer.material.color = originalColor;
    }
}
