using Godot;
using System;
using System.Threading.Tasks;

public class CompShip : Ship1
{
	/*
	name or id?
	*/
	
	new public Team team = Team.Computer;
	public override int maxHP {get;} = 10;//maximum hp
    public override int HP { get; set;} = 10;//current hp   

    public override int firepower { get; set; } = 5;//the ships firepower multiplier
    public override int penetration { get; set; } = 5;//the ships ability to ignore armour
    public override int armour { get; set; } = 5;//the ships resistance to damage
    public override int accuracy { get; set; } = 5;//odds of hitting an opponent
    public override int evasion { get; set; } = 5;//odds of dodging an attack
    public override int AP { get; set; } = 5;//The current action points of a ship, how many times it may use it's weapons or skill in a turn
    public override int maxAP {get;}= 5;//the maximum action points of a ship, it will reset to this value at the start of every turn
    public int maxRange = 3;//the range it can move
    public override int range {get; set;} = 3;
	
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
