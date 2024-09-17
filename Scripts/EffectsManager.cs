using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;

[System.Serializable]
public class WickedThunderEffects
{
    [SerializeField] private GameObject wickedThunderFX_BF_Beam1;
    [SerializeField] private GameObject wickedThunderFX_BF_Beam2;
    [SerializeField] private GameObject wickedThunderFX_BF_Beam3;
    [SerializeField] private GameObject wickedThunderFX_BF_Beam4;
    [SerializeField] private GameObject wickedThunderFX_BF_Beam5;

    // Expose these values via properties (optional)
    public GameObject WickedThunderFX_BF_Beam1 => wickedThunderFX_BF_Beam1;
    public GameObject WickedThunderFX_BF_Beam2 => wickedThunderFX_BF_Beam2;
    public GameObject WickedThunderFX_BF_Beam3 => wickedThunderFX_BF_Beam3;
    public GameObject WickedThunderFX_BF_Beam4 => wickedThunderFX_BF_Beam4;
    public GameObject WickedThunderFX_BF_Beam5 => wickedThunderFX_BF_Beam5;
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

    public void CreateFX(Effects whichFx, FxLifeTimeType lifetimeType, Vector3 position, float lifetimeTimer = 0, int lifetimeStep = 0)
    {
        GameObject effectInstance = null;

        // Determine which effect to instantiate
        switch (whichFx)
        {
            case Effects.WickedThunderBeam1:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam1, position, Quaternion.identity);
                break;
            case Effects.WickedThunderBeam2:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam2, position, Quaternion.identity);
                break;
            case Effects.WickedThunderBeam3:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam3, position, Quaternion.identity);
                break;            
            case Effects.WickedThunderBeam4:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam4, position, Quaternion.identity);
                break;            
            case Effects.WickedThunderBeam5:
                effectInstance = Instantiate(wickedThunderEffects.WickedThunderFX_BF_Beam5, position, Quaternion.identity);
                break;
            default:
                Debug.LogWarning("Unknown effect type");
                return;
        }

        // Handle effect's lifetime based on the chosen lifetime type
        if (effectInstance != null)
        {
            switch (lifetimeType)
            {
                case FxLifeTimeType.None:
                    // The effect stays indefinitely
                    break;

                case FxLifeTimeType.Timer:
                    // Destroy after a specified time (lifetimeTimer)
                    Destroy(effectInstance, lifetimeTimer);
                    break;

                case FxLifeTimeType.Step:
                    // Store the effect instance and its associated step for future destruction
                    StartCoroutine(HandleStepDestruction(effectInstance, lifetimeStep));
                    break;

                default:
                    Debug.LogWarning("Unknown lifetime type");
                    break;
            }
        }
    }

    private IEnumerator HandleStepDestruction(GameObject effectInstance, int lifetimeStep)
    {
        // Wait until the current step in the FightManager reaches or exceeds the lifetimeStep
        while (fm.GetCurrentStep() < lifetimeStep)
        {
            yield return null;  // Wait for the next frame
        }

        // Once the step is reached, destroy the effect
        Destroy(effectInstance);
    }
    
    public void CheckForFxDestroyStep()
    {
        // Get the current fight, attack, and step from FightManager
        FightManager.FightEnum currentFight = fm.GetCurrentFight();
        string currentAttack = fm.GetCurrentAttack();
        int currentStep = fm.GetCurrentStep();
    }
    #endregion
}
