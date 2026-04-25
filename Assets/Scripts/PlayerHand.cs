using UnityEngine;

public class PlayerHand : TileCollection
{
    public TileBase SelectedTile { get; private set; }

    public bool SelectTile(TileBase tile)
    {
        if (!Contains(tile)) return false;
        SelectedTile = tile;
        return true;
    }

    public void DeselectTile()
    {
        SelectedTile = null;
    }

    public void ReturnTile(TileBase tile)
    {
        Add(tile);
    }

    public void ConsumeTile(TileBase tile, DiscardPile discard)
    {
        if (!Contains(tile)) return;
        if (discard == null) return;
        TransferTo(tile, discard);
    }

    protected override void OnTileAdded(TileBase tile)
    {
        // future: notify UI to update hand display
    }

    protected override void OnTileRemoved(TileBase tile)
    {
        if (SelectedTile == tile) SelectedTile = null;
        // future: notify UI to update hand display
    }
}
