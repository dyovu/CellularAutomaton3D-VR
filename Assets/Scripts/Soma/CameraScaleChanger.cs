using UnityEngine;

public class CameraScaleChanger : MonoBehaviour
{
    [SerializeField] private OVRPlayerController playerController;

    public void ChangeScaleToFour()
    {
        playerController.transform.localScale = new Vector3(4f, 4f, 4f);
        Debug.Log("Camera scale changed to 4");
    }
}
