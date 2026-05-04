using System;
using UnityEngine;

[System.Serializable]
public class Skill
{
    [SerializeField] private string skillName;
    [SerializeField] private int level;
    [SerializeField] private float currentExp;
    [SerializeField] private int maxLevel = 100;

    public event Action<int> OnLevelUp;
    public event Action<float> OnExpGained;

    public string SkillName => skillName;
    public int Level => level;
    public float CurrentExp => currentExp;
    public int MaxLevel => maxLevel;

    public Skill(string name, int startLevel = 1, int maxLvl = 100)
    {
        skillName = name;
        level = Mathf.Clamp(startLevel, 1, maxLvl);
        currentExp = 0f;
        maxLevel = maxLvl;
    }

    public float GetExpForNextLevel()
    {
        if (level >= maxLevel)
            return 0f;
        return (level * level * 10f) + (level * 50f);
    }

    public float GetRemainingExpForNextLevel()
    {
        if (level >= maxLevel)
            return 0f;

        float expNeeded = GetExpForNextLevel();
        float remaining = expNeeded - currentExp;
        return Mathf.Max(0f, remaining);
    }

    public void AddExperience(float expAmount)
    {
        if (level >= maxLevel)
        {
            Debug.Log($"{skillName} is already at max level ({maxLevel})");
            return;
        }

        currentExp += expAmount;
        OnExpGained?.Invoke(expAmount);

        // Check for level up(s)
        while (currentExp >= GetExpForNextLevel() && level < maxLevel)
        {
            currentExp -= GetExpForNextLevel();
            level++;
            
            Debug.Log($"{skillName} leveled up to {level}!");
            OnLevelUp?.Invoke(level);

            // If we hit max level, clear any overflow exp
            if (level >= maxLevel)
            {
                currentExp = 0f;
                break;
            }
        }
    }

    public float GetExpPercentage()
    {
        if (level >= maxLevel)
            return 1f;

        float expNeeded = GetExpForNextLevel();
        return expNeeded > 0 ? currentExp / expNeeded : 0f;
    }

    public string GetExpDisplayString()
    {
        if (level >= maxLevel)
            return "MAX LEVEL";

        return $"{currentExp:F0} / {GetExpForNextLevel():F0} XP";
    }
}
