using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [System.Serializable]
    public abstract class Steering : Behaviour
    {
        protected Vector3 SteerTowards(Vector3 target)
        {
            Vector3 steering = target.normalized * owner.MaxSpeed; // compute desired velocity
            steering -= owner.Velocity; // desired velocity - velocity
            return steering;
        }
    }
}