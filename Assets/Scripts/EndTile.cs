using UnityEngine;

public class EndTile : TileBase
{
    GridManager gridManagerRef;
    bool end = true;
    [SerializeField] int[] openDirections = new int[4];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManagerRef = FindAnyObjectByType<GridManager>();
        Vector3 endpoint = new Vector3(9, 5, 0);
        this.transform.SetPositionAndRotation(endpoint, this.transform.rotation);
        gridManagerRef.PutTileOnGrid(this, endpoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
