using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterInfo;

public class PlayerPositionManager : MonoBehaviour
{ 
    [SerializeField] private bool usingGuidePoints; // Checks if the player is allowed to pick any position (false) or only pre-chosen points (true)
    [SerializeField] private bool pickingPosition; // If the player is currently picking a position (true)
    #region // References
    [SerializeField] private PlayerManager player; // object reference to Player
    [SerializeField] private GameObject guidePointObject; // Small circular gameobject representing where the player can go
    [SerializeField] private FightManager fm; // Object reference to fight manager
    #endregion

    private bool guidePointsSet;
    private FightManager.FightEnum setGpFight;
    private string setGpAttack;
    private int setGpStep;

    private List<GameObject> guidePointList = new List<GameObject>(); // Initialize the list

    [SerializeField] private bool isDebugging;

    private Vector3 selectedPosition; // Stores the player's selected position

    // Start is called before the first frame update
    void Start()
    {
        if (fm == null)
        {
            Debug.LogError("PlayerPositionManager doesn't contain a reference to Fight Manager");
        }
        if (player == null)
        {
            Debug.LogError("PlayerPositionManager doesn't contain a reference to Player Manager");
        }
        //Start with using guide points
        SetUsingGuidePoints(true);
    }

    // Update is called once per frame
    void Update()
    {
        PickingPosition();

        CheckIfStillUsingGuidePoints();
    }

    private void CheckIfStillUsingGuidePoints()
    {
        //If not picking position don't check
        if (!pickingPosition) return;

        // Get Fight Manager Fight info
        CurrentFightInfo cfi = fm.currentFightInfo;
        //Check for any changes from the setstep - attack - fight
        if (setGpFight != cfi.CurrentFight ||
            setGpAttack != cfi.CurrentAttack ||
            setGpStep != cfi.CurrentStep)
        {
            //Clear guide points
            ClearGuidePoints();
        }


    }

    // Changes if using guide points is true or not
    public void SetUsingGuidePoints(bool isUsingGuidePoints)
    {
        usingGuidePoints = isUsingGuidePoints;
    }

    // Returns if using guide points is true or not
    public bool GetUsingGuidePoints()
    {
        // Get Fight Manager Fight info
        CurrentFightInfo cfi = fm.currentFightInfo;

        setGpFight  = cfi.CurrentFight;
        setGpAttack = cfi.CurrentAttack;
        setGpStep   = cfi.CurrentStep;
        return usingGuidePoints;
    }

    // Starts the position picking process
    public void StartPickingPosition()
    {
        pickingPosition = true;
    }

