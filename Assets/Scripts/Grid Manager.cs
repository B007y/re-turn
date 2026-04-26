using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    // singleton 
    public static GridManager Instance { get; private set; }

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
    int rotationAngle = 90;
    [SerializeField] int rotationRadius = 1;
    [SerializeField] GameObject rotationAnchor;
    [SerializeField] InputAction MouseLeftAction;
    [SerializeField] bool waitingForTileChoose = false;
    [SerializeField] Tile rotateTileData;
    List<Tile> tileSOList = new List<Tile>();
    [SerializeField] int randomTileGen = 20;

    System.Action<int> onRotationPlayedCallback;

    [Header("Starting Setup")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Tile startingTile;
    [SerializeField] Tile endingTile;

    TileBase spawnedEndingTile;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        tileSOList = Resources.LoadAll<Tile>("TilesSO").ToList();
        Debug.Log("Loaded " + tileSOList.Count + " tile scriptable objects.");
    }

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

        if (gridPosition.x < 0 || gridPosition.x >= gridSize.x || gridPosition.y < 0 || gridPosition.y >= gridSize.y)
        {
            Debug.Log("Clicked position is out of bounds");
            return;
        }

        // Perform rotation logic here using the chosen tile
        waitingForTileChoose = false;
        Vector2Int center = new Vector2Int(gridSize.x / 2, gridSize.y / 2);
        rotationAnchor.transform.position = new Vector2(center.x * (tileSize.x + gridGap.x), center.y * (tileSize.y + gridGap.y));

        List<TileBase> tilesToRotate = new();
        for (int i = -rotationRadius; i <= rotationRadius; i++)
        {
            for (int j = -rotationRadius; j <= rotationRadius; j++)
            {
                // Vector2Int checkPosition = new Vector2Int(gridPosition.x + i, gridPosition.y + j);
                Vector2Int checkPosition = new Vector2Int(center.x + i, center.y + j);
                if (CheckPositionOccupied(checkPosition))
                {
                    TileBase tileToRotate = tiles[checkPosition.y][checkPosition.x];
                    tileToRotate.RotateTile(1, rotateTileData.rotateClockWise);
                    tilesToRotate.Add(tileToRotate);
                    tileToRotate.transform.SetParent(rotationAnchor.transform);
                }
            }
        }

        // Rotate the anchor point
        StartCoroutine(RotateTiles(tilesToRotate, center));
    }

    // rotation animaiton, update the tiles array after the animation is done
    IEnumerator RotateTiles(List<TileBase> tilesToRotate, Vector2Int center)
    {
        float rotationTime = 0.5f; // Duration of the rotation
        float elapsedTime = 0f;

        Quaternion initialRotation = rotationAnchor.transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, rotateTileData.rotateClockWise ? -rotationAngle : rotationAngle);

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
                if (rotateTileData.rotateClockWise)
                    temp[blockSize - 1 - x, y] = tiles[offsetY][offsetX];
                else
                    temp[x, blockSize - 1 - y] = tiles[offsetY][offsetX];
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

        onRotationPlayedCallback?.Invoke(0);

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
        for (int i = 0; i < randomTileGen; i++)
        {
            SpawnRandomTile();
        }
    }

    void SpawnRandomTile()
    {
        GameObject newObj = Instantiate(tilePrefabs, new Vector2(0, 0), Quaternion.identity);
        TileBase tile = newObj.GetComponent<TileBase>();
        tile.Init(tileSOList[Random.Range(0, tileSOList.Count)]);

        Vector2 roundVector2 = new Vector2(Random.Range(0, 9), Random.Range(0, 9));
        while (!CheckPositionAvailability(roundVector2))
        {
            roundVector2 = new Vector2(Random.Range(0, 9), Random.Range(0, 9));
        }
        PutTileOnGrid(tile, roundVector2);
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
        spawnedEndingTile = tile;
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
                        TileBase tile = tiles[(int)startPosition.y + 1][(int)startPosition.x];
                        if (tile != null)
                        {
                            if (tile.GetDirectionValid(3))
                                tilesToReturn.Append(tile);
                        }
                        break;
                    }
                case 2:
                    {
                        if (startPosition.x == gridSize.x - 1) break;
                        TileBase tile = tiles[(int)startPosition.y][(int)startPosition.x + 1];
                        if (tile != null)
                        {
                            if (tile.GetDirectionValid(4))
                                tilesToReturn.Append(tile);
                        }
                        break;
                    }
                case 3:
                    {
                        if (startPosition.y == 0) break;
                        TileBase tile = tiles[(int)startPosition.y - 1][(int)startPosition.x];
                        if (tile != null)
                        {
                            if (tile.GetDirectionValid(1))
                                tilesToReturn.Append(tile);
                        }
                        break;
                    }
                case 4:
                    {
                        if (startPosition.x == 0) break;
                        TileBase tile = tiles[(int)startPosition.y][(int)startPosition.x - 1];
                        if (tile != null)
                        {
                            if (tile.GetDirectionValid(2))
                                tilesToReturn.Append(tile);
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
                    // if at the edge of the screen, checks if there is a tile on the opposite end to wrap to.
                    if (startPosition.y == gridSize.y - 1)
                    {
                        TileBase tile = tiles[0][(int)startPosition.x];
                        if (tile != null)
                        {
                            if (tile.GetDirectionValid(3))
                            {
                                CheckPlayerReachedEnd(tile);
                                return tile;
                            }
                        }
                    }
                    else
                    {
                        TileBase tile = tiles[(int)startPosition.y + 1][(int)startPosition.x];
                        if (tile != null && tile.GetDirectionValid(3))
                        {
                            CheckPlayerReachedEnd(tile);
                            return tile;
                        }
                    }
                    break;
                }
            case 2:
                {
                    if (startPosition.x == gridSize.x - 1)
                    {
                        TileBase tile = tiles[(int)startPosition.y][0];
                        if (tile != null)
                        {
                            if (tile.GetDirectionValid(4))
                            {
                                CheckPlayerReachedEnd(tile);
                                return tile;
                            }
                        }
                    }
                    else
                    {
                        TileBase tile = tiles[(int)startPosition.y][(int)startPosition.x + 1];
                        if (tile != null && tile.GetDirectionValid(4))
                        {
                            CheckPlayerReachedEnd(tile);
                            return tile;
                        }
                    }
                    break;
                }
            case 3:
                {
                    if (startPosition.y == 0)
                    {
                        TileBase tile = tiles[gridSize.y - 1][(int)startPosition.x];
                        if (tile != null)
                        {
                            if (tile.GetDirectionValid(1))
                            {
                                CheckPlayerReachedEnd(tile);
                                return tile;
                            }
                        }
                        break;
                    }
                    else
                    {
                        TileBase tile = tiles[(int)startPosition.y - 1][(int)startPosition.x];
                        if (tile != null && tile.GetDirectionValid(1))
                        {
                            CheckPlayerReachedEnd(tile);
                            return tile;
                        }
                    }
                    break;
                }
            case 4:
                {
                    if (startPosition.x == 0)
                    {
                        TileBase tile = tiles[(int)startPosition.y][gridSize.x - 1];
                        if (tile != null)
                        {
                            if (tile.GetDirectionValid(2))
                            {
                                CheckPlayerReachedEnd(tile);
                                return tile;
                            }
                        }
                    }
                    else
                    {
                        TileBase tile = tiles[(int)startPosition.y][(int)startPosition.x - 1];
                        if (tile != null && tile.GetDirectionValid(2))
                        {
                            CheckPlayerReachedEnd(tile);
                            return tile;
                        }
                    }
                    break;
                }
        }
        return null;
    }

    public TileBase GetTileAtPosition(Vector2 p)
    {
        return tiles[(int)p.y][(int)p.x];
    }

    public void PrintGrid()
    {
        string gridmsg = "\n";
        for (int i = 0; i < gridSize.y; i++)
        {
            for (int j = 0; j < gridSize.x; j++)
            {
                gridmsg += (tiles[gridSize.y - 1 - i][j] == null ? "0" : "1");
            }
            gridmsg += "\n";
        }
        Debug.Log(gridmsg);
    }

    #region Rotation

    public void InitRotation(Tile tile, System.Action<int> callback)
    {
        rotateTileData = tile;
        MouseLeftAction.Enable();
        waitingForTileChoose = true;
        onRotationPlayedCallback = callback;
    }
    #endregion


    public void CheckPlayerReachedEnd(TileBase tile)
    {
        if (tile.transform.position == spawnedEndingTile.transform.position)
        {
            SceneManager.LoadScene("WinScene");
        }
    }
}
