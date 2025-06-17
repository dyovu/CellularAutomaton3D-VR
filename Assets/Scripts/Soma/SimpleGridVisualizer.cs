using UnityEngine;

public class SimpleGridVisualizer : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    [SerializeField] int width = 10;
    [SerializeField] int height = 5;
    [SerializeField] int depth = 10;
    [SerializeField] Vector3 offset = new Vector3(-8, 0, -8);

    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 position = new Vector3(x, y, z) + offset;
                    Instantiate(cubePrefab, position, Quaternion.identity, this.transform);
                }
            }
        }
    }
}
