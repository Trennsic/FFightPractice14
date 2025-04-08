using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterAddon : MonoBehaviour
{
    CharacterManager characterManager;
    
    [SerializeField] private CharacterManager.CharacterType characterType;
    [SerializeField] private CharacterManager.SpriteTypes spriteType;
    [SerializeField] private CharacterManager.Roles role;
    [SerializeField] private CharacterManager.Jobs job;
    [SerializeField] private CharacterManager.RolePositions rolePosition;
    [SerializeField] private CharacterManager.FacingDirections facing;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 basePosition;
    [SerializeField] private float targetRotation;
    [SerializeField] private float moveDuration;
    [SerializeField] private float moveStartTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitializeCharacter(CharacterManager characterManager)
    {
        SetCharacterManager(characterManager);
        #region // Get or add the SpriteRenderer component

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        #endregion
    }
    public void UpdateCharacterInfo(CharacterManager.Jobs job, CharacterManager.RolePositions rolePosition)
    {
        //
        SetRolePosition(rolePosition);
        SetJob(job);

        // Set role
        switch (GetRolePosition())
        {
            case CharacterManager.RolePositions.Mt:
            case CharacterManager.RolePositions.Ot:
                SetRole(CharacterManager.Roles.Tank);
                break;
            case CharacterManager.RolePositions.H1:
            case CharacterManager.RolePositions.H2:
                SetRole(CharacterManager.Roles.Healer);
                break;
            case CharacterManager.RolePositions.M1:
            case CharacterManager.RolePositions.M2:
                SetRole(CharacterManager.Roles.MeleeDps);
                break;
            case CharacterManager.RolePositions.R1:
            case CharacterManager.RolePositions.R2:
                SetRole(CharacterManager.Roles.RangedDps);
                break;
        }
    }
    public void MoveCharacter(Vector3 goalPosition, float goalRotation = 0, float duration = .1f)
    {
        //if (debugInfo.GetIsDebugging())
        //{
        //    Debug.Log($"MoveNpc called with goalPosition: {goalPosition}, goalRotation: {goalRotation}, duration: {duration}");
        //}

        // Ensure the Z position remains fixed
        goalPosition = new Vector3(goalPosition.x, goalPosition.y, 0);

        // Initialize target values
        targetPosition = goalPosition;
        targetRotation = goalRotation;
        moveDuration = duration;
        moveStartTime = Time.time;

        //if (debugInfo.GetIsDebugging())
        //{
        //    Debug.Log($"Initialized move parameters. targetPosition: {targetPosition}, targetRotation: {targetRotation}, moveDuration: {moveDuration}, moveStartTime: {moveStartTime}");
        //}

        // Start the coroutine to move the player
        StartCoroutine(MoveCharacterCoroutine());
    }

    private IEnumerator MoveCharacterCoroutine()
    {
        Vector3 initialPosition = new Vector3(transform.position.x, transform.position.y, 0); // Ensure initial Z position is used
        Quaternion initialRotation = transform.rotation;

        float elapsedTime = 0f;

        //if (debugInfo.GetIsDebugging())
        //{
        //    Debug.Log($"Starting player move coroutine: Initial Position = {initialPosition}, Target Position = {targetPosition}");
        //}

        while (elapsedTime < moveDuration)
        {
            elapsedTime = Time.time - moveStartTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // Interpolate position and rotation
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(0, 0, targetRotation), t);

            //if (debugInfo.GetIsDebugging())
            //{
            //    Debug.Log($"Moving NPC. Elapsed time: {elapsedTime}, t: {t}, Current Position: {transform.position}");
            //}

            yield return null; // Wait for the next frame
        }

        // Ensure final position and rotation are exactly the target values
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        // Set base position to new position, with correct Z position
        basePosition = new Vector3(transform.position.x, transform.position.y, 0);

        //if (debugInfo.GetIsDebugging())
        //{
        //    Debug.Log($"Player move complete: Final Position/Base Position = {transform.position}, Final Rotation = {transform.rotation.eulerAngles.z}");
        //}
    }

    public void UpdateSprite()
    {
        
        CharacterManager.SpriteTypes spriteType = GetSpriteType();
        CharacterManager.Jobs job = GetJob();
        CharacterManager.RolePositions rolePosition = GetRolePosition();
        Sprite selectedSprite = GetCharacterManager().GetSprite(spriteType, job, rolePosition );

        //if (debugInfo.GetIsDebugging())
        //{
        //    Debug.Log($"Updating NPC sprite: SpriteType = {spriteType}, Job = {job}, RolePosition = {rolePosition}");
        //}


        // Determine the sprite based on the job and sprite type
        Vector3 offset = Vector3.zero;

        if (spriteType == CharacterManager.SpriteTypes.Chibi)
        {
            transform.localScale = new Vector3(GetCharacterManager().GetChibiXscale(), GetCharacterManager().GetChibiYscale(), 1);
            offset = new Vector3(GetCharacterManager().GetChibiXoffset(), GetCharacterManager().GetChibiYoffset(), 0f); // Apply Chibi offsets
        }
        else if (spriteType == CharacterManager.SpriteTypes.Icon)
        {
            transform.localScale = new Vector3(GetCharacterManager().GetIconXscale(), GetCharacterManager().GetIconYscale(), 1);
            offset = new Vector3(GetCharacterManager().GetIconXoffset(), GetCharacterManager().GetIconYoffset(), 0f); // Apply Icon offsets
        }
        else if (spriteType == CharacterManager.SpriteTypes.Role)
        {
            transform.localScale = new Vector3(GetCharacterManager().GetRoleXscale(), GetCharacterManager().GetRoleYscale(), 1);
            offset = new Vector3(GetCharacterManager().GetRoleXoffset(), GetCharacterManager().GetRoleYoffset(), 0f); // Apply Role offsets
        }

        // Apply the base position plus offset as the new local position
        transform.localPosition = basePosition + offset;

        //if (debugInfo.GetIsDebugging())
        //{
        //    Debug.Log($"Applied offsets: Base position = {basePosition}, Offset = {offset}, New position = {transform.localPosition}");
        //}

        // Flip the sprite if the player is facing right
        if (GetFacingDirection() == CharacterManager.FacingDirections.Right)
        {
            // Flip the sprite by inverting the X scale
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // Ensure the sprite is not flipped when facing left
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        //if (debugInfo.GetIsDebugging())
        //{
        //    Debug.Log($"Sprite facing: {GetCharacterManager().Facing}");
        //}

        // Set the sprite renderer to the selected sprite
        if (selectedSprite != null)
        {
            spriteRenderer.sprite = selectedSprite;
            //if (debugInfo.GetIsDebugging())
            //{
            //    Debug.Log($"Sprite set to {selectedSprite.name}");
            //}
        }
        else
        {
            Debug.LogWarning($"No sprite found for job {job} and sprite type {spriteType}");
        }
        //Update facing
        //UpdateFacingDirection();
    }
    public void SetCharacterType(CharacterManager.CharacterType newCharacterType) => characterType = newCharacterType;
    public CharacterManager.CharacterType GetCharacterType() => characterType;
    public void SetSpriteType(CharacterManager.SpriteTypes newSpriteType) => spriteType = newSpriteType;
    public CharacterManager.SpriteTypes GetSpriteType() => spriteType;
    public void SetRole(CharacterManager.Roles newRole) => role = newRole;
    public CharacterManager.Roles GetRole() => role;
    public void SetJob(CharacterManager.Jobs newJob) => job = newJob;
    public CharacterManager.Jobs GetJob() => job;
    public void SetRolePosition(CharacterManager.RolePositions newRolePosition) => rolePosition = newRolePosition;
    public CharacterManager.RolePositions GetRolePosition() => rolePosition;
    public void SetFacingDirection(CharacterManager.FacingDirections newFacingDirection) => facing = newFacingDirection;
    public CharacterManager.FacingDirections GetFacingDirection() => facing;
    public void SetCharacterManager(CharacterManager newCharacterManager) => characterManager = newCharacterManager;
    public CharacterManager GetCharacterManager() => characterManager;
}
