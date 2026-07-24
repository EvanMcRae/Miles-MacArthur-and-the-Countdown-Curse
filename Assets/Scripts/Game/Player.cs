using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class Player : MonoBehaviour
{
    public PlayerInput inputSettings;
    public Tilemap ColliderTiles;

    Vector2Int lastMoveDirInput = default;

    const float MOVE_COOLDOWN = .15f;

    float lastMoveXTimer = MOVE_COOLDOWN;
    float lastMoveYTimer = MOVE_COOLDOWN;

    public CameraController cam;

    public LayerMask collidersLayer;

    int xDirection = 1;
    int yDirection = 0;

    public Item heldItem;

    public Animator anim;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite[] upSprite, downSprite, leftSprite, rightSprite;
    int curSprite = 0;
    bool moveLockedX = false, moveLockedY = false;
    private SoundPlayer soundPlayer;

    void Start()
    {
        heldItem = null;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        soundPlayer = GetComponentInChildren<SoundPlayer>();
    }

    void Update()
    {
        if (GameManager.paused) return;

        HandleMovement();
        if (inputSettings.actions["PickUpItem"].WasPressedThisFrame())
        {
            if (heldItem == null) PickUpItem();
            else PutDownItem();
        }
        else if (inputSettings.actions["Interact"].WasPressedThisFrame())
        {
            if (heldItem != null)
            {
                heldItem.Usefunction(GetPointInFrontOfPlayer(), xDirection, yDirection, gameObject.GetComponent<Player>());
            }
        }
    }

    public void HandleMovement()
    {
        Vector2 inputMove = inputSettings.actions["Move"].ReadValue<Vector2>();

        Vector2Int currPosition = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        Vector2Int inputMoveDir = new(Mathf.RoundToInt(inputMove.x), Mathf.RoundToInt(inputMove.y));

        int prevSprite = curSprite;

        if (inputMoveDir == default)
        {
            lastMoveDirInput = inputMoveDir;
            return;
        }

        if (lastMoveDirInput.y != inputMoveDir.y || CheckOpenTile(currPosition + Vector2Int.up * inputMoveDir.y))
        {
            moveLockedY = false;
        }
        if (inputMoveDir.y != 0 && (lastMoveDirInput.y != inputMoveDir.y || (!moveLockedY && lastMoveYTimer < 0)))
        {
            //Separate vertical movement and horizontal separately, to prevent moving diagonally without testing either side.
            if (CheckOpenTile(currPosition + Vector2Int.up * inputMoveDir.y))
            {
                transform.position += Vector3.up * inputMoveDir.y;
                currPosition += Vector2Int.up * inputMoveDir.y;
                soundPlayer.PlaySound("Game.Step");
            }
            else
            {
                moveLockedY = true;
                soundPlayer.PlaySound("Game.Thud");
            }
            xDirection = 0;
            yDirection = inputMoveDir.y;
            spriteRenderer.sprite = yDirection < 0 ? downSprite[prevSprite] : upSprite[prevSprite];
            anim.Play("PlayerMove", 0, 0);
            curSprite = 1 - prevSprite;
            lastMoveYTimer = MOVE_COOLDOWN;
        }
        else
        {
            lastMoveYTimer -= Time.deltaTime;
        }

        if (lastMoveDirInput.x != inputMoveDir.x || CheckOpenTile(currPosition + Vector2Int.right * inputMoveDir.x))
        {
            moveLockedX = false;
        }
        if (inputMoveDir.x != 0 && (lastMoveDirInput.x != inputMoveDir.x || (!moveLockedX && lastMoveXTimer < 0)))
        {
            //Separate vertical movement and horizontal separately, to prevent moving diagonally without testing either side.
            if (CheckOpenTile(currPosition + Vector2Int.right * inputMoveDir.x))
            {
                transform.position += Vector3.right * inputMoveDir.x;
                soundPlayer.PlaySound("Game.Step");
            }
            else
            {
                moveLockedX = true;
                soundPlayer.PlaySound("Game.Thud");
            }
            yDirection = 0;
            xDirection = inputMoveDir.x;
            spriteRenderer.sprite = xDirection < 0 ? leftSprite[prevSprite] : rightSprite[prevSprite];
            anim.Play("PlayerMove", 0, 0);
            curSprite = 1 - prevSprite;
            lastMoveXTimer = MOVE_COOLDOWN;
        }
        else
        {
            lastMoveXTimer -= Time.deltaTime;
        }

        lastMoveDirInput = inputMoveDir;
        ////print("(" + xDirection + ", " + yDirection + ")");
    }

    /// <summary>
    /// Checks if you can collide into a tile or not.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool CheckOpenTile(Vector2Int tile)
    {
        if (ColliderTiles.GetTile((Vector3Int)(tile)) != null)
            return false;

        if (Physics2D.Linecast(ColliderTiles.CellToWorld((Vector3Int)tile) + Vector3.one * .25f, ColliderTiles.CellToWorld((Vector3Int)tile) + Vector3.one * .75f, collidersLayer))
        {
            Debug.DrawLine(ColliderTiles.CellToWorld((Vector3Int)tile) + Vector3.one * .25f, ColliderTiles.CellToWorld((Vector3Int)tile) + Vector3.one * .75f, Color.red, 1);
            return false;
        }
        else
            return true;
    }

    public void PickUpItem(Item in_item = null)
    {
        bool removeKeyFlag = false;
        if (heldItem == null) removeKeyFlag = true;

        if (in_item != null) heldItem = in_item;
        else heldItem = GetItemInFrontOfPlayer();

        if (heldItem != null)
        {
            //Case where you have nothing in your hands and want to pick up a key thats sitting in a slot hole.
            if (heldItem.GetComponent<Key>() != null && removeKeyFlag) heldItem.GetComponent<Key>().RemoveFromKeyhole();

            //Normal Pick Up behavior.
            heldItem.isBeingHeld = true;
            heldItem.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
            heldItem.transform.SetParent(transform, false);
            heldItem.transform.position = Vector2.right * transform.position.x + Vector2.up * (transform.position.y + .25f);
        }
    }

    public void PutDownItem()
    {
        //Get point to space in front of player.
        Vector2Int frontTile = GetPointInFrontOfPlayer();

        if (CheckOpenTile(frontTile))
        {
            Item itemInFront = GetItemInFrontOfPlayer();
            if (itemInFront != null && !itemInFront.canBePickedUp) return;
            else
            {
                //If the item is a key, try to use it when its put down (in case it's put down on top of a floor lock)
                //Do not do this if you used Interact or else it stackoverflows.
                if (heldItem.GetComponent<Key>() != null && !inputSettings.actions["Interact"].WasPressedThisFrame())
                {
                    heldItem.Usefunction(GetPointInFrontOfPlayer(), xDirection, yDirection, gameObject.GetComponent<Player>());
                }

                //Put Down Item behavior.
                if (heldItem != null)
                {
                    heldItem.isBeingHeld = false;
                    heldItem.transform.SetParent(null);
                    heldItem.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
                    heldItem.transform.position = Vector2.one * .5f + frontTile; //Vector2.one * .5f -> Allows you to move the sprite to the center of the tile.

                    //Swap item for item on floor.
                    if (itemInFront != null && itemInFront.canBePickedUp) PickUpItem(itemInFront);
                    else heldItem = null;
                }
            }
        }
    }

    public Vector2Int GetPointInFrontOfPlayer()
    {
        return Vector2Int.right * Mathf.FloorToInt(transform.position.x + xDirection) + Vector2Int.up * Mathf.FloorToInt(transform.position.y + yDirection);
    }

    public Item GetItemInFrontOfPlayer()
    {
        //Get point to space in front of player.
        Vector2Int frontTile = GetPointInFrontOfPlayer();

        //Check tile in front of player for item.
        ////Vector2 test = Vector2.one * .5f + frontTile;
        ////Debug.DrawLine((Vector3)test, new Vector3(test.x - .3f, test.y -.3f, 0), Color.white, 5);
        Collider2D[] cols = Physics2D.OverlapBoxAll(Vector2.one * .5f + frontTile, Vector2.one * .3f, 0);


        //Search for items gotten from prev method.
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.GetComponent<Item>() != null && col.gameObject.GetComponent<Item>().canBePickedUp)
            {
                return col.gameObject.GetComponent<Item>();
            }
        }

        return null;
    }


    void OnPause(InputValue _)
    {
        if (PopupPanel.unpausablePanelsOpen > 0) return;
        GameManager.instance.PressPause();
    }
}