using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public PlayerInput inputSettings;
    public Tilemap ColliderTiles;

    Vector2Int lastMoveDirInput = default;

    const float MOVE_COOLDOWN = .125f;

    float lastMoveTimer = MOVE_COOLDOWN;

    public CameraController cam;

    public LayerMask collidersLayer;

    int xDirection = 1;
    int yDirection = 0;

    public Item heldItem;

    public Animator anim;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite[] upSprite, downSprite, leftSprite, rightSprite;
    [SerializeField] private Sprite deathSprite;
    int curSprite = 0;
    bool moveLockedX = false, moveLockedY = false;
    private SoundPlayer soundPlayer;
    public static Player instance;

    /// <summary>
    /// Check if standing in water, which presumably means player is also holding the Water Orb
    /// </summary>
    bool inWater = false;
    /// <summary>
    /// Check if able to walk in water, should be true while holding Water Orb and false otherwise
    /// </summary>
    public bool canWalkInWater = false;
    [SerializeField]
    LayerMask waterLayer;

    [SerializeField] private MovementGlyphRenderer movementGlyphRenderer;
    [SerializeField] private GlyphRenderer pickupGlyphRenderer;
    private bool killedMovementGlyphs = false;

    void Start()
    {
        heldItem = null;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        soundPlayer = GetComponentInChildren<SoundPlayer>();
        instance = this;
    }

    void Update()
    {
        if (GameManager.paused || GameManager.quitting || ScreenTransition.active) return;

        HandleMovement();
        CheckIfWallLogged();
        if (inputSettings.actions["PickUpItem"].WasPressedThisFrame() && !inWater)
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

    private IEnumerator KillMovementGlyphs()
    {
        yield return new WaitForSeconds(5.0f);
        movementGlyphRenderer.Deactivate();
    }

    public void HandleMovement()
    {
        Vector2 inputMove = inputSettings.actions["Move"].ReadValue<Vector2>();
        movementGlyphRenderer.SendInput(inputMove);
        if (inputMove.magnitude != 0)
        {
            if (!killedMovementGlyphs)
            {
                StartCoroutine(KillMovementGlyphs());
                killedMovementGlyphs = true;
            }
        }

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

        bool movedThisFrame = false;

        if (inputMoveDir.y != 0 && (lastMoveDirInput.y != inputMoveDir.y || (!moveLockedY && lastMoveTimer < 0)))
        {
            //Separate vertical movement and horizontal separately, to prevent moving diagonally without testing either side.
            if (CheckOpenTile(currPosition + Vector2Int.up * inputMoveDir.y) && (canWalkInWater || !IsWaterTile(currPosition + Vector2Int.up * inputMoveDir.y)))
            {
                if (IsWaterTile(currPosition + Vector2Int.up * inputMoveDir.y))
                {
                    inWater = true;
                    soundPlayer.PlaySound("Game.Splash");
                }
                else
                {
                    inWater = false;
                }

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
            movedThisFrame = true;
        }

        if (lastMoveDirInput.x != inputMoveDir.x || CheckOpenTile(currPosition + Vector2Int.right * inputMoveDir.x))
        {
            moveLockedX = false;
        }
        if (inputMoveDir.x != 0 && (lastMoveDirInput.x != inputMoveDir.x || (!moveLockedX && lastMoveTimer < 0)))
        {
            //Separate vertical movement and horizontal separately, to prevent moving diagonally without testing either side.
            if (CheckOpenTile(currPosition + Vector2Int.right * inputMoveDir.x) && (canWalkInWater || !IsWaterTile(currPosition + Vector2Int.right * inputMoveDir.x)))
            {
                transform.position += Vector3.right * inputMoveDir.x;

                if(IsWaterTile(currPosition + Vector2Int.right * inputMoveDir.x))
                {
                    inWater = true;
                    soundPlayer.PlaySound("Game.Splash");
                }
                else
                {
                    inWater = false;
                }

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
            movedThisFrame = true;
        }

        if (movedThisFrame)
        {
            lastMoveTimer = MOVE_COOLDOWN;
            CheckForInputPrompts();
        }
        else
            lastMoveTimer -= Time.deltaTime;

        lastMoveDirInput = inputMoveDir;
        ////print("(" + xDirection + ", " + yDirection + ")");
    }

    private void CheckForInputPrompts()
    {
        if (heldItem == null)
        {
            Vector2Int[] probeTiles = new Vector2Int[] {
                GetPointInFrontOfPlayer(),
                GetPointLeftOfPlayer(),
                GetPointRightOfPlayer(),
                GetPointBehindPlayer()
            };
            Item item = GetItem(probeTiles);
            if (item != null)
            {
                pickupGlyphRenderer.Activate();
            }
            else
            {
                pickupGlyphRenderer.Deactivate();
            }
        }
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

    /// <summary>
    /// Check if the chosen tile has the Water layer set to it
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsWaterTile(Vector2Int tile)
    {
        RaycastHit2D rayCast = Physics2D.Linecast(ColliderTiles.CellToWorld((Vector3Int)tile) + Vector3.one * .25f, ColliderTiles.CellToWorld((Vector3Int)tile) + Vector3.one * .75f, waterLayer);
        return rayCast;
    }

    public void PickUpItem(Item in_item = null)
    {
        bool removeKeyFlag = false;
        if (heldItem == null) removeKeyFlag = true;

        if (in_item != null) heldItem = in_item;
        else
        {
            Vector2Int[] probeTiles = new Vector2Int[] {
                GetPointInFrontOfPlayer(),
                GetPointLeftOfPlayer(), 
                GetPointRightOfPlayer(),
                GetPointBehindPlayer()
            };
            heldItem = GetItem(probeTiles);
        }

        if (heldItem != null)
        {
            //Case where you have nothing in your hands and want to pick up a key thats sitting in a slot hole.
            if (heldItem.GetComponent<Key>() != null && removeKeyFlag) heldItem.GetComponent<Key>().RemoveFromKeyhole();

            //Normal Pick Up behavior.
            heldItem.isBeingHeld = true;
            heldItem.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
            heldItem.transform.SetParent(transform, false);
            heldItem.transform.position = Vector2.right * transform.position.x + Vector2.up * (transform.position.y + .25f);
            heldItem.ActivateEffectOnPickup(this);

            soundPlayer.PlaySound("Game.ItemPickUp");

            pickupGlyphRenderer.Deactivate();
        }
    }

    public void PutDownItem()
    {
        //Get point to space in front of player.
        Vector2Int frontTile = GetPointInFrontOfPlayer();

        if (IsWaterTile(frontTile)) return;
                   
        Vector2Int[] pointInFront = { GetPointInFrontOfPlayer() };
        Item itemInFront = GetItemInFrontOfPlayer();

        // if no item, ensure tile is not a wall
        if (itemInFront == null && !CheckOpenTile(pointInFront[0])) return;
        // if item, ensure it can be swapped
        if (itemInFront != null && !itemInFront.canBePickedUp) return;

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
            heldItem.ActivateEffectOnPutDown(this);

            //Swap item for item on floor.
            if (itemInFront != null && itemInFront.canBePickedUp) PickUpItem(itemInFront);
            else heldItem = null;

            soundPlayer.PlaySound("Game.ItemDrop");
        }

        CheckForInputPrompts();
    }

    /// <summary>
    /// All get point functions are relative to the player's frontward direction
    /// </summary>

    public Vector2Int GetPointInFrontOfPlayer()
    {
        return Vector2Int.right * Mathf.FloorToInt(transform.position.x + xDirection) + 
            Vector2Int.up * Mathf.FloorToInt(transform.position.y + yDirection);
    }

    public Vector2Int GetPointBehindPlayer()
    {
        return Vector2Int.right * Mathf.FloorToInt(transform.position.x - xDirection) +
            Vector2Int.up * Mathf.FloorToInt(transform.position.y - yDirection);
    }

    public Vector2Int GetPointLeftOfPlayer()
    {
        Vector2Int forwardDirection = new Vector2Int(xDirection, yDirection);
        return new Vector2Int(
            Mathf.FloorToInt(transform.position.x + -forwardDirection.y),
            Mathf.FloorToInt((transform.position.y + forwardDirection.x)
            ));
    }

    public Vector2Int GetDiagonal(int x, int y)
    {
        Vector2Int forwardDirection = new Vector2Int(xDirection, yDirection);
        return new Vector2Int(
            Mathf.FloorToInt(transform.position.x + x),
            Mathf.FloorToInt((transform.position.y + y)
            ));
    }

    public Vector2Int GetPointRightOfPlayer()
    {
        Vector2Int forwardDirection = new Vector2Int(xDirection, yDirection);
        return new Vector2Int(
            Mathf.FloorToInt(transform.position.x + forwardDirection.y),
            Mathf.FloorToInt((transform.position.y + -forwardDirection.x)
            ));
    }

    public Item GetItem(Vector2Int[] tilesToProbe)
    {
        // get first valid item, if checking multiple tiles this should be given the one in front of the player first
        Item targetItem = null;
        foreach (Vector2Int tile in tilesToProbe)
        {
            Collider2D[] cols = Physics2D.OverlapBoxAll(Vector2.one * .5f + tile, Vector2.one * .3f, 0);
            
            foreach (Collider2D col in cols)
            {
                targetItem = col.gameObject.GetComponent<Item>();

                if (targetItem != null && targetItem.canBePickedUp)
                {
                    return targetItem;
                }
            }
        }
        return null;
    }

    // I hate this and its ugly but oh well !
    public Item GetItemInFrontOfPlayer()
    {
        Vector2Int[] pointInFront = { GetPointInFrontOfPlayer() };
        return GetItem(pointInFront);
    }


    void OnPause(InputValue _)
    {
        if (ScreenTransition.active || PopupPanel.unpausablePanelsOpen > 0) return;
        if (GameManager.paused) GameManager.instance.PlayPlaySound();
        GameManager.instance.PressPause();
    }

    void OnReset(InputValue _)
    {
        if (ScreenTransition.active || PopupPanel.unpausablePanelsOpen > 0) return;
        GameManager.instance.PressRetry();
    }

    public void Die()
    {
        spriteRenderer.sprite = deathSprite;
    }

    public void NewItemGet()
    {
        switch (AudioManager.instance.currentArea)
        {
            case AudioManager.GameArea.SANDSCAPE:
                soundPlayer.PlaySound("Game.SandscapeItemA"); // TODO: item get sound for second half is in a different key, not easy to check for
                break;
            case AudioManager.GameArea.CRYSTALSCAPE:
                soundPlayer.PlaySound("Game.CrystalscapeItem");
                break;
            case AudioManager.GameArea.GARDENSCAPE:
                soundPlayer.PlaySound("Game.GardenscapeItem");
                break;
        }

    }

    public void UnlockDoor()
    {
        soundPlayer.PlaySound("Game.UnlockDoor");
    }

    public void ShootFireball()
    {
        soundPlayer.PlaySound("Game.ShootFireball");
    }

    /// <summary>
    /// Checks if all 8 tiles adjacent to the player are collideable,
    /// if so, end the game (softlock).
    /// </summary>
    public void CheckIfWallLogged()
    {
        if (!CheckOpenTile(GetPointInFrontOfPlayer()) &&
            !CheckOpenTile(GetPointBehindPlayer()) &&
            !CheckOpenTile(GetPointLeftOfPlayer()) &&
            !CheckOpenTile(GetPointRightOfPlayer()) &&
            !CheckOpenTile(GetDiagonal(1, 1)) &&
            !CheckOpenTile(GetDiagonal(-1, -1)) &&
            !CheckOpenTile(GetDiagonal(-1, 1)) &&
            !CheckOpenTile(GetDiagonal(1, -1))
            )
        {
            GameManager.instance.Lose();
        }
    }
}