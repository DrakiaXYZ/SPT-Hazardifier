using System;
using UnityEngine;

namespace EFT.Interactive
{
    public abstract class InteractableObject : MonoBehaviour
    {
        public Vector3 WorldInteractionDirection
        {
            get
            {
                return -base.transform.TransformDirection(this.InteractionDirection);
            }
        }
        
        public virtual void OnDrawGizmosSelected()
        {
            if (this.InteractionDirection.sqrMagnitude > 0f)
            {
                Vector3 worldInteractionDirection = this.WorldInteractionDirection;
                Debug.DrawLine(base.transform.position - worldInteractionDirection, base.transform.position, Color.magenta, Time.deltaTime);
            }
        }

        public ESpecificInteractionContext specificInteractionContext;
        public float InteractionDot;
        public Vector3 InteractionDirection;
    }
}
