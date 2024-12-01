using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    public int ammoAmountPerGun = 30; // Amount of ammo added to each gun type
    public GameObject interactionPrompt; // Reference to the UI prompt
    public float respawnTime = 3f; // Time in seconds for ammo pack to respawn
    private MeshRenderer meshRenderer;
    private Collider ammoCollider;

    private bool isPlayerNearby = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ammoCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) // Player presses 'E' to interact
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.RefillAmmo(ammoAmountPerGun); // Refill the player's ammo
                HideAmmoPack(); // Hide the ammo pack after use
                StartCoroutine(RespawnAmmoPack()); // Start the respawn coroutine
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactionPrompt != null && meshRenderer.enabled)
            {
                interactionPrompt.SetActive(true); // Show prompt if ammo pack is active
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

    private void HideAmmoPack()
    {
        meshRenderer.enabled = false; // Disable the mesh renderer to hide the ammo pack
        ammoCollider.enabled = false; // Disable the collider to prevent interaction
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false); // Hide prompt
        }
    }

    private System.Collections.IEnumerator RespawnAmmoPack()
    {
        yield return new WaitForSeconds(respawnTime); // Wait for the respawn time
        meshRenderer.enabled = true; // Enable the mesh renderer to show the ammo pack
        ammoCollider.enabled = true; // Enable the collider to allow interaction
    }
}
