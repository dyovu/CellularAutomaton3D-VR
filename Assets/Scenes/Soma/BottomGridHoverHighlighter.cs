using UnityEngine;
using Oculus.Interaction;

public class BottomGridHoverHighlighter : MonoBehaviour
{
    [SerializeField] private Transform cubePlane; // スケール(6,1,6)のCube
    [SerializeField] private Transform highlightPrefab;
    [SerializeField] private RayInteractor rayInteractor;
    [SerializeField] private ToroidalBoundsCellularAutomaton automaton;

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
        highlightPrefab.gameObject.SetActive(true);
    }

    public void OnUnhover()
    {
        isHovering = false;
        highlightPrefab.gameObject.SetActive(false);
    }

    public void OnSelect()
    {
        automaton.CreateNewGlider(lastGridPosition);
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
        Vector3 localPoint = cubePlane.InverseTransformPoint(hitPoint); // (-0.5 ~ 0.5)

        float normalizedX = localPoint.x + 0.5f;
        float normalizedZ = localPoint.z + 0.5f;

        int gridX = Mathf.Clamp(Mathf.FloorToInt(normalizedX * gridSize), 0, gridSize - 1);
        int gridZ = Mathf.Clamp(Mathf.FloorToInt(normalizedZ * gridSize), 0, gridSize - 1);
        lastGridPosition = new Vector3Int(gridX, 0, gridZ);

        float cellSize = 1f / gridSize;
        float snappedX = -0.5f + (gridX + 0.5f) * cellSize;
        float snappedZ = -0.5f + (gridZ + 0.5f) * cellSize;

        Vector3 planeScale = cubePlane.localScale;
        highlightPrefab.localScale = new Vector3(1f / planeScale.x, 0.01f, 1f / planeScale.z);
        highlightPrefab.localPosition = new Vector3(snappedX, 0.5f, snappedZ);
    }
}
