using UnityEngine;

public class CameraScaleChanger : MonoBehaviour
{
    [SerializeField] private OVRPlayerController playerController;
    private Material _material;

    private void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    public void OnHover()
    {
        _material.EnableKeyword("_EMISSION");
        _material.SetColor("_EmissionColor", Color.white * 0.1f);
    }

    public void OnUnhover()
    {
        _material.DisableKeyword("_EMISSION");
        _material.SetColor("_EmissionColor", Color.black);
    }


    public void ChangeScaleToThree()
    {
        playerController.transform.localScale = new Vector3(3f, 3f, 3f);
    }

    public void ChangeScaleToFive()
    {
        playerController.transform.localScale = new Vector3(5f, 5f, 5f);
    }

    public void ChangeScaleToEight()
    {
        playerController.transform.localScale = new Vector3(8f, 8f, 8f);
    }

}
