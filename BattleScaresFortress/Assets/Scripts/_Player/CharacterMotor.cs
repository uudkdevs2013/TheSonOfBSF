using UnityEngine;
using System.Collections;


// Require a character controller to be attached to the same game object
[RequireComponent (typeof(CharacterController))]
[AddComponentMenu ("Character/Character Motor")]
public class CharacterMotor : MonoBehaviour
{

	// Does this script currently respond to input?
	[SerializeField] private bool canControl = true;
	[SerializeField] private bool useFixedUpdate = true;
	
	// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
	// Very handy for organization!
	
	// The current global direction we want the character to move in.
	private Vector3 inputMoveDirection = Vector3.zero;
	public Vector3 InputMoveDirection
	{
		get
		{
			return inputMoveDirection;
		}
		set
		{
			inputMoveDirection = value;
		}
	}
	
	// Is the jump button held down? We use this interface instead of checking
	// for the jump button directly so this script can also be used by AIs.
	private bool inputJump = false;
	public bool InputJump
	{
		get
		{
			return inputJump;
		}
		set
		{
			inputJump = value;
		}
	}
	
	[System.Serializable]
	public class CharacterMotorMovement
	{
		// The maximum horizontal speed when moving
		public float maxForwardSpeed = 10.0f;
		public float maxSidewaysSpeed = 10.0f;
		public float maxBackwardsSpeed = 10.0f;
		
