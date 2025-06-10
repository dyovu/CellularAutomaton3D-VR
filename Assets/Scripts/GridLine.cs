using UnityEngine;

public class GridLine : MonoBehaviour
{
    [SerializeField] int width = 30;
    [SerializeField] int height = 30;
    [SerializeField] int depth = 30;

    void Start()
    {
        CreateGridMesh();
    }

    void CreateGridMesh()
    {
        var mesh = new Mesh();
        var vertices = Vertices();
        int verticesCount = vertices.Length;
        var indices = Indices(verticesCount);
        
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().mesh = mesh;
    }

    Vector3[] Vertices()
    {
        // X, Y, Z軸方向の線分用の頂点を生成
        var verticesList = new System.Collections.Generic.List<Vector3>();

        // X軸方向の線分
        for (int y = 0; y <= height; y+= 5) 
        {
            for (int z = 0; z <= depth; z+= 5)
            {
                verticesList.Add(NormalizePosition(0, y, z));
                verticesList.Add(NormalizePosition(width, y, z));
            }
        }

        // Y軸方向の線分
        for (int x = 0; x <= width; x+= 5) 
        {
            for (int z = 0; z <= depth; z+= 5)
            {
                verticesList.Add(NormalizePosition(x, 0, z));
                verticesList.Add(NormalizePosition(x, height, z));
            }
        }

        // Z軸方向の線分
        for (int x = 0; x <= width; x+= 5)
        {
            for (int y = 0; y <= height; y+= 5)
            {
                verticesList.Add(NormalizePosition(x, y, 0));
                verticesList.Add(NormalizePosition(x, y, depth));
            }
        }

        return verticesList.ToArray();
    }

    Vector3 NormalizePosition(int x, int y, int z)
    {
        return new Vector3(x-0.5f, y-0.5f, z-0.5f);
    }

    int[] Indices(int verticesCount)
    {
        var indicesList = new System.Collections.Generic.List<int>();
        
        int vertexCount = 0;
        
        // 各線分は2つの頂点で構成される
        int totalLines = verticesCount/ 2;
        for (int i = 0; i < totalLines; i++)
        {
            indicesList.Add(vertexCount);
            indicesList.Add(vertexCount + 1);
            vertexCount += 2;
        }

        return indicesList.ToArray();
    }
}