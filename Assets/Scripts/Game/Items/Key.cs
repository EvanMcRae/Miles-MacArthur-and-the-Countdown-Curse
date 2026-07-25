using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Key : Item
{
    [Tooltip("Slots into any keyhole with the same integer value.")]
    public int keyID;

    //Destroy On Open: Will destroy both the key and keyhole when the keyID and keyholeID match
    //Slot In Place: Will set the key directly on top the keyhole without doing anything.
    public enum Behaviors {DESTROY_ON_OPEN, SLOT_IN_PLACE};
    [Tooltip("Destroy On Open: Will destroy both the key and keyhole when the keyID and keyholeID match.\nSlot In Place: Will set the key directly on top the keyhole without doing anything.")]
    public Behaviors behavior;

    public Keyhole keyholeItsInside = null;

    //point = point in front of player.
    public override void Usefunction(Vector2Int point, int xDirection, int yDirection, Player player = null) {

        //Check tile in front of player for keyhole.
        Collider2D[] cols = Physics2D.OverlapBoxAll(Vector2.one * .5f + point, Vector2.one * .3f, 0);


        //Search for items gotten from prev method.
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.GetComponent<Keyhole>() != null)
            {
                Keyhole hole = col.gameObject.GetComponent<Keyhole>();

                //Destroy both the key and keyhole.
                if (behavior == Key.Behaviors.DESTROY_ON_OPEN)
                {
                    if (hole != null && hole.keyholeID == keyID)
                    {
                        Destroy(hole.gameObject);
                        player.UnlockDoor();
                        player.heldItem = null;
                        Destroy(gameObject);
                    }
                }
                else if (behavior == Key.Behaviors.SLOT_IN_PLACE)
                {
                    if (hole != null && hole.canRemoveKey == true)
                    {
                        //Case where the player presses the "use" key in front of a floor keyhole instead of just putting it down
                        //(Allows for swaps in case theres a key already there).
                        if (!player.inputSettings.actions["PickUpItem"].WasPressedThisFrame())
                        {
                            player.PutDownItem();
                        }

                        //Fill hole
                        hole.heldKey = gameObject.GetComponent<Key>();
                        keyholeItsInside = hole;

                        //Check if IDs match, and handle it.
                        if(hole.keyholeID == keyID)
                        {
                            hole.activated = true;
                            if (hole.handleMatchImmediately) hole.HandleMatchingKey();
                        }
                    }
                }

                break;
            }
        }
    }

    public void RemoveFromKeyhole()
    {
        if(keyholeItsInside != null && canBePickedUp)
        {
            keyholeItsInside.heldKey = null;
            if (keyholeItsInside.activated == true) keyholeItsInside.activated = false;
        }
    }
}
