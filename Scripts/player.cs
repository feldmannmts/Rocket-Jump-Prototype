using Godot;
using System;

public partial class player : CharacterBody3D
{
	public float current_speed = 5.0f;
	

	[ExportCategory("Movement")]
	[Export] 
	public float walking_speed = 5f;
	[Export] 
	public float sprinting_speed = 8f;
	[Export] 
	public float crouching_speed = 3f;
	[Export] 
	public float jump_velocity = 4.5f;
	[Export] float crouching_depth = -0.25f;
	[Export] float crouch_lerp_speed = 10f;
	private float default_head_height = 0.5f;

	[ExportCategory("Mouse")]
	[Export] public float mouse_sens = 0.4f;



	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		default_head_height = GetNode<Node3D>("Head").Position.Y;
	}

	public override void _PhysicsProcess(double delta)
	{
		
		if (!Input.IsActionPressed("crouch"))
		{
			ResetCrouching(delta);
			if(!Input.IsActionPressed("sprint")){
				Walking();
			}else{
				Sprinting();
			}
		}else{
			Crouching(delta);
		}

		PlayerMovement(delta);
	}

	
	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseMotion mouseMotionEvent){
			RotateY(Mathf.DegToRad(mouseMotionEvent.Relative.X * mouse_sens) * -1);
			Node3D _head = GetNode<Node3D>("Head");

			_head.RotateX(Mathf.DegToRad(mouseMotionEvent.Relative.Y * mouse_sens) * -1);

			//Clamping the vertical movement of the camera
			_head.Rotation = new Godot.Vector3(
				Mathf.Clamp(_head.Rotation.X, Mathf.DegToRad(-89f),Mathf.DegToRad(89f)),
				_head.Rotation.Y,
				_head.Rotation.Z
			);
		}
	}

	private void Walking(){	
		current_speed = walking_speed;			
	}
	private void Sprinting(){	
		current_speed = sprinting_speed;			
	}
	private void Crouching(double delta){
		
		current_speed = crouching_speed;
		Node3D _head = GetNode<Node3D>("Head");

		Vector3 crouch_head_height = new Vector3(
			_head.Position.X,
			Mathf.Clamp(_head.Position.Y, Mathf.DegToRad(_head.Position.Y + crouching_depth),Mathf.DegToRad(default_head_height)),
			_head.Position.Z
		);

		_head.Position = _head.Position.Lerp(crouch_head_height, (float)(crouch_lerp_speed * delta));
	}
	private void ResetCrouching(double delta) {
			Node3D _head = GetNode<Node3D>("Head");
			Vector3 reset_head_height = new Vector3(
			_head.Position.X,
			default_head_height,
			_head.Position.Z
		);	

		_head.Position = _head.Position.Lerp(reset_head_height, (float)(crouch_lerp_speed * delta));
	}
	private void PlayerMovement(double delta){
		
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = jump_velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * current_speed;
			velocity.Z = direction.Z * current_speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, current_speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, current_speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
