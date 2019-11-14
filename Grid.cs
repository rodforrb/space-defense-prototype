using Godot;
using System;

/*	This is the main code for controlling objects on the Grid */

public class Grid : TileMap
{
	// where the mouse was last clicked (for click and drag)
	private int mouseClick;
	// currently selected node/sprite
	private Node2D selected = null;
	
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// iterate children of this node (all the Node2D characters)
		for (int i = 0; i < GetChildCount(); i++)
		{
			Node2D child = (Node2D)GetChild(i);
			// WorldToMap converts pixel coordinates to grid coordinates
			GD.Print("Node loaded: ", child, " at ", WorldToMap(child.GetPosition()));
		}
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
	
	// Called when there is input
	public override void _Input(InputEvent @event)
	{
		// mouse press/release event
		if (@event is InputEventMouseButton mouseClick)
		{
			// check if specifically left mouse press event (BUTTON_LEFT = 1)
			// list of enums here but I can't figure out how to access them: https://docs.godotengine.org/en/3.1/classes/class_@globalscope.html
			if (mouseClick.IsPressed() && mouseClick.GetButtonIndex() == 1)
			{
				Vector2 cell = WorldToMap(mouseClick.Position);
				GD.Print("Mouse Click at: ", mouseClick.Position, ", Cell: ", cell);
				
				if (this.selected == null)
				{
					// check if user clicked on something
					for (int i = 0; i < GetChildCount(); i++)
					{
						Node2D child = (Node2D)GetChild(i);
						if (cell == WorldToMap(child.GetPosition()))
						{
							this.selected = child;
							GD.Print("Selected: ", child);
							break;
						}
					}
				} else {
					// something was previously selected, decide what to do
					
				}
			}
		}
	}
}