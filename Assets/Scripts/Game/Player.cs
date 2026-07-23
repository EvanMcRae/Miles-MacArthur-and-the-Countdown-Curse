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


    void Start()
    {
        AudioManager.OnBeat += OnBeat;
        heldItem = null;
    }

    void OnBeat(int beatNum)
    {
        //Debug.Log("beat #" + beatNum);
        GetComponentInChildren<SpriteRenderer>().color = beatNum % 2 == 0 ? Color.yellow : Color.white;
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
                heldItem.Usefunction(GetPointInFrontOfPlayer(), lastMoveDirInput);
            }
        }
    }

    public void HandleMovement()
    {
        Vector2 inputMove = inputSettings.actions["Move"].ReadValue<Vector2>();

        

        Vector2Int currPosition = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        Vector2Int inputMoveDir = new Vector2Int(Mathf.RoundToInt(inputMove.x), Mathf.RoundToInt(inputMove.y));

        if (inputMoveDir == default)
        {
            lastMoveDirInput = inputMoveDir;
            return;
        }

        if (inputMoveDir.y != 0 && (lastMoveDirInput.y != inputMoveDir.y || lastMoveYTimer < 0))
        {
            //Separate vertical movement and horizontal separately, to prevent moving diagonally without testing either side.
            if (CheckOpenTile(currPosition + Vector2Int.up * inputMoveDir.y))
            {
                transform.position += new Vector3(0, inputMoveDir.y, 0);
                xDirection = 0;
                yDirection = inputMoveDir.y;
                currPosition += Vector2Int.up * inputMoveDir.y;
                anim.Play("PlayerMove", 0, 0);
            }
            lastMoveYTimer = MOVE_COOLDOWN;
        }
        else
        {
            lastMoveYTimer -= Time.deltaTime;
        }

        if (inputMoveDir.x != 0 && (lastMoveDirInput.x != inputMoveDir.x || lastMoveXTimer < 0))
        {
            //Separate vertical movement and horizontal separately, to prevent moving diagonally without testing either side.
            if (CheckOpenTile((currPosition + Vector2Int.right * inputMoveDir.x)))
            {
                transform.position += new Vector3(inputMoveDir.x, 0, 0);
                yDirection = 0;
                xDirection =  inputMoveDir.x;
                anim.Play("PlayerMove", 0, 0);
            }
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
        if (in_item != null) heldItem = in_item;
        else heldItem = GetItemInFrontOfPlayer();

        if (heldItem != null)
        {
            heldItem.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
            heldItem.transform.SetParent(transform, false);
            heldItem.transform.position = new Vector2(transform.position.x, transform.position.y + .25f);
        }
    }

    public void PutDownItem()
    {
        //Get point to space in front of player.
        Vector2Int frontTile = GetPointInFrontOfPlayer();

        if (CheckOpenTile(frontTile))
        {
            Item itemInFront = GetItemInFrontOfPlayer();
            heldItem.transform.SetParent(null);
            heldItem.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
            heldItem.transform.position = Vector2.one * .5f + frontTile; //Vector2.one * .5f -> Allows you to move the sprite to the center of the tile.
            
            if (itemInFront != null) PickUpItem(itemInFront);
            else heldItem = null;
            
        }
    }

    public Vector2Int GetPointInFrontOfPlayer()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x + xDirection), Mathf.FloorToInt(transform.position.y + yDirection));
    }

    public Item GetItemInFrontOfPlayer()
    {
        //Get point to space in front of player.
        Vector2Int frontTile = GetPointInFrontOfPlayer();

        //Check tile in front of player for item.
        ////Vector2 test = Vector2.one * .5f + frontTile;
        ////Debug.DrawLine((Vector3)test, new Vector3(test.x - .3f, test.y -.3f, 0), Color.white, 5);
        Collider2D[] cols = Physics2D.OverlapBoxAll(Vector2.one * .5f + frontTile, new Vector2(.3f, .3f), 0);


        //Search for items gotten from prev method.
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.GetComponent<Item>() != null)
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

    private void OnDestroy()
    {
        AudioManager.OnBeat -= OnBeat;
    }
}