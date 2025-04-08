using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaymarkManager : MonoBehaviour
{
    public enum Waymarks
    {
        One     = 0, 
        Two     = 1, 
        Three   = 2, 
        Four    = 3,
        A       = 4, 
        B       = 5, 
        C       = 6, 
        D       = 7
    }
    public enum WaymarkSets
    {
        M4S_Hector,
        M4S_SunriseUptime
    }

    [SerializeField] private GameObject Waymark1Object;
    [SerializeField] private GameObject Waymark2Object;
    [SerializeField] private GameObject Waymark3Object;
    [SerializeField] private GameObject Waymark4Object;
    [SerializeField] private GameObject WaymarkAObject;
    [SerializeField] private GameObject WaymarkBObject;
    [SerializeField] private GameObject WaymarkCObject;
    [SerializeField] private GameObject WaymarkDObject;

    [SerializeField] private float scaleWidth = 35f;
    [SerializeField] private float scaleHeight = 35f;
    [SerializeField] private float zPosition = 1.5f;

    [SerializeField] private float waymarkZOffset = .5f;  // A z offset for the waymarks to appear in front


    // Alpha value for the transparency of the waymarks
    [Range(0, 1)] public float alpha = 1f; // Value that can be changed in the Inspector

    private Vector3[] waymarkPositions;

    #region // References 
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;
    #endregion

    public void InitializeWaymarkManager()
    {
        InitializeDebug();

        // Initialize waymark positions array
        waymarkPositions = new Vector3[8];

        // Set the waymarks' positions
        for (int i = 0; i < waymarkPositions.Length; i++)
        {
            // Set each waymark's z position higher than the background's z position
            waymarkPositions[i] = new Vector3(transform.position.x, transform.position.y, zPosition + waymarkZOffset);
        }

        DisableAllWaymarks();
    }
    private void InitializeDebug() { debug = new DebugInfo(); }

    public void SetWaymarkUsingSets()
    {
        // Get Fight info
        FightManager.FightEnum whichFight = GetGameManager().GetFightManager().GetCurrentFight();
        FightManager.FightGuide whichGuide = GetGameManager().GetFightManager().GetCurrentGuide();


        // Disable all waymark objects before setting them
        DisableAllWaymarks();

        if (whichFight == FightManager.FightEnum.M5S)
        {
            if (whichGuide == FightManager.FightGuide.Hector)
            {
                SetM5S_HectorPositions();
                ActivateWaymarks(true, true, true, true, true, true, true, true);
            }
        }
    }
    private void SetM5S_HectorPositions()
    {
        GameManager gm = GetGameManager();

        // Set positions in world space for M5S_Hector with updated z position
        waymarkPositions[(int)Waymarks.One  ] = new Vector3(gm.GetXAsArenaPercentage(25.5f), gm.GetYAsArenaPercentage(74.5f), 0);
        waymarkPositions[(int)Waymarks.Two  ] = new Vector3(gm.GetXAsArenaPercentage(75f), gm.GetYAsArenaPercentage(74.5f), 0);
        waymarkPositions[(int)Waymarks.Three] = new Vector3(gm.GetXAsArenaPercentage(75f), gm.GetYAsArenaPercentage(24.5f), 0);
        waymarkPositions[(int)Waymarks.Four ] = new Vector3(gm.GetXAsArenaPercentage(25.5f), gm.GetYAsArenaPercentage(24.5f), 0);
        waymarkPositions[(int)Waymarks.A    ] = new Vector3(gm.GetXAsArenaPercentage(50f), gm.GetYAsArenaPercentage(65), 0);
        waymarkPositions[(int)Waymarks.B    ] = new Vector3(gm.GetXAsArenaPercentage(65f), gm.GetYAsArenaPercentage(50f), 0);
        waymarkPositions[(int)Waymarks.C    ] = new Vector3(gm.GetXAsArenaPercentage(50f), gm.GetYAsArenaPercentage(35), 0);
        waymarkPositions[(int)Waymarks.D    ] = new Vector3(gm.GetXAsArenaPercentage(35f), gm.GetYAsArenaPercentage(50f), 0);

        // Update the GameObject positions and apply transparency
        UpdateWaymark(Waymarks.One  , waymarkPositions[(int)Waymarks.One  ]);
        UpdateWaymark(Waymarks.Two  , waymarkPositions[(int)Waymarks.Two  ]);
        UpdateWaymark(Waymarks.Three, waymarkPositions[(int)Waymarks.Three]);
        UpdateWaymark(Waymarks.Four , waymarkPositions[(int)Waymarks.Four ]);
        UpdateWaymark(Waymarks.A    , waymarkPositions[(int)Waymarks.A    ]);
        UpdateWaymark(Waymarks.B    , waymarkPositions[(int)Waymarks.B    ]);
        UpdateWaymark(Waymarks.C    , waymarkPositions[(int)Waymarks.C    ]);
        UpdateWaymark(Waymarks.D    , waymarkPositions[(int)Waymarks.D    ]);
    }

    //private void SetM4S_HectorPositions()
    //{
    //    float zp = zPosition;
    //    // Set positions in world space for M4S_Hector
    //    waymarkPositions[0] = new Vector3(0, 2f, zp);       // WaymarkA - Top
    //    waymarkPositions[1] = new Vector3(2.25f, 2f, zp);   // Waymark2 - Top/Right
    //    waymarkPositions[2] = new Vector3(2.25f, 0, zp);    // WaymarkB - Right
    //    waymarkPositions[3] = new Vector3(2.25f, -2f, zp);  // Waymark3 - Bottom/Right
    //    waymarkPositions[4] = new Vector3(0, -2f, zp);      // WaymarkC - Bottom
    //    waymarkPositions[5] = new Vector3(-2.25f, -2f, zp); // Waymark4 - Bottom/Left
    //    waymarkPositions[6] = new Vector3(-2.25f, 0, zp);   // WaymarkD - Left
    //    waymarkPositions[7] = new Vector3(-2.25f, 2f, zp);  // Waymark1 - Top/Left
    //
    //    // Update the GameObject positions and apply transparency
    //    UpdateWaymark(WaymarkAObject, waymarkPositions[0]);
    //    UpdateWaymark(Waymark2Object, waymarkPositions[1]);
    //    UpdateWaymark(WaymarkBObject, waymarkPositions[2]);
    //    UpdateWaymark(Waymark3Object, waymarkPositions[3]);
    //    UpdateWaymark(WaymarkCObject, waymarkPositions[4]);
    //    UpdateWaymark(Waymark4Object, waymarkPositions[5]);
    //    UpdateWaymark(WaymarkDObject, waymarkPositions[6]);
    //    UpdateWaymark(Waymark1Object, waymarkPositions[7]);
    //}
    //
    //private void SetM4S_SunriseUptimePositions()
    //{
    //    float zp = zPosition;
    //    // Set different positions for M4S_SunriseUptime if required
    //    waymarkPositions[0] = new Vector3(0, 6, zp);  // WaymarkA - Example
    //    waymarkPositions[1] = new Vector3(6, 6, zp);  // Waymark2 - Example
    //    waymarkPositions[4] = new Vector3(0, -6, zp); // WaymarkC - Example
    //    waymarkPositions[5] = new Vector3(-6, -6, zp);// Waymark4 - Example
    //
    //    // Update only the active GameObject positions and apply transparency
    //    UpdateWaymark(WaymarkAObject, waymarkPositions[0]);
    //    UpdateWaymark(Waymark2Object, waymarkPositions[1]);
    //    UpdateWaymark(WaymarkCObject, waymarkPositions[4]);
    //    UpdateWaymark(Waymark4Object, waymarkPositions[5]);
    //}

    

    private void UpdateWaymark(Waymarks waymarkEnum, Vector3 position)
    {
        GameObject waymarkObject = GetWaymarkFromEnum(waymarkEnum);

        // Move the GameObject to the specified position
        waymarkObject.transform.position = position;

        // Optionally set the scale here based on the specified scaleWidth and scaleHeight
        waymarkObject.transform.localScale = new Vector3(scaleWidth, scaleHeight, 1);

        // Apply alpha transparency to the SpriteRenderer
        SpriteRenderer spriteRenderer = waymarkObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha; // Set the alpha to the value specified in the Inspector
            spriteRenderer.color = color;
        }
    }
    private GameObject GetWaymarkFromEnum (Waymarks waymarkEnum)
    {
        switch (waymarkEnum)
        {
            case Waymarks.One   : return Waymark1Object; break;
            case Waymarks.Two  : return Waymark2Object; break;
            case Waymarks.Three: return Waymark3Object; break;
            case Waymarks.Four : return Waymark4Object; break;
            case Waymarks.A    : return WaymarkAObject; break;
            case Waymarks.B    : return WaymarkBObject; break;
            case Waymarks.C    : return WaymarkCObject; break;
            case Waymarks.D    : return WaymarkDObject; break;
            default: return null;
        }
    }

    private void DisableAllWaymarks()
    {
        // Disable all waymark objects
        Waymark1Object.SetActive(false);
        Waymark2Object.SetActive(false);
        Waymark3Object.SetActive(false);
        Waymark4Object.SetActive(false);
        WaymarkAObject.SetActive(false);
        WaymarkBObject.SetActive(false);
        WaymarkCObject.SetActive(false);
        WaymarkDObject.SetActive(false);
    }

    private void ActivateWaymarks(bool waymark1, bool waymark2, bool waymark3, bool waymark4,
                                  bool waymarkA, bool waymarkB, bool waymarkC, bool waymarkD)
    {
        // Activate or deactivate GameObjects based on input
        Waymark1Object.SetActive(waymark1);
        Waymark2Object.SetActive(waymark2);
        Waymark3Object.SetActive(waymark3);
        Waymark4Object.SetActive(waymark4);
        WaymarkAObject.SetActive(waymarkA);
        WaymarkBObject.SetActive(waymarkB);
        WaymarkCObject.SetActive(waymarkC);
        WaymarkDObject.SetActive(waymarkD);
    }
    #region // References 
    private GameManager GetGameManager() => gameManager;
    #endregion
}
