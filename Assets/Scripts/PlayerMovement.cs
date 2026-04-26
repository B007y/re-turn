using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameObject charSpriteObj;
    [SerializeField] float floatingSpeed = 1f;
    [SerializeField] float floatingHeight = 0.1f;


    [SerializeField] InputAction LeftAction;
    [SerializeField] InputAction RightAction;
    [SerializeField] InputAction UpAction;
    [SerializeField] InputAction DownAction;

    public TileBase StartingTile;
    [SerializeField] InputAction PrintTileAction;
    GridManager gridManager;

    public TileBase CurrentTile;

    [SerializeField] float TimerMax = 0.2f;
    float timer = 0;
    Coroutine moveCoroutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LeftAction.Enable();
        RightAction.Enable();
        UpAction.Enable();
        DownAction.Enable();

        PrintTileAction.Enable();

        gridManager = FindAnyObjectByType<GridManager>();
        //StartingTile = GameObject.FindWithTag("Starting Tile");
        //Debug.Log(StartingTile);
    }

    // Update is called once per frame
    void Update()
    {
        if (LeftAction.IsPressed())
        {
            MovePlayer(4);
        }
        if (RightAction.IsPressed())
        {
            MovePlayer(2);
        }
        if (UpAction.IsPressed())
        {
            MovePlayer(1);
        }
        if (DownAction.IsPressed())
        {
            MovePlayer(3);
        }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (PrintTileAction.IsPressed() && timer <= 0)
        {
            timer = TimerMax;
            gridManager.PrintGrid();
        }

        // animate sprite floating
        if (charSpriteObj != null)
        {
            Vector3 position = charSpriteObj.transform.localPosition;
            position.y = (Mathf.Sin(Time.time * floatingSpeed) + 1) * floatingHeight;
            charSpriteObj.transform.localPosition = position;
        }
    }

    void MovePlayer(int Direction)
    {
        if (CurrentTile == null) SetPlayer();

        SetPlayerFacing(Direction);
        if (!CurrentTile.GetDirectionValid(Direction)) return;
        if (timer > 0) return;
        if (moveCoroutine != null) return;

        timer = TimerMax;

        Debug.Log("Starting Movement");
        TileBase tile = gridManager.GetTileByDirection(Direction, CurrentTile.cellPosition);
        // if no tile is found, do nothing
        if (tile == null)
        {
            Debug.Log("Tile not found");
            return;
        }

        Debug.Log("Tile is valid");

        //Vector2 position = transform.position;

        FMODUnity.RuntimeManager.PlayOneShot("event:/Footsteps", Vector3.zero);
        if (moveCoroutine == null)
        {
            if (CheckWalkingOutsideBorder(Direction))
                moveCoroutine = StartCoroutine(MovePlayerOutsideBorderCoroutine(tile.transform.position, Direction));
            else
                moveCoroutine = StartCoroutine(MovePlayerCoroutine(tile.transform.position));
        }
        CurrentTile = tile;
    }

    void SetPlayerFacing(int Direction)
    {
        // 1 = up | 2 = right | 3 = down | 4 = left
        switch (Direction)
        {
            case 1:
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    break;
                }
            case 2:
                {
                    transform.eulerAngles = new Vector3(0, 0, 270);
                    break;
                }
            case 3:
                {
                    transform.eulerAngles = new Vector3(0, 0, 180);
                    break;
                }
            case 4:
                {
                    transform.eulerAngles = new Vector3(0, 0, 90);
                    break;
                }
        }
    }


    bool CheckWalkingOutsideBorder(int Direction)
    {
        if (CurrentTile == null) return false;

        if (CurrentTile.cellPosition.x == 0 && Direction == 4) return true;
        if (CurrentTile.cellPosition.x == gridManager.gridSize.x - 1 && Direction == 2) return true;
        if (CurrentTile.cellPosition.y == 0 && Direction == 3) return true;
        if (CurrentTile.cellPosition.y == gridManager.gridSize.y - 1 && Direction == 1) return true;
        return false;
    }

    IEnumerator MovePlayerCoroutine(Vector2 position)
    {
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < TimerMax)
        {
            transform.position = Vector2.Lerp(startPosition, position, (elapsedTime / TimerMax));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = position;
        moveCoroutine = null;
    }

    IEnumerator MovePlayerOutsideBorderCoroutine(Vector2 targetPosition, int Direction)
    {
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;
        Vector2 midTarget = startPosition;
        switch (Direction)
        {
            case 1:
                midTarget.y += 1f;
                break;
            case 2:
                midTarget.x += 1f;
                break;
            case 3:
                midTarget.y -= 1f;
                break;
            case 4:
                midTarget.x -= 1f;
                break;
        }

        while (elapsedTime < TimerMax)
        {
            transform.position = Vector2.Lerp(startPosition, midTarget, (elapsedTime / TimerMax));
            charSpriteObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - (elapsedTime / TimerMax));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        startPosition = targetPosition;
        switch (Direction)
        {
            case 1:
                startPosition.y -= 1f;
                break;
            case 2:
                startPosition.x -= 1f;
                break;
            case 3:
                startPosition.y += 1f;
                break;
            case 4:
                startPosition.x += 1f;
                break;
        }
        elapsedTime = 0f;

        while (elapsedTime < TimerMax)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, (elapsedTime / TimerMax));
            charSpriteObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, elapsedTime / TimerMax);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        charSpriteObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        moveCoroutine = null;
    }

    public void SetPlayer()
    {
        CurrentTile = StartingTile;
        transform.position = CurrentTile.transform.position;
    }
}
