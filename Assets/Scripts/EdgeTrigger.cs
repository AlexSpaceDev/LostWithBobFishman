using UnityEngine;

public class EdgeTrigger : MonoBehaviour
{
    public UIManager uiManager; // referencia al UI para mostrar el botón

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.ShowDiveButton(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.ShowDiveButton(false);
        }
    }
}
