using Godot;
using System;

public class main : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	private bool compTurn = false; 

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

	// this is attached to the "Button" node
	private void _on_Button_pressed()
	{
	    GetNode<Button>("Panel/Button").Text = "HELLO!";
	}

	private void _on_CompTurn_pressed()
	{
		//the bool is currently only a sample, in actual use case
		// will probably need some sort of global var
		if (compTurn){
			GetNode<Button>("Panel/CompTurn").Text = "turn state 1";
			//do?
			compTurn = false;
		}
		else{
			GetNode<Button>("Panel/CompTurn").Text = "turn state 2";;
			compTurn = true;
			
		}
	    
	}
	public override void _Process(float delta)
	{
		if (Input.IsActionPressed("ui_cancel"))
		{
			GetTree().ChangeScene("res://MainMenu.tscn");
		}
	}		
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
