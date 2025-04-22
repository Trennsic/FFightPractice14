using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static CastManager;
using static FightManager;
using static UiManager;
using static UnityEditor.PlayerSettings;

public class CastManager : MonoBehaviour
{
    public enum CastType { RealTime, Step }

    public enum CastDisplayType { CastBar, AttackBar }

    [Header("Cast Settings")]
    [SerializeField] private string currentAttack;
    [SerializeField] private CastType currentCastType;
    [SerializeField] private CastDisplayType currentDisplayType;
    [SerializeField] private float realTimeDuration;
    [SerializeField] private List<float> stepPausePoints = new List<float>();
    [SerializeField] private int currentStepIndex = 0;
    
    [SerializeField] private float castProgress = 0f;
    [SerializeField] private float castProgressTarget = 0f;
    
    [SerializeField] private Coroutine castRoutine;
    [SerializeField] private bool isCasting = false;

    [SerializeField] private Dictionary<FightManager.M5SAttacks, CastInfo> m5sCastInfo;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;

    void Update()
    {
        UpdateCastProgress();
    }

    // Initializes CastManager, sets up initial states
    public void InitializeCastManager()
    {
        InitializeDebug();
        InitializeCastsM5S();
        currentCastType = CastType.Step; // Default to Step
    }

    // Called to start casting an attack
    public void StartAttack(string attackText, CastType castType, CastDisplayType castDisplayType, float duration = 1f, List<float> stepPauses = null)
    {
        currentAttack = attackText;
        currentCastType = castType;
        realTimeDuration = duration;
        stepPausePoints = stepPauses ?? new List<float> { .5f };
        castProgress = 0f;
        castProgressTarget = 0f;
        currentStepIndex = 0;

        if (castRoutine != null)
            StopCoroutine(castRoutine);

        GetGameManager().GetUiManager().SetAttackText(attackText);
        //GetGameManager().GetUiManager().SetCastDisplayType(castDisplayType);
        castRoutine = StartCoroutine(HandleCastProgress());
    }

    

    // Updates the cast progress bar
    private void UpdateCastProgress()
    {
        //if (!isCasting) return;

        // Smooth progress bar movement
        castProgress = Mathf.MoveTowards(castProgress, castProgressTarget, Time.deltaTime);
        GetGameManager().GetUiManager().UpdateCastbarSlider(castProgress);
    }

    // Coroutine to handle cast progress
    private IEnumerator HandleCastProgress()
    {
        isCasting = true;

        switch (currentCastType)
        {
            case CastType.RealTime:
                float timer = 0f;
                while (timer < realTimeDuration)
                {
                    timer += Time.deltaTime;
                    castProgressTarget = Mathf.Clamp01(timer / realTimeDuration);
                    yield return null;
                }
                break;

            case CastType.Step:
                for (currentStepIndex = 0; currentStepIndex < stepPausePoints.Count; currentStepIndex++)
                {
                    castProgressTarget = stepPausePoints[currentStepIndex];
                    yield return new WaitUntil(() => Mathf.Abs(castProgress - castProgressTarget) < 0.01f);
                    yield return new WaitForSeconds(0.2f); // Optional pause at step
                }
                break;
        }

        // After finishing the cast
        yield return new WaitForSeconds(0.5f);
        CastTimeFinished();
    }
    // After cast completes, update UI and reset state
    private void CastTimeFinished()
    {
        isCasting = false;
        GetGameManager().GetUiManager().SetCastDisplayType(CastDisplayType.AttackBar);
        Debug.Log($"Cast Complete: {currentAttack}");
    }

