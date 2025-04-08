using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    #region // References 
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;
    #endregion

    [SerializeField] private SpriteRenderer backgroundFillerRenderer; // Black background (1000x1000)
    [SerializeField] private SpriteRenderer arenaFillerRenderer; // Arena background (800x600)
    [SerializeField] private SpriteRenderer arenaRenderer; // Arena (600x600)
    [SerializeField] private float zPosition = 10f;

    // Individual arena sprites for each enum
    [SerializeField] private Sprite m1sArena;
    [SerializeField] private Sprite m2sArena;
    [SerializeField] private Sprite m3sArena;
    [SerializeField] private Sprite m4sArena;
    [SerializeField] private Sprite m4sArena_2;
    [SerializeField] private Sprite m5sArena;
    [SerializeField] private Sprite m6sArena;
    [SerializeField] private Sprite m7sArena;
    [SerializeField] private Sprite m8sArena;
    [SerializeField] private Sprite m8sArena_2;

    [SerializeField] private Sprite titanStory;
    [SerializeField] private Sprite titanHard;
    [SerializeField] private Sprite titanExtreme;

    private void Start()
    {
        // Optional: auto-initialize
        // InitializeBackgroundManager();
    }

    public void InitializeBackgroundManager()
    {
        float arenaFillerOffset = .01f;
        float backgroundFillerOffset = .02f;
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        arenaFillerRenderer.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, (zPosition + arenaFillerOffset));
        backgroundFillerRenderer.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, (zPosition + backgroundFillerOffset));

        InitializeDebug();
    }
    private void InitializeDebug() { debug = new DebugInfo(true); }
    public void UpdateArena()
    {
        FightManager.FightEnum fightType = GetGameManager().GetFightManager().GetCurrentFight();
        Sprite selectedSprite = null;

        switch (fightType)
        {
            case FightManager.FightEnum.M1S:
                selectedSprite = m1sArena;
                break;
            case FightManager.FightEnum.M2S:
                selectedSprite = m2sArena;
                break;
            case FightManager.FightEnum.M3S:
                selectedSprite = m3sArena;
                break;
            case FightManager.FightEnum.M4S:
                selectedSprite = m4sArena;
                break;
            // case FightManager.FightEnum.M4S_2:
            //     selectedSprite = m4sArena_2;
            //     break;
            case FightManager.FightEnum.M5S:
                selectedSprite = m5sArena;
                break;
            case FightManager.FightEnum.M6S:
                selectedSprite = m6sArena;
                break;
            case FightManager.FightEnum.M7S:
                selectedSprite = m7sArena;
                break;
            case FightManager.FightEnum.M8S:
                selectedSprite = m8sArena;
                break;
            // case FightManager.FightEnum.M8S_2:
            //     selectedSprite = m8sArena_2;
            //     break;
            case FightManager.FightEnum.Titan_Story:
                selectedSprite = titanStory;
                break;
            case FightManager.FightEnum.Titan_Hard:
                selectedSprite = titanHard;
                break;
            case FightManager.FightEnum.Titan_Extreme:
                selectedSprite = titanExtreme;
                break;
            default:
                Debug.LogWarning($"No arena sprite assigned for enum: {fightType}");
                break;
        }

        arenaRenderer.sprite = selectedSprite;
        ResizeArenaSprite();
    }
    private void SetupArenaFiller()
    {
        if (arenaFillerRenderer.sprite == null)
        {
            Debug.LogWarning("ArenaFiller does not have a sprite assigned.");
            return;
        }

        Vector2 spriteSize = arenaFillerRenderer.sprite.bounds.size;
        Vector3 parentScale = transform.lossyScale;

        if (Mathf.Approximately(parentScale.x, 0f) || Mathf.Approximately(parentScale.y, 0f))
        {
            Debug.LogWarning("Parent scale is zero; cannot apply arena scale properly.");
            return;
        }

        arenaFillerRenderer.transform.localScale = new Vector3(
            600f / (spriteSize.x * parentScale.x),
            400f / (spriteSize.y * parentScale.y),
            1f
        );

        Vector2 worldSize = spriteSize * arenaFillerRenderer.transform.lossyScale;
        debug.Log($"[ArenaFiller] Final World Size: {worldSize.x:F2} x {worldSize.y:F2}");
    }
    private void SetupBackgroundFiller()
    {
        if (backgroundFillerRenderer.sprite == null)
        {
            Debug.LogWarning("BackgroundFiller does not have a sprite assigned.");
            return;
        }

        Vector2 spriteSize = backgroundFillerRenderer.sprite.bounds.size;
        Vector3 parentScale = transform.lossyScale;

        if (Mathf.Approximately(parentScale.x, 0f) || Mathf.Approximately(parentScale.y, 0f))
        {
            Debug.LogWarning("Parent scale is zero; cannot apply background scale properly.");
            return;
        }

        backgroundFillerRenderer.transform.localScale = new Vector3(
            1000f / (spriteSize.x * parentScale.x),
            450f / (spriteSize.y * parentScale.y),
            1f
        );

        Vector2 worldSize = spriteSize * backgroundFillerRenderer.transform.lossyScale;
        debug.Log($"[BackgroundFiller] Final World Size: {worldSize.x:F2} x {worldSize.y:F2}");
    }



    private void ResizeArenaSprite()
    {
        if (arenaRenderer.sprite == null) return;

        Vector2 spriteSize = arenaRenderer.sprite.bounds.size;
        Vector3 parentScale = transform.lossyScale;

        arenaRenderer.transform.localScale = new Vector3(
            400f / (spriteSize.x * parentScale.x),
            400f / (spriteSize.y * parentScale.y),
            1f
        );

        arenaRenderer.sortingOrder = backgroundFillerRenderer.sortingOrder + 1;

        Vector2 worldSize = spriteSize * arenaRenderer.transform.lossyScale;
        debug.Log($"[ArenaRenderer] Final World Size: {worldSize.x:F2} x {worldSize.y:F2}");

        SetupArenaFiller();
        SetupBackgroundFiller();
    }


    #region // References 
    private GameManager GetGameManager() => gameManager;
    #endregion
}
