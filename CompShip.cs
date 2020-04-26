using Godot;
using System;
using System.Threading.Tasks;

public class CompShip : Ship1
{
	/*
	name or id?
	*/
	private int maxRange = 3;
	new public int range {get; set;} = 3;
	private int maxAP = 5;
	new public int AP {get; set;} = 5;
	
	[Export]
	new public Team team {get;} = Team.Computer;
	[Export]
	public Type type {get; set; } = Type.Medium;
	
		public int firepower { get; set; } = 5;//the ships firepower multiplier
	public int penetration { 
		get{
			if (type == Type.Destroyer) 
				return 7; 
			else if (type == Type.Sniper) 
				return 5; 
			else 
				return 5;
		} 
		set{
			this.penetration = value;
		}
	}//the ships ability to ignore armour
	public int armour { 
		get{
			if (type == Type.Heavy) 
				return 6; 
			else if (type == Type.Lite) 
				return 3; 
			else 
				return 5;
		} 
		set{
			this.armour = value;
		}
	}//the ships resistance to damage
	public int accuracy { 
		get{
			if (type == Type.Destroyer) 
				return 10; 
			else if (type == Type.Sniper) 
				return 20; 
			else 
				return 15;
		} 
		set{
			this.accuracy = value;
		}
	}//odds of hitting an opponent
	public int evasion { 
		get{
			if (type == Type.Heavy) 
				return 3; 
			else if (type == Type.Lite) 
				return 10; 
			else 
				return 5;
		} 
		set{
			this.evasion = value;
		}
	}//odds of dodging an attack
	/* Used to get instantiated Grid object
	 * Unfortunately GetNode cannot be used by a static class.
	 * @return Grid
	*/
	public Grid GetGrid ()
	{
		return (GetNode<Grid>("/root/Game/Grid"));
	}
	
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
	//Todo:
	//public void PlayTurn(Array of player ships)
	//also: make this a bool
	//that returns true when the turn is complete
	//this could be useful for switching turns
	public void PlayTurn()
	{
		// nothing to do but game might not have ended yet because of async
		try
		{
		if (GetGrid().playerShips.Count == 0) return;
		} catch (System.Exception e) {
			return;
		}

		Ship1 target = GetGrid().playerShips[0];
		float distance = 10000000;
		foreach (Ship1 ship in GetGrid().playerShips)
		{
			float lenSquared = (this.Position - ship.Position).LengthSquared();
			if (lenSquared < distance)
			{
				target = ship;
				distance = lenSquared;
			}
		}


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

			Vector2 shipCell = GetGrid().WorldToMap(this.GetPosition());
			Vector2 targetCell = GetGrid().WorldToMap(target.Position); 
			Vector2 moveNorth = new Vector2(shipCell.x, shipCell.y - 1);
			Vector2 moveEast = new Vector2(shipCell.x + 1, shipCell.y);
			Vector2 moveSouth = new Vector2(shipCell.x, shipCell.y + 1);
			Vector2 moveWest = new Vector2(shipCell.x - 1, shipCell.y);


			Vector2 difference = (targetCell - shipCell);
			if(fight==1){			
				if((Math.Abs(difference.x)+Math.Abs(difference.y)) <= range){
					GetGrid().Attack(this, target, new Projectile(ProjectileType.Gun));
				}
				else if(Math.Abs(difference.x) < Math.Abs(difference.y) ){
					if(difference.y > 0){
						GetGrid().Move(this, moveSouth);
					}
					else{
						GetGrid().Move(this, moveNorth);
					}
				}
				else if(Math.Abs(difference.x) >= Math.Abs(difference.y) ){
					if(difference.x > 0){
						GetGrid().Move(this, moveEast);
					}
					else{
						GetGrid().Move(this, moveWest);
					}

				}
			}
			else if(fight == 0){
				if(Math.Abs(difference.x) <= Math.Abs(difference.y) ){
					if(difference.y > 0){
						GetGrid().Move(this, moveNorth);
					}
					else{
						GetGrid().Move(this, moveSouth);
					}
				}
				else if(Math.Abs(difference.x) > Math.Abs(difference.y) ){
					if(difference.x > 0){
						GetGrid().Move(this, moveWest);
					}
					else{
						GetGrid().Move(this, moveEast);
					}
				}				
			}			
		}
	}
	
}