using UnityEngine;

public class DiscardPile : TileCollection
{
    public bool IsEmpty => tiles.Count == 0;

    public void RestoreAllTo(TileCollection destination)
    {
        Tile[] snapshot = new Tile[tiles.Count];
        tiles.CopyTo(snapshot);
        foreach (Tile tile in snapshot)
            TransferTo(tile, destination);
    }

    protected override void OnTileAdded(Tile tile)
    {
        // future: play discard animation / sound
    }
}
