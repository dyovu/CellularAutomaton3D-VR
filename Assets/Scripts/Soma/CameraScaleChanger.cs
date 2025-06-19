using UnityEngine;

public class CameraScaleChanger : MonoBehaviour
{
    [SerializeField] private OVRPlayerController playerController;

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
