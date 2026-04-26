using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPathFinding : MonoBehaviour
{
    // Getting references to find what tiles are on the grid and where the player is
    GridManager GridManagerRef;
    PlayerMovement PlayerRef;

    // Required for finding a path
    [SerializeField] TileBase StartingTile;
    [SerializeField] Vector2 StartingPosition;

    // How many tiles are moved per turn
    [SerializeField] int MovesPerTurn = 0;

    // Currently not connected to a trigger like the end turn button. So a button can be bound to force path finding
    [SerializeField] InputAction forceTesting;
    
    // Pathfinding variables
    TileNode startingTileNode;
    Queue<TileNode> frontier;
    Dictionary<Vector2, TileNode> reached;

    TileNode currentSearchNode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridManagerRef = FindAnyObjectByType<GridManager>();
        PlayerRef = FindAnyObjectByType<PlayerMovement>();
        forceTesting.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(forceTesting.IsPressed())
        {
            StartPathFinding();
            forceTesting.Disable();
            System.Threading.Thread.Sleep(2000);
            forceTesting.Enable();
        }
    }

    public void StartPathFinding()
    {
        // Gets the object's position tile (start) and the player tile (destination)
        StartingTile = GridManagerRef.GetTileAtPosition(transform.position);
        TileBase playerTile = GridManagerRef.GetTileAtPosition(PlayerRef.transform.position);
        
        // If the player is reachable, it will start finding a path
        if (playerTile != null)
            Move(GetNewPath(StartingTile, playerTile));
    }

    List<TileNode> GetNewPath(TileBase startTile, TileBase endTile)
    {
        CreatePathFromTiles(startTile, endTile);
        return BuildPath(endTile);
    }

    void CreatePathFromTiles(TileBase startTile, TileBase endTile)
    {
        startingTileNode.tile = startTile;

        frontier = new Queue<TileNode>();
        reached = new Dictionary<Vector2, TileNode>();

        currentSearchNode = null;
        
        frontier.Enqueue(startingTileNode);
        reached.Add(startTile.transform.position, startingTileNode);


        bool active = true;

        while(frontier.Count > 0 && active)
        {
            currentSearchNode = frontier.Dequeue();
            currentSearchNode.isExplored = true;
            ExploreNeighbors(currentSearchNode);

            if(currentSearchNode.tile.transform.position == endTile.transform.position)
            {
                active = false;
            }
        }
    }

    void ExploreNeighbors(TileNode targetTile)
    {
        List<TileNode> neighbors = new List<TileNode>();

        foreach(int direction in targetTile.tile.GetTileDirections())
        {

            TileBase foundTile = GridManagerRef.GetTileByDirection(direction, new Vector2Int((int)targetTile.tile.transform.position.x, (int)targetTile.tile.transform.position.y));
            if (foundTile != null) 
            {
                Vector2 tempCoordinates = foundTile.transform.position;
                if(!reached.ContainsKey(tempCoordinates))
                    neighbors.Add(new TileNode(foundTile)); 
            
            }
        }

        foreach (TileNode neighbor in neighbors) 
        {
            if(!reached.ContainsKey(neighbor.tile.transform.position))
            {
                neighbor.connectedTo = currentSearchNode;
                reached.Add(neighbor.tile.transform.position, neighbor);

                frontier.Enqueue(neighbor);
            }
        }
    }

    List<TileNode> BuildPath(TileBase desinationNode)
    {
        List<TileNode> path = new List<TileNode>();
        
        TileNode currentNode = new TileNode(desinationNode);

        path.Add(currentNode);
        currentNode.isPath = true;

        while (currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;
            currentNode.isPath = true;
        }

        path.Reverse();

        return path;
    }


    void Move(List<TileNode> path)
    {
        foreach (TileNode move in path)
        {
            if(MovesPerTurn-- <= 0)
            {
                return;
            }
            transform.position = move.tile.transform.position;     
        }
    }
}
