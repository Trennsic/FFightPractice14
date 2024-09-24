using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMarkerManager : MonoBehaviour
{
    [SerializeField] private float setPositionZ = -0.1f;
    [SerializeField] private bool isDebugging;
    [SerializeField] private GameObject damageMarkerObjectPrefab; // Prefab with DamageMarkerAddon attached

    // Define custom orange color
    private Color customOrange = new Color(1f, 0.5f, 0f);

    // Method to create a circle damage marker
    public void CreateDamageMarkerCircle(Vector3 position, float radius, float lifetime, bool isInvisible = false)
    {
        GameObject damageMarker = Instantiate(damageMarkerObjectPrefab, new Vector3(position.x, position.y, setPositionZ), Quaternion.identity);
        DamageMarkerAddon addon = damageMarker.GetComponent<DamageMarkerAddon>();

        if (addon != null)
        {
            addon.SetMarketType(DamageMarkerAddon.MarkerTypes.Circle);
            Transform sphereTransform = addon.damageMarkerSphereObject.transform;

            // Set the position, scale, and rotation
            damageMarker.transform.localRotation = Quaternion.Euler(0, 180, 0);
            sphereTransform.localScale = new Vector3(radius, radius, 1f); // Keeping Z scale 1 as it's only a flat 2D circle.
            addon.SetLifetime(lifetime);

            // Disable MeshRenderer if isInvisible is true
            if (isInvisible)
            {
                MeshRenderer renderer = addon.damageMarkerSphereObject.GetComponent<MeshRenderer>();
                if (renderer != null) renderer.enabled = false;
            }
        }
    }

    // Method to create a rectangle damage marker
    public void CreateDamageMarkerRectangle(Vector3 position, float width, float height, float lifetime, bool isInvisible = false)
    {
        GameObject damageMarker = Instantiate(damageMarkerObjectPrefab, new Vector3(position.x, position.y, setPositionZ), Quaternion.identity);
        DamageMarkerAddon addon = damageMarker.GetComponent<DamageMarkerAddon>();

        if (addon != null)
        {
            addon.SetMarketType(DamageMarkerAddon.MarkerTypes.Rectangle);
            Transform boxTransform = addon.damageMarkerBoxObject.transform;

            // Set the position, scale, and rotation
            boxTransform.localScale = new Vector3(width, height, 0.1f); // Z scale is 0.1f as specified
            addon.SetLifetime(lifetime);

            // Disable MeshRenderer if isInvisible is true
            if (isInvisible)
            {
                MeshRenderer renderer = addon.damageMarkerBoxObject.GetComponent<MeshRenderer>();
                if (renderer != null) renderer.enabled = false;
            }
        }
    }

    // Method to create a cone damage marker
    public void CreateDamageMarkerCone(Vector3 position, float direction, float length, float angle, float lifetime, bool isInvisible = false)
    {
        GameObject damageMarker = Instantiate(damageMarkerObjectPrefab, new Vector3(position.x, position.y, setPositionZ), Quaternion.identity);
        DamageMarkerAddon addon = damageMarker.GetComponent<DamageMarkerAddon>();

        if (addon != null)
        {
            addon.SetMarketType(DamageMarkerAddon.MarkerTypes.Cone);
            Transform coneTransform = addon.damageMarkerPrismObject.transform;

            // Clear existing prisms if any
            foreach (Transform child in addon.damageMarkerPrismObject.transform)
            {
                Destroy(child.gameObject);
            }

            // Set the base prism object scale
            coneTransform.localScale = new Vector3(length, length, 1f); // Scale for each individual prism

            // Create and rotate the prism objects
            int prismCount = Mathf.CeilToInt(angle / 15f);
            float baseRotation = direction - (angle / 2);

            for (int i = 0; i < prismCount; i++)
            {
                float currentRotation = baseRotation + (i * 15f);
                GameObject prism = Instantiate(addon.damageMarkerPrismObject, coneTransform);
                prism.transform.localRotation = Quaternion.Euler(0, 0, currentRotation);
                prism.SetActive(true); // Make sure the instantiated prism is active

                // Disable MeshRenderer if isInvisible is true
                if (isInvisible)
                {
                    MeshRenderer renderer = prism.GetComponent<MeshRenderer>();
                    if (renderer != null) renderer.enabled = false;
                }
            }

            addon.SetLifetime(lifetime);
        }
    }

    // Method to create a donut damage marker
    public void CreateDamageMarkerDonut(Vector3 position, float innerRadius, float outerRadius, float lifetime, bool isInvisible = false)
    {
        GameObject damageMarker = Instantiate(damageMarkerObjectPrefab, new Vector3(position.x, position.y, setPositionZ), Quaternion.identity);
        DamageMarkerAddon addon = damageMarker.GetComponent<DamageMarkerAddon>();

        if (addon != null)
        {
            addon.SetMarketType(DamageMarkerAddon.MarkerTypes.Donut);
            Transform donutTransform = addon.damageMarkerDonutObject.transform;

            // Set the position, scale, and rotation
            donutTransform.localScale = new Vector3(outerRadius, outerRadius, 1f); // Set the scale of the outer circle

            // Assuming the donut object has a child or a mask to create the inner radius effect
            Transform innerCircle = donutTransform.Find("InnerCircle"); // Find the inner circle object
            if (innerCircle != null)
            {
                innerCircle.localScale = new Vector3(innerRadius / outerRadius, innerRadius / outerRadius, 1f); // Adjust inner circle scale relative to outer

                // Disable MeshRenderer of the inner circle if isInvisible is true
                if (isInvisible)
                {
                    MeshRenderer renderer = innerCircle.GetComponent<MeshRenderer>();
                    if (renderer != null) renderer.enabled = false;
                }
            }

            // Disable MeshRenderer of the outer donut if isInvisible is true
            if (isInvisible)
            {
                MeshRenderer renderer = addon.damageMarkerDonutObject.GetComponent<MeshRenderer>();
                if (renderer != null) renderer.enabled = false;
            }

            addon.SetLifetime(lifetime);
        }
    }
}
