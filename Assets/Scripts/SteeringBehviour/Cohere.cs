using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [System.Serializable]
    public class Cohere : Steering
    {
        private Vector3 steer;
        private int neighbour;
        [SerializeField] private float radius;

        public override void Execute()
        {
            steer = Vector3.zero;
            neighbour = 0;

            Agent current;
            List<Agent> agents = owner.Brain.agents;
            int length = agents.Count;
            for (int i = 0; i < length; ++i)
            {
                current = agents[i];
                if (current == owner)
                    continue;

                float dist = (current.transform.position - owner.transform.position).magnitude;

                if (dist <= 0)
                    continue;

                if (dist <= radius)
                {
                    steer += current.transform.position;
                    ++neighbour;
                }
            }

            if (neighbour > 0)
            {
                steer /= neighbour;
                steer -= owner.transform.position;
                steer = SteerTowards(steer);
                //cohesion.steer.Normalize();
            }

            owner.ApplyForce(steer * weight);
        }
    }
}