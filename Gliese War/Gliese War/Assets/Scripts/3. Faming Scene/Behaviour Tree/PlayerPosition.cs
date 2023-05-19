using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class PlayerPosition : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate()
    {
        Vector3 pos = context.player.transform.position;
        blackboard.moveToPosition = pos;
        
        return State.Success;
    }
}
