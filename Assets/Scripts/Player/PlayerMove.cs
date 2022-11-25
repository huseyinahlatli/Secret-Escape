using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private DynamicJoystick joystick;
        [SerializeField] private float moveSpeed;
    
        public static bool IsMoving;
        private Rigidbody _rigidbody;
        private Vector3 _movement;
        
        private void Start() => _rigidbody = GetComponent<Rigidbody>();

        private void Update()
        {
            if (joystick.Horizontal != 0 || joystick.Vertical != 0)
                IsMoving = true;
            else
                IsMoving = false;
        }

        private void FixedUpdate()
        {
            Movement();

            if (IsMoving)
            {
                Direction();
            }
        }

        private void Movement()
        {
            _movement = new Vector3(joystick.Horizontal, 0, joystick.Vertical) * (moveSpeed * Time.deltaTime);
            _rigidbody.velocity = _movement;
        }

        private void Direction()
        {
            if (_movement != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
            }
        }
    }
}
