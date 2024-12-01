using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    public int ammoAmountPerGun = 30; // Amount of ammo added to each gun type
    public GameObject interactionPrompt; // Reference to the UI prompt

    private bool isPlayerNearby = false;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) // Player presses 'E' to interact
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.RefillAmmo(ammoAmountPerGun); // Refill the player's ammo
                Destroy(gameObject); // Destroy the ammo pack after use
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true); // Show prompt
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false); // Hide prompt
            }
        }
    }
}
