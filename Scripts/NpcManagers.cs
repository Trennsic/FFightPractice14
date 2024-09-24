using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NpcManagers : MonoBehaviour
{
    [SerializeField] private GameObject MtObject;
    [SerializeField] private GameObject OtObject;
    [SerializeField] private GameObject H1Object;
    [SerializeField] private GameObject H2Object;
    [SerializeField] private GameObject M1Object;
    [SerializeField] private GameObject M2Object;
    [SerializeField] private GameObject R1Object;
    [SerializeField] private GameObject R2Object;

    [SerializeField] private DebugInfo debugInfo;
    [SerializeField] private float zPosition = -.3f;
    [SerializeField] private CharacterInfo.RolePositions PlayerRole;
    [SerializeField] private CharacterInfo.Jobs PlayerJob;

    private Vector3 targetPosition;
    private float targetRotation;
    private float moveDuration;
    private float moveStartTime;

    // Start is called before the first frame update
    void Awake()
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Initializing NpcManagers.");
        }

        InitalizeAllPartyObjects();
        DeactivateAllPartyObjects();
    }

    // Update is called once per frame
    void Update()
    {
        // You can add trace logs here if you have update-related logic.
    }

    private void InitalizeAllPartyObjects()
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Initializing all party objects.");
        }

        // Initialize NpcAddon var
        NpcAddon npc;

        // Set Correct Role Position for each NPC
        npc = MtObject.GetComponent<NpcAddon>();
        npc.SetRolePosition(CharacterInfo.RolePositions.MT);
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Set MT role for MtObject.");
        }

        npc = OtObject.GetComponent<NpcAddon>();
        npc.SetRolePosition(CharacterInfo.RolePositions.OT);
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Set OT role for OtObject.");
        }

        npc = H1Object.GetComponent<NpcAddon>();
        npc.SetRolePosition(CharacterInfo.RolePositions.H1);
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Set H1 role for H1Object.");
        }

        npc = H2Object.GetComponent<NpcAddon>();
        npc.SetRolePosition(CharacterInfo.RolePositions.H2);
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Set H2 role for H2Object.");
        }

        npc = M1Object.GetComponent<NpcAddon>();
        npc.SetRolePosition(CharacterInfo.RolePositions.M1);
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Set M1 role for M1Object.");
        }

        npc = M2Object.GetComponent<NpcAddon>();
        npc.SetRolePosition(CharacterInfo.RolePositions.M2);
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Set M2 role for M2Object.");
        }

        npc = R1Object.GetComponent<NpcAddon>();
        npc.SetRolePosition(CharacterInfo.RolePositions.R1);
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Set R1 role for R1Object.");
        }

        npc = R2Object.GetComponent<NpcAddon>();
        npc.SetRolePosition(CharacterInfo.RolePositions.R2);
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Set R2 role for R2Object.");
        }
    }

    private void DeactivateAllPartyObjects()
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Deactivating all party member objects.");
        }

        // Deactivate all party member objects
        MtObject.SetActive(false);
        OtObject.SetActive(false);
        H1Object.SetActive(false);
        H2Object.SetActive(false);
        M1Object.SetActive(false);
        M2Object.SetActive(false);
        R1Object.SetActive(false);
        R2Object.SetActive(false);
    }

    public void MoveNpc(Vector3 goalPosition, float goalRotation, float duration, CharacterInfo.RolePositions myRole)
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"MoveNpc called with goalPosition: {goalPosition}, goalRotation: {goalRotation}, duration: {duration}, myRole: {myRole}");
        }

        CharacterInfo.RolePositions playerRole = PlayerRole;
        // Initialize temporary NPC object
        GameObject selectedNpc = null;

        // Select the correct subobject based on role
        // (but only if player isn't already that role, player selected role will be handled by player)
        if (myRole == CharacterInfo.RolePositions.MT && playerRole != CharacterInfo.RolePositions.MT) { selectedNpc = MtObject; }
        if (myRole == CharacterInfo.RolePositions.OT && playerRole != CharacterInfo.RolePositions.OT) { selectedNpc = OtObject; }
        if (myRole == CharacterInfo.RolePositions.H1 && playerRole != CharacterInfo.RolePositions.H1) { selectedNpc = H1Object; }
        if (myRole == CharacterInfo.RolePositions.H2 && playerRole != CharacterInfo.RolePositions.H2) { selectedNpc = H2Object; }
        if (myRole == CharacterInfo.RolePositions.M1 && playerRole != CharacterInfo.RolePositions.M1) { selectedNpc = M1Object; }
        if (myRole == CharacterInfo.RolePositions.M2 && playerRole != CharacterInfo.RolePositions.M2) { selectedNpc = M2Object; }
        if (myRole == CharacterInfo.RolePositions.R1 && playerRole != CharacterInfo.RolePositions.R1) { selectedNpc = R1Object; }
        if (myRole == CharacterInfo.RolePositions.R2 && playerRole != CharacterInfo.RolePositions.R2) { selectedNpc = R2Object; }

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Selected NPC for role {myRole}: {(selectedNpc != null ? selectedNpc.name : "None")}");
        }

        // Move Npc
        if (selectedNpc != null)
        {
            NpcAddon npc = selectedNpc.GetComponent<NpcAddon>();
            npc.MoveNpc(goalPosition, goalRotation, duration);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log($"Moving NPC: {selectedNpc.name} to position {goalPosition} with rotation {goalRotation} over {duration} seconds.");
            }
        }
    }

    public void SetupParty(CharacterInfo.RolePositions playerRole, CharacterInfo.Jobs playerJob)
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"SetupParty called with playerRole: {playerRole}");
        }

        PlayerJob = playerJob;
        PlayerRole = playerRole;

        // Deactivate all party members
        DeactivateAllPartyObjects();
        // Initialize NpcAddon var
        NpcAddon npc;

        // Reactivate party members who don't match player role
        // Send command to set self up
        if (PlayerRole != CharacterInfo.RolePositions.MT)
        {
            MtObject.SetActive(true);
            npc = MtObject.GetComponent<NpcAddon>();
            npc.SetupNpc(PlayerRole, PlayerJob);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("Activated MtObject and set up NPC.");
            }
        }

        if (PlayerRole != CharacterInfo.RolePositions.OT)
        {
            OtObject.SetActive(true);
            npc = OtObject.GetComponent<NpcAddon>();
            npc.SetupNpc(PlayerRole, PlayerJob);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("Activated OtObject and set up NPC.");
            }
        }

        if (PlayerRole != CharacterInfo.RolePositions.H1)
        {
            H1Object.SetActive(true);
            npc = H1Object.GetComponent<NpcAddon>();
            npc.SetupNpc(PlayerRole, PlayerJob);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("Activated H1Object and set up NPC.");
            }
        }

        if (PlayerRole != CharacterInfo.RolePositions.H2)
        {
            H2Object.SetActive(true);
            npc = H2Object.GetComponent<NpcAddon>();
            npc.SetupNpc(PlayerRole, PlayerJob);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("Activated H2Object and set up NPC.");
            }
        }

        if (PlayerRole != CharacterInfo.RolePositions.M1)
        {
            M1Object.SetActive(true);
            npc = M1Object.GetComponent<NpcAddon>();
            npc.SetupNpc(PlayerRole, PlayerJob);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("Activated M1Object and set up NPC.");
            }
        }

        if (PlayerRole != CharacterInfo.RolePositions.M2)
        {
            M2Object.SetActive(true);
            npc = M2Object.GetComponent<NpcAddon>();
            npc.SetupNpc(PlayerRole, PlayerJob);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("Activated M2Object and set up NPC.");
            }
        }

        if (PlayerRole != CharacterInfo.RolePositions.R1)
        {
            R1Object.SetActive(true);
            npc = R1Object.GetComponent<NpcAddon>();
            npc.SetupNpc(PlayerRole, PlayerJob);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("Activated R1Object and set up NPC.");
            }
        }

        if (PlayerRole != CharacterInfo.RolePositions.R2)
        {
            R2Object.SetActive(true);
            npc = R2Object.GetComponent<NpcAddon>();
            npc.SetupNpc(PlayerRole, PlayerJob);
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("Activated R2Object and set up NPC.");
            }
        }
    }

    public float GetZPosition()
    {
        if (debugInfo.GetIsDebugging())
        {
            //Debug.Log($"GetZPosition called, returning: {zPosition}");
        }
        return zPosition;
    }

    public Vector3 GetBossPosition()
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("GetBossPosition called, returning Vector3.zero.");
        }
        return Vector3.zero;
    }
    //
    public Vector3 GetNpcPosition(CharacterInfo.RolePositions role)
    {
        if (role == CharacterInfo.RolePositions.MT) { return MtObject.transform.position; }
        if (role == CharacterInfo.RolePositions.OT) { return OtObject.transform.position; }
        if (role == CharacterInfo.RolePositions.H1) { return H1Object.transform.position; }
        if (role == CharacterInfo.RolePositions.H2) { return H2Object.transform.position; }
        if (role == CharacterInfo.RolePositions.M1) { return M1Object.transform.position; }
        if (role == CharacterInfo.RolePositions.M2) { return M2Object.transform.position; }
        if (role == CharacterInfo.RolePositions.R1) { return R1Object.transform.position; }
        if (role == CharacterInfo.RolePositions.R2) { return R2Object.transform.position; }
        return Vector3.zero;
    }
}
