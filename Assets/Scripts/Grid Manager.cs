using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // size of the board
    [SerializeField] Vector2 gridSize;
    // gap between tiles
    [SerializeField] Vector2 gridGap;
    // object size of a single tile
    [SerializeField] Vector2 tileSize;
    // 2d array of tile objects
    [SerializeField] TileTemp[][] tiles;
    // prefab of the tile to spawn
    [SerializeField] GameObject tilePrefabs;
    // parent object to hold all the tile objects in hierarchy
    [SerializeField] Transform tileParent;


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
        tileSize = tilePrefabs.GetComponent<SpriteRenderer>().bounds.size;

        if (tiles != null)  ClearGrid();

        tiles = new TileTemp[(int)gridSize.x][];
        for (int i = 0; i < gridSize.x; i++)
        {
            tiles[i] = new TileTemp[(int)gridSize.y];

            // spawn empty tile on every grid position
            for (int j = 0; j < gridSize.y; j++)
            {
                GameObject newObj = Instantiate(tilePrefabs, new Vector2(i * (tileSize.x + gridGap.x), j * (tileSize.y + gridGap.y)), Quaternion.identity);
                newObj.transform.SetParent(tileParent);

                TileTemp tile = newObj.GetComponent<TileTemp>();
                tiles[i][j] = tile;
            }
        }
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
    void PutTileOnGrid(TileTemp tile, Vector2 position) { }

    // remove a tile from the grid at the specified position if there's a tile there
    void RemoveTileFromGrid(Vector2 position) { }

    // check if the specified position on the grid is available for placing a tile
    void CheckPositionAvailability(Vector2 position) { }

    
}
