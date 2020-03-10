using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [System.Serializable]
    public class Wander : Behaviour
    {
        private Vector3 circleCenter;
        private Vector3 displacement;

        [SerializeField] private float circleDistance = 1f;
        [SerializeField] private float circleRadius = 5f;
        [SerializeField] private float wanderAngle = 0f;
        [SerializeField] private float angleChange = 15f;

        public override void Execute()
        {
            circleCenter = owner.Velocity.normalized * circleDistance;
            displacement = Vector3.right * circleRadius;

            wanderAngle += (Random.Range(-1f, 1f) * angleChange) - (angleChange * 0.5f);
            displacement = Quaternion.AngleAxis(wanderAngle, Vector3.up) * displacement;

            owner.ApplyForce((circleCenter + displacement).normalized * weight);
        }
    }
}