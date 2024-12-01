using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    public int ammoAmountPerGun = 999; // Amount of ammo added to each gun type
    public GameObject interactionPrompt; // UI prompt

    private bool isPlayerNearby = false;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) // Player presses 'E'
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.RefillAmmo(ammoAmountPerGun); // Refill ammo
                //Destroy(transform.parent.gameObject); // Destroy the entire ammo pack
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true); // Show prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false); // Hide prompt
        }
    }
}
