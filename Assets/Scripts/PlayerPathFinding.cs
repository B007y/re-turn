using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerPathFinding : MonoBehaviour
{
    // Getting references to find what tiles are on the grid and where the player is
    GridManager GridManagerRef;
    PlayerMovement PlayerRef;

    // Required for finding a path
    public TileBase StartingTile;
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

    public TileBase CurrentTile;
    public bool running = false;

    [Header("Optional Effects")]
    [SerializeField] bool IncreasePowerByTurn = false;
    [SerializeField] bool Teleportation = false;
    int teleportationState = 0;
    TileBase LastTileFoundFromPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridManagerRef = FindAnyObjectByType<GridManager>();
        PlayerRef = FindAnyObjectByType<PlayerMovement>();
        forceTesting.Enable();

        gameObject.SetActive(false);
        running = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(forceTesting.IsPressed())
        {
            StartPathFinding();
            forceTesting.Disable();
        }
    }

    public void StartPathFinding()
    {
        if(IncreasePowerByTurn)
        {
            MovesPerTurn++;
        }

        // Gets the object's position tile (start) and the player tile (destination)
        //Debug.Log(transform.position + " " + transform.forward);
        StartingTile = GridManagerRef.GetTileAtPosition(transform.position);
        Debug.Log(StartingTile.transform.position);
        TileBase playerTile = GridManagerRef.GetTileAtPosition(PlayerRef.transform.position);

        // If the player is reachable, it will start finding a path
        if (playerTile != null && StartingTile != null)
        {
            if (teleportationState > 0)
            {
                if (teleportationState == 1 && LastTileFoundFromPlayer != null)
                {
                    transform.position = LastTileFoundFromPlayer.transform.position;
                    CurrentTile = LastTileFoundFromPlayer;
                }
                LastTileFoundFromPlayer = playerTile;
                teleportationState--;
                return;
            }
            startingTileNode = new TileNode(StartingTile);
            Move(GetNewPath(StartingTile, playerTile));

            // if (transform.position == playerTile.transform.position)
            if (CurrentTile == PlayerRef.CurrentTile)
            {
                SceneManager.LoadScene("LoseScene");
            }
        }
    }

    List<TileNode> GetNewPath(TileBase startTile, TileBase endTile)
    {   
        return BuildPath(CreatePathFromTiles(startTile, endTile));
    }

    TileNode CreatePathFromTiles(TileBase pathStartTile, TileBase pathEndTile)
    {
        //startingTileNode.tile = pathStartTile;

        frontier = new Queue<TileNode>();
        reached = new Dictionary<Vector2, TileNode>();

        currentSearchNode = null;
        
        frontier.Enqueue(startingTileNode);
        reached.Add(pathStartTile.transform.position, startingTileNode);


        bool active = true;
        bool found = false;

        while(frontier.Count > 0 && active)
        {
            currentSearchNode = frontier.Dequeue();
            currentSearchNode.isExplored = true;
            ExploreNeighbors(currentSearchNode);

            if(currentSearchNode.tile.transform.position == pathEndTile.transform.position)
            {
                active = false;
                found = true;
            }
        }
        if (!found && Teleportation)
        {
            teleportationState = 2;
        }
        Debug.Log("End of search node neighbor" + currentSearchNode.connectedTo.tile.transform.position);
        return currentSearchNode;
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

    List<TileNode> BuildPath(TileNode desinationNode)
    {
        List<TileNode> path = new List<TileNode>();
        
        TileNode currentNode = desinationNode;

        path.Add(currentNode);
        currentNode.isPath = true;

        while (currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;
            currentNode.isPath = true;

            path.Add(currentNode);
        }

        path.Reverse();

        return path;
    }


    void Move(List<TileNode> path)
    {
        //string dMSG = "Tile Path : " + path.Count + "\n";
        //foreach (TileNode node in path)
        //{
        //    dMSG += node.tile.transform.position + "\n";
        //}
        //Debug.Log(dMSG);

        int tempMoves = MovesPerTurn;
        foreach (TileNode move in path)
        {
            if(tempMoves <= 0)
            {
                return;
            }
            transform.position = move.tile.transform.position;
            CurrentTile = move.tile;
            tempMoves--;
        }
    }

    public void InitPosition()
    {
        CurrentTile = StartingTile;
        transform.position = CurrentTile.transform.position;
    }
}
