using UnityEditor;
using UnityEngine;

public class GuidePointManager : MonoBehaviour
{
    [SerializeField] private GameObject guidePoint;
    [SerializeField] private string sortingLayerName = "GuidePoints";
    [SerializeField] private int sortingOrder = 0;
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitializeGuidePointManager()
    {

    }
    public void SetGuidePoints() 
    {
        // Get Fight info
        FightManager.FightEnum whichFight = GetGameManager().GetFightManager().GetCurrentFight();
        FightManager.FightGuide whichGuide = GetGameManager().GetFightManager().GetCurrentGuide();
        int whichAttack = GetGameManager().GetFightManager().GetCurrentAttack();
        int whichStep = GetGameManager().GetFightManager().GetCurrentStep();

        // M5S
        if (whichFight == FightManager.FightEnum.M5S)
        {
            string m5sAttack = GetGameManager().GetFightManager().GetAttackNameByInt(whichFight,whichAttack);
            // 
            if (whichGuide == FightManager.FightGuide.Hector)
            {
                // Setup
                if (m5sAttack == FightManager.M5SAttacks.Setup.ToString())
                {
                    // Light Parties
                    if (whichStep == 1)
                    {

                    }

                }
            }
        }

    }
    private void InitializeDebug() { debug = new DebugInfo(); }
    #region // References
    private GameManager GetGameManager() => gameManager;
    #endregion
}
