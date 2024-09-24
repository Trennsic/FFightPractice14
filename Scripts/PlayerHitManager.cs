using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private bool isDebugging;
    [SerializeField] private bool CheckedThisStep;
    [SerializeField] private float CheckAmountOfFramesToWait = 1f;
    [SerializeField] private bool PoFDoneThisStep;
    [SerializeField] private float PoFAmountOfFramesToWait = 3f;

    [SerializeField] private bool PlayerWasHit;
    [SerializeField] private float iconLifetime = 1.5f;
    [SerializeField] private GameObject CheckmarkIconObject;
    [SerializeField] private GameObject XIconObject;

    void Start()
    {
        ResetCheckedThisStep();
    }

    void Update()
    {

    }

    public void CheckIfPlayerIsHit()
    {
        // If already checked this step, do not check again
        if (CheckedThisStep)
        {
            if (isDebugging)
            {
                //Debug.Log("CheckIfPlayerIsHit called, but already checked this step.");
            }
            return;
        }

        // Otherwise flip the checked this step switch
        CheckedThisStep = true;

        // Start the coroutine to wait and then check if the player is hit
        StartCoroutine(WaitCheckIfPlayerIsHit());
    }

    private IEnumerator WaitCheckIfPlayerIsHit()
    {
        // Convert PoFAmountOfFramesToWait to an integer number of frames to wait
        int framesToWait = Mathf.RoundToInt(CheckAmountOfFramesToWait);

        // Wait for the specified number of frames
        for (int i = 0; i < framesToWait; i++)
        {
            yield return null; // Wait for one frame
        }

        // Get the main camera
        Camera mainCamera = Camera.main;

        if (mainCamera == null || player == null)
        {
            if (isDebugging)
            {
                Debug.LogError("Main Camera or Player is not set.");
            }
            yield break; // Exit the coroutine early if the icons are not set
        }

        // Get player's position
        Vector3 playerPosition = player.transform.position;

        // Create a ray starting from the camera's position but with player's x and y, pointing forward
        Ray ray = new Ray(new Vector3(playerPosition.x, playerPosition.y, mainCamera.transform.position.z), Vector3.forward);

        // Draw the ray in the Scene view if debugging is enabled
        if (isDebugging)
        {
            Debug.DrawRay(ray.origin, ray.direction * 30f, Color.red, 2.0f);
            Debug.Log($"Ray Origin: {ray.origin}, Direction: {ray.direction}");
        }

        // Check if the ray hits an object with the tag "DamagerMarker"
        if (Physics.Raycast(ray, out RaycastHit hit, 30f))
        {
            if (isDebugging)
            {
                Debug.Log($"Ray hit: {hit.collider.name}, Tag: {hit.collider.tag}");
            }

            if (hit.collider.CompareTag("DamageMarker"))
            {
                Debug.Log($"Player was Hit!");
                PlayerWasHit = true;
                yield break; // Exit the coroutine early if the icons are not set
            }
        }
        if (isDebugging)
            Debug.Log($"Player was not Hit by anything");
    }

    public bool CheckIfPlayerWasHit()
    {
        return PlayerWasHit;
    }

    public void ResetCheckedThisStep()
    {
        if (isDebugging)
        {
            Debug.Log("ResetCheckedThisStep called.");
        }

        CheckedThisStep = false;
        PlayerWasHit = false;
        PoFDoneThisStep = false;
    }

    public void ShowPassOrFail()
    {
        // If pass or fail was done this step return
        if (PoFDoneThisStep) return;
        PoFDoneThisStep = true;

        // Start coroutine to wait for specified frames before showing pass or fail
        StartCoroutine(WaitAndShowPassOrFail());
    }

    private IEnumerator WaitAndShowPassOrFail()
    {
        // Convert PoFAmountOfFramesToWait to an integer number of frames to wait
        int framesToWait = Mathf.RoundToInt(PoFAmountOfFramesToWait);

        // Wait for the specified number of frames
        for (int i = 0; i < framesToWait; i++)
        {
            yield return null; // Wait for one frame
        }

        // Make sure both CheckmarkIconObject and XIconObject are set
        if (CheckmarkIconObject == null || XIconObject == null)
        {
            if (isDebugging)
            {
                Debug.LogError("CheckmarkIconObject or XIconObject is not set.");
            }
            yield break; // Exit the coroutine early if the icons are not set
        }

        // Check if the player was hit
        if (PlayerWasHit)
        {
            // Create XIconObject at player.position with an offset, and destroy it after iconLifetime
            Vector3 offset = new Vector3(0, 0.75f, 0); // Adjust the offset as needed
            GameObject xIcon = Instantiate(XIconObject, player.transform.position + offset, Quaternion.identity);
            Destroy(xIcon, iconLifetime);

            if (isDebugging)
            {
                Debug.Log("Player was hit, showing X icon.");
            }
        }
        else
        {
            // Create CheckmarkIconObject at player.position with an offset, and destroy it after iconLifetime
            Vector3 offset = new Vector3(0, 0.75f, 0); // Adjust the offset as needed
            GameObject checkmarkIcon = Instantiate(CheckmarkIconObject, player.transform.position + offset, Quaternion.identity);
            Destroy(checkmarkIcon, iconLifetime);

            if (isDebugging)
            {
                Debug.Log("Player was not hit, showing Checkmark icon.");
            }
        }
    }
}
