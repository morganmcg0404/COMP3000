using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSystem : MonoBehaviour
{
    [Header("Combat Skills")]
    [SerializeField] private Skill combatSkill;
    [SerializeField] private Skill healthSkill;

    [Header("Gathering Skills")]
    [SerializeField] private Skill miningSkill;
    [SerializeField] private Skill fishingSkill;

    [Header("Crafting Skills")]
    [SerializeField] private Skill smithingSkill;
    [SerializeField] private Skill cookingSkill;

    [Header("Experience Multipliers")]
    [SerializeField] private float combatExpMultiplier = 1f;
    [SerializeField] private float skillingExpMultiplier = 1f;

    [Header("UI")]
    [SerializeField] private GameObject xpDropPrefab;
    [SerializeField] private GameObject xpDropTextPrefab;

    private Dictionary<string, Skill> skillDictionary;

    public Skill CombatSkill => combatSkill;
    public Skill HealthSkill => healthSkill;
    public Skill MiningSkill => miningSkill;
    public Skill FishingSkill => fishingSkill;
    public Skill SmithingSkill => smithingSkill;
    public Skill CookingSkill => cookingSkill;

    void Awake()
    {
        InitializeSkills();
    }

    void Start()
    {
        combatSkill.OnLevelUp += OnCombatLevelUp;
        healthSkill.OnLevelUp += OnHealthLevelUp;
        miningSkill.OnLevelUp += OnMiningLevelUp;
        fishingSkill.OnLevelUp += OnFishingLevelUp;
        smithingSkill.OnLevelUp += OnSmithingLevelUp;
        cookingSkill.OnLevelUp += OnCookingLevelUp;
    }

    void OnDestroy()
    {
        combatSkill.OnLevelUp -= OnCombatLevelUp;
        healthSkill.OnLevelUp -= OnHealthLevelUp;
        miningSkill.OnLevelUp -= OnMiningLevelUp;
        fishingSkill.OnLevelUp -= OnFishingLevelUp;
        smithingSkill.OnLevelUp -= OnSmithingLevelUp;
        cookingSkill.OnLevelUp -= OnCookingLevelUp;
    }

    private void InitializeSkills()
    {
        combatSkill = new Skill("Combat", 1, 100);
        healthSkill = new Skill("Health", 10, 100);
        miningSkill = new Skill("Mining", 1, 100);
        fishingSkill = new Skill("Fishing", 1, 100);

        smithingSkill = new Skill("Smithing", 1, 100);
        cookingSkill = new Skill("Cooking", 1, 100);

        skillDictionary = new Dictionary<string, Skill>
        {
            { "Combat", combatSkill },
            { "Health", healthSkill },
            { "Mining", miningSkill },
            { "Fishing", fishingSkill },
            { "Smithing", smithingSkill },
            { "Cooking", cookingSkill }
        };

        Debug.Log("Skill System Initialized - All skills created");
    }

    public void OnEnemyHit(float hitAmount)
    {
        float expGain = 50f * hitAmount * combatExpMultiplier;
        
        combatSkill.AddExperience(expGain);
        healthSkill.AddExperience(expGain);

        // Instantiate the XP drop canvas once
        if (xpDropPrefab == null)
        {
            Debug.LogWarning("SkillSystem: XP drop prefab not assigned!");
            return;
        }

        GameObject xpDropCanvas = Instantiate(xpDropPrefab);
        Transform canvasTransform = xpDropCanvas.transform;

        // Find and initialize the first XP drop text (should be a child of canvas with XPDrop script)
        XPDrop firstXPDrop = xpDropCanvas.GetComponentInChildren<XPDrop>();
        if (firstXPDrop != null)
        {
            firstXPDrop.Initialize((int)expGain, "Combat");
        }

        // Spawn the second XP drop text as a child of the canvas
        SpawnXPDropText((int)expGain, "Health", canvasTransform);

        Debug.Log($"Enemy hit! Gained {expGain} exp to Combat and Health skills");
    }

    private void SpawnXPDropText(int xpAmount, string skillName, Transform canvasTransform)
    {
        if (xpDropTextPrefab == null)
        {
            Debug.LogError("SkillSystem: XP drop text prefab NOT assigned!");
            return;
        }

        Debug.Log($"SkillSystem: Spawning {skillName} XP drop text");

        GameObject xpDropTextObj = Instantiate(xpDropTextPrefab, canvasTransform, false);
        Debug.Log($"SkillSystem: {skillName} XP drop text instantiated: {xpDropTextObj.name}");

        XPDropChild xpDropChild = xpDropTextObj.GetComponent<XPDropChild>();
        if (xpDropChild != null)
        {
            xpDropChild.Initialize(xpAmount, skillName);
            Debug.Log($"SkillSystem: {skillName} XP drop initialized via XPDropChild");
        }
        else
        {
            XPDrop xpDrop = xpDropTextObj.GetComponent<XPDrop>();
            if (xpDrop != null)
            {
                xpDrop.Initialize(xpAmount, skillName);
                Debug.Log($"SkillSystem: {skillName} XP drop initialized via XPDrop");
            }
            else
            {
                Debug.LogError($"SkillSystem: {skillName} XP drop text prefab has no XPDropChild or XPDrop component!");
            }
        }
    }

    public Skill GetSkill(string skillName)
    {
        if (skillDictionary.TryGetValue(skillName, out Skill skill))
        {
            return skill;
        }

        Debug.LogWarning($"Skill '{skillName}' not found!");
        return null;
    }

    public List<Skill> GetAllSkills()
    {
        return new List<Skill> { combatSkill, healthSkill, miningSkill, fishingSkill, smithingSkill, cookingSkill };
    }

    public void OnOreMined(float expAmount)
    {
        float adjustedExp = expAmount * skillingExpMultiplier;
        miningSkill.AddExperience(adjustedExp);
        Debug.Log($"Ore mined! Gained {adjustedExp} Mining exp");
    }

    public void OnFishCaught(float expAmount)
    {
        float adjustedExp = expAmount * skillingExpMultiplier;
        fishingSkill.AddExperience(adjustedExp);
        Debug.Log($"Fish caught! Gained {adjustedExp} Fishing exp");
    }

    public void OnItemSmithed(float expAmount)
    {
        float adjustedExp = expAmount * skillingExpMultiplier;
        smithingSkill.AddExperience(adjustedExp);
        Debug.Log($"Item smithed! Gained {adjustedExp} Smithing exp");
    }

    public void OnFoodCooked(float expAmount)
    {
        float adjustedExp = expAmount * skillingExpMultiplier;
        cookingSkill.AddExperience(adjustedExp);
        Debug.Log($"Food cooked! Gained {adjustedExp} Cooking exp");
    }

    // Event handlers for level ups
    private void OnCombatLevelUp(int newLevel)
    {
        Debug.Log($"<color=yellow>COMBAT SKILL LEVEL UP! Now level {newLevel}</color>");
    }

    private void OnHealthLevelUp(int newLevel)
    {
        Debug.Log($"<color=green>HEALTH SKILL LEVEL UP! Now level {newLevel}</color>");
    }

    private void OnMiningLevelUp(int newLevel)
    {
        Debug.Log($"<color=cyan>MINING SKILL LEVEL UP! Now level {newLevel}</color>");
    }

    private void OnFishingLevelUp(int newLevel)
    {
        Debug.Log($"<color=blue>FISHING SKILL LEVEL UP! Now level {newLevel}</color>");
    }

    private void OnSmithingLevelUp(int newLevel)
    {
        Debug.Log($"<color=orange>SMITHING SKILL LEVEL UP! Now level {newLevel}</color>");
    }

    private void OnCookingLevelUp(int newLevel)
    {
        Debug.Log($"<color=magenta>COOKING SKILL LEVEL UP! Now level {newLevel}</color>");
    }

    public void SaveSkills()
    {
        PlayerPrefs.SetInt("Combat_Level", combatSkill.Level);
        PlayerPrefs.SetFloat("Combat_Exp", combatSkill.CurrentExp);
        PlayerPrefs.SetInt("Health_Level", healthSkill.Level);
        PlayerPrefs.SetFloat("Health_Exp", healthSkill.CurrentExp);
        PlayerPrefs.Save();
        
        Debug.Log("Skills saved!");
    }

    public void LoadSkills()
    {
        if (PlayerPrefs.HasKey("Combat_Level"))
        {
            int combatLevel = PlayerPrefs.GetInt("Combat_Level", 1);
            float combatExp = PlayerPrefs.GetFloat("Combat_Exp", 0f);
            int healthLevel = PlayerPrefs.GetInt("Health_Level", 1);
            float healthExp = PlayerPrefs.GetFloat("Health_Exp", 0f);

            combatSkill = new Skill("Combat", combatLevel, 100);
            healthSkill = new Skill("Health", healthLevel, 100);
            
            // Note: Would need to add a method to set current exp directly in Skill class
            // For now, this initializes at the correct level but exp at 0
            
            Debug.Log("Skills loaded!");
        }
    }
}
