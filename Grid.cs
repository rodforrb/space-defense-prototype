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
				
				// check if user clicked on something
				for (int i = 0; i < GetChildCount(); i++)
				{
					Node2D child = (Node2D)GetChild(i);
					// if user clicked on a child
					if (cell == WorldToMap(child.GetPosition()))
					{
						if (this.selected == null)
						{
							this.selected = child;
							GD.Print("Selected: ", child);
							break;
						} else {
							// something was previously selected, decide what to do
							// clicked new child: update current selection
							if (this.selected != child)
							{
								this.selected = child;
								GD.Print("Selected: ", child);
								break;
							}
						}
					}
					// clicked any other cell: move the selected ship
					// MapToWorld converts grid to pixel coordinates
					if (this.selected != null)
					{
						this.selected.SetPosition(MapToWorld(cell));
						GD.Print(this.selected, " moved to ", cell);
					}
				}
			}
			
			// right click
			if (mouseClick.IsPressed() && mouseClick.GetButtonIndex() == 2)
			{
				// eg. clear selection
				if (this.selected != null)
				{
					GD.Print("Deselected ", this.selected);
					this.selected = null;
				}
			}
		}
	}
}