using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckPlayerPosition : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate()
    {
        if (Vector3.Distance(context.transform.position, context.player.transform.position) < context.findRange)
        {
            context.isFind = true;
            return State.Success;
        }
        else
        {
            context.isFind = false;
            return State.Failure;
        }
    }
}