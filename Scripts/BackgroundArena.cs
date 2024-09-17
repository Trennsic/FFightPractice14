using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;

public class BackgroundArena : MonoBehaviour
{
    public enum FightArenaTexturesEnum
    {
        M4N,
        // Add more fight texture enums as needed
    }


    // Use an Enum-Indexed List for textures
    public List<Texture2D> fightTextures = new List<Texture2D>(); // List of textures to be mapped by enum index

    // Dictionary to hold width and height for each texture
    public Dictionary<FightArenaTexturesEnum, Vector2> textureSizes = new Dictionary<FightArenaTexturesEnum, Vector2>();

    public GameObject fightManagerObject;  // The GameObject containing the FightManager
    private FightManager.FightEnum currentlySetFightBackground;
    private string currentlySetAttack;
    private int currentlySetStep;

    private bool fightHasChanged = false;

    private FightManager fightManager;     // Reference to the FightManager component
    private SpriteRenderer spriteRenderer; // The SpriteRenderer on BackgroundArena
    [SerializeField] private float zPosition = 1.5f;

    void Start()
    {
        // Get the FightManager component from the provided GameObject
        if (fightManagerObject != null)
        {
            fightManager = fightManagerObject.GetComponent<FightManager>();
        }
        else
        {
            Debug.LogError("FightManagerObject is not set.");
            return;
        }
        #region // Set the Z position
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        #endregion

        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing from BackgroundArena.");
        }

        // Ensure the texture list is filled
        if (fightTextures.Count == 0)
        {
            Debug.LogError("FightTextures list is empty. Please populate it in the Inspector.");
        }

        // Initialize texture sizes (you can set these in Start or Inspector as needed)
        InitializeTextureSizes();
    }

    void Update()
    {
        // Check if the fight, attack, or step has changed
        UpdateFight();

        if (fightHasChanged)
        {
            // Ensure that the necessary components are valid
            if (fightManager != null && spriteRenderer != null)
            {
                // Set the appropriate texture based on the fight, attack, and step
                SetTextureBasedOnFightDetails();
            }
        }
    }

    // Update fight status (check if the current fight, attack, or step has changed)
    private void UpdateFight()
    {
        if (fightManager != null)
        {
            // Get the current fight, attack, and step from FightManager
            FightManager.FightEnum currentFight = fightManager.GetCurrentFight();
            string currentAttack = fightManager.GetCurrentAttack();
            int currentStep = fightManager.GetCurrentStep();

            // Check if the fight, attack, or step has changed
            if (currentlySetFightBackground != currentFight || currentlySetAttack != currentAttack || currentlySetStep != currentStep)
            {
                fightHasChanged = true;
                currentlySetFightBackground = currentFight;
                currentlySetAttack = currentAttack;
                currentlySetStep = currentStep;
            }
            else
            {
                fightHasChanged = false;
            }
        }
    }

    // Manually set the texture based on the current fight, attack, and step using the enum
    private void SetTextureBasedOnFightDetails()
    {
        // Example logic to map to FightArenaTexturesEnum (modify this to fit your exact logic)
        FightArenaTexturesEnum textureEnumKey = DetermineTextureEnumKey();

        // Convert the enum to an index to access the correct texture from the list
        int textureIndex = (int)textureEnumKey;

        if (textureIndex >= 0 && textureIndex < fightTextures.Count)
        {
            // Get the texture from the list based on the enum index
            Texture2D chosenTexture = fightTextures[textureIndex];

            // Create a sprite from the texture and assign it to the SpriteRenderer
            Sprite sprite = Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;

            // Adjust the sprite size based on the specific size for this texture
            AdjustSpriteSize(textureEnumKey);
        }
        else
        {
            Debug.LogError($"Texture index out of range: {textureIndex}");
        }
    }

    // Function to determine the correct enum key based on the current fight, attack, and step
    private FightArenaTexturesEnum DetermineTextureEnumKey()
    {
        // Example logic to determine which texture enum corresponds to the current fight, attack, and step
        if (currentlySetFightBackground == FightManager.FightEnum.M4N)
        {
            return FightArenaTexturesEnum.M4N;
        }
        // Add more logic here based on your fight data
        return FightArenaTexturesEnum.M4N;
    }

    // Function to adjust the sprite's size based on the custom width and height set for each texture
    private void AdjustSpriteSize(FightArenaTexturesEnum textureEnumKey)
    {
        if (spriteRenderer.sprite != null && textureSizes.ContainsKey(textureEnumKey))
        {
            // Get the custom width and height from the dictionary
            Vector2 size = textureSizes[textureEnumKey];
            float desiredWidth = size.x;
            float desiredHeight = size.y;

            // Get the texture dimensions
            float spriteWidth = spriteRenderer.sprite.bounds.size.x;
            float spriteHeight = spriteRenderer.sprite.bounds.size.y;

            // Calculate the scale factor based on the custom width and height
            Vector3 newScale = new Vector3(desiredWidth / spriteWidth, desiredHeight / spriteHeight, 1);

            // Apply the new scale to the SpriteRenderer's transform
            spriteRenderer.transform.localScale = newScale;
        }
    }

    // Initialize the dictionary of texture sizes (you can set these values via Inspector if needed)
    private void InitializeTextureSizes()
    {
        // Add texture size mappings for each FightArenaTexturesEnum
        textureSizes[FightArenaTexturesEnum.M4N] = new Vector2(20f, 10f);
        // Add more size mappings as needed
    }
}
