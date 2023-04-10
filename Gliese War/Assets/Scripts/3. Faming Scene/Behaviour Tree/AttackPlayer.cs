using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEditor.Rendering;
using UnityEngine.AI;

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
            if (Vector3.Distance(context.transform.position, context.player.transform.position) < 4f)
            {
                context.agent.isStopped = true;
                context.animator.SetBool("isAttack",true);
                return State.Success;
            }

            return State.Running;
        }
        else
        {
            return State.Failure;
        }
    }
}
