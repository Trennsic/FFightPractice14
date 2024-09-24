using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    #region // Definitions
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
        None,
        MT,
        OT,
        H1,
        H2,
        M1,
        M2,
        R1,
        R2
    }
    public enum FacingDirections
    {
        Left,
        Right
    }
    #endregion
    #region // Position
    [Header("Position")]
    [SerializeField] private float posX;
    [SerializeField] private float posY;
    #endregion
    #region // Enums
    [Header("Enum Types")]
    [SerializeField] private SpriteTypes spriteType = SpriteTypes.Icon;
    [SerializeField] private Roles role = Roles.None;
    [SerializeField] private Jobs job = Jobs.None;
    [SerializeField] private RolePositions rolePosition = RolePositions.None;
    [SerializeField] private FacingDirections facing = FacingDirections.Left;
    #endregion
    #region // Scale
    [Header("Scale Settings")]
    [SerializeField] private float chibiXscale = .25f;
    [SerializeField] private float chibiYscale = .25f;
    [SerializeField] private float chibiXoffset = -.1f;
    [SerializeField] private float chibiYoffset = .1f;

    [SerializeField] private float iconXscale = 1.15f;
    [SerializeField] private float iconYscale = 1.15f;
    [SerializeField] private float iconXoffset = 0f;
    [SerializeField] private float iconYoffset = .05f;

    [SerializeField] private float roleXscale = .23f;
    [SerializeField] private float roleYscale = .18f;
    [SerializeField] private float roleXoffset = 0f;
    [SerializeField] private float roleYoffset = .05f;



    #endregion
    #region // Public Vars
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
    public float RoleXscale => roleXscale;
    public float RoleYscale => roleYscale;    
    
    public float ChibiXoffset => chibiXoffset;
    public float ChibiYoffset => chibiYoffset;
    public float IconXoffset => iconXoffset;
    public float IconYoffset => iconYoffset;
    public float RoleXoffset => roleXoffset;
    public float RoleYoffset => roleYoffset;
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
    #endregion
    #region // Functions
    #region // Setters
    public void SetSpriteType(SpriteTypes whichSpriteType) { spriteType = whichSpriteType; }
    public void SetRole(Roles whichRoles) { role = whichRoles; }
    public void SetJob(Jobs whichJob) { job = whichJob; }
    public void SetRolePosition(RolePositions whichRolePos) { rolePosition = whichRolePos; }
    public void SetFacing(FacingDirections facingDirection) { facing = facingDirection; }
    #endregion
    #region // Getters
    public SpriteTypes GetSpriteType() => SpriteType;
    public Roles GetRoleType() => Role;
    public Jobs GetJobType() => Job;
    public RolePositions GetRolePosition() => RolePosition;
    public FacingDirections GetFacing() => facing;
    public Sprite GetSprite(CharacterInfo.SpriteTypes spriteType = CharacterInfo.SpriteTypes.Role, 
        CharacterInfo.Jobs job = CharacterInfo.Jobs.None, 
        CharacterInfo.RolePositions rolePosition = CharacterInfo.RolePositions.None)
    {
        if (spriteType == CharacterInfo.SpriteTypes.Chibi)
        {
            switch (job)
            {
                case CharacterInfo.Jobs.PLD: return chibiPLD;
                case CharacterInfo.Jobs.WAR: return chibiWAR;
                case CharacterInfo.Jobs.DRK: return chibiDRK;
                case CharacterInfo.Jobs.GNB: return chibiGNB;
                case CharacterInfo.Jobs.WHM: return chibiWHM;
                case CharacterInfo.Jobs.SCH: return chibiSCH;
                case CharacterInfo.Jobs.AST: return chibiAST;
                case CharacterInfo.Jobs.SGE: return chibiSGE;
                case CharacterInfo.Jobs.MNK: return chibiMNK;
                case CharacterInfo.Jobs.DRG: return chibiDRG;
                case CharacterInfo.Jobs.NIN: return chibiNIN;
                case CharacterInfo.Jobs.SAM: return chibiSAM;
                case CharacterInfo.Jobs.RPR: return chibiRPR;
                case CharacterInfo.Jobs.VIP: return chibiVIP;
                case CharacterInfo.Jobs.BRD: return chibiBRD;
                case CharacterInfo.Jobs.MCH: return chibiMCH;
                case CharacterInfo.Jobs.DNC: return chibiDNC;
                case CharacterInfo.Jobs.PIC: return chibiPIC;
                case CharacterInfo.Jobs.BLM: return chibiBLM;
                case CharacterInfo.Jobs.SMN: return chibiSMN;
                case CharacterInfo.Jobs.RDM: return chibiRDM;
                default: return IconNone;
            }
        }
        else if (spriteType == CharacterInfo.SpriteTypes.Icon)
        {
            switch (job)
            {
                case CharacterInfo.Jobs.PLD: return IconPLD;
                case CharacterInfo.Jobs.WAR: return IconWAR;
                case CharacterInfo.Jobs.DRK: return IconDRK;
                case CharacterInfo.Jobs.GNB: return IconGNB;
                case CharacterInfo.Jobs.WHM: return IconWHM;
                case CharacterInfo.Jobs.SCH: return IconSCH;
                case CharacterInfo.Jobs.AST: return IconAST;
                case CharacterInfo.Jobs.SGE: return IconSGE;
                case CharacterInfo.Jobs.MNK: return IconMNK;
                case CharacterInfo.Jobs.DRG: return IconDRG;
                case CharacterInfo.Jobs.NIN: return IconNIN;
                case CharacterInfo.Jobs.SAM: return IconSAM;
                case CharacterInfo.Jobs.RPR: return IconRPR;
                case CharacterInfo.Jobs.VIP: return IconVIP;
                case CharacterInfo.Jobs.BRD: return IconBRD;
                case CharacterInfo.Jobs.MCH: return IconMCH;
                case CharacterInfo.Jobs.DNC: return IconDNC;
                case CharacterInfo.Jobs.PIC: return IconPIC;
                case CharacterInfo.Jobs.BLM: return IconBLM;
                case CharacterInfo.Jobs.SMN: return IconSMN;
                case CharacterInfo.Jobs.RDM: return IconRDM;
                default: return IconNone;
            }
        }
        else if (spriteType == CharacterInfo.SpriteTypes.Role)
        {
            switch (rolePosition)
            {
                case CharacterInfo.RolePositions.MT: return RoleIconMT;
                case CharacterInfo.RolePositions.OT: return RoleIconOT;
                case CharacterInfo.RolePositions.H1: return RoleIconH1;
                case CharacterInfo.RolePositions.H2: return RoleIconH2;
                case CharacterInfo.RolePositions.M1: return RoleIconM1;
                case CharacterInfo.RolePositions.M2: return RoleIconM2;
                case CharacterInfo.RolePositions.R1: return RoleIconR1;
                case CharacterInfo.RolePositions.R2: return RoleIconR2;

                default: return IconNone;
            }
        }
        return IconNone;
    }
    #endregion
    #endregion
}
