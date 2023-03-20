using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class AttackPlayer : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate()
    {
        if (context.isFind)
        {
            if (Vector3.Distance(context.transform.position, context.player.transform.position) < 1f)
            {
                Debug.Log("공격");
                return State.Success;
            }
            else
                return State.Failure;
        }
        else
        {
            return State.Failure;
        }
    }
}
