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

	[ExportCategory("Mouse")]
	[Export] public float mouse_sens = 0.4f;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion mouseMotionEvent){
			RotateY(Mathf.DegToRad(mouseMotionEvent.Relative.X));
		}
    }



    public override void _PhysicsProcess(double delta)
	{

		if(Input.IsActionPressed("sprint")){
			current_speed = sprinting_speed;
		}else 
			current_speed = walking_speed;

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
