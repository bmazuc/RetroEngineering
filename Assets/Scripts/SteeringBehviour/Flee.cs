using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [System.Serializable]
    public class Flee : Steering
    {
        public override void Execute()
        {
            owner.ApplyForce(SteerTowards(owner.transform.position - owner.FearSource.transform.position) * weight);
        }
    }
}