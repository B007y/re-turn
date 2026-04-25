using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class TileBase : MonoBehaviour
{

    Vector2 cellPosition;
    [SerializeField] int[] openDirections = new int[3];

    //north = 1
    //east = 2
    //south = 3
    //west = 4

    bool selected = false;
    Collider2D tileCollider;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileCollider = this.gameObject.AddComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected) 
        {
          //  this.transform.SetPositionAndRotation(, transform.localRotation);
        }
    }

    private void OnMouseDown(Collider2D tile)
    {
        selected = !selected;
        
    }

    private void RotateTile(int amount, bool right)
    {
        switch (right)
        {
            case true: foreach (int direction in openDirections) 
                { 

                }
            break;
        }
    }
}

