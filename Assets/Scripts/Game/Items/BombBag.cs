using UnityEngine;

public class BombBag : Item
{
    [SerializeField]
    GameObject BombPrefab;

    GameObject ActiveBomb;

    public override void Usefunction(Vector2 Point, Vector2 MoveDir) 
    {
        if(ActiveBomb == null)
        {
            ActiveBomb = Instantiate(BombPrefab, new Vector3(Point.x + .5f, Point.y + .5f), Quaternion.identity);
        }
        
    }
}
