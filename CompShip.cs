using Godot;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;


public class CompShip : Ship1
{
	/*
	name or id?
	*/
	private int maxRange = 3;
	new public int range {get; set;} = 3;
	private int maxAP = 5;
	new public int AP {get; set;} = 5;
	
	new public Team team = Team.Computer;
	new public int penetration { get; set; } = 5;//the ships ability to ignore armour
	new public int armour { get; set; } = 5;//the ships resistance to damage
	new public int accuracy { get; set; } = 5;//odds of hitting an opponent
	new public int evasion { get; set; } = 5;//odds of dodging an attack
	
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
			else if (lenSquared ==  distance && ship.HP < target.HP)
			{
				target = ship;
				distance = lenSquared;
			}			
		}


		int fight;
		//todo: add logic comparing stats
		//ie, pass this function an array of ships, try to destroy weakest, or run from strongest
		//have some var like fight, 0 = run, 1 = fight, 2 = freeze, 3 = something like "support" idk
		if(this.HP <= target.firepower && this.firepower * this.AP < target.HP){
			fight = 0;
		}
		else{
			fight = 1;
		}
		//generating path
		Vector2 shipCell = GetGrid().WorldToMap(this.GetPosition());
		Vector2 targetCell = GetGrid().WorldToMap(target.Position); 
		Vector2 moveNorth = new Vector2(shipCell.x, shipCell.y - 1);
		Vector2 moveEast = new Vector2(shipCell.x + 1, shipCell.y);
		Vector2 moveSouth = new Vector2(shipCell.x, shipCell.y + 1);
		Vector2 moveWest = new Vector2(shipCell.x - 1, shipCell.y);
		Vector2[] path = null;

		Queue queue = new Queue();
		//assuming forever square grids
		//BFS
		int v = GetGrid().gridSize;
		bool[,] visited = new bool[v, v]; 
		int[,] dist = new int[v,v]; 
		int[,] pred = new int[v,v];
		List<int[]> movePath = new List<int[]>();
		bool reachDest;
		for (int i = 0; i < v; i++){
			for(int j=0; j < v; j++){
				visited[i, j] = false;
				dist[i, j] = v*v;
			}
		}
		int[] posHolder = new int[2];
		posHolder[0] = (int)shipCell[0];
		posHolder[1] = (int)shipCell[1];
		queue.Enqueue(posHolder.Clone());
		/*
		posHolder[0] = 55;
		posHolder[1] = 66;	
		queue.Enqueue(posHolder.Clone());

        GD.Print("\nQueue contains:");
        foreach( int[] number in queue )
        {
            GD.Print(number[0],", " number[1]);
        }	
		*/
		//GD.Print((int)shipCell[0], (int)shipCell[1]);
		visited[(int)shipCell[0], (int)shipCell[1]] = true;
		dist[(int)shipCell[0], (int)shipCell[1]] = 0;
		pred[(int)shipCell[0], (int)shipCell[1]] = -1;
		
		while(queue.Count != 0){
			int[] activeStep = (int[]) queue.Dequeue();
			int addX = 0;
			int addY = 0;
			for(int i = 0; i<4; i++){
				//adjs
				if(i == 0){
					addX = 0;
					addY = -1;
				}
				if(i== 1){
					addX = 1;
					addY = 0;					
				}
				if(i== 2){
					addX = 0;
					addY = 1;					
				}	
				if(i== 3){
					addX = -1;
					addY = 0;					
				}								
				if(((0 <= (activeStep[0] + addX) && (activeStep[0] + addX) < v) && (0 <= (activeStep[1] + addY) && (activeStep[1] + addY)< v) ) && (visited[activeStep[0] + addX, activeStep[1]+addY] == false)){
					//obst check todo
					bool freeTile = true;
					foreach (int obs in GetGrid().obst){
						if(GetGrid().GetCellv(new Vector2((activeStep[0] + addX), (activeStep[1] + addY))) == obs){
								freeTile = false;
						}
					}
					if(freeTile){

						visited[activeStep[0] + addX, activeStep[1]+addY] = true;
						dist[activeStep[0] + addX, activeStep[1]+addY] = dist[activeStep[0], activeStep[1]] + 1;
						posHolder[0] = (int)activeStep[0] + addX;
						posHolder[1] = (int)activeStep[1] + addY;
						pred[activeStep[0] + addX, activeStep[1]+addY] = i;
						queue.Enqueue(posHolder.Clone());	
						if (posHolder[0] == (int)targetCell[0] && posHolder[1] == (int)targetCell[1] ){
							//hit target
							queue.Clear();
							reachDest = true;
						}
					}
				}		
			}
		}
		//TODO
		//ACTUALLY MAKE IT MOVE!
		//using the list
		//https://www.geeksforgeeks.org/shortest-path-unweighted-graph/
	
		/*
		foreach(int number in GetGrid().obst){
			GD.Print(number);
		}*/
		


		for (int i = 0; i < maxRange; i++){
		



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
