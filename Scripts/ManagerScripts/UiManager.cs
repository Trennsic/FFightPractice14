using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static CastManager;

public class UiManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject castBar;
    [SerializeField] private GameObject attackBar;

    [SerializeField] private Slider castbarSlider;
    [SerializeField] private TextMeshProUGUI castbarText;
    [SerializeField] private TextMeshProUGUI attackText;

    // NEW: drag & drop the CanvasGroups for your text objects
    [Header("Separate Text CanvasGroups")]
    [SerializeField] private CanvasGroup castbarTextGroup;
    [SerializeField] private CanvasGroup attackTextGroup;

    #region References 
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DebugInfo debug;
    #endregion

    

    private CastDisplayType currentCastDisplay;

    void Start()
    {
        InitializeUiManager();
    }

    public void InitializeUiManager()
    {
        if (castBar == null)
        {
            Debug.LogError("CastBar GameObject is not assigned in UiManager.");
        }
        else
        {
            castbarSlider = castBar.GetComponentInChildren<Slider>();
            castbarText = castBar.GetComponentInChildren<TextMeshProUGUI>();

            if (castbarSlider == null)
                Debug.LogError("Slider not found in CastBar GameObject.");
            if (castbarText == null)
                Debug.LogError("TextMeshProUGUI not found in CastBar GameObject.");
        }

        if (attackBar == null)
        {
            Debug.LogError("AttackBar GameObject is not assigned in UiManager.");
        }
        else
        {
            attackText = attackBar.GetComponentInChildren<TextMeshProUGUI>();
            if (attackText == null)
                Debug.LogError("TextMeshProUGUI not found in AttackBar GameObject.");
        }

        SetCastDisplayType(CastDisplayType.AttackBar);
    }
    public void SetAttackText(string text)
    {
        if (castbarText != null)
            castbarText.text = text;

        if (attackText != null)
            attackText.text = text;
    }

    public void SetCastDisplayType(CastDisplayType targetDisplay, bool isNewAttack = false, float fadeDuration = .25f)
    {
        targetDisplay = CastDisplayType.CastBar;
        bool modeChanged = (targetDisplay != currentCastDisplay);
        bool toggleSlider = modeChanged || isNewAttack;

        // 1) Slider panel toggling (no fade)
        if (toggleSlider)
        {
            castBar.SetActive(targetDisplay == CastDisplayType.CastBar);
            attackBar.SetActive(targetDisplay == CastDisplayType.AttackBar);
            currentCastDisplay = targetDisplay;
        }

        // 2) Text fading (only on new attack)
        if (isNewAttack)
        {
            // choose which text to fade in
            var incomingText = (targetDisplay == CastDisplayType.CastBar)
                ? castbarText
                : attackText;

            // reset alpha instantly
            incomingText.canvasRenderer.SetAlpha(0f);
            // then fade up
            incomingText.CrossFadeAlpha(1f, fadeDuration, false);
        }


    }

    private IEnumerator TransitionUI(
        CastDisplayType targetDisplay,
        float duration,
        bool toggleSlider,
        bool fadeText
    )
    {
        // 1) Panel canvas‑groups (for slider vs attack bar)
        var castBarCG = castBar.GetComponent<CanvasGroup>() ?? castBar.AddComponent<CanvasGroup>();
        var attackBarCG = attackBar.GetComponent<CanvasGroup>() ?? attackBar.AddComponent<CanvasGroup>();

        // 2) Text canvas‑groups on the existing TMP objects
        var castTextCG = castbarText.gameObject.GetComponent<CanvasGroup>()
                           ?? castbarText.gameObject.AddComponent<CanvasGroup>();
        var attackTextCG = attackText.gameObject.GetComponent<CanvasGroup>()
                           ?? attackText.gameObject.AddComponent<CanvasGroup>();

        bool toCastBar = (targetDisplay == CastDisplayType.CastBar);
        float elapsed = 0f;

        // Activate incoming slider panel if needed
        if (toggleSlider)
        {
            if (toCastBar && !castBar.activeSelf) castBar.SetActive(true);
            if (!toCastBar && !attackBar.activeSelf) attackBar.SetActive(true);
        }

        // Prep text fade: if we're fading in new text, zero its alpha & ensure active
        if (fadeText)
        {
            var incomingTextCG = toCastBar ? castTextCG : attackTextCG;
            incomingTextCG.alpha = 0f;
            incomingTextCG.gameObject.SetActive(true);
        }

        // Cross‑fade over duration
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            if (toggleSlider)
            {
                castBarCG.alpha = toCastBar ? t : 1f - t;
                attackBarCG.alpha = toCastBar ? 1f - t : t;
            }

            if (fadeText)
            {
                var incomingTextCG = toCastBar ? castTextCG : attackTextCG;
                incomingTextCG.alpha = t;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Finalize slider panels
        if (toggleSlider)
        {
            castBarCG.alpha = toCastBar ? 1f : 0f;
            attackBarCG.alpha = toCastBar ? 0f : 1f;

            if (toCastBar) attackBar.SetActive(false);
            else castBar.SetActive(false);

            currentCastDisplay = targetDisplay;
        }

        // Finalize text fade
        if (fadeText)
        {
            var incomingTextCG = toCastBar ? castTextCG : attackTextCG;
            incomingTextCG.alpha = 1f;
        }
    }




    public void UpdateCastbarSlider(float value)
    {
        if (castbarSlider != null)
            castbarSlider.value = Mathf.Clamp01(value);
    }



    private void InitializeDebug()
    {
        debug = new DebugInfo();
    }

    #region Helper
    private GameManager GetGameManager() => gameManager;
    #endregion
}
