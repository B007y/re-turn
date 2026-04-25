using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2 gridSize;
    [SerializeField] List<TileTemp> tiles = new();
    [SerializeField] GameObject tilePrefabs;
    [SerializeField] Transform tileParent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    void InitGrid() { }

    void PutTileOnGrid(TileTemp tile, Vector2 position) { }

    void CheckPositionAvailability(Vector2 position) { }

    class TileTemp
    {
        
    }
}
