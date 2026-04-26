using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerPathFinding : MonoBehaviour
{
    GridManager GridManagerRef;
    [SerializeField] TileBase StartingTile;
    [SerializeField] Vector2 StartingPosition;
    TileNode tilePathing;
    Stack<Vector2> path;
    int lastTravelled = 0;

    Queue<TileNode> frontier = new Queue<TileNode>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridManagerRef = FindAnyObjectByType<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPathFinding()
    {
        tilePathing.tile = StartingTile;
        PopulateTileLinkedList(GridManagerRef.GetValidTilesByDirections(StartingTile.GetTileDirections(), StartingPosition));
        CreatePathFromTiles();
        Move();
    }

    void PopulateTileLinkedList(TileBase[] tiles)
    {
        bool active = true;
        while(frontier.Count > 0 && active)
        {

        }
    }

    void CreatePathFromTiles()
    {

    }

    void Move()
    {
        foreach (Vector2 move in path)
        {
            transform.position = (Vector2)transform.position + move;
            System.Threading.Thread.Sleep(500);        
        }
    }
}
