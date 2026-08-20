using UnityEngine;

public enum RaceType
{
    Human,
    Elf,
    Orc,
    Demon
}

[System.Serializable]
public class CharacterStats
{
    public int maxHp;
    public int currentHp;
    public int attack;
    public int magic;
    public int defense;
    public int speed;
}

public abstract class CharacterBase : MonoBehaviour
{
    public string characterName;
    public RaceType race;

    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    public CharacterStats stats;

    [HideInInspector] public float currentActionGauge = 0f;
    [HideInInspector] public bool isReadyToAct = false;

    protected virtual void Start()
    {
        InitializeRaceStats();
    }

    public void InitializeRaceStats()
    {
        stats.maxHp = 5;
        stats.attack = 5;
        stats.magic = 5;
        stats.defense = 5;
        stats.speed = 5;

        switch (race)
        {
            case RaceType.Human:
                break;
            case RaceType.Elf:
                stats.defense = 3;
                stats.speed = 7;
                break;
            case RaceType.Orc:
                stats.attack = 6;
                stats.defense = 6;
                stats.maxHp = 6;
                stats.speed = 2;
                break;
            case RaceType.Demon:
                stats.attack = 3;
                stats.magic = 7;
                break;
        }

        RestoreHp();
        Debug.Log($"[{characterName}] 종족: {race} / 스탯 세팅 완료");
    }

    public void ResetTurn()
    {
        currentActionGauge = 0f;
        isReadyToAct = false;
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            level++;
            expToNextLevel = Mathf.FloorToInt(expToNextLevel * 1.5f);
            LevelUp();
        }
    }

    protected abstract void LevelUp();

    protected void RestoreHp()
    {
        stats.currentHp = stats.maxHp;
    }
}