using UnityEngine;
using Oculus.Interaction;

public class BottomGridHoverHighlighter : MonoBehaviour
{
    [SerializeField] private Transform cubePlane; // スケール(6,1,6)のCube
    [SerializeField] private Transform highlightPrefab;
    [SerializeField] private RayInteractor rayInteractor;
    [SerializeField] private ToroidalBoundsCellularAutomaton automaton;
    // [SerializeField] private int gridSize = 6;

    private int gridSize;

    private void Start()
    {
        gridSize = (int)cubePlane.localScale.x;
    }
    private void Update()
    {
        if (rayInteractor.CollisionInfo.HasValue)
        {
            var collisionInfo = rayInteractor.CollisionInfo.Value;
            Vector3 hitNormal = collisionInfo.Normal;
            Vector3 planeUp = cubePlane.up; // cubePlaneの上方向ベクトル

            if (Vector3.Dot(hitNormal, planeUp) < 0.1f) // 衝突面の法線がcubePlaneの上方向と十分に近いかチェック
            {
                highlightPrefab.gameObject.SetActive(false); // 法線が違う場合はハイライトを非表示
                return;
            }

            Vector3 hitPoint = rayInteractor.CollisionInfo.Value.Point;
            // Debug.Log($"[debug]Hit Point: {hitPoint}");


            Vector3 localPoint = cubePlane.InverseTransformPoint(hitPoint); // (-0.5 ~ 0.5)
            Debug.Log($"[debug]Local Point: {localPoint}");


            Vector3 scaledSize = cubePlane.lossyScale; // cubePlaneのXZのscaleが6,6だとしたら(6, _, 6)
            // Debug.Log($"[debug]Scaled Size: {scaledSize}");

            float normalizedX = localPoint.x + 0.5f; // 0 ~ 1
            int gridX = Mathf.Clamp(Mathf.FloorToInt(normalizedX * gridSize), 0, gridSize - 1);

            float normalizedZ = localPoint.z + 0.5f; // 0 ~ 1
            int gridZ = Mathf.Clamp(Mathf.FloorToInt(normalizedZ * gridSize), 0, gridSize - 1);

            float cellSize = 1f / gridSize;

            float snappedX = -0.5f + (gridX + 0.5f) * cellSize;
            float snappedZ = -0.5f + (gridZ + 0.5f) * cellSize;

            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                Debug.Log($"[TRIGGER] Glider placement at local (X: {snappedX}, Z: {snappedZ})");
                Vector3 snappedWorldPos = cubePlane.TransformPoint(new Vector3(snappedX, 0f, snappedZ));
                Debug.Log($"[TRIGGER] Glider placement at world position: {snappedWorldPos}");
                Vector3Int snappedGridPos = new Vector3Int(gridX, 0, gridZ);
                automaton.CreateNewGlider(snappedGridPos);
            }

            // グリッドサイズに応じてスケールを調整
            Vector3 planeScale = cubePlane.localScale;
            highlightPrefab.localScale = new Vector3(1f / planeScale.x, 0.01f, 1f / planeScale.z); // ハイライトのスケールを設定

            highlightPrefab.localPosition = new Vector3(snappedX, 0.5f, snappedZ);

            highlightPrefab.gameObject.SetActive(true);
        }
        else
        {
            highlightPrefab.gameObject.SetActive(false);
        }
    }
}
