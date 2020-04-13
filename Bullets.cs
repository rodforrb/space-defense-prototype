using Godot;
using System;
//Area2D?
//RigidBody2D?
public class Bullets : Area2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	//Node2D Grid = GetParent();
	
	public int move_speed = 1000;
	public Vector2 direction;
	public Vector2 startPos;
	public Vector2 currentPos;
	public Vector2 endPos;
	public int firepow {get; set;}
	public int penetra {get; set;}
	public int accurat {get; set;}
	private PackedScene _bullet;
	
	[Signal]
	delegate void hit_target(Vector2 cell, int fp, int pen);
	
	public PackedScene Bullet
	{
		get { return _bullet; }
		set
		{
			if (Engine.IsEditorHint())
			{
				_bullet = value;
			}
		}
	}
	
//	    // Warn users if the value hasn't been set.
//    public String _GetConfigurationWarning()
//    {
//        if (Bullet == null)
//            return "Must initialize property 'Bullet'.";
//        return "";
//    }
	
	//constructor with parameters
	public Bullets(Vector2 st, Vector2 en, int f, int p, int a){
		direction = en - st;
		startPos = st;
		endPos = en;
		currentPos = st;
		firepow = f;
		penetra = p;
		accurat = a;
	}
	
	public void start_at(Vector2 st, Vector2 en, int f, int p, int a){
		direction = en - st;
		startPos = st;
		endPos = en;
		currentPos = st;
		firepow = f;
		penetra = p;
		accurat = a;
	}
	
	public Vector2 getPosition()
	{
		return currentPos;
	}
	
	//constructor without parameters
	public Bullets(){
		
	}
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//set_as_toplevel(true);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (currentPos != endPos)
		{
			//move towards the goal
			currentPos += direction * (int)Math.Ceiling(delta);
		}
		else 
		{
			
			EmitSignal("hit_target", endPos, firepow, penetra, accurat);
			
			
			Free();
		}
	}
	
	
	private void _on_VisibilityNotifier2D_screen_exited()
	{
		//Free();
		//queue_free();//destroy as soon as possible
	}
	private void _on_Bullets_area_entered(object area)
	{
		// Replace with function body.
		//move
		if (currentPos == endPos)
		{
			EmitSignal("hit_target", endPos, firepow, penetra, accurat);

		}
		
		Free();
	}
	
	

}