    // Progress to the next step in step-based casting
    public void ProgressStep()
    {
        if (currentCastType != CastType.Step || currentStepIndex >= stepPausePoints.Count)
            return;

        castProgressTarget = stepPausePoints[currentStepIndex];
        currentStepIndex++;
    }
    public void UpdateAttackInfo()
    {
        // Get managers
        var gm = GetGameManager();
        var fm = gm.GetFightManager();
        var um = gm.GetUiManager();

        // Get fight status
        var fight = fm.GetCurrentFight();
        var guide = fm.GetCurrentGuide();
        int attack = fm.GetCurrentAttack();
        int step = fm.GetCurrentStep();

        

        // Only handle M5S Hector for now
        if (fight == FightManager.FightEnum.M5S && guide == FightManager.FightGuide.Hector)
        {
            var attackEnum = (FightManager.M5SAttacks)attack;

            if (attackEnum == M5SAttacks.Setup)
            {
                //um.SetCastDisplayType(CastDisplayType.AttackBar);
            }

            // Try to get the CastInfo for this attack
            if (!m5sCastInfo.TryGetValue(attackEnum, out CastInfo castInfo))
            {
                Debug.LogWarning($"No CastInfo found for attack {attackEnum}");
                return;
            }

            List<string> stepNames = castInfo.stepNames;

            #region// Handle dynamic step names (e.g., permutations)
            switch (attackEnum)
            {
                case FightManager.M5SAttacks.Flip_AB_1:
                    var permFlip1 = fm.GetPermutationForAttack(attackEnum);
                    string flipName1 = (permFlip1.value == 1) ? "Flip to A side" :
                                      (permFlip1.value == 2) ? "Flip to B side" : "Flip ???";
                    stepNames = Enumerable.Repeat(flipName1, castInfo.stepNames.Count).ToList();
                    break;
                case FightManager.M5SAttacks.Flip_AB_2:
                    var permFlip2 = fm.GetPermutationForAttack(attackEnum);
                    string flipName2 = (permFlip2.value == 1) ? "Flip to A side" :
                                      (permFlip2.value == 2) ? "Flip to B side" : "Flip ???";
                    stepNames = Enumerable.Repeat(flipName2, castInfo.stepNames.Count).ToList();
                    break;

                case FightManager.M5SAttacks.Out_In_1:
                    var permOutIn = fm.GetPermutationForAttack(attackEnum);
                    string outInName = (permOutIn.value == 1) ? "Inside Out" :
                                       (permOutIn.value == 2) ? "Outside In" : "Out/In ???";
                    stepNames = Enumerable.Repeat(outInName, castInfo.stepNames.Count).ToList();
                    break;

                case FightManager.M5SAttacks.Snap_Twist_1:
                    var permSnap1 = fm.GetPermutationForAttack(attackEnum);
                    string snapText1 = permSnap1.value switch
                    {
                        1 => "2-Snap Twist & Drop the Needle",
                        2 => "3-Snap Twist & Drop the Needle",
                        3 => "4-Snap Twist & Drop the Needle",
                        4 => "2-Snap Twist & Drop the Needle",
                        5 => "3-Snap Twist & Drop the Needle",
                        6 => "4-Snap Twist & Drop the Needle",
                        _ => "Snap ???"
                    };
                    stepNames = Enumerable.Repeat(snapText1, castInfo.stepNames.Count).ToList();
                    break;
                case FightManager.M5SAttacks.Snap_Twist_2:
                    var permSnap2 = fm.GetPermutationForAttack(attackEnum);
                    string snapText2 = permSnap2.value switch
                    {
                        1 => "2-Snap Twist & Drop the Needle",
                        2 => "3-Snap Twist & Drop the Needle",
                        3 => "4-Snap Twist & Drop the Needle",
                        4 => "2-Snap Twist & Drop the Needle",
                        5 => "3-Snap Twist & Drop the Needle",
                        6 => "4-Snap Twist & Drop the Needle",
                        _ => "Snap ???"
                    };
                    stepNames = Enumerable.Repeat(snapText2, castInfo.stepNames.Count).ToList();
                    break;
            }
            #endregion

            // Clamp step index to valid range
            int clampedStep = Mathf.Clamp(step, 0, stepNames.Count - 1);

            // Update UI text and cast bar
            string attackText = stepNames[clampedStep];
            um.SetAttackText(attackText);

            currentAttack = attack.ToString();
            currentStepIndex = step;

            // Determine cast behavior
            currentCastType = castInfo.castType;
            currentDisplayType = castInfo.displayType;
            stepPausePoints = castInfo.castBarPauses;
            realTimeDuration = castInfo.castTimeDuration;
            
            castProgressTarget = 0f;

            // Only Reset cast progress if it's the first step of an attack
            bool isNewAttack = (currentStepIndex <= 0);
            if (isNewAttack) { castProgress = 0f; }

            // Set the UI to correct display type
            um.SetCastDisplayType(currentDisplayType, isNewAttack);

            // Cancel any existing coroutine
            if (castRoutine != null)
                StopCoroutine(castRoutine);

            // Start cast logic
            if (currentCastType == CastType.RealTime)
            {
                castRoutine = StartCoroutine(HandleCastProgress());
            }
            else if (currentCastType == CastType.Step)
            {
                // Manually set the target for this step
                if (stepPausePoints != null && stepPausePoints.Count > clampedStep)
                    castProgressTarget = stepPausePoints[clampedStep];
                else
                    castProgressTarget = 1f; // fallback
            }
        }
    }
    public void InitializeCastsM5S()
    {
        m5sCastInfo = new Dictionary<FightManager.M5SAttacks, CastInfo>();

        void AddCast(FightManager.M5SAttacks attack, List<string> stepNames, List<float> pauses, float castDuration, float fullDuration, CastType castType, CastDisplayType displayType)
        {
            m5sCastInfo[attack] = new CastInfo(stepNames, pauses, castDuration, fullDuration, castType, displayType);
        }

        AddCast(FightManager.M5SAttacks.Setup,
            new List<string> { "Setup - Clock Positions", "Setup - Light Parties", "Setup - Light Parties", "Setup - Color Partners", "Setup - Color Partners" },
            new List<float> { 0f, 0.3f, 0.6f, 0.9f, 1f },
            -1f, -1f,
            CastType.Step,
            CastDisplayType.AttackBar);

        AddCast(FightManager.M5SAttacks.Deep_Cut_1,
            new List<string> { "Deep Cut", "Deep Cut" },
            new List<float> { 0.5f, 1f },
            2f, 2f,
            CastType.Step,
            CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Flip_AB_1,
            new List<string> { "Flip to A side" }, // dynamic
            new List<float> { 0.9f },
            2f, 2f,
            CastType.Step,
            CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Snap_Twist_1,
            new List<string> { "Snap Twist" },
            new List<float> { 0.5f, 1f, 1f },
            2f, 2f,
            CastType.Step, CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Celebrate_1,
            new List<string> { "Celebrate Good Times" },
            new List<float> { 0.5f },
            1f, 1f,
            CastType.RealTime, CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Disco_Infernal_1,
            new List<string> { "Disco Infernal" },
            new List<float> { 0.5f },
            1f, 1f,
            CastType.RealTime, CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Funky_Floor_1,
            new List<string> { "Funky Floor", "Funky Floor", "Funky Floor" },
            new List<float> { 0.9f, 1f, 1f },
            1f, 1f,
            CastType.Step, CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Out_In_1,
            new List<string> { "Funky Floor", "Funky Floor", "Funky Floor" }, // dynamic
            new List<float> { 0.9f, 1f, 1f },
            1f, 1f,
            CastType.Step, CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Flip_AB_2,
            new List<string> { "Flip to A side", "Flip to A side", "Flip to A side" }, // dynamic
            new List<float> { 0.9f, 1f, 1f },
            2f, 2f,
            CastType.Step, CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Snap_Twist_2,
            new List<string> { "Snap Twist" },
            new List<float> { 0.5f, 1f, 1f },
            2f, 2f,
            CastType.Step, CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Celebrate_2,
            new List<string> { "Celebrate Good Times" },
            new List<float> { 0.5f },
            1f, 1f,
            CastType.RealTime, CastDisplayType.CastBar);

        AddCast(FightManager.M5SAttacks.Deep_Cut_2,
            new List<string> { "Deep Cut", "Deep Cut" },
            new List<float> { 0.5f, 1f },
            2f, 2f,
            CastType.Step, CastDisplayType.CastBar);
    }

