using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class TileBase : MonoBehaviour
{
    public string tileName;
    public Sprite tileSprite;
    Vector2 cellPosition;
    [SerializeField] int[] openDirections = new int[4];

    //north = 1
    //east = 2
    //south = 3
    //west = 4
    bool selected = false;
    Collider2D tileCollider;
    GridManager gridManagerRef;

    bool locked = false;
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
        if (selected)
        {
            Vector3 mousePositionGet = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 10);

            this.transform.SetPositionAndRotation(Camera.main.ScreenToWorldPoint(mousePositionGet), transform.rotation);
        }
    }

    public void Init(TileData tileData, System.Action<int> OnCardPlayed = null)
    {
        this.tileName = tileData.tileName;
        this.tileSprite = tileData.tileSprite;
        this.openDirections = tileData.openDirections;
        this.onCardPlayedCallback = OnCardPlayed;

        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = tileSprite;
        }
    }

    void OnMouseDown()
    {
        if (!locked)
        {
            if (selected)
            {
                PlaceTile();
            }
            else
            {
                PickTileFromGrid();
            }

        }


    }

    public void SelectTile()
    {
        selected = true;
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
            cellPosition = roundVector2;
            selected = !selected;

            onCardPlayedCallback?.Invoke(0);
        }
    }

    public void RotateTile(int amount, bool right)
    {
        switch (right)
        {
            case true:
                foreach (int direction in openDirections)
                {
                    if (openDirections[direction] != 0)
                    {
                        openDirections[direction] += amount;

                        if (openDirections[direction] < 4)
                        {
                            openDirections[direction] -= 4;
                        }
                    }
                    ;

                }
                break;
            case false:
                foreach (int direction in openDirections)
                {
                    if (openDirections[direction] != 0)
                    {
                        openDirections[direction] -= amount;

                        if (openDirections[direction] > 4)
                        {
                            openDirections[direction] += 4;
                        }
                    }
                    ;

                }
                break;
        }
    }

}

