using UnityEngine;

public class BadFish : Fish
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            if (health != null && health.IsAlive()) // 🧠 Solo si está vivo
            {
                health.TakeDamage(1);
                Debug.Log($"🦈 Daño aplicado a {health.name} por {gameObject.name}");

                Destroy(gameObject); // 🔹 Solo se destruye si el jugador está vivo
            }
            else
            {
                Debug.Log("❌ El jugador está muerto, el pez malo no hace nada y sigue su camino.");
            }
        }
    }
}




