using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;
using static FightManager;

[System.Serializable]
public class WickedThunderSettings
{
    [SerializeField] private Sprite wicked_Thunder_Base_Image;
    [SerializeField] private Sprite wicked_Thunder_Wings_Image;
    [SerializeField] private float wicked_Thunder_Xscale = 1f;
    [SerializeField] private float wicked_Thunder_Yscale = 1f;

    [SerializeField] private int bf_LaserDashRand = 0;
    [SerializeField] private int bf_ElctroMineRand = 0;
    [SerializeField] private int bf_ElctroMine2Rand = 0;
    [SerializeField] private int bf_LaserExplodeRand = 0;
    [SerializeField] private int bf_NearFarRand = 0;



    // Expose these values via properties (optional)
    public Sprite Wicked_Thunder_Base_Image => wicked_Thunder_Base_Image;
    public Sprite Wicked_Thunder_Wings_Image => wicked_Thunder_Wings_Image;
    public float Wicked_Thunder_Xscale => wicked_Thunder_Xscale;
    public float Wicked_Thunder_Yscale => wicked_Thunder_Yscale;
    public int Bf_LaserDashRand => bf_LaserDashRand;
    public int Bf_ElctroMineRand => bf_ElctroMineRand;
    public int BF_ElctroMine2Rand => bf_ElctroMine2Rand;
    public int Bf_LaserExplodeRand => bf_LaserExplodeRand;
    public int Bf_NearFarRand => bf_NearFarRand;

    public void WickedThunderRandomize()
    {

        // Bewitching Flight
        bf_LaserDashRand    = Random.Range(0, 2);
        bf_ElctroMineRand   = Random.Range(0, 2);
        bf_LaserExplodeRand = Random.Range(0, 2);
        bf_ElctroMine2Rand  = Random.Range(0, 2);
        bf_NearFarRand      = Random.Range(0, 2);
    }
}

public class BossManager : MonoBehaviour
{
    public enum Bosses
    {
        None,
        Wicked_Thunder
    }
    public enum BossImageStages 
    {
        First,
        Second,
        Third,
        Fourth,
        Fifth,
    }


    [SerializeField] public WickedThunderSettings wickedThunderSettings; // Collapsible section

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BossImageStages spriteImageStage;
    //[SerializeField] private float percentWidthOfSpriteToDraw = 1f;
    //[SerializeField] private float percentHeightOfSpriteToDraw = 1f;
    private Vector3 targetPosition;
    private float targetRotation;
    private float moveDuration;
    private float moveStartTime;
    [SerializeField] private float zPosition = -2f;
    [SerializeField] private FightManager fm;

    // Start is called before the first frame update
    void Start()
    {

        // Get or add the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        #region // Set the Z position
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // Optional: Smooth movement and rotation could be handled here if not using coroutines
        if (spriteImageStage != BossImageStages.First)
        {
            //
        }
    }
    public void UpdateBoss()
    {
        /*
        //Get Current Fight, Attack, and Step
        FightManager.FightEnum currFight = 0;
        string currAttack = "";
        int currStep = -1;
        if (fm != null)
        {
            currFight = fm.GetCurrentFight();
            currAttack = fm.GetCurrentAttack();
            currStep = fm.GetCurrentStep();
        }

        //Update Boss Sprite
        #region // M4S
        if (currFight == FightEnum.M4S)
        {
            if (currAttack == "Bewitching Flight")
            {
                WickedThunderSettings wts = wickedThunderSettings;
                if (currStep <= 1)
                {
                    UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
                if (currentStep == 1)
                {

                }
                // Move North
                else if (currentStep == 2)
                {
                    bm.MoveBoss(new Vector3(0f, 4f, zp), 0, 1f);
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Wings_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
                //Set Lines - 3
                // Move Mid
                else if (currentStep == 4)
                {
                    bm.MoveBoss(new Vector3(0f, 0f, zp), 180, 1f);
                    //Change boss to base form
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
            }
        }
        #endregion
        */
    }
    /*
    "Bewitching Flight", "Witch Hunt", "Electrope Edge 1", "Electrope Edge 2 (Lightning Cage)",
    "Ion Cluster (Ion Cannon)", "Electrope Transplant", "Cross Tail Switch",
    "Twilight Sabbath", "Midnight Sabbath", "Raining Swords (Chain Lightning)",
    "Sunrise Sabbath", "Sword Quiver"
    */
    public void SetupBoss(Bosses whichBoss)
    {
        //
        spriteImageStage = BossImageStages.First;
        if (whichBoss == Bosses.Wicked_Thunder)
        {
            WickedThunderSettings  wts = wickedThunderSettings;
            wts.WickedThunderRandomize();
            UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
        }
    }

    public void MoveBoss(Vector3 goalPosition, float goalRotation, float duration)
    {
        //Take out Z value 
        goalPosition = new Vector3 (goalPosition.x, goalPosition.y, transform.position.z);
        // Initialize target values
        targetPosition = goalPosition;
        targetRotation = goalRotation;
        moveDuration = duration;
        moveStartTime = Time.time;

        // Start the coroutine to move the boss
        StartCoroutine(MoveBossCoroutine());
    }
    public void UpdateBossSprite(Sprite bossSprite, float xScale = 1, float yScale = 1, float perWidOfSpriteToDraw = 1,
        float perHeiOfSpriteToDraw = 1, float perStartXOfSpriteToDraw = 0f, float perStartYOfSpriteToDraw = 0f)
    {
        // Ensure the percentages are clamped between 0 and 1
        perWidOfSpriteToDraw = Mathf.Clamp01(perWidOfSpriteToDraw);
        perHeiOfSpriteToDraw = Mathf.Clamp01(perHeiOfSpriteToDraw);
        perStartXOfSpriteToDraw = Mathf.Clamp01(perStartXOfSpriteToDraw);
        perStartYOfSpriteToDraw = Mathf.Clamp01(perStartYOfSpriteToDraw);

        // Calculate the new cropped sprite's size based on the given percentages
        int croppedWidth = Mathf.RoundToInt(bossSprite.texture.width * perWidOfSpriteToDraw);
        int croppedHeight = Mathf.RoundToInt(bossSprite.texture.height * perHeiOfSpriteToDraw);

        // Calculate the starting position based on the given start percentages
        int startX = Mathf.RoundToInt(bossSprite.texture.width * perStartXOfSpriteToDraw);
        int startY = Mathf.RoundToInt(bossSprite.texture.height * perStartYOfSpriteToDraw);

        // Ensure the cropped region stays within the bounds of the texture
        startX = Mathf.Clamp(startX, 0, bossSprite.texture.width - croppedWidth);
        startY = Mathf.Clamp(startY, 0, bossSprite.texture.height - croppedHeight);

        // Define the rectangle that represents the portion of the sprite to use
        Rect spriteRect = new Rect(startX, startY, croppedWidth, croppedHeight);

        // Create a new sprite based on the cropped portion
        Sprite croppedSprite = Sprite.Create(
            bossSprite.texture,
            spriteRect,
            new Vector2(0.5f, 0.5f), // Pivot in the center
            bossSprite.pixelsPerUnit);

        // Set the sprite renderer's sprite to the new cropped sprite
        spriteRenderer.sprite = croppedSprite;

        // Ensure the Xscale and Yscale are applied
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
