using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EffectsManager;
using UnityEngine.UIElements;
using static FightManager;
using static BossManager;
using System.Collections;
using System;
using static GameManager;

[Serializable]
public struct PermutationOption
{
    public string label; // e.g. "Flip A Side", "3 Snap - Left First"
    public int value; // int representation of permutation

    public PermutationOption(string label, int value)
    {
        this.label = label;
        this.value = value;
    }
}

#region // Fight Manager
public class FightManager : MonoBehaviour
{
    #region // Definitions
    public enum FightEnum
    {
        M1N, M2N, M3N, M4N,
        M1S, M2S, M3S, M4S,
        M5S, M6S, M7S, M8S,
        Titan_Story, Titan_Hard, Titan_Extreme,
    }
    public enum FightGuide { Hector, Custom }
    public enum StepType { Wait    , Place    }
    [SerializeField] private FightEnum currentFight;
    [SerializeField] private FightGuide currentGuide;
    [SerializeField] private int currentAttack;
    [SerializeField] private int currentStep;
    [SerializeField] private StepType currentStepType;
    [SerializeField] private int permutation;
    #region // References 
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    #endregion

    [SerializeField] private DebugInfo debug;

    #region // M5S
    public enum M5SAttacks
    {
        // Setup
        Setup                   = 0,
        // Introduction          
        Deep_Cut_1              = 1,
        Flip_AB_1               = 2,
        Snap_Twist_1            = 3,
        Celebrate_1             = 4,
        // Disco Infernal        
        Disco_Infernal_1        = 5,
        Funky_Floor_1           = 6,
        Out_In_1                = 7,
        Flip_AB_2               = 8,
        Snap_Twist_2            = 9,
        Celebrate_2             = 10,
        Deep_Cut_2              = 11,
        // Ensemble_Assemble     
        Ensemble_Assemble_1     = 12,
        Arcady_1                = 13,
        Lets_Dance_1            = 14,
        Lets_Pose_1             = 15,
        // Ride_the_Waves        
        Flip_AB_3               = 16,
        Ride_the_Waves_1        = 17,
        Quarter_Eight_Beats_1   = 18,
        Quarter_Eight_Beats_2   = 19,
        Out_In_2                = 20,
        Snap_Twist_3            = 21,
        Deep_Cut_3              = 22,
        Celebrate_3             = 23,
        // Frogtourage_1         
        Frogtourage_1           = 24,
        Quarter_Eight_Beats_3   = 25,
        // Disc_Infernal_2       
        Disc_Infernal_2         = 26,
        Flip_AB_4               = 27,
        Snap_Twist_4            = 28,
        Celebrate_4             = 29,
        // Ensemble_Assemble_2   
        Ensemble_Assemble_2     = 30,
        Arcady_2                = 31,
        Lets_Dance_2            = 32,
        Lets_Pose_2             = 33,
        // Frogtourage_2         
        Frogtourage_2           = 34,
        Do_the_Hustle_1         = 35,
        FrogBaits_1             = 36,
        Do_the_Hustle_2         = 37,
        Deep_Cut_4              = 38,
        // Funky_Floor_2         
        Funky_Floor_2           = 39,
        Quarter_Eight_Beats_4   = 40,
        Out_In_3                = 41,
        Quarter_Eight_Beats_5   = 42,
        Celebrate_5             = 43,
        Celebrate_6             = 44,
        // Enrage                
        Enrage                  = 45,
    }
    private Dictionary<M5SAttacks, int> m5sAttackMaxSteps;
    private Dictionary<M5SAttacks, List<PermutationOption>> m5sAttackPermutations;
    private Dictionary<M5SAttacks, PermutationOption> m5sSelectedPermutations;


    #endregion

    #endregion
    #region // Functions

    public void InitializeFightManager()
    {

    }

    public void InitializeFightInfo(FightEnum fightEnum, int fightAttack = 0, int fightStep = 0, FightGuide fightGuide = FightGuide.Hector)
    {
        InitializeDebug();
        SetCurrentFight(fightEnum);
        SetCurrentAttack(fightAttack);
        SetCurrentStep(fightStep);
        SetCurrentGuide(fightGuide);

        if (GetCurrentFight() == FightEnum.M5S)
        {
            InitializeM5S();
        }
        
    }

