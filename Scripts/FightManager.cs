using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EffectsManager;
using UnityEngine.UIElements;
using static FightManager;
using static BossManager;
using System.Collections;

#region // Current Fight Info
[System.Serializable]
public class CurrentFightInfo
{
    #region // Definitions
    [SerializeField] private FightEnum currentFight;
    [SerializeField] private string currentAttack;
    [SerializeField] private int currentStep;
    [SerializeField] private int currentAttackIndex;
    #endregion
    #region // Functions

    // Expose these values via properties (optional)
    public FightEnum CurrentFight   => currentFight;
    public string CurrentAttack     => currentAttack;
    public int CurrentStep          => currentStep;
    public int CurrentAttackIndex   => currentAttackIndex;

    public void SetCurrentFight(FightEnum newFight) { currentFight = newFight;  }
    public void SetCurrentAttack(string newAttack) { currentAttack = newAttack;  }
    public void SetCurrentStep(int newStep) { currentStep = newStep;  }
    public void SetCurrentAttackIndex(int newAtkIndex) { currentAttackIndex = newAtkIndex;  }
    public void GotoNexAttackIndex() { currentAttackIndex++;  }
    public void ResetStepIndex() { currentStep = 0;  }
    public void GotoNextStepIndex() { currentStep++;  }
    #endregion
}
#endregion
#region // Debug Info
[System.Serializable]
public class DebugInfo
{
    #region // Definitions
    [SerializeField] public bool isDebugging;
    // Enable or disable debugging
    public void SetIsDebugging(bool IsDebugging) { isDebugging = IsDebugging; }

    // Check if debugging is enabled
    public bool GetIsDebugging() { return isDebugging; }
    #endregion
}
#endregion
#region // Fight Manager
public class FightManager : MonoBehaviour
{
    #region // Definitions
    public enum FightEnum
    {
        M1N, M2N, M3N, M4N,
        M1S, M2S, M3S, M4S,
        Titan_Story, Titan_Hard, Titan_Extreme,
    }

    public FightEnum currentFight;
    [SerializeField] private bool isDebugging;
   
    [SerializeField] public CurrentFightInfo currentFightInfo;
    #region // References 
    [Header("References")]
    public GameObject fightTextGameObject;
    public GameObject MarkerManager;
    private Waymarks waymarks;
    public GameObject Boss;
    public GameObject EffectsManager;
    public GameObject PPositionManager;
    public GameObject PlayerManager;
    public GameObject DamageMarkerManager;
    public GameObject PlayerHitManager;
    public GameObject NpcManager;
    private EffectsManager effects;
    private BossManager bossManager;
    private PlayerPositionManager playerPositionManager;
    private PlayerManager playerManager;
    private DamageMarkerManager damageManager;
    private PlayerHitManager playerHitManager;
    private NpcManagers npcManager;

    private TMP_Text fightText;
    #endregion

    private Dictionary<FightEnum, List<string>> fightAttacks = new Dictionary<FightEnum, List<string>>();
    private Dictionary<string, int> attackSteps = new Dictionary<string, int>();
    private Dictionary<FightEnum, List<List<bool>>> fightAwaitingSteps = new Dictionary<FightEnum, List<List<bool>>>();

    private bool isStartOfFight = true;

    [SerializeField] private float zPosition = 0f;
    [SerializeField] private int seedValue = 12345; // Your specific seed value
    #endregion
    #region // Functions
    private void Awake()
    {
        seedValue = System.Guid.NewGuid().GetHashCode(); // Using a random GUID hash as seed
        Random.InitState(seedValue);


    }
    void Start()
    {
        #region // References
        if (fightTextGameObject != null)
        {
            fightText = fightTextGameObject.GetComponent<TMP_Text>();
            if (fightText == null) { Debug.LogError(" Unable to find fightText component"); }
        } else { Debug.LogError(" Unable to find FightTextGameObject"); }
        if (MarkerManager != null)
        {
            waymarks = MarkerManager.GetComponent<Waymarks>();
            if (waymarks == null) { Debug.LogError(" Unable to find waymarks component"); }
        }
        else { Debug.LogError(" Unable to find MarkerManager"); }
        if (Boss != null)
        {
            bossManager = Boss.GetComponent<BossManager>();
            if (bossManager == null) { Debug.LogError(" Unable to find bossManager component"); }
        } else { Debug.LogError(" Unable to find Boss"); }
        if (EffectsManager != null)
        {
            effects = EffectsManager.GetComponent<EffectsManager>();
            if (effects == null) { Debug.LogError(" Unable to find effects component"); }
        } else { Debug.LogError(" Unable to find EffectsManager"); }
        if (PPositionManager != null)
        {
            playerPositionManager = PPositionManager.GetComponent<PlayerPositionManager>();
            if (playerPositionManager == null) { Debug.LogError(" Unable to find playerPositionManager component"); }
        } else { Debug.LogError(" Unable to find PPositionManager"); }        
        if (PlayerManager != null)
        {
            playerManager = PlayerManager.GetComponent<PlayerManager>();
            if (playerManager == null) { Debug.LogError(" Unable to find playerManager component"); }
        } else { Debug.LogError(" Unable to find PlayerManager"); }
        if (DamageMarkerManager != null)
        {
            damageManager = DamageMarkerManager.GetComponent<DamageMarkerManager>();
            if (damageManager == null) { Debug.LogError(" Unable to find damageManager component"); }
        }
        else { Debug.LogError(" Unable to find DamageMarkerManager"); }
        if (PlayerHitManager != null)
        {
            playerHitManager = PlayerHitManager.GetComponent<PlayerHitManager>();
            if (playerHitManager == null) { Debug.LogError(" Unable to find playerhitManager component"); }
        }
        else { Debug.LogError(" Unable to find PlayerHitManager"); }        
        if (NpcManager != null)
        {
            npcManager = NpcManager.GetComponent<NpcManagers>();
            if (npcManager == null) { Debug.LogError(" Unable to find npcManager component"); }
        }
        else { Debug.LogError(" Unable to find NpcManager"); }
        #endregion
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        InitializeFights();
        StartFight();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcessNextStep();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            ResetFight();
        }

