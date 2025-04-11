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

    public void SetCastDisplayType(CastDisplayType targetDisplay, float fadeDuration = 1)
    {
        StopAllCoroutines(); // Stop any existing fade transitions
        StartCoroutine(FadeAndToggleUiElements(targetDisplay, fadeDuration));
    }

    private IEnumerator FadeAndToggleUiElements(CastDisplayType targetDisplay, float duration)
    {
        float elapsedTime = 0f;

        CanvasGroup castBarGroup = castBar.GetComponent<CanvasGroup>();
        CanvasGroup attackBarGroup = attackBar.GetComponent<CanvasGroup>();

        if (castBarGroup == null) castBarGroup = castBar.AddComponent<CanvasGroup>();
        if (attackBarGroup == null) attackBarGroup = attackBar.AddComponent<CanvasGroup>();

        bool toCastBar = targetDisplay == CastDisplayType.CastBar;

        // Only activate the one that needs to fade in
        if (toCastBar && !castBar.activeSelf)
            castBar.SetActive(true);
        else if (!toCastBar && !attackBar.activeSelf)
            attackBar.SetActive(true);

        while (elapsedTime < duration)
        {
            float alpha = elapsedTime / duration;

            castBarGroup.alpha = toCastBar ? alpha : 1f - alpha;
            attackBarGroup.alpha = toCastBar ? 1f - alpha : alpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final alpha values
        castBarGroup.alpha = toCastBar ? 1f : 0f;
        attackBarGroup.alpha = toCastBar ? 0f : 1f;

        // Deactivate the one that faded out
        if (toCastBar)
            attackBar.SetActive(false);
        else
            castBar.SetActive(false);

        currentCastDisplay = targetDisplay;
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
