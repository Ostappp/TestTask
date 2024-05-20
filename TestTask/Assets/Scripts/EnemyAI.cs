using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{

    [SerializeField] private string targetTag;
    private Transform _target;

    [SerializeField, Min(.1f)] private float entitySpeed;
    [SerializeField, Min(2)] private float dontMoveDistance; // distance where entity don't come closer to the target

    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private float fireBallSpeed;

    [SerializeField, Min(0)] private float attackDistance;
    [SerializeField] private float attackDelay;

    private bool _canAttack = true;

    [SerializeField] private Transform head;
    private Rigidbody _rigidbody;


    [SerializeField] private Gradient entityColor;
    private Material _entityMaterial;

    private float _colorValue = 0;
    private float ColorValue { get => _colorValue; set => _colorValue = value > 1 ? value - 1 : value; }

    [SerializeField] private GameObject deathEffect;
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag(targetTag).transform;
        _rigidbody = GetComponent<Rigidbody>();
        _entityMaterial = GetComponentInChildren<MeshRenderer>().material;
    }

    private void FixedUpdate()
    {

        ColorValue += Time.deltaTime;
        _entityMaterial.color = entityColor.Evaluate(ColorValue);

        if (_target != null)
        {
            SetLook();
            if (IsGrounded() && TargetDistance() > dontMoveDistance)
            {
                Move();
            }
            if (_canAttack && TargetDistance() <= attackDistance)
            {
                Attack();
            }
        }
        else
            _target = GameObject.FindGameObjectWithTag(targetTag).transform;
    }

    public void OnDestroy()
    {
        Instantiate(deathEffect, transform.position + Vector3.up, Quaternion.identity);
    }

    private void SetLook()
    {
        head.LookAt(_target);
        //rotate entity to look at player
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, head.eulerAngles.y, transform.eulerAngles.z);
        head.LookAt(_target);
    }
    private void Move()
    {
        Vector3 velocity = transform.forward * entitySpeed;
        velocity.y = _rigidbody.velocity.y;

        _rigidbody.velocity = velocity;
    }
    private void Attack()
    {
        //Create fireball
        GameObject fireBall = Instantiate(fireBallPrefab, head.position + head.forward * 2, Quaternion.identity);
        fireBall.transform.LookAt(_target);

        //Send fireball into a player
        var fRb = fireBall.GetComponent<Rigidbody>();
        fRb.useGravity = false;
        fRb.velocity = head.forward * fireBallSpeed;

        //Reload
        _canAttack = false;
        StartCoroutine(ResetAttack());
    }



    private float TargetDistance()
    {
        return Vector3.Distance(head.position, _target.position);
    }
    private bool IsGrounded()
    {
        return Physics.CheckBox(transform.position, new Vector3(.55f, .1f, .55f), transform.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(.55f, .1f, .55f) * 2);
    }
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        _canAttack = true;
    }
}
