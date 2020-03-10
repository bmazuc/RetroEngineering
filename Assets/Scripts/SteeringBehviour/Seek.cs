using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [System.Serializable]
    public class Seek : Steering
    {
        public override void Execute()
        {
            if (owner.Brain.target)
                owner.ApplyForce(SteerTowards(owner.Brain.target.position - owner.transform.position) * weight);
        }
    }
}