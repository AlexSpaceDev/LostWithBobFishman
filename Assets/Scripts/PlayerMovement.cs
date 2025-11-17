using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 15f;
    private Rigidbody rb;
    private Animator animator;

    public Collider surfaceCollider; // El collider del padre (capsula vertical)
    public Collider swimCollider;    // El collider del hijo (horizontal)

    public enum MovementMode { Surface, Underwater }
    public MovementMode currentMode = MovementMode.Surface;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        rb.useGravity = false;
    }

    void Start()
    {
        // Si arrancamos dentro de la escena Underwater, forzamos modo submarino
        if (SceneManager.GetActiveScene().name == "Underwater")
        {
            currentMode = MovementMode.Underwater;
            if (animator != null) animator.SetBool("isUnderWater", true);
        }

        // Asegurar estado inicial de colliders
        if (surfaceCollider != null) surfaceCollider.enabled = (currentMode == MovementMode.Surface);
        if (swimCollider != null) swimCollider.enabled = (currentMode == MovementMode.Underwater);
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = 0f;

        if (currentMode == MovementMode.Underwater)
            moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0f);
        rb.velocity = movement * speed; // rb.linearVelocity → rb.velocity

        if (animator != null)
        {
            float currentSpeed = new Vector2(moveHorizontal, moveVertical).magnitude;
            animator.SetFloat("speed", currentSpeed);
            animator.SetBool("isUnderWater", currentMode == MovementMode.Underwater);

            UpdateColliderState();

            // Rotación simple del modelo (si tu modelo está en el children del animator)
            if (movement.x > 0.1f)
                SetModelRotation(-270f);
            else if (movement.x < -0.1f)
                SetModelRotation(-90f);
            else if (currentMode == MovementMode.Surface && currentSpeed < 0.1f)
                SetModelRotation(-180f);
        }
    }

    private void SetModelRotation(float yRotation)
    {
        if (animator == null) return;
        Vector3 euler = animator.transform.localEulerAngles;
        euler.y = yRotation;
        animator.transform.localEulerAngles = euler;
    }

    private void UpdateColliderState()
    {
        if (animator == null) return;

        // Ajusta el nombre a tu animacion exacta si difiere
        bool isSwimming = animator.GetCurrentAnimatorStateInfo(0).IsName("rig|Swim_swim");

        if (surfaceCollider != null)
            surfaceCollider.enabled = !isSwimming;

        if (swimCollider != null)
            swimCollider.enabled = isSwimming;
    }

    public void SwitchToUnderwater()
    {
        currentMode = MovementMode.Underwater;
        if (animator != null) animator.SetBool("isUnderWater", true);

        // Ajustes físicos para nadar
        rb.useGravity = false;
        rb.drag = 1.5f; // sensación de resistencia en el agua (ajusta a tu gusto)
        Debug.Log("Modo submarino activado");
    }

    public void SwitchToSurface()
    {
        currentMode = MovementMode.Surface;
        if (animator != null) animator.SetBool("isUnderWater", false);

        rb.useGravity = false;
        rb.drag = 0.5f;
        Debug.Log("Modo superficie activado");
    }
}




