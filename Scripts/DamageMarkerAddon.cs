using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMarkerAddon : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] private bool lifetimeStarted;

    [SerializeField] public GameObject damageMarkerSphereObject; // Reference to Circle object
    [SerializeField] public GameObject damageMarkerBoxObject; // Reference to Box object
    [SerializeField] public GameObject damageMarkerPrismObject; // Reference to Prism object
    [SerializeField] public GameObject damageMarkerDonutObject; // Reference to Prism object

    [SerializeField] private MarkerTypes dmMarkerType; // Reference to Circle object

    [SerializeField] public bool isDebugging { get; private set; } // Toggle this in Inspector for debugging logs

    public enum MarkerTypes
    {
        Circle,
        Rectangle,
        Cone,
        Donut,
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Debug log to check the lifetime value
        if (isDebugging)
        {
            Debug.Log($"Lifetime set to: {lifetime}");
        }
        //
        lifetimeStarted = false;


        // Make sure subobjects are filled and disabled
        if (damageMarkerSphereObject != null)
        {
            damageMarkerSphereObject.SetActive(false);
        }
        else
        {
            if (isDebugging)
            {
                Debug.LogWarning("Sphere object not set");
            }
        }

        if (damageMarkerBoxObject != null)
        {
            damageMarkerBoxObject.SetActive(false);
        }
        else
        {
            if (isDebugging)
            {
                Debug.LogWarning("Box object not set");
            }
        }

        if (damageMarkerPrismObject != null)
        {
            damageMarkerPrismObject.SetActive(false);
        }
        else
        {
            if (isDebugging)
            {
                Debug.LogWarning("Prism object not set");
            }
        }
    }

    // Method to set the lifetime for the marker
    public void SetLifetime(float lifetimeDuration)
    {
        lifetime = lifetimeDuration;
        if (isDebugging)
        {
            Debug.Log("Setting lifetime: " + lifetimeDuration);
        }
    }

    // Coroutine to handle the lifetime of the damage marker
    private IEnumerator LifetimeCoroutine()
    {
        if (isDebugging)
        {
            Debug.Log("Starting lifetime coroutine for " + gameObject.name);
        }
        lifetimeStarted = true;

        yield return new WaitForSeconds(lifetime);

        if (isDebugging)
        {
            Debug.Log("Lifetime ended, destroying object: " + gameObject.name);
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Optional: Add any per-frame logic or animation here
        // Start the lifetime countdown
        if (!lifetimeStarted && lifetime > 0)
        {
            StartCoroutine(LifetimeCoroutine());
        }
    }

    public MarkerTypes GetMarkerType()
    {
        return dmMarkerType;
    }

    public void SetMarketType(MarkerTypes markerType)
    {
        dmMarkerType = markerType;

        if (isDebugging)
        {
            Debug.Log("Setting marker type to: " + markerType);
        }

        switch (markerType)
        {
            case MarkerTypes.Circle:
                damageMarkerSphereObject.SetActive(true);
                break;
            case MarkerTypes.Rectangle:
                damageMarkerBoxObject.SetActive(true);
                break;
            case MarkerTypes.Cone:
                damageMarkerPrismObject.SetActive(true);
                break;
            case MarkerTypes.Donut:
                damageMarkerDonutObject.SetActive(true);
                break;
            default:
                if (isDebugging)
                {
                    Debug.LogWarning("Unknown MarkerType in SetMarkerType - DamageMarkerAddon");
                }
                break;
        }
    }
}
