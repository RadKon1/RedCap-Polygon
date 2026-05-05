using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D _bodyCollider;

    private Rigidbody2D _rb;

    // movement variables
    public float HorizontalVelocity { get; private set; }
    private bool _isFacingRight;


    // collision check variables
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _bumpedHead;


    // jump variables
    public float VerticalVelocity { get; private set;  }
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;



    // apex variables
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;


    // jump buffer variables
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;


    // cotote time variables
    private float _coyoteTimer;



    // dash variables
    private bool _isDashing;
    private bool _isAirDashing;
    private float _dashTimer;
    private float _dashOnGroundTimer;
    private int _numberOfDashesUsed;
    private Vector2 _dashDirection;
    private bool _isDashFastFalling;
    private float _dashFastFallTime;
    private float _dashFastFallReleaseSpeed;



    private void Update()
    {
        CountTimers();
        JumpChecks();
        LandCheck();
        DashCheck();
    }




    private void Awake()
    {
        _isFacingRight = true;

        _rb = GetComponent<Rigidbody2D>();

    }


    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();
        Fall();
        Dash();


        if (_isGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
        }

        ApplyVelocity();
    }


    private void ApplyVelocity()
    {
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);

        _rb.linearVelocity = new Vector2(HorizontalVelocity, VerticalVelocity);
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (Mathf.Abs(moveInput.x) >= MoveStats.MoveThreshold)
        {
            // Sprawdź, czy gracz musi się obrócić
            TurnCheck(moveInput);

            // USTAWIENIE TYLKO BIEGU: 
            // Zawsze używamy MaxRunSpeed, gdy moveInput.x nie jest zerem
            float targetVelocity = moveInput.x * MoveStats.MaxRunSpeed;

            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Stan IDLE: Wyhamowanie do zera
            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (_isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }

        else if (!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            _isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion

    #region Land/Fall

    private void LandCheck()
    {
        // check landed

        if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            // then reset all flags
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }


    private void Fall()
    {
        // Grawitacja gdy nie skaczemy (np. spadanie z krawędzi)
        if (!_isGrounded && !_isJumping && !_isFastFalling)
        {
            if (!_isFalling) _isFalling = true;
            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }
        else if (_isGrounded && !_isJumping)
        {
            VerticalVelocity = MoveStats.Gravity * Time.fixedDeltaTime;
        }

    }

    #endregion

    #region Jump

    private void JumpChecks()
    {

        if (InputManager.JumpWasPressed) Debug.Log($"Skok naciśnięty! Grounded: {_isGrounded}, Coyote: {_coyoteTimer}");
        // when pressed jump button
        if (InputManager.JumpWasPressed)
        {
            _jumpBufferTimer = MoveStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        // when released jump button

        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    // reached peak of the jump
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }


        // initiate jump with jump buffering and coyote time
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump();

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }
    }

    private void InitiateJump()
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }
        _jumpBufferTimer = 0f;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }


    private void Jump()
    {
        // 1. Grawitacja podczas skoku (wznoszenie i opadanie)
        if (_isJumping)
        {
            if (_bumpedHead) _isFastFalling = true;

            if (VerticalVelocity > 0f) // Wznoszenie
            {
                _apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    _timePastApexThreshold += Time.fixedDeltaTime;
                    if (_timePastApexThreshold < MoveStats.ApexHangTime)
                        VerticalVelocity = 0f;
                    else
                        VerticalVelocity = -0.01f;
                }
                else if (!_isFastFalling) 
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold) _isPastApexThreshold = false;
                }
            }
            else if (!_isFastFalling) // Normalne opadanie po skoku
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMutliplier * Time.fixedDeltaTime;
            }
        }

        // 2. Obsługa szybkiego spadania (Jump Cut / Head Bump)
        if (_isFastFalling)
        {
            if (_fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMutliplier * Time.fixedDeltaTime;
            }
            else
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MoveStats.TimeForUpwardsCancel));
            }
            _fastFallTime += Time.fixedDeltaTime;
        }

        // 3. Zastosowanie prędkości do Rigidbody
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);
    }


    #endregion

    #region Dash

    private void DashCheck()
    {
        if (InputManager.DashWasPressed)
        {
            // ground dash

            if (_isGrounded && _dashOnGroundTimer < 0 && !_isDashing)
            {
                InitiateDash();
            }
            // air dash
            else if (!_isGrounded && !_isDashing && _numberOfDashesUsed < MoveStats.NumberOfDashes)
            {
                _isAirDashing = true;
                InitiateDash();
            }
        }
    }

    private void InitiateDash()
    {
        _dashDirection = InputManager.Movement;
        Vector2 closestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[0]);

        for (int i = 0; i < MoveStats.DashDirections.Length; i++)
        {
            // if we hit exact direction, break immediately
            if (_dashDirection == MoveStats.DashDirections[i])
            {
                closestDirection = _dashDirection;
                break;
            }

            float distance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[i]);

            bool isDiagonal = (Mathf.Abs(MoveStats.DashDirections[i].x) == 1 && Mathf.Abs(MoveStats.DashDirections[i].y) == 1);
            if (isDiagonal)
            {
                distance -= MoveStats.DashDiagonallyBias;
            }

            else if (distance < minDistance)
            {
                minDistance = distance;
                closestDirection = MoveStats.DashDirections[i];
            }
        }

        // handle direction with zero input (dash in place)

        if (closestDirection == Vector2.zero)
        {
            closestDirection = _isFacingRight ? Vector2.right : Vector2.left;
        }

        _dashDirection = closestDirection;
        _numberOfDashesUsed++;
        _isDashing = true;
        _dashTimer = 0f;
        _dashOnGroundTimer = MoveStats.TimeBetweenDashesOnGround;
    }


    private void Dash()
    {

        if (_isDashing)
        {
            _dashTimer += Time.fixedDeltaTime;
            if (_dashTimer > MoveStats.DashTime)
            {
                if (_isGrounded)
                {
                    ResetDashes();
                }

                _isAirDashing = false;
                _isDashing = false;

                if (!_isJumping)
                {
                    _dashFastFallTime = 0f;
                    _dashFastFallReleaseSpeed = VerticalVelocity;

                    if (!_isGrounded)
                    {
                        _isDashFastFalling = true;
                    }
                }

                return;
            }

            HorizontalVelocity = _dashDirection.x * MoveStats.DashSpeed;

            if (_dashDirection.y != 0f || _isAirDashing)
            {
                VerticalVelocity = _dashDirection.y * MoveStats.DashSpeed;
            }
        }

        // handling dash cut time

        else if (_isDashFastFalling)
        {
            if (VerticalVelocity > 0f)
            {
                if (_dashFastFallTime < MoveStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(_dashFastFallReleaseSpeed, 0f, (_dashFastFallTime / MoveStats.DashTimeForUpwardsCancel));
                }
                else if (_dashFastFallTime >= MoveStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity += MoveStats.Gravity * MoveStats.DashGravityOnReleaseMutliplier * Time.fixedDeltaTime;
                }

                _dashFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.DashGravityOnReleaseMutliplier * Time.fixedDeltaTime;
            }
        }
    }

    private void ResetDashes()
    {
        _numberOfDashesUsed = 0;

    }
    #endregion


    #region Timers

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;

        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else
        {
            _coyoteTimer = MoveStats.JumpCoyoteTime;

        }

        // dash timer
        if (_isGrounded)
        {
            _dashOnGroundTimer -= Time.deltaTime;
        }
    }

    #endregion


    #region Collision Checks


    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_bodyCollider.bounds.center.x, _bodyCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_bodyCollider.bounds.size.x, MoveStats.GroundDetectionRayLength);


        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

    }


    private void BumpedHead()
    {
        // Używamy _bodyCollider do wyznaczenia punktu startowego na górze postaci
        Vector2 boxCastOrigin = new Vector2(_bodyCollider.bounds.center.x, _bodyCollider.bounds.max.y);

        // Rozmiar pudełka detekcji - szerokość mnożymy przez HeadWidth dla lepszej kontroli
        Vector2 boxCastSize = new Vector2(_bodyCollider.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        // Wykonujemy BoxCast w górę
        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);

        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else
        {
            _bumpedHead = false;
        }

        #region Debug Visualization

        if (MoveStats.DebugShowHeadBumpBox)
        {
            float headWidth = MoveStats.HeadWidth;
            Color rayColor = _bumpedHead ? Color.green : Color.red;

            // Rysowanie wizualizacji w oknie Scene (3 linie tworzące obrys "pudełka")
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + MoveStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }

        #endregion
    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }

    #endregion

}