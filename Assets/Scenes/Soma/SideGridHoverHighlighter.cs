using UnityEngine;
using Oculus.Interaction;

public class SideGridHoverHighlighter : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        if (rayInteractor.CollisionInfo.HasValue)
        {
            var collisionInfo = rayInteractor.CollisionInfo.Value;
            Vector3 hitNormal = collisionInfo.Normal;
            Vector3 planeUp = cubePlane.up; // cubePlaneの上方向ベクトル

            if (Vector3.Dot(hitNormal, planeUp) > 0.9f) // 衝突面の法線がcubePlaneの上方向と十分に近いかチェック
            {
                highlightPrefab.gameObject.SetActive(false); // 法線が違う場合はハイライトを非表示
                return;
            }

            // rayInteractorの衝突情報からヒットポイントを取得
            {
                Vector3 hitPoint = rayInteractor.CollisionInfo.Value.Point;
                // Debug.Log($"[debug]Hit Point: {hitPoint}");

                Vector3 localPoint = cubePlane.InverseTransformPoint(hitPoint); // (-0.5 ~ 0.5)
                Debug.Log($"[debug]Local Point: {localPoint}");


                float normalizedX = localPoint.x + 0.5f; // 0 ~ 1
                int gridX = Mathf.Clamp(Mathf.FloorToInt(normalizedX * gridSize), 0, gridSize - 1);

                float normalizedY = localPoint.y + 0.5f; // 0 ~ 1
                int gridY = Mathf.Clamp(Mathf.FloorToInt(normalizedY * gridSize), 0, gridSize - 1);

                float cellSize = 1f / gridSize;

                float snappedX = -0.5f + (gridX + 0.5f) * cellSize;
                float snappedY = -0.5f + (gridY + 0.5f) * cellSize;

                if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                {
                    Debug.Log($"[TRIGGER] gridY: {gridY}, gridX: {gridX}");
                    Debug.Log($"[TRIGGER] Glider placement at local (X: {snappedX}, Y: {snappedY})");
                    Vector3 snappedWorldPos = cubePlane.TransformPoint(new Vector3(snappedX, snappedY, 0f));
                    Debug.Log($"[TRIGGER] Glider placement at world position: {snappedWorldPos}");
                    Vector3Int snappedGridPos = new Vector3Int(gridX, gridY, 0);
                    automaton.CreateNewGlider(snappedGridPos);
                }

                // グリッドサイズに応じてスケールを調整
                Vector3 planeScale = cubePlane.localScale;
                highlightPrefab.localScale = new Vector3(1f / planeScale.x, 0.01f, 1f / planeScale.z); // ハイライトのスケールを設定

                highlightPrefab.localPosition = new Vector3(snappedX, snappedY, 0.7f);

                highlightPrefab.gameObject.SetActive(true);
            }
        }
    }
}
