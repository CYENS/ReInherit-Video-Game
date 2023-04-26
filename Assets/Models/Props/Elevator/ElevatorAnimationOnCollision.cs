using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class ElevatorAnimationOnCollision : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private string animationPlay;

        private void Start()
        {
            animationPlay = "Open-Close Elevator Animation";
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.name == "Keeper")
                animator.Play(animationPlay);
        }
    }
}
