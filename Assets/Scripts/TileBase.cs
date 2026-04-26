using System;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class TileBase : MonoBehaviour
{
    public Tile tileData;
    public Sprite tileSprite;

    public Vector2Int cellPosition;
    [SerializeField] int[] openDirections = new int[4];
    [SerializeField] GameObject border;

    //north = 1
    //east = 2
    //south = 3
    //west = 4
    bool selected = false;
    Collider2D tileCollider;
    GridManager gridManagerRef;

    System.Action<int> onCardPlayedCallback;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileCollider = this.gameObject.GetComponent<Collider2D>();
        gridManagerRef = FindAnyObjectByType<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // follow mouse position if selected
        if (selected)
        {
            Vector3 mousePositionGet = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 10);

            this.transform.SetPositionAndRotation(Camera.main.ScreenToWorldPoint(mousePositionGet), transform.rotation);
        }
    }

    public void Init(Tile tileData, System.Action<int> OnCardPlayed = null, bool showBorder = true)
    {
        this.tileData = tileData;
        this.tileSprite = tileData.sprite;
        this.openDirections = tileData.openDirections.Clone() as int[];
        this.onCardPlayedCallback = OnCardPlayed;

        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = tileSprite;
        }
        
        if (border != null)
        {
            border.SetActive(showBorder);
        }
    }

    void OnMouseDown()
    {
        if (selected)
        {
            if (!tileData.isRotationCard)
            {
                PlaceTile();
            }
        }
    }

    public void SelectTile()
    {
        selected = true;
        if (tileData.isRotationCard)
        {
            GridManager.Instance.InitRotation(tileData, onCardPlayedCallback);
        }
    }

    public void DeselectTile()
    {
        selected = false;
    }

    public void PickTileFromGrid()
    {
        selected = !selected;
        if (cellPosition != null)
        {
            gridManagerRef.RemoveTileFromGrid(cellPosition);

        }
    }

    public void PlaceTile()
    {
        Vector2 roundVector2 = new Vector2(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));

        if (gridManagerRef.CheckPositionAvailability(roundVector2) == true)
        {
            this.transform.SetPositionAndRotation(roundVector2, transform.rotation);
            gridManagerRef.PutTileOnGrid(this, roundVector2);
            cellPosition = new Vector2Int(Mathf.RoundToInt(roundVector2.x), Mathf.RoundToInt(roundVector2.y));
            selected = !selected;


            onCardPlayedCallback?.Invoke(0);
        }
    }

    public void RotateTile(int amount, bool clockwise)
    {
        switch (clockwise)
        {
            case true:
                for (int i = 0; i < openDirections.Length; i++)
                {
                    openDirections[i] += amount;
                    if (openDirections[i] > 4) openDirections[i] = 1;
                }
                break;
            case false:
                for (int i = 0; i < openDirections.Length; i++)
                {
                    openDirections[i] -= amount;
                    if (openDirections[i] < 1) openDirections[i] = 4;
                }
                break;
        }

    }


    public int[] GetTileDirections()
    {
        return openDirections;
    }

    public bool GetDirectionValid(int direction)
    {
        for (int i = 0; i < openDirections.Length; i++)
        {
            if (openDirections[i] == direction) return true;
        }
        return false;
    }

    public static implicit operator TileBase(GameObject v)
    {
        throw new NotImplementedException();
    }
}
