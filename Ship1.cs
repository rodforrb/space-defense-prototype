using Godot;
using System;

public class Ship1 : Node2D
{
	
    // Called when the node enters the scene tree for the first time.
    private int range = 3;

    public int GetRange()
    {
        return range;
    }

    
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
