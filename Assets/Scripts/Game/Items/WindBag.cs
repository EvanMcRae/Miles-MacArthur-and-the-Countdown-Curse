using UnityEngine;

public class WindBag : Item
{
    [SerializeField]
    GameObject TornadoPrefab;

    GameObject ActiveTornado;

    public override void Usefunction(Vector2 Point, int xDirection, int yDireciton, Player player = null)
    {
        if (ActiveTornado == null)
        {
            ActiveTornado = Instantiate(TornadoPrefab, new Vector3(Point.x + .5f, Point.y + .5f), Quaternion.identity);
        }

    }
}