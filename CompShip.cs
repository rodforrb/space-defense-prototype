using Godot;
using System;

public class CompShip : Ship1
{
	/*
    public const int maxHP = 50;//maximum hp
    public int HP { get; set;} = 50;//current hp

    public int firepower { get; set; } = 5;//the ships firepower multiplier
    public int penetration { get; set; } = 5;//the ships ability to ignore armour
    public int armour { get; set; } = 5;//the ships resistance to damage
    public int accuracy { get; set; } = 5;//odds of hitting an opponent
    public int evasion { get; set; } = 5;//odds of dodging an attack
	*/
	
	
    private int range = 5;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	// requests the ship's actions to the grid
	public void PlayTurn()
	{
		Vector2 location = this.GetPosition();
		Random r = new Random();
		int randDirection = r.Next(0, 4); //0 = north, 1 east, 2 south, 3 west
		
		switch (randDirection){
			// todo: is statically referencing the grid the best way ?
			case 0:
				((Grid) GetNode("/root/Game/Grid")).move(this, location+Vector2.Up);
				break;			
			case 1:
				((Grid) GetNode("/root/Game/Grid")).move(this, location+Vector2.Right);
				break;
			case 2:
				((Grid) GetNode("/root/Game/Grid")).move(this, location+Vector2.Down);
				break;			
			case 3:
				((Grid) GetNode("/root/Game/Grid")).move(this, location+Vector2.Left);
				break;
		}
	}
}