    public string GetAttackNameByInt(FightEnum fightEnum, int attackIndex)
    {
        switch (fightEnum)
        {
            case FightEnum.M5S:
                if (Enum.IsDefined(typeof(M5SAttacks), attackIndex))
            {
                // Return the name of the enum as a string
                return Enum.GetName(typeof(M5SAttacks), attackIndex);
            }
            else
            {
                throw new ArgumentException($"Invalid attack index: {attackIndex}");
                }

            // Add more cases if you want to support other fights (e.g. M6S, Titan, etc.)

            default:
                throw new ArgumentException($"Unsupported fight type: {fightEnum}");
        }
    }

    public int GetAttackIndexFromEnum(FightEnum fightEnum, Enum attackEnum)
    {
        switch (fightEnum)
        {
            case FightEnum.M5S:
                if (attackEnum is M5SAttacks m5sAttack)
                {
                    return (int)m5sAttack;
                }
                else
                {
                    throw new ArgumentException($"Provided attack enum is not a valid M5SAttacks value: {attackEnum}");
                }

            // Add more cases if you want to support other fights (e.g. M6S, Titan, etc.)

            default:
                throw new ArgumentException($"Unsupported fight type: {fightEnum}");
        }
    }


    public void ProgressFightStep()
    {
        if (currentFight == FightEnum.M5S)
        {
            M5SAttacks currentM5SAttack = (M5SAttacks)currentAttack;

            if (!m5sAttackMaxSteps.ContainsKey(currentM5SAttack))
            {
                Debug.LogWarning($"Attack {currentM5SAttack} not found in max steps dictionary.");
                return;
            }

            int maxSteps = m5sAttackMaxSteps[currentM5SAttack];

            if (currentStep + 1 >= maxSteps)
            {
                // Move to next attack and reset step
                currentAttack++;
                currentStep = 0;

                // Optional: Clamp if you're at the final attack
                if (!Enum.IsDefined(typeof(M5SAttacks), currentAttack))
                {
                    Debug.Log("Reached end of attack list.");
                    currentAttack = Enum.GetValues(typeof(M5SAttacks)).Length - 1;
                    currentStep = m5sAttackMaxSteps[(M5SAttacks)currentAttack] - 1;
                }
            }
            else
            {
                // Just increment step
                currentStep++;
            }

            DisplayFightInformation(); // Optional: for debug info
        }
        else
        {
            // Future: Add handling for other fight types
            Debug.LogWarning("ProgressFightStep not implemented for this fight type.");
        }
    }

    public void DisplayFightInformation()
    {
        GetGameManager().GetDebugManager().AddDebug("Fight: ", GetCurrentFight().ToString(), 5f);
        GetGameManager().GetDebugManager().AddDebug("Attack: ", GetAttackNameByInt(GetCurrentFight(),GetCurrentAttack()), 5f);
        GetGameManager().GetDebugManager().AddDebug("Step: ", GetCurrentStep().ToString(), 5f);
    }
    public void SetCurrentFight(FightEnum newFight) { currentFight = newFight; }
    public void SetCurrentAttack(int newAttack) { currentAttack = newAttack; }
    public void SetCurrentStep(int newStep) { currentStep = newStep; }
    public void SetCurrentGuide(FightGuide newFightGuide) => currentGuide = newFightGuide;
    public void SetCurrentStepType(StepType newStepType) => currentStepType = newStepType;
    public FightEnum GetCurrentFight() => currentFight; 
    public int GetCurrentAttack() => currentAttack; 
    public int GetCurrentStep() => currentStep; 
    public FightGuide GetCurrentGuide() => currentGuide; 
    public StepType GetCurrentStepType() => currentStepType;     
    
