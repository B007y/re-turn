using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    List<Tile> tileSOList = new List<Tile>();
    [SerializeField] int randomTileGen = 20;


    [Header("Debug")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Tile startingTile;
    [SerializeField] Tile endingTile;


    void Awake()
    {
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

    }


    // create a 2d array of position used to place tiles on grid
    void InitGrid()
    {
        tileSize = emptyTilePrefabs.GetComponent<SpriteRenderer>().bounds.size;

        if (tiles != null) ClearGrid();

        tiles = new TileBase[(int)gridSize.x][];
        for (int i = 0; i < gridSize.x; i++)
        {
            tiles[i] = new TileBase[(int)gridSize.y];

            // spawn empty tile on every grid position
            for (int j = 0; j < gridSize.y; j++)
            {
                GameObject newObj = Instantiate(emptyTilePrefabs, new Vector2(i * (tileSize.x + gridGap.x), j * (tileSize.y + gridGap.y)), Quaternion.identity);
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

        Vector2 roundVector2 = new Vector2(Random.Range(0, 9), Random.Range(0, 9));
        PutTileOnGrid(tile, roundVector2);
        playerMovement.StartingTile = tile;
        playerMovement.SetPlayer();
    }

    void SpawnEndingTile()
    {
        GameObject newObj = Instantiate(tilePrefabs, new Vector2(0, 0), Quaternion.identity);
        TileBase tile = newObj.GetComponent<TileBase>();
        tile.Init(endingTile);

        Vector2 roundVector2 = new Vector2(Random.Range(0, 9), Random.Range(0, 9));
        while (!CheckPositionAvailability(roundVector2))
        {
            roundVector2 = new Vector2(Random.Range(0, 9), Random.Range(0, 9));
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
            tiles[(int)position.x][(int)position.y] = tile;
            tile.transform.position = new Vector2(position.x * (tileSize.x + gridGap.x), position.y * (tileSize.y + gridGap.y));
            tile.transform.SetParent(tileParent);
            // save position in tile object
            // tile.setCellPosition(position);
        }
    }

    // remove a tile from the grid at the specified position if there's a tile there
    public void RemoveTileFromGrid(Vector2 position)
    {
        if (CheckPositionOccupied(position))
        {
            // replace the tile with an empty tile
            tiles[(int)position.x][(int)position.y] = null;
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
        if (tiles[(int)position.x][(int)position.y] != null)
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
        if (tiles[(int)position.x][(int)position.y] != null)
        {
            return true;
        }

        Debug.Log("There's no tile at this position");
        return false;
    }

    public TileBase[] GetValidTilesByDirections(int[] directions, Vector2 startPosition)
    {
        TileBase[] tilesToReturn = { };

        foreach (int direction in directions)
        {
            switch (direction)
            {
                case 1:
                    {
                        if (tiles[(int)startPosition.x][(int)startPosition.y + 1] != null)
                        {
                            tilesToReturn.Append(tiles[(int)startPosition.x][(int)startPosition.y + 1]);
                        }
                        break;
                    }
                case 2:
                    {
                        if (tiles[(int)startPosition.x + 1][(int)startPosition.y] != null)
                        {
                            tilesToReturn.Append(tiles[(int)startPosition.x + 1][(int)startPosition.y]);
                        }
                        break;
                    }
                case 3:
                    {
                        if (startPosition.y == 0) break;
                        if (tiles[(int)startPosition.x][(int)startPosition.y - 1] != null)
                        {
                            tilesToReturn.Append(tiles[(int)startPosition.x][(int)startPosition.y - 1]);
                        }
                        break;
                    }
                case 4:
                    {
                        if (startPosition.x == 0) break;
                        if (tiles[(int)startPosition.x - 1][(int)startPosition.y] != null)
                        {
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
        switch (direction)
        {
            case 1:
                {
                    if (tiles[(int)startPosition.x][(int)startPosition.y + 1] != null)
                    {
                        if (tiles[(int)startPosition.x][(int)startPosition.y + 1].GetDirectionValid(3))
                            return tiles[(int)startPosition.x][(int)startPosition.y + 1];
                    }
                    break;
                }
            case 2:
                {
                    if (tiles[(int)startPosition.x + 1][(int)startPosition.y] != null)
                    {
                        if (tiles[(int)startPosition.x + 1][(int)startPosition.y].GetDirectionValid(4))
                            return tiles[(int)startPosition.x + 1][(int)startPosition.y];
                    }
                    break;
                }
            case 3:
                {
                    // if (startPosition.y == 0) break;
                    if (tiles[(int)startPosition.x][(int)startPosition.y - 1] != null)
                    {
                        if (tiles[(int)startPosition.x][(int)startPosition.y - 1].GetDirectionValid(1))
                            return tiles[(int)startPosition.x][(int)startPosition.y - 1];
                    }
                    break;
                }
            case 4:
                {
                    // if (startPosition.x == 0) break;
                    if (tiles[(int)startPosition.x - 1][(int)startPosition.y] != null)
                    {
                        if (tiles[(int)startPosition.x - 1][(int)startPosition.y].GetDirectionValid(2))
                            return tiles[(int)startPosition.x - 1][(int)startPosition.y];
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
                gridmsg += (tiles[j][i] == null ? "0" : "1");
            }
            gridmsg += "\n";
        }
        Debug.Log(gridmsg);
    }
}
