using UnityEngine;

public class MoveRoad : MonoBehaviour
{
    [SerializeReference] int WallSpeed = -4;
    private GameManager GM;

    void Awake()
    {
        GM = GameObject.FindAnyObjectByType<GameManager>();
    }


    private void Update()
    {
        if (GM.canMoveRoad) transform.position += new Vector3(0, 0, WallSpeed) * Time.deltaTime;

        
    }
}
