using UnityEngine;

public class BombBag : Item
{
    [SerializeField]
    GameObject BombPrefab;

    GameObject ActiveBomb;

    public override void Usefunction(Vector2Int Point, int xDirection, int yDireciton, Player player = null) 
    {
        if(ActiveBomb == null && (player == null || player.CheckOpenTile(Point) && !player.IsWaterTile(Point)))
        {
            ActiveBomb = Instantiate(BombPrefab, new Vector3(Point.x + .5f, Point.y + .5f), Quaternion.identity);
        }
        
    }
}
