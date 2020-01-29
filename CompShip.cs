using Godot;
using System;
using System.Threading.Tasks;

public class CompShip : Ship1
{
	/*
	name or id?
	*/
	private int range = 2;
	private int maxAP = 2;
	new public int shipType = 1;
	public const int maxHP = 50;//maximum hp
	new public int HP { get; set;} = 50;//current hp
	new public int penetration { get; set; } = 5;//the ships ability to ignore armour
	new public int armour { get; set; } = 5;//the ships resistance to damage
	new public int accuracy { get; set; } = 5;//odds of hitting an opponent
	new public int evasion { get; set; } = 5;//odds of dodging an attack

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
			
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	//todo: move, validate move, ensure inboundness

	// requests the ship's actions to the grid
	//Todo:
	//public void PlayTurn(Array of player ships)
	//also: make this a bool
	//that returns true when the turn is complete
	//this could be useful for switching turns
	public async void PlayTurn(Ship1 target)
	{
		Vector2 dist;

		int fight;
		//todo: add logic comparing stats
		//ie, pass this function an array of ships, try to destroy weakest, or run from strongest
		//have some var like fight, 0 = run, 1 = fight, 2 = freeze, 3 = something like "support" idk
		if(this.HP <= target.firepower && this.firepower < target.HP){
			fight = 0;
		}
		else{
			fight = 1;
		}
		
		for (int i = 0; i < maxAP; i++){


			Vector2 shipCell = this.GetPosition();
			Node2D compShip = (Node2D)this;
			Vector2 targetCell = target.Position; 
			int gridSize = ((Grid) GetNode("/root/Game/Grid")).gridSize;
			Vector2 moveNorth = new Vector2(shipCell.x, shipCell.y - 1 * gridSize);
			Vector2 moveEast = new Vector2(shipCell.x + 1 * gridSize, shipCell.y);
			Vector2 moveSouth = new Vector2(shipCell.x, shipCell.y + 1 * gridSize);
			Vector2 moveWest = new Vector2(shipCell.x - 1 * gridSize, shipCell.y);


			dist = (targetCell - shipCell)/gridSize;
			GD.Print(dist);
			//to do
			GD.Print("Test", i);
			if(fight==1){			
				if((Math.Abs(dist.x)+Math.Abs(dist.y)) <= range){
					((Grid) GetNode("/root/Game/Grid")).attack(this, target, new Projectile(ProjectileType.Gun));
				}
				else if(Math.Abs(dist.x) < Math.Abs(dist.y) ){
					if(dist.y > 0){
						compShip.SetPosition(moveSouth);
					}
					else{
						compShip.SetPosition(moveNorth);
					}
				}
				else if(Math.Abs(dist.x) >= Math.Abs(dist.y) ){
					if(dist.x > 0){
						compShip.SetPosition(moveEast);
					}
					else{
						compShip.SetPosition(moveWest);
					}

				}
			}
			else if(fight == 0){
				if(Math.Abs(dist.x) <= Math.Abs(dist.y) ){
					if(dist.y > 0){
						compShip.SetPosition(moveNorth);
					}
					else{
						compShip.SetPosition(moveSouth);
					}
				}
				else if(Math.Abs(dist.x) > Math.Abs(dist.y) ){
					if(dist.x > 0){
						compShip.SetPosition(moveWest);
					}
					else{
						compShip.SetPosition(moveEast);
					}

				}				
			}


			await Task.Delay(TimeSpan.FromSeconds(0.2));
			
		}
	}
	
}
