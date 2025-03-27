using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float damage = 1f;
    public float knockbackStrength = 10f;  // Increase for stronger effect
    public float verticalBoost = 2f;  // Controls how much the player jumps
    private Renderer playerRenderer;
    private Color originalColor;
    public float flashDuration = 0.2f;
    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

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
                Vector3 knockbackDirection = (transform.position + transform.forward).normalized;
                Vector3 finalForce = (knockbackDirection * knockbackStrength + Vector3.forward) + (Vector3.up * verticalBoost);
                rb.AddForce(finalForce, ForceMode.Impulse);
                if (player.hitCount == player.maxHitCount)
                {



                    playerRenderer = other.GetComponent<Renderer>();
                    originalColor = playerRenderer.material.color;


                    TakeDamage();
                    
                }
            }
            StopAllCoroutines();

            Debug.Log("Player Hit: " + other.name + " | Hits Taken: " + player.hitCount);
        }
        
    }
    public void TakeDamage()
    {
        audioManager.PlaySFX(audioManager.Hit);
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
