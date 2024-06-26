using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody), typeof(Player))]
public class PlayerControls : MonoBehaviour
{
    [SerializeField] private Transform checkGroundPosition;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float playerSpeed;

    private Player _player;
    private Rigidbody _rigidbody;
    private Vector2 _moveInput;
    private bool _isGrounded { get => IsGrounded(); }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (_isGrounded)
            Move();
    }
    //Set move direction
    private void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    private void OnAttack()
    {
        _player.Attack();
    }

    //Move player by velocity
    private void Move()
    {
        Vector3 velocity = Vector3.forward * _moveInput.y + Vector3.right * _moveInput.x;
        velocity *= playerSpeed; 
        velocity.y = _rigidbody.velocity.y;

        _rigidbody.velocity = velocity;
    }
    //Check if player is grounded
    private bool IsGrounded()
    {
        
        return Physics.CheckBox(checkGroundPosition.position, new Vector3(.55f, groundCheckDistance, .55f), transform.rotation); ///Physics.Raycast(checkGroundPosition.position, Vector3.down, groundCheckDistance);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(checkGroundPosition.position, new Vector3(.55f, .1f, .55f) * 2);
    }
}
