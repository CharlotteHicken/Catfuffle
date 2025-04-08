using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float damage = 1f;
    public float knockbackStrength = 10f;  // Increase for stronger effect
    public float verticalBoost = 2f;  // Controls how much the player jumps
    private SkinnedMeshRenderer playerRenderer;
    Material originalMaterial;
    public Material redMaterial;
    public float flashDuration = 0.2f;
    public AudioManager audioManager;
    public GameObject smackDisplayF1;
    public PlayerController scoreKeeper;
    public Material backupMaterial;
    PlayerController player;
    bool smackBool = false;
    bool isFlashing = false;
    Coroutine Flashyred;
    // public GameObject smackDisplayF2;

    float timer;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void Update()
    {

        
        //Debug.Log(timer);
        if (smackBool == true)
        {
            timer += Time.deltaTime;  // Keep increasing the timer
        }

        if (timer > 1)  // Check if the timer has passed 3 seconds
        {
            smackDisplayF1.SetActive(false);
            if (isFlashing == false)
            {
                playerRenderer.material = player.playerMat;
            }
            timer = 0;  // Reset the timer
        }
       

    }
    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlayerController>();
        playerRenderer = other.GetComponentInChildren<SkinnedMeshRenderer>();
        originalMaterial = playerRenderer.material;
        backupMaterial = playerRenderer.material;
        if (player != null)
        {
           
            // Apply damage
            player.hitCount += damage;
            audioManager.PlaySFX(audioManager.Hit);
            smackDisplayF1.SetActive(true);
            smackBool = true;
            player.slider.value -= damage;

            // Apply knockback
            
            // Pass the slapper's PlayerController to the PEliminator method


            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Assign the eliminator (this is the player who hits)
               
                player.eliminatedBy = scoreKeeper;
                player.otherPlayer = player; // Set the eliminatedBy reference
               // scoreKeeper.otherPlayer = player.otherPlayer;
            
                if (playerRenderer!=null && !isFlashing)
                {
                    isFlashing = true;
                    Flashyred = StartCoroutine(FlashRed());

                }

                //StopCoroutine(Flashyred);
                if (player.otherPlayer.isDying == true)
                {
                    scoreKeeper.hasScored = false;
                    player.otherPlayer = null;
                    player.eliminatedBy = null;
                    scoreKeeper.otherPlayer = null;
                }
             
                player.CheckElimination();

                Vector3 knockbackDirection = (transform.position + transform.forward).normalized;
                Vector3 finalForce = (knockbackDirection * knockbackStrength) + (Vector3.up * verticalBoost);
                rb.AddForce(finalForce, ForceMode.Impulse);


            }
        }
    }

    public IEnumerator FlashRed()
    {
        // Store the original material (color)
    

        // Set the player to the red material
        playerRenderer.material = redMaterial;

        // Wait for the duration of the flash
        yield return new WaitForSeconds(0.1f);
        if (playerRenderer.material == redMaterial)
        {
            playerRenderer.material = originalMaterial;
        }
        playerRenderer.material = backupMaterial;
       
        // Restore the original material (color)
     
        isFlashing = false;

        // Reset the flag when the flashing is complete
    }

}

