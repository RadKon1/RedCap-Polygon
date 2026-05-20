using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float initialHealth = 50f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float speed = 2.4f;
    [SerializeField] private float memoryDuration = 2f;
    [Header("References")]
    private Rigidbody2D _rb;
    private BoxCollider2D _col;
    private Animator _anim;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;
        
    private Vector2 _boxWallScale = new Vector2(2f, 1f);
    private Vector3 _frontWallOffset = new Vector3(1.5f, 0f, 0f);
    private Vector3 _frontWallCheckPos;
    
    private Vector2 _detectionScale = new Vector2(15f, 6f);
    private Vector3 _detectionOffset = new Vector3(3f, 0f, 0f);
    private Vector3 _detectionCheckPos;
    
    private Vector2 _attackHitScale = new Vector2(1.2f, 1f);
    private Vector3 _attackHitOffset = new Vector3(1.5f, 0f, 0f);
    private Vector3 _attackHitPos;
    
    private Vector2 _boxEdgeScale = new Vector2(0.9f, 0.5f);
    private Vector3 _edgeOffset = new Vector3(1.5f, -1f, 0f);
    private Vector3 _edgeCheckPos;
    
    private Transform _player;
    private Vector3 _lastPlayerPosition;
    private float _lostPlayerTimer = 0f;
    private float _time = 0f;
    private float _attackTimer = 0.5f;
    private float _attackCooldown = 1.2f;
    private float _stateDuration = 2f;
    private float _direction = 1f;
    private float _rayLenght = 0.05f;
    private float _distance;
    private float _attackRange = 1.8f;
    private bool isGrounded;
    private bool isPlayerDetected;
    private bool hasHitPlayer;
    private bool isAggresive;
    private State _state;
    
    private AttackState _attackState;
    private float _windupDuration = 0.5f;
    private float _activeDuration = 0.2f;
    private float _recoveryDuration = 1f;
    private float _attackDirection;
    private float _phaseTimer = 0f;
    private bool isAnimationPlayed;
    
    void Awake()
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.InitializeHealth(initialHealth);
        }
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
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

                if (HasWall() || HasEdge())
                {
                    _direction *= -1;
                    Rotate();
                }
                Move();
                
                if (_time >= _stateDuration)
                {
                    _state = State.Idle;
                    _stateDuration = Random.Range(2f, 5f);
                    _time = 0f;
                }
                break;
            
            case State.Chase:
                
                if (_player == null)
                {
                    _state = State.Move;
                    break;
                }
                
                Vector3 targetPos;
                if (_player != null)
                {
                    targetPos = _player.position;
                }
                else
                {
                    targetPos = _lastPlayerPosition;
                }
                _distance = Vector2.Distance(transform.position, _player.position);
                float xDifference = _player.position.x - transform.position.x;
                if (Mathf.Abs(xDifference) > 0.5f)
                {
                    _direction = Mathf.Sign(xDifference);
                }
                
                if (HasWall() || HasEdge())
                {
                    _state = State.Move;
                    break;
                }
                
                Rotate();
                
                Move();
                
                if (_distance < _attackRange)
                {
                    _state = State.Attack;
                }
                
                break;
            
            case State.Attack:
                
                if (_player == null)
                {
                    _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                    Attack();
                    if (_attackState == AttackState.None)
                    {
                        _state = State.Move;
                    }

                    break;
                }
                _distance = Vector2.Distance(transform.position, _player.position);
                if (_distance > _attackRange && _attackState == AttackState.None)
                {
                    _state = State.Chase;
                    break;
                }

                _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                Attack();
                
                break;
        }

        _anim.SetFloat("Speed", Mathf.Abs(_rb.linearVelocity.x));
        _anim.SetBool("IsMoving", Mathf.Abs(_rb.linearVelocity.x) > 0.1f);
    }

    void Move()
    {
        _rb.linearVelocity = new Vector2(_direction * speed, _rb.linearVelocity.y);
    }
    
    void Rotate()
    {

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * _direction, transform.localScale.y, transform.localScale.z);
    }

    void Attack()
    {
        _attackTimer += Time.deltaTime;
        _phaseTimer += Time.deltaTime;

        switch (_attackState)
        {
            case AttackState.None:

                if (_attackTimer >= _attackCooldown)
                {
                    isAnimationPlayed = false;
                    _attackState = AttackState.Windup;
                    _phaseTimer = 0f;
                    hasHitPlayer = false;
                    Debug.Log("Can hit player again");
                    isAnimationPlayed = false;
                    
                    _attackDirection = _direction;
                    _attackHitPos = transform.position + new Vector3(
                        _attackHitOffset.x * _attackDirection,
                        _attackHitOffset.y,
                        _attackHitOffset.z
                    );

                    //Debug.Log("WINDUP");
                }

                break;

            case AttackState.Windup:

                if (!isAnimationPlayed)
                {
                    _anim.SetTrigger("Attack");
                    isAnimationPlayed = true;
                }
                if (_phaseTimer >= _windupDuration)
                {
                    _attackState = AttackState.Active;
                    _phaseTimer = 0f;

                    //Debug.Log("ACTIVE");
                }

                break;

            case AttackState.Active:

                Collider2D hit = Physics2D.OverlapBox(
                    _attackHitPos,
                    _attackHitScale,
                    0f,
                    playerMask
                );
                if (hit != null && !hasHitPlayer)
                {
                    Debug.Log($"WRÓG UDERZYŁ W: {hit.gameObject.name} (Tag: {hit.gameObject.tag})");

                    Health playerHealth = hit.GetComponent<Health>();

                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                    }
                    else
                    {
                        // Sprawdźmy, czy ten obiekt ma jakiegokolwiek rodzica z Health
                        Health parentHealth = hit.GetComponentInParent<Health>();
                        if (parentHealth != null)
                        {
                            Debug.Log("Znalazłem Health na rodzicu!");
                            parentHealth.TakeDamage(damage);
                        }
                        else
                        {
                            Debug.LogError($"BŁĄD: Obiekt {hit.gameObject.name} nie ma Health ani w sobie, ani w rodzicu!");
                        }
                    }
                    hasHitPlayer = true;
                }

                if (_phaseTimer >= _activeDuration)
                {
                    _attackState = AttackState.Recovery;
                    _phaseTimer = 0f;   
                }

                break;

            case AttackState.Recovery:
                
                if (_phaseTimer >= _recoveryDuration)
                {
                    _attackState = AttackState.None;
                    _attackTimer = 0f;
                    _phaseTimer = 0f;
                    //Debug.Log("RECOVERY END");
                }

                break;
        }
    }
    
    public void OnHit()
    {
        _attackState = AttackState.None;
        _phaseTimer = 0f;
        Debug.Log("Enemy stunned");
    }
    //public void TakeDamage(int damage)
    //{
    //    health -= damage;
    //    _attackState = AttackState.None;
    //    _phaseTimer = 0f;
        
    //    // Death
    //    if (health <= 0)
    //        Destroy(gameObject);
    //}

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

    bool HasWall()
    {
            _frontWallCheckPos = new Vector2(
            transform.position.x + (_col.bounds.extents.x + 0.1f) * _direction,
            transform.position.y
        );
        bool isFrontWallHit = Physics2D.OverlapBox(
            _frontWallCheckPos, 
            _boxWallScale, 
            0, 
            groundMask);
        
        return isFrontWallHit;
    }

    bool HasEdge()
    {
        float checkX =
            _col.bounds.extents.x + 0.2f;

        float checkY =
            _col.bounds.extents.y + 0.1f;

        Vector2 edgeCheckPos = new Vector2(
            transform.position.x + checkX * _direction,
            transform.position.y - checkY
        );

        bool isEdgeHit = !Physics2D.OverlapBox(
            edgeCheckPos,
            _boxEdgeScale,
            0,
            groundMask
        );

        _edgeCheckPos = edgeCheckPos;

        return isEdgeHit;
    }
    
    void Detection()
    {
        _detectionCheckPos = transform.position + _detectionOffset * _direction;
        Collider2D playerCollider = Physics2D.OverlapBox(_detectionCheckPos, _detectionScale, 0f, playerMask);
        isPlayerDetected = playerCollider != null;
        
        if (playerCollider != null)
        {
            
            Transform detectedPlayer = playerCollider.transform;
            Vector2 directionToPlayer = (detectedPlayer.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, detectedPlayer.position);
            
            RaycastHit2D wallHit = Physics2D.Raycast(
                transform.position,
                directionToPlayer,
                distanceToPlayer,
                groundMask);
            
            float directionToPlayerX = detectedPlayer.position.x - transform.position.x;
            bool isPlayerInFront = Math.Sign(directionToPlayerX) == _direction;
            bool canSeePlayer = wallHit.collider == null;
            bool alreadyAggro = _state == State.Chase || _state == State.Attack;

            if (canSeePlayer && (isPlayerInFront || alreadyAggro))
            {
                isAggresive = true;
                _player = detectedPlayer;
                _lastPlayerPosition = _player.position;
                _lostPlayerTimer = 0f;
                
                if (_state == State.Idle || _state == State.Move)
                {
                    _state = State.Chase;   
                }
            }
            
        }

        if (_player != null)
        {
            _lostPlayerTimer += Time.deltaTime;
            if (_lostPlayerTimer >= memoryDuration)
            {
                isAggresive = false;
                _player = null;
                if (_state == State.Chase)
                {
                    _state = State.Move;
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_frontWallCheckPos, _boxWallScale);
        Gizmos.DrawWireCube(_edgeCheckPos, _boxEdgeScale);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_detectionCheckPos, _detectionScale);

        if (_attackState == AttackState.Windup || _attackState == AttackState.Active)
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
        Chase,
        Attack
    }

    enum AttackState
    {
        None,
        Windup,
        Active,
        Recovery
    }
}