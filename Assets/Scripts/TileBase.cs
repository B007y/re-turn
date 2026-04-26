using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class TileBase : MonoBehaviour
{

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

    void OnMouseDown()
    {
        if (!locked)
        {
            if (selected)
            {

                Vector2 roundVector2 = new Vector2(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));

                if(gridManagerRef.CheckPositionAvailability(roundVector2) == true)
                {
                    this.transform.SetPositionAndRotation(roundVector2 , transform.rotation);
                    gridManagerRef.PutTileOnGrid(this, roundVector2);
                    cellPosition = roundVector2;
                    selected = !selected;
                }
            }
            else
            {
                selected = !selected;
                if (cellPosition != null) 
                {
                    gridManagerRef.RemoveTileFromGrid(cellPosition);
                }
            }
            
        }
        

    }

    public void RotateTile(int amount, bool right)
    {
        switch (right)
        {
            case true: foreach(int direction in openDirections) 
                {
                    if (openDirections[direction] != 0) 
                    {
                        openDirections[direction] += amount;

                        if(openDirections[direction] < 4) 
                        { 
                            openDirections[direction] -= 4; 
                        }
                    };
                   
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
                    };

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
        for(int i = 0; i<openDirections.Length;i++)
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

