using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;
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
    [SerializeField] VisualEffect gliderCollideEffect;
    [SerializeField] VisualEffect bayCollideEffect;
    [System.Serializable]
    public struct InitialSpaceshipConfig
    {
        public Vector3Int position;
        public GliderDirection direction;
        public GliderPhase phase;
    }

    // 初期配置設定
    [SerializeField] InitialSpaceshipConfig[] initialSpaceships;

    Dictionary<Vector3Int, GameObject> GRID = new Dictionary<Vector3Int, GameObject>();
    HashSet<Vector3Int> currentGliderCells = new HashSet<Vector3Int>();
    HashSet<Vector3Int> currentBaysCells = new HashSet<Vector3Int>();

    private SpaceshipsManager SpaceshipsManager;

    void Start()
    {
        SpaceshipsManager = new SpaceshipsManager();
        GridUtils.SetGridSize(new Vector3Int(width, height, depth));
        // ここで全てのセルを非アクティブでinstantiateして初期化している
        InitializeGrid();
        // 初期セルをアクティブに変更
        Dictionary<int, Vector3Int[]> InitialCells = SetupInitialSpaceships();
        HashSet<Vector3Int>initialCells = ActivateGlidersWithId(InitialCells);
        currentGliderCells = initialCells;

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

    Dictionary<int, Vector3Int[]> SetupInitialSpaceships()
    {
        Dictionary<int, Vector3Int[]> initialCells = new Dictionary<int, Vector3Int[]>();
        foreach (var config in initialSpaceships)
        {
            (Vector3Int[] initialCell, int id) = SpaceshipsManager.CreateGlider(config.position);
            initialCells[id] = initialCell;
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

        // 次世代のグライダーの位置と衝突しているグライダーの座標とIDを取得
        GliderInfo gliderInfo = SpaceshipsManager.GetGliderInfo();

        HashSet<Vector3Int> gliderCells = gliderInfo.AllCells; // 今は使いません
        Dictionary<Vector3Int, List<int>> gliderCollisions = gliderInfo.Collisions;
        Dictionary<int, Vector3Int[]> gliderCellsWithId = gliderInfo.IDToCells;

        // 衝突したグライダー削除し、ベイを作成
        RemoveCollidedGliders(gliderCollisions, gliderCellsWithId);
        Debug.Log($"Collisions found: {gliderCollisions.Count}");
        CreateBays(gliderCollisions);

        // 次世代のbaysの位置と衝突しているグライダーの座標を取得
        BaysInfo baysInfo = SpaceshipsManager.GetBaysInfo();
        HashSet<Vector3Int> BaysCells = baysInfo.AllCells; // 今は使いません
        Dictionary<Vector3Int, List<int>> baysCollisions = baysInfo.Collisions;
        Dictionary<int, Vector3Int[]> BaysCellsWithId = baysInfo.IDToCells;

        // 衝突したベイ削除し、ベイを作成
        RemoveCollidedBays(baysCollisions, BaysCellsWithId);


        HashSet<Vector3Int> activeGlider = ActivateGlidersWithId(gliderCellsWithId);
        HashSet<Vector3Int> activeBays = ActivateBaysWithId(BaysCellsWithId);


        currentGliderCells = activeGlider;
        currentBaysCells = activeBays;
    }

    public void CreateNewGlider(Vector3Int CenterPosition)
    {
        (Vector3Int[] initialCell, int id)  = SpaceshipsManager.CreateGlider(CenterPosition);
    }


    void DeactivateCurrentCells()
    {
        foreach (var cell in currentGliderCells)
        {
            if (!GridUtils.IsInVisibleArea(cell)) continue; // return → continue
            if (GRID.TryGetValue(cell, out GameObject cube))
            {
                StartCoroutine(AnimateScale(cube, Vector3.one, Vector3.zero));
            }
        }
        foreach (var cell in currentBaysCells)
        {
            if (!GridUtils.IsInVisibleArea(cell)) continue; // return → continue
            if (GRID.TryGetValue(cell, out GameObject cube))
            {
                StartCoroutine(AnimateScale(cube, Vector3.one, Vector3.zero));
            }
        }
    }

    

    private HashSet<Vector3Int> ActivateGlidersWithId(Dictionary<int, Vector3Int[]> cells)
    {
        HashSet<Vector3Int> activeCells = new HashSet<Vector3Int>();
        foreach (var (id, cell) in cells)
        {
            if (SpaceshipsManager.GetActiveGliders().ContainsKey(id))
            {
                Color col = SpaceshipsManager.GetActiveGliders()[id].Color;
                foreach (Vector3Int position in cell)
                {
                    if (GridUtils.IsInVisibleArea(position)) // 表示領域のみ追加
                    {
                        activeCells.Add(position);
                    }
                    ActivateCellWithId(position, col);
                }
            }
        }
        return activeCells;
    }

    private HashSet<Vector3Int> ActivateBaysWithId(Dictionary<int, Vector3Int[]> cells)
    {
        HashSet<Vector3Int> activeCells = new HashSet<Vector3Int>();
        foreach (var (id, cell) in cells)
        {
            Color col = Color.black;
            foreach (Vector3Int position in cell)
            {
                if (GridUtils.IsInVisibleArea(position)) // 表示領域のみ追加
                {
                    activeCells.Add(position);
                }
                ActivateCellWithId(position, col);
            }
        }
        return activeCells;
    }

    void ActivateCellWithId(Vector3Int position, Color color)
    {
        if (!GridUtils.IsInVisibleArea(position)) return;

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
                // 色を白に設定
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white;
                }
                StartCoroutine(AnimateScale(cube, Vector3.zero, Vector3.one));
            }
        }
        // 表示領域のセルのみ追跡
        currentGliderCells = cells.Where(cell => GridUtils.IsInVisibleArea(cell)).ToHashSet();
    }

    


    void RemoveCollidedGliders(Dictionary<Vector3Int, List<int>> collisions, Dictionary<int, Vector3Int[]> idToCells)
    {
        foreach (var collision in collisions)
        {
            List<int> GliderIDs = collision.Value;
            foreach (int id in GliderIDs)
            {
                SpaceshipsManager.RemoveGlider(id);
                idToCells.Remove(id);
            }
        }
    }

    void RemoveCollidedBays(Dictionary<Vector3Int, List<int>> collisions, Dictionary<int, Vector3Int[]> idToCells)
    {
        foreach (var collision in collisions)
        {
            Vector3Int position = collision.Key;

            // 
            var effectInstance = Instantiate(bayCollideEffect, position, Quaternion.identity);
            effectInstance.SendEvent("OnBayCollide");
            Destroy(effectInstance.gameObject, 3f);

            List<int> BaysIDs = collision.Value;
            foreach (int id in BaysIDs)
            {
                SpaceshipsManager.RemoveBays(id);
                idToCells.Remove(id);
            }
        }
    }

    void CreateBays(Dictionary<Vector3Int, List<int>> collisions)
    {
        HashSet<int> processedGliders = new HashSet<int>();

        foreach (var collision in collisions)
        {
            List<int> gliderIDs = collision.Value;
            bool alreadyProcessed = gliderIDs.Any(id => processedGliders.Contains(id));

            if (!alreadyProcessed)
            {
                Vector3Int centerPosition = collision.Key;

                // ★ ここで1回だけインスタンス化
                var effectInstance = Instantiate(gliderCollideEffect, centerPosition, Quaternion.identity);
                effectInstance.SendEvent("OnGliderCollide");
                Destroy(effectInstance.gameObject, 3f);

                SpaceshipsManager.CreateBays(centerPosition, BaysDirection.Up);

                foreach (int id in gliderIDs)
                {
                    processedGliders.Add(id); // これで次回以降は出さない
                }
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