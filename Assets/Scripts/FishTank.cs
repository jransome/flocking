using UnityEngine;

public class FishTank : MonoBehaviour
{
    public float size = 400;
    public float wallThickness = 20;

    void Start()
    {
        float halfSize = size / 2;
        CreateWall(new Vector3(size, size, wallThickness), new Vector3(0, 0, -halfSize)); // front
        CreateWall(new Vector3(size, size, wallThickness), new Vector3(0, 0, halfSize)); // back
        CreateWall(new Vector3(wallThickness, size, size), new Vector3(halfSize, 0, 0)); // left
        CreateWall(new Vector3(wallThickness, size, size), new Vector3(-halfSize, 0, 0)); // right
        CreateWall(new Vector3(size, wallThickness, size), new Vector3(0, halfSize, 0)); // top
        CreateWall(new Vector3(size, wallThickness, size), new Vector3(0, -halfSize, 0)); // bottom
    }

    void CreateWall(Vector3 size, Vector3 position)
    {
        BoxCollider wall = gameObject.AddComponent<BoxCollider>();
        wall.size = size;
        wall.center = position;
    }
}
