using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dictates player-character behaviour, including taking input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Configuration Variables
    [Header("Configuration")]
    [SerializeField] float runningSpeed = 5f;
    [SerializeField] float jumpPower = 10f;
    [SerializeField] float climbSpeed = 500f;
    [SerializeField] float startPauseTime = 1f;
    [SerializeField] float landingHeight = 1.9f;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform climbCheck;
    [SerializeField] LayerMask walkableLayers;
    [SerializeField] LayerMask climbableLayers;
    [SerializeField] Vector2 groundedCheckSize, climbCheckSize;

    [Header("Sounds")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;

    // Misc Variables
    [Header("Misc")]
    bool alive = true;
    bool moving = false;
    bool grounded = true;
    bool facingRight = true;
    bool climbing = false;
    float timeLastGrounded;
    float lastGroundedClimbHeight = 0f;

    private float jumpCooldown = 0.1f;
    private float jumpTimer = 0f;

    [SerializeField] private Vector3 velocity;

    // Gameobject/Component References
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private AudioSource walkingSound;

    public bool Grounded
    {
        get
        {
            return grounded;
        }
    }

    public bool Climbing
    {
        get
        {
            return climbing;
        }
    }

    public bool Alive
    {
        get
        {
            return alive;
        }
    }

    void Start ()
    {
        // Initialise member variables 
        gameManager = GameObject.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        walkingSound = GetComponent<AudioSource>();

        // Pause for moment, then start running
        //      To be replaced by interaction from Game Manager
        StartCoroutine(StartPlay());
	}

    void Update()
    {
        if (!alive)
            return;

        // Check if player attempted to jump
        JumpCheck();
    }

    private void FixedUpdate()
    {
        if (!alive)
            return;

        // Check if Climbing state has changed
        if (climbing != Physics2D.OverlapBox(climbCheck.position, climbCheckSize, 0f, climbableLayers))
            SetClimbing(!climbing);

        // Check if Grounded state has changed
        if (grounded != Physics2D.OverlapBox(groundCheck.position, groundedCheckSize, 0f, walkableLayers))
        {
            SetGrounded(!grounded);

            // Play land animation if now grounded, and has fallen from a specified height
            if (!climbing && lastGroundedClimbHeight - transform.position.y >= landingHeight)
                anim.SetTrigger("Land");
        }

        if (grounded || climbing)
            lastGroundedClimbHeight = transform.position.y;

        ApplyMovement();
    }

    private void ApplyMovement()
    {
        velocity.y = rb.velocity.y;

        // Set uniform upward motion if climbing
        if (climbing)
            velocity.y = climbSpeed * Time.fixedDeltaTime;

        rb.velocity = velocity;
    }

    private void JumpCheck()
    {
        jumpTimer += Time.deltaTime;
        if ((Input.touchCount > 0 || Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && jumpTimer >= jumpCooldown && (grounded || climbing || timeLastGrounded <= 0.1f))
        {
            jumpTimer = 0f;
            Jump();
        }
    }

    private void Jump()
    {
        if (climbing)
            SetFacingRight(!facingRight);

        // Override y velocity
        //
        // Do not override y velocity if more than jump power
        //      (Which may happen if jumping on a spring)
        if (rb.velocity.y <= jumpPower)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            // Play jump sound
            walkingSound.PlayOneShot(jumpSound);
        }
    }

    /// <summary>
    /// Small pause at the begin of play before movement.
    ///     Should eventually be changed to StartPlay function called by Game Manager
    ///     at the same time as game timers etc.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartPlay()
    {
        yield return new WaitForSeconds(startPauseTime);
        SetMoving(true);
        SetFacingRight(true);
    }

    /// <summary>
    /// Forces player to jump. bool to force wall-jump
    ///     Potentially used by obstacles.
    /// </summary>
    /// <param name="forceClimbing"></param>
    public void ForceJump(bool forceClimbing = false)
    {
        SetClimbing(forceClimbing ? true : climbing);
        Jump();
    }

    /// <summary>
    /// Kill the player-character. Informs the Game Manager
    /// </summary>
    public void DamagePlayer()
    {
        // Check if already dead (may have been damaged by multiple things at once)
        if (!alive)
            return;

        // Set variables to reflect death-state (includes turning off collision)
        alive = false;
        SetGrounded(false);
        SetClimbing(false);
        col.enabled = false;
        spriteRenderer.color = Color.red;

        // Add small upward force for makeshift death animation (includes unlocking rotation)
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(Random.Range(-2f, 2f), 8f), ForceMode2D.Impulse);
        rb.AddTorque(20f);

        // Inform animator (so it stops playing movement anims)
        anim.SetTrigger("Dead");

        // Inform gm that player has died
        gameManager.PlayerDeath();

        Destroy(this.gameObject, 5f);
    }

    /// <summary>
    /// Freeze player position - Stop taking input
    ///     Done by pretending the player is not alive.
    /// </summary>
    /// <param name="freeze"></param>
    public void FreezePlayer(bool freeze = true)
    {
        alive = !freeze;
        rb.simulated = alive;
    }

    #region Variable Setting

    private void SetMoving(bool value)
    {
        moving = value;
        anim.SetFloat("CurrentSpeed", moving ? 1f : 0f);
    }

    private void SetFacingRight(bool value)
    {
        facingRight = value;

        // Flip sprite (vie entire object scale)
        var scale = new Vector3(facingRight ? 1f : -1f, 1f, 1f);
        transform.localScale = scale;

        // Update velocity to move towards correct direction
        velocity.x = (value ? runningSpeed : -runningSpeed);
    }

    private void SetGrounded(bool value)
    {
        grounded = value;
        anim.SetBool("Grounded", value);

        // If changing to grounded, play land sound
        if (value)
            walkingSound.PlayOneShot(landSound);

        if (!grounded)
            timeLastGrounded = Time.time;
    }

    private void SetClimbing(bool value)
    {
        climbing = value;

        // Climbing animation currently unused
        //anim.SetBool("Climbing", value);
    }

    #endregion
}
