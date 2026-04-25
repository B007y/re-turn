using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "Scriptable Objects/Tile")]
public class Tile : ScriptableObject
{
    [SerializeField] public string TileName;
    [SerializeField] public Sprite sprite;
    [SerializeField] public Vector2 cellPosition;
    [SerializeField] public int[] openDirections;
}
