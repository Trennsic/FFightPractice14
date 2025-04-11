using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera ")]
    [SerializeField]private Camera cam;

    [Header("Camera Focus Settings")]
    public Transform focusTarget;             // The target to focus on
    public float zDistance = -10f;            // Z distance of the camera

    [Header("Image Dimensions")]
    public float targetWidth = 1000f;
    public float targetHeight = 220;

    

    #region // References 
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;
    #endregion

    void Start()
    {
           
    }

    public void InitializeCameraManager()
    {
        InitializeDebug();

        
        cam.orthographic = true; // Orthographic for precise 2D control

        AdjustOrthographicSize();
        FocusCameraOnTarget();
    }

    private void InitializeDebug() { debug = new DebugInfo(true); }

    void AdjustOrthographicSize()
    {
        cam.orthographicSize = 440f / 2f; // Half the desired height
    }



    void FocusCameraOnTarget()
    {
        if (focusTarget != null)
        {
            Vector3 newPosition = focusTarget.position;
            newPosition.z = zDistance;
            cam.gameObject.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("CameraManager: No focus target set.");
        }
    }

    #region // References 
    private GameManager GetGameManager() => gameManager;
    #endregion
}
