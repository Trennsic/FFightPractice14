using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static PlayerInfo;

[System.Serializable]
public class PlayerInfo
{
    public enum SpriteTypes
    {
        Icon,
        Chibi
    }
    public enum Roles
    {
        None,
        Tank,
        Healer,
        MeleeDps,
        RangedDps,
    }
    public enum Jobs
    {
        None,
        PLD,
        WAR,
        DRK,
        GNB,

        WHM,
        SCH,
        AST,
        SGE,

        MNK,
        DRG,
        NIN,
        SAM,
        RPR,
        VIP,

        BRD,
        MCH,
        DNC,

        PIC,
        BLM,
        SMN,
        RDM,
    }
    public enum RolePositions
    {
        None    ,
        MT      ,
        OT      ,
        H1      ,
        H2      ,
        M1      ,
        M2      ,
        R1      ,
        R2
    }
    public enum FacingDirections
    {
        Left,
        Right
    }

    [SerializeField] private float posX;
    [SerializeField] private float posY;
    [SerializeField] private SpriteTypes spriteType = SpriteTypes.Chibi;
    [SerializeField] private Roles role = Roles.None;
    [SerializeField] private Jobs job = Jobs.None;
    [SerializeField] private RolePositions rolePosition = RolePositions.None;
    [SerializeField] private float chibiXscale = .25f;
    [SerializeField] private float chibiYscale = .25f;
    [SerializeField] private float iconXscale = .25f;
    [SerializeField] private float iconYscale = .25f;
    [SerializeField] private FacingDirections facing = FacingDirections.Left;

    public float PosX => posX;
    public float PosY => posY;
    public SpriteTypes SpriteType => spriteType;
    public Roles Role => role;
    public Jobs Job => job;
    public RolePositions RolePosition => rolePosition;
    public FacingDirections Facing => facing;
    public float ChibiXscale => chibiXscale;
    public float ChibiYscale => chibiYscale;
    public float IconXscale => iconXscale;
    public float IconYscale => iconYscale;

    public void SetSpriteType(SpriteTypes whichSpriteType) { spriteType = whichSpriteType; }
    public void SetRole(Roles whichRoles) { role = whichRoles; }
    public void SetJob(Jobs whichJob) { job = whichJob; }
    public void SetRolePosition(RolePositions whichRolePos) { rolePosition = whichRolePos; }
    public void SetFacing(FacingDirections facingDirection) { facing = facingDirection; }
}

public class PlayerManager : MonoBehaviour
{
    #region // References
    public PlayerInfo playerInfo = new PlayerInfo();
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float zPosition = -2f;
    [SerializeField] private FightManager fm;
    #endregion
    #region // Sprites
    [SerializeField] Sprite chibiPLD;
    [SerializeField] Sprite chibiWAR;
    [SerializeField] Sprite chibiDRK;
    [SerializeField] Sprite chibiGNB;
    [SerializeField] Sprite chibiWHM;
    [SerializeField] Sprite chibiSCH;
    [SerializeField] Sprite chibiAST;
    [SerializeField] Sprite chibiSGE;
    [SerializeField] Sprite chibiMNK;
    [SerializeField] Sprite chibiDRG;
    [SerializeField] Sprite chibiNIN;
    [SerializeField] Sprite chibiSAM;
    [SerializeField] Sprite chibiRPR;
    [SerializeField] Sprite chibiVIP;
    [SerializeField] Sprite chibiBRD;
    [SerializeField] Sprite chibiMCH;
    [SerializeField] Sprite chibiDNC;
    [SerializeField] Sprite chibiPIC;
    [SerializeField] Sprite chibiBLM;
    [SerializeField] Sprite chibiSMN;
    [SerializeField] Sprite chibiRDM;

    [SerializeField] Sprite IconPLD;
    [SerializeField] Sprite IconWAR;
    [SerializeField] Sprite IconDRK;
    [SerializeField] Sprite IconGNB;
    [SerializeField] Sprite IconWHM;
    [SerializeField] Sprite IconSCH;
    [SerializeField] Sprite IconAST;
    [SerializeField] Sprite IconSGE;
    [SerializeField] Sprite IconMNK;
    [SerializeField] Sprite IconDRG;
    [SerializeField] Sprite IconNIN;
    [SerializeField] Sprite IconSAM;
    [SerializeField] Sprite IconRPR;
    [SerializeField] Sprite IconVIP;
    [SerializeField] Sprite IconBRD;
    [SerializeField] Sprite IconMCH;
    [SerializeField] Sprite IconDNC;
    [SerializeField] Sprite IconPIC;
    [SerializeField] Sprite IconBLM;
    [SerializeField] Sprite IconSMN;
    [SerializeField] Sprite IconRDM;
    #endregion

