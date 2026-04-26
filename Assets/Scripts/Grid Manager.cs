using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    // size of the board
    [SerializeField] Vector2Int gridSize;
    // gap between tiles
    [SerializeField] Vector2Int gridGap;
    // object size of a single tile
    [SerializeField] Vector2 tileSize;
    // 2d array of tile objects
    [SerializeField] TileBase[][] tiles;
    // prefab of the tile to spawn
    [SerializeField] GameObject emptyTilePrefabs;
    [SerializeField] GameObject tilePrefabs;
    // parent object to hold all the tile objects in hierarchy
    [SerializeField] Transform tileParent;

    [Header("Rotation")]
    [SerializeField] int rotationAngle;
    [SerializeField] int rotationRadius = 1;
    [SerializeField] GameObject rotationAnchor;
    [SerializeField] InputAction MouseLeftAction;
    [SerializeField] bool waitingForTileChoose = false;

    [Header("Starting Setup")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Tile startingTile;
    [SerializeField] Tile endingTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (MouseLeftAction.IsPressed() && waitingForTileChoose)
        {
            RotateBlock();
        }
    }

    void RotateBlock()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(mousePosition.x / (tileSize.x + gridGap.x)), Mathf.RoundToInt(mousePosition.y / (tileSize.y + gridGap.y)));

        waitingForTileChoose = false;
        if (gridPosition.x <= 0 || gridPosition.x >= gridSize.x - 1 || gridPosition.y <= 0 || gridPosition.y >= gridSize.y - 1)
        {
            Debug.Log("Clicked position is out of bounds");
            return;
        }

        // Perform rotation logic here using the chosen tile
        rotationAnchor.transform.position = new Vector2(gridPosition.x * (tileSize.x + gridGap.x), gridPosition.y * (tileSize.y + gridGap.y));

        List<TileBase> tilesToRotate = new();
        for (int i = -rotationRadius; i <= rotationRadius; i++)
        {
            for (int j = -rotationRadius; j <= rotationRadius; j++)
            {
                Vector2Int checkPosition = new Vector2Int(gridPosition.x + i, gridPosition.y + j);
                if (CheckPositionOccupied(checkPosition))
                {
                    TileBase tileToRotate = tiles[checkPosition.y][checkPosition.x];
                    tilesToRotate.Add(tileToRotate);
                    tileToRotate.transform.SetParent(rotationAnchor.transform);
                }
            }
        }

        // Rotate the anchor point
        StartCoroutine(RotateTiles(tilesToRotate, gridPosition));
    }

    // rotation animaiton, update the tiles array after the animation is done
    IEnumerator RotateTiles(List<TileBase> tilesToRotate, Vector2Int center)
    {
        float rotationTime = 0.5f; // Duration of the rotation
        float elapsedTime = 0f;

        Quaternion initialRotation = rotationAnchor.transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, -rotationAngle);

        while (elapsedTime < rotationTime)
        {
            rotationAnchor.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rotationAnchor.transform.rotation = targetRotation;

        // Detach tiles from the anchor after rotation
        foreach (TileBase tile in tilesToRotate)
            tile.transform.SetParent(tileParent);

        int blockSize = 1 + 2 * rotationRadius;
        TileBase[,] temp = new TileBase[blockSize, blockSize];
        for (int y = 0; y < blockSize; y++)
        {
            int offsetY = center.y - rotationRadius + y;
            for (int x = 0; x < blockSize; x++)
            {
                int offsetX = center.x - rotationRadius + x;
                // calculate new grid position based on the rotation
                switch (rotationAngle)
                {
                    case 90:
                        temp[blockSize - 1 - x, y] = tiles[offsetY][offsetX];
                        break;

                    case 180:
                        temp[blockSize - 1 - x, blockSize - 1 - y] = tiles[offsetY][offsetX];
                        break;

                    case 270:
                        temp[x, blockSize - 1 - y] = tiles[offsetY][offsetX];
                        break;

                    default:
                        Debug.LogError("Only 90, 180, 270 supported.");
                        yield return 0;
                        break;
                }
            }
        }

        // Copy rotated values back into original grid
        for (int y = 0; y < blockSize; y++)
        {
            int offsetY = center.y - rotationRadius + y;
            for (int x = 0; x < blockSize; x++)
            {
                int offsetX = center.x - rotationRadius + x;
                tiles[offsetY][offsetX] = temp[y, x];
                if (temp[y, x] != null)
                {
                    temp[y, x].cellPosition = new Vector2Int(offsetY, offsetX);
                }
            }
        }
        yield return 0;
    }


    // create a 2d array of position used to place tiles on grid
    void InitGrid()
    {
        tileSize = emptyTilePrefabs.GetComponent<SpriteRenderer>().bounds.size;

        if (tiles != null) ClearGrid();

        tiles = new TileBase[(int)gridSize.y][];
        for (int i = 0; i < gridSize.y; i++)
        {
            tiles[i] = new TileBase[(int)gridSize.x];

            // spawn empty tile on every grid position
            for (int j = 0; j < gridSize.x; j++)
            {
                GameObject newObj = Instantiate(emptyTilePrefabs, new Vector2(j * (tileSize.x + gridGap.x), i * (tileSize.y + gridGap.y)), Quaternion.identity);
                newObj.transform.SetParent(tileParent);

                TileBase tile = newObj.GetComponent<TileBase>();
                // PutTileOnGrid(tile, new Vector2(i, j));
            }
        }

        SpawnStartingTile();
        SpawnEndingTile();
    }

    void SpawnStartingTile()
    {
        GameObject newObj = Instantiate(tilePrefabs, new Vector2(0, 0), Quaternion.identity);
        TileBase tile = newObj.GetComponent<TileBase>();
        tile.Init(startingTile);

        Vector2 roundVector2 = new Vector2(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        PutTileOnGrid(tile, roundVector2);
        playerMovement.StartingTile = tile;
        playerMovement.SetPlayer();
    }

    void SpawnEndingTile()
    {
        GameObject newObj = Instantiate(tilePrefabs, new Vector2(0, 0), Quaternion.identity);
        TileBase tile = newObj.GetComponent<TileBase>();
        tile.Init(endingTile);

        Vector2 roundVector2 = new Vector2(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        while (!CheckPositionAvailability(roundVector2))
        {
            roundVector2 = new Vector2(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
        }
        PutTileOnGrid(tile, roundVector2);
    }

    // delete all the tile objects on the grid and clear the 2d array
    void ClearGrid()
    {
        Debug.Log("tiles length: " + tiles.GetLength(0) + " x " + tiles.GetLength(1));
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tiles[i][j] != null)
                {
                    Destroy(tiles[i][j].gameObject);
                    tiles[i][j] = null;
                }
            }
        }
    }

    // place a tile on the grid at the specified position if it's available
    public void PutTileOnGrid(TileBase tile, Vector2 position)
    {
        if (CheckPositionAvailability(position))
        {
            // place the tile on the grid
            tiles[(int)position.y][(int)position.x] = tile;
            tile.transform.position = new Vector2(position.x * (tileSize.x + gridGap.x), position.y * (tileSize.y + gridGap.y));
            tile.transform.SetParent(tileParent);
            // save position in tile object
            tile.cellPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        }
    }

    // remove a tile from the grid at the specified position if there's a tile there
    public void RemoveTileFromGrid(Vector2 position)
    {
        if (CheckPositionOccupied(position))
        {
            // replace the tile with an empty tile
            tiles[(int)position.y][(int)position.x] = null;
        }
    }

    // check if the specified position on the grid is available for placing a tile
    public bool CheckPositionAvailability(Vector2 position)
    {
        // check if the position is within the grid bounds
        if (position.x < 0 || position.x >= gridSize.x || position.y < 0 || position.y >= gridSize.y)
        {
            Debug.Log("Position is out of bounds");
            return false;
        }

        // check if there's already a tile at the position
        if (tiles[(int)position.y][(int)position.x] != null)
        {
            Debug.Log("There's already a tile at this position");
            return false;
        }

        return true;
    }

    // check if the specified position on the occupied by a tile
    bool CheckPositionOccupied(Vector2 position)
    {
        // check if the position is within the grid bounds
        if (position.x < 0 || position.x >= gridSize.x || position.y < 0 || position.y >= gridSize.y)
        {
            Debug.Log("Position is out of bounds");
            return false;
        }

        // check if there's already a tile at the position
        if (tiles[(int)position.y][(int)position.x] != null)
        {
            return true;
        }

        // Debug.Log("There's no tile at this position");
        return false;
    }

    // See: GetTileByDirection
    // This one iterates through all directions given and then returns an array
    public TileBase[] GetValidTilesByDirections(int[] directions, Vector2 startPosition)
    {
        TileBase[] tilesToReturn = { };

        foreach (int direction in directions)
        {
            switch (direction)
            {
                case 1:
                    {
                        if (startPosition.y == gridSize.y - 1) break;
                        if (tiles[(int)startPosition.x][(int)startPosition.y + 1] != null)
                        {
                            if (tiles[(int)startPosition.x][(int)startPosition.y + 1].GetDirectionValid(3))
                                tilesToReturn.Append(tiles[(int)startPosition.x][(int)startPosition.y + 1]);
                        }
                        break;
                    }
                case 2:
                    {
                        if (startPosition.x == gridSize.x - 1) break;
                        if (tiles[(int)startPosition.x + 1][(int)startPosition.y] != null)
                        {
                            if (tiles[(int)startPosition.x + 1][(int)startPosition.y].GetDirectionValid(4))
                                tilesToReturn.Append(tiles[(int)startPosition.x + 1][(int)startPosition.y]);
                        }
                        break;
                    }
                case 3:
                    {
                        if (startPosition.y == 0) break;
                        if (tiles[(int)startPosition.y - 1][(int)startPosition.x] != null)
                        {
                            if (tiles[(int)startPosition.x][(int)startPosition.y - 1].GetDirectionValid(1))
                                tilesToReturn.Append(tiles[(int)startPosition.x][(int)startPosition.y - 1]);
                        }
                        break;
                    }
                case 4:
                    {
                        if (startPosition.x == 0) break;
                        if (tiles[(int)startPosition.y][(int)startPosition.x - 1] != null)
                        {
                            if (tiles[(int)startPosition.x - 1][(int)startPosition.y].GetDirectionValid(2))
                                tilesToReturn.Append(tiles[(int)startPosition.x - 1][(int)startPosition.y]);
                        }
                        break;

                    }
            }
        }

        return tilesToReturn;
    }

    public TileBase GetTileByDirection(int direction, Vector2 startPosition)
    {
        // Checks for a tile in the array depending on the direction. 
        // If the tile is there and the position is within the array bounds then check if that tile has the connecting direction as valid
        switch (direction)
        {
            case 1:
                {
                    if (startPosition.y == gridSize.y - 1) break;
                    if (tiles[(int)startPosition.x][(int)startPosition.y + 1] != null)
                    {
                        if (tiles[(int)startPosition.y + 1][(int)startPosition.x].GetDirectionValid(3))
                            return tiles[(int)startPosition.y + 1][(int)startPosition.x];
                    }
                    break;
                }
            case 2:
                {
                    if (startPosition.x == gridSize.x - 1) break;
                    if (tiles[(int)startPosition.x + 1][(int)startPosition.y] != null)
                    {
                        if (tiles[(int)startPosition.y][(int)startPosition.x + 1].GetDirectionValid(4))
                            return tiles[(int)startPosition.y][(int)startPosition.x + 1];
                    }
                    break;
                }
            case 3:
                {
                    if (startPosition.y == 0) break;
                    if (tiles[(int)startPosition.y - 1][(int)startPosition.x] != null)
                    {
                        if (tiles[(int)startPosition.y - 1][(int)startPosition.x].GetDirectionValid(1))
                            return tiles[(int)startPosition.y - 1][(int)startPosition.x];
                    }
                    break;
                }
            case 4:
                {
                    if (startPosition.x == 0) break;
                    if (tiles[(int)startPosition.y][(int)startPosition.x - 1] != null)
                    {
                        if (tiles[(int)startPosition.y][(int)startPosition.x - 1].GetDirectionValid(2))
                            return tiles[(int)startPosition.y][(int)startPosition.x - 1];
                    }
                    break;
                }
        }
        return null;
    }

    public void PrintGrid()
    {
        string gridmsg = "\n";
        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
            {
                gridmsg += (tiles[i][j] == null ? "0" : "1");
            }
            gridmsg += "\n";
        }
        Debug.Log(gridmsg);
    }

    #region Rotation

    public void InitRotation()
    {
        MouseLeftAction.Enable();
        waitingForTileChoose = true;
    }
    #endregion

}
