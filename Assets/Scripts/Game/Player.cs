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

    public Camera cam;

    public LayerMask collidersLayer;

    int xDirection = 1;
    int yDirection = 0;

    public GameObject heldItem;

    void Start()
    {
        AudioManager.OnBeat += OnBeat;
        heldItem = null;
    }

    void OnBeat(int beatNum)
    {
        Debug.Log("beat #" + beatNum);
        GetComponentInChildren<SpriteRenderer>().color = beatNum % 2 == 0 ? Color.yellow : Color.white;
    }

    void Update()
    {
        HandleMovement();
        if (inputSettings.actions["Interact"].WasPressedThisFrame())
        {
            if (heldItem == null) PickUpItem();
            else PutDownItem();
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



        if (lastMoveDirInput.y != inputMoveDir.y || lastMoveYTimer < 0)
        {
            //Separate vertical movement and horizontal separately, to prevent moving diagonally without testing either side
            if (checkOpenTile(currPosition + Vector2Int.up * inputMoveDir.y))
            {
                transform.position += new Vector3(0, inputMoveDir.y, 0);
                currPosition += Vector2Int.up * inputMoveDir.y;
            }
            lastMoveYTimer = MOVE_COOLDOWN;
        }
        else
        {
            lastMoveYTimer -= Time.deltaTime;
        }

        if (lastMoveDirInput.x != inputMoveDir.x || lastMoveXTimer < 0)
        {
            //Separate vertical movement and horizontal separately, to prevent moving diagonally without testing either side
            if (checkOpenTile((currPosition + Vector2Int.right * inputMoveDir.x)))
            {
                transform.position += new Vector3(inputMoveDir.x, 0, 0);
            }
            lastMoveXTimer = MOVE_COOLDOWN;
        }
        else
        {
            lastMoveXTimer -= Time.deltaTime;
        }

            lastMoveDirInput = inputMoveDir;
    }

    //Checks if you can collide into a tile or not.
    public bool checkOpenTile(Vector2Int tile)
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

    public void PickUpItem()
    {
        //Get point to space in front of player.
        Vector2Int frontTile = GetPointInFrontOfPlayer();

        //Check tile in front of player for item.
        Collider2D[] cols =  Physics2D.OverlapBoxAll(frontTile, new Vector2(1, 1), 0);

        //Search for items gotten from prev method.
        foreach (Collider2D col in cols)
        {
            if(col.gameObject.GetComponent<Item>() != null)
            {
                col.gameObject.GetComponent<Item>().PickUp(gameObject);
                heldItem = col.gameObject;
                break;
            }
        }
    }

    public void PutDownItem()
    {
        heldItem.GetComponent<Item>().PutDown(gameObject);
        heldItem = null;
    }

    public Vector2Int GetPointInFrontOfPlayer()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x + xDirection), Mathf.FloorToInt(transform.position.y + yDirection));
    }


    void OnPause(InputValue _)
    {
        if (PopupPanel.unpausablePanelsOpen > 0) return;
        GameManager.instance.PressPause();
    }

    void OnDestroy()
    {
        AudioManager.OnBeat -= OnBeat;
    }
}