        // Execute the middle step logic continuously in Update
        ExecuteMiddleOfStep();
    }

    void InitializeFights()
    {
        // Initialize Titan Story Attacks (as previously done)
        fightAttacks[FightEnum.Titan_Story] = new List<string>()
        {
            "Rock Buster", "Tumult", "Geocrush", "Landslide", "Granite Gaol",
            "Titan's Heart & Earthen Fury", "Rock Buster2", "Tumult2",
            "Geocrush2", "Landslide2", "Granite Gaol2", "Weight of the Land"
        };

        // Set steps for Titan Story
        attackSteps["Rock Buster"] = 2;
        attackSteps["Tumult"] = 1;
        attackSteps["Geocrush"] = 2;
        // and so on...

        // Set awaiting steps for Titan Story
        fightAwaitingSteps[FightEnum.Titan_Story] = new List<List<bool>>()
        {
            new List<bool> { true, true }, // Rock Buster
            new List<bool> { true }, // Tumult
            // and so on...
        };
        #region // M4S
        /* 
         * Bewitching Flight                    - Move N, P1 Lines, R1 Lines, P2, R2, P3 + Mechanic, R3;
         * Witch Hunt                           - P1, R1, P2, R2, P3, R3, P4, R4;
         * Electrope Edge 1                     - Pos Cardinals, Gleam 1, Gleam 2, Gleam 3, Gleam 4, SideSpark, Pos 2, Res;
         * Electrope Edge 2 (Lightning Cage)    - P Clocks,R Gleam 1 - 4,R2 Lightning Cage, P2 Major 1, R3 Major 1,R4 SideSpark, P3 StackOrSpread, R5 StackOrSpread, R6 Lightning Cage 2, P4, Major 2, R5 Major 2;
         * Ion Cluster (Ion Cannon)             - R1 Boss Move/Rotate, P1 Dodge Side, R2 Platform Destroyed, R3 Cannon Charge, P2 Position Colors, R4 Debuffs 1, P3 Debuffs 1, R5 Debuff Resolve 1, P4 Position Colors, R6 Debuffs 2, P5 Debuffs 2, R7 Debuff Resolve 2,P6 Position Colors, R8 Debuffs 3, P7 Debuffs 3, R9 Debuff Resolve 3,;
         * Electrope Transplant                 - Repeat 2x [R1 Setup, Repeat 5x [Dodge, Prots] P1 - 6, R2 - 7, R8 - Orbs, P9 - Protect], P? Stage Swap, R? Swap Resolve
         * Cross Tail Switch                    - ;
         * Twilight Sabbath                     - ;
         * Midnight Sabbath                     - ;
         * Raining Swords (Chain Lightning)     - ;
         * Sunrise Sabbath                      - ;
         * Sword Quiver                         - ;
         */
        // Initialize M4S Attacks
        fightAttacks[FightEnum.M4S] = new List<string>()
        {
            "Bewitching Flight", "Witch Hunt", "Electrope Edge 1", "Electrope Edge 2 (Lightning Cage)",
            "Ion Cluster (Ion Cannon)", "Electrope Transplant", "Cross Tail Switch",
            "Twilight Sabbath", "Midnight Sabbath", "Raining Swords (Chain Lightning)",
            "Sunrise Sabbath", "Sword Quiver"
        };

        // Set the number of steps for each attack (M4S)
        attackSteps["Bewitching Flight"] = 15;
        attackSteps["Witch Hunt"] = 8;
        attackSteps["Electrope Edge 1"] = 8;
        attackSteps["Electrope Edge 2 (Lightning Cage)"] = 15;
        attackSteps["Ion Cluster (Ion Cannon)"] = 16;
        attackSteps["Electrope Transplant"] = 32;
        attackSteps["Cross Tail Switch"] = 3;
        attackSteps["Twilight Sabbath"] = 3;
        attackSteps["Midnight Sabbath"] = 3;
        attackSteps["Raining Swords (Chain Lightning)"] = 3;
        attackSteps["Sunrise Sabbath"] = 3;
        attackSteps["Sword Quiver"] = 3;

        // Initialize the awaiting steps for M4S
        fightAwaitingSteps[FightEnum.M4S] = new List<List<bool>>()
        {
            new List<bool> { true, false , true, false , true, false }, // Bewitching Flight
            new List<bool> { true, false , true, false , true, false, true, false}, // Witch Hunt
            new List<bool> { true, false }, // Electrope Edge 1
            new List<bool> { true, false }, // Electrope Edge 2 (Lightning Cage)
            new List<bool> { true, false }, // Ion Cluster (Ion Cannon)
            new List<bool> { true, false }, // Electrope Transplant
            new List<bool> { true, false }, // Cross Tail Switch
            new List<bool> { true, false }, // Twilight Sabbath
            new List<bool> { true, false }, // Midnight Sabbath
            new List<bool> { true, false }, // Raining Swords (Chain Lightning)
            new List<bool> { true, false }, // Sunrise Sabbath
            new List<bool> { true, false }, // Sword Quiver
        };
        #endregion
    }

    public void StartFight(FightEnum selectedFight = FightEnum.M4S)
    {
        currentFightInfo.SetCurrentFight(selectedFight);
        currentFightInfo.SetCurrentAttackIndex(0);
        currentFightInfo.SetCurrentStep(0);
        isStartOfFight = true;
        SetBoss(); SetPlayer();
        SetupParty();
        SetWaymarks();
        UpdateFightActions();
        UpdateFightAttackString();
        UpdateFightText();
        ExecuteStartOfStep();  // Execute StartOfStep when starting the fight
    }

    public void ProcessNextStep()
    {
        if (currentFightInfo.CurrentAttackIndex >= fightAttacks[currentFightInfo.CurrentFight].Count)
        {
            if (GetIsDebugging()) { Debug.Log("Fight Completed"); }
            if (fightText != null)
            {
                fightText.text = "Fight Completed";
            }
            return;
        }

        // Only execute EndOfStep when moving to the next step after the first step
        if (!isStartOfFight)
        {
            ExecuteEndOfStep();
        }

        string currentAttack = fightAttacks[currentFightInfo.CurrentFight][currentFightInfo.CurrentAttackIndex];
        int steps = attackSteps[currentAttack];


        if (currentFightInfo.CurrentStep < steps - 1)
        {
            currentFightInfo.GotoNextStepIndex();  // Move to the next step
        }
        else
        {
            currentFightInfo.ResetStepIndex();  // Reset step
            currentFightInfo.GotoNexAttackIndex();  // Move to the next attack
        }
      
        if (currentFightInfo.CurrentAttackIndex < fightAttacks[currentFightInfo.CurrentFight].Count)
        {
            if (GetIsDebugging())
            {
                Debug.Log($"Executing step {currentFightInfo.CurrentStep} of attack: {fightAttacks[currentFightInfo.CurrentFight][currentFightInfo.CurrentAttackIndex]}");
            }
            UpdateFightText();
        }
        //Update fight attack string
        UpdateFightAttackString();

        //
        UpdateFightActions();

        // Execute StartOfStep when moving to the new step
        ExecuteStartOfStep();

        // Once we’ve processed the start of fight, mark it as false so EndOfStep is called afterward
        isStartOfFight = false;
    }

    public bool GetIsCurrentStepAwaiting()
    {
        if (currentFightInfo.CurrentAttackIndex < fightAwaitingSteps[currentFightInfo.CurrentFight].Count && currentFightInfo.CurrentStep < fightAwaitingSteps[currentFightInfo.CurrentFight][currentFightInfo.CurrentAttackIndex].Count)
        {
            return fightAwaitingSteps[currentFightInfo.CurrentFight][currentFightInfo.CurrentAttackIndex][currentFightInfo.CurrentStep];
        }

        return false;
    }

    void UpdateFightText()
    {
        
        if (fightText != null && fightAttacks[currentFightInfo.CurrentFight].Count > currentFightInfo.CurrentAttackIndex)
        {
            string currentAttack = fightAttacks[currentFightInfo.CurrentFight][currentFightInfo.CurrentAttackIndex];
            int totalSteps = attackSteps[currentAttack];
            fightText.text = $"Fight: {currentFightInfo.CurrentFight}\nAttack: {currentAttack}\nStep: {currentFightInfo.CurrentStep} of {totalSteps}";
        }
    }
    private void UpdateFightAttackString()
    {
        //Update fight attack string
        currentFightInfo.SetCurrentAttack(fightAttacks[currentFightInfo.CurrentFight][currentFightInfo.CurrentAttackIndex]);
    }

    private void SetBoss() 
    {
        //Default start
        BossManager.Bosses whichBoss = BossManager.Bosses.Wicked_Thunder;

        if (bossManager != null)
        {
            // M4S
            whichBoss = BossManager.Bosses.Wicked_Thunder;
            if (currentFightInfo.CurrentFight == FightEnum.M4S) { bossManager.SetupBoss(whichBoss); }
        }
    }    
    private void SetPlayer() 
    {

        //Default start
        CharacterInfo.Jobs whichJob = CharacterInfo.Jobs.WHM;
        CharacterInfo.RolePositions whichRolePos = CharacterInfo.RolePositions.R1;
        // the grid will always be 1, 2, 3, 4, or 5 prefabs wide
        int role = Random.Range(0, 8);
        switch (role)
        {
            case 0: whichRolePos = CharacterInfo.RolePositions.MT; break;
            case 1: whichRolePos = CharacterInfo.RolePositions.OT; break;
            case 2: whichRolePos = CharacterInfo.RolePositions.H1; break;
            case 3: whichRolePos = CharacterInfo.RolePositions.H2; break;
            case 4: whichRolePos = CharacterInfo.RolePositions.M1; break;
            case 5: whichRolePos = CharacterInfo.RolePositions.M2; break;
            case 6: whichRolePos = CharacterInfo.RolePositions.R1; break;
            default : whichRolePos = CharacterInfo.RolePositions.R2; break;
        }

        int job = 0;
        //Tanks
        if (whichRolePos == CharacterInfo.RolePositions.MT || whichRolePos == CharacterInfo.RolePositions.OT)
        {
            job = Random.Range(0, 4);
            switch (role)
            {
                case 0: whichJob = CharacterInfo.Jobs.PLD; break;
                case 1: whichJob = CharacterInfo.Jobs.WAR; break;
                case 2: whichJob = CharacterInfo.Jobs.DRK; break;
                default: whichJob = CharacterInfo.Jobs.GNB; break;
            }
        }
        //Heals
        if (whichRolePos == CharacterInfo.RolePositions.H1 || whichRolePos == CharacterInfo.RolePositions.H2)
        {
            job = Random.Range(0,4);
            switch (role)
            {
                case 0: whichJob  = CharacterInfo.Jobs.WHM; break;
                case 1: whichJob  = CharacterInfo.Jobs.SCH; break;
                case 2: whichJob  = CharacterInfo.Jobs.AST; break;
                default: whichJob = CharacterInfo.Jobs.SGE; break;
            }
        }
        //M Dps
        if (whichRolePos == CharacterInfo.RolePositions.M1 || whichRolePos == CharacterInfo.RolePositions.M2)
        {
            job = Random.Range(0, 6);
            switch (role)
            {
                case 0: whichJob = whichJob  = CharacterInfo.Jobs.MNK; break;
                case 1: whichJob = whichJob  = CharacterInfo.Jobs.DRG; break;
                case 2: whichJob = whichJob  = CharacterInfo.Jobs.NIN; break;
                case 3: whichJob = whichJob  = CharacterInfo.Jobs.SAM; break;
                case 4: whichJob = whichJob  = CharacterInfo.Jobs.RPR; break;
                default: whichJob = whichJob  = CharacterInfo.Jobs.VIP; break;
            }
        }
        //R Dps
        if (whichRolePos == CharacterInfo.RolePositions.R1 || whichRolePos == CharacterInfo.RolePositions.R2)
        {
            job = Random.Range(0, 7);
            switch (role)
            {
                case 0: whichJob = whichJob = CharacterInfo.Jobs.BRD; break;
                case 1: whichJob = whichJob  = CharacterInfo.Jobs.MCH; break;
                case 2: whichJob = whichJob  = CharacterInfo.Jobs.DNC; break;
                case 3: whichJob = whichJob  = CharacterInfo.Jobs.PIC; break;
                case 4: whichJob = whichJob  = CharacterInfo.Jobs.BLM; break;
                case 5: whichJob = whichJob  = CharacterInfo.Jobs.SMN; break;
                default: whichJob = whichJob  = CharacterInfo.Jobs.RDM; break;
            }
        }

        //Default start
        whichJob = CharacterInfo.Jobs.WHM;
        whichRolePos = CharacterInfo.RolePositions.H2;


        if (playerManager != null)
        {
            playerManager.SetupPlayer(whichJob, whichRolePos);
        }
    }
    private void SetupParty()
    {
        //Should be ran after Set Player
        //Send Setup command to npc manager w/ which role player picked
        PlayerManager pm = playerManager;
        npcManager.SetupParty(pm.GetRolePosition(), pm.GetJob());
    }
    private void SetWaymarks() 
    {
        //Default start
        Waymarks.WaymarkSets whichWaymarks = Waymarks.WaymarkSets.M4S_Hector;

        if (waymarks != null)
        {
            //M4S
            if (currentFightInfo.CurrentFight == FightEnum.M4S) { waymarks.SetWaymarkUsingSets(whichWaymarks); }

        }
    }

    private void UpdateFightActions()
    {

    }
    #region // Reset Fight
    // Function to reset the current fight
    public void ResetFight()
    {
        Debug.Log("Resetting the fight.");
        StartFight(currentFightInfo.CurrentFight);  // Restart the current fight
    }
    #endregion
    // New functions

    // Called at the start of each new step
    private void ExecuteStartOfStep()
    {
        if (GetIsDebugging())
        { 
            Debug.Log("Executing Start of Step");
            // Add custom behavior for start of step
            // Get the current fight, attack, and step from FightManager
            Debug.Log("Updating fight actions");
            Debug.Log($"Current Fight: {currentFightInfo.CurrentFight}");
            Debug.Log($"Current Attack: {currentFightInfo.CurrentAttack}");
            Debug.Log($"Current Step: {currentFightInfo.CurrentStep}");
        }



        if (bossManager == null) { Debug.LogError("BossManager isn't found"); return; }
        BossManager bm = bossManager;
        EffectsManager em = effects;
        PlayerPositionManager ppm = playerPositionManager;
        PlayerManager pm = playerManager;
        CurrentFightInfo cfi = currentFightInfo;
        DamageMarkerManager dm = damageManager;
        NpcManagers nm = npcManager;
        float zp = transform.position.z;
        //Specific fight action

        #region//M4S
        //Goes from x: -5 to 5, 
        if (currentFightInfo.CurrentFight == FightEnum.M4S)
        {
            if (currentFightInfo.CurrentAttack == "Bewitching Flight")
            {
                if (currentFightInfo.CurrentStep == 0)
                {
                    #region //Setup Party Start Positions
                    #region // Move player to starting position
                    Vector3 startPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                    float startRot = 0;
                    CharacterInfo.RolePositions rolePos = pm.GetRolePosition();
                    switch (rolePos)
                    {
                        case CharacterInfo.RolePositions.MT:
                            startPos.x = -.1f; startPos.y = -2.05f; break;
                        case CharacterInfo.RolePositions.OT:
                            startPos.x = .1f; startPos.y = -3.75f; break;
                        case CharacterInfo.RolePositions.H1:
                            startPos.x = -0.8f; startPos.y = -2.9f; break;
                        case CharacterInfo.RolePositions.H2:
                            startPos.x = 0.8f; startPos.y = -2.9f; break;
                        case CharacterInfo.RolePositions.M1:
                            startPos.x = -0.8f; startPos.y = -3.75f; break;
                        case CharacterInfo.RolePositions.M2:
                            startPos.x = 0.8f; startPos.y = -3.75f; break;
                        case CharacterInfo.RolePositions.R1:
                            startPos.x = -0.8f; startPos.y = -2.05f; break;
                        case CharacterInfo.RolePositions.R2:
                            startPos.x = 0.8f; startPos.y = -2.05f; break;
                        default: Debug.LogError("Unknown Role Position"); break;
                    }
                    if (isDebugging) { Debug.Log($"Moving player to start position: {startPos}, with rotation: {startRot}"); }
                    // Move player to starting position
                    pm.MovePlayer(startPos, startRot, 0f); // "Snap" to position
                    #endregion
                    #region // Move Party to starting position
                    //MT Position
                    startPos.x = 0f; startPos.y = -2.05f;
                    nm.MoveNpc(startPos, startRot, 0f, CharacterInfo.RolePositions.MT);
                    //OT Position
                    startPos.x = 0f; startPos.y = -3.75f;
                    nm.MoveNpc(startPos, startRot, 0f, CharacterInfo.RolePositions.OT);
                    //H1 Position
                    startPos.x = -0.8f; startPos.y = -2.9f;
                    nm.MoveNpc(startPos, startRot, 0f, CharacterInfo.RolePositions.H1);
                    //H2 Position
                    startPos.x = 0.8f; startPos.y = -3.75f;
                    nm.MoveNpc(startPos, startRot, 0f, CharacterInfo.RolePositions.H2);
                    //M1 Position
                    startPos.x = -0.8f; startPos.y = -3.75f;
                    nm.MoveNpc(startPos, startRot, 0f, CharacterInfo.RolePositions.M1);
                    //M2 Position
                    startPos.x = 0.8f; startPos.y = -3.75f;
                    nm.MoveNpc(startPos, startRot, 0f, CharacterInfo.RolePositions.M2);
                    //R1 Position
                    startPos.x = -0.8f; startPos.y = -2.05f;
                    nm.MoveNpc(startPos, startRot, 0f, CharacterInfo.RolePositions.R1);
                    //R2 Position
                    startPos.x = 0.8f; startPos.y = -2.05f;
                    nm.MoveNpc(startPos, startRot, 0f, CharacterInfo.RolePositions.R2);
                    #endregion
                    #endregion
                    //Move Boss To Center
                    bm.MoveBoss(new Vector3(0f, 0f, zp), 180, 0f);
                    //Change boss to base form
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
                // Move Party to waypoints
                else if (currentFightInfo.CurrentStep == 1)
                {
                    #region //Setup Party Waymark Positions
                    #region // Move player to Waymark positions
                    Vector3 startPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                    float startRot = 0;
                    CharacterInfo.RolePositions rolePos = pm.GetRolePosition();
                    switch (rolePos)
                    {
                        case CharacterInfo.RolePositions.MT:
                            startPos.x = -.1f; startPos.y = 2.2f; break;
                        case CharacterInfo.RolePositions.OT:
                            startPos.x = -.1f; startPos.y = -2.0f; break;
                        case CharacterInfo.RolePositions.H1:
                            startPos.x = -2.35f; startPos.y = .1f; break;
                        case CharacterInfo.RolePositions.H2:
                            startPos.x = 2.25f; startPos.y = .0f; break;
                        case CharacterInfo.RolePositions.M1:
                            startPos.x = -2.35f; startPos.y = -2.0f; break;
                        case CharacterInfo.RolePositions.M2:
                            startPos.x = 2.15f; startPos.y = -2.0f; break;
                        case CharacterInfo.RolePositions.R1:
                            startPos.x = -2.35f; startPos.y = 2.2f; break;
                        case CharacterInfo.RolePositions.R2:
                            startPos.x = 2.15f; startPos.y = 2.2f; break;
                        default: Debug.LogError("Unknown Role Position"); break;
                    }
                    if (isDebugging) { Debug.Log($"Moving player to start position: {startPos}, with rotation: {startRot}"); }
                    // Move player to Waymark position
                    pm.MovePlayer(startPos, startRot, 1f); // "Snap" to position
                    #endregion
                    #region // Move Party to waymark position
                    #region // Setup position values
                    Vector3 npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                    float xLeftNpcPos = -2.25f; float xMidNpcPos = -.00f; float xRightNpcPos = 2.25f;
                    float yTopNpcPos = 2.0f; float yMidNpcPos = -.00f; float yBotNpcPos = -2.0f;
                    float npcRot = 0; float npcTime = .75f;
                    #endregion
                    #region // Move Party to positions
                    //MT Position
                    npcPos.x = xMidNpcPos; npcPos.y = yTopNpcPos;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                    //OT Position
                    npcPos.x = xMidNpcPos; npcPos.y = yBotNpcPos;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                    //H1 Position
                    npcPos.x = xLeftNpcPos; npcPos.y = yMidNpcPos;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                    //H2 Position
                    npcPos.x = xRightNpcPos; npcPos.y = xRightNpcPos;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                    //M1 Position
                    npcPos.x = xLeftNpcPos; npcPos.y = yBotNpcPos;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                    //M2 Position
                    npcPos.x = xRightNpcPos; npcPos.y = yBotNpcPos;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                    //R1 Position
                    npcPos.x = xLeftNpcPos; npcPos.y = yTopNpcPos;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                    //R2 Position
                    npcPos.x = xRightNpcPos; npcPos.y = yTopNpcPos;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                    #endregion
                    #endregion

                    #endregion
                }
                // Move North
                else if (currentFightInfo.CurrentStep == 2)
                {
                    #region // Move Party to North position
                    #region // Setup position values
                    Vector3 npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                    float npcRot = 0; float npcTime = .75f;
                    #endregion
                    #region // Move Party to positions
                    //MT Position
                    npcPos.x = -0.5f; npcPos.y = 3.1f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                    //OT Position
                    npcPos.x = 0.5f; npcPos.y = 3.1f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                    //H1 Position
                    npcPos.x = -.6f; npcPos.y = .5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                    //H2 Position
                    npcPos.x = .6f; npcPos.y = .5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                    //M1 Position
                    npcPos.x = -1.5f; npcPos.y = 3.5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                    //M2 Position
                    npcPos.x = 1.5f; npcPos.y = 3.5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                    //R1 Position
                    npcPos.x = -1.8f; npcPos.y = 1.5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                    //R2 Position
                    npcPos.x = 1.8f; npcPos.y = 1.5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                    #endregion
                    #endregion
                    bm.MoveBoss(new Vector3(0f, 4.8f, zp), 0, 1f);
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Wings_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
                // Start Lasters
                else if (currentFightInfo.CurrentStep == 3)
                {
                    #region // Move Boss North
                    bm.MoveBoss(new Vector3(0f, 5.1f, zp), 0, 0f);
                    #endregion
                    #region // Get Wicked Thunder Settings
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Wings_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale, 1f, .8f, 0f, .2f);
                    #endregion
                    #region // Initalize effects vars
                    FightEnum fight = cfi.CurrentFight;
                    string attack = cfi.CurrentAttack;
                    int step = cfi.CurrentStep;
                    #endregion
                    #region// Spawn laser
                    int laserRand = wts.Bf_LaserDashRand;
                    if (laserRand == 0)
                    {
                        em.CreateFX(Effects.WickedThunderBeam1, FxLifeTimeType.Step, new Vector3(-.2f, 4.85f, -1f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderBeam2, FxLifeTimeType.Step, new Vector3(-1.8f, 4.9f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderBeam3, FxLifeTimeType.Step, new Vector3(-2.5f, 5.2f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderBeam4, FxLifeTimeType.Step, new Vector3(0.9f, 4.9f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderBeam5, FxLifeTimeType.Step, new Vector3(1.9f, 5.2f, -.4f), 0, fight, attack, step);
                    }
                    else
                    {
                        GameObject inst, inst1, inst2, inst3, inst4;
                        inst = em.CreateFX(Effects.WickedThunderBeam1, FxLifeTimeType.Step, new Vector3(.3f, 4.85f, -01f), 0, fight, attack, step);
                        inst1 = em.CreateFX(Effects.WickedThunderBeam2, FxLifeTimeType.Step, new Vector3(1.9f, 4.90f, -.4f), 0, fight, attack, step);
                        inst2 = em.CreateFX(Effects.WickedThunderBeam3, FxLifeTimeType.Step, new Vector3(2.6f, 5.20f, -.4f), 0, fight, attack, step);
                        inst3 = em.CreateFX(Effects.WickedThunderBeam4, FxLifeTimeType.Step, new Vector3(-0.8f, 4.90f, -.4f), 0, fight, attack, step);
                        inst4 = em.CreateFX(Effects.WickedThunderBeam5, FxLifeTimeType.Step, new Vector3(-1.8f, 5.20f, -.4f), 0, fight, attack, step);

                        // Set a new local rotation using a Quaternion
                        inst.transform.localRotation = Quaternion.Euler(0, -180, 0); // Replace with desired rotation angles (x, y, z)
                        inst1.transform.localRotation = Quaternion.Euler(0, -180, 0); // Replace with desired rotation angles (x, y, z)
                        inst2.transform.localRotation = Quaternion.Euler(0, -180, 0); // Replace with desired rotation angles (x, y, z)
                        inst3.transform.localRotation = Quaternion.Euler(0, -180, 0); // Replace with desired rotation angles (x, y, z)
                        inst4.transform.localRotation = Quaternion.Euler(0, -180, 0); // Replace with desired rotation angles (x, y, z)
                    }
                    #endregion
                    #region // Spawn Elctromines
                    int electroIan = wts.Bf_ElctroMineRand;
                    if (electroIan == 0)
                    {
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, 2.5f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, .5f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, -1.5f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, -3.5f, -.4f), 0, fight, attack, step);
                    }
                    else
                    {
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, 3.6f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, 1.6f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, -.4f, -.4f), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, -2.4f, -.4f), 0, fight, attack, step);
                    }
                    #endregion
                    #region // Setup guide points
                    ppm.SetupGuidePoints();
                    #endregion
                }
                //Set Lines - 3
                else if (currentFightInfo.CurrentStep == 4)
                {

                    #region // Move Party to North position
                    #region // Setup position values
                    Vector3 npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                    float npcRot = 0; float npcTime = .75f;
                    #endregion
                    #region // Move Party to positions
                    //MT Position
                    npcPos.x = -0.5f; npcPos.y = 3.1f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                    //OT Position
                    npcPos.x = 0.5f; npcPos.y = 3.1f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                    //H1 Position
                    npcPos.x = -.6f; npcPos.y = .5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                    //H2 Position
                    npcPos.x = .6f; npcPos.y = .5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                    //M1 Position
                    npcPos.x = -1.5f; npcPos.y = 3.5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                    //M2 Position
                    npcPos.x = 1.5f; npcPos.y = 3.5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                    //R1 Position
                    npcPos.x = -1.8f; npcPos.y = 1.5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                    //R2 Position
                    npcPos.x = 1.8f; npcPos.y = 1.5f;
                    nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                    #endregion
                    #endregion

                    #region // Get Wicked Thunder Settings
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    #endregion

                    #region// Create damage markers
                    float lasterTimer = 1f;
                    int laserRand = wts.Bf_LaserDashRand;
                    if (laserRand == 0)
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(-4.60f, -0f, zp), .8f, 8f, lasterTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-2.75f, -0f, zp), .8f, 8f, lasterTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.90f, -0f, zp), .8f, 8f, lasterTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(+0.95f, -0f, zp), .8f, 8f, lasterTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(+2.80f, -0f, zp), .8f, 8f, lasterTimer);
                    }
                    else
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(-3.60f, -0f, zp), .8f, 8f, lasterTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-1.75f, -0f, zp), .8f, 8f, lasterTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(+0.01f, -0f, zp), .8f, 8f, lasterTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(+1.95f, -0f, zp), .8f, 8f, lasterTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(+3.80f, -0f, zp), .8f, 8f, lasterTimer);
                    }
                    #endregion

                    #region// Create damage markers for electromines
                    float mineTimer = 1f;
                    int mineRand = wts.Bf_ElctroMineRand;
                    if (mineRand == 0)
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, .5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, 2.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, -1.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, -3.5f, zp), 9f, 1f, mineTimer);
                    }
                    else
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, 1.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, 3.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, -0.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, -2.5f, zp), 9f, 1f, mineTimer);
                    }
                    #endregion

                    #region // Move party based on mines
                    #region //Bottom Mines and Near right boss clear
                    if (laserRand == 0 && mineRand == 0)
                    {
                        #region // Move Party to North position
                        #region // Setup position values
                        npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                        npcRot = 0; npcTime = .2f;
                        #endregion
                        #region // Move Party to positions
                        //MT Position
                        npcPos.x = -1.5f; npcPos.y = 3.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                        //OT Position
                        npcPos.x = 0.5f; npcPos.y = 3.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                        ////H1 Position
                        npcPos.x = -1.5f; npcPos.y = 1.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                        ////H2 Position
                        npcPos.x = 0.5f; npcPos.y = 1.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                        ////M1 Position
                        npcPos.x = -1.5f; npcPos.y = 3.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                        ////M2 Position
                        npcPos.x = 0.5f; npcPos.y = 3.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                        ////R1 Position
                        npcPos.x = -1.5f; npcPos.y = 1.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                        ////R2 Position
                        npcPos.x = 0.5f; npcPos.y = 1.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region //Bottom Mines and Near left boss clear
                    else if (laserRand == 1 && mineRand == 0)
                    {
                        #region // Move Party to North position
                        #region // Setup position values
                        npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                        npcRot = 0; npcTime = .2f;
                        #endregion
                        #region // Move Party to positions
                        //MT Position
                        npcPos.x = -0.5f; npcPos.y = 3.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                        //OT Position
                        npcPos.x = 1.5f; npcPos.y = 3.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                        ////H1 Position
                        npcPos.x = -0.5f; npcPos.y = 1.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                        ////H2 Position
                        npcPos.x = 1.5f; npcPos.y = 1.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                        ////M1 Position
                        npcPos.x = -0.5f; npcPos.y = 3.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                        ////M2 Position
                        npcPos.x = 1.5f; npcPos.y = 3.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                        ////R1 Position
                        npcPos.x = -0.5f; npcPos.y = 1.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                        ////R2 Position
                        npcPos.x = 1.5f; npcPos.y = 1.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region //Top Mines and Near right boss clear
                    else if (laserRand == 0 && mineRand == 1)
                    {
                        #region // Move Party to North position
                        #region // Setup position values
                        npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                        npcRot = 0; npcTime = .2f;
                        #endregion
                        #region // Move Party to positions
                        //MT Position
                        npcPos.x = -1.5f; npcPos.y = 2.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                        //OT Position
                        npcPos.x = 0.5f; npcPos.y = 2.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                        ////H1 Position
                        npcPos.x = -1.5f; npcPos.y = 0.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                        ////H2 Position
                        npcPos.x = 0.5f; npcPos.y = 0.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                        ////M1 Position
                        npcPos.x = -1.5f; npcPos.y = 32.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                        ////M2 Position
                        npcPos.x = 0.5f; npcPos.y = 2.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                        ////R1 Position
                        npcPos.x = -1.5f; npcPos.y = 0.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                        ////R2 Position
                        npcPos.x = 0.5f; npcPos.y = 0.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region //Bottom Mines and Near left boss clear
                    else if (laserRand == 1 && mineRand == 1)
                    {
                        #region // Move Party to North position
                        #region // Setup position values
                        npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                        npcRot = 0; npcTime = .2f;
                        #endregion
                        #region // Move Party to positions
                        //MT Position
                        npcPos.x = -0.5f; npcPos.y = 2.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                        //OT Position
                        npcPos.x = 1.5f; npcPos.y = 2.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                        ////H1 Position
                        npcPos.x = -0.5f; npcPos.y = 0.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                        ////H2 Position
                        npcPos.x = 1.5f; npcPos.y = 0.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                        ////M1 Position
                        npcPos.x = -0.5f; npcPos.y = 2.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                        ////M2 Position
                        npcPos.x = 1.5f; npcPos.y = 2.6f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                        ////R1 Position
                        npcPos.x = -0.5f; npcPos.y = 0.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                        ////R2 Position
                        npcPos.x = 1.5f; npcPos.y = 0.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                        #endregion
                        #endregion
                    }
                    #endregion
                    #endregion
                }
                // Move Mid, Spawn ground lines
                else if (currentFightInfo.CurrentStep == 5)
                {
                    #region // Move Boss back to mid
                    bm.MoveBoss(new Vector3(0f, 0f, zp), 180, 1f);
                    #endregion
                    #region // Get Wicked Thunder Settings
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                    #endregion

                    #region // Initalize effects vars
                    FightEnum fight = cfi.CurrentFight;
                    string attack = cfi.CurrentAttack;
                    int step = cfi.CurrentStep;
                    #endregion

                    float zp1 = -.1f;
                    #region// Spawn ground lines
                    em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(0.20f, 0f, zp1), 0, fight, attack, step);
                    em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(-1.6f, 0f, zp1), 0, fight, attack, step);
                    em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(-3.4f, 0f, zp1), 0, fight, attack, step);
                    em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(2.00f, 0f, zp1), 0, fight, attack, step);
                    em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(3.80f, 0f, zp1), 0, fight, attack, step);
                    #endregion

                    #region // Spawn Elctromines
                    int electroIan = wts.BF_ElctroMine2Rand;
                    if (electroIan == 0)
                    {
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, 2.5f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, .5f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, -1.5f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, -3.5f, zp1), 0, fight, attack, step);
                    }
                    else
                    {
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, 3.6f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, 1.6f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, zp1, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, -2.4f, zp1), 0, fight, attack, step);
                    }
                    em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(0.1f, 4.4f, zp1), 0, fight, attack, step);
                    #endregion

                }
                // Player move
                else if (currentFightInfo.CurrentStep == 6)
                {
                    #region // Setup guide points
                    ppm.SetupGuidePoints();
                    #endregion

                    #region // Get Wicked Thunder Settings
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    #endregion

                    #region // Initalize effects vars
                    FightEnum fight = cfi.CurrentFight;
                    string attack = cfi.CurrentAttack;
                    int step = cfi.CurrentStep;
                    #endregion

                    float zp1 = -.1f;
                    #region// Flare up ground lines
                    int groundRand = wts.Bf_LaserExplodeRand;
                    if (groundRand == 0)
                    {
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(-3.4f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(3.80f, 0f, zp1), 0, fight, attack, step);

                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(0.20f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(-1.6f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(2.00f, 0f, zp1), 0, fight, attack, step);
                    }
                    else
                    {
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(0.20f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(-1.6f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(2.00f, 0f, zp1), 0, fight, attack, step);

                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(-3.4f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(3.80f, 0f, zp1), 0, fight, attack, step);
                    }
                    #endregion

                    #region // Spawn Elctromines
                    int electroIan = wts.BF_ElctroMine2Rand;

                    if (electroIan == 0)
                    {
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, 2.5f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, .5f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, -1.5f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, -3.5f, zp1), 0, fight, attack, step);
                    }
                    else
                    {
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, 3.6f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, 1.6f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, zp1, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(5f, -2.4f, zp1), 0, fight, attack, step);
                    }
                    em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(0.1f, 4.4f, zp1), 0, fight, attack, step);
                    #endregion
                }
                // Ground lines flare up
                else if (currentFightInfo.CurrentStep == 7)
                {
                    #region // Get Wicked Thunder Settings
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    #endregion

                    #region // Initalize effects vars
                    FightEnum fight = cfi.CurrentFight;
                    string attack = cfi.CurrentAttack;
                    int step = cfi.CurrentStep;
                    #endregion

                    float zp1 = -.1f;
                    #region// Flare up ground lines
                    int groundRand = wts.Bf_LaserExplodeRand;
                    if (groundRand == 0)
                    {
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(-3.4f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(3.80f, 0f, zp1), 0, fight, attack, step);
                    }
                    else
                    {
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(0.20f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(-1.6f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGroundline, FxLifeTimeType.Step, new Vector3(2.00f, 0f, zp1), 0, fight, attack, step);
                    }
                    #endregion

                    #region// Create damage markers
                    float groundTimer = 1f;
                    if (groundRand == 0)
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(1.4f, -0f, zp), 4f, 8f, groundTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-2.40f, -0f, zp), 4f, 8f, groundTimer);

                    }
                    else
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.4f, -0f, zp), 2f, 8f, groundTimer);


                        dm.CreateDamageMarkerRectangle(new Vector3(-3.7f, -0f, zp), 2.6f, 8f, groundTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(2.7f, -0f, zp), 2.6f, 8f, groundTimer);
                    }
                    #endregion

                    #region// Create damage markers for electromines
                    float mineTimer = 1f;
                    int mineRand = wts.BF_ElctroMine2Rand;
                    if (mineRand == 0)
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, .5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, 2.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, -1.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, -3.5f, zp), 9f, 1f, mineTimer);
                    }
                    else
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, 1.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, 3.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, -0.5f, zp), 9f, 1f, mineTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-0.50f, -2.5f, zp), 9f, 1f, mineTimer);
                    }
                    #endregion

                    #region // Create Damage markers for four star
                    int fourRand = wts.Bf_FourStar;
                    Vector3 npcPosition;
                    //Get player role
                    CharacterInfo.RolePositions playerRP = pm.GetRolePosition();
                    // Hit Supp
                    if (fourRand == 0)
                    {
                        
                        #region //Inner flares and bottom mines
                        if (groundRand == 0 && mineRand == 0)
                        {
                            //Hit Supports
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.2f, 1.55f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.2f, -2.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.1f, 1.55f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.1f, -2.5f, -.3f)), 0, fight, attack, step); 
                        }
                        #endregion
                        #region //Outer flaes and bottom mines
                        else if (groundRand == 1 && mineRand == 0)
                        {
                            //Hit Supports
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, 1.55f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, -2.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, 1.55f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, -2.5f, -.3f)), 0, fight, attack, step);
                        }
                        #endregion
                        #region //Inner flares and top mines
                        else if (groundRand == 0 && mineRand == 1)
                        {
                            //Hit Supports
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.2f, 0.55f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.2f, -3.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.1f, 0.55f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.1f, -3.5f, -.3f)), 0, fight, attack, step);
                        }
                        #endregion
                        #region //Outer flaes and top mines
                        else if (groundRand == 1 && mineRand == 1)
                        {
                            //Hit Supports
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, 0.55f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, -3.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, 0.55f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, -3.5f, -.3f)), 0, fight, attack, step);
                        }
                        #endregion

                    }
                    //Hit Dps
                    else
                    {
                        #region //Inner flares and bottom mines
                        if (groundRand == 0 && mineRand == 0)
                        {
                            
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.2f, 3.2f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.2f, -0.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.1f, 3.2f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.1f, -0.5f, -.3f)), 0, fight, attack, step);
                        }
                        #endregion
                        #region //Outer flaes and bottom mines 
                        else if (groundRand == 1 && mineRand == 0)
                        {
                            
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, 3.2f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, -0.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, 3.2f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, -0.5f, -.3f)), 0, fight, attack, step);
                        }
                        #endregion
                        #region //Inner flares and top mines
                        else if (groundRand == 0 && mineRand == 1)
                        {
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.2f, 2.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.2f, -1.6f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.1f, 2.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.1f, -1.6f, -.3f)), 0, fight, attack, step);
                        }
                        #endregion
                        #region //Outer flaes and top mines
                        else if (groundRand == 1 && mineRand == 1)
                        {
                            
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, 2.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, -1.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, 2.5f, -.3f)), 0, fight, attack, step);
                            em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, -1.5f, -.3f)), 0, fight, attack, step);
                        }
                        #endregion
                    }
                    #endregion

                    #region // Create damage markers on all but player 
                    #region //Inner flares and bottom mines
                    if (groundRand == 0 && mineRand == 0)
                    {
                        //Hit Supports
                        if (playerRP != CharacterInfo.RolePositions.MT) { dm.CreateDamageMarkerCircle((new Vector3(-3.7f, 1.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.OT) { dm.CreateDamageMarkerCircle((new Vector3(4.7f, 1.55f, -.3f)), 2f, groundTimer , true); }
                        if (playerRP != CharacterInfo.RolePositions.H1) { dm.CreateDamageMarkerCircle((new Vector3(-3.7f, -2.5f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.H2) { dm.CreateDamageMarkerCircle((new Vector3(4.7f, -2.5f, -.3f)), 2f, groundTimer , true); }
                        //Hit Dps                                                                                                                         truealse
                        if (playerRP != CharacterInfo.RolePositions.R1) { dm.CreateDamageMarkerCircle((new Vector3(-3.7f, 3.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.R2) { dm.CreateDamageMarkerCircle((new Vector3(4.7f, 3.55f, -.3f)), 2f, groundTimer , true); }
                        if (playerRP != CharacterInfo.RolePositions.M1) { dm.CreateDamageMarkerCircle((new Vector3(-3.7f, -0.5f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.M2) { dm.CreateDamageMarkerCircle((new Vector3(4.7f, -0.5f, -.3f)), 2f, groundTimer , true); }
                    }
                    #endregion
                    #region //Outer flaes and bottom mines
                    else if (groundRand == 1 && mineRand == 0)
                    {
                        //Hit Supports
                        if (playerRP != CharacterInfo.RolePositions.MT) { dm.CreateDamageMarkerCircle((new Vector3(-1.2f, 1.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.OT) { dm.CreateDamageMarkerCircle((new Vector3(2.2f, 1.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.H1) { dm.CreateDamageMarkerCircle((new Vector3(-1.2f, -2.5f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.H2) { dm.CreateDamageMarkerCircle((new Vector3(2.2f, -2.5f, -.3f)), 2f, groundTimer, true); }
                        //Hit Dps                                                                                                                             true
                        if (playerRP != CharacterInfo.RolePositions.R1) { dm.CreateDamageMarkerCircle((new Vector3(-1.2f, 3.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.R2) { dm.CreateDamageMarkerCircle((new Vector3(2.2f, 3.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.M1) { dm.CreateDamageMarkerCircle((new Vector3(-1.2f, -0.5f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.M2) { dm.CreateDamageMarkerCircle((new Vector3(2.2f, -0.5f, -.3f)), 2f, groundTimer, true); }
                    }
                    #endregion
                    #region //Inner flares and top mines
                    else if (groundRand == 0 && mineRand == 1)
                    {
                        //Hit Supports
                        if (playerRP != CharacterInfo.RolePositions.MT) { dm.CreateDamageMarkerCircle((new Vector3(-3.7f, 0.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.OT) { dm.CreateDamageMarkerCircle((new Vector3(4.7f, 0.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.H1) { dm.CreateDamageMarkerCircle((new Vector3(-3.7f, -3.5f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.H2) { dm.CreateDamageMarkerCircle((new Vector3(4.7f, -3.5f, -.3f)), 2f, groundTimer, true); }
                        //Hit Dps                                                                                                                         true
                        if (playerRP != CharacterInfo.RolePositions.R1) { dm.CreateDamageMarkerCircle((new Vector3(-3.7f, 2.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.R2) { dm.CreateDamageMarkerCircle((new Vector3(4.7f, 2.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.M1) { dm.CreateDamageMarkerCircle((new Vector3(-3.7f, -1.5f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.M2) { dm.CreateDamageMarkerCircle((new Vector3(4.7f, -1.5f, -.3f)), 2f, groundTimer, true); }
                    }
                    #endregion
                    #region //Outer flaes and top mines
                    else if (groundRand == 1 && mineRand == 1)
                    {
                        //Hit Supports
                        if (playerRP != CharacterInfo.RolePositions.MT) { dm.CreateDamageMarkerCircle((new Vector3(-1.2f, 0.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.OT) { dm.CreateDamageMarkerCircle((new Vector3(2.2f, 0.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.H1) { dm.CreateDamageMarkerCircle((new Vector3(-1.2f, -3.5f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.H2) { dm.CreateDamageMarkerCircle((new Vector3(2.2f, -3.5f, -.3f)), 2f, groundTimer, true); }
                        //Hit Dps                                                                                                                              true
                        if (playerRP != CharacterInfo.RolePositions.R1) { dm.CreateDamageMarkerCircle((new Vector3(-1.2f, 2.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.R2) { dm.CreateDamageMarkerCircle((new Vector3(2.2f, 2.55f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.M1) { dm.CreateDamageMarkerCircle((new Vector3(-1.2f, -1.5f, -.3f)), 2f, groundTimer, true); }
                        if (playerRP != CharacterInfo.RolePositions.M2) { dm.CreateDamageMarkerCircle((new Vector3(2.2f, -1.5f, -.3f)), 2f, groundTimer, true); }
                    }
                    #endregion
                    #endregion

                    #region // Create near or far marker
                    int nfRand = wts.Bf_NearFarRand;
                    if (nfRand == 0)
                    {
                        em.CreateFX(Effects.WickedThunderNearMark, FxLifeTimeType.Step, new Vector3(0f, 0f, -.3f), 0, fight, attack, step);
                    } 
                    else
                    {
                        em.CreateFX(Effects.WickedThunderFarMark, FxLifeTimeType.Step, new Vector3(0f, 0f, -.3f), 0, fight, attack, step);
                    }
                    #endregion

                    #region // Move party based on flareups
                    Vector3 npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                    float npcRot = 0; float npcTime = .2f;
                    #region //Inner flares and bottom mines
                    if (groundRand == 0 && mineRand == 0)
                    {
                        #region // Move Party to North position
                        #region // Setup position values
                        #endregion
                        #region // Move Party to positions
                        //MT Position
                        npcPos.x = -4.2f; npcPos.y = 1.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                        //OT Position
                        npcPos.x = 4.2f; npcPos.y = 1.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                        ////H1 Position
                        npcPos.x = -4.2f; npcPos.y = -2.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                        ////H2 Position
                        npcPos.x = 4.2f; npcPos.y = -2.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                        ////M1 Position
                        npcPos.x = -4.2f; npcPos.y = -0.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                        ////M2 Position
                        npcPos.x = 4.2f; npcPos.y = -0.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                        ////R1 Position
                        npcPos.x = -4.2f; npcPos.y = 3.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                        ////R2 Position
                        npcPos.x = 4.2f; npcPos.y = 3.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region //Outer flaes and bottom mines
                    else if (groundRand == 1 && mineRand == 0)
                    {
                        #region // Move Party to North position
                        #region // Setup position values
                        npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                        npcRot = 0; npcTime = .2f;
                        #endregion
                        #region // Move Party to positions
                        //MT Position
                        npcPos.x = -1.4f; npcPos.y = 1.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                        //OT Position
                        npcPos.x = 1.5f; npcPos.y = 1.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                        ////H1 Position
                        npcPos.x = -1.4f; npcPos.y = -2.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                        ////H2 Position
                        npcPos.x = 1.5f; npcPos.y = -2.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                        ////M1 Position
                        npcPos.x = -1.4f; npcPos.y = -0.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                        ////M2 Position
                        npcPos.x = 1.5f; npcPos.y = -0.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                        ////R1 Position
                        npcPos.x = -1.4f; npcPos.y = 3.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                        ////R2 Position
                        npcPos.x = 1.5f; npcPos.y = 3.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region //Inner flares and top mines
                    else if (groundRand == 0 && mineRand == 1)
                    {
                        #region // Move Party to North position
                        #region // Setup position values
                        npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                        npcRot = 0; npcTime = .2f;
                        #endregion
                        #region // Move Party to positions
                        //MT Position
                        npcPos.x = -4.2f; npcPos.y = 0.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                        //OT Position
                        npcPos.x = 4.2f; npcPos.y = 0.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                        ////H1 Position
                        npcPos.x = -4.2f; npcPos.y = -3.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                        ////H2 Position
                        npcPos.x = 4.2f; npcPos.y = -3.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                        ////M1 Position
                        npcPos.x = -4.2f; npcPos.y = -1.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                        ////M2 Position
                        npcPos.x = 4.2f; npcPos.y = -1.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                        ////R1 Position
                        npcPos.x = -4.2f; npcPos.y = 2.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                        ////R2 Position
                        npcPos.x = 4.2f; npcPos.y = 2.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region //Outer flaes and top mines
                    else if (groundRand == 1 && mineRand == 1)
                    {
                        #region // Move Party to North position
                        #region // Setup position values
                        npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                        npcRot = 0; npcTime = .2f;
                        #endregion
                        #region // Move Party to positions
                        //MT Position
                        npcPos.x = -1.4f; npcPos.y = 0.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                        //OT Position
                        npcPos.x = 1.5f; npcPos.y = 0.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                        ////H1 Position
                        npcPos.x = -1.4f; npcPos.y = -3.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                        ////H2 Position
                        npcPos.x = 1.5f; npcPos.y = -3.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                        ////M1 Position
                        npcPos.x = -1.4f; npcPos.y = -1.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                        ////M2 Position
                        npcPos.x = 1.5f; npcPos.y = -1.55f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                        ////R1 Position
                        npcPos.x = -1.4f; npcPos.y = 2.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                        ////R2 Position
                        npcPos.x = 1.5f; npcPos.y = 2.5f;
                        nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                        #endregion
                        #endregion
                    }
                    #endregion
                    #endregion

                }
                // Second Ground lines flare up
                else if (currentFightInfo.CurrentStep == 8)
                {
                    #region // Initalize effects vars
                    FightEnum fight = cfi.CurrentFight;
                    string attack = cfi.CurrentAttack;
                    int step = cfi.CurrentStep;
                    float zp1 = -.1f;
                    #endregion
                    #region // Get Wicked Thunder Settings
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    #endregion
                    #region// Flare up second ground lines
                    int groundRand = wts.Bf_LaserExplodeRand;
                    if (groundRand == 0)
                    {
                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(-3.4f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(3.80f, 0f, zp1), 0, fight, attack, step);

                    }
                    else
                    {
                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(0.20f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(-1.6f, 0f, zp1), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderGlFlare, FxLifeTimeType.Step, new Vector3(2.00f, 0f, zp1), 0, fight, attack, step);

                    }
                    #endregion
                    #region // Create near or far marker
                    int nfRand = wts.Bf_NearFarRand;
                    if (nfRand == 0)
                    {
                        em.CreateFX(Effects.WickedThunderNearMark, FxLifeTimeType.Step, new Vector3(0f, 0f, -.3f), 0, fight, attack, step);
                    }
                    else
                    {
                        em.CreateFX(Effects.WickedThunderFarMark, FxLifeTimeType.Step, new Vector3(0f, 0f, -.3f), 0, fight, attack, step);
                    }
                    #endregion

                    #region // Setup guide points
                    ppm.SetupGuidePoints();
                    #endregion
                }
                // Resolve Final Bewitching Flight
                else if (currentFightInfo.CurrentStep == 9)
                {
                    #region // Initalize effects vars
                    FightEnum fight = cfi.CurrentFight;
                    string attack = cfi.CurrentAttack;
                    int step = cfi.CurrentStep;
                    float zp1 = -.1f;
                    #endregion
                    #region // Get Wicked Thunder Settings
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    #endregion
                    #region // Create Second Damage markers for four star
                    int groundRand = wts.Bf_LaserExplodeRand;
                    int nfRand = wts.Bf_NearFarRand;
                    int fourRand = wts.Bf_FourStar;
                    Vector3 npcPosition;
                    //Get player role
                    CharacterInfo.RolePositions playerRP = pm.GetRolePosition();
                    //Outer Flares + Near Marker
                    if (groundRand == 0 && nfRand == 0)
                    {
                        // Hit Dps
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, 1.0f, -.3f)), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, -1.0f, -.3f)), 0, fight, attack, step);

                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, 1.0f, -.3f)), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, -1.0f, -.3f)), 0, fight, attack, step);
                    }
                    //Inner Flare + Near Markers
                    else if (groundRand == 1 && nfRand == 0)
                    {
                        // Hit Dps
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.0f, 1.0f, -.3f)), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.0f, -1.0f, -.3f)), 0, fight, attack, step);

                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.0f, 1.0f, -.3f)), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.0f, -1.0f, -.3f)), 0, fight, attack, step);
                    }
                    //Outer Flares + Far Marker
                    else if (groundRand == 0 && nfRand == 1)
                    {
                        // Hit Dps
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, 3.0f, -.3f)), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-1.5f, -3.0f, -.3f)), 0, fight, attack, step);

                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, 3.0f, -.3f)), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(1.5f, -3.0f, -.3f)), 0, fight, attack, step);
                    }
                    //Inner Flare + Far Markers
                    else if (groundRand == 1 && nfRand == 1)
                    {
                        // Hit Dps
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.0f, 3.0f, -.3f)), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(-4.0f, -3.0f, -.3f)), 0, fight, attack, step);

                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.0f, 3.0f, -.3f)), 0, fight, attack, step);
                        em.CreateFX(Effects.WickedThunderFourStar, FxLifeTimeType.Step, (new Vector3(4.0f, -3.0f, -.3f)), 0, fight, attack, step);
                    }
                    #endregion
                    #region // Create Damage makrkers for second flare ups 
                    groundRand = wts.Bf_LaserExplodeRand;
                    float groundTimer = 1f;
                    if (groundRand == 1)
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(1.4f, -0f, zp), 4f, 8f, groundTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(-2.40f, -0f, zp), 4f, 8f, groundTimer);

                    }
                    else
                    {
                        dm.CreateDamageMarkerRectangle(new Vector3(-3.7f, -0f, zp), 2.6f, 8f, groundTimer);
                        dm.CreateDamageMarkerRectangle(new Vector3(2.7f, -0f, zp), 2.6f, 8f, groundTimer);
                    }
                    #endregion
                    #region // Move party based on flareups
                    Vector3 npcPos = new Vector3(transform.position.x, transform.position.y, 0f); // Z Position is set in the player object
                    float npcRot = 0; float npcTime = .2f;
                    #region //Inner flares and bottom mines
                    //Out -> In
                    if (groundRand == 0)
                    {
                        // Supps Hit first
                        if (fourRand == 0) 
                        {
                            // Near Marker
                            if (nfRand == 0)
                            {
                                #region // Outer->Inner / Supp 1st / Near 
                                #region // Move Party to final Position
                                //MT Position
                                npcPos.x = -1.5f; npcPos.y = 3.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                                //OT Position
                                npcPos.x = 1.5f; npcPos.y = 3.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                                ////H1 Position
                                npcPos.x = -1.5f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                                ////H2 Position
                                npcPos.x = 1.5f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                                ////M1 Position
                                npcPos.x = -1.5f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                                ////M2 Position
                                npcPos.x = 1.5f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                                ////R1 Position
                                npcPos.x = -1.5f; npcPos.y = 1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                                ////R2 Position
                                npcPos.x = 1.5f; npcPos.y = 1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                                #endregion
                                #endregion
                            }
                            // Far Marker
                            else
                            {
                                #region // Outer->Inner / Supp 1st / Far 
                                #region // Move Party to final Position
                                //MT Position
                                npcPos.x = -1.5f; npcPos.y = 1.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                                //OT Position
                                npcPos.x = 1.5f; npcPos.y = 1.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                                ////H1 Position
                                npcPos.x = -1.5f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                                ////H2 Position
                                npcPos.x = 1.5f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                                ////M1 Position
                                npcPos.x = -1.5f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                                ////M2 Position
                                npcPos.x = 1.5f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                                ////R1 Position
                                npcPos.x = -1.5f; npcPos.y = 3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                                ////R2 Position
                                npcPos.x = 1.5f; npcPos.y = 3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                                #endregion
                                #endregion
                            }
                        }
                        //DPS hit first
                        else
                        {
                            // Near Marker
                            if (nfRand == 0)
                            {
                                #region // Outer->Inner / DPS 1st / Near 
                                #region // Move Party to final Position
                                //MT Position
                                npcPos.x = -1.5f; npcPos.y = 1.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                                //OT Position
                                npcPos.x = 1.5f; npcPos.y = 1.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                                ////H1 Position
                                npcPos.x = -1.5f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                                ////H2 Position
                                npcPos.x = 1.5f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                                ////M1 Position
                                npcPos.x = -1.5f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                                ////M2 Position
                                npcPos.x = 1.5f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                                ////R1 Position
                                npcPos.x = -1.5f; npcPos.y = 3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                                ////R2 Position
                                npcPos.x = 1.5f; npcPos.y = 3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                                #endregion
                                #endregion
                            }
                            // Far Marker
                            else
                            {
                                #region // Outer->Inner / DPS 1st / Far 
                                #region // Move Party to final Position
                                //MT Position
                                npcPos.x = -1.5f; npcPos.y = 3.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                                //OT Position
                                npcPos.x = 1.5f; npcPos.y = 3.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                                ////H1 Position
                                npcPos.x = -1.5f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                                ////H2 Position
                                npcPos.x = 1.5f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                                ////M1 Position
                                npcPos.x = -1.5f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                                ////M2 Position
                                npcPos.x = 1.5f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                                ////R1 Position
                                npcPos.x = -1.5f; npcPos.y = 1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                                ////R2 Position
                                npcPos.x = 1.5f; npcPos.y = 1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                                #endregion
                                #endregion
                            }
                        }
                    } 
                    #endregion
                    #region //In -> Out
                    else
                    {
                        // Supps Hit first
                        if (fourRand == 0)
                        {
                            // Near Marker
                            if (nfRand == 0)
                            {
                                #region // In -> Out / Supp 1st / Near 
                                #region // Move Party to final Position
                                //MT Position
                                npcPos.x = -4.2f; npcPos.y = 3.2f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                                //OT Position
                                npcPos.x = 4.2f; npcPos.y = 3.2f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                                ////H1 Position
                                npcPos.x = -4.2f; npcPos.y = -2.5f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                                ////H2 Position
                                npcPos.x = 4.2f; npcPos.y = -2.5f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                                ////M1 Position
                                npcPos.x = -4.2f; npcPos.y = -0.8f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                                ////M2 Position
                                npcPos.x = 4.2f; npcPos.y = -0.8f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                                ////R1 Position
                                npcPos.x = -4.2f; npcPos.y = 1.2f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                                ////R2 Position
                                npcPos.x = 4.2f; npcPos.y = 1.2f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                                #endregion
                                #endregion
                            }
                            // Far Marker
                            else
                            {
                                #region // Outer->Inner / Supp 1st / Far 
                                #region // Move Party to final Position
                                //MT Position
                                npcPos.x = -4.2f; npcPos.y = 1.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                                //OT Position
                                npcPos.x = 4.2f; npcPos.y = 1.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                                ////H1 Position
                                npcPos.x = -4.2f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                                ////H2 Position
                                npcPos.x = 4.2f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                                ////M1 Position
                                npcPos.x = -4.2f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                                ////M2 Position
                                npcPos.x = 4.2f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                                ////R1 Position
                                npcPos.x = -4.2f; npcPos.y = 3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                                ////R2 Position
                                npcPos.x = 4.2f; npcPos.y = 3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                                #endregion
                                #endregion
                            }
                        }
                        //DPS hit first
                        else
                        {
                            // Near Marker
                            if (nfRand == 0)
                            {
                                #region // In -> Out / DPS 1st / Near 
                                #region // Move Party to final Position
                                //MT Position
                                npcPos.x = -4.2f; npcPos.y = 1.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                                //OT Position
                                npcPos.x = 4.2f; npcPos.y = 1.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                                ////H1 Position
                                npcPos.x = -4.2f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                                ////H2 Position
                                npcPos.x = 4.2f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                                ////M1 Position
                                npcPos.x = -4.2f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                                ////M2 Position
                                npcPos.x = 4.2f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                                ////R1 Position
                                npcPos.x = -4.2f; npcPos.y = 3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                                ////R2 Position
                                npcPos.x = 4.2f; npcPos.y = 3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                                #endregion
                                #endregion
                            }
                            // Far Marker
                            else
                            {
                                #region // In -> Out / DPS 1st / Far 
                                #region // Move Party to final Position
                                //MT Position
                                npcPos.x = -4.2f; npcPos.y = 3.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.MT);
                                //OT Position
                                npcPos.x = 4.2f; npcPos.y = 3.00f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.OT);
                                ////H1 Position
                                npcPos.x = -4.2f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H1);
                                ////H2 Position
                                npcPos.x = 4.2f; npcPos.y = -3.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.H2);
                                ////M1 Position
                                npcPos.x = -4.2f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M1);
                                ////M2 Position
                                npcPos.x = 4.2f; npcPos.y = -1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.M2);
                                ////R1 Position
                                npcPos.x = -4.2f; npcPos.y = 1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R1);
                                ////R2 Position
                                npcPos.x = 4.2f; npcPos.y = 1.0f;
                                nm.MoveNpc(npcPos, npcRot, npcTime, CharacterInfo.RolePositions.R2);
                                #endregion
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #endregion

                }
                else if (currentFightInfo.CurrentStep >= 10)
                {
                    ResetFight();
                }
            }
            else if (currentFightInfo.CurrentAttack == "Witch Hunt")
            {
                if (currentFightInfo.CurrentStep == 1)
                {

                }
            }
            else if (currentFightInfo.CurrentAttack == "Electrope Edge 1")
            {
                if (currentFightInfo.CurrentStep == 6)
                {
                    int RandomDir = Random.Range(1, 2);
                    float dir;
                    if (RandomDir == 1) { dir = 0; } else { dir = 180; }
                    //Move Boss To Center
                    bm.MoveBoss(new Vector3(0f, 0f, zp), dir, .25f);
                    //Change boss to base form
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
            }
        }
        #endregion
    }

    // Called every frame during the middle of each step
    private void ExecuteMiddleOfStep()
    {
        //Debug.Log("Executing Middle of Step");
        // Add custom behavior for the middle of each step
        #region // Check for hits
        PlayerHitManager phm = playerHitManager;
        // Check if player got hit
        phm.CheckIfPlayerIsHit();
        #endregion


        #region//M4S
        //Goes from x: -5 to 5, 
        if (currentFightInfo.CurrentFight == FightEnum.M4S)
        {
            if (currentFightInfo.CurrentAttack == "Bewitching Flight")
            {
                if (currentFightInfo.CurrentStep == 4)
                {
                    //Show pass or fail after Electro mines
                    phm.ShowPassOrFail();

                }
                else if (currentFightInfo.CurrentStep == 7)
                {
                    //Show pass or fail after Electro mines
                    phm.ShowPassOrFail();

                }
            }
        }
        #endregion
    }

    // Called before proceeding to the next step, but not on the first step
    private void ExecuteEndOfStep()
    {
        if (GetIsDebugging()) 
        { 
            Debug.Log("Executing End of Step");
        }
        #region // Player hit detection
        PlayerHitManager phm = playerHitManager;
        // Check if player was hit
        phm.CheckIfPlayerWasHit();
        // Reset Check for hits
        phm.ResetCheckedThisStep();
        #endregion


        BossManager bm = bossManager;
        EffectsManager em = effects;
        PlayerPositionManager ppm = playerPositionManager;
        CurrentFightInfo cfi = currentFightInfo;
        DamageMarkerManager dm = damageManager;
        float zp = transform.position.z;
        //Specific fight action


    }
    public CurrentFightInfo GetFightInfo() { return currentFightInfo; } 
    public FightEnum GetCurrentFight() => currentFightInfo.CurrentFight;
    public string GetCurrentAttack() => currentFightInfo.CurrentAttack;
    public int GetAttackIndex() => currentFightInfo.CurrentAttackIndex;
    public int GetCurrentStep() => currentFightInfo.CurrentStep;

    public Vector3 GetPlayerPosition()
    {
        //Initalize a player position var
        Vector3 playerPos = new Vector3(0, 0, 0);
        //Find player transform position
        if (playerManager != null)
        {
            playerPos = playerManager.transform.position;
        }
        else { Debug.LogWarning("Unable to find player position due to not finding playerManager"); }
        //Return player position
        return playerPos;
    }
    public Vector3 GetBossPosition()
    {
        //Initalize a boss position var
        Vector3 bossPos = new Vector3(0, 0, 0);
        //Find boss transform position
        if (bossManager != null)
        {
            bossPos = bossManager.transform.position;
        }
        else { Debug.LogWarning("Unable to find boss position due to not finding bossManager"); }
        //Return boss position
        return bossPos;
    }
    #endregion
    #region // Debugging
    private void SetIsDebugging(bool debugging) { isDebugging = debugging; }
    private bool GetIsDebugging() { return isDebugging; }
    #endregion
}
#endregion