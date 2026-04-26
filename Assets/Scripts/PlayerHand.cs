using UnityEngine;

public class PlayerHand : TileCollection
{
<<<<<<< HEAD
    public TileBase SelectedTile { get; private set; }

    public bool SelectTile(TileBase tile)
=======
    public Tile SelectedTile { get; private set; }

    public bool SelectTile(Tile tile)
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        if (!Contains(tile)) return false;
        SelectedTile = tile;
        return true;
    }

    public void DeselectTile()
    {
        SelectedTile = null;
    }

<<<<<<< HEAD
    public void ReturnTile(TileBase tile)
=======
    public void ReturnTile(Tile tile)
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        Add(tile);
    }

<<<<<<< HEAD
    public void ConsumeTile(TileBase tile, DiscardPile discard)
=======
    public void ConsumeTile(Tile tile, DiscardPile discard)
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        if (!Contains(tile)) return;
        if (discard == null) return;
        TransferTo(tile, discard);
    }

<<<<<<< HEAD
    protected override void OnTileAdded(TileBase tile)
=======
    protected override void OnTileAdded(Tile tile)
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        // future: notify UI to update hand display
    }

<<<<<<< HEAD
    protected override void OnTileRemoved(TileBase tile)
=======
    protected override void OnTileRemoved(Tile tile)
>>>>>>> 80197db25b0734cba68ba23129fa9892e8de7dfd
    {
        if (SelectedTile == tile) SelectedTile = null;
        // future: notify UI to update hand display
    }
}
