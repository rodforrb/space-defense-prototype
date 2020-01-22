using Godot;
using System;

public class level_select : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
	
	// load a level
	private void _on_Start_pressed()
	{
			// loads a level
	        GetTree().ChangeScene("res://main.tscn");
	}
}


