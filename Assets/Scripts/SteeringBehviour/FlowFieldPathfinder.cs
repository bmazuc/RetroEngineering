using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [System.Serializable]
    public class FlowFieldPathfinder : Behaviour
    {
        public override void Execute()
        {
            Cell cell = owner.Brain.flowField.Grid.GetCellFromWorld(owner.transform.position);
            if (cell == null)
                return;
            Vector2 dir = cell.direction * owner.MaxForce;
            owner.ApplyForce(new Vector3(dir.x, 0f, dir.y) * weight);
        }
    }
}