using UnityEngine;

public class MoveRoad : MonoBehaviour
{
    [SerializeReference] int WallSpeed = -4;
    private void Update()
    {
        transform.position += new Vector3(0, 0, WallSpeed) * Time.deltaTime;
    }
}
