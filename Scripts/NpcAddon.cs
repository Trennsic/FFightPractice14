using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NpcAddon : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 basePosition;
    private float targetRotation;
    private float moveDuration;
    private float moveStartTime;
    private float zPosition = 0f;
    private CharacterInfo.RolePositions rolePosition;
    [SerializeField] private DebugInfo debugInfo;
    [SerializeField] private NpcManagers NpcManager;
    [SerializeField] private CharacterInfo NpcInfo;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        #region // Get or add the SpriteRenderer component
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("Starting initialization of NpcAddon script.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log("SpriteRenderer component not found, added new SpriteRenderer.");
            }
        }
        #endregion

        // Make sure to find Npc Manager
        NpcManager = gameObject.GetComponentInParent<NpcManagers>();
        if (NpcManager == null)
        {
            Debug.LogError("Unable to find NPC manager on parent");
        }
        else
        {
            zPosition = NpcManager.GetZPosition();
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log($"NpcManager found. zPosition set to {zPosition}.");
            }
        }

        // Get Character info
        NpcInfo = GetComponent<CharacterInfo>();
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"CharacterInfo component fetched: {NpcInfo != null}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // You can add trace logs here as needed for debugging update-related logic.
    }

    public void MoveNpc(Vector3 goalPosition, float goalRotation, float duration)
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"MoveNpc called with goalPosition: {goalPosition}, goalRotation: {goalRotation}, duration: {duration}");
        }

        // Ensure the Z position remains fixed
        goalPosition = new Vector3(goalPosition.x, goalPosition.y, zPosition);

        // Initialize target values
        targetPosition = goalPosition;
        targetRotation = goalRotation;
        moveDuration = duration;
        moveStartTime = Time.time;

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Initialized move parameters. targetPosition: {targetPosition}, targetRotation: {targetRotation}, moveDuration: {moveDuration}, moveStartTime: {moveStartTime}");
        }

        // Start the coroutine to move the player
        StartCoroutine(MoveNpcCoroutine());
    }

    private IEnumerator MoveNpcCoroutine()
    {
        Vector3 initialPosition = new Vector3(transform.position.x, transform.position.y, zPosition); // Ensure initial Z position is used
        Quaternion initialRotation = transform.rotation;

        float elapsedTime = 0f;

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Starting player move coroutine: Initial Position = {initialPosition}, Target Position = {targetPosition}");
        }

        while (elapsedTime < moveDuration)
        {
            elapsedTime = Time.time - moveStartTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // Interpolate position and rotation
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(0, 0, targetRotation), t);

            if (debugInfo.GetIsDebugging())
            {
                Debug.Log($"Moving NPC. Elapsed time: {elapsedTime}, t: {t}, Current Position: {transform.position}");
            }

            yield return null; // Wait for the next frame
        }

        // Ensure final position and rotation are exactly the target values
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        // Set base position to new position, with correct Z position
        basePosition = new Vector3(transform.position.x, transform.position.y, zPosition);

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Player move complete: Final Position/Base Position = {transform.position}, Final Rotation = {transform.rotation.eulerAngles.z}");
        }
    }

    public void SetupNpc(CharacterInfo.RolePositions playerRole, CharacterInfo.Jobs playerJob)
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"SetupNpc called with playerRole: {playerRole} + playerJob: {playerJob}");
        }
        CharacterInfo.Jobs selectedJob;
        CharacterInfo.Jobs jobToAvoid;
        jobToAvoid = playerJob;
        selectedJob = jobToAvoid;
        int randomJob;
        //Picks the job based on current role
        //Set up character Info
        if (rolePosition == CharacterInfo.RolePositions.MT || rolePosition == CharacterInfo.RolePositions.OT)
        {
            while (selectedJob == jobToAvoid)
            {
                // Pick a random job
                randomJob = Random.Range(1, 4);
                switch(randomJob)
                {
                    case 0: selectedJob = CharacterInfo.Jobs.PLD; break;
                    case 1: selectedJob = CharacterInfo.Jobs.WAR; break;
                    case 2: selectedJob = CharacterInfo.Jobs.DRK; break;
                    case 3: selectedJob = CharacterInfo.Jobs.GNB; break;
                }
            }
        }
        else if (rolePosition == CharacterInfo.RolePositions.H1 || rolePosition == CharacterInfo.RolePositions.H2)
        {
            while (selectedJob == jobToAvoid)
            {
                // Pick a random job
                randomJob = Random.Range(1, 4);
                switch (randomJob)
                {
                    case 0: selectedJob = CharacterInfo.Jobs.WHM; break;
                    case 1: selectedJob = CharacterInfo.Jobs.SCH; break;
                    case 2: selectedJob = CharacterInfo.Jobs.AST; break;
                    case 3: selectedJob = CharacterInfo.Jobs.SGE; break;
                }
            }
        }
        else if (rolePosition == CharacterInfo.RolePositions.M1 || rolePosition == CharacterInfo.RolePositions.M2)
        {
            while (selectedJob == jobToAvoid)
            {
                // Pick a random job
                randomJob = Random.Range(1, 6);
                switch (randomJob)
                {
                    case 0: selectedJob = CharacterInfo.Jobs.MNK; break;
                    case 1: selectedJob = CharacterInfo.Jobs.DRG; break;
                    case 2: selectedJob = CharacterInfo.Jobs.NIN; break;
                    case 3: selectedJob = CharacterInfo.Jobs.SAM; break;
                    case 4: selectedJob = CharacterInfo.Jobs.RPR; break;
                    case 5: selectedJob = CharacterInfo.Jobs.VIP; break;
                }
            }
        }
        else if (rolePosition == CharacterInfo.RolePositions.R1 || rolePosition == CharacterInfo.RolePositions.R2)
        {
            while (selectedJob == jobToAvoid)
            {
                // Pick a random job
                randomJob = Random.Range(1, 7);
                switch (randomJob)
                {
                    case 0: selectedJob = CharacterInfo.Jobs.BRD; break;
                    case 1: selectedJob = CharacterInfo.Jobs.MCH; break;
                    case 2: selectedJob = CharacterInfo.Jobs.DNC; break;
                    case 3: selectedJob = CharacterInfo.Jobs.PIC; break;
                    case 4: selectedJob = CharacterInfo.Jobs.BLM; break;
                    case 5: selectedJob = CharacterInfo.Jobs.SMN; break;
                    case 6: selectedJob = CharacterInfo.Jobs.RDM; break;
                }
            }
        }
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Job Selected is {selectedJob}");
        }
        //Update info
        NpcInfo.SetRolePosition(rolePosition);
        NpcInfo.SetJob(selectedJob);
        NpcInfo.SetSpriteType(CharacterInfo.SpriteTypes.Chibi);
        SetZPosition(NpcManager.GetZPosition());


        UpdateNpcSprite();
    }

    // Helper function to update player's facing direction based on position relative to the boss
    private void UpdateFacingDirection()
    {
        Vector3 playerPos = transform.position;
        Vector3 bossPos = GetBossPosition();

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"UpdateFacingDirection called. Player position: {playerPos}, Boss position: {bossPos}");
        }

        if (playerPos.x < bossPos.x)
        {
            // Player is to the left of the boss, so face right
            NpcInfo.SetFacing(CharacterInfo.FacingDirections.Right);
        }
        else if (playerPos.x > bossPos.x)
        {
            // Player is to the right of the boss, so face left
            NpcInfo.SetFacing(CharacterInfo.FacingDirections.Left);
        }

        // Update the sprite
        //UpdateNpcSprite();

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Player is now facing: {(playerPos.x < bossPos.x ? "Right" : "Left")}");
        }
    }

    public void UpdateNpcSprite()
    {
        Sprite selectedSprite = null;
        CharacterInfo.SpriteTypes spriteType = NpcInfo.SpriteType;
        CharacterInfo.Jobs job = NpcInfo.Job;
        CharacterInfo.RolePositions rolePosition = NpcInfo.RolePosition;

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Updating NPC sprite: SpriteType = {spriteType}, Job = {job}, RolePosition = {rolePosition}");
        }

        if (NpcInfo != null)
        {
            selectedSprite = NpcInfo.GetSprite(spriteType, job, rolePosition);
        }

        // Determine the sprite based on the job and sprite type
        Vector3 offset = Vector3.zero;

        if (spriteType == CharacterInfo.SpriteTypes.Chibi)
        {
            transform.localScale = new Vector3(NpcInfo.ChibiXscale, NpcInfo.ChibiYscale, 1);
            offset = new Vector3(NpcInfo.ChibiXoffset, NpcInfo.ChibiYoffset, 0f); // Apply Chibi offsets
        }
        else if (spriteType == CharacterInfo.SpriteTypes.Icon)
        {
            transform.localScale = new Vector3(NpcInfo.IconXscale, NpcInfo.IconYscale, 1);
            offset = new Vector3(NpcInfo.IconXoffset, NpcInfo.IconYoffset, 0f); // Apply Icon offsets
        }
        else if (spriteType == CharacterInfo.SpriteTypes.Role)
        {
            transform.localScale = new Vector3(NpcInfo.RoleXscale, NpcInfo.RoleYscale, 1);
            offset = new Vector3(NpcInfo.RoleXoffset, NpcInfo.RoleYoffset, 0f); // Apply Role offsets
        }

        // Apply the base position plus offset as the new local position
        transform.localPosition = basePosition + offset;

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Applied offsets: Base position = {basePosition}, Offset = {offset}, New position = {transform.localPosition}");
        }

        // Flip the sprite if the player is facing right
        if (NpcInfo.Facing == CharacterInfo.FacingDirections.Right)
        {
            // Flip the sprite by inverting the X scale
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // Ensure the sprite is not flipped when facing left
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"Sprite facing: {NpcInfo.Facing}");
        }

        // Set the sprite renderer to the selected sprite
        if (selectedSprite != null)
        {
            spriteRenderer.sprite = selectedSprite;
            if (debugInfo.GetIsDebugging())
            {
                Debug.Log($"Sprite set to {selectedSprite.name}");
            }
        }
        else
        {
            Debug.LogWarning($"No sprite found for job {job} and sprite type {spriteType}");
        }
        //Update facing
        //UpdateFacingDirection();
    }

    public void SetZPosition(float zPos)
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"SetZPosition called with zPos: {zPos}");
        }
        zPosition = zPos;
    }

    private Vector3 GetBossPosition()
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log("GetBossPosition called.");
        }
        return NpcManager.GetBossPosition();
    }

    public CharacterInfo.RolePositions GetRolePosition() => rolePosition;
    public void SetRolePosition(CharacterInfo.RolePositions rolePos)
    {
        if (debugInfo.GetIsDebugging())
        {
            Debug.Log($"SetRolePosition called with rolePos: {rolePos}");
        }
        rolePosition = rolePos;
    }
}
