using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SpaceShipConstants;


// ToroidalBoundsCellulerAutomaton - Grid操作を集約
public partial class ToroidalBoundsCellulerAutomaton : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    [SerializeField] int width = 30;
    [SerializeField] int height = 30;
    [SerializeField] int depth = 30;
    [SerializeField] float stepInterval = 1.3f;
    [SerializeField] float animationDuration = 0.1f;
    [System.Serializable]
    public struct InitialSpaceShipConfig
    {
        public Vector3Int position;
        public GliderDirection direction;
    }

    // 初期配置設定
    [SerializeField] InitialSpaceShipConfig[] initialSpaceShips = new InitialSpaceShipConfig[]
    {
        new InitialSpaceShipConfig { position = new Vector3Int(15, 15, 0), direction = GliderDirection.RightBackward },
    };

    Dictionary<Vector3Int, GameObject> GRID = new Dictionary<Vector3Int, GameObject>();
    HashSet<Vector3Int> currentActiveCells = new HashSet<Vector3Int>();

    private SpaceShipsManager spaceShipsManager;

    void Start()
    {
        spaceShipsManager = new SpaceShipsManager();
        GridUtils.SetGridSize(new Vector3Int(width, height, depth));
        // ここで全てのセルを非アクティブでinstantiateして初期化している
        InitializeGrid();
        // 初期セルをアクティブに変更
        HashSet<Vector3Int> InitialCells = SetupInitialSpaceShips();
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

    HashSet<Vector3Int> SetupInitialSpaceShips()
    {
        HashSet<Vector3Int> initialCells = new HashSet<Vector3Int>();
        foreach (var config in initialSpaceShips)
        {
            Vector3Int[] initialCell = spaceShipsManager.CreateSpaceShip(config.position, config.direction);
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

        // 次世代のセル位置を取得
        HashSet<Vector3Int> nextCells = spaceShipsManager.GetNextGenerationCells();

        // 新しいセルをアクティブに
        ActivateCells(nextCells);

        currentActiveCells = nextCells;
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