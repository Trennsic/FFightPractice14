using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FightManager : MonoBehaviour
{
    public enum FightEnum
    {
        M1N, M2N, M3N, M4N,
        M1S, M2S, M3S, M4S,
        Titan_Story, Titan_Hard, Titan_Extreme,
    }

    public FightEnum currentFight;
    public GameObject fightTextGameObject; // Reference to the GameObject in the Inspector
    public GameObject MarkerManager; // Reference to the GameObject in the Inspector
    private Waymarks waymarks; // Reference to the GameObject in the Inspector
    public GameObject Boss; // Reference to the GameObject in the Inspector
    private BossManager bossManager; // Reference to the GameObject in the Inspector
    private TMP_Text fightText;  // Internal reference to the TextMeshPro component

    private Dictionary<FightEnum, List<string>> fightAttacks = new Dictionary<FightEnum, List<string>>();
    private Dictionary<string, int> attackSteps = new Dictionary<string, int>();
    private Dictionary<FightEnum, List<List<bool>>> fightAwaitingSteps = new Dictionary<FightEnum, List<List<bool>>>(); // Stores isAwaiting for each step of each attack

    private int currentAttackIndex = 0;
    private int currentStep = 0;

    void Start()
    {
        // Search for the TextMeshPro component on the assigned GameObject
        if (fightTextGameObject != null)
        {
            fightText = fightTextGameObject.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogError("No GameObject assigned for fightTextGameObject.");
        }
        if (MarkerManager != null)
        {
            waymarks = MarkerManager.GetComponent<Waymarks>();
        }
        if (Boss != null)
        {
            bossManager = Boss.GetComponent<BossManager>();
        }


        InitializeFights();
        StartFight();  // Starts with the default fight (M4S) unless another is passed later
    }

    void Update()
    {
        // Check if the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcessNextStep();
        }
        // Check if Left Control + R is pressed for resetting the fight
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            ResetFight();
        }
    }

    // Initialize fight data, attack steps, and isAwaiting steps
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
        attackSteps["Bewitching Flight"]                    = 7;
        attackSteps["Witch Hunt"]                           = 8;
        attackSteps["Electrope Edge 1"]                     = 8;
        attackSteps["Electrope Edge 2 (Lightning Cage)"]    = 15; 
        attackSteps["Ion Cluster (Ion Cannon)"]             = 16; 
        attackSteps["Electrope Transplant"]                 = 32;
        attackSteps["Cross Tail Switch"]                    = 3;
        attackSteps["Twilight Sabbath"]                     = 3;
        attackSteps["Midnight Sabbath"]                     = 3;
        attackSteps["Raining Swords (Chain Lightning)"]     = 3;
        attackSteps["Sunrise Sabbath"]                      = 3;
        attackSteps["Sword Quiver"]                         = 3;

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

    // Start the selected fight with an optional parameter, default to M4S
    public void StartFight(FightEnum selectedFight = FightEnum.M4S)
    {
        currentFight = selectedFight;  // Set the current fight to the selected fight
        currentAttackIndex = 0;
        currentStep = 0;
        SetBoss();              // Update Boss
        SetWaymarks();          // Update waymarks
        UpdateFightActions();   //Update inital fight action
        UpdateFightText();      // Update the text with the initial attack and step
    }

    // Function to advance to the next step of the current attack
    public void ProcessNextStep()
    {
        if (currentAttackIndex >= fightAttacks[currentFight].Count)
        {
            Debug.Log("Fight Completed");
            if (fightText != null)
            {
                fightText.text = "Fight Completed";
            }
            return;
        }

        string currentAttack = fightAttacks[currentFight][currentAttackIndex];
        int steps = attackSteps[currentAttack];

        if (currentStep < steps - 1)
        {
            currentStep++;  // Move to the next step if there are more steps in the current attack
        }
        else
        {
            currentStep = 0;  // Reset steps for the next attack
            currentAttackIndex++;  // Move to the next attack
        }

        if (currentAttackIndex < fightAttacks[currentFight].Count)
        {
            Debug.Log($"Executing step {currentStep + 1} of attack: {fightAttacks[currentFight][currentAttackIndex]}");
            UpdateFightText();  // Update text after each step or attack change
        }
        //Update fight actions
        UpdateFightActions();
    }

    // Function to get if the current step of the current attack is awaiting input
    public bool GetIsCurrentStepAwaiting()
    {
        if (currentAttackIndex < fightAwaitingSteps[currentFight].Count && currentStep < fightAwaitingSteps[currentFight][currentAttackIndex].Count)
        {
            return fightAwaitingSteps[currentFight][currentAttackIndex][currentStep];
        }

        return false;  // Return false if out of bounds
    }

    // Update TextMeshPro with the current fight, attack, and step
    void UpdateFightText()
    {
        if (fightText != null && fightAttacks[currentFight].Count > currentAttackIndex)
        {
            string currentAttack = fightAttacks[currentFight][currentAttackIndex];
            int totalSteps = attackSteps[currentAttack];
            fightText.text = $"Fight: {currentFight}\nAttack: {currentAttack}\nStep: {currentStep + 1} of {totalSteps}";
        }
    }
    // Function to get the current fight
    public FightEnum GetCurrentFight()
    {
        return currentFight;
    }
    // Function to get the current attack (returns the name of the current attack)
    public string GetCurrentAttack()
    {
        if (currentAttackIndex < fightAttacks[currentFight].Count)
        {
            return fightAttacks[currentFight][currentAttackIndex];
        }
        return null; // Return null if no valid attack exists
    }
    // Function to get the current step (returns the current step number starting from 1)
    public int GetCurrentStep()
    {
        return currentStep + 1; // Returning 1-based step instead of 0-based
    }
    private void SetBoss()
    {
        //Default start
        BossManager.Bosses whichBoss = BossManager.Bosses.Wicked_Thunder;

        if (bossManager != null)
        {
            // M4S
            whichBoss = BossManager.Bosses.Wicked_Thunder;
            if (currentFight == FightEnum.M4S) { bossManager.SetupBoss(whichBoss); }
        }
    }
    private void SetWaymarks()
    {
        //Default start
        Waymarks.WaymarkSets whichWaymarks = Waymarks.WaymarkSets.M4S_Hector;

        if (waymarks != null)
        {
            //M4S
            if (currentFight == FightEnum.M4S) { waymarks.SetWaymarkUsingSets(whichWaymarks); }
            
        }

    }

    //This is run everytime the step is changed in a fight
    private void UpdateFightActions()
    {
        // Get the current fight, attack, and step from FightManager
        FightManager.FightEnum currentFight = GetCurrentFight();
        string currentAttack = GetCurrentAttack();
        int currentStep = GetCurrentStep();
        Debug.Log("Updating fight actions");
        Debug.Log($"Current Fight: {currentFight}");
        Debug.Log($"Current Attack: {currentAttack}");
        Debug.Log($"Current Step: {currentStep}");



        if (bossManager == null) { Debug.LogError("BossManager isn't found"); return; }
        BossManager bm = bossManager;
        float zp = transform.position.z;
        //Specific fight action

        #region//M4S
        //Goes from x: -5 to 5, 
        if (currentFight == FightEnum.M4S)
        {
            if (currentAttack == "Bewitching Flight")
            {
                if (currentStep == 1)
                {
                    //Move Boss To Center
                    bm.MoveBoss(new Vector3(0f, 0f, zp), 180, 0f);
                    //Change boss to base form
                    WickedThunderSettings wts = bm.wickedThunderSettings;
                    bm.UpdateBossSprite(wts.Wicked_Thunder_Base_Image, wts.Wicked_Thunder_Xscale, wts.Wicked_Thunder_Yscale);
                }
                // Move North
                else if (currentStep == 2)
                {
                    bm.MoveBoss( new Vector3( 0f, 4f, zp), 0, 1f);
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
            else if (currentAttack == "Witch Hunt")
            {
                if (currentStep == 1)
                {

                }
            }
            else if (currentAttack == "Electrope Edge 1")
            {
                if (currentStep == 6)
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
    #region // Reset Fight
    // Function to reset the current fight
    public void ResetFight()
    {
        Debug.Log("Resetting the fight.");
        StartFight(currentFight);  // Restart the current fight
    }
    #endregion

}












