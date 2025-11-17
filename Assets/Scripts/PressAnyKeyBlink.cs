using UnityEngine;
using TMPro;

public class PressAnyKeyBlink : MonoBehaviour
{
    private TextMeshProUGUI text;
    public float speed = 2f;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        float alpha = (Mathf.Sin(Time.time * speed) + 1) / 2f;
        text.alpha = alpha;
    }
}
