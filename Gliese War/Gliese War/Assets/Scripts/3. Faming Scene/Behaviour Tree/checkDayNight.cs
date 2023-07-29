using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class checkDayNight : ActionNode
{
    protected override void OnStart()
    {
        if (!FarmingManager.Instance._isNight)
        {
            // 낮
            context.findRange = 10;
            context.agent.speed = 3;
            context.agent.GetComponent<Monster>().nowDamage = context.agent.GetComponent<Monster>().initDamage;
        }
        else
        {
            // 밤
            context.findRange = 20;
            context.agent.speed = 5;
            context.agent.GetComponent<Monster>().nowDamage = (int)(context.agent.GetComponent<Monster>().initDamage * 1.2);
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate()
    {
        return State.Failure;
    }
}
