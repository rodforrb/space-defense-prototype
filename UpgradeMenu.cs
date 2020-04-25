using Godot;
using System;
using System.Collections.Generic;

public class UpgradeMenu : Popup
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private List<Ship1> playerShips = new List<Ship1>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void addShip(Ship1 ship){
        this.playerShips.Add(ship);
    }

    public void printShips(){
        for(int i = 0; i < this.playerShips.Count; i++){
            GD.Print(this.playerShips[i]);
        }
    }
    public void showUpgradeMenu()
    {
        Show();
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
