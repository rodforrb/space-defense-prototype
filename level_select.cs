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
		switch (State.currentLevel)
		{
			case 1:
				GetTree().ChangeScene("res://Level1.tscn");
				break;
			case 2:
				GetTree().ChangeScene("res://Level2.tscn");
				break;
			case 3:
				GetTree().ChangeScene("res://Level3.tscn");
				break;
			default:
				GetTree().ChangeScene("res://Level0.tscn");
				break;
		}
	}
}


