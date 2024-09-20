using UnityEngine;

public class ProjectorManager : MonoBehaviour
{
    private Projector projector;
    private Camera mainCamera;

    void Start()
    {
        // Find the Projector component on the current GameObject
        projector = GetComponent<Projector>();

        if (projector == null)
        {
            Debug.LogError("No Projector component found on this GameObject.");
            return;
        }

        // Find the Main Camera
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No Main Camera found in the scene.");
            return;
        }
    }

    void Update()
    {
        if (mainCamera != null && projector != null)
        {
            // Set the Projector's position and rotation to match the Main Camera
            projector.transform.position = mainCamera.transform.position;
            projector.transform.rotation = mainCamera.transform.rotation;
        }
    }
}
