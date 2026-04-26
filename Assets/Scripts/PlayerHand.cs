using UnityEngine;

public class PlayerHand : TileCollection
{
    public Tile SelectedTile { get; private set; }

    public bool SelectTile(Tile tile)
    {
        if (!Contains(tile)) return false;
        SelectedTile = tile;
        return true;
    }

    public void DeselectTile()
    {
        SelectedTile = null;
    }

    public void ReturnTile(Tile tile)
    {
        Add(tile);
    }

    public void ConsumeTile(Tile tile, DiscardPile discard)
    {
        if (!Contains(tile)) return;
        if (discard == null) return;
        TransferTo(tile, discard);
    }

    protected override void OnTileAdded(Tile tile)
    {
        // future: notify UI to update hand display
    }

    protected override void OnTileRemoved(Tile tile)
    {
        if (SelectedTile == tile) SelectedTile = null;
        // future: notify UI to update hand display
    }
}
