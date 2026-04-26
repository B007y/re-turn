using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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


    [Header("Debug")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Tile startingTile;
    [SerializeField] Tile endingTile;

    TileBase spawnedEndingTile;


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
                        if (tiles[(int)startPosition.x][(int)startPosition.y - 1] != null)
                        {
                            if (tiles[(int)startPosition.x][(int)startPosition.y - 1].GetDirectionValid(1))
                                tilesToReturn.Append(tiles[(int)startPosition.x][(int)startPosition.y - 1]);
                        }
                        break;
                    }
                case 4:
                    {
                        if (startPosition.x == 0) break;
                        if (tiles[(int)startPosition.x - 1][(int)startPosition.y] != null)
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
                    // if at the edge of the screen, checks if there is a tile on the opposite end to wrap to.
                    if (startPosition.y == gridSize.y - 1)
                    {
                        if (tiles[(int)startPosition.x][0] != null)
                        {
                            if (tiles[(int)startPosition.x][0].GetDirectionValid(3))
                            {
                                CheckPlayerReachedEnd(tiles[(int)startPosition.x][0]);
                                return tiles[(int)startPosition.x][0];
                            }
                        }
                        break; 
                    }
                    if (tiles[(int)startPosition.x][(int)startPosition.y + 1] != null)
                    {
                        if (tiles[(int)startPosition.x][(int)startPosition.y + 1].GetDirectionValid(3))
                        {
                            CheckPlayerReachedEnd(tiles[(int)startPosition.x][(int)startPosition.y + 1]);
                            return tiles[(int)startPosition.x][(int)startPosition.y + 1];
                        }
                    }
                    break;
                }
            case 2:
                {
                    if (startPosition.x == gridSize.x - 1)
                    {
                        if (tiles[0][(int)startPosition.y] != null)
                        {
                            if (tiles[0][(int)startPosition.y].GetDirectionValid(4))
                            {
                                CheckPlayerReachedEnd(tiles[0][(int)startPosition.y]);
                                return tiles[0][(int)startPosition.y];
                            }
                        }
                        break;
                    }
                    if (tiles[(int)startPosition.x + 1][(int)startPosition.y] != null)
                    {
                        if (tiles[(int)startPosition.x + 1][(int)startPosition.y].GetDirectionValid(4))
                        {
                            CheckPlayerReachedEnd(tiles[(int)startPosition.x + 1][(int)startPosition.y]);
                            return tiles[(int)startPosition.x + 1][(int)startPosition.y];
                        }
                    }
                    break;
                }
            case 3:
                {
                    if (startPosition.y == 0)
                    {
                        if (tiles[(int)startPosition.x][gridSize.y - 1] != null)
                        {
                            if (tiles[(int)startPosition.x][gridSize.y - 1].GetDirectionValid(1))
                            {
                                CheckPlayerReachedEnd(tiles[(int)startPosition.x][gridSize.y - 1]);
                                return tiles[(int)startPosition.x][gridSize.y - 1];
                            }
                        }
                        break;
                    }
                    if (tiles[(int)startPosition.x][(int)startPosition.y - 1] != null)
                    {
                        if (tiles[(int)startPosition.x][(int)startPosition.y - 1].GetDirectionValid(1))
                        {
                            CheckPlayerReachedEnd(tiles[(int)startPosition.x][(int)startPosition.y - 1]);
                            return tiles[(int)startPosition.x][(int)startPosition.y - 1];
                        }
                    }
                    break;
                }
            case 4:
                {
                    if (startPosition.x == 0)
                    {
                        if (tiles[gridSize.x - 1][(int)startPosition.y] != null)
                        {
                            if (tiles[gridSize.x - 1][(int)startPosition.y].GetDirectionValid(2))
                            {
                                CheckPlayerReachedEnd(tiles[gridSize.x - 1][(int)startPosition.y]);
                                return tiles[gridSize.x - 1][(int)startPosition.y];
                            }
                        }
                        break;
                    }
                    if (tiles[(int)startPosition.x - 1][(int)startPosition.y] != null)
                    {
                        if (tiles[(int)startPosition.x - 1][(int)startPosition.y].GetDirectionValid(2))
                        {
                            CheckPlayerReachedEnd(tiles[(int)startPosition.x - 1][(int)startPosition.y]);
                            return tiles[(int)startPosition.x - 1][(int)startPosition.y];
                        }
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

    public void CheckPlayerReachedEnd(TileBase tile)
    {
        if(tile.transform.position == spawnedEndingTile.transform.position)
        {
            SceneManager.LoadScene("WinScene");
        }
    }
}
