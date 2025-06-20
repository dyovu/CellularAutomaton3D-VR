using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SpaceshipConstants;

public class CellEmissionController : MonoBehaviour
{   
    [SerializeField] private float emissionMaxIntensity = 1.0f;
    [SerializeField] private float emissionIntensity = 0.1f;
    [SerializeField] private float emissionDecaySpeed = 3.0f;

    // GliderとBaysの色をオートマトン側から取得する
    // 一番下にSetEmissionColor()があるからそれで
    private Color gliderColor = new Color(169, 169, 169);
    private Color baysColor = new Color(119, 136, 153);
    
    private Dictionary<Vector3Int, float> cellEmissionLevels = new Dictionary<Vector3Int, float>();
    private Dictionary<Vector3Int, Color> cellColors = new Dictionary<Vector3Int, Color>();
    private Dictionary<Vector3Int, GameObject> gridReference;

    public void Initialize(Dictionary<Vector3Int, GameObject> grid)
    {
        gridReference = grid;
    }

    public void TriggerCellEmission(HashSet<Vector3Int> cells, SpaceshipType type)
    {
        foreach (var cell in cells)
        {
            Color emissionColor = type == SpaceshipType.Glider ? gliderColor : baysColor;
            if (GridUtils.IsInVisibleArea(cell))
            {
                cellEmissionLevels[cell] = emissionIntensity;
                cellColors[cell] = emissionColor;
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
                    cellEmissionLevels.Remove(cell);  // 辞書から削除
                                                      // マテリアルの発光も無効化
                    cellColors.Remove(cell);
                    UpdateCellEmission(cell, 0);
                    continue;   
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
                Color baseColor = cellColors.ContainsKey(position) ? cellColors[position] : gliderColor;
                Color finalEmissionColor = baseColor * emissionLevel;

                material.SetColor("_EmissionColor", finalEmissionColor);
                material.EnableKeyword("_EMISSION");
                
            }
        }
    }

    // 設定用メソッド
    public void SetEmissionColor(Color gliderColor, Color baysColor)
    {
        this.gliderColor = gliderColor;
        this.baysColor = baysColor;
    }
}