    private Vector3 targetPosition;
    private float targetRotation;
    private float moveDuration;
    private float moveStartTime;

    [SerializeField] private bool isDebugging;

    // Start is called before the first frame update
    void Start()
    {
        #region // Get or add the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        #endregion
        #region // Set the Z position
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        #endregion
        // Start with chibi icons
        playerInfo.SetSpriteType(PlayerInfo.SpriteTypes.Chibi);
    }

    // Update is called once per frame
    void Update()
    {
        // Additional updates can go here
    }

    public void SetupPlayer(PlayerInfo.Jobs whichJob, PlayerInfo.RolePositions whichRolePos)
    {
        // Set Starting Info
        playerInfo.SetJob(whichJob);
        playerInfo.SetRolePosition(whichRolePos);
        // Update the sprite based on the info
        UpdatePlayerSprite();

        //Move player to starting postion
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        float startRot = 0;
        switch (whichRolePos)
        {
            case PlayerInfo.RolePositions.MT: startPos.x = -.1f;    startPos.y = 2.2f;      playerInfo.SetFacing(PlayerInfo.FacingDirections.Left);   break;
            case PlayerInfo.RolePositions.OT: startPos.x = -.1f;    startPos.y = -2.0f;     playerInfo.SetFacing(PlayerInfo.FacingDirections.Right);   break;
            case PlayerInfo.RolePositions.H1: startPos.x = -2.35f;  startPos.y = .1f;       playerInfo.SetFacing(PlayerInfo.FacingDirections.Right);   break;
            case PlayerInfo.RolePositions.H2: startPos.x = 2.15f;   startPos.y = .1f;       playerInfo.SetFacing(PlayerInfo.FacingDirections.Left);   break;
            case PlayerInfo.RolePositions.M1: startPos.x = -2.35f;  startPos.y = -2.0f;     playerInfo.SetFacing(PlayerInfo.FacingDirections.Right);   break;
            case PlayerInfo.RolePositions.M2: startPos.x = 2.15f;   startPos.y = -2.0f;     playerInfo.SetFacing(PlayerInfo.FacingDirections.Left);   break;
            case PlayerInfo.RolePositions.R1: startPos.x = -2.35f;  startPos.y = 2.2f;      playerInfo.SetFacing(PlayerInfo.FacingDirections.Right);   break;
            case PlayerInfo.RolePositions.R2: startPos.x = 2.15f;   startPos.y = 2.2f;      playerInfo.SetFacing(PlayerInfo.FacingDirections.Left); break;
        }
        //Move player to starting position
        MovePlayer(startPos, startRot, 0f);

        //Update the facing
        UpdatePlayerSprite();
    }

