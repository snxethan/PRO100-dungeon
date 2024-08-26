using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera battleCamera;
    [SerializeField] private Fader fader;

    public void ActivateMainCamera()
    {
        //switches from battle camera to main camera
        Debug.Log("Activating Main Camera...");
        mainCamera.gameObject.SetActive(true); 
        battleCamera.gameObject.SetActive(false);
    }

    public void ActivateBattleCamera()
    {
        //switches from main camera to battle camera
        Debug.Log("Activating Battle Camera...");
        mainCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(true);
    }
}