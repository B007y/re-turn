using UnityEngine;
using System.Collections.Generic;

public class TilesObjPool : MonoBehaviour
{
    public static TilesObjPool Instance { get; private set; }
    [SerializeField] GameObject tilePrefab;

    private Queue<TileBase> tilePool = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // init pool
        int poolSize = 30;
        for (int i = 0; i < poolSize; i++)
        {
            TileBase tile = Instantiate(tilePrefab, transform).GetComponent<TileBase>();
            tile.gameObject.SetActive(false);
            tilePool.Enqueue(tile);
        }
    }

    public TileBase GetTile()
    {
        if (tilePool.Count > 0)
        {
            TileBase tile = tilePool.Dequeue();
            tile.gameObject.SetActive(true);
            return tile;
        }
        Debug.LogWarning("Tile pool is empty! Consider pre-populating the pool.");
        return null;
    }

    public void ReturnTile(TileBase tile)
    {
        tile.transform.SetParent(transform);
        tile.gameObject.SetActive(false);
        tilePool.Enqueue(tile);
    }
}