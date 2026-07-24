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
            //spawn a tornado
            ActiveTornado = Instantiate(TornadoPrefab, new Vector3(Point.x + .5f, Point.y + .5f), Quaternion.identity);
            Tornado tornado = ActiveTornado.GetComponent<Tornado>();

            //blow an object up to 2 tiles in faced direction
            Player player = FindAnyObjectByType<Player>();
            Item item = player.GetItemInFrontOfPlayer();
            if (item != null)
            {
                Vector2Int pointInFront = player.GetPointInFrontOfPlayer();
                Vector2Int pointOneAway = new Vector2Int(pointInFront.x + xDirection, pointInFront.y + yDireciton);
                Vector2Int pointTwoAway = new Vector2Int(pointInFront.x + xDirection*2, pointInFront.y + yDireciton*2);

                if (player.CheckOpenTile(pointTwoAway) && player.CheckOpenTile(pointOneAway))
                {
                    item.transform.transform.position = new Vector3(pointTwoAway.x + .5f, pointTwoAway.y + .5f);
                }
                else if (player.CheckOpenTile(pointOneAway))
                {
                    item.transform.transform.position = new Vector3(pointOneAway.x + .5f, pointOneAway.y + .5f);
                }
            }
        }

    }
}