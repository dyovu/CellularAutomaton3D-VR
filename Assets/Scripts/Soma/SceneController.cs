using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private Material _material;

    private void Start()
    {
        _material = GetComponent<Renderer>().material;
         _material.EnableKeyword("_EMISSION");
        _material.SetColor("_EmissionColor", Color.white * 0.1f);
    }

    public void OnHover()
    {
        _material.EnableKeyword("_EMISSION");
        _material.SetColor("_EmissionColor", Color.white * 0.3f);
    }

    public void OnUnhover()
    {
        _material.DisableKeyword("_EMISSION");
        _material.SetColor("_EmissionColor", Color.black);
    }

    public void OnSelect()
    {
        Invoke("ChangeToMainScene", 0.0f);
    }

    public void ChangeToMainScene()
    {
        SceneManager.LoadScene("VrSim_v3");
    }
}
