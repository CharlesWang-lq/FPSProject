using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 0.3f;  // Speed at which the text moves upwards
    public float fadeSpeed = 5f;   // Speed at which the text fades out
    public float lifetime = 1f;    // How long the popup stays visible before fading completely

    private RectTransform rectTransform;
    private TextMeshProUGUI damageText;
    private Color textColor;
    private float timer;

    private void Awake()
    {
        // Cache the components
        rectTransform = GetComponent<RectTransform>();
        damageText = GetComponent<TextMeshProUGUI>();
        textColor = damageText.color;
    }

    public void Setup(int damage)
    {
        // Set the damage text and ensure it starts fully opaque
        damageText.text = $"*{damage}";
        textColor.a = 1f;  // Start with full opacity
        damageText.color = textColor;
    }

    private void Update()
    {
        rectTransform.localPosition += Vector3.up;

        timer += 0.01f;

        if (timer > lifetime)
        {
            FadeOut();
        }

        if (textColor.a <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // Fade out the text by reducing its alpha value
    private void FadeOut()
    {
        textColor.a -= fadeSpeed; // Reduce alpha over time
        damageText.color = textColor;
    }
}
