using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public List<CharacterBase> allCharacters = new List<CharacterBase>();
    private Queue<MyBattleCommand> actionQueue = new Queue<MyBattleCommand>();
    private const float MAX_ACTION_GAUGE = 100f;

    void Update()
    {
        UpdateActionGauges();

        if (actionQueue.Count > 0)
        {
            ResolveQueue();
        }
    }

    private void UpdateActionGauges()
    {
        // 전투 배속 적용
        float battleSpeed = PlayerPrefs.GetFloat("BattleSpeed", 1.0f);

        foreach (var character in allCharacters)
        {
            if (character.isReadyToAct || character.stats.currentHp <= 0) continue;

            character.currentActionGauge += character.stats.speed * Time.deltaTime * battleSpeed;

            if (character.currentActionGauge >= MAX_ACTION_GAUGE)
            {
                character.currentActionGauge = MAX_ACTION_GAUGE;
                character.isReadyToAct = true;

                Debug.Log($"[{character.characterName}]의 턴이 돌아왔습니다!");
            }
        }
    }

    public void EnqueueAction(MyBattleCommand command)
    {
        actionQueue.Enqueue(command);
    }

    private void ResolveQueue()
    {
        MyBattleCommand currentCommand = actionQueue.Dequeue();
        currentCommand.Execute();
    }
}