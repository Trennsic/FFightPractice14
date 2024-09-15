using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFiller : MonoBehaviour
{
    public GameObject targetObject;  // The GameObject that holds the Camera component
    public Color backgroundColor = Color.black;  // The color you want to fill the background with
    public float zOffset = 10.1f;  // The Z position offset for the background (adjustable in the inspector)

    private Camera cam;
    private GameObject backgroundQuad;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("No target GameObject set!");
            return;
        }

        // Get the Camera component from the provided GameObject
        cam = targetObject.GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("The target GameObject does not have a Camera component!");
            return;
        }

        // Create a quad that will act as the background
        backgroundQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        backgroundQuad.transform.SetParent(cam.transform);

        // Set the background color
        backgroundQuad.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Color"));
        backgroundQuad.GetComponent<Renderer>().material.color = backgroundColor;

        // Position the quad behind everything else in the camera view using zOffset from the Inspector
        backgroundQuad.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + zOffset);

        // Scale the quad to fill the entire camera view
        ResizeBackground();
    }

    void Update()
    {
        // Dynamically resize the background if the camera's orthographic size or aspect ratio changes
        ResizeBackground();
    }

    void ResizeBackground()
    {
        if (!cam.orthographic)
        {
            Debug.LogError("This script works only with Orthographic Cameras!");
            return;
        }

        // Adjust size based on the camera's orthographic size and aspect ratio
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        backgroundQuad.transform.localScale = new Vector3(camWidth, camHeight, 1f);
    }
}
