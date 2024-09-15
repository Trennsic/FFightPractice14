using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waymarks : MonoBehaviour
{
    public enum WaymarkSets
    {
        M4S_Hector,
        M4S_SunriseUptime
    }

    [SerializeField] private GameObject Waymark1Object;
    [SerializeField] private GameObject Waymark2Object;
    [SerializeField] private GameObject Waymark3Object;
    [SerializeField] private GameObject Waymark4Object;
    [SerializeField] private GameObject WaymarkAObject;
    [SerializeField] private GameObject WaymarkBObject;
    [SerializeField] private GameObject WaymarkCObject;
    [SerializeField] private GameObject WaymarkDObject;

    [SerializeField] private float scaleWidth = .5f;
    [SerializeField] private float scaleHeight = .5f;
    [SerializeField] private float zPosition = 1.5f;

    // Alpha value for the transparency of the waymarks
    [Range(0, 1)] public float alpha = 1f; // Value that can be changed in the Inspector

    private Vector3[] waymarkPositions;

    private void Start()
    {
        // Initialize waymark positions array
        waymarkPositions = new Vector3[8];
    }

    public void SetWaymarkUsingSets(WaymarkSets whichSet)
    {
        // Disable all waymark objects before setting them
        DisableAllWaymarks();

        // Set positions and enable relevant waymarks based on the set
        if (whichSet == WaymarkSets.M4S_Hector)
        {
            SetM4S_HectorPositions();
            ActivateWaymarks(true, true, true, true, true, true, true, true);
        }
        else if (whichSet == WaymarkSets.M4S_SunriseUptime)
        {
            SetM4S_SunriseUptimePositions();
            ActivateWaymarks(true, true, false, false, true, true, false, false);
        }
    }

    private void SetM4S_HectorPositions()
    {
        float zp = zPosition;
        // Set positions in world space for M4S_Hector
        waymarkPositions[0] = new Vector3(0, 2f, zp);       // WaymarkA - Top
        waymarkPositions[1] = new Vector3(2.25f, 2f, zp);   // Waymark2 - Top/Right
        waymarkPositions[2] = new Vector3(2.25f, 0, zp);    // WaymarkB - Right
        waymarkPositions[3] = new Vector3(2.25f, -2f, zp);  // Waymark3 - Bottom/Right
        waymarkPositions[4] = new Vector3(0, -2f, zp);      // WaymarkC - Bottom
        waymarkPositions[5] = new Vector3(-2.25f, -2f, zp); // Waymark4 - Bottom/Left
        waymarkPositions[6] = new Vector3(-2.25f, 0, zp);   // WaymarkD - Left
        waymarkPositions[7] = new Vector3(-2.25f, 2f, zp);  // Waymark1 - Top/Left

        // Update the GameObject positions and apply transparency
        UpdateWaymark(WaymarkAObject, waymarkPositions[0]);
        UpdateWaymark(Waymark2Object, waymarkPositions[1]);
        UpdateWaymark(WaymarkBObject, waymarkPositions[2]);
        UpdateWaymark(Waymark3Object, waymarkPositions[3]);
        UpdateWaymark(WaymarkCObject, waymarkPositions[4]);
        UpdateWaymark(Waymark4Object, waymarkPositions[5]);
        UpdateWaymark(WaymarkDObject, waymarkPositions[6]);
        UpdateWaymark(Waymark1Object, waymarkPositions[7]);
    }

    private void SetM4S_SunriseUptimePositions()
    {
        float zp = zPosition;
        // Set different positions for M4S_SunriseUptime if required
        waymarkPositions[0] = new Vector3(0, 6, zp);  // WaymarkA - Example
        waymarkPositions[1] = new Vector3(6, 6, zp);  // Waymark2 - Example
        waymarkPositions[4] = new Vector3(0, -6, zp); // WaymarkC - Example
        waymarkPositions[5] = new Vector3(-6, -6, zp);// Waymark4 - Example

        // Update only the active GameObject positions and apply transparency
        UpdateWaymark(WaymarkAObject, waymarkPositions[0]);
        UpdateWaymark(Waymark2Object, waymarkPositions[1]);
        UpdateWaymark(WaymarkCObject, waymarkPositions[4]);
        UpdateWaymark(Waymark4Object, waymarkPositions[5]);
    }

    private void UpdateWaymark(GameObject waymarkObject, Vector3 position)
    {
        // Move the GameObject to the specified position
        waymarkObject.transform.position = position;

        // Optionally set the scale here based on the specified scaleWidth and scaleHeight
        waymarkObject.transform.localScale = new Vector3(scaleWidth, scaleHeight, 1);

        // Apply alpha transparency to the SpriteRenderer
        SpriteRenderer spriteRenderer = waymarkObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha; // Set the alpha to the value specified in the Inspector
            spriteRenderer.color = color;
        }
    }

    private void DisableAllWaymarks()
    {
        // Disable all waymark objects
        Waymark1Object.SetActive(false);
        Waymark2Object.SetActive(false);
        Waymark3Object.SetActive(false);
        Waymark4Object.SetActive(false);
        WaymarkAObject.SetActive(false);
        WaymarkBObject.SetActive(false);
        WaymarkCObject.SetActive(false);
        WaymarkDObject.SetActive(false);
    }

    private void ActivateWaymarks(bool waymark1, bool waymark2, bool waymark3, bool waymark4,
                                  bool waymarkA, bool waymarkB, bool waymarkC, bool waymarkD)
    {
        // Activate or deactivate GameObjects based on input
        Waymark1Object.SetActive(waymark1);
        Waymark2Object.SetActive(waymark2);
        Waymark3Object.SetActive(waymark3);
        Waymark4Object.SetActive(waymark4);
        WaymarkAObject.SetActive(waymarkA);
        WaymarkBObject.SetActive(waymarkB);
        WaymarkCObject.SetActive(waymarkC);
        WaymarkDObject.SetActive(waymarkD);
    }
}
