using UnityEngine;
using TMPro; // Import TextMeshPro namespace

// Script on Drag and drop will create box collider component automatically
[RequireComponent(typeof(BoxCollider))]
public class KeyController : MonoBehaviour
{
    BoxCollider keyCollider;
    Rigidbody keyRB;
    public TextMeshProUGUI txtToDisplay; // Use TextMeshProUGUI instead of Text
    public DoorController DC;

    /// <summary>
    /// In case user forgets to uncheck isTrigger in box collider
    /// This sets them automatically
    /// </summary>
    private void Start()
    {
        keyCollider = GetComponent<BoxCollider>();
        keyCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DC.gotKey = true;
            txtToDisplay.gameObject.SetActive(true);
            txtToDisplay.text = "Key Acquired";
            this.gameObject.SetActive(false);
        }
    }
}