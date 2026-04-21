using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health;
    [SerializeField] private int damage = 15;
    [SerializeField] private float speed = 2.4f;
    [Header("References")]
    private Rigidbody2D _rb;
    private BoxCollider2D _col;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;
    
    private Vector2 _boxWallScale = new Vector2(2f, 1f);
    private Vector3 _frontWallOffset = new Vector3(1.5f, 0f, 0f);
    private Vector3 _frontWallCheckPos;
    
    private Vector2 _attackHitScale = new Vector2(1.2f, 1f);
    private Vector3 _attackHitOffset = new Vector3(1.5f, 0f, 0f);
    private Vector3 _attackHitPos;
    
    private Vector2 _boxEdgeScale = new Vector2(0.9f, 0.5f);
    private Vector3 _edgeOffset = new Vector3(1.5f, -1f, 0f);
    private Vector3 _edgeCheckPos;
    
    private Transform _player;
    private float _time = 0f;
    private float _attackTimer = 0f;
    private float _attackCooldown = 2.5f;
    private float _stateDuration = 2f;
    private float _direction = 1f;
    private float _rayLenght = 0.05f;
    private float _distance;
    private float _attackRange = 1.8f;
    private bool isGrounded;
    private bool isPlayerDetected;
    private State _state;

    void Awake()
    {
        health = maxHealth;
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        groundMask = LayerMask.GetMask("Ground");
        playerMask = LayerMask.GetMask("Player");
        _state = State.Idle;
        if (Random.value < 0.5f)
            _direction *= -1;
        Rotate();
    }
    
    void Update()
    {
        _time += Time.deltaTime;
        GroundCheck();
        if (!isGrounded)
        {
            return;
        }
        Detection();

        switch (_state)
        {
            case State.Idle:
                _rb.linearVelocity = new Vector2(0f,  _rb.linearVelocity.y);
                if (_time >= _stateDuration)
                {
                        _direction = Random.value < 0.5f ? -1f : 1f;
                    Rotate();
                    _state = State.Move;
                    _stateDuration = Random.Range(3f, 9f);
                    _time = 0f;
                }
                break;
            
            case State.Move:
                WallCheck();
                EdgeCheck();
                Move();
                if (_time >= _stateDuration)
                {
                    _state = State.Idle;
                    _stateDuration = Random.Range(2f, 5f);
                    _time = 0f;
                }
                break;
            
            case State.Attack:
                if (_player == null)
                {
                    _state = State.Move;
                    break;
                }
                _distance = Vector2.Distance(transform.position, _player.position);
                _direction = _player.position.x > transform.position.x ?  1f : -1f;
                Rotate();
                
                if (_distance > _attackRange)
                {
                    Move();   
                } else
                {
                    _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                    Attack();
                }
                break;
        }
    }

    void Move()
    {
        _rb.linearVelocity = new Vector2(_direction * speed, _rb.linearVelocity.y);
    }
    
    void Rotate()
    {

            transform.localScale = new Vector3(_direction, transform.localScale.y, transform.localScale.z);
    }

    void Attack()
    {
        _attackTimer += Time.deltaTime;
        _attackHitPos = transform.position + new Vector3 (_attackHitOffset.x * _direction, _attackHitOffset.y, _attackHitOffset.z);

        bool AttackReady = _attackTimer > _attackCooldown ? true : false;
        if (AttackReady)
        {
            Debug.Log("Attack!");
            Collider2D hit = Physics2D.OverlapBox(
                _attackHitPos, _attackHitScale, 0f, playerMask);
            
            if (hit != null)
            {
                PlayerTemp playerHealth = hit.GetComponent<PlayerTemp>();
                
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
            _attackTimer = 0f;
        }

    }
    
    public void TakeDamage(int damage)
    {
        // PlaceHolder
        health -= damage;
    }

    #region  Collision
    void GroundCheck()
    {
        // cRc - center RayCast, lRc - left RayCast, rRc - right RayCast
        Vector2 cOrigin = _col.bounds.center - new Vector3(0, _col.bounds.extents.y, 0);
        Vector2 lOrigin = _col.bounds.center - new Vector3(_col.bounds.extents.x, _col.bounds.extents.y, 0);
        Vector2 rOrigin = _col.bounds.center + new Vector3(_col.bounds.extents.x, -_col.bounds.extents.y, 0);

        RaycastHit2D cRc = Physics2D.Raycast(cOrigin, Vector2.down, _rayLenght, groundMask);
        RaycastHit2D lRc = Physics2D.Raycast(lOrigin, Vector2.down, _rayLenght, groundMask);
        RaycastHit2D rRc = Physics2D.Raycast(rOrigin, Vector2.down, _rayLenght, groundMask);
        
        Debug.DrawRay(cOrigin, Vector2.down * _rayLenght, Color.darkRed);
        Debug.DrawRay(lOrigin, Vector2.down * _rayLenght, Color.darkRed);
        Debug.DrawRay(rOrigin, Vector2.down * _rayLenght, Color.darkRed);
        isGrounded = cRc.collider != null || lRc.collider != null || rRc.collider != null;
    }

    void WallCheck()
    {
        _frontWallCheckPos = transform.position + _frontWallOffset * _direction;
        bool isFrontWallHit = Physics2D.OverlapBox(_frontWallCheckPos, _boxWallScale, 0, groundMask);
        if (isFrontWallHit)
        {
            _direction *= -1;
            Rotate();
        }
    }

    void EdgeCheck()
    {
        _edgeCheckPos = transform.position + new Vector3 (_edgeOffset.x * _direction,_edgeOffset.y, _edgeOffset.z);
        bool isEdgeHit = !Physics2D.OverlapBox(_edgeCheckPos, _boxEdgeScale, 0, groundMask);
        if (isEdgeHit)
        {
            _direction *= -1;
            Rotate();
        }
    }
    
    void Detection()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, 3f, playerMask);
        isPlayerDetected = playerCollider != null;
        if (isPlayerDetected)
        {
            _player = playerCollider.transform;
            Debug.Log("Player Detected");
            if (_state != State.Attack)
            {
                _state = State.Attack;   
            }
        }

        if (playerCollider == null)
        {
            _player = null;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_frontWallCheckPos, _boxWallScale);
        Gizmos.DrawWireCube(_edgeCheckPos, _boxEdgeScale);
        Gizmos.DrawWireSphere(transform.position, 3f);

        if (_attackTimer >= _attackCooldown - 1f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(_attackHitPos,  _attackHitScale);
        }
    }

    #endregion

    enum State
    {
        Idle,
        Move,
        Attack
    }
}