using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [System.Serializable]
    public class Flock : Behaviour
    {
        public override Agent Owner
        {
            set
            {
                base.Owner = value;
                alignment.Owner = value;
                cohesion.Owner = value;
                separation.Owner = value;
            }
        }
        [SerializeField] private Align alignment;
        [SerializeField] private Cohere cohesion;
        [SerializeField] private Separation separation;


        public override void Execute()
        {
            alignment.Execute();
            cohesion.Execute();
            separation.Execute();
        }
    }
}