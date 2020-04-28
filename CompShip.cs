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
	
	[Export]
	new public int maxRange = 3;
	[Export]
	new public int range {get; set;} = 3;
	
	[Export]
	public int maxAP = 2;
	[Export]
	new public int AP {get; set;} = 2;
	
	[Export]
	new public Team team {get;} = Team.Computer;
	[Export]
	new public Type type {get; set; } = Type.Medium;
	
	[Export]
	public int firepower = 5;//the ships firepower multiplier
	
	[Export]
	new public int penetration = 5;
	[Export]
	new public int armour = 5;
	
	public int atkRange {
		get{
			if (type == Type.Lite)
				return 4;
			else
				return 3;
		}
		set{
			this.atkRange = value;
		}
	}//the range the ship can attack from
	
	new public string name {
		get{
			if (type == Type.Destroyer) 
				return "Destroyer"; 
			else if (type == Type.Heavy) 
				return "Heavy"; 
			else if (type == Type.Lite) 
				return "Scout"; 
			else 
				return "Medium";
		}
		set{
			this.name = value;
		}
	}
	
	/*public int penetration { 
		get{
			if (type == Type.Destroyer) 
				return 8; 
			else if (type == Type.Lite) 
				return 3; 
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
	*/
	public void take_hit(int damage, int pen)
	{
		//HP = Math.Max(0, HP-damage);
		HP = Math.Max(0, HP - ( Math.Max(1, (damage) / ((Math.Max(1, ((armour) - pen) ) ))) ));
		var bar = (TextureProgress)GetNode("HPbar");

		bar.Value = (int)((double) HP / maxHP * 100.0);
	}
	
	private int targX = -1;//used for avoiding move deadlock
	private int targY = -1;
	private Vector2[] oldMoves;
	private int moveStep = 0;
	
	
	/* Used to get instantiated Grid object
	 * Unfortunately GetNode cannot be used by a static class.
	 * @return Grid
	*/
	public Node GetGrid ()
	{
		return (GetNode("/root/Game/Grid"));
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

	//Exhibits strange diagonal movement
	//The calling loop in grid and this code
	//interact in an odd manner
	//
	public void PlayTurn()
	{
		Godot.Collections.Array PlayerShips = GetGrid().Get("playerShips") as Godot.Collections.Array;
		// nothing to do but game might not have ended yet because of async
		try
		{
		if (PlayerShips.Count == 0) return;
		} catch (System.Exception e) {
			return;
		}

		//GD.Print("Comp Move Called");
		
		Ship1 target = (Ship1)PlayerShips[0];
		float distance = 10000000;
		//may need to switch targetting?
		//or like modify BFS so it gets the path dist to ALL ships and uses that?
		foreach (Ship1 ship in PlayerShips)
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
		//This is broken on current build b/c of oneshot potential
		/*
		if(this.HP <= target.firepower && this.firepower * this.AP < target.HP){
			fight = 0;
		}
		else{
			fight = 1;
		}
		*/
		fight = 1;
		//generating path
		Vector2 shipCell = (Vector2)GetGrid().Call("world_to_map",this.GetPosition());
		Vector2 targetCell = (Vector2)GetGrid().Call("world_to_map", target.Position); 
		Vector2 moveNorth = new Vector2(shipCell.x, shipCell.y - 1);
		Vector2 moveEast = new Vector2(shipCell.x + 1, shipCell.y);
		Vector2 moveSouth = new Vector2(shipCell.x, shipCell.y + 1);
		Vector2 moveWest = new Vector2(shipCell.x - 1, shipCell.y);
		
		//if the target hasnt moved, use old path
		//attempt to avoid up-down deadlock
		//super broke it
		/*
		if (targetCell[0] == this.targX && targetCell[1] ==this.targY ){
			for (int i = 0; i < maxRange; i++){

				Vector2 difference = (targetCell - shipCell);
				//if(fight==1)
				{
					if((Math.Abs(difference.x)+Math.Abs(difference.y)) <= range){
						GetGrid().Attack(this, target, new Projectile(ProjectileType.Gun));
					}
					else{
						GetGrid().Move(this, this.oldMoves[i]);
						this.moveStep = i;
						}
				}
				
			}
			Array.Copy(this.oldMoves, this.moveStep, this.oldMoves, this.oldMoves.GetLowerBound(0), this.oldMoves.Length -  this.moveStep );
			return;
		}
		*/
		Queue queue = new Queue();
		//assuming forever square grids
		//BFS
		int v = 32;
		bool[,] visited = new bool[v, v]; 
		int[,] dist = new int[v,v]; 
		int[,] pred = new int[v,v];
		
		bool reachDest = false;
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
					bool freeTile = true;
					Godot.Collections.Array tiles = GetGrid().Get("obstacle_tiles") as Godot.Collections.Array;
					foreach (int obs in tiles){
						if((int)GetGrid().Call("get_cellv", new Vector2((activeStep[0] + addX), (activeStep[1] + addY))) == obs){
								freeTile = false;
						}
					}
					//Avoid "passing through" other comp ships
					Godot.Collections.Array compships = GetGrid().Get("enemyShips") as Godot.Collections.Array;
					foreach (Ship1 obs in compships){
						Vector2 tempV = new Vector2((activeStep[0] + addX), (activeStep[1] + addY));
						if(tempV == (Vector2)GetGrid().Call("world_to_map", obs.Position)){
								freeTile = false;
								//GD.Print("COMP SHIP COLLISON");
						}
					}					

					if(freeTile){
						visited[activeStep[0] + addX, activeStep[1]+addY] = true;
						dist[activeStep[0] + addX, activeStep[1]+addY] = dist[activeStep[0], activeStep[1]] + 1;
						
						posHolder[0] = (int)activeStep[0] + addX;
						posHolder[1] = (int)activeStep[1] + addY;
						//could switch the active steps below to posholder
						pred[activeStep[0] + addX, activeStep[1]+addY] = i;
						queue.Enqueue(posHolder.Clone());	
						if (posHolder[0] == (int)targetCell[0] && posHolder[1] == (int)targetCell[1] ){
							//hit target
							queue.Clear();
							reachDest = true;
							//GD.Print("BFS Algo Hit");
							//GD.Print("Distance:", dist[activeStep[0] + addX, activeStep[1]+addY] );
						}
					}
				}		
			}
		}

		//For reference, below is a BFS algo
		//https://www.geeksforgeeks.org/shortest-path-unweighted-graph/
		//List<int[]> movePath = new List<int[]>();
		int pathDist = dist[(int)targetCell[0], (int)targetCell[1]];
		Vector2[] movePath = new Vector2[pathDist];	
			
		if (reachDest){
			int currentX = (int)targetCell[0];
			int currentY = (int)targetCell[1];

			for(int i = pathDist-1; i>= 0; i--){
				int crawlPred = pred[currentX, currentY];
				//GD.Print("Crawl: ", crawlPred);
				Vector2 moveToAdd = new Vector2();

				moveNorth.x =  currentX;
				moveNorth.y = currentY;

				moveEast.x = currentX;
				moveEast.y =  currentY;

				moveSouth.x =  currentX;
				moveSouth.y = currentY;

				moveWest.x =  currentX;
				moveWest.y = currentY;


				if(crawlPred == 0){
					moveToAdd = moveNorth;
					currentY += 1;			
				}
				if(crawlPred == 1){
					moveToAdd = moveEast;	
					currentX += -1;										
				}
				if(crawlPred == 2){
					moveToAdd = moveSouth;
					currentY += -1;											
				}	
				if(crawlPred == 3){
					moveToAdd = moveWest;
					currentX += 1;				
				}
				//GD.Print("\n Step ", i, "(", currentX, ", ", currentY, "),  Move added: ", moveToAdd);
				movePath[i] = moveToAdd;

			}
		}
		else{
			//No logic for unreachable target
			fight = 0;
		}
			
		Random r = new Random();

		//movement control
		Vector2 difference = (targetCell - shipCell);
		if(fight==1){
			//needs to check for valid attack
			if(this.AP != 0 && (Math.Abs(difference.x)+Math.Abs(difference.y)) <= maxRange){
				// GetGrid().Attack(this, target, new Projectile(ProjectileType.Gun));
				GetGrid().Call("attack", this, target);
			}
			if(movePath.Length > 0){
				//Might need to add a last move check to stop ships from ending up inside each other
				//Godot.Collections.Array ship = GetGrid().Get("enemy_ships") as Godot.Collections.Array;
				GetGrid().Call("move", this, movePath[0]);
				//GD.Print(movePath[0]);
				//GD.Print("\nShip position: (", shipCell.x, ", ", shipCell.y, ") Move0: ", movePath[0], "Target: (", targetCell.x, ", ", targetCell.y, ")" );
				//this.moveStep = i;
			}
			//"just in case" rando movement
			else{
				//set for random movement
				moveNorth.x =  shipCell.x;
				moveNorth.y = shipCell.y - 1;

				moveEast.x =  shipCell.x + 1;
				moveEast.y =  shipCell.y;

				moveSouth.x =  shipCell.x;
				moveSouth.y = shipCell.y + 1;

				moveWest.x =  shipCell.x - 1;
				moveWest.y = shipCell.y;
			
				int randDirection = r.Next(0, 4); //0 = north, 1 east, 2 south, 3 west						
				switch (randDirection){
					case 0:
						GetGrid().Call("move", this, moveNorth);	
						break;			
					case 1:
						GetGrid().Call("move", this, moveEast);
						break;
					case 2:
						GetGrid().Call("move", this, moveSouth);
						break;			
					case 3:
						GetGrid().Call("move", this, moveWest);
						break;
				}

			}
			
		}
		else if(fight == 0){

			//set for random movement
			moveNorth.x =  shipCell.x;
			moveNorth.y = shipCell.y - 1;

			moveEast.x =  shipCell.x + 1;
			moveEast.y =  shipCell.y;

			moveSouth.x =  shipCell.x;
			moveSouth.y = shipCell.y + 1;

			moveWest.x =  shipCell.x - 1;
			moveWest.y = shipCell.y;
		
			int randDirection = r.Next(0, 4); //0 = north, 1 east, 2 south, 3 west						
			switch (randDirection){
				case 0:
					GetGrid().Call("move", this, moveNorth);	
					break;			
				case 1:
					GetGrid().Call("move", this, moveEast);
					break;
				case 2:
					GetGrid().Call("move", this, moveSouth);
					break;			
				case 3:
					GetGrid().Call("move", this, moveWest);
					break;
			}								
		}

		//post-move attacking


		//update ship position
		shipCell = (Vector2)GetGrid().Call("world_to_map",this.GetPosition());			
		Vector2 finalDifference = (targetCell - shipCell);
		if((Math.Abs(finalDifference.x)+Math.Abs(finalDifference.y)) <= maxRange){
			while(target.HP > 0 && this.AP > 0){
				var preAP = this.AP;
				GetGrid().Call("attack", this, target);
				//No AP change indicates invalid attack, avoids infinite loop.
				//GD.Print("Attack Loop");
				if (preAP == this.AP) break;
			}

		}
	//GD.Print("TEST");
	}
	
}
