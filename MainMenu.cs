using Godot;
using System;

public class MainMenu : MarginContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
    //Load game (does nothing)
    public void _on_Continue_gui_input(InputEvent @event){
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
        {
            GetTree().ChangeScene("res://level_select.tscn");
        }
    }

    //New game
    public void _on_NewGame_gui_input(InputEvent @event){
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
        {
            GetTree().ChangeScene("res://Level1.tscn");
        }
    }
    //Options (does nothing)
    public void _on_Options_gui_input(InputEvent @event){
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
        {
			// TODO
        }
    }
    //leaves game
    public void _on_Quit_gui_input(InputEvent @event){
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
        {
            GetTree().Quit();
        }
    }    

    //test function
    /*
    public override void _GuiInput(InputEvent @event)
{
    if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
    {
        GD.Print("Left mouse button was pressed!");
    }
}
*/
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
