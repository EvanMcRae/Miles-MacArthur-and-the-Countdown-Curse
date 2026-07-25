using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Item : MonoBehaviour
{
    public bool isBeingHeld = false;
    public bool canBePickedUp = true;
    public new string name; //Example: KEY, RED_CRYSTAL, BOMB
    private float startPos;
    [SerializeField] private float oscillationAmplitude = 0.125f;
    private float oscillationAmount;
    private Tween oscillationTween;
    [SerializeField] private SpriteRenderer visual, shadow;
    [SerializeField] private Sprite[] shadowSprites;

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
                Vector2 pos = visual.transform.localPosition;
                pos.y = oscillationAmount;
                visual.transform.localPosition = pos;
                shadow.sprite = shadowSprites[Mathf.FloorToInt(oscillationAmount / oscillationAmplitude * shadowSprites.Length)];
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

    public virtual void ActivateEffectOnPickup(Player player)
    {
        visual.transform.localPosition = Vector3.zero;
        shadow.enabled = false;
    }
    
    public virtual void ActivateEffectOnPutDown(Player player)
    {
        shadow.enabled = true;
    }

    public virtual void OnDestroy()
    {
        AudioManager.OnBeat -= Oscillate;
        Utils.KillTween(ref oscillationTween);
    }
}
