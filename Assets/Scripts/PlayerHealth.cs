using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("UI de corazones")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Referencias")]
    public UIManager uiManager; // 🔹 Referencia al UIManager

    [HideInInspector] 
    public bool isDead = false; // 🧠 Nuevo: control de estado

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();

        if (uiManager == null)
            uiManager = Object.FindFirstObjectByType<UIManager>();
    }

    public void TakeDamage(int amount)
    {
        // ❌ Si ya está muerto, ignorar daño
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Jugador recibió daño. Vidas restantes: {currentHealth}");

        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

void Die()
{
    if (isDead) return; // Evita múltiples llamadas
    isDead = true;

    Debug.Log("☠ El jugador ha perdido todas las vidas");

    // 🧩 Reproducir animación de muerte
    Animator animator = GetComponentInChildren<Animator>();
    if (animator != null)
    {
        animator.SetTrigger("Die");
    }

    // 🔹 Llamar al UIManager que ya maneja la muerte
    if (uiManager != null)
    {
        uiManager.PlayerDeath();
    }
    else
    {
        Debug.LogWarning("⚠ UIManager no asignado en PlayerHealth.");
    }
}


    void UpdateHeartsUI()
    {
        if (hearts.Length == 0) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }

    // 🔹 Método auxiliar para otros scripts (Fish / BadFish)
    public bool IsAlive()
    {
        return !isDead;
    }



}


