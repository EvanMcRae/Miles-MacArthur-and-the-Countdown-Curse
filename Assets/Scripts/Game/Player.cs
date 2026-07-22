using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public PlayerInput inputSettings;
    public Tilemap ColliderTiles;

    Vector2Int lastMoveDirInput = default;

    const float MOVE_COOLDOWN = .15f;

    float lastMoveXTimer = MOVE_COOLDOWN;
    float lastMoveYTimer = MOVE_COOLDOWN;

    void Start()
    {
        AudioManager.OnBeat += OnBeat;
    }

    void OnBeat(int beatNum)
    {
        Debug.Log("beat #" + beatNum);
        GetComponentInChildren<SpriteRenderer>().color = beatNum % 2 == 0 ? Color.yellow : Color.white;
    }

    void Update()
    {
        HandleMovement();
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
            if (ColliderTiles.GetTile((Vector3Int)(currPosition + Vector2Int.up * inputMoveDir.y)) == null)
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
            if (ColliderTiles.GetTile((Vector3Int)(currPosition + Vector2Int.right * inputMoveDir.x)) == null)
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