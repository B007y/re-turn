using UnityEngine;

public class DiscardPile : TileCollection
{
    public bool IsEmpty => tiles.Count == 0;

    public void RestoreAllTo(TileCollection destination)
    {
        TileBase[] snapshot = new TileBase[tiles.Count];
        tiles.CopyTo(snapshot);
        foreach (TileBase tile in snapshot)
            TransferTo(tile, destination);
    }

    protected override void OnTileAdded(TileBase tile)
    {
        // future: play discard animation / sound
    }
}
