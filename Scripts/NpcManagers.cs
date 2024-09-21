using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region // NpcInfo
public class NpcInfo
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

    [SerializeField] private float posX;
    [SerializeField] private float posY;
    [SerializeField] private SpriteTypes spriteType = SpriteTypes.Icon;
    [SerializeField] private Roles role             = Roles.None;
    [SerializeField] private Jobs job               = Jobs.None;
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
#endregion
public class NpcManagers : MonoBehaviour
{
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
