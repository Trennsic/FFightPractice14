using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;
using static FightManager;

[System.Serializable]
public class WickedThunderEffects
{
    [SerializeField] private GameObject wickedThunderFX_BF_Beam1;
    [SerializeField] private GameObject wickedThunderFX_BF_Beam2;
    [SerializeField] private GameObject wickedThunderFX_BF_Beam3;
    [SerializeField] private GameObject wickedThunderFX_BF_Beam4;
    [SerializeField] private GameObject wickedThunderFX_BF_Beam5;
    [SerializeField] private GameObject wickedThunderFX_BF_Electromine;
    [SerializeField] private GameObject wickedThunderFX_BF_Groundline;
    [SerializeField] private GameObject wickedThunderFX_BF_GlFlare;
    [SerializeField] private GameObject wickedThunderFX_BF_FourStar;
    [SerializeField] private GameObject wickedThunderFX_BF_Near;
    [SerializeField] private GameObject wickedThunderFX_BF_Far;

    // Expose these values via properties (optional)
    public GameObject WickedThunderFX_BF_Beam1 => wickedThunderFX_BF_Beam1;
    public GameObject WickedThunderFX_BF_Beam2 => wickedThunderFX_BF_Beam2;
    public GameObject WickedThunderFX_BF_Beam3 => wickedThunderFX_BF_Beam3;
    public GameObject WickedThunderFX_BF_Beam4 => wickedThunderFX_BF_Beam4;
    public GameObject WickedThunderFX_BF_Beam5 => wickedThunderFX_BF_Beam5;
    public GameObject WickedThunderFX_BF_Electromine => wickedThunderFX_BF_Electromine;
    public GameObject WickedThunderFX_BF_Groundline => wickedThunderFX_BF_Groundline;
    public GameObject WickedThunderFX_BF_GlFlare => wickedThunderFX_BF_GlFlare;
    public GameObject WickedThunderFX_BF_FourStar => wickedThunderFX_BF_FourStar;
    public GameObject WickedThunderFX_BF_Near => wickedThunderFX_BF_Near;
    public GameObject WickedThunderFX_BF_Far => wickedThunderFX_BF_Far;
}


public class EffectsManager : MonoBehaviour
{
    #region // Definitions
    public enum Effects
    {
        WickedThunderBeam1,
        WickedThunderBeam2,
        WickedThunderBeam3,
        WickedThunderBeam4,
        WickedThunderBeam5,
        WickedThunderElectromine,
        WickedThunderGroundline,
        WickedThunderGlFlare,
        WickedThunderFourStar,
        WickedThunderFarMark,
        WickedThunderNearMark,
    }
    [SerializeField] private Effects whichEffects;
    public enum FxLifeTimeType
    {
        None, 
        Timer,
        Step
    }
    [SerializeField] private GameObject Manager;
    [SerializeField] private FightManager fm;

    // Declare an instance of WickedThunderEffects
    [SerializeField] private WickedThunderEffects wickedThunderEffects;

    [SerializeField] private bool IsDebugging;

