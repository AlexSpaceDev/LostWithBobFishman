using UnityEngine;

public class SimpleFloat : MonoBehaviour
{
    [Header("Movimiento Vertical (olas)")]
    public float floatAmplitude = 0.2f;  // Altura del movimiento
    public float floatFrequency = 1f;    // Velocidad del movimiento
    
    [Header("Balanceo (rotación)")]
    public float tiltAmplitude = 2f;     // Grados de inclinación
    public float tiltFrequency = 0.5f;   // Velocidad del balanceo

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update()
    {
        // Movimiento vertical
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Balanceo (roll y pitch)
        float tiltX = Mathf.Sin(Time.time * tiltFrequency) * tiltAmplitude;
        float tiltZ = Mathf.Cos(Time.time * tiltFrequency) * tiltAmplitude;

        transform.rotation = startRot * Quaternion.Euler(tiltX, 0f, tiltZ);
    }
}
