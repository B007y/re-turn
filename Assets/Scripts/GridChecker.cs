using UnityEngine;

public class GridChecker : MonoBehaviour
{
    public Vector2[] FilledCellPositions = new Vector2[0];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool FillCheck(Vector2 checkCell)
    {
        foreach (var cell in FilledCellPositions)
        {
            if (cell == checkCell)
            {
                return false;
            }
        }
        return true;
    }
}
