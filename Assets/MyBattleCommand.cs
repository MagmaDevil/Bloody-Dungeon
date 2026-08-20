using UnityEngine;

public interface MyBattleCommand
{
    void Execute();
}

public class AttackCommand : MyBattleCommand
{
    private CharacterBase attacker;
    private CharacterBase target;

    public AttackCommand(CharacterBase attacker, CharacterBase target)
    {
        this.attacker = attacker;
        this.target = target;
    }

    public void Execute()
    {
        int damage = attacker.stats.attack - target.stats.defense;
        damage = Mathf.Max(1, damage);

        target.stats.currentHp -= damage;
        Debug.Log($"[전투] {attacker.characterName}이(가) {target.characterName}에게 {damage} 피해!");

        attacker.ResetTurn();
    }
}