using UnityEngine;

public class Fish : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private UIManager uiManager; // 🔹 para actualizar el contador

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        speed = Random.Range(15f, 20f);
    }

    void Start()
    {
        // Buscar automáticamente el UIManager en escena
        uiManager = FindFirstObjectByType<UIManager>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        Vector3 dir = targetPosition - transform.position;

        // ✅ Mantener escala original y solo invertir X si cambia de dirección
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
        transform.localScale = scale;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            if (health != null && health.IsAlive()) // 🧠 Solo si está vivo
            {
                if (CompareTag("GoodFish"))
                {
                    uiManager?.AddFish();
                    Debug.Log("🐠 Pez bueno recogido");
                }

                Destroy(gameObject); // 🔹 Solo se destruye si el jugador está vivo
            }
            else
            {
                Debug.Log("❌ El jugador está muerto, el pez no se puede recoger.");
            }
        }
    }
}