    // Sets up guide points for predefined positions
    public void SetupGuidePoints()
    {
        if (!GetUsingGuidePoints()) return;
        if (guidePointObject == null)
        {
            Debug.LogWarning("Unable to find Guide point Object");
            return;
        }

        // Clear any existing guide points
        ClearGuidePoints();

        // Set Guide Point Z position
        float gpz = -3f;

        // Get Fight Manager Fight info
        CurrentFightInfo cfi = fm.currentFightInfo;

        //Get Player role position
        CharacterInfo.RolePositions rp = GetPlayerRolePosition();

        #region // M4S
        if (GetIsDebugging()) { Debug.Log("Current Fight = " + cfi.CurrentFight); }
        if (cfi.CurrentFight == FightManager.FightEnum.M4S)
        {
            if (GetIsDebugging()) { Debug.Log("Current Fight = " + cfi.CurrentAttack); }
            if (cfi.CurrentAttack == "Bewitching Flight")
            {
                if (GetIsDebugging()) { Debug.Log("Current Fight = " + cfi.CurrentStep); }
                // Bewitching flight North Lines
                if (cfi.CurrentStep == 3)
                {
                    // Populate Guide Point list with instantiations of Guide point objects at set locations
                    #region Tank and Melees GPs
                    if (rp == RolePositions.MT || rp == RolePositions.OT || rp == RolePositions.M1 || rp == RolePositions.M2)
                    {
                        #region // Far Top Points

                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Top Points
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region Healer1 Ranged 1 GPs
                    else if (rp == RolePositions.H1 || rp == RolePositions.R1)
                    {
                        #region // Near Top Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                    }
                    #endregion
                    #region Healer 2 Ranged 2 GPs
                    else if (rp == RolePositions.H2 || rp == RolePositions.R2)
                    {
                        #region // Near Top Points
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion
                    }
                    #endregion
                    else
                    {
                        #region // Far Top Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Top Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion
                    }

                    // Picking Position should be set to true
                    StartPickingPosition();
                }
                // Bewitching flight First Ground Lines
                else if (cfi.CurrentStep == 6)
                {
                    // Populate Guide Point list with instantiations of Guide point objects at set locations
                    #region Tank and Melees GPs
                    if (rp == RolePositions.MT || rp == RolePositions.OT || rp == RolePositions.M1 || rp == RolePositions.M2)
                    {
                        #region // Far Top Points

                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Top Points
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion
                    }
                    #endregion
                    #region Healer2 
                    else if (rp == RolePositions.H2)
                    {
                        #region // Near Top Points
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4.1f   , -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.2f   , -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.35f  , -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4.1f   , -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.2f   , -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.35f  , -0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.5f   , -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.6f   , -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.5f   , -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.6f   , -0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4.1f  , -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.2f   , -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.35f  , -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4.1f  , -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.2f   , -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.35f  , -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.5f   , -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.6f   , -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.5f   , -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.6f   , -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                    }
                    #endregion
                    #region Healer1 Ranged 1 GPs
                    else if (rp == RolePositions.R1)
                    {
                        #region // Near Top Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                    }
                    #endregion
                    #region Healer 2 Ranged 2 GPs
                    else if (rp == RolePositions.R2)
                    {
                        #region // Near Top Points
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion
                    }
                    #endregion
                    else
                    {
                        #region // Far Top Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 2.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Top Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, 0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, 0.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -0.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -1.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -1.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion

                        #region // Near Bottom Points
                        #region //Far Top Far Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-3.1f, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-2.25f, -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Left Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.3f, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-0.4f, -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top Far Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(3.1f, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(2.25f, -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #region //Far Top near Right Square
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -2.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.3f, -3.5f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(0.4f, -3.5f, gpz), Quaternion.identity));
                        #endregion
                        #endregion
                    }

                    // Picking Position should be set to true
                    StartPickingPosition();
                }
                // Bewitching flight First Ground Lines
                else if (cfi.CurrentStep == 8)
                {
                    // Populate Guide Point list with instantiations of Guide point objects at set locations
                    #region // Main Tank and Ranged 1
                    if (rp == RolePositions.MT || rp == RolePositions.R1)
                    {
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4.15f, 1.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.50f, 1.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.50f, 3.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4.15f, 3.0f, gpz), Quaternion.identity));
                    }
                    #endregion
                    #region Offtank and Ranged 2 
                    else if (rp == RolePositions.OT || rp == RolePositions.R2)
                    {
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4.15f, 1.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.50f, 1.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.50f, 3.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4.15f, 3.0f, gpz), Quaternion.identity));

                    }
                    #endregion
                    #region Healer1 Ranged 1 GPs
                    else if (rp == RolePositions.H1 || rp == RolePositions.M1)
                    {
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4.15f, -1.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.50f, -1.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-1.50f, -3.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(-4.15f, -3.0f, gpz), Quaternion.identity));

                    }
                    #endregion
                    #region Healer 2 Ranged 2 GPs
                    else if (rp == RolePositions.H2 || rp == RolePositions.M2)
                    {
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4.15f  , -1.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.50f  , -1.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(1.50f  , -3.0f, gpz), Quaternion.identity));
                        guidePointList.Add(Instantiate(guidePointObject, new Vector3(4.15f  , -3.0f, gpz), Quaternion.identity));
                    }
                    #endregion

                    // Picking Position should be set to true
                    StartPickingPosition();
                }

            }
        }
        #endregion
    }

    // Clears all guide point objects from the list
    public void ClearGuidePoints()
    {
        

        foreach (GameObject gp in guidePointList)
        {
            Destroy(gp);
        }
        guidePointList.Clear();

        //Make sure you're no longer picking positions thatdon't exist
        pickingPosition = false;
    }

    // Handles player picking a position using 3D Physics
    public void PickingPosition()
    {
        if (!pickingPosition) return;

        // Check for player mouse input
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            // Get the mouse position in world space (X, Y from mouse, Z from camera)
            Vector3 mousePosition = Input.mousePosition;
            Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
            float rayLength = 10f;

            // Define the direction along the Z-axis for the ray
            Vector3 rayDirection = Camera.main.transform.forward; // Along the camera's forward direction

            // Cast a ray for a distance of 10 units in 3D space
            Ray ray = new Ray(rayOrigin, rayDirection);
            RaycastHit hit;

            // Perform the 3D raycast
            bool isHit = Physics.Raycast(ray, out hit, rayLength);

            // Show the debug line (only visible in Scene view) for 2 seconds
            if (isDebugging)
            {
                Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * rayLength, Color.green, 2f);
                Debug.Log($"Raycast sent from {rayOrigin} along the Z-axis");

                // Log the object hit by the raycast, if any
                if (isHit && hit.collider != null)
                {
                    Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
                }
                else
                {
                    Debug.LogWarning("Raycast didn't hit any objects.");
                }
            }

            // Process the object hit
            if (isHit && hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;

                // If using guide points
                if (usingGuidePoints)
                {
                    if (guidePointList.Contains(clickedObject))
                    {
                        selectedPosition = clickedObject.transform.position;

                        if (isDebugging)
                        {
                            Debug.Log($"Valid position selected: {selectedPosition}");
                        }

                        // Move Player to the position
                        Vector3 newPosition = new Vector3(selectedPosition.x, selectedPosition.y, player.transform.position.z);
                        player.MovePlayer(newPosition, 0, .1f);

                        // Clear guide points when one is chosen
                        ClearGuidePoints();
                        // Picking positions is done
                        pickingPosition = false;
                    }
                    else
                    {
                        if (isDebugging)
                        {
                            Debug.LogWarning("Clicked position is not a valid guide point");
                        }
                    }
                }
                // If not using guide points, custom logic can be implemented here
                else
                {
                    selectedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    selectedPosition.z = 0; // Ensure it's at the correct z position

                    if (isDebugging)
                    {
                        Debug.Log($"Position selected: {selectedPosition}");
                    }

                    // Move Player to the position
                    Vector3 newPosition = new Vector3(selectedPosition.x, selectedPosition.y, player.transform.position.z);
                    player.MovePlayer(newPosition, 0, .1f);

                    pickingPosition = false;

                    
                }
            }
        }
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

    // Returns the selected position
    public Vector3 GetSelectedPosition()
    {
        return selectedPosition;
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
    public Vector3 GetPlayerPosition()
    {
        //Initalize a boss position var
        Vector3 playerPos = new Vector3(0, 0, 0);
        //Find boss transform position
        if (fm != null)
        {
            playerPos = fm.GetPlayerPosition();
        }
        else { Debug.LogWarning("Unable to find player position due to not finding fight manager"); }
        //Return boss position
        return playerPos;
    }
    public CharacterInfo.RolePositions GetPlayerRolePosition()
    {
        //Initalize role var
        CharacterInfo.RolePositions playerRole = CharacterInfo.RolePositions.None;
        //Find boss transform position
        if (player != null)
        {
            playerRole = player.characterInfo.RolePosition;
        }
        else { Debug.LogWarning("Unable to find player role due to not finding player manager"); }
        //Return boss position
        return playerRole;
    }
}
