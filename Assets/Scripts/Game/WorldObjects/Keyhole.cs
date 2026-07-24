using UnityEngine;

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

    public void HandleMatchingKey()
    {
        if (behaviorOnMatch == MatchBehaviors.DEACTIVATE)
        {
            canRemoveKey = false;
            heldKey.canBePickedUp = false;
            print("Deactivated!");
        }
        else
        {
            Destroy(heldKey.gameObject);
            Destroy(gameObject);
        }
    }
}
