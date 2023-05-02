using UnityEngine;

public class Movement : MonoBehaviour
{
	[Header("Movement")]
	private float moveSpeed;
	[SerializeField] private float walkSpeed;
	[SerializeField] private float sprintSpeed;
	[SerializeField] private float groundDrag;

	[Header("Jumping")]
	[SerializeField] private float jumpForce;
	[SerializeField] private float jumpCooldown;
	[SerializeField] private float airMultiplier;
	[SerializeField] private float jumpTime;
	private float jumpTimeC;
	private bool jumping;
	bool readyToJump;

	[Header("Crouching")]
	[SerializeField] private float crouchSpeed;
	[SerializeField] private float crouchYScale;
	private float startYScale;

	[Header("Keybinds")]
	private KeyCode jumpKey = KeyCode.Space;
	private KeyCode sprintKey = KeyCode.LeftShift;
	private KeyCode crouchKey = KeyCode.LeftControl;

	[Header("Ground Check")]
	[SerializeField] private float playerHeight;
	[SerializeField] private LayerMask whatIsGround;
	bool grounded;

	[Header("Slope Handling")]
	[SerializeField] private float maxSlopeAngle;
	private RaycastHit slopeHit;
	private bool exitingSlope;

	[SerializeField] private Transform orientation;

	float horizontalInput;
	float verticalInput;

	Vector3 moveDirection; // The direction of the player's movement

	Rigidbody rb; // The Rigidbody component of the player

	[SerializeField] private MovementState state;
	[SerializeField] private enum MovementState
	{
		walking,
		sprinting,
		crouching,
		air
	}

	// Start is called before the first frame update
	private void Start()
	{
		// Get the Rigidbody component and freeze its rotation
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;

		readyToJump = true;

		startYScale = transform.localScale.y;
	}

	// Update is called once per frame
	private void Update()
	{
		// Check if the player is grounded
		grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

		UpdateInput();
		SpeedControl();
		StateHandler();

		// Set drag based on whether the player is grounded or not
		if(grounded)
			rb.drag = groundDrag;
		else
			rb.drag = 0;
	}
	
	// FixedUpdate is called at a fixed interval, used for physics calculations
	private void FixedUpdate()
	{
		// Calculate the moveDirection of the player
		moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

		// Check if on slope
		if(OnSlope() && !exitingSlope)
		{
			rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

			if(rb.velocity.y > 0)
				rb.AddForce(Vector3.down * 80f, ForceMode.Force);
		}

		// Add force to the player's Rigidbody component based on whether the player is grounded or not
		if(grounded)
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
		else if(!grounded)
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

		// turn gravity off while on slope
		rb.useGravity = !OnSlope();
	}
	private void StateHandler()
	{
		// Crouching
		if(Input.GetKey(crouchKey))
		{
			state = MovementState.crouching;
			moveSpeed = crouchSpeed;
		}
		// Sprinting
		else if(grounded && Input.GetKey(sprintKey))
		{
			state = MovementState.sprinting;
			moveSpeed = sprintSpeed;
		}
		// Walking
		else if(grounded)
		{
			state = MovementState.walking;
			moveSpeed = walkSpeed;
		}
		// Air
		else
		{
			state = MovementState.air;
		}
	}
	private void UpdateInput()
	{
		// Get the user input
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		// Check if the jump key is pressed, if the player is ready to jump, and if the player is grounded before jumping
		if(Input.GetKey(jumpKey) && readyToJump && grounded)
		{
			readyToJump = false;
			rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

			jumping = true;
			jumpTimeC = jumpTime;
			rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
		}

		// keep jumping when holding space for x time
		if (Input.GetKey(jumpKey) && jumping)
		{
			if (jumpTimeC > 0)
			{
				rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
				jumpTimeC -= Time.deltaTime;
			}
			else
			{
				jumping = false;
				Invoke("ResetJump", jumpCooldown);
			}
		};
		// Reset jump so you can't jump twice
		if (Input.GetKeyUp(jumpKey))
		{
			jumping = false;
			Invoke("ResetJump", jumpCooldown);
		}

		// Check if the crouch key is down or up, crouch if it's down
		if (Input.GetKeyDown(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
			rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
		}
		if(Input.GetKeyUp(crouchKey))
		{
			transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
		}

	}
	private void SpeedControl()
	{
		// Limit speed on slopes
		if(OnSlope() && !exitingSlope)
		{
			if(rb.velocity.magnitude > moveSpeed)
				rb.velocity = rb.velocity.normalized * moveSpeed;
		}
		else
		{
			Vector3 velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			// Check if the magnitude of the horizontal velocity is greater than moveSpeed.
			if(velocity.magnitude > moveSpeed)
			{
				Vector3 limitedVel = velocity.normalized * moveSpeed;
				rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
			}
		}	
	}
	private bool OnSlope()
	{
		if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
		{
			float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
			return angle < maxSlopeAngle && angle != 0;
		}
		return false;
	}
	private Vector3 GetSlopeMoveDirection()
	{
		return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
	}

	private void ResetJump()
	{
		readyToJump = true;
	}
}