		// Curve for multiplying speed based on slope (negative = downwards)
		public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));
		
		// How fast does the character change speeds?  Higher is faster.
		public float maxGroundAcceleration = 30.0f;
		public float maxAirAcceleration = 20.0f;
		
		// The gravity for the character
		public float gravity = 10.0f;
		public float maxFallSpeed = 20.0f;
		
		// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
		// Very handy for organization!
	
		// The last collision flags returned from controller.Move
		private CollisionFlags collisionFlags;
		public CollisionFlags TheCollisionFlags
		{
			get
			{
				return collisionFlags;
			}
			set
			{
				collisionFlags = value;
			}
		}
	
		// We will keep track of the character's current velocity,
		private Vector3 velocity;
		public Vector3 Velocity
		{
			get
			{
				return velocity;
			}
			set
			{
				velocity = value;
			}
		}
		
		// This keeps track of our current velocity while we're not grounded
		private Vector3 frameVelocity = Vector3.zero;
		public Vector3 FrameVelocity
		{
			get
			{
				return frameVelocity;
			}
			set
			{
				frameVelocity = value;
			}
		}
		
		private Vector3 hitPoint = Vector3.zero;
		public Vector3 HitPoint
		{
			get
			{
				return hitPoint;
			}
			set
			{
				hitPoint = value;
			}
		}
		
		private Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
		public Vector3 LastHitPoint
		{
			get
			{
				return lastHitPoint;
			}
			set
			{
				lastHitPoint = value;
			}
		}
	}
	
	[SerializeField] private CharacterMotorMovement movement = new CharacterMotorMovement();
	public CharacterMotorMovement Movement
	{
		get
		{
			return movement;
		}
	}
	
	enum MovementTransferOnJump
	{
		None, // The jump is not affected by velocity of floor at all.
		InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
		PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
		PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
	}
	
	// We will contain all the jumping related variables in one helper class for clarity.
	[System.Serializable]
	private class CharacterMotorJumping
	{
		// Can the character jump?
		public bool enabled = true;
	
		// How high do we jump when pressing jump and letting go immediately
		public float baseHeight = 1.0f;
		
		// We add extraHeight units (meters) on top when holding the button down longer while jumping
		public float extraHeight = 4.1f;
		
		// How much does the character jump out perpendicular to the surface on walkable surfaces?
		// 0 means a fully vertical jump and 1 means fully perpendicular.
		public float perpAmount = 0.0f;
		
		// How much does the character jump out perpendicular to the surface on too steep surfaces?
		// 0 means a fully vertical jump and 1 means fully perpendicular.
		public float steepPerpAmount = 0.5f;
		
		// For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
		// Very handy for organization!
	
		// Are we jumping? (Initiated with jump button and not grounded yet)
		// To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
		private bool jumping = false;
		public bool Jumping
		{
			get
			{
				return jumping;
			}
			set
			{
				jumping = value;
			}
		}
		
		private bool holdingJumpButton = false;
		public bool HoldingJumpButton
		{
			get
			{
				return holdingJumpButton;
			}
			set
			{
				holdingJumpButton = value;
			}
		}
		
		// the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
		private float lastStartTime = 0.0f;
		public float LastStartTime
		{
			get
			{
				return lastStartTime;
			}
			set
			{
				lastStartTime = value;
			}
		}
		
		private float lastButtonDownTime = -100;
		public float LastButtonDownTime
		{
			get
			{
				return lastButtonDownTime;
			}
			set
			{
				lastButtonDownTime = value;
			}
		}
		
		private Vector3 jumpDir = Vector3.up;
		public Vector3 JumpDir
		{
			get
			{
				return jumpDir;
			}
			set
			{
				jumpDir = value;
			}
		}
	}
	
	[SerializeField] private CharacterMotorJumping jumping = new CharacterMotorJumping();
	
	[System.Serializable]
	class CharacterMotorMovingPlatform
	{
		public bool enabled = true;
		public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;
		public Transform hitPlatform;
		public Transform activePlatform;
		public Vector3 activeLocalPoint;
		public Vector3 activeGlobalPoint;
		public Quaternion activeLocalRotation;
		public Quaternion activeGlobalRotation;
		public Matrix4x4 lastMatrix;
		public Vector3 platformVelocity;
		public bool newPlatform;
	}
	
	[SerializeField] private CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();
	
	[System.Serializable]
	private class CharacterMotorSliding
	{
		// Does the character slide on too steep surfaces?
		public bool enabled = true;
		
		// How fast does the character slide on steep surfaces?
		public float slidingSpeed = 15f;
		
		// How much can the player control the sliding direction?
		// If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
		public float sidewaysControl = 1.0f;
		
		// How much can the player influence the sliding speed?
		// If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
		public float speedControl = 0.4f;
	}
	
	[SerializeField] private CharacterMotorSliding sliding = new CharacterMotorSliding();
	
	private bool grounded = true;
	private Vector3 groundNormal = Vector3.zero;
	private Vector3 lastGroundNormal = Vector3.zero;
	private Transform tr;
	private CharacterController controller;
	
	private void Awake()
	{
		controller = GetComponent<CharacterController>();
		tr = transform;
	}
	
	private void UpdateFunction()
	{
		// We copy the actual velocity into a temporary variable that we can manipulate.
		Vector3 velocity = movement.Velocity;
		
		// Update velocity based on input
		velocity = ApplyInputVelocityChange(velocity);
		
		// Apply gravity and jumping force
		velocity = ApplyGravityAndJumping (velocity);
		
		// Moving platform support
		Vector3 moveDistance = Vector3.zero;
		if (MoveWithPlatform())
		{
			Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
			moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
			if (moveDistance != Vector3.zero)
			{
				controller.Move(moveDistance);
			}
			
			// Support moving platform rotation as well:
	        Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
	        Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);
	        
	        var yRotation = rotationDiff.eulerAngles.y;
	        if (yRotation != 0)
			{
		        // Prevent rotation of the local up vector
		        tr.Rotate(0, yRotation, 0);
	        }
		}
		
		// Save lastPosition for velocity calculation.
		Vector3 lastPosition = tr.position;
		
		// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
		Vector3 currentMovementOffset = velocity * Time.deltaTime;
		
		// Find out how much we need to push towards the ground to avoid loosing grouning
		// when walking down a step or over a sharp change in slope.
		float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
		if (grounded)
		{
			currentMovementOffset -= pushDownOffset * Vector3.up;
		}
		
		// Reset variables that will be set by collision function
		movingPlatform.hitPlatform = null;
		groundNormal = Vector3.zero;
		
	   	// Move our character!
		movement.TheCollisionFlags = controller.Move (currentMovementOffset);
		
		movement.LastHitPoint = movement.HitPoint;
		lastGroundNormal = groundNormal;
		
		if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform)
		{
			if (movingPlatform.hitPlatform != null)
			{
				movingPlatform.activePlatform = movingPlatform.hitPlatform;
				movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
				movingPlatform.newPlatform = true;
			}
		}
		
		// Calculate the velocity based on the current and previous position.  
		// This means our velocity will only be the amount the character actually moved as a result of collisions.
		Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
		movement.Velocity = (tr.position - lastPosition) / Time.deltaTime;
		Vector3 newHVelocity = new Vector3(movement.Velocity.x, 0, movement.Velocity.z);
		
		// The CharacterController can be moved in unwanted directions when colliding with things.
		// We want to prevent this from influencing the recorded velocity.
		if (oldHVelocity == Vector3.zero)
		{
			movement.Velocity = new Vector3(0, movement.Velocity.y, 0);
		}
		else
		{
			float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
			movement.Velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.Velocity.y * Vector3.up;
		}
		
		if (movement.Velocity.y < velocity.y - 0.001)
		{
			if (movement.Velocity.y < 0)
			{
				// Something is forcing the CharacterController down faster than it should.
				// Ignore this
				var temp = movement.Velocity;
				temp.y = velocity.y;
				movement.Velocity = temp;
			}
			else
			{
				// The upwards movement of the CharacterController has been blocked.
				// This is treated like a ceiling collision - stop further jumping here.
				jumping.HoldingJumpButton = false;
			}
		}
		
		// We were grounded but just loosed grounding
		if (grounded && !IsGroundedTest())
		{
			grounded = false;
			
			// Apply inertia from platform
			if (movingPlatform.enabled &&
				(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
				movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
			)
			{
				movement.FrameVelocity = movingPlatform.platformVelocity;
				movement.Velocity += movingPlatform.platformVelocity;
			}
			
			SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			// We pushed the character down to ensure it would stay on the ground if there was any.
			// But there wasn't so now we cancel the downwards offset to make the fall smoother.
			tr.position += pushDownOffset * Vector3.up;
		}
		// We were not grounded but just landed on something
		else if (!grounded && IsGroundedTest())
		{
			grounded = true;
			jumping.Jumping = false;
			SubtractNewPlatformVelocity();
			
			SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
		}
		
		// Moving platforms support
		if (MoveWithPlatform())
		{
			// Use the center of the lower half sphere of the capsule as reference point.
			// This works best when the character is standing on moving tilting platforms. 
			movingPlatform.activeGlobalPoint = tr.position + Vector3.up * (controller.center.y - controller.height * 0.5f + controller.radius);
			movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
			
			// Support moving platform rotation as well:
	        movingPlatform.activeGlobalRotation = tr.rotation;
	        movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation; 
		}
	}
	
	private void FixedUpdate ()
	{
		if (movingPlatform.enabled)
		{
			if (movingPlatform.activePlatform != null)
			{
				if (!movingPlatform.newPlatform)
				{
					Vector3 lastVelocity = movingPlatform.platformVelocity;
					
					movingPlatform.platformVelocity = (
						movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
						- movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
					) / Time.deltaTime;
				}
				movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
				movingPlatform.newPlatform = false;
			}
			else
			{
				movingPlatform.platformVelocity = Vector3.zero;	
			}
		}
		
		if (useFixedUpdate)
		{
			UpdateFunction();
		}
	}
	
	private void Update ()
	{
		if (!useFixedUpdate)
		{
			UpdateFunction();
		}
	}
	
	private Vector3 ApplyInputVelocityChange (Vector3 velocity)
	{
		if (!canControl)
		{
			inputMoveDirection = Vector3.zero;
		}
		
		// Find desired velocity
		Vector3 desiredVelocity;
		if (grounded && TooSteep())
		{
			// The direction we're sliding in
			desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
			// Find the input movement direction projected onto the sliding direction
			var projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
			// Add the sliding direction, the spped control, and the sideways control vectors
			desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
			// Multiply with the sliding speed
			desiredVelocity *= sliding.slidingSpeed;
		}
		else
		{
			desiredVelocity = GetDesiredHorizontalVelocity();
		}
		
		if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
		{
			desiredVelocity += movement.FrameVelocity;
			desiredVelocity.y = 0;
		}
		
		if (grounded)
		{
			desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
		}
		else
		{
			velocity.y = 0;
		}
		
		// Enforce max velocity change
		float maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime;
		Vector3 velocityChangeVector = (desiredVelocity - velocity);
		if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange)
		{
			velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
		}
		// If we're in the air and don't have control, don't apply any velocity change at all.
		// If we're on the ground and don't have control we do apply it - it will correspond to friction.
		if (grounded || canControl)
		{
			velocity += velocityChangeVector;
		}
		
		if (grounded)
		{
			// When going uphill, the CharacterController will automatically move up by the needed amount.
			// Not moving it upwards manually prevent risk of lifting off from the ground.
			// When going downhill, DO move down manually, as gravity is not enough on steep hills.
			velocity.y = Mathf.Min(velocity.y, 0);
		}
		
		return velocity;
	}
	
	private Vector3 ApplyGravityAndJumping (Vector3 velocity)
	{
		if (!inputJump || !canControl)
		{
			jumping.HoldingJumpButton = false;
			jumping.LastButtonDownTime = -100;
		}
		
		if (inputJump && jumping.LastButtonDownTime < 0 && canControl)
		{
			jumping.LastButtonDownTime = Time.time;
		}
		
		if (grounded)
		{
			velocity.y = Mathf.Min(0, velocity.y) - movement.gravity * Time.deltaTime;
		}
		else
		{
			velocity.y = movement.Velocity.y - movement.gravity * Time.deltaTime;
			
			// When jumping up we don't apply gravity for some time when the user is holding the jump button.
			// This gives more control over jump height by pressing the button longer.
			if (jumping.Jumping && jumping.HoldingJumpButton)
			{
				// Calculate the duration that the extra jump force should have effect.
				// If we're still less than that duration after the jumping time, apply the force.
				if (Time.time < jumping.LastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight))
				{
					// Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
					velocity += jumping.JumpDir * movement.gravity * Time.deltaTime;
				}
			}
			
			// Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
			velocity.y = Mathf.Max (velocity.y, -movement.maxFallSpeed);
		}
			
		if (grounded)
		{
			// Jump only if the jump button was pressed down in the last 0.2 seconds.
			// We use this check instead of checking if it's pressed down right now
			// because players will often try to jump in the exact moment when hitting the ground after a jump
			// and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
			// it's confusing and it feels like the game is buggy.
			if (jumping.enabled && canControl && (Time.time - jumping.LastButtonDownTime < 0.2))
			{
				grounded = false;
				jumping.Jumping = true;
				jumping.LastStartTime = Time.time;
				jumping.LastButtonDownTime = -100;
				jumping.HoldingJumpButton = true;
				
				// Calculate the jumping direction
				if (TooSteep())
				{
					jumping.JumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
				}
				else
				{
					jumping.JumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
				}
				
				// Apply the jumping force to the velocity. Cancel any vertical velocity first.
				velocity.y = 0;
				velocity += jumping.JumpDir * CalculateJumpVerticalSpeed (jumping.baseHeight);
				
				// Apply inertia from platform
				if (movingPlatform.enabled &&
					(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
					movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
				)
				{
					movement.FrameVelocity = movingPlatform.platformVelocity;
					velocity += movingPlatform.platformVelocity;
				}
				
				SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				jumping.HoldingJumpButton = false;
			}
		}
		
		return velocity;
	}
	
	private void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0)
		{
			if ((hit.point - movement.LastHitPoint).sqrMagnitude > 0.001 || lastGroundNormal == Vector3.zero)
			{
				groundNormal = hit.normal;
			}
			else
			{
				groundNormal = lastGroundNormal;
			}
			
			movingPlatform.hitPlatform = hit.collider.transform;
			movement.HitPoint = hit.point;
			movement.FrameVelocity = Vector3.zero;
		}
	}
	
	private void SubtractNewPlatformVelocity()
	{
		StartCoroutine(SubtractNewPlatformVelocityRoutine());
	}
	
	private IEnumerator SubtractNewPlatformVelocityRoutine()
	{
		// When landing, subtract the velocity of the new ground from the character's velocity
		// since movement in ground is relative to the movement of the ground.
		if (movingPlatform.enabled &&
			(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
			movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
		)
		{
			// If we landed on a new platform, we have to wait for two FixedUpdates
			// before we know the velocity of the platform under the character
			if (movingPlatform.newPlatform)
			{
				Transform platform = movingPlatform.activePlatform;
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				if (grounded && platform == movingPlatform.activePlatform)
				{
					yield return new WaitForSeconds(1);
				}
			}
			movement.Velocity -= movingPlatform.platformVelocity;
		}
	}
	
	private bool MoveWithPlatform ()
	{
		return (
			movingPlatform.enabled
			&& (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
			&& movingPlatform.activePlatform != null
		);
	}
	
	private Vector3 GetDesiredHorizontalVelocity()
	{
		// Find desired velocity
		Vector3 desiredLocalDirection = tr.InverseTransformDirection(inputMoveDirection);
		float maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
		if (grounded)
		{
			// Modify max speed on slopes based on slope speed multiplier curve
			var movementSlopeAngle = Mathf.Asin(movement.Velocity.normalized.y)  * Mathf.Rad2Deg;
			maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
		}
		return tr.TransformDirection(desiredLocalDirection * maxSpeed);
	}
	
	private Vector3 AdjustGroundVelocityToNormal (Vector3 hVelocity, Vector3 groundNormal)
	{
		Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
	}
	
	private bool IsGroundedTest ()
	{
		return (groundNormal.y > 0.01);
	}
	
	private float GetMaxAcceleration (bool grounded)
	{
		// Maximum acceleration on ground and in air
		if (grounded)
		{
			return movement.maxGroundAcceleration;
		}
		else
		{
			return movement.maxAirAcceleration;
		}
	}
	
	private float CalculateJumpVerticalSpeed (float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
	}
	
	public bool IsJumping()
	{
		return jumping.Jumping;
	}
	
	public bool IsSliding()
	{
		return (grounded && sliding.enabled && TooSteep());
	}
	
	public bool IsTouchingCeiling()
	{
		return (movement.TheCollisionFlags & CollisionFlags.CollidedAbove) != 0;
	}
	
	public bool IsGrounded()
	{
		return grounded;
	}
	
	public bool TooSteep()
	{
		return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
	}
	
	public Vector3 GetDirection()
	{
		return inputMoveDirection;
	}
	
	public void SetControllable (bool controllable)
	{
		canControl = controllable;
	}
	
	// Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
	// The function returns the length of the resulting vector.
	public float MaxSpeedInDirection (Vector3 desiredMovementDirection)
	{
		if (desiredMovementDirection == Vector3.zero)
		{
			return 0;
		}
		else
		{
			float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
			Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
			float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
			return length;
		}
	}
	
	public void SetVelocity (Vector3 velocity)
	{
		grounded = false;
		movement.Velocity = velocity;
		movement.FrameVelocity = Vector3.zero;
		SendMessage("OnExternalVelocity");
	}

}
