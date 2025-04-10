using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    #region // Definition
    CharacterManager.RolePositions playerRole;
    #endregion



    #region // Functions


    private CharacterManager.RolePositions GetPlayerRole() => playerRole;
    private void SetPlayerRole(CharacterManager.RolePositions newPlayerRole) => playerRole = newPlayerRole;

    #endregion
}