    public void SetCastProgressTarget(float duration, List<float> stepPauses)
    {
        if (duration > 0f)
        {
            currentCastType = CastType.RealTime;
            realTimeDuration = duration;
            stepPausePoints = new List<float> { 1f };
        }
        else
        {
            currentCastType = CastType.Step;
            realTimeDuration = -1f;
            stepPausePoints = stepPauses ?? new List<float> { 0.5f };
        }

        castProgressTarget = 0f;
        castProgress = 0f;
        currentStepIndex = 0;

        if (castRoutine != null)
            StopCoroutine(castRoutine);

        castRoutine = StartCoroutine(HandleCastProgress());
    }


    private void InitializeDebug()
    {
        debug = new DebugInfo();
    }

    private GameManager GetGameManager() => gameManager;
}
public class CastInfo
{
    public List<string> stepNames { get; private set; }
    public List<float> castBarPauses { get; private set; }
    public float castTimeDuration { get; private set; }
    public float fullTimeDuration { get; private set; }
    public CastType castType { get; private set; }  // ← Added
    public CastDisplayType displayType { get; private set; }  // ← Added

    public int StepCount => stepNames?.Count ?? 0;

    public CastInfo(
        List<string> stepNames,
        List<float> castBarPauses,
        float castTimeDuration,
        float fullTimeDuration,
        CastType castType,
        CastDisplayType displayType)  // ← Added
    {
        this.stepNames = stepNames;
        this.castBarPauses = castBarPauses;
        this.castTimeDuration = castTimeDuration;
        this.fullTimeDuration = fullTimeDuration;
        this.castType = castType;  // ← Added
        this.displayType = displayType;
    }

    // Optional: get step name safely
    public string GetStepName(int stepIndex)
    {
        if (stepNames == null || stepIndex < 0 || stepIndex >= stepNames.Count)
            return "";
        return stepNames[stepIndex];
    }
}


