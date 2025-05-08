using UnityEngine;

public class DesroyRoad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RoadTrigger"))
        {
            Destroy(other.gameObject);
        }
    }
}
