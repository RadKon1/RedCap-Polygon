using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health;
    [SerializeField] private int damage = 3;
    [SerializeField] private float speed = 2.4f;
    [Header("References")]
    private Rigidbody2D _rb;
    private BoxCollider2D _col;
    [SerializeField] private LayerMask groundMask;

    private Vector2 _boxWallScale = new Vector2(2f, 1f);
    private Vector3 _frontWallOffset = new Vector3(1.5f, 0f, 0f);
    private Vector3 _frontWallCheckPos;
    private Vector2 _boxEdgeScale = new Vector2(0.9f, 0.5f);
    private Vector3 _edgeOffset = new Vector3(1.5f, -1f, 0f);
    private Vector3 _edgeCheckPos;
    private float _time = 0f;
    private float _direction = 1f;
    private float _rayLenght = 0.05f;
    private bool isGrounded;

    void Awake()
    {
        health = maxHealth;
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        groundMask = LayerMask.GetMask("Ground");
        
        _direction = Random.Range(-1, 1);
        if (_direction == 0)
            _direction = 1;
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
        WallCheck();
        EdgeCheck();
        Move();
    }

    void Move()
    {
        _rb.linearVelocity = new Vector2(_direction * speed, _rb.linearVelocity.y);
    }
    
    void Rotate()
    {
        if (_direction > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(_direction), transform.localScale.y, transform.localScale.z);
        }
        else if (_direction < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(_direction), transform.localScale.y, transform.localScale.z);
        }
    }

    void Attack()
    {
        // PlaceHolder
    }
    void TakeDamage(int damage)
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_frontWallCheckPos, _boxWallScale);
        Gizmos.DrawWireCube(_edgeCheckPos, _boxEdgeScale);
    }

    #endregion
}