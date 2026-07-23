using UnityEngine;
using UnityEngine.Tilemaps;

public class Item : MonoBehaviour
{
    bool isBeingHeld = false;
    public string id; //Example: KEY, RED_CRYSTAL, BOMB

    public void PickUp(GameObject player)
    {
        if (!isBeingHeld)
        {
            isBeingHeld = true;
            transform.SetParent(player.transform, false);
            transform.position = new Vector2(player.transform.position.x, player.transform.position.y + .25f);
        }
    }

    //WIP
    public void PutDown(GameObject player)
    {
        Vector2Int frontTile = player.GetComponent<Player>().GetPointInFrontOfPlayer();

        if (player.GetComponent<Player>().checkOpenTile(frontTile) && isBeingHeld)
        {
            isBeingHeld = false;
            transform.SetParent(null);
            transform.position = Vector2.one * .5f + frontTile; //Vector2.one * .5f -> Allows you to move the sprite to the center of the tile.
        }
    }
}
