using UnityEngine;
using UnityEngine.Tilemaps;

public class Keyhole : MonoBehaviour
{
    [Tooltip("Opens when a key with the same integer value is used on it.")]
    public int keyholeID;
    public Key heldKey = null;

    [Tooltip("For slot type keyholes, if it both holds a key AND that keyID matches the keyholeID.")]
    public bool activated = false;
    public bool canRemoveKey = true;

    [Tooltip("For slot type keyholes, if the hole should do something when the key matches or wait for an external call to HandleMatchingKey().")]
    public bool handleMatchImmediately = true;

    [Tooltip("What the hole should do when the key matches.\n\nDeactivate: Disable the ability to pick up the key out the hole.\nDestroy: Destroy both the key and keyhole.")]
    public enum MatchBehaviors {DEACTIVATE, DESTROY};
    public MatchBehaviors behaviorOnMatch;

    [SerializeField] private GameObject unlockParticles;

    public void HandleMatchingKey()
    {
        // Debug.Log("Handling a matching key :3");
        // Handle particles spawning over each tile when unlocked
        var tiles = GetComponentInChildren<Tilemap>();
        // if (tiles == null) Debug.Log("Tile map? What tile map?");
        var bounds = tiles.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tiles.HasTile(pos))
            {
                // Debug.Log("omfg a tile :O");
                Instantiate(unlockParticles, tiles.CellToWorld(pos), Quaternion.identity);
            }
            else
            {
                // Debug.Log("I don't see no tile here.");
            }
        }


        if (behaviorOnMatch == MatchBehaviors.DEACTIVATE)
        {
            canRemoveKey = false;
            heldKey.canBePickedUp = false;
            // print("Deactivated!");
        }
        else
        {
            Player.instance.UnlockDoor();
            Destroy(heldKey.gameObject);
            Destroy(gameObject);
        }
    }
}
