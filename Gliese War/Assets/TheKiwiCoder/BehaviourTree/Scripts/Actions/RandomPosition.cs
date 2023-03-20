using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class RandomPosition : ActionNode {
    public Vector2 min;
    public Vector2 max;
    private int moveRange;

    protected override void OnStart()
    {
        moveRange = 10;
        min = new Vector2(context.transform.position.x - moveRange,context.transform.position.z - moveRange);
        max = new Vector2(context.transform.position.x + moveRange,context.transform.position.z + moveRange);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Vector3 pos = new Vector3();
        pos.x = Random.Range(min.x, max.x);
        pos.y = context.transform.position.y;
        pos.z = Random.Range(min.y, max.y);
        blackboard.moveToPosition = pos;
        
        return State.Success;
    }
}
