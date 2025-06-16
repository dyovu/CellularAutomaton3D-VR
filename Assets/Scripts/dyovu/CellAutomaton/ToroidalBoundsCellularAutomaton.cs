using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using static SpaceshipConstants;


// ToroidalBoundsCellulerAutomaton - Grid操作を集約
public partial class ToroidalBoundsCellularAutomaton : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    [SerializeField] int width = 30;
    [SerializeField] int height = 30;
    [SerializeField] int depth = 30;
    [SerializeField] float stepInterval = 1.3f;
    [SerializeField] float animationDuration = 0.1f;
    [System.Serializable]
    public struct InitialSpaceshipConfig
    {
        public Vector3Int position;
        public GliderDirection direction;
        public GliderPhase phase;
    }

    // 初期配置設定
    [SerializeField] InitialSpaceshipConfig[] initialSpaceships = new InitialSpaceshipConfig[]
    {
        new InitialSpaceshipConfig { position = new Vector3Int(15, 15, 0), direction = GliderDirection.RightBackward, phase = GliderPhase.Phase1 },
    };

    Dictionary<Vector3Int, GameObject> GRID = new Dictionary<Vector3Int, GameObject>();
    HashSet<Vector3Int> currentActiveCells = new HashSet<Vector3Int>();

    private SpaceshipsManager SpaceshipsManager;

    void Start()
    {
        SpaceshipsManager = new SpaceshipsManager();
        GridUtils.SetGridSize(new Vector3Int(width, height, depth));
        // ここで全てのセルを非アクティブでinstantiateして初期化している
        InitializeGrid();
        // 初期セルをアクティブに変更
        HashSet<Vector3Int> InitialCells = SetupInitialSpaceships();
        ActivateCells(InitialCells);

        StartCoroutine(StepRoutine());
    }

    void InitializeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, this.transform);
                    Vector3Int gridPos = new Vector3Int(x, y, z);
                    GRID[gridPos] = cube;

                    cube.SetActive(false);
                    cube.transform.localScale = Vector3.zero;
                }
            }
        }
    }

    HashSet<Vector3Int> SetupInitialSpaceships()
    {
        HashSet<Vector3Int> initialCells = new HashSet<Vector3Int>();
        foreach (var config in initialSpaceships)
        {
            Vector3Int[] initialCell = SpaceshipsManager.CreateGlider(config.position, config.direction, config.phase);
            initialCells.UnionWith(initialCell);
        }
        return initialCells;
    }

    IEnumerator StepRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepInterval);
            UpdateGeneration();
        }
    }

    void UpdateGeneration()
    {
        // 現在のセルを非アクティブに
        DeactivateCurrentCells();

        // 次世代のセル位置と衝突しているグライダーの座標とIDを取得
        GliderInfo nextCellsInfo = SpaceshipsManager.GetNextGenerationWithCollisions();

        HashSet<Vector3Int> nextCells = nextCellsInfo.AllCells;
        Dictionary<Vector3Int, List<int>> collisions = nextCellsInfo.Collisions;
        Dictionary<int, Vector3Int[]> nextCellsWithId = nextCellsInfo.IDToCells;

        HashSet<Vector3Int> activeCellsWithID = ActivateCellsWithId(nextCellsWithId);

        if (activeCellsWithID.Count == nextCells.Count)
        {
            Debug.Log("All active cells are accounted for in the next generation.");
        }
        else
        {
            Debug.Log(activeCellsWithID.Except(nextCells));
        }

        // 新しいセルをアクティブに
        // ActivateCells(nextCells);
        // RemoveCollidedSpaceships(collisions);
        // 現在のアクティブセルを更新

        // currentActiveCells = nextCells;
    }




    void DeactivateCurrentCells()
    {
        foreach (var cell in currentActiveCells)
        {
            if (GRID.TryGetValue(cell, out GameObject cube))
            {
                StartCoroutine(AnimateScale(cube, Vector3.one, Vector3.zero));
            }
        }
    }

    private HashSet<Vector3Int> ActivateCellsWithId(Dictionary<int, Vector3Int[]> cells)
    {
        HashSet<Vector3Int> activeCells = new HashSet<Vector3Int>();
        foreach (var (id, cell) in cells)
        {
            Color col = SpaceshipsManager.GetActiveGliders()[id].Color;
            activeCells.UnionWith(cell);
            Debug.Log($"Activating cells for Spaceship ID {id} with color {col}");
            foreach (Vector3Int position in cell)
            {
                ActivateCellWithId(position, col);
            }
        }
        currentActiveCells = activeCells; // 現在のアクティブセルを更新
        return activeCells;
    }

    void ActivateCellWithId(Vector3Int position, Color color)
    {
        if (GRID.TryGetValue(position, out GameObject cube))
        {
            cube.SetActive(true);
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
            else
            {
                Debug.LogWarning($"Renderer not found on the cube at position {position}. Cannot set color.");
            }
            StartCoroutine(AnimateScale(cube, Vector3.zero, Vector3.one));
        }
        else
        {
            Debug.LogWarning($"Cell at {position} does not exist in the grid.");
        }
    }

    

    void ActivateCells(HashSet<Vector3Int> cells)
    {
        foreach (var cell in cells)
        {
            if (GRID.TryGetValue(cell, out GameObject cube))
            {
                cube.SetActive(true);
                StartCoroutine(AnimateScale(cube, Vector3.zero, Vector3.one));
            }
        }
        currentActiveCells = cells; 
    }


    void RemoveCollidedSpaceships(Dictionary<Vector3Int, List<int>> collisions)
    {
        foreach (var collision in collisions)
        {
            Vector3Int cell = collision.Key;
            List<int> SpaceshipIDs = collision.Value;

            // ここで衝突したスペースシップを削除
            foreach (int id in SpaceshipIDs)
            {
                SpaceshipsManager.RemoveGlider(id);
                Debug.Log($"Spaceship with ID {id} has been removed due to collision at cell {cell}.");
            }

            // セルを非アクティブにする
            if (GRID.TryGetValue(cell, out GameObject cube))
            {
                StartCoroutine(AnimateScale(cube, Vector3.one, Vector3.zero, () => cube.SetActive(false)));
            }
        }
    }


    IEnumerator AnimateScale(GameObject obj, Vector3 from, Vector3 to, System.Action onComplete = null)
    {
        float time = 0f;
        obj.transform.localScale = from;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            obj.transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        obj.transform.localScale = to;
        onComplete?.Invoke();
    }
}