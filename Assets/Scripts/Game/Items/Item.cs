using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Item : MonoBehaviour
{
    public bool isBeingHeld = false;
    public bool canBePickedUp = true;
    public new string name; //Example: KEY, RED_CRYSTAL, BOMB
    private float startPos;
    [SerializeField] private float oscillationAmplitude = 0.1f;
    private float oscillationAmount;
    private Tween oscillationTween;

    protected virtual void Awake()
    {
        AudioManager.OnBeat += Oscillate;
        startPos = transform.position.y;
    }

    void Oscillate(int beatNum)
    {
        Utils.KillTween(ref oscillationTween);
        oscillationTween = DOTween.To(() => oscillationAmount, x => oscillationAmount = x, oscillationAmplitude * (beatNum % 2), 1f).OnUpdate(() =>
        {
            if (!isBeingHeld)
            {
                Vector2 pos = transform.position;
                pos.y = startPos + oscillationAmount;
                transform.position = pos;
            }
        });
    }

    //public void PickUp(GameObject player)
    //{
    //    if (!isBeingHeld)
    //    {
    //        isBeingHeld = true;
    //        transform.SetParent(player.transform, false);
    //        transform.position = new Vector2(player.transform.position.x, player.transform.position.y + .25f);
    //    }
    //}

    //public void PutDown(GameObject player)
    //{
    //    Vector2Int frontTile = player.GetComponent<Player>().GetPointInFrontOfPlayer();

    //    if (player.GetComponent<Player>().checkOpenTile(frontTile) && isBeingHeld)
    //    {
    //        isBeingHeld = false;
    //        transform.SetParent(null);
    //        transform.position = Vector2.one * .5f + frontTile; //Vector2.one * .5f -> Allows you to move the sprite to the center of the tile.
    //    }

    /// <summary>
    /// Activate the unique functionality of the item
    /// </summary>
    public virtual void Usefunction(Vector2Int point, int xDirection, int yDirection, Player player = null) { }

    public virtual void ActivateEffectOnPickup(Player player) {}
    
    public virtual void ActivateEffectOnPutDown(Player player)
    {
        startPos = transform.position.y;
    }
}
