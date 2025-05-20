using UnityEngine;

public class CheckForWallTrigger : MonoBehaviour
{
    [SerializeField] private GameObject RoadPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RoadTrigger"))
        {
            Instantiate(RoadPrefab, other.transform.position+new Vector3(0,0,75), Quaternion.identity);
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("obstacle");
            GetComponentInChildren<Animator>().SetTrigger("End");
            GameObject.FindAnyObjectByType<GameManager>().End();
        }
    }
}
