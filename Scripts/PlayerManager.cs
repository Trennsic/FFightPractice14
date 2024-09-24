using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EffectsManager;
using UnityEngine.UIElements;
using static FightManager;
using static BossManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EffectsManager;
using UnityEngine.UIElements;
using static FightManager;
using static BossManager;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    #region // References
    public CharacterInfo characterInfo;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float zPosition = -2f; // Set the desired Z position for the player
    [SerializeField] private FightManager fm;
    #endregion

    private Vector3 targetPosition;
    private float targetRotation;
    private float moveDuration;
    private float moveStartTime;

    [SerializeField] private bool isDebugging;

    // Store previous values to detect changes
    private CharacterInfo.SpriteTypes previousSpriteType;
    private CharacterInfo.Jobs previousJob;
    private CharacterInfo.RolePositions previousRolePosition;

    // Keep track of the base position separately
    private Vector3 basePosition;

    // Start is called before the first frame update
    void Start()
    {
        #region // Get or add the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        #endregion

        #region // Set the Z position
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosition); // Ensure initial Z position
        #endregion

        // Store the initial position as the base position
        basePosition = new Vector3(transform.position.x, transform.position.y, zPosition);

        if (isDebugging)
        {
            Debug.Log($"Initial base position set to: {basePosition}");
            Debug.Log("Initializing with Chibi icons.");
        }

        // Start with chibi icons
        characterInfo.SetSpriteType(CharacterInfo.SpriteTypes.Chibi);

        // Initialize previous values
        previousSpriteType = characterInfo.SpriteType;
        previousJob = characterInfo.Job;
        previousRolePosition = characterInfo.RolePosition;

        // Initial update
        UpdatePlayerSprite();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if SpriteType, Job, or RolePosition has changed
        if (previousSpriteType != characterInfo.SpriteType ||
            previousJob != characterInfo.Job ||
            previousRolePosition != characterInfo.RolePosition)
        {
            if (isDebugging)
            {
                Debug.Log($"SpriteType or job or role position changed: {previousSpriteType} -> {characterInfo.SpriteType}, {previousJob} -> {characterInfo.Job}, {previousRolePosition} -> {characterInfo.RolePosition}");
            }

            // Update sprite
            UpdatePlayerSprite();

            // Update stored values
            previousSpriteType = characterInfo.SpriteType;
            previousJob = characterInfo.Job;
            previousRolePosition = characterInfo.RolePosition;
        }
    }

    public void SetupPlayer(CharacterInfo.Jobs whichJob, CharacterInfo.RolePositions whichRolePos)
    {
        if (isDebugging)
        {
            Debug.Log($"Setting up player: Job = {whichJob}, Role Position = {whichRolePos}");
        }

        // Set Starting Info
        characterInfo.SetJob(whichJob);
        characterInfo.SetRolePosition(whichRolePos);

        // Update the sprite based on the info
        UpdatePlayerSprite();

    }

    public void MovePlayer(Vector3 goalPosition, float goalRotation, float duration)
    {
        if (isDebugging)
        {
            Debug.Log($"Starting to move player to: {goalPosition} with rotation: {goalRotation} over duration: {duration}");
        }

        // Ensure the Z position remains fixed
        goalPosition = new Vector3(goalPosition.x, goalPosition.y, zPosition);

        // Initialize target values
        targetPosition = goalPosition;
        targetRotation = goalRotation;
        moveDuration = duration;
        moveStartTime = Time.time;

        // Start the coroutine to move the player
        StartCoroutine(MovePlayerCoroutine());
    }

    // Helper function to update player's facing direction based on position relative to the boss
    private void UpdateFacingDirection()
    {
        Vector3 playerPos = transform.position;
        Vector3 bossPos = GetBossPosition();

        if (playerPos.x < bossPos.x)
        {
            // Player is to the left of the boss, so face right
            characterInfo.SetFacing(CharacterInfo.FacingDirections.Right);
        }
        else if (playerPos.x > bossPos.x)
        {
            // Player is to the right of the boss, so face left
            characterInfo.SetFacing(CharacterInfo.FacingDirections.Left);
        }

        // Update the sprite
        UpdatePlayerSprite();

        if (isDebugging)
        {
            Debug.Log($"Player position: {playerPos}, Boss position: {bossPos}");
            Debug.Log($"Player is now facing: {(playerPos.x < bossPos.x ? "Right" : "Left")}");
        }
    }

    public void UpdatePlayerSprite()
    {
        Sprite selectedSprite = null;
        CharacterInfo.SpriteTypes spriteType = characterInfo.SpriteType;
        CharacterInfo.Jobs job = characterInfo.Job;
        CharacterInfo.RolePositions rolePosition = characterInfo.RolePosition;

        if (isDebugging)
        {
            Debug.Log($"Updating player sprite: SpriteType = {spriteType}, Job = {job}, RolePosition = {rolePosition}");
        }

        if (characterInfo != null)
        {
            selectedSprite = characterInfo.GetSprite(spriteType, job, rolePosition);
        }

        // Determine the sprite based on the job and sprite type
        Vector3 offset = Vector3.zero;

        if (spriteType == CharacterInfo.SpriteTypes.Chibi)
        {
            transform.localScale = new Vector3(characterInfo.ChibiXscale, characterInfo.ChibiYscale, 1);
            offset = new Vector3(characterInfo.ChibiXoffset, characterInfo.ChibiYoffset, 0f); // Apply Chibi offsets
        }
        else if (spriteType == CharacterInfo.SpriteTypes.Icon)
        {
            transform.localScale = new Vector3(characterInfo.IconXscale, characterInfo.IconYscale, 1);
            offset = new Vector3(characterInfo.IconXoffset, characterInfo.IconYoffset, 0f); // Apply Icon offsets
        }
        else if (spriteType == CharacterInfo.SpriteTypes.Role)
        {
            transform.localScale = new Vector3(characterInfo.RoleXscale, characterInfo.RoleYscale, 1);
            offset = new Vector3(characterInfo.RoleXoffset, characterInfo.RoleYoffset, 0f); // Apply Role offsets
        }

        // Apply the base position plus offset as the new local position
        transform.localPosition = basePosition + offset;

        if (isDebugging)
        {
            Debug.Log($"Applied offsets: Base position = {basePosition}, Offset = {offset}, New position = {transform.localPosition}");
        }

        // Flip the sprite if the player is facing right
        if (characterInfo.Facing == CharacterInfo.FacingDirections.Right)
        {
            // Flip the sprite by inverting the X scale
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // Ensure the sprite is not flipped when facing left
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (isDebugging)
        {
            Debug.Log($"Sprite facing: {characterInfo.Facing}");
        }

        // Set the sprite renderer to the selected sprite
        if (selectedSprite != null)
        {
            spriteRenderer.sprite = selectedSprite;
        }
        else
        {
            Debug.LogWarning($"No sprite found for job {job} and sprite type {spriteType}");
        }
    }

    private IEnumerator MovePlayerCoroutine()
    {
        Vector3 initialPosition = new Vector3(transform.position.x, transform.position.y, zPosition); // Ensure initial Z position is used
        Quaternion initialRotation = transform.rotation;

        float elapsedTime = 0f;

        if (isDebugging)
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

            yield return null; // Wait for the next frame
        }

        // Ensure final position and rotation are exactly the target values
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        // Set base position to new position, with correct Z position
        basePosition = new Vector3(transform.position.x, transform.position.y, zPosition);
        //Whenever player moves, update it's facing direction
        UpdateFacingDirection();
        if (isDebugging)
        {
            Debug.Log($"Player move complete: Final Position/Base Position = {transform.position}, Final Rotation = {transform.rotation.eulerAngles.z}");
        }
    }

    // Assuming you have a way to get the boss position
    private Vector3 GetBossPosition()
    {
        // Replace this with your actual boss position retrieval logic
        return Vector3.zero;
    }
    public Vector3 GetPlayerPosition() => transform.position;
    public CharacterInfo.RolePositions GetRolePosition() => characterInfo.RolePosition;
    public CharacterInfo.Jobs GetJob() => characterInfo.Job;
}
