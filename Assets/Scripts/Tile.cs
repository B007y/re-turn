using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "Scriptable Objects/Tile")]
public class Tile : ScriptableObject
{
    [SerializeField] public string TileName;
    [SerializeField] public Sprite sprite;
    [SerializeField] public int[] openDirections;
    [SerializeField] public bool isRotationCard = false;
    [SerializeField] public bool rotateClockWise = true;
}
