using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 50f; // Speed of upward movement
    public float fadeSpeed = 5f; // Speed of fading
    public float lifetime = 1f; // How long the popup stays visible

    private RectTransform rectTransform;
    private TextMeshProUGUI damageText;
    private Color textColor;
    private float timer;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        damageText = GetComponent<TextMeshProUGUI>();
        textColor = damageText.color;
    }

    public void Setup(int damage)
    {
        damageText.text = $"*{damage}"; // Display the damage value with a star
    }

    private void Update()
    {
        // Move the text upward
        Vector3 oldPosition = rectTransform.localPosition; // Log old position
        rectTransform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
        Debug.Log($"Moving DamagePopup: Old Position: {oldPosition}, New Position: {rectTransform.localPosition}");

        // Fade out the text
        timer += Time.deltaTime;
        if (timer > lifetime)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            damageText.color = textColor;

            if (textColor.a <= 0f)
            {
                Debug.Log("Destroying DamagePopup");
                Destroy(gameObject);
            }
        }
    }
}
