using UnityEngine;

namespace Police
{
    public class PoliceAnimation : MonoBehaviour
    {
        private static Animator _animator;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    
        private void Start() => _animator = GetComponent<Animator>();

        public void Update() => _animator.SetBool(IsWalking, true); // true => PlayerController.WalkState
    }
}

