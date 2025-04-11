using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;

    [SerializeField] private Dictionary<CharacterManager.RolePositions, Vector3> npcPosition;


    // Define a delegate that matches the signature of the function
    private Func<float, float, Vector3> ArenaPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void InitializePositionManager()
    {
        npcPosition = new Dictionary<CharacterManager.RolePositions, Vector3>() { } ;
        // Assign the GameManager method to the delegate
        ArenaPosition = GetGameManager().GetArenaPositionFromPercentage;
        InitializeDebug(true);
    }
    public void UpdatePositions()
    {
        #region// Get Fight info
        FightManager fm = GetGameManager().GetFightManager();
        FightManager.FightEnum whichFight = fm.GetCurrentFight();
        FightManager.FightGuide whichGuide = fm.GetCurrentGuide();
        int whichAttack = fm.GetCurrentAttack();
        int whichStep = fm.GetCurrentStep();
        
        #endregion
        #region// Set NPC positions
        // Clear positions and setup default values
        npcPosition.Clear();
        bool skipPlacement = false;
        Vector3 mtPos = Vector3.zero; Vector3 otPos = Vector3.zero;
        Vector3 h1Pos = Vector3.zero; Vector3 h2Pos = Vector3.zero;
        Vector3 m1Pos = Vector3.zero; Vector3 m2Pos = Vector3.zero;
        Vector3 r1Pos = Vector3.zero; Vector3 r2Pos = Vector3.zero;
        // M5S
        if (whichFight == FightManager.FightEnum.M5S)
        {
            string m5sAttack = GetGameManager().GetFightManager().GetAttackNameByInt(whichFight, whichAttack);
            // 
            if (whichGuide == FightManager.FightGuide.Hector)
            {
                #region// Setup
                if (m5sAttack == FightManager.M5SAttacks.Setup.ToString())
                {
                    // Inital Clock Positions
                    if (whichStep == 0)
                    {
                        // Setup Role Positions
                        mtPos = ArenaPosition(50f, 32f);
                        otPos = ArenaPosition(50f, 14f);
                        h1Pos = ArenaPosition(40f, 23f);
                        h2Pos = ArenaPosition(60f, 23f);
                        m1Pos = ArenaPosition(40f, 14f);
                        m2Pos = ArenaPosition(60f, 14f);
                        r1Pos = ArenaPosition(40f, 32f);
                        r2Pos = ArenaPosition(60f, 32f);
                    }
                    // Light Parties
                    else if (whichStep == 1)
                    {
                        // Setup Role Positions
                        mtPos = ArenaPosition(35f, 65f);
                        h1Pos = ArenaPosition(29f, 71f);
                        m1Pos = ArenaPosition(35f, 71f);
                        r1Pos = ArenaPosition(29f, 65f);

                        otPos = ArenaPosition(65f, 35f);
                        h2Pos = ArenaPosition(71f, 29f);
                        m2Pos = ArenaPosition(65f, 29f);
                        r2Pos = ArenaPosition(71f, 35f);
                    }
                    // Color Partners
                    else if (whichStep == 2)
                    {
                        // Setup Role Positions
                        mtPos = ArenaPosition(40f, 65f);
                        r1Pos = ArenaPosition(35f, 59f);

                        h1Pos = ArenaPosition(40f, 35f);
                        m1Pos = ArenaPosition(35f, 41f);
                        
                        otPos = ArenaPosition(60f, 65f);
                        r2Pos = ArenaPosition(65f, 59f);

                        h2Pos = ArenaPosition(60f, 35f);
                        m2Pos = ArenaPosition(65f, 41f);
                        
                    }

                }
                #endregion
                #region // Deep Cut 1
                else if (m5sAttack == FightManager.M5SAttacks.Deep_Cut_1.ToString())
                {
                    // Inital Clock Positions
                    if (whichStep == 0)
                    {
                        // Fight Start Loose Stacks
                        mtPos = ArenaPosition(50f, 35f);
                        otPos = ArenaPosition(62f, 32f);
                        h1Pos = ArenaPosition(35f, 23f);
                        h2Pos = ArenaPosition(58f, 25f);
                        m1Pos = ArenaPosition(46f, 26f);
                        m2Pos = ArenaPosition(50f, 18f);
                        r1Pos = ArenaPosition(40f, 16f);
                        r2Pos = ArenaPosition(60f, 14f);
                    }
                    // Deep Cut Choose Pos
                    else if (whichStep == 1)
                    {
                        // Setup Role Positions
                        mtPos = ArenaPosition(50f, 65f);
                        otPos = ArenaPosition(65f, 55f);

                        h1Pos = ArenaPosition(46f, 42f);
                        h2Pos = ArenaPosition(54f, 42f);

                        m1Pos = ArenaPosition(43f, 35f);
                        m2Pos = ArenaPosition(57f, 35f);

                        r1Pos = ArenaPosition(45f, 28f);
                        r2Pos = ArenaPosition(55f, 28f);
                    }
                    // Deep Cut Resolve
                    else if (whichStep == 2)
                    {
                        // Setup Role Positions
                        mtPos = ArenaPosition(50f, 65f);
                        otPos = ArenaPosition(65f, 55f);

                        h1Pos = ArenaPosition(46f, 42f);
                        h2Pos = ArenaPosition(54f, 42f);

                        m1Pos = ArenaPosition(43f, 35f);
                        m2Pos = ArenaPosition(57f, 35f);

                        r1Pos = ArenaPosition(45f, 28f);
                        r2Pos = ArenaPosition(55f, 28f);

                    }

                }
                #endregion
                #region // Flip_AB_1
                else if (m5sAttack == FightManager.M5SAttacks.Flip_AB_1.ToString())
                {
                    #region // Get Permutations
                    var permAB = fm.GetPermutationForAttack(FightManager.M5SAttacks.Flip_AB_1);
                    var permSnap = fm.GetPermutationForAttack(FightManager.M5SAttacks.Snap_Twist_1);
                    Debug.Log($"Current Attack Permutation: {permAB.label},");
                    Debug.Log($"Current Attack Permutation: {permSnap.label}");
                    #endregion

                    // Fight Start Loose Stacks
                    mtPos = ArenaPosition(50f, 65f);
                    otPos = ArenaPosition(50f, 35f);
                    h1Pos = ArenaPosition(35f, 50f);
                    h2Pos = ArenaPosition(65f, 50f);
                    m1Pos = ArenaPosition(35f, 65f);
                    m2Pos = ArenaPosition(65f, 65f);
                    r1Pos = ArenaPosition(35f, 35f);
                    r2Pos = ArenaPosition(65f, 35f);

                }
                #endregion
                #region // Snap_Twist_1
                else if (m5sAttack == FightManager.M5SAttacks.Snap_Twist_1.ToString())
                {
                    #region // Get Permutations
                    var permAB = fm.GetPermutationForAttack(FightManager.M5SAttacks.Flip_AB_1);
                    var permSnap = fm.GetPermutationForAttack(FightManager.M5SAttacks.Snap_Twist_1);
                    Debug.Log($"Current Attack Permutation: {permAB.label},");
                    Debug.Log($"Current Attack Permutation: {permSnap.label}");
                    #endregion
                    #region // Start position for mechanic
                    if (whichStep == 0)
                    {
                        mtPos = ArenaPosition(50f, 65f);
                        otPos = ArenaPosition(50f, 35f);
                        h1Pos = ArenaPosition(35f, 50f);
                        h2Pos = ArenaPosition(65f, 50f);
                        m1Pos = ArenaPosition(35f, 65f);
                        m2Pos = ArenaPosition(65f, 65f);
                        r1Pos = ArenaPosition(35f, 35f);
                        r2Pos = ArenaPosition(65f, 35f);
                    }
                    #endregion
                    #region// Flip to A Side - (Roles)
                    if (permAB.value == 1)
                    {
                        #region// Left First snap
                        if (permSnap.value == 1 || permSnap.value == 2 || permSnap.value == 3)
                        {
                            // Resolve avoid snap + roles then setup for twist avoid + roles
                            if (whichStep == 1 || whichStep == 2)
                            {
                                mtPos = ArenaPosition(53f, 62f);
                                otPos = ArenaPosition(58f, 59f);
                                h1Pos = ArenaPosition(63f, 48f);
                                h2Pos = ArenaPosition(67f, 52f);
                                m1Pos = ArenaPosition(52f, 38f);
                                m2Pos = ArenaPosition(58f, 40f);
                                r1Pos = ArenaPosition(58f, 34f);
                                r2Pos = ArenaPosition(52f, 32f);
                            }
                            // Twist Resolve
                            else if (whichStep == 3)
                            {
                                mtPos = ArenaPosition(47f, 62f);
                                otPos = ArenaPosition(42f, 59f);
                                h1Pos = ArenaPosition(37f, 48f);
                                h2Pos = ArenaPosition(33f, 52f);
                                m1Pos = ArenaPosition(48f, 38f);
                                m2Pos = ArenaPosition(42f, 40f);
                                r1Pos = ArenaPosition(42f, 34f);
                                r2Pos = ArenaPosition(48f, 32f);
                            }
                        }
                        #endregion
                        #region// Right First snap
                        else if (permSnap.value == 4 || permSnap.value == 5 || permSnap.value == 6)
                        {
                            // Resolve avoid snap + roles then setup for twist avoid + roles
                            if (whichStep == 1 || whichStep == 2)
                            {
                                mtPos = ArenaPosition(47f, 62f);
                                otPos = ArenaPosition(42f, 59f);
                                h1Pos = ArenaPosition(37f, 48f);
                                h2Pos = ArenaPosition(33f, 52f);
                                m1Pos = ArenaPosition(48f, 38f);
                                m2Pos = ArenaPosition(42f, 40f);
                                r1Pos = ArenaPosition(42f, 34f);
                                r2Pos = ArenaPosition(48f, 32f);
                            }
                            // Twist Resolve
                            else if (whichStep == 3)
                            {
                                mtPos = ArenaPosition(53f, 62f);
                                otPos = ArenaPosition(58f, 59f);
                                h1Pos = ArenaPosition(63f, 48f);
                                h2Pos = ArenaPosition(67f, 52f);
                                m1Pos = ArenaPosition(52f, 38f);
                                m2Pos = ArenaPosition(58f, 40f);
                                r1Pos = ArenaPosition(58f, 34f);
                                r2Pos = ArenaPosition(52f, 32f);
                            }

                        }
                        #endregion
                    }
                    #endregion
                    #region// Flip to B Side - (Light Party)
                    else if (permAB.value == 2)
                    {
                        #region// Left First snap
                        if (permSnap.value == 1 || permSnap.value == 2 || permSnap.value == 3)
                        {
                            // Resolve avoid snap + LPs then setup for twist avoid + roles
                            if (whichStep == 1 || whichStep == 2)
                            {
                                mtPos = ArenaPosition(52f, 62f);
                                h1Pos = ArenaPosition(58f, 66f);
                                m1Pos = ArenaPosition(58f, 60f);
                                r1Pos = ArenaPosition(52f, 68f);
                                otPos = ArenaPosition(52f, 38f);
                                h2Pos = ArenaPosition(58f, 34f);
                                m2Pos = ArenaPosition(58f, 40f);
                                r2Pos = ArenaPosition(52f, 32f);
                            }
                            // Twist Resolve
                            else if (whichStep == 3)
                            {
                                mtPos = ArenaPosition(48f, 62f);
                                h1Pos = ArenaPosition(42f, 66f);
                                m1Pos = ArenaPosition(42f, 60f);
                                r1Pos = ArenaPosition(48f, 68f);
                                otPos = ArenaPosition(48f, 38f);
                                h2Pos = ArenaPosition(42f, 34f);
                                m2Pos = ArenaPosition(42f, 40f);
                                r2Pos = ArenaPosition(48f, 32f);
                            }
                        }
                        #endregion
                        #region// Right First snap
                        else if (permSnap.value == 4 || permSnap.value == 5 || permSnap.value == 6)
                        {
                            // Resolve avoid snap + LPs then setup for twist avoid + roles
                            if (whichStep == 1 || whichStep == 2)
                            {
                                mtPos = ArenaPosition(48f, 62f);
                                h1Pos = ArenaPosition(42f, 66f);
                                m1Pos = ArenaPosition(42f, 60f);
                                r1Pos = ArenaPosition(48f, 68f);
                                otPos = ArenaPosition(48f, 38f);
                                h2Pos = ArenaPosition(42f, 34f);
                                m2Pos = ArenaPosition(42f, 40f);
                                r2Pos = ArenaPosition(48f, 32f);
                                
                            }
                            // Twist Resolve
                            else if (whichStep == 3)
                            {
                                mtPos = ArenaPosition(52f, 62f);
                                h1Pos = ArenaPosition(58f, 66f);
                                m1Pos = ArenaPosition(58f, 60f);
                                r1Pos = ArenaPosition(52f, 68f);
                                otPos = ArenaPosition(52f, 38f);
                                h2Pos = ArenaPosition(58f, 34f);
                                m2Pos = ArenaPosition(58f, 40f);
                                r2Pos = ArenaPosition(52f, 32f);
                            }

                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
                #region // Celebrate_1
                else if (m5sAttack == FightManager.M5SAttacks.Celebrate_1.ToString())
                {
                    // Clock around boss
                    mtPos = ArenaPosition(50f, 65f);
                    otPos = ArenaPosition(50f, 35f);
                    h1Pos = ArenaPosition(35f, 50f);
                    h2Pos = ArenaPosition(65f, 50f);
                    m1Pos = ArenaPosition(35f, 65f);
                    m2Pos = ArenaPosition(65f, 65f);
                    r1Pos = ArenaPosition(35f, 35f);
                    r2Pos = ArenaPosition(65f, 35f);

                }
                #endregion
                #region // Disco_infernal
                else if (m5sAttack == FightManager.M5SAttacks.Disco_Infernal_1.ToString())
                {
                    #region // Start position for mechanic
                    if (whichStep == 0)
                    {
                        // Clock positions
                        mtPos = ArenaPosition(50f, 65f);
                        otPos = ArenaPosition(50f, 35f);
                        h1Pos = ArenaPosition(35f, 50f);
                        h2Pos = ArenaPosition(65f, 50f);
                        m1Pos = ArenaPosition(35f, 65f);
                        m2Pos = ArenaPosition(65f, 65f);
                        r1Pos = ArenaPosition(35f, 35f);
                        r2Pos = ArenaPosition(65f, 35f);
                    }
                    #endregion
                }
                #endregion
                #region // Funky_Floor_1
                else if (m5sAttack == FightManager.M5SAttacks.Funky_Floor_1.ToString())
                {
                    #region // Get Permutation
                    var permFunky_1 = fm.GetPermutationForAttack(FightManager.M5SAttacks.Funky_Floor_1);
                    Debug.Log($"{m5sAttack} Permutation: {permFunky_1.label}");
                    #endregion
                    #region // Start position for mechanic
                    if (whichStep == 0)
                    {
                        skipPlacement = true;
                        // G1 West G2 East
                        //mtPos = ArenaPosition(50f, 65f);
                        //h1Pos = ArenaPosition(35f, 55f);
                        //m1Pos = ArenaPosition(40f, 35f);
                        //r1Pos = ArenaPosition(35f, 45f);
                        //
                        //otPos = ArenaPosition(50f, 35f);
                        //h2Pos = ArenaPosition(65f, 55f);
                        //m2Pos = ArenaPosition(60f, 35f);
                        //r2Pos = ArenaPosition(65f, 45f);

                    }
                    #endregion
                    #region// NW Safe
                    if (permFunky_1.value == 1)
                    {
                        // Funky Floor resolve 1 -> Position for 2nd
                        if (whichStep == 1 || whichStep == 2)
                        {
                            mtPos = ArenaPosition(55f, 65f);
                            h1Pos = ArenaPosition(25f, 55f);
                            m1Pos = ArenaPosition(45f, 35f);
                            r1Pos = ArenaPosition(35f, 45f);

                            otPos = ArenaPosition(65f, 35f);
                            h2Pos = ArenaPosition(65f, 55f);
                            m2Pos = ArenaPosition(55f, 45f);
                            r2Pos = ArenaPosition(75f, 45f);
                        }
                        // Funky Floor Resolve 2
                        else if (whichStep == 3)
                        {
                            mtPos = ArenaPosition(45f, 65f);
                            m1Pos = ArenaPosition(45f, 45f);

                            h1Pos = ArenaPosition(35f, 55f);
                            r1Pos = ArenaPosition(35f, 35f);

                            otPos = ArenaPosition(65f, 45f);
                            m2Pos = ArenaPosition(55f, 35f);

                            h2Pos = ArenaPosition(75f, 55f);
                            r2Pos = ArenaPosition(75f, 35f);
                        }
                    }
                    #endregion
                    #region// NW Unsafe
                    else if(permFunky_1.value == 2)
                    {
                        // Funky Floor resolve 1 -> Position for 2nd
                        if (whichStep == 1 || whichStep == 2)
                        {
                            mtPos = ArenaPosition(45f, 65f);
                            m1Pos = ArenaPosition(45f, 45f);

                            h1Pos = ArenaPosition(35f, 55f);
                            r1Pos = ArenaPosition(35f, 35f);

                            otPos = ArenaPosition(65f, 45f);
                            m2Pos = ArenaPosition(55f, 35f);

                            h2Pos = ArenaPosition(75f, 55f);
                            r2Pos = ArenaPosition(75f, 35f);

                            
                        }
                        // Funky Floor Resolve 2
                        else if (whichStep == 3)
                        {
                            mtPos = ArenaPosition(55f, 65f);
                            h1Pos = ArenaPosition(25f, 55f);
                            m1Pos = ArenaPosition(45f, 35f);
                            r1Pos = ArenaPosition(35f, 45f);

                            otPos = ArenaPosition(65f, 35f);
                            h2Pos = ArenaPosition(65f, 55f);
                            m2Pos = ArenaPosition(55f, 45f);
                            r2Pos = ArenaPosition(75f, 45f);
                        }
                    }
                    #endregion
                }
                #endregion
                #region // Out_In_1
                else if (m5sAttack == FightManager.M5SAttacks.Out_In_1.ToString())
                {
                    #region // Get Permutation
                    var permFunky_1 = fm.GetPermutationForAttack(FightManager.M5SAttacks.Funky_Floor_1);
                    var permOutIn_1 = fm.GetPermutationForAttack(FightManager.M5SAttacks.Out_In_1);
                    Debug.Log($"{m5sAttack} Permutation: {permFunky_1.label}");
                    Debug.Log($"{m5sAttack} Permutation: {permOutIn_1.label}");
                    #endregion
                    #region // Start position for mechanic
                    if (whichStep == 0)
                    {
                        skipPlacement = true;
                        // G1 West G2 East
                        //mtPos = ArenaPosition(50f, 65f);
                        //h1Pos = ArenaPosition(35f, 55f);
                        //m1Pos = ArenaPosition(40f, 35f);
                        //r1Pos = ArenaPosition(35f, 45f);
                        //
                        //otPos = ArenaPosition(50f, 35f);
                        //h2Pos = ArenaPosition(65f, 55f);
                        //m2Pos = ArenaPosition(60f, 35f);
                        //r2Pos = ArenaPosition(65f, 45f);
                    }
                    #endregion
                    #region// NW Safe
                    // NW tile safe
                    if (permFunky_1.value == 1)
                    {
                        // Outside first
                        if (permOutIn_1.value == 1)
                        {
                            // Dodge Out
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(53f, 63f);
                                h1Pos = ArenaPosition(33f, 43f);
                                m1Pos = ArenaPosition(42f, 37f);
                                r1Pos = ArenaPosition(37f, 47f);

                                otPos = ArenaPosition(67f, 53f);
                                h2Pos = ArenaPosition(57f, 67f);
                                m2Pos = ArenaPosition(47f, 33f);
                                r2Pos = ArenaPosition(63f, 57f);
                            }
                            // Dodge In
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(53f, 57f);
                                h2Pos = ArenaPosition(57f, 56f);
                                r2Pos = ArenaPosition(53f, 54f);
                                otPos = ArenaPosition(57f, 53f);


                                h1Pos = ArenaPosition(43f, 43f);
                                m1Pos = ArenaPosition(43f, 47f);
                                r1Pos = ArenaPosition(47f, 47f);
                                m2Pos = ArenaPosition(47f, 43f);
                                
                            }
                        }
                        // In First
                        else if(permOutIn_1.value == 2)
                        {
                            // Dodge In
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(53f, 57f);
                                h2Pos = ArenaPosition(57f, 56f);
                                r2Pos = ArenaPosition(53f, 54f);
                                otPos = ArenaPosition(57f, 53f);


                                h1Pos = ArenaPosition(43f, 43f);
                                m1Pos = ArenaPosition(43f, 47f);
                                r1Pos = ArenaPosition(47f, 47f);
                                m2Pos = ArenaPosition(47f, 43f);

                            }
                            // Dodge Out
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(53f, 63f);
                                h1Pos = ArenaPosition(33f, 43f);
                                m1Pos = ArenaPosition(42f, 37f);
                                r1Pos = ArenaPosition(37f, 47f);

                                otPos = ArenaPosition(67f, 53f);
                                h2Pos = ArenaPosition(57f, 67f);
                                m2Pos = ArenaPosition(47f, 33f);
                                r2Pos = ArenaPosition(63f, 57f);
                            }
                            
                        }

                    }
                    #endregion
                    #region// NW Unsafe
                    else if (permFunky_1.value == 2)
                    {
                        // Outside first
                        if (permOutIn_1.value == 1)
                        {
                            // Dodge Out
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(43f, 63f);
                                h2Pos = ArenaPosition(47f, 67f);

                                h1Pos = ArenaPosition(33f, 53f);
                                r1Pos = ArenaPosition(37f, 57f);

                                m1Pos = ArenaPosition(52f, 37f);
                                m2Pos = ArenaPosition(57f, 33f);

                                otPos = ArenaPosition(67f, 43f);
                                r2Pos = ArenaPosition(63f, 47f);


                            }
                            // Dodge In
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(43f, 57f);
                                h2Pos = ArenaPosition(47f, 56f);

                                r2Pos = ArenaPosition(53f, 44f);
                                otPos = ArenaPosition(57f, 43f);


                                h1Pos = ArenaPosition(43f, 53);
                                m1Pos = ArenaPosition(47f, 54f);

                                r1Pos = ArenaPosition(57f, 47f);
                                m2Pos = ArenaPosition(57f, 43f);

                            }
                        }
                        // In First
                        else if (permOutIn_1.value == 2)
                        {
                            // Dodge In
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(43f, 57f);
                                h2Pos = ArenaPosition(47f, 56f);

                                r2Pos = ArenaPosition(53f, 44f);
                                otPos = ArenaPosition(57f, 43f);


                                h1Pos = ArenaPosition(43f, 53);
                                m1Pos = ArenaPosition(47f, 54f);

                                r1Pos = ArenaPosition(57f, 47f);
                                m2Pos = ArenaPosition(57f, 43f);

                            }
                            // Dodge Out
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(43f, 63f);
                                h2Pos = ArenaPosition(47f, 67f);

                                h1Pos = ArenaPosition(33f, 53f);
                                r1Pos = ArenaPosition(37f, 57f);

                                m1Pos = ArenaPosition(52f, 37f);
                                m2Pos = ArenaPosition(57f, 33f);

                                otPos = ArenaPosition(67f, 43f);
                                r2Pos = ArenaPosition(63f, 47f);


                            }
                            

                        }
                    }
                    #endregion
                }
                #endregion
                #region // Flip_AB_2
                else if (m5sAttack == FightManager.M5SAttacks.Flip_AB_2.ToString())
                {
                    #region // Get Permutation
                    var permFunky_1 = fm.GetPermutationForAttack(FightManager.M5SAttacks.Funky_Floor_1);
                    var permDiTimers_1 = fm.GetPermutationForAttack(FightManager.M5SAttacks.Disco_Infernal_1);
                    Debug.Log($"{m5sAttack} Permutation: {permFunky_1.label}");
                    Debug.Log($"{m5sAttack} Permutation: {permDiTimers_1.label}");
                    #endregion
                    #region // Start position for mechanic
                    if (whichStep == 0)
                    {
                        skipPlacement = true;
                    }
                    #endregion
                    #region// Supports First
                    // Supports Pop First
                    if (permDiTimers_1.value == 1)
                    {
                        #region// NW tile Safe
                        if (permFunky_1.value == 1)
                        {
                            // Supports in spotlights
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(35f, 55f);
                                h1Pos = ArenaPosition(25f, 25f);

                                otPos = ArenaPosition(65f, 45f);
                                h2Pos = ArenaPosition(75f, 75f);

                                m1Pos = ArenaPosition(45f, 65f);
                                r1Pos = ArenaPosition(35f, 35f);
                                m2Pos = ArenaPosition(55f, 35f);
                                r2Pos = ArenaPosition(65f, 65f);
                            }
                            // Dodge Funky Floor
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(35f, 65f);
                                m1Pos = ArenaPosition(45f, 55f);

                                h1Pos = ArenaPosition(35f, 25f);
                                r1Pos = ArenaPosition(25f, 35f);

                                otPos = ArenaPosition(65f, 35f);
                                m2Pos = ArenaPosition(55f, 45f);

                                h2Pos = ArenaPosition(65f, 75f);
                                r2Pos = ArenaPosition(75f, 65f);
                            }
                            // Damage in spotlights
                            else if (whichStep == 3)
                            {
                                mtPos = ArenaPosition(45f, 65f);
                                m1Pos = ArenaPosition(35f, 55f);

                                h1Pos = ArenaPosition(35f, 35f);
                                r1Pos = ArenaPosition(25f, 25f);

                                otPos = ArenaPosition(55f, 35f);
                                m2Pos = ArenaPosition(65f, 45f);

                                h2Pos = ArenaPosition(65f, 65f);
                                r2Pos = ArenaPosition(75f, 75f);

                            }
                            // Dodge Funky Floor again
                            else if (whichStep == 4)
                            {
                                mtPos = ArenaPosition(45f, 55f);
                                m1Pos = ArenaPosition(35f, 65f);

                                h1Pos = ArenaPosition(25f, 35f);
                                r1Pos = ArenaPosition(35f, 25f);

                                otPos = ArenaPosition(55f, 45f);
                                m2Pos = ArenaPosition(65f, 35f);

                                h2Pos = ArenaPosition(75f, 65f);
                                r2Pos = ArenaPosition(65f, 75f);
                            }

                        }
                        #endregion
                        #region// NW tile UNsafe
                        else if (permFunky_1.value == 2)
                        {
                            
                            // Supports in spotlights
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(35f, 45f);
                                m1Pos = ArenaPosition(45f, 35f);

                                h1Pos = ArenaPosition(25f, 75f);
                                r1Pos = ArenaPosition(35f, 65f);

                                otPos = ArenaPosition(65f, 55f);
                                m2Pos = ArenaPosition(55f, 65f);

                                h2Pos = ArenaPosition(75f, 25f);
                                r2Pos = ArenaPosition(65f, 35f);
                            }
                            // Dodge Funky Floor
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(35f, 35f);
                                m1Pos = ArenaPosition(45f, 45f);

                                h1Pos = ArenaPosition(35f, 75f);
                                r1Pos = ArenaPosition(25f, 65f);

                                otPos = ArenaPosition(65f, 65f);
                                m2Pos = ArenaPosition(55f, 55f);

                                h2Pos = ArenaPosition(65f, 25f);
                                r2Pos = ArenaPosition(75f, 35f);
                            }
                            // Damage in spotlights
                            else if (whichStep == 3)
                            {
                                mtPos = ArenaPosition(45f, 35f);
                                m1Pos = ArenaPosition(35f, 45f);

                                h1Pos = ArenaPosition(35f, 75f);
                                r1Pos = ArenaPosition(25f, 65f);

                                otPos = ArenaPosition(55f, 65f);
                                m2Pos = ArenaPosition(65f, 55f);

                                h2Pos = ArenaPosition(65f, 35f);
                                r2Pos = ArenaPosition(75f, 25f);

                            }
                            // Dodge Funky Floor again
                            else if (whichStep == 4)
                            {
                                mtPos = ArenaPosition(45f, 45f);
                                m1Pos = ArenaPosition(35f, 35f);

                                h1Pos = ArenaPosition(25f, 65f);
                                r1Pos = ArenaPosition(35f, 75f);

                                otPos = ArenaPosition(55f, 55f);
                                m2Pos = ArenaPosition(65f, 65f);

                                h2Pos = ArenaPosition(75f, 35f);
                                r2Pos = ArenaPosition(65f, 25f);
                            }

                        }
                        #endregion

                    }
                    #endregion
                    #region// Damage First
                    // Damage Pop First
                    else if (permDiTimers_1.value == 2)
                    {
                       
                        #region// NW tile Safe
                        if (permFunky_1.value == 1)
                        {
                            // Damage in spotlights
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(45f, 65f);
                                m1Pos = ArenaPosition(35f, 55f);

                                h1Pos = ArenaPosition(35f, 35f);
                                r1Pos = ArenaPosition(25f, 25f);

                                otPos = ArenaPosition(55f, 35f);
                                m2Pos = ArenaPosition(65f, 45f);

                                h2Pos = ArenaPosition(65f, 65f);
                                r2Pos = ArenaPosition(75f, 75f);
                            }
                            // Dodge Funky Floor
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(45f, 55f);
                                m1Pos = ArenaPosition(35f, 65f);

                                h1Pos = ArenaPosition(25f, 35f);
                                r1Pos = ArenaPosition(35f, 25f);

                                otPos = ArenaPosition(55f, 45f);
                                m2Pos = ArenaPosition(65f, 35f);

                                h2Pos = ArenaPosition(75f, 65f);
                                r2Pos = ArenaPosition(65f, 75f);
                            }
                            // Supports in spotlights
                            else if (whichStep == 3)
                            {
                                mtPos = ArenaPosition(35f, 55f);
                                m1Pos = ArenaPosition(45f, 65f);

                                h1Pos = ArenaPosition(25f, 25f);
                                r1Pos = ArenaPosition(35f, 35f);

                                otPos = ArenaPosition(65f, 45f);
                                m2Pos = ArenaPosition(55f, 35f);

                                h2Pos = ArenaPosition(75f, 75f);
                                r2Pos = ArenaPosition(65f, 65f);

                            }
                            // Dodge Funky Floor again
                            else if (whichStep == 4)
                            {
                                mtPos = ArenaPosition(35f, 65f);
                                m1Pos = ArenaPosition(45f, 55f);

                                h1Pos = ArenaPosition(35f, 25f);
                                r1Pos = ArenaPosition(25f, 35f);

                                otPos = ArenaPosition(65f, 35f);
                                m2Pos = ArenaPosition(55f, 45f);

                                h2Pos = ArenaPosition(65f, 75f);
                                r2Pos = ArenaPosition(75f, 65f);
                            }

                        }
                        #endregion
                        #region// NW tile UNsafe
                        else if (permFunky_1.value == 2)
                        {
                            // Damage in spotlights
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(35f, 45f);
                                m1Pos = ArenaPosition(45f, 35f);

                                h1Pos = ArenaPosition(35f, 65f);
                                r1Pos = ArenaPosition(25f, 75f);

                                otPos = ArenaPosition(65f, 55f);
                                m2Pos = ArenaPosition(55f, 65f);

                                h2Pos = ArenaPosition(65f, 35f);
                                r2Pos = ArenaPosition(75f, 25f);
                            }
                            // Dodge Funky Floor
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(45f, 45f);
                                m1Pos = ArenaPosition(35f, 35f);

                                h1Pos = ArenaPosition(25f, 65f);
                                r1Pos = ArenaPosition(35f, 75f);

                                otPos = ArenaPosition(55f, 55f);
                                m2Pos = ArenaPosition(65f, 65f);

                                h2Pos = ArenaPosition(75f, 35f);
                                r2Pos = ArenaPosition(65f, 25f);
                            }
                            // Supports in spotlights
                            else if (whichStep == 3)
                            {
                                mtPos = ArenaPosition(45f, 35f);
                                m1Pos = ArenaPosition(35f, 45f);

                                h1Pos = ArenaPosition(25f, 65f);
                                r1Pos = ArenaPosition(35f, 75f);

                                otPos = ArenaPosition(55f, 65f);
                                m2Pos = ArenaPosition(65f, 55f);

                                h2Pos = ArenaPosition(75f, 25f);
                                r2Pos = ArenaPosition(65f, 35f);

                            }
                            // Dodge Funky Floor again
                            else if (whichStep == 4)
                            {
                                mtPos = ArenaPosition(35f, 35f);
                                m1Pos = ArenaPosition(45f, 45f);

                                h1Pos = ArenaPosition(35f, 75f);
                                r1Pos = ArenaPosition(25f, 65f);

                                otPos = ArenaPosition(65f, 65f);
                                m2Pos = ArenaPosition(55f, 55f);

                                h2Pos = ArenaPosition(65f, 25f);
                                r2Pos = ArenaPosition(75f, 35f);
                            }

                        }
                        #endregion

                    }
                    #endregion
                }
                #endregion
                #region // Snap_Twist_2
                else if (m5sAttack == FightManager.M5SAttacks.Snap_Twist_2.ToString())
                {
                    #region // Get Relavent Permutations
                    var permFunky_1 = fm.GetPermutationForAttack(FightManager.M5SAttacks.Funky_Floor_1);
                    var permFlipAB_2 = fm.GetPermutationForAttack(FightManager.M5SAttacks.Flip_AB_2);
                    var permSnapTwist_2 = fm.GetPermutationForAttack(FightManager.M5SAttacks.Snap_Twist_2);
                    if (debug.GetIsDebugging())
                    {
                        Debug.Log($"{m5sAttack} Permutation: {permFunky_1.label}");
                        Debug.Log($"{m5sAttack} Permutation: {permFlipAB_2.label}");
                        Debug.Log($"{m5sAttack} Permutation: {permSnapTwist_2.label}");
                    }
                    #endregion
                    #region // Start position for mechanic
                    if (whichStep == 0)
                    {
                        skipPlacement = true;
                    }
                    #endregion
                    #region // NW Safe
                    if (permFunky_1.value == 1)
                    {
                        #region // Snap Left First
                        if (permSnapTwist_2.value >= 1 && permSnapTwist_2.value <= 3)
                        {
                            // Dodge funky/snap and get close to boss
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(53f, 47f);
                                m1Pos = ArenaPosition(57f, 43f);

                                h1Pos = ArenaPosition(53f, 27f);
                                r1Pos = ArenaPosition(57f, 23f);

                                otPos = ArenaPosition(53f, 67f);
                                m2Pos = ArenaPosition(57f, 63f);

                                h2Pos = ArenaPosition(53f, 87f);
                                r2Pos = ArenaPosition(57f, 83f);
                            }
                            // Dodge funky 2 wait snap 
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(53f, 37f);
                                m1Pos = ArenaPosition(57f, 33f);

                                h1Pos = ArenaPosition(53f, 17f);
                                r1Pos = ArenaPosition(57f, 13f);

                                otPos = ArenaPosition(53f, 57f);
                                m2Pos = ArenaPosition(57f, 53f);

                                h2Pos = ArenaPosition(53f, 77f);
                                r2Pos = ArenaPosition(57f, 73f);
                            }
                            // Dodge Twist & Flip placement
                            else if (whichStep == 3)
                            {
                                // Flip A - Roles
                                if (permFlipAB_2.value == 1) 
                                {
                                    mtPos = ArenaPosition(47f, 63f);
                                    otPos = ArenaPosition(43f, 67f);

                                    h1Pos = ArenaPosition(35f, 53f);
                                    h2Pos = ArenaPosition(35f, 47f);

                                    m1Pos = ArenaPosition(47f, 37f);
                                    m2Pos = ArenaPosition(43f, 37f);
                                    r1Pos = ArenaPosition(47f, 33f);
                                    r2Pos = ArenaPosition(43f, 33f);
                                }
                                // Flip B - Light Parties
                                else if (permFlipAB_2.value == 2)
                                {
                                    mtPos = ArenaPosition(47f, 63f);
                                    h1Pos = ArenaPosition(43f, 63f);
                                    m1Pos = ArenaPosition(47f, 67f);
                                    r1Pos = ArenaPosition(43f, 67f);

                                    otPos = ArenaPosition(47f, 37f);
                                    h2Pos = ArenaPosition(43f, 37f);
                                    m2Pos = ArenaPosition(47f, 33f);
                                    r2Pos = ArenaPosition(43f, 33f);
                                }
                            }

                        }
                        #endregion
                        #region // Snap Right First
                        else if (permSnapTwist_2.value >= 4 && permSnapTwist_2.value <= 6)
                        {
                            // Dodge funky/snap and get close to boss
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(43f, 47f);
                                m1Pos = ArenaPosition(47f, 43f);

                                h1Pos = ArenaPosition(43f, 27f);
                                r1Pos = ArenaPosition(47f, 23f);

                                otPos = ArenaPosition(43f, 67f);
                                m2Pos = ArenaPosition(47f, 63f);

                                h2Pos = ArenaPosition(43f, 87f);
                                r2Pos = ArenaPosition(47f, 83f);
                            }
                            // Dodge funky 2 wait snap 
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(43f, 37f);
                                m1Pos = ArenaPosition(47f, 33f);

                                h1Pos = ArenaPosition(43f, 17f);
                                r1Pos = ArenaPosition(47f, 13f);

                                otPos = ArenaPosition(43f, 57f);
                                m2Pos = ArenaPosition(47f, 53f);

                                h2Pos = ArenaPosition(43f, 77f);
                                r2Pos = ArenaPosition(47f, 73f);
                            }
                            // Dodge Twist & Flip placement
                            else if (whichStep == 3)
                            {
                                // Flip A - Roles
                                if (permFlipAB_2.value == 1)
                                {
                                    mtPos = ArenaPosition(57f, 63f);
                                    otPos = ArenaPosition(53f, 67f);

                                    h1Pos = ArenaPosition(65f, 53f);
                                    h2Pos = ArenaPosition(65f, 47f);

                                    m1Pos = ArenaPosition(57f, 37f);
                                    m2Pos = ArenaPosition(53f, 37f);
                                    r1Pos = ArenaPosition(57f, 33f);
                                    r2Pos = ArenaPosition(53f, 33f);
                                }
                                // Flip B - Light Parties
                                else if (permFlipAB_2.value == 2)
                                {
                                    mtPos = ArenaPosition(57f, 63f);
                                    h1Pos = ArenaPosition(53f, 63f);
                                    m1Pos = ArenaPosition(57f, 67f);
                                    r1Pos = ArenaPosition(53f, 67f);

                                    otPos = ArenaPosition(57f, 37f);
                                    h2Pos = ArenaPosition(53f, 37f);
                                    m2Pos = ArenaPosition(57f, 33f);
                                    r2Pos = ArenaPosition(53f, 33f);
                                }
                            }

                        }
                        #endregion
                    }
                    #endregion
                    #region // NW Unsafe
                    else if (permFunky_1.value == 2)
                    {
                        #region // Snap Left First
                        if (permSnapTwist_2.value >= 1 && permSnapTwist_2.value <= 3)
                        {
                            // Dodge funky/snap and get close to boss
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(53f, 37f);
                                m1Pos = ArenaPosition(57f, 33f);

                                h1Pos = ArenaPosition(53f, 17f);
                                r1Pos = ArenaPosition(57f, 13f);

                                otPos = ArenaPosition(53f, 57f);
                                m2Pos = ArenaPosition(57f, 53f);

                                h2Pos = ArenaPosition(53f, 77f);
                                r2Pos = ArenaPosition(57f, 73f);
                            }
                            // Dodge funky 2 wait snap 
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(53f, 47f);
                                m1Pos = ArenaPosition(57f, 43f);

                                h1Pos = ArenaPosition(53f, 27f);
                                r1Pos = ArenaPosition(57f, 23f);

                                otPos = ArenaPosition(53f, 67f);
                                m2Pos = ArenaPosition(57f, 63f);

                                h2Pos = ArenaPosition(53f, 87f);
                                r2Pos = ArenaPosition(57f, 83f);
                            }
                            // Dodge Twist & Flip placement
                            else if (whichStep == 3)
                            {
                                // Flip A - Roles
                                if (permFlipAB_2.value == 1)
                                {
                                    mtPos = ArenaPosition(47f, 63f);
                                    otPos = ArenaPosition(43f, 67f);

                                    h1Pos = ArenaPosition(35f, 53f);
                                    h2Pos = ArenaPosition(35f, 47f);

                                    m1Pos = ArenaPosition(47f, 37f);
                                    m2Pos = ArenaPosition(43f, 37f);
                                    r1Pos = ArenaPosition(47f, 33f);
                                    r2Pos = ArenaPosition(43f, 33f);
                                }
                                // Flip B - Light Parties
                                else if (permFlipAB_2.value == 2)
                                {
                                    mtPos = ArenaPosition(47f, 63f);
                                    h1Pos = ArenaPosition(43f, 63f);
                                    m1Pos = ArenaPosition(47f, 67f);
                                    r1Pos = ArenaPosition(43f, 67f);

                                    otPos = ArenaPosition(47f, 37f);
                                    h2Pos = ArenaPosition(43f, 37f);
                                    m2Pos = ArenaPosition(47f, 33f);
                                    r2Pos = ArenaPosition(43f, 33f);
                                }
                            }

                        }
                        #endregion
                        #region // Snap Right First
                        else if (permSnapTwist_2.value >= 4 && permSnapTwist_2.value <= 6)
                        {
                            // Dodge funky/snap and get close to boss
                            if (whichStep == 1)
                            {
                                mtPos = ArenaPosition(43f, 47f);
                                m1Pos = ArenaPosition(47f, 43f);

                                h1Pos = ArenaPosition(43f, 27f);
                                r1Pos = ArenaPosition(47f, 23f);

                                otPos = ArenaPosition(43f, 67f);
                                m2Pos = ArenaPosition(47f, 63f);

                                h2Pos = ArenaPosition(43f, 87f);
                                r2Pos = ArenaPosition(47f, 83f);
                            }
                            // Dodge funky 2 wait snap 
                            else if (whichStep == 2)
                            {
                                mtPos = ArenaPosition(43f, 37f);
                                m1Pos = ArenaPosition(47f, 33f);

                                h1Pos = ArenaPosition(43f, 17f);
                                r1Pos = ArenaPosition(47f, 13f);

                                otPos = ArenaPosition(43f, 57f);
                                m2Pos = ArenaPosition(47f, 53f);

                                h2Pos = ArenaPosition(43f, 77f);
                                r2Pos = ArenaPosition(47f, 73f);
                            }
                            // Dodge Twist & Flip placement
                            else if (whichStep == 3)
                            {
                                // Flip A - Roles
                                if (permFlipAB_2.value == 1)
                                {
                                    mtPos = ArenaPosition(57f, 63f);
                                    otPos = ArenaPosition(53f, 67f);

                                    h1Pos = ArenaPosition(65f, 53f);
                                    h2Pos = ArenaPosition(65f, 47f);

                                    m1Pos = ArenaPosition(57f, 37f);
                                    m2Pos = ArenaPosition(53f, 37f);
                                    r1Pos = ArenaPosition(57f, 33f);
                                    r2Pos = ArenaPosition(53f, 33f);
                                }
                                // Flip B - Light Parties
                                else if (permFlipAB_2.value == 2)
                                {
                                    mtPos = ArenaPosition(57f, 63f);
                                    h1Pos = ArenaPosition(53f, 63f);
                                    m1Pos = ArenaPosition(57f, 67f);
                                    r1Pos = ArenaPosition(53f, 67f);

                                    otPos = ArenaPosition(57f, 37f);
                                    h2Pos = ArenaPosition(53f, 37f);
                                    m2Pos = ArenaPosition(57f, 33f);
                                    r2Pos = ArenaPosition(53f, 33f);
                                }
                            }

                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
                #region // Celebrate_2
                else if (m5sAttack == FightManager.M5SAttacks.Celebrate_2.ToString())
                {
                    // Clock around boss
                    mtPos = ArenaPosition(50f, 65f);
                    otPos = ArenaPosition(50f, 35f);
                    h1Pos = ArenaPosition(35f, 50f);
                    h2Pos = ArenaPosition(65f, 50f);
                    m1Pos = ArenaPosition(35f, 65f);
                    m2Pos = ArenaPosition(65f, 65f);
                    r1Pos = ArenaPosition(35f, 35f);
                    r2Pos = ArenaPosition(65f, 35f);

                }
                #endregion
                #region // Deep Cut 2
                else if (m5sAttack == FightManager.M5SAttacks.Deep_Cut_2.ToString())
                {
                    // Inital Clock Positions
                    if (whichStep == 0)
                    {
                        // Fight Start Loose Stacks
                        mtPos = ArenaPosition(50f, 35f);
                        otPos = ArenaPosition(62f, 32f);
                        h1Pos = ArenaPosition(35f, 23f);
                        h2Pos = ArenaPosition(58f, 25f);
                        m1Pos = ArenaPosition(46f, 26f);
                        m2Pos = ArenaPosition(50f, 18f);
                        r1Pos = ArenaPosition(40f, 16f);
                        r2Pos = ArenaPosition(60f, 14f);
                    }
                    // Deep Cut Choose Pos
                    else if (whichStep == 1)
                    {
                        // Setup Role Positions
                        mtPos = ArenaPosition(50f, 65f);
                        otPos = ArenaPosition(65f, 55f);

                        h1Pos = ArenaPosition(46f, 42f);
                        h2Pos = ArenaPosition(54f, 42f);

                        m1Pos = ArenaPosition(43f, 35f);
                        m2Pos = ArenaPosition(57f, 35f);

                        r1Pos = ArenaPosition(45f, 28f);
                        r2Pos = ArenaPosition(55f, 28f);
                    }
                    // Deep Cut Resolve
                    else if (whichStep == 2)
                    {
                        // Setup Role Positions
                        mtPos = ArenaPosition(50f, 65f);
                        otPos = ArenaPosition(65f, 55f);

                        h1Pos = ArenaPosition(46f, 42f);
                        h2Pos = ArenaPosition(54f, 42f);

                        m1Pos = ArenaPosition(43f, 35f);
                        m2Pos = ArenaPosition(57f, 35f);

                        r1Pos = ArenaPosition(45f, 28f);
                        r2Pos = ArenaPosition(55f, 28f);

                    }

                }
                #endregion

            }
        }
        // Need to have a function to keep units in the same place 
        if (skipPlacement)
        {
            mtPos = GetGameManager().GetCharacterManager().GetArenaPositionByRole(CharacterManager.RolePositions.Mt);
            otPos = GetGameManager().GetCharacterManager().GetArenaPositionByRole(CharacterManager.RolePositions.Ot);
            h1Pos = GetGameManager().GetCharacterManager().GetArenaPositionByRole(CharacterManager.RolePositions.H1);
            h2Pos = GetGameManager().GetCharacterManager().GetArenaPositionByRole(CharacterManager.RolePositions.H2);
            m1Pos = GetGameManager().GetCharacterManager().GetArenaPositionByRole(CharacterManager.RolePositions.M1);
            m2Pos = GetGameManager().GetCharacterManager().GetArenaPositionByRole(CharacterManager.RolePositions.M2);
            r1Pos = GetGameManager().GetCharacterManager().GetArenaPositionByRole(CharacterManager.RolePositions.R1);
            r2Pos = GetGameManager().GetCharacterManager().GetArenaPositionByRole(CharacterManager.RolePositions.R2);
        }


        // Save Role Positions
        npcPosition.Add(CharacterManager.RolePositions.Mt, mtPos);
        npcPosition.Add(CharacterManager.RolePositions.Ot, otPos);
        npcPosition.Add(CharacterManager.RolePositions.H1, h1Pos);
        npcPosition.Add(CharacterManager.RolePositions.H2, h2Pos);
        npcPosition.Add(CharacterManager.RolePositions.M1, m1Pos);
        npcPosition.Add(CharacterManager.RolePositions.M2, m2Pos);
        npcPosition.Add(CharacterManager.RolePositions.R1, r1Pos);
        npcPosition.Add(CharacterManager.RolePositions.R2, r2Pos);


        #endregion

    }
    public Vector3 GetNpcRolePosition(CharacterManager.RolePositions role)
    {
        if (npcPosition != null && npcPosition.ContainsKey(role))
        {
            return npcPosition[role];
        }
        else
        {
            Debug.LogWarning($"Requested position for role {role}, but it was not found in npcPosition.");
            return Vector3.zero; // or a sentinel value, depending on your use case
        }
    }

    private void InitializeDebug(bool isDebugging = false) { debug = new DebugInfo(isDebugging); }
    #region // References
    private GameManager GetGameManager() => gameManager;
    #endregion
}
