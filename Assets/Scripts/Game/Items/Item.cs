using UnityEngine;
using UnityEngine.Tilemaps;

public class Item : MonoBehaviour
{
    bool isBeingHeld = false;
    public string id; //Example: KEY, RED_CRYSTAL, BOMB

    //public Item()
    //{
    //    isBeingHeld = false;
    //}

    private void Update()
    {
        
    }

    public void PickUp(GameObject player)
    {
        if (!isBeingHeld)
        {
            isBeingHeld = true;
            transform.SetParent(player.transform, false);
        }
    }

    //TODO
    public void PutDown(Vector2Int currPos, Vector2Int directionFacing)
    {
        //Check if tile adjacent to direction is empty

        //Place object in front of player
        if (isBeingHeld)
        {

            isBeingHeld = false;
            transform.SetParent(null);
        }
    }
}
