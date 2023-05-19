using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.AI;

[System.Serializable]
public class AttackPlayer : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (context.isFind)
        {
            float dis = 0f;

            if (context.transform.CompareTag("Golam"))
                dis = 6f;
            else if (context.transform.CompareTag("Bee"))
                dis = 2f;
            
            if (Vector3.Distance(context.transform.position, context.player.transform.position) < dis )
            {
                if (!context.agent.isStopped && !context.animator.GetBool("isAttack"))
                {
                    context.animator.SetBool("isAttack", true);
                    context.agent.isStopped = true;
                    context.agent.velocity = Vector3.zero;
                    return State.Running;
                }
            }
            return State.Success;
        }
        
        return State.Failure;

    }
}