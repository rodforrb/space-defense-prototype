using Godot;
using System;

public class level_select : Node2D
{
	Label curr;
	// Called when the node enters the scene tree for the first time.	
	public override void _Ready()
	{
		curr = (Label) this.GetNode("Panel/Currency");
		curr.Text = ("Currency: " +  (Loot.Loot.getValue()).ToString());
		int l = State.currentLevel;
		update(l);
	}
	
	private void update(int l)
	{
		Label diff = (Label) this.GetNode("Panel/CurrentDifficulty");
		Label selected = (Label) this.GetNode("Panel/CurrentLevel");
		Sprite arrow = (Sprite) this.GetNode("Grid/Selected/Arrow");
		TileMap grid = (TileMap) this.GetNode("TileMap");
		curr.Text = ("Currency: " +  (Loot.Loot.getValue()).ToString());
		int m = State.maxLevel;
		State.currentLevel = l;
		if(l == 1){
			diff.Text = ("Current Difficulty:\n 1");
			selected.Text = ("Current Selected Level:\n Gala");
			arrow.GlobalPosition = (new Vector2 (254,222));
		}else if (l == 2){
			diff.Text = ("Current Difficulty:\n 1");
			selected.Text = ("Current Selected Level:\n Keplar");
			arrow.GlobalPosition = (new Vector2 (600,311));
		}else if (l == 3){
			diff.Text = ("Current Difficulty:\n 2");
			selected.Text = ("Current Selected Level:\n Gilese");
			arrow.GlobalPosition = (new Vector2 (950,157));
		}else if (l == 4){
			diff.Text = ("Current Difficulty:\n 3");
			selected.Text = ("Current Selected Level:\n Gala (Ranarr)");
			arrow.GlobalPosition = (new Vector2 (249,439));
		}else if (l == 5){
			diff.Text = ("Current Difficulty:\n 3");
			selected.Text = ("Current Selected Level:\n Keplar (Ranarr)");
			arrow.GlobalPosition = (new Vector2 (595,86));
		}else if (l == 6){
			diff.Text = ("Current Difficulty:\n 4");
			selected.Text = ("Current Selected Level:\n Gilese (Ranarr)");
			arrow.GlobalPosition = (new Vector2 (952,411));
		}else{
			diff.Text = ("Current Difficulty:\n NULL");
			selected.Text = ("Current Selected Level:\n NULL");
			arrow.GlobalPosition = (new Vector2 (254,222));
		}
		if(m == 1){
			grid.SetCellv(new Vector2 (3,6), 43);
			grid.SetCellv(new Vector2 (14,9), 60);
			grid.SetCellv(new Vector2 (25,4), 61);
			grid.SetCellv(new Vector2 (3,13), 59);
			grid.SetCellv(new Vector2 (14,2), 60);
			grid.SetCellv(new Vector2 (25,12), 61);
		}else if (m == 2){
			grid.SetCellv(new Vector2 (3,6), 43);
			grid.SetCellv(new Vector2 (14,9), 44);
			grid.SetCellv(new Vector2 (25,4), 61);
			grid.SetCellv(new Vector2 (3,13), 59);
			grid.SetCellv(new Vector2 (14,2), 60);
			grid.SetCellv(new Vector2 (25,12), 61);
		}else if (m == 3){
			grid.SetCellv(new Vector2 (3,6), 43);
			grid.SetCellv(new Vector2 (14,9), 44);
			grid.SetCellv(new Vector2 (25,4), 45);
			grid.SetCellv(new Vector2 (3,13), 59);
			grid.SetCellv(new Vector2 (14,2), 60);
			grid.SetCellv(new Vector2 (25,12), 61);
		}else if (m == 4){
			grid.SetCellv(new Vector2 (3,6), 43);
			grid.SetCellv(new Vector2 (14,9), 44);
			grid.SetCellv(new Vector2 (25,4), 45);
			grid.SetCellv(new Vector2 (3,13), 43);
			grid.SetCellv(new Vector2 (14,2), 60);
			grid.SetCellv(new Vector2 (25,12), 61);

		}else if (m == 5){
			grid.SetCellv(new Vector2 (3,6), 43);
			grid.SetCellv(new Vector2 (14,9), 44);
			grid.SetCellv(new Vector2 (25,4), 45);
			grid.SetCellv(new Vector2 (3,13), 43);
			grid.SetCellv(new Vector2 (14,2), 44);
			grid.SetCellv(new Vector2 (25,12), 61);

		}else if (m == 6){
			grid.SetCellv(new Vector2 (3,6), 43);
			grid.SetCellv(new Vector2 (14,9), 44);
			grid.SetCellv(new Vector2 (25,4), 45);
			grid.SetCellv(new Vector2 (3,13), 43);
			grid.SetCellv(new Vector2 (14,2), 44);
			grid.SetCellv(new Vector2 (25,12), 45);

		}else{
			grid.SetCellv(new Vector2 (3,6), 59);
			grid.SetCellv(new Vector2 (14,9), 60);
			grid.SetCellv(new Vector2 (25,4), 61);
			grid.SetCellv(new Vector2 (3,13), 59);
			grid.SetCellv(new Vector2 (14,2), 60);
			grid.SetCellv(new Vector2 (25,12), 61);
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
			case 4:
				GetTree().ChangeScene("res://Level4.tscn");
				break;
			case 5:
				GetTree().ChangeScene("res://Level5.tscn");
				break;
			case 6:
				GetTree().ChangeScene("res://Level6.tscn");
				break;
			default:
				GetTree().ChangeScene("res://Level0.tscn");
				break;
		}
	}
	
	private void _on_selectGala_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
		{
			if (State.maxLevel >= 1){
				int l = 1;
				update(l);
			}
		}
	}

	private void _on_selectKeplar_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
		{
			if (State.maxLevel >= 2){
				int l = 2;
				update(l);
			}
		}
	}

	private void _on_selectGliese_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
		{
			if (State.maxLevel >= 3){
				int l = 3;
				update(l);
			}
		}
	}

	private void _on_selectGalaR_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
		{
			if (State.maxLevel >= 4){
				int l = 4;
				update(l);
			}
		}
	}

	private void _on_selectKeplarR_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
		{
			if (State.maxLevel >= 5){
				int l = 5;
				update(l);
			}
		}
	}

	private void _on_selectGlieseR_gui_input(object @event)
	{
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
		{
			if (State.maxLevel >= 6){
				int l = 6;
				update(l);
			}
		}
	}
	
	private void _on_Save_pressed()
	{
		State.Save();
	}
	
	
	private void _on_Exit_pressed()
	{
		GetTree().ChangeScene("res://MainMenu.tscn");
	}
}
