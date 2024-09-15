using System.Collections;
using UnityEngine;

[System.Serializable]
public class WickedThunderSettings
{
    [SerializeField] private Sprite wicked_Thunder_Base_Image;
    [SerializeField] private Sprite wicked_Thunder_Wings_Image;
    [SerializeField] private float wicked_Thunder_Xscale = 1f;
    [SerializeField] private float wicked_Thunder_Yscale = 1f;

    // Expose these values via properties (optional)
    public Sprite Wicked_Thunder_Base_Image => wicked_Thunder_Base_Image;
    public Sprite Wicked_Thunder_Wings_Image => wicked_Thunder_Wings_Image;
    public float Wicked_Thunder_Xscale => wicked_Thunder_Xscale;
    public float Wicked_Thunder_Yscale => wicked_Thunder_Yscale;
}

public class BossManager : MonoBehaviour
{
    public enum Bosses
    {
        None,
        Wicked_Thunder
    }


    [SerializeField] public WickedThunderSettings wickedThunderSettings; // Collapsible section

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;
    private float targetRotation;
    private float moveDuration;
    private float moveStartTime;

    // Start is called before the first frame update
    void Start()
    {
        // Get or add the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Optional: Smooth movement and rotation could be handled here if not using coroutines
    }

    public void SetupBoss(Bosses whichBoss)
    {
        if (whichBoss == Bosses.Wicked_Thunder)
        {
            WickedThunderSettings  wts = wickedThunderSettings;
            UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
        }
    }

    public void MoveBoss(Vector3 goalPosition, float goalRotation, float duration)
    {
        // Initialize target values
        targetPosition = goalPosition;
        targetRotation = goalRotation;
        moveDuration = duration;
        moveStartTime = Time.time;

        // Start the coroutine to move the boss
        StartCoroutine(MoveBossCoroutine());
    }
    public void UpdateBossSprite(Sprite bossSprite, float xScale,float yScale)
    {
        // Set sprite renderer image to Wicked Thunder Image
        spriteRenderer.sprite = bossSprite;

        // Make sure Xscale and Yscale are set correctly
        transform.localScale = new Vector3(xScale, yScale, 1f);
    }

    private IEnumerator MoveBossCoroutine()
    {
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime = Time.time - moveStartTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // Interpolate position and rotation
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(0, 0, targetRotation), t);

            yield return null; // Wait for the next frame
        }

        // Ensure final position and rotation are exactly the target values
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }
}