    public void MovePlayer(Vector3 goalPosition, float goalRotation, float duration)
    {
        // Remove Z value from goal position 
        goalPosition = new Vector3(goalPosition.x, goalPosition.y, transform.position.z);
        // Initialize target values
        targetPosition = goalPosition;
        targetRotation = goalRotation;
        moveDuration = duration;
        moveStartTime = Time.time;

        // Start the coroutine to move the player
        StartCoroutine(MovePlayerCoroutine());
    }
    // Helper function to update player's facing direction based on position relative to the boss
    private void UpdateFacingDirection()
    {
        Vector3 playerPos = transform.position;
        Vector3 bossPos = GetBossPosition();

        if (playerPos.x < bossPos.x)
        {
            // Player is to the left of the boss, so face right
            playerInfo.SetFacing(FacingDirections.Right);
        }
        else if (playerPos.x > bossPos.x)
        {
            // Player is to the right of the boss, so face left
            playerInfo.SetFacing(FacingDirections.Left);
        }
        //Update the sprite
        UpdatePlayerSprite();
        if (isDebugging)
        {
            Debug.Log($"Player position: {playerPos}, Boss position: {bossPos}");
            Debug.Log($"Player is now facing: {(playerPos.x < bossPos.x ? "Right" : "Left")}");
        }
    }
    public void UpdatePlayerSprite()
    {
        Sprite selectedSprite = null;
        PlayerInfo.SpriteTypes spriteType = playerInfo.SpriteType;
        PlayerInfo.Jobs job = playerInfo.Job;

        // Determine the sprite based on the job and sprite type
        if (spriteType == PlayerInfo.SpriteTypes.Chibi)
        {
            selectedSprite = GetChibiSprite(job);
            transform.localScale = new Vector3(playerInfo.ChibiXscale, playerInfo.ChibiYscale, 1);
        }
        else if (spriteType == PlayerInfo.SpriteTypes.Icon)
        {
            selectedSprite = GetIconSprite(job);
            transform.localScale = new Vector3(playerInfo.IconXscale, playerInfo.IconYscale, 1);
        }

        // Flip the sprite if the player is facing right
        if (playerInfo.Facing == PlayerInfo.FacingDirections.Right)
        {
            // Flip the sprite by inverting the X scale
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // Ensure the sprite is not flipped when facing left
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // Set the sprite renderer to the selected sprite
        if (selectedSprite != null)
        {
            spriteRenderer.sprite = selectedSprite;
        }
        else
        {
            Debug.LogWarning($"No sprite found for job {job} and sprite type {spriteType}");
        }
    }


    private Sprite GetChibiSprite(PlayerInfo.Jobs job)
    {
        switch (job)
        {
            case PlayerInfo.Jobs.PLD: return chibiPLD;
            case PlayerInfo.Jobs.WAR: return chibiWAR;
            case PlayerInfo.Jobs.DRK: return chibiDRK;
            case PlayerInfo.Jobs.GNB: return chibiGNB;
            case PlayerInfo.Jobs.WHM: return chibiWHM;
            case PlayerInfo.Jobs.SCH: return chibiSCH;
            case PlayerInfo.Jobs.AST: return chibiAST;
            case PlayerInfo.Jobs.SGE: return chibiSGE;
            case PlayerInfo.Jobs.MNK: return chibiMNK;
            case PlayerInfo.Jobs.DRG: return chibiDRG;
            case PlayerInfo.Jobs.NIN: return chibiNIN;
            case PlayerInfo.Jobs.SAM: return chibiSAM;
            case PlayerInfo.Jobs.RPR: return chibiRPR;
            case PlayerInfo.Jobs.VIP: return chibiVIP;
            case PlayerInfo.Jobs.BRD: return chibiBRD;
            case PlayerInfo.Jobs.MCH: return chibiMCH;
            case PlayerInfo.Jobs.DNC: return chibiDNC;
            case PlayerInfo.Jobs.PIC: return chibiPIC;
            case PlayerInfo.Jobs.BLM: return chibiBLM;
            case PlayerInfo.Jobs.SMN: return chibiSMN;
            case PlayerInfo.Jobs.RDM: return chibiRDM;
            default: return null;
        }
    }

    private Sprite GetIconSprite(PlayerInfo.Jobs job)
    {
        switch (job)
        {
            case PlayerInfo.Jobs.PLD: return IconPLD;
            case PlayerInfo.Jobs.WAR: return IconWAR;
            case PlayerInfo.Jobs.DRK: return IconDRK;
            case PlayerInfo.Jobs.GNB: return IconGNB;
            case PlayerInfo.Jobs.WHM: return IconWHM;
            case PlayerInfo.Jobs.SCH: return IconSCH;
            case PlayerInfo.Jobs.AST: return IconAST;
            case PlayerInfo.Jobs.SGE: return IconSGE;
            case PlayerInfo.Jobs.MNK: return IconMNK;
            case PlayerInfo.Jobs.DRG: return IconDRG;
            case PlayerInfo.Jobs.NIN: return IconNIN;
            case PlayerInfo.Jobs.SAM: return IconSAM;
            case PlayerInfo.Jobs.RPR: return IconRPR;
            case PlayerInfo.Jobs.VIP: return IconVIP;
            case PlayerInfo.Jobs.BRD: return IconBRD;
            case PlayerInfo.Jobs.MCH: return IconMCH;
            case PlayerInfo.Jobs.DNC: return IconDNC;
            case PlayerInfo.Jobs.PIC: return IconPIC;
            case PlayerInfo.Jobs.BLM: return IconBLM;
            case PlayerInfo.Jobs.SMN: return IconSMN;
            case PlayerInfo.Jobs.RDM: return IconRDM;
            default: return null;
        }
    }

    private IEnumerator MovePlayerCoroutine()
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
        // After moving, check player and boss positions
        UpdateFacingDirection();
    }
 
    // Enable or disable debugging
    public void SetIsDebugging(bool IsDebugging)
    {
        isDebugging = IsDebugging;
    }

    // Check if debugging is enabled
    public bool GetIsDebugging()
    {
        return isDebugging;
    }
    public Vector3 GetBossPosition()
    {
        //Initalize a boss position var
        Vector3 bossPos = new Vector3(0, 0, 0);
        //Find boss transform position
        if (fm != null)
        {
            bossPos = fm.GetBossPosition();
        }
        else { Debug.LogWarning("Unable to find boss position due to not finding fight manager"); }
        //Return boss position
        return bossPos;
    }
}