    #endregion
    #region // Functions
    // Start is called before the first frame update
    void Start()
    {
        if (Manager != null)
        {
            if (fm == null)
            {
                fm = Manager.GetComponent<FightManager>();
            } 
            else
            {
                Debug.LogWarning("Unable to find Fight manager");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject CreateFX( Effects whichFx, FxLifeTimeType lifetimeType, Vector3 position, float lifetimeTimer = 0,
    FightEnum? lifetimeFight = null, string lifetimeAttack = null, int lifetimeStep = 0)
    {
        if (IsDebugging)
        {
            Debug.Log($"CreateFX called with parameters: {whichFx}, {lifetimeType}, {position}, {lifetimeTimer}, {lifetimeFight}, {lifetimeAttack}, {lifetimeStep}");
        }

        GameObject effectInstance = null;

        // Determine which effect to instantiate
        switch (whichFx)
        {
            case Effects.WickedThunderBeam1:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam1, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Beam1");
                break;
            case Effects.WickedThunderBeam2:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam2, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Beam2");
                break;
            case Effects.WickedThunderBeam3:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam3, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Beam3");
                break;
            case Effects.WickedThunderBeam4:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam4, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Beam4");
                break;
            case Effects.WickedThunderBeam5:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam5, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Beam5");
                break;
            case Effects.WickedThunderElectromine:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Electromine, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Electromine");
                break;
            case Effects.WickedThunderGroundline:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Groundline, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Groundline");
                break;
            case Effects.WickedThunderGlFlare:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_GlFlare, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_GlFlare");
                break;
            case Effects.WickedThunderFourStar:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_FourStar, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_FourStar");
                break;
            case Effects.WickedThunderNearMark:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Near, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Near");
                break;
            case Effects.WickedThunderFarMark:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Far, position, Quaternion.identity);
                if (IsDebugging) Debug.Log("Instantiated WickedThunderFX_BF_Far");
                break;
            default:
                Debug.LogWarning("Unknown effect type");
                return effectInstance;
        }

        // Handle effect's lifetime based on the chosen lifetime type
        if (effectInstance != null)
        {
            if (IsDebugging) Debug.Log("Effect instance created successfully.");

            switch (lifetimeType)
            {
                case FxLifeTimeType.None:
                    if (IsDebugging) Debug.Log("Lifetime type is None, effect will stay indefinitely.");
                    break;

                case FxLifeTimeType.Timer:
                    if (IsDebugging) Debug.Log($"Lifetime type is Timer, effect will be destroyed in {lifetimeTimer} seconds.");
                    Destroy(effectInstance, lifetimeTimer);
                    break;

                case FxLifeTimeType.Step:
                    if (IsDebugging)
                    {
                        Debug.Log("Lifetime type is Step, creating CurrentFightInfo and starting step destruction.");
                        Debug.Log($"Setting CurrentFight: {lifetimeFight}, CurrentAttack: {lifetimeAttack}, CurrentStep: {lifetimeStep}");
                    }
                    CurrentFightInfo lifeTimeFightInfo = new CurrentFightInfo();
                    lifeTimeFightInfo.SetCurrentFight(lifetimeFight.Value);
                    lifeTimeFightInfo.SetCurrentAttack(lifetimeAttack);
                    lifeTimeFightInfo.SetCurrentStep(lifetimeStep);
                    StartCoroutine(HandleStepDestruction(effectInstance, lifeTimeFightInfo));
                    break;

                default:
                    Debug.LogWarning("Unknown lifetime type");
                    break;
            }
        }
        return effectInstance;
    }



    private IEnumerator HandleStepDestruction(GameObject effectInstance, CurrentFightInfo lifeTimeFightInfo)
    {
        if (IsDebugging)
        {
            Debug.Log($"HandleStepDestruction started with: \n" +
                      $"Fight: {lifeTimeFightInfo.CurrentFight}, \n" +
                      $"Attack: {lifeTimeFightInfo.CurrentAttack}, \n" +
                      $"Step: {lifeTimeFightInfo.CurrentStep}");
        }

        // Simulate pulling current fight, attack, and step
        CurrentFightInfo finfo = new CurrentFightInfo();
        finfo.SetCurrentFight(fm.GetCurrentFight());
        finfo.SetCurrentAttack(fm.GetCurrentAttack());
        finfo.SetCurrentStep(fm.GetCurrentStep());

        FightEnum? currentFight = finfo.CurrentFight;
        string currentAttack = finfo.CurrentAttack;
        int currentStep = finfo.CurrentStep;

        if (IsDebugging)
        {
            Debug.Log($"Current state pulled from fm: \n" +
                      $"Fight: {currentFight}, \n" +
                      $"Attack: {currentAttack}, \n" +
                      $"Step: {currentStep}");
        }

        bool fightsMatch = (lifeTimeFightInfo.CurrentFight == currentFight);
        bool attacksMatch = (lifeTimeFightInfo.CurrentAttack == currentAttack);
        bool stepsMatch = (lifeTimeFightInfo.CurrentStep == currentStep);

        if (IsDebugging)
        {
            Debug.Log($"Initial match check: \n" +
                      $"FightsMatch: {fightsMatch}, \n" +
                      $"AttacksMatch: {attacksMatch}, \n" +
                      $"StepsMatch: {stepsMatch}");
        }

        // Check for matches in a loop
        while (fightsMatch && attacksMatch && stepsMatch)
        {
            if (IsDebugging)
            {
                Debug.Log($"Match in progress: \n" +
                          $"Fight: {lifeTimeFightInfo.CurrentFight} == {currentFight}, \n" +
                          $"Attack: {lifeTimeFightInfo.CurrentAttack} == {currentAttack}, \n" +
                          $"Step: {lifeTimeFightInfo.CurrentStep} == {currentStep}");
                Debug.Log("Step matches, continuing...");
            }

            // Update matches in each iteration
            finfo.SetCurrentFight(fm.GetCurrentFight());
            finfo.SetCurrentAttack(fm.GetCurrentAttack());
            finfo.SetCurrentStep(fm.GetCurrentStep());
            currentFight = finfo.CurrentFight;
            currentAttack = finfo.CurrentAttack;
            currentStep = finfo.CurrentStep;

            fightsMatch = (lifeTimeFightInfo.CurrentFight == currentFight);
            attacksMatch = (lifeTimeFightInfo.CurrentAttack == currentAttack);
            stepsMatch = (lifeTimeFightInfo.CurrentStep == currentStep);

            yield return null;  // Wait for the next frame

            if (IsDebugging)
            {
                Debug.Log($"Updated matches after frame: \n" +
                          $"FightsMatch: {fightsMatch}, \n" +
                          $"AttacksMatch: {attacksMatch}, \n" +
                          $"StepsMatch: {stepsMatch}");
            }
        }

        if (IsDebugging)
        {
            Debug.Log("Step no longer matches, destroying the effect.");
        }

        // Once the step is reached, destroy the effect
        Destroy(effectInstance);
    }


    public void SetIsDebugging(bool debugging) {  IsDebugging = debugging; }
    public bool GetIsDebugging() { return IsDebugging; }
    
    public void CheckForFxDestroyStep()
    {
        // Get the current fight, attack, and step from FightManager
        FightManager.FightEnum currentFight = fm.GetCurrentFight();
        string currentAttack = fm.GetCurrentAttack();
        int currentStep = fm.GetCurrentStep();
    }
    #endregion
}
