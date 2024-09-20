using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMarkerAddon : MonoBehaviour
{
    private float lifetime;

    // Start is called before the first frame update
    void Start()
    {
        // Start the lifetime countdown
        if (lifetime > 0)
        {
            StartCoroutine(LifetimeCoroutine());
        }
    }

    // Method to set the lifetime for the marker
    public void SetLifetime(float lifetimeDuration)
    {
        lifetime = lifetimeDuration;
    }

    // Coroutine to handle the lifetime of the damage marker
    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Optional: Add any per-frame logic or animation here
    }
}
