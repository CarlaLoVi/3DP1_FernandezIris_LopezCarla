using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInterior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("InteriorZone");
        }
    }
}
