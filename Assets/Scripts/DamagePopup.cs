using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 1f; // Speed at which the text moves up
    public float fadeSpeed = 2f; // Speed at which the text fades
    public float lifetime = 1f; // How long the text stays visible

    private TextMeshProUGUI damageText;
    private Color textColor;
    private float timer;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        textColor = damageText.color;
    }

    public void Setup(int damage)
    {
        damageText.text = damage.ToString(); // Set the damage value
    }

    private void Update()
    {
        // Move the text upward over time
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out the text
        timer += Time.deltaTime;
        if (timer > lifetime)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            damageText.color = textColor;

            if (textColor.a <= 0f)
            {
                Destroy(gameObject); // Destroy the popup when fully faded
            }
        }
    }
}
