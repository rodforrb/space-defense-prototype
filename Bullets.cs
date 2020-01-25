using Godot;
using System;
//Area2D?
//RigidBody2D?
public class Bullets : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	
	public int move_speed = 100;
	public Vector2 direction;
	public Vector2 startPos;
	public Vector2 currentPos;
	public Vector2 endPos;
	public int firepow {get; set;}
	public int penetra {get; set;}
	public int accurat {get; set;}
	public int eva {get; set;}
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
	public Bullets(Vector2 st, Vector2 en, int f, int p, int a, int ev){
		direction = en - st;
		startPos = st;
		endPos = en;
		currentPos = st;
		firepow = f;
		penetra = p;
		accurat = a;
		eva = ev;
	}
	
	public void Bulle(Vector2 st, Vector2 en, int f, int p, int a, int ev){
		direction = en - st;
		startPos = st;
		endPos = en;
		currentPos = st;
		firepow = f;
		penetra = p;
		accurat = a;
		eva = ev;
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
			//move
			float hits = (float)accurat / (float)(accurat + eva);
			int chance = (int) (hits * 100);
			Random random = new Random();
			int result = random.Next(0, 100);
			
			if (result <= chance)
			{
				EmitSignal("hit_target", endPos, firepow, penetra);
			}
			else
			{
				//EmitSignal("missed_target");
			}
			
			Free();
		}
	}
	
	
	private void _on_VisibilityNotifier2D_screen_exited()
	{
		Free();
		//queue_free();//destroy as soon as possible
	}
	private void _on_Bullets_area_entered(object area)
	{
		// Replace with function body.
		//move
		if (currentPos == endPos)
		{
			float hits = (float)accurat / (float)(accurat + eva);
			int chance = (int) (hits * 100);
			Random random = new Random();
			int result = random.Next(0, 100);
			
			if (result <= chance)
			{
				EmitSignal("hit_target", endPos, firepow, penetra);
			}
			else
			{
				//EmitSignal("missed_target");
			}
		}
		
		Free();
	}
	
	

}





