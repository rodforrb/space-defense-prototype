using Godot;
using System;

public class Start : Button
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
	
	// this is attached to the "Button" node
	private void _on_Button_pressed()
	{
	    GetNode<Button>("Panel/Button").Text = "HELLO!";
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
