using UnityEngine;
using Oculus.Interaction;
using UnityEngine.VFX;

public class SideGridHoverHighlighter : MonoBehaviour
{
    [SerializeField] private Transform cubePlane;
    [SerializeField] private Transform highlightPrefab;
    [SerializeField] private ToroidalBoundsCellularAutomaton automaton;
    [SerializeField] private RayInteractor rayInteractor;
    [SerializeField] private VisualEffect explosionPrefab;

    private int gridSize;
    private Vector3Int lastGridPosition;

    private bool isHovering = false;

    private void Start()
    {
        gridSize = (int)cubePlane.localScale.x;
        highlightPrefab.gameObject.SetActive(false);
    }

    public void OnHover()
    {
        isHovering = true;
        highlightPrefab.gameObject.SetActive(true); // 最初に表示
    }

    public void OnUnhover()
    {
        isHovering = false;
        highlightPrefab.gameObject.SetActive(false);
    }

    public void OnSelect()
    {
        automaton.CreateNewGlider(lastGridPosition);

        // グリッド座標をワールド座標に変換
        Vector3 worldPosition = cubePlane.TransformPoint(new Vector3(
            (lastGridPosition.x + 0.5f) / gridSize - 0.5f,
            (lastGridPosition.y + 0.5f) / gridSize - 0.5f,
            0f // Z固定（面がXY平面にある想定）
        ));

        // エフェクトを生成してその位置に配置
        var explosion = Instantiate(explosionPrefab, worldPosition, Quaternion.identity);

        // エフェクトを再生（Event名はVFX Graphと一致させる）
        explosion.SendEvent("OnPlayExplosion");

        // 一定時間後に自動削除
        Destroy(explosion.gameObject, 3f);
    }

    private void Update()
    {
        if (isHovering && rayInteractor.CollisionInfo.HasValue)
        {
            UpdateHighlight(rayInteractor.CollisionInfo.Value.Point);
        }
    }

    private void UpdateHighlight(Vector3 hitPoint)
    {
        Vector3 localPoint = cubePlane.InverseTransformPoint(hitPoint);
        float normalizedX = localPoint.x + 0.5f;
        float normalizedY = localPoint.y + 0.5f;

        int gridX = Mathf.Clamp(Mathf.FloorToInt(normalizedX * gridSize), 0, gridSize - 1);
        int gridY = Mathf.Clamp(Mathf.FloorToInt(normalizedY * gridSize), 0, gridSize - 1);
        lastGridPosition = new Vector3Int(gridX, gridY, 0);

        float cellSize = 1f / gridSize;
        float snappedX = -0.5f + (gridX + 0.5f) * cellSize;
        float snappedY = -0.5f + (gridY + 0.5f) * cellSize;

        highlightPrefab.localScale = new Vector3(1f / cubePlane.localScale.x, 1f / cubePlane.localScale.y, 1f / cubePlane.localScale.y);
        highlightPrefab.localPosition = new Vector3(snappedX, snappedY, 0.7f);
    }
}
