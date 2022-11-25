using Player;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private float _velocity;
    private const float TransitionSpeed = 3.0f;
    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
        
    private void Start() => _animator = GetComponentInChildren<Animator>();

    private void Update()
    {
        switch (PlayerMove.IsMoving)
        {
            case true when _velocity < 1.0f:
                _velocity += Time.deltaTime * TransitionSpeed;
                break;
            case false when _velocity > 0.0f:
                _velocity -= Time.deltaTime * TransitionSpeed;
                break;
        }

        if (!PlayerMove.IsMoving && _velocity < 0.0f)
        {
            _velocity = 0f;
        }
            
        _animator.SetFloat(VelocityHash, _velocity);
    }
}
