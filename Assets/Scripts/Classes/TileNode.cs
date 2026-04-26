using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileNode
{
    public TileBase tile;
    public int previousDirection;
    public bool isExplored = false;
    public bool isPath = false;
    public TileNode connectedTo;

    public TileNode(TileBase initTile)
    {
        tile = initTile;
    }
}
