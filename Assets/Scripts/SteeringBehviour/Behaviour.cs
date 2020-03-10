using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [System.Serializable]
    public abstract class Behaviour
    {
        protected Agent owner;
        public virtual Agent Owner { set { owner = value; } }

        [SerializeField] protected float weight = 1f;

        public abstract void Execute();
    }
}