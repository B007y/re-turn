using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputAction LeftAction;
    [SerializeField] InputAction RightAction;
    [SerializeField] InputAction UpAction;
    [SerializeField] InputAction DownAction;

    public TileBase StartingTile;
    [SerializeField] InputAction PrintTileAction;
    GridManager gridManager;

    TileBase CurrentTile;

    [SerializeField] float TimerMax = 0.2f;
    float timer = 0;


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

        if(PrintTileAction.IsPressed() && timer <= 0)
        {
            timer = TimerMax;
            gridManager.PrintGrid();
        }
    }

    void MovePlayer(int Direction)
    {
        if (CurrentTile == null) SetPlayer();
        if (!CurrentTile.GetDirectionValid(Direction)) return;
        if (timer > 0) return;
        timer = TimerMax;

        Debug.Log("Starting Movement");

        Debug.Log("Direction: " + Direction);
        Debug.Log("Current Tile: " + CurrentTile.transform.position);
        TileBase tile = gridManager.GetTileByDirection(Direction, CurrentTile.transform.position);
        // if no tile is found, do nothing
        if (tile == null) {
            Debug.Log("Tile not found");
            return; 
        }

        Debug.Log("Tile is valid");

        Vector2 position = transform.position;
        
        switch (Direction)
        {
            case 1:
                {
                    position.y += 1;
                    break;
                }
            case 2:
                {
                    position.x += 1;
                    break;
                }
            case 3:
                {
                    position.y -= 1;
                    break;
                }
            case 4:
                {
                    position.x -= 1;
                    break;
                }
        }
        CurrentTile = tile;
        transform.position = position;
    }

    public void SetPlayer()
    {
        CurrentTile = StartingTile;
        transform.position = CurrentTile.transform.position;
    }
}
