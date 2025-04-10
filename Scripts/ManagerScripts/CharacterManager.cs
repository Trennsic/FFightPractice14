using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class CharacterManager : MonoBehaviour
{
    #region // Enum Definitions
    public enum SpriteTypes
    {
        Icon,
        Chibi,
        Role
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
        None = -1,
        Mt = 0,
        Ot = 1,
        H1 = 2,
        H2 = 3,
        M1 = 4,
        M2 = 5,
        R1 = 6,
        R2 = 7
    }
    public enum FacingDirections
    {
        Left,
        Right
    }
    public enum CharacterType { Player, NPC }
    #endregion
    #region // Characters
    RolePositions playerRolePosition = RolePositions.None;

    // Object references for characters
    [SerializeField] private GameObject MtObject;
    [SerializeField] private GameObject OtObject;
    [SerializeField] private GameObject H1Object;
    [SerializeField] private GameObject H2Object;
    [SerializeField] private GameObject M1Object;
    [SerializeField] private GameObject M2Object;
    [SerializeField] private GameObject R1Object;
    [SerializeField] private GameObject R2Object;

    [SerializeField] private CharacterAddon Mt;
    [SerializeField] private CharacterAddon Ot;
    [SerializeField] private CharacterAddon H1;
    [SerializeField] private CharacterAddon H2;
    [SerializeField] private CharacterAddon M1;
    [SerializeField] private CharacterAddon M2;
    [SerializeField] private CharacterAddon R1;
    [SerializeField] private CharacterAddon R2;


    [SerializeField] private List<CharacterAddon> characters;



    #endregion
    #region // Sprites
    #region // Chibi Sprites
    [Header("Chibi Sprites")]
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
    #endregion
    #region // Icon Sprites
    [Header("Icon Sprites")]
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
    #region // Role Sprites
    [Header("Role Sprites")]
    [SerializeField] Sprite RoleIconMT;
    [SerializeField] Sprite RoleIconOT;
    [SerializeField] Sprite RoleIconH1;
    [SerializeField] Sprite RoleIconH2;
    [SerializeField] Sprite RoleIconM1;
    [SerializeField] Sprite RoleIconM2;
    [SerializeField] Sprite RoleIconR1;
    [SerializeField] Sprite RoleIconR2;
    #endregion
    [SerializeField] Sprite IconNone;
    #endregion
    #region // Scale
    [Header("Scale Settings")]
    [SerializeField] private float chibiXscale = 9f;
    [SerializeField] private float chibiYscale = 9f;
    [SerializeField] private float chibiXoffset = -.1f;
    [SerializeField] private float chibiYoffset = .1f;

    [SerializeField] private float iconXscale = 40f;
    [SerializeField] private float iconYscale = 40f;
    [SerializeField] private float iconXoffset = 0f;
    [SerializeField] private float iconYoffset = .05f;

    [SerializeField] private float roleXscale = 8f;
    [SerializeField] private float roleYscale = 7f;
    [SerializeField] private float roleXoffset = 0f;
    [SerializeField] private float roleYoffset = .05f;
    #endregion
    // Object references for characters
    [SerializeField] private SpriteTypes spriteType;
    #region // References 
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #region // Functions
    public void InitializeCharacterManager()
    {

        InitializedTeam();
        InitializeDebug();
    }
    private void InitializeDebug() { debug = new DebugInfo(); }
    private void InitializedTeam()
    {
        // Add Characters
        if (GetMtObject() != null) { CharacterAddon addon = GetMtObject().GetComponent<CharacterAddon>(); if (addon != null) { SetMt(addon); } }
        if (GetOtObject() != null) { CharacterAddon addon = GetOtObject().GetComponent<CharacterAddon>(); if (addon != null) { SetOt(addon); } }
        if (GetH1Object() != null) { CharacterAddon addon = GetH1Object().GetComponent<CharacterAddon>(); if (addon != null) { SetH1(addon); } }
        if (GetH2Object() != null) { CharacterAddon addon = GetH2Object().GetComponent<CharacterAddon>(); if (addon != null) { SetH2(addon); } }
        if (GetM1Object() != null) { CharacterAddon addon = GetM1Object().GetComponent<CharacterAddon>(); if (addon != null) { SetM1(addon); } }
        if (GetM2Object() != null) { CharacterAddon addon = GetM2Object().GetComponent<CharacterAddon>(); if (addon != null) { SetM2(addon); } }
        if (GetR1Object() != null) { CharacterAddon addon = GetR1Object().GetComponent<CharacterAddon>(); if (addon != null) { SetR1(addon); } }
        if (GetR2Object() != null) { CharacterAddon addon = GetR2Object().GetComponent<CharacterAddon>(); if (addon != null) { SetR2(addon); } }

        //Set Job and Role
        
        Jobs mt = GetRandomJobBasedOnRole(Roles.Tank);
        HashSet<Jobs> excludedTanks = new HashSet<Jobs> { mt };
        Jobs ot = GetRandomJobBasedOnRole(Roles.Tank, excludedTanks);
        Jobs h1 = GetRandomJobBasedOnRole(Roles.Healer);
        HashSet<Jobs> excludedHealer = new HashSet<Jobs> { h1 };
        Jobs h2 = GetRandomJobBasedOnRole(Roles.Healer, excludedHealer);
        Jobs m1 = GetRandomJobBasedOnRole(Roles.MeleeDps);
        HashSet<Jobs> excludedMelee = new HashSet<Jobs> { m1 };
        Jobs m2 = GetRandomJobBasedOnRole(Roles.MeleeDps, excludedMelee);
        Jobs r1 = GetRandomJobBasedOnRole(Roles.RangedDps);
        HashSet<Jobs> excludedRanged = new HashSet<Jobs> { r1 };
        Jobs r2 = GetRandomJobBasedOnRole(Roles.RangedDps, excludedRanged);

        GetMt().UpdateCharacterInfo(mt, RolePositions.Mt);
        GetOt().UpdateCharacterInfo(ot, RolePositions.Ot);
        GetH1().UpdateCharacterInfo(h1, RolePositions.H1);
        GetH2().UpdateCharacterInfo(h2, RolePositions.H2);
        GetM1().UpdateCharacterInfo(m1, RolePositions.M1);
        GetM2().UpdateCharacterInfo(m2, RolePositions.M2);
        GetR1().UpdateCharacterInfo(r1, RolePositions.R1);
        GetR2().UpdateCharacterInfo(r2, RolePositions.R2);

        // Add all characters to character list
        characters = new List<CharacterAddon>();
        characters.Add(GetMt());
        characters.Add(GetOt());
        characters.Add(GetH1());
        characters.Add(GetH2());
        characters.Add(GetM1());
        characters.Add(GetM2());
        characters.Add(GetR1());
        characters.Add(GetR2());
        // Link character manager to each character
        foreach (CharacterAddon character in GetCharacters()) { character.InitializeCharacter(this); }

        UpdateTeam();

        // Move units to clock positions
        //Vector3 position = GetGameManager().GetArenaPositionFromPercentage(50f, 32f);
        //GetMt().MoveCharacter(position);
        //position = GetGameManager().GetArenaPositionFromPercentage(50f, 14f);
        //GetOt().MoveCharacter(position);
        //position = GetGameManager().GetArenaPositionFromPercentage(40f, 23f);
        //GetH1().MoveCharacter(position);
        //position = GetGameManager().GetArenaPositionFromPercentage(60f, 23f);
        //GetH2().MoveCharacter(position);
        //position = GetGameManager().GetArenaPositionFromPercentage(40f, 14f);
        //GetM1().MoveCharacter(position);
        //position = GetGameManager().GetArenaPositionFromPercentage(60f, 14f);
        //GetM2().MoveCharacter(position);
        //position = GetGameManager().GetArenaPositionFromPercentage(40f, 32f);
        //GetR1().MoveCharacter(position);
        //position = GetGameManager().GetArenaPositionFromPercentage(60f, 32f);
        //GetR2().MoveCharacter(position);
        //
        SetSpriteType(SpriteTypes.Role);
    }
    private void UpdateTeamSprites()
    {
        SpriteTypes st = GetSpriteType();
        foreach (CharacterAddon character in GetCharacters())
        {
            character.SetSpriteType(st);
            character.UpdateSprite();
        }
    }
    public void UpdateNpcPositions()
    {
        foreach(CharacterAddon character in GetCharacters())
        {
            // If target is an NPC move them to their position
            if (character.GetCharacterType() == CharacterType.NPC)
            {
                RolePositions rolePosition = character.GetRolePosition();
                Vector3 targetPosition = GetGameManager().GetPositionManager().GetNpcRolePosition(rolePosition);
                character.MoveCharacter(targetPosition);
            }
        }
    }
    public CharacterAddon GetAddonByRole(RolePositions role)
    {
        // Pull the position by role
        CharacterAddon character = null;
        bool roleFound = false;
        List<CharacterAddon> characters = GetCharacters();
        for (int i = 0; i < characters.Count; i++)
        {
            // Pull the current character
            character = characters[i];
            // Check if it matches role
            if (character.GetRolePosition() == role)
            {
                // If true leave loop;
                roleFound = true;
                break;
            }
        }
        if (!roleFound) Debug.LogWarning($"Characters doesn't have a {role}.");


        return character;
    }
    public Vector3 GetArenaPositionByRole(RolePositions role)
    {
        // Pull the position by role
        CharacterAddon character = GetAddonByRole(role) ;

        return character.gameObject.transform.position;
    }
    #region // Get Random Jobs
    public Jobs GetRandomJobBasedOnRole(Roles role, HashSet<Jobs> excludedJobs = null)
    {
        excludedJobs ??= new HashSet<Jobs>();

        Jobs[] availableJobs = role switch
        {
            Roles.Tank => new Jobs[] { Jobs.PLD, Jobs.WAR, Jobs.DRK, Jobs.GNB },
            Roles.Healer => new Jobs[] { Jobs.WHM, Jobs.SCH, Jobs.AST, Jobs.SGE },
            Roles.MeleeDps => new Jobs[] { Jobs.MNK, Jobs.DRG, Jobs.NIN, Jobs.SAM, Jobs.RPR, Jobs.VIP },
            Roles.RangedDps => new Jobs[] { Jobs.BRD, Jobs.MCH, Jobs.DNC, Jobs.PIC, Jobs.BLM, Jobs.SMN, Jobs.RDM },
            _ => new Jobs[] { Jobs.None }
        };

        // Filter out excluded jobs
        List<Jobs> filteredJobs = new List<Jobs>();
        foreach (Jobs job in availableJobs)
        {
            if (!excludedJobs.Contains(job))
            {
                filteredJobs.Add(job);
            }
        }

        if (filteredJobs.Count == 0)
        {
            Debug.LogWarning("No available jobs found after exclusions!");
            return Jobs.None;
        }

        return filteredJobs[Random.Range(0, filteredJobs.Count)];
    }


    #endregion
    private void UpdateTeam()
    {
        // Update player vs npcs
        if (playerRolePosition == RolePositions.Mt) { GetMt().SetCharacterType(CharacterType.Player); } else { GetMt().SetCharacterType(CharacterType.NPC); }
        if (playerRolePosition == RolePositions.Ot) { GetOt().SetCharacterType(CharacterType.Player); } else { GetOt().SetCharacterType(CharacterType.NPC); }
        if (playerRolePosition == RolePositions.H1) { GetH1().SetCharacterType(CharacterType.Player); } else { GetH1().SetCharacterType(CharacterType.NPC); }
        if (playerRolePosition == RolePositions.H2) { GetH2().SetCharacterType(CharacterType.Player); } else { GetH2().SetCharacterType(CharacterType.NPC); }
        if (playerRolePosition == RolePositions.M1) { GetM1().SetCharacterType(CharacterType.Player); } else { GetM1().SetCharacterType(CharacterType.NPC); }
        if (playerRolePosition == RolePositions.M2) { GetM2().SetCharacterType(CharacterType.Player); } else { GetM2().SetCharacterType(CharacterType.NPC); }
        if (playerRolePosition == RolePositions.R1) { GetR1().SetCharacterType(CharacterType.Player); } else { GetR1().SetCharacterType(CharacterType.NPC); }
        if (playerRolePosition == RolePositions.R2) { GetR2().SetCharacterType(CharacterType.Player); } else { GetR2().SetCharacterType(CharacterType.NPC); }
        // Update sprites
        foreach (CharacterAddon character in GetCharacters()) { character.UpdateSprite(); }
    }
    #region // Setters
    public void SetMt(CharacterAddon newMt) => Mt = newMt;
    public void SetOt(CharacterAddon newOt) => Ot = newOt;
    public void SetH1(CharacterAddon newH1) => H1 = newH1;
    public void SetH2(CharacterAddon newH2) => H2 = newH2;
    public void SetM1(CharacterAddon newM1) => M1 = newM1;
    public void SetM2(CharacterAddon newM2) => M2 = newM2;
    public void SetR1(CharacterAddon newR1) => R1 = newR1;
    public void SetR2(CharacterAddon newR2) => R2 = newR2;
    #endregion
    #region // Getters
    public CharacterAddon GetMt() => Mt;
    public CharacterAddon GetOt() => Ot;
    public CharacterAddon GetH1() => H1;
    public CharacterAddon GetH2() => H2;
    public CharacterAddon GetM1() => M1;
    public CharacterAddon GetM2() => M2;
    public CharacterAddon GetR1() => R1;
    public CharacterAddon GetR2() => R2;
    public GameObject GetMtObject() => MtObject;
    public GameObject GetOtObject() => OtObject;
    public GameObject GetH1Object() => H1Object;
    public GameObject GetH2Object() => H2Object;
    public GameObject GetM1Object() => M1Object;
    public GameObject GetM2Object() => M2Object;
    public GameObject GetR1Object() => R1Object;
    public GameObject GetR2Object() => R2Object;
    public float GetChibiXscale() => chibiXscale;
    public float GetChibiYscale() => chibiYscale;
    public float GetChibiXoffset() => chibiXoffset;
    public float GetChibiYoffset() => chibiYoffset;

    public float GetIconXscale() => iconXscale;
    public float GetIconYscale() => iconYscale;
    public float GetIconXoffset() => iconXoffset;
    public float GetIconYoffset() => iconYoffset;

    public float GetRoleXscale() => roleXscale;
    public float GetRoleYscale() => roleYscale;
    public float GetRoleXoffset() => roleXoffset;
    public float GetRoleYoffset() => roleYoffset;

    public List<CharacterAddon> GetCharacters() => characters;

    public Sprite GetSprite(SpriteTypes spriteType = SpriteTypes.Chibi,
        Jobs job = Jobs.None,
        RolePositions rolePosition = RolePositions.None)
    {
        if (spriteType == SpriteTypes.Chibi)
        {
            switch (job)
            {
                case Jobs.PLD: return chibiPLD;
                case Jobs.WAR: return chibiWAR;
                case Jobs.DRK: return chibiDRK;
                case Jobs.GNB: return chibiGNB;
                case Jobs.WHM: return chibiWHM;
                case Jobs.SCH: return chibiSCH;
                case Jobs.AST: return chibiAST;
                case Jobs.SGE: return chibiSGE;
                case Jobs.MNK: return chibiMNK;
                case Jobs.DRG: return chibiDRG;
                case Jobs.NIN: return chibiNIN;
                case Jobs.SAM: return chibiSAM;
                case Jobs.RPR: return chibiRPR;
                case Jobs.VIP: return chibiVIP;
                case Jobs.BRD: return chibiBRD;
                case Jobs.MCH: return chibiMCH;
                case Jobs.DNC: return chibiDNC;
                case Jobs.PIC: return chibiPIC;
                case Jobs.BLM: return chibiBLM;
                case Jobs.SMN: return chibiSMN;
                case Jobs.RDM: return chibiRDM;
                default: return IconNone;
            }
        }
        else if (spriteType == SpriteTypes.Icon)
        {
            switch (job)
            {
                case Jobs.PLD: return IconPLD;
                case Jobs.WAR: return IconWAR;
                case Jobs.DRK: return IconDRK;
                case Jobs.GNB: return IconGNB;
                case Jobs.WHM: return IconWHM;
                case Jobs.SCH: return IconSCH;
                case Jobs.AST: return IconAST;
                case Jobs.SGE: return IconSGE;
                case Jobs.MNK: return IconMNK;
                case Jobs.DRG: return IconDRG;
                case Jobs.NIN: return IconNIN;
                case Jobs.SAM: return IconSAM;
                case Jobs.RPR: return IconRPR;
                case Jobs.VIP: return IconVIP;
                case Jobs.BRD: return IconBRD;
                case Jobs.MCH: return IconMCH;
                case Jobs.DNC: return IconDNC;
                case Jobs.PIC: return IconPIC;
                case Jobs.BLM: return IconBLM;
                case Jobs.SMN: return IconSMN;
                case Jobs.RDM: return IconRDM;
                default: return IconNone;
            }
        }
        else if (spriteType == SpriteTypes.Role)
        {
            switch (rolePosition)
            {
                case RolePositions.Mt: return RoleIconMT;
                case RolePositions.Ot: return RoleIconOT;
                case RolePositions.H1: return RoleIconH1;
                case RolePositions.H2: return RoleIconH2;
                case RolePositions.M1: return RoleIconM1;
                case RolePositions.M2: return RoleIconM2;
                case RolePositions.R1: return RoleIconR1;
                case RolePositions.R2: return RoleIconR2;

                default: return IconNone;
            }
        }
        return IconNone;
    }

    #endregion
    #endregion
    public void SetSpriteType(SpriteTypes newSpriteType) {spriteType = newSpriteType; UpdateTeamSprites(); }
    public SpriteTypes GetSpriteType() => spriteType;
    public void SetPlayerRolePosition(RolePositions newPlayerRole) => playerRolePosition = newPlayerRole;
    public RolePositions GetPlayerRolePosition() => playerRolePosition;
    #region // References 
    private GameManager GetGameManager() => gameManager;
    #endregion
}
