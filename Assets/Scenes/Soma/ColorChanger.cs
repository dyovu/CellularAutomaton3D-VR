using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Color originalColor;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    public void OnHoverEnter()
    {
        meshRenderer.material.color = Color.red; // ホバー時に赤色に変更
    }

    public void OnHoverExit()
    {
        meshRenderer.material.color = originalColor; // ホバー解除時に元の色に戻す
    }
}