using UnityEngine;

public class BombBag : Item
{
    [SerializeField]
    GameObject BombPrefab;

    GameObject ActiveBomb;

    public override void Usefunction(Vector2Int Point, int xDirection, int yDireciton, Player player = null) 
    {
        if(ActiveBomb == null)
        {
            ActiveBomb = Instantiate(BombPrefab, new Vector3(Point.x + .5f, Point.y + .5f), Quaternion.identity);
        }
        
    }

    public override string GetUseText()
    {
        return "<size=+0.2><voffset=-0.25em>&<voffset=0em><size=+0> Bomb";
    }
}
