using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public enum FightEnum
    {
        Titan_Story, Titan_Hard, Titan_Extreme,
        M1N, M2N, M3N, M4N,
        M1S, M2S, M3S, M4S
    }

    public FightEnum currentFight;
    public GameObject fightTextGameObject; // Reference to the GameObject in the Inspector
    private TMP_Text fightText;  // Internal reference to the TextMeshPro component

    private Dictionary<FightEnum, List<string>> fightAttacks = new Dictionary<FightEnum, List<string>>();
    private Dictionary<string, int> attackSteps = new Dictionary<string, int>();

    private int currentAttackIndex = 0;
    private int currentStep = 0;

    void Start()
    {
        // Search for the TextMeshPro component on the assigned GameObject
        if (fightTextGameObject != null)
        {
            fightText = fightTextGameObject.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogError("No GameObject assigned for fightTextGameObject.");
        }

        InitializeFights();
        StartFight();
    }

    void Update()
    {
        // Check if the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcessNextStep();
        }
    }

    // Initialize fight data and attack steps
    void InitializeFights()
    {
        // Titan Story Attacks
        fightAttacks[FightEnum.Titan_Story] = new List<string>()
        {
            "Rock Buster", "Tumult", "Geocrush", "Landslide", "Granite Gaol",
            "Titan's Heart & Earthen Fury", "Rock Buster2", "Tumult2",
            "Geocrush2", "Landslide2", "Granite Gaol2", "Weight of the Land"
        };

        // Set the number of steps for each attack
        attackSteps["Rock Buster"] = 2;
        attackSteps["Tumult"] = 1;
        attackSteps["Geocrush"] = 2;
        attackSteps["Landslide"] = 1;
        attackSteps["Granite Gaol"] = 1;
        attackSteps["Titan's Heart & Earthen Fury"] = 3;
        attackSteps["Rock Buster2"] = 2;
        attackSteps["Tumult2"] = 1;
        attackSteps["Geocrush2"] = 2;
        attackSteps["Landslide2"] = 1;
        attackSteps["Granite Gaol2"] = 1;
        attackSteps["Weight of the Land"] = 1;

        // Add more fights and their attacks as needed
    }

    // Start the selected fight
    void StartFight()
    {
        currentAttackIndex = 0;
        currentStep = 0;
        UpdateFightText();
        ProcessNextStep();
    }

    // Function to advance to the next step of the current attack
    public void ProcessNextStep()
    {
        if (currentAttackIndex >= fightAttacks[currentFight].Count)
        {
            Debug.Log("Fight Completed");
            if (fightText != null)
            {
                fightText.text = "Fight Completed";
            }
            return;
        }

        string currentAttack = fightAttacks[currentFight][currentAttackIndex];
        int steps = attackSteps[currentAttack];

        if (currentStep < steps - 1)
        {
            currentStep++;
            Debug.Log($"Executing step {currentStep + 1} of attack: {currentAttack}");
        }
        else
        {
            Debug.Log($"Completed attack: {currentAttack}");
            currentStep = 0;
            currentAttackIndex++;
            if (currentAttackIndex < fightAttacks[currentFight].Count)
            {
                Debug.Log($"Moving to next attack: {fightAttacks[currentFight][currentAttackIndex]}");
            }
        }
        UpdateFightText(); // Update the TextMeshPro object with the current attack
    }

    // Optionally jump to a specific attack
    public void GoToAttack(int attackIndex)
    {
        if (attackIndex >= 0 && attackIndex < fightAttacks[currentFight].Count)
        {
            currentAttackIndex = attackIndex;
            currentStep = 0;
            Debug.Log($"Jumping to attack: {fightAttacks[currentFight][currentAttackIndex]}");
            UpdateFightText();
        }
    }

    // Update TextMeshPro with the current fight, attack, and step
    void UpdateFightText()
    {
        if (fightText != null && fightAttacks[currentFight].Count > currentAttackIndex)
        {
            string currentAttack = fightAttacks[currentFight][currentAttackIndex];
            int totalSteps = attackSteps[currentAttack];
            fightText.text = $"Fight: {currentFight}\nAttack: {currentAttack}\nStep: {currentStep + 1} of {totalSteps}";
        }
    }
}
