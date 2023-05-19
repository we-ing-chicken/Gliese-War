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
        }
        else
        {
            // 밤
            context.findRange = 15;
            context.agent.speed = 5;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate()
    {
        return State.Failure;
    }
}