    public void GotoNextAttack() { currentAttack++; }
    public void GotoNextStep() { currentStep++; }
    public void ResetStepIndex() { currentStep = 0; }
    #region // M5S
    private void InitializeM5S()
    {
        // Set up the max steps for the M5S fight
        m5sAttackMaxSteps = new Dictionary<M5SAttacks, int>(){
            // Setup
            {M5SAttacks.Setup                  , 5},// 5 Steps - Start Position, Pick Clock, LP Position, LP Resolve / Color Position, Color Resolve
            // Introduction
            {M5SAttacks.Deep_Cut_1             , 3}, // 3 Steps - Start Position, DC Position, DC Resolve
            {M5SAttacks.Flip_AB_1              , 1}, // 1 Step  - AB Wait
            {M5SAttacks.Snap_Twist_1           , 3}, // 3 Steps - Snap Position, Snap Resolve / Position, Twist Resolve
            {M5SAttacks.Celebrate_1            , 1}, // 1 Step  - Raidwide Wait
            // Disco Infernal
            {M5SAttacks.Disco_Infernal_1       , 1}, // 1 Step  - Clock Position Wait
            {M5SAttacks.Funky_Floor_1          , 3}, // 3 Steps - Floor 1 Position, Floor 1 Resolve / Floor 2 Position, Floor 2 Resolve
            {M5SAttacks.Out_In_1               , 3}, // 3 Steps - Oi 1 Position, Oi 1 Resolve / Oi 2 Position, Oi 1 Resolve
            {M5SAttacks.Flip_AB_2              , 5}, // 3 Steps - Floor 3 Position, Floor 3 Resolve / Spotlight Position, Spotlight Resolve
            {M5SAttacks.Snap_Twist_2           , 4}, // 3 Steps - Snap Position, Snap Resolve / Position, Twist Resolve
            {M5SAttacks.Celebrate_2            , 3},
            {M5SAttacks.Deep_Cut_2             , 3},
            // Ensemble_Assemble
            {M5SAttacks.Ensemble_Assemble_1    , 3},
            {M5SAttacks.Arcady_1               , 3},
            {M5SAttacks.Lets_Dance_1           , 3},
            {M5SAttacks.Lets_Pose_1            , 3},
            // Ride_the_Waves
            {M5SAttacks.Flip_AB_3              , 3},
            {M5SAttacks.Ride_the_Waves_1       , 3},
            {M5SAttacks.Quarter_Eight_Beats_1  , 3},
            {M5SAttacks.Quarter_Eight_Beats_2  , 3},
            {M5SAttacks.Out_In_2               , 3},
            {M5SAttacks.Snap_Twist_3           , 3},
            {M5SAttacks.Deep_Cut_3             , 3},
            {M5SAttacks.Celebrate_3            , 3},
            // Frogtourage_1
            {M5SAttacks.Frogtourage_1          , 3},
            {M5SAttacks.Quarter_Eight_Beats_3  , 3},
            // Disc_Infernal_2
            {M5SAttacks.Disc_Infernal_2        , 3},
            {M5SAttacks.Flip_AB_4              , 3},
            {M5SAttacks.Snap_Twist_4           , 3},
            {M5SAttacks.Celebrate_4            , 3},
            // Ensemble_Assemble_2
            {M5SAttacks.Ensemble_Assemble_2    , 3},
            {M5SAttacks.Arcady_2               , 3},
            {M5SAttacks.Lets_Dance_2           , 3},
            {M5SAttacks.Lets_Pose_2            , 3},
            // Frogtourage_2
            {M5SAttacks.Frogtourage_2          , 3},
            {M5SAttacks.Do_the_Hustle_1        , 3},
            {M5SAttacks.FrogBaits_1            , 3},
            {M5SAttacks.Do_the_Hustle_2        , 3},
            {M5SAttacks.Deep_Cut_4             , 3},
            // Funky_Floor_2
            {M5SAttacks.Funky_Floor_2          , 3},
            {M5SAttacks.Quarter_Eight_Beats_4  , 3},
            {M5SAttacks.Out_In_3               , 3},
            {M5SAttacks.Quarter_Eight_Beats_5  , 3},
            {M5SAttacks.Celebrate_5            , 3},
            {M5SAttacks.Celebrate_6            , 3},
            // Enrage
            {M5SAttacks.Enrage                 , 3},


        };
        // When Initializing M5S use test mapping for Permutations
        // Example test mapping: Funky_Floor_1 = 0, Snap_Twist_2 = 0, Flip_AB_2 = 1.
        //var testMapping = new Dictionary<M5SAttacks, int>
        //{
        //    { M5SAttacks.Funky_Floor_1, 1 },
        //    { M5SAttacks.Snap_Twist_2, 1 },
        //    { M5SAttacks.Flip_AB_2, 0 }
        //};
        //// Use Test mapping for Random Generator
        //GetGameManager().SetRandomGenerator(new TestRandomGenerator(testMapping));


        InitializeM5SPermutations();
    }
    private void InitializeM5SPermutations()
    {
        m5sAttackPermutations = new Dictionary<M5SAttacks, List<PermutationOption>>();
        m5sSelectedPermutations = new Dictionary<M5SAttacks, PermutationOption>();

        // Get Generator
        IRandomGenerator randomGenerator = GetGameManager().GetRandomGenerator();

        // Define permutations
        #region // Introduction
        m5sAttackPermutations[M5SAttacks.Flip_AB_1] = new List<PermutationOption>()
        {
            new PermutationOption("Flip to A Side", 1),
            new PermutationOption("Flip to B Side", 2),
        };

        m5sAttackPermutations[M5SAttacks.Snap_Twist_1] = new List<PermutationOption>()
        {
            new PermutationOption("2 Snap - Left First", 1),
            new PermutationOption("3 Snap - Left First", 2),
            new PermutationOption("4 Snap - Left First", 3),
            new PermutationOption("2 Snap - Right First", 4),
            new PermutationOption("3 Snap - Right First", 5),
            new PermutationOption("4 Snap - Right First", 6),
        };
        #endregion
        #region // Disco Infernal 1
        m5sAttackPermutations[M5SAttacks.Disco_Infernal_1] = new List<PermutationOption>()
        {
            new PermutationOption("Supports First"    , 1), // Supp short timers
            new PermutationOption("Damage First"      , 2), // Dps short timers
        };
        m5sAttackPermutations[M5SAttacks.Funky_Floor_1] = new List<PermutationOption>()
        {
            new PermutationOption("NW Tile Safe"    , 1),
            new PermutationOption("NW Tile Unsafe"  , 2),
        };
        m5sAttackPermutations[M5SAttacks.Out_In_1] = new List<PermutationOption>()
        {
            new PermutationOption("Out first"   , 1),
            new PermutationOption("In first"    , 2),
        };
        m5sAttackPermutations[M5SAttacks.Flip_AB_2] = new List<PermutationOption>()
        {
            new PermutationOption("Flip to A Side", 1), // Roles
            new PermutationOption("Flip to B Side", 2), // Light Parties
        };
        m5sAttackPermutations[M5SAttacks.Snap_Twist_2] = new List<PermutationOption>()
        {
            new PermutationOption("2 Snap - Left First", 1),
            new PermutationOption("3 Snap - Left First", 2),
            new PermutationOption("4 Snap - Left First", 3),
            new PermutationOption("2 Snap - Right First", 4),
            new PermutationOption("3 Snap - Right First", 5),
            new PermutationOption("4 Snap - Right First", 6),
        };
        #endregion 

        // If no random generator was injected, create one using the default implementation.
        if (randomGenerator == null)
        {
            randomGenerator = new DefaultRandomGenerator(GetGameManager().GetSeedValue());
        }


        if (debug.GetIsDebugging()) Debug.Log("[FightManager] Initializing permutations using injected random generator...");

        // Use the attack-specific random generation logic.
        foreach (var kvp in m5sAttackPermutations)
        {
            var attack = kvp.Key;
            var permutations = kvp.Value;

            if (permutations.Count > 0)
            {
                int index = randomGenerator.Next(attack, permutations.Count);
                m5sSelectedPermutations[attack] = permutations[index];
                if (debug.GetIsDebugging()) Debug.Log($"[FightManager] Attack {attack} selected permutation: {permutations[index].label} (value {permutations[index].value}) at index {index} out of {permutations.Count}");
            }
            else
            {
                Debug.LogWarning($"[FightManager] No permutations available for attack {attack}.");
            }
        }
    }
    public void SetPermutation(M5SAttacks attack, PermutationOption option)
    {
        if (m5sAttackPermutations.ContainsKey(attack) &&
            m5sAttackPermutations[attack].Contains(option))
        {
            m5sSelectedPermutations[attack] = option;
        }
        else
        {
            Debug.LogWarning($"Option {option.label} is not valid for {attack}.");
        }
    }

    public PermutationOption GetPermutationForAttack(M5SAttacks attack)
    {
        if (m5sSelectedPermutations.TryGetValue(attack, out var permutation))
        {
            return permutation;
        }

        return new PermutationOption("Default", 0);
    }


    #endregion
    #region // References 
    private GameManager GetGameManager() => gameManager;
    private void InitializeDebug(bool isDebugging = false) { debug = new DebugInfo(isDebugging); }
    #endregion
    #endregion
}
#endregion