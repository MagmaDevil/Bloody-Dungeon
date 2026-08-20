using UnityEngine;

public enum PlayerJobType
{
    Warrior,
    Archer,
    Defender,
    Mage
}

public class Swordsman : CharacterBase
{
    protected override void LevelUp()
    {
        stats.maxHp += 20;
        stats.attack += 5;
        RestoreHp();

        Debug.Log($"[레벨업] {characterName}(검사) Lv.{level}");
    }
}

public class Archer : CharacterBase
{
    protected override void LevelUp()
    {
        stats.speed += 5;
        stats.attack += 3;
        RestoreHp();

        Debug.Log($"[레벨업] {characterName}(궁수) Lv.{level}");
    }
}

public class Tanker : CharacterBase
{
    protected override void LevelUp()
    {
        stats.maxHp += 30;
        stats.defense += 5;
        RestoreHp();

        Debug.Log($"[레벨업] {characterName}(탱커) Lv.{level}");
    }
}

public class Mage : CharacterBase
{
    protected override void LevelUp()
    {
        stats.magic += 10;
        RestoreHp();

        Debug.Log($"[레벨업] {characterName}(마법사) Lv.{level}");
    }
}