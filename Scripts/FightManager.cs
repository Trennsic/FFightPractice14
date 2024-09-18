using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EffectsManager;
using UnityEngine.UIElements;
using static FightManager;
using static BossManager;

[System.Serializable]
public class CurrentFightInfo
{
    [SerializeField] private FightEnum currentFight;
    [SerializeField] private string currentAttack;
    [SerializeField] private int currentStep;
    [SerializeField] private int currentAttackIndex;


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
}
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
   
    [SerializeField] public CurrentFightInfo currentFightInfo;
    #region // References
    public GameObject fightTextGameObject;
    public GameObject MarkerManager;
    private Waymarks waymarks;
    public GameObject Boss;
    public GameObject EffectsManager;
    public GameObject PPositionManager;
    public GameObject PlayerManager;
    private EffectsManager effects;
    private BossManager bossManager;
    private PlayerPositionManager playerPositionManager;
    private PlayerManager playerManager;

    private TMP_Text fightText;
    #endregion

    private Dictionary<FightEnum, List<string>> fightAttacks = new Dictionary<FightEnum, List<string>>();
    private Dictionary<string, int> attackSteps = new Dictionary<string, int>();
    private Dictionary<FightEnum, List<List<bool>>> fightAwaitingSteps = new Dictionary<FightEnum, List<List<bool>>>();

    private bool isStartOfFight = true;

    [SerializeField] private float zPosition = 0f;
    #endregion
    #region // Functions
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
            if (fightText == null) { Debug.LogError(" Unable to find waymarks component"); }
        }
        else { Debug.LogError(" Unable to find MarkerManager"); }
        if (Boss != null)
        {
            bossManager = Boss.GetComponent<BossManager>();
            if (fightText == null) { Debug.LogError(" Unable to find bossManager component"); }
        } else { Debug.LogError(" Unable to find Boss"); }
        if (EffectsManager != null)
        {
            effects = EffectsManager.GetComponent<EffectsManager>();
            if (fightText == null) { Debug.LogError(" Unable to find effects component"); }
        } else { Debug.LogError(" Unable to find EffectsManager"); }
        if (PPositionManager != null)
        {
            playerPositionManager = PPositionManager.GetComponent<PlayerPositionManager>();
            if (fightText == null) { Debug.LogError(" Unable to find playerPositionManager component"); }
        } else { Debug.LogError(" Unable to find PPositionManager"); }        
        if (PlayerManager != null)
        {
            playerManager = PlayerManager.GetComponent<PlayerManager>();
            if (fightText == null) { Debug.LogError(" Unable to find playerManager component"); }
        } else { Debug.LogError(" Unable to find PlayerManager"); }
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
        attackSteps["Bewitching Flight"] = 7;
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
            Debug.Log("Fight Completed");
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
            Debug.Log($"Executing step {currentFightInfo.CurrentStep + 1} of attack: {fightAttacks[currentFightInfo.CurrentFight][currentFightInfo.CurrentAttackIndex]}");
            UpdateFightText();
        }
        //Update fight attack string
        UpdateFightAttackString();

        //
        UpdateFightActions();

        // Execute StartOfStep when moving to the new step
        ExecuteStartOfStep();

        // Once we�ve processed the start of fight, mark it as false so EndOfStep is called afterward
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
        PlayerInfo.Jobs whichJob = PlayerInfo.Jobs.WHM;
        PlayerInfo.RolePositions whichRolePos = PlayerInfo.RolePositions.R1;
        // the grid will always be 1, 2, 3, 4, or 5 prefabs wide
        int role = Random.Range(0, 8);
        switch (role)
        {
            case 0: whichRolePos = PlayerInfo.RolePositions.MT; break;
            case 1: whichRolePos = PlayerInfo.RolePositions.OT; break;
            case 2: whichRolePos = PlayerInfo.RolePositions.H1; break;
            case 3: whichRolePos = PlayerInfo.RolePositions.H2; break;
            case 4: whichRolePos = PlayerInfo.RolePositions.M1; break;
            case 5: whichRolePos = PlayerInfo.RolePositions.M2; break;
            case 6: whichRolePos = PlayerInfo.RolePositions.R1; break;
            default : whichRolePos = PlayerInfo.RolePositions.R2; break;
        }

        int job = 0;
        //Tanks
        if (whichRolePos == PlayerInfo.RolePositions.MT || whichRolePos == PlayerInfo.RolePositions.OT)
        {
            job = Random.Range(0, 4);
            switch (role)
            {
                case 0: whichJob = PlayerInfo.Jobs.PLD; break;
                case 1: whichJob = PlayerInfo.Jobs.WAR; break;
                case 2: whichJob = PlayerInfo.Jobs.DRK; break;
                default: whichJob = PlayerInfo.Jobs.GNB; break;
            }
        }
        //Heals
        if (whichRolePos == PlayerInfo.RolePositions.H1 || whichRolePos == PlayerInfo.RolePositions.H2)
        {
            job = Random.Range(0,4);
            switch (role)
            {
                case 0: whichJob  = PlayerInfo.Jobs.WHM; break;
                case 1: whichJob  = PlayerInfo.Jobs.SCH; break;
                case 2: whichJob  = PlayerInfo.Jobs.AST; break;
                default: whichJob = PlayerInfo.Jobs.SGE; break;
            }
        }
        //M Dps
        if (whichRolePos == PlayerInfo.RolePositions.M1 || whichRolePos == PlayerInfo.RolePositions.M2)
        {
            job = Random.Range(0, 6);
            switch (role)
            {
                case 0: whichJob = whichJob  = PlayerInfo.Jobs.MNK; break;
                case 1: whichJob = whichJob  = PlayerInfo.Jobs.DRG; break;
                case 2: whichJob = whichJob  = PlayerInfo.Jobs.NIN; break;
                case 3: whichJob = whichJob  = PlayerInfo.Jobs.SAM; break;
                case 4: whichJob = whichJob  = PlayerInfo.Jobs.RPR; break;
                default: whichJob = whichJob  = PlayerInfo.Jobs.VIP; break;
            }
        }
        //R Dps
        if (whichRolePos == PlayerInfo.RolePositions.R1 || whichRolePos == PlayerInfo.RolePositions.R2)
        {
            job = Random.Range(0, 7);
            switch (role)
            {
                case 0: whichJob = whichJob = PlayerInfo.Jobs.BRD; break;
                case 1: whichJob = whichJob  = PlayerInfo.Jobs.MCH; break;
                case 2: whichJob = whichJob  = PlayerInfo.Jobs.DNC; break;
                case 3: whichJob = whichJob  = PlayerInfo.Jobs.PIC; break;
                case 4: whichJob = whichJob  = PlayerInfo.Jobs.BLM; break;
                case 5: whichJob = whichJob  = PlayerInfo.Jobs.SMN; break;
                default: whichJob = whichJob  = PlayerInfo.Jobs.RDM; break;
            }
        }


        if (playerManager != null)
        {
            playerManager.SetupPlayer(whichJob, whichRolePos);
        }
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
        Debug.Log("Executing Start of Step");
        // Add custom behavior for start of step
        // Get the current fight, attack, and step from FightManager
        Debug.Log("Updating fight actions");
        Debug.Log($"Current Fight: {currentFightInfo.CurrentFight}");
        Debug.Log($"Current Attack: {currentFightInfo.CurrentAttack}");
        Debug.Log($"Current Step: {currentFightInfo.CurrentStep}");



        if (bossManager == null) { Debug.LogError("BossManager isn't found"); return; }
        BossManager bm = bossManager;
        EffectsManager em = effects;
        PlayerPositionManager ppm = playerPositionManager;
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
                    //Move Boss To Center
                    bm.MoveBoss(new Vector3(0f, 0f, zp), 180, 0f);
                    //Change boss to base form
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
                // Move North
                else if (currentFightInfo.CurrentStep == 1)
                {
                    bm.MoveBoss(new Vector3(0f, 4.8f, zp), 0, 1f);
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Wings_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
                // Start Lasters
                else if (currentFightInfo.CurrentStep == 2)
                {
                    bm.MoveBoss(new Vector3(0f, 5.1f, zp), 0, 0f);
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Wings_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale, 1f, .8f, 0f, .2f);
                    //Spawn lasers
                    em.CreateFX(Effects.WickedThunderBeam1, FxLifeTimeType.Step, new Vector3(-.2f, 4.85f, -1f), 0, 3);
                    em.CreateFX(Effects.WickedThunderBeam2, FxLifeTimeType.Step, new Vector3(-1.8f, 4.9f, -.4f), 0, 3);
                    em.CreateFX(Effects.WickedThunderBeam3, FxLifeTimeType.Step, new Vector3(-2.5f, 5.2f, -.4f), 0, 3);
                    em.CreateFX(Effects.WickedThunderBeam4, FxLifeTimeType.Step, new Vector3(1.2f, 4.9f, -.4f), 0, 3);
                    em.CreateFX(Effects.WickedThunderBeam5, FxLifeTimeType.Step, new Vector3(2f, 5.2f, -.4f), 0, 3);

                    int ran = 1;
                    if (ran == 0)
                    {
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, 2.5f, -.4f), 0, 3);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, .5f, -.4f), 0, 3);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, -1.5f, -.4f), 0, 3);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, -3.5f, -.4f), 0, 3);
                    }
                    else if (ran == 1)
                    {
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, 3.6f, -.4f), 0, 3);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, 1.6f, -.4f), 0, 3);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, -.4f, -.4f), 0, 3);
                        em.CreateFX(Effects.WickedThunderElectromine, FxLifeTimeType.Step, new Vector3(-5f, -2.4f, -.4f), 0, 3);
                    }

                    //Setup guide points
                    Debug.Log("Sending Debug Log stuff over");
                    ppm.SetupGuidePoints();
                }
                //Set Lines - 3
                // Move Mid
                else if (currentFightInfo.CurrentStep == 3)
                {
                    bm.MoveBoss(new Vector3(0f, 0f, zp), 180, 1f);
                    //Change boss to base form
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
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
    }

    // Called before proceeding to the next step, but not on the first step
    private void ExecuteEndOfStep()
    {
        Debug.Log("Executing End of Step");
        // Add custom behavior for the end of step
    }

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
}