using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("Canvas Groups")]
    public CanvasGroup menuUI;
    public CanvasGroup heartsUI;
    public CanvasGroup buttonsUI;
    public CanvasGroup daysUI;

    [Header("Fade Settings")]
    public float fadeSpeed = 2f;

    [Header("Dive UI")]
    public GameObject diveButton;
    public Image fadeImage;
    public PlayerMovement player;

    [Header("Underwater Settings")]
    public Transform underwaterSpawnPoint;
    public FishSpawner fishSpawner;
    public Camera mainCamera;

    [Header("Oxygen Timer UI")]
    public TextMeshProUGUI oxygenTimerText;
    public Image oxygenTimerIcon;
    public int diveTime = 60;

    [Header("Fish Counter UI")]
    public TextMeshProUGUI fishCountText; // Asigna el TMP aquí
    public Image fishIcon;                // Asigna el ícono de pez aquí

    [Header("Warning Message UI")]
    public TextMeshProUGUI warningText;


    private int fishCount = 0;
    private bool hasDived = false; // Evita que el botón vuelva a aparecer después del buceo
    private bool gameStarted = false;
    private Coroutine oxygenRoutine;

    void Start()
    {
        if (diveButton != null)
            diveButton.SetActive(false);

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);

        if (oxygenTimerText != null)
            oxygenTimerText.gameObject.SetActive(false);

        // Ocultar contador al inicio
        if (fishCountText != null)
            fishCountText.gameObject.SetActive(false);
        if (fishIcon != null)
            fishIcon.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted && AnyInputPressed())
        {
            gameStarted = true;
            StartCoroutine(FadeOut(menuUI));
            StartCoroutine(FadeIn(heartsUI));
            StartCoroutine(FadeIn(buttonsUI));
            StartCoroutine(FadeIn(daysUI));
        }
    }

    bool AnyInputPressed()
    {
        // Teclado (PC)
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        // Mouse (click)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        // Pantalla táctil (móvil)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        // Gamepad / Joystick
        if (Gamepad.current != null)
            {
                if (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                    Gamepad.current.buttonNorth.wasPressedThisFrame ||
                    Gamepad.current.buttonEast.wasPressedThisFrame ||
                    Gamepad.current.buttonWest.wasPressedThisFrame ||
                    Gamepad.current.startButton.wasPressedThisFrame ||
                    Gamepad.current.selectButton.wasPressedThisFrame)
                    return true;

                if (Gamepad.current.leftStick.ReadValue().magnitude > 0.2f)
                    return true;
            }

        return false;
    }


    public void ShowDiveButton(bool show)
    {
        if (diveButton != null)
        diveButton.SetActive(show && !hasDived);
    }

    public void Clickprueba()
    {
        Debug.Log("Botón de prueba presionado");
    }

    public void OnDiveButtonPressed()
    {
        if (diveButton != null)
            diveButton.SetActive(false); // Asegurar que desaparezca antes del fade

        StartCoroutine(DiveTransition());
    }

    private IEnumerator DiveTransition()
    {
        hasDived = true; // Marcar que ya se sumergió
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        if (underwaterSpawnPoint != null)
        {
            player.transform.position = underwaterSpawnPoint.position;

            if (mainCamera != null)
            {
                Vector3 camPos = mainCamera.transform.position;
                camPos.y = 0f;
                mainCamera.transform.position = camPos;
            }
        }

        player.SwitchToUnderwater();
        yield return new WaitForSeconds(0.5f);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        if (fishSpawner != null)
        {
            yield return new WaitForSeconds(1.5f);
            fishSpawner.StartSpawning();
        }

        // Mostrar contador y cronómetro al sumergirse
        if (fishCountText != null) fishCountText.gameObject.SetActive(true);
        if (fishIcon != null) fishIcon.gameObject.SetActive(true);

        if (oxygenRoutine != null)
            StopCoroutine(oxygenRoutine);
        oxygenRoutine = StartCoroutine(StartOxygenTimer());
    }

    private IEnumerator StartOxygenTimer()
    {
        if (oxygenTimerText == null)
            yield break;

        oxygenTimerText.gameObject.SetActive(true);
        if (oxygenTimerIcon != null) 
            oxygenTimerIcon.gameObject.SetActive(true); // Mostrar imagen del cronómetro

        int timeLeft = diveTime;

        Color normalColor = Color.white;
        Color warningColor = Color.red;
        bool warningShown = false;

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);

        while (timeLeft > 0)
        {
            if (timeLeft <= 10)
            {
                oxygenTimerText.color = warningColor;
                if (oxygenTimerIcon != null)
                    oxygenTimerIcon.color = warningColor; // La imagen también se pone roja

                if (!warningShown)
                {
                    warningShown = true;
                    StartCoroutine(ShowTemporaryMessage("¡Te estás quedando sin oxígeno!", 2f));
                }

                if (fadeImage != null)
                {
                    float blinkAlpha = Mathf.PingPong(Time.time * 2f, 0.5f);
                    fadeImage.color = new Color(1f, 0f, 0f, blinkAlpha * 0.5f);
                }
            }

            oxygenTimerText.text = $"{timeLeft}s";
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        // Tiempo agotado
        oxygenTimerText.text = "¡Sin oxígeno!";
        oxygenTimerText.color = warningColor;
        if (oxygenTimerIcon != null)
            oxygenTimerIcon.color = warningColor;

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);

        player.enabled = false;
        if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.AddTorque(new Vector3(0f, 0f, Random.Range(-0.5f, 0.5f)), ForceMode.VelocityChange);
            rb.AddForce(Vector3.up * 1.5f, ForceMode.VelocityChange);
        }

        yield return new WaitForSeconds(2f);

        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * 0.5f;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Ocultar texto e imagen cuando el tiempo termina
        oxygenTimerText.gameObject.SetActive(false);
        if (oxygenTimerIcon != null)
            oxygenTimerIcon.gameObject.SetActive(false); // Ocultar imagen

        // Reiniciar escena
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }



    public void PlayerDeath()
    {
        StartCoroutine(HandlePlayerDeath());
    }

    private IEnumerator HandlePlayerDeath()
    {
        // Detener movimiento del jugador
        player.enabled = false;
        if (player.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;

            rb.AddTorque(new Vector3(0f, 0f, Random.Range(-0.5f, 0.5f)), ForceMode.VelocityChange);
            rb.AddForce(Vector3.up * 1.5f, ForceMode.VelocityChange);
        }

        // Mensaje opcional de advertencia
        if (warningText != null)
        {
            warningText.text = "¡Has perdido todas las vidas!";
            warningText.color = Color.red;
            warningText.gameObject.SetActive(true);
        }

        // Fade out suave antes de reiniciar
        yield return new WaitForSeconds(2f);

        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * 0.5f;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Reiniciar la escena
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }


    // --- NUEVO: Actualizar contador de peces ---
    public void AddFish()
    {
        fishCount++;
        if (fishCountText != null)
            fishCountText.text = $"{fishCount}";
    }

    private IEnumerator FadeOut(CanvasGroup cg)
    {
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        cg.alpha = 0;
        cg.gameObject.SetActive(false);
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);
        cg.interactable = false;      // Desactivado mientras hace fade
        cg.blocksRaycasts = false;

        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        cg.alpha = 1;

        // Activar interacción después del fade
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private IEnumerator ShowTemporaryMessage(string message, float duration)
    {
        if (warningText == null)
            yield break;

        warningText.text = message;
        warningText.color = new Color(1f, 0.85f, 0f); // Amarillo cálido
        warningText.gameObject.SetActive(true);

        // Fade In
        CanvasGroup cg = warningText.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = warningText.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0;
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime * 3f;
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        // Fade Out
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        warningText.gameObject.SetActive(false);
    }


}
