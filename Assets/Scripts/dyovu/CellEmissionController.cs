using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellEmissionController : MonoBehaviour
{
    [SerializeField] private Color emissionColor = Color.white;
    [SerializeField] private float emissionIntensity = 2.0f;
    [SerializeField] private float emissionDecaySpeed = 3.0f;
    
    private Dictionary<Vector3Int, float> cellEmissionLevels = new Dictionary<Vector3Int, float>();
    private Dictionary<Vector3Int, GameObject> gridReference;

    public void Initialize(Dictionary<Vector3Int, GameObject> grid)
    {
        gridReference = grid;
    }

    public void TriggerCellEmission(HashSet<Vector3Int> cells)
    {
        foreach (var cell in cells)
        {
            if (GridUtils.IsInVisibleArea(cell))
            {
                cellEmissionLevels[cell] = emissionIntensity;
            }
        }
    }

    void Update()
    {
        UpdateEmissionLevels();
    }

    private void UpdateEmissionLevels()
    {
        var keysToUpdate = cellEmissionLevels.Keys.ToList();
        foreach (var cell in keysToUpdate)
        {
            if (cellEmissionLevels[cell] > 0)
            {
                cellEmissionLevels[cell] -= emissionDecaySpeed * Time.deltaTime;
                if (cellEmissionLevels[cell] <= 0)
                {
                    cellEmissionLevels[cell] = 0;
                }
                UpdateCellEmission(cell, cellEmissionLevels[cell]);
            }
        }
    }

    private void UpdateCellEmission(Vector3Int position, float emissionLevel)
    {
        if (gridReference.TryGetValue(position, out GameObject cube))
        {
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                Color finalEmissionColor = emissionColor * emissionLevel;
                material.SetColor("_EmissionColor", finalEmissionColor);
                material.EnableKeyword("_EMISSION");
            }
        }
    }

    // 設定用メソッド
    public void SetEmissionParameters(Color color, float intensity, float decaySpeed)
    {
        emissionColor = color;
        emissionIntensity = intensity;
        emissionDecaySpeed = decaySpeed;
    }
}