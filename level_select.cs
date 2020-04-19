using Godot;
using System;

public class level_select : Node2D
{
	// Called when the node enters the scene tree for the first time.	
	public override void _Ready()
	{
		Label curr = (Label) this.GetNode("Panel/Currency");
		Label diff = (Label) this.GetNode("Panel/CurrentDifficulty");
		Label selected = (Label) this.GetNode("Panel/CurrentLevel");
		Sprite arrow = (Sprite) this.GetNode("Grid/Selected/Arrow");
		curr.SetText("Currency: " +  (Loot.Loot.getValue()).ToString());
		int l = State.currentLevel;
		if(l == 1){
			diff.SetText("Current Difficulty:\n 1");
			selected.SetText("Current Selected Level:\n Gala");
			arrow.SetGlobalPosition(new Vector2 (254,222));
		}else if (l == 2){
			diff.SetText("Current Difficulty:\n 1");
			selected.SetText("Current Selected Level:\n Keplar");
			arrow.SetGlobalPosition(new Vector2 (600,311));
		}else if (l == 3){
			diff.SetText("Current Difficulty:\n 2");
			selected.SetText("Current Selected Level:\n Gilese");
			arrow.SetGlobalPosition(new Vector2 (950,157));
		}else{
			diff.SetText("Current Difficulty:\n NULL");
			selected.SetText("Current Selected Level:\n NULL");
			arrow.SetGlobalPosition(new Vector2 (254,222));
		}
		
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


