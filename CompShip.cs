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

	private int targX = -1;//used for avoiding move deadlock
	private int targY = -1;
	private Vector2[] oldMoves;
	private int moveStep = 0;
	
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
	//KNOWN ISSUE:
	//SOMETIMES SHIPS GET STUCK PINGING THEMSELVES UP AND DOWN OVER AND OVER
	//WE'LL FILE THIS UNDER "KNOWN BUGS" FOR NOW
	//FOR THE SAKE OF GETTING SOME LEVEL OF AI LOGIC PUSHED TO MASTER
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
		//may need to switch targetting?
		//or like modify BFS so it gets the path dist to ALL ships and uses that?
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
		Vector2 shipCell = GetGrid().WorldToMap(this.GetPosition());
		Vector2 targetCell = GetGrid().WorldToMap(target.Position); 
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
		int v = GetGrid().gridSize;
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
				movePath[i] = moveToAdd;
			}
		}
		else{
			//Currently no logic for unreachable target
			fight = 0;
		}	
		Random r = new Random();


		for (int i = 0; i < maxRange; i++){

			Vector2 difference = (targetCell - shipCell);
			if(fight==1){
				if((Math.Abs(difference.x)+Math.Abs(difference.y)) <= range){
					GetGrid().Attack(this, target, new Projectile(ProjectileType.Gun));
				}
				else{
					GetGrid().Move(this, movePath[i]);
					this.moveStep = i;
					}
			}
			else if(fight == 0){
				
				int randDirection = r.Next(0, 4); //0 = north, 1 east, 2 south, 3 west						
				switch (randDirection){
					case 0:
						GetGrid().Move(this, moveNorth);	
						break;			
					case 1:
						GetGrid().Move(this, moveEast);
						break;
					case 2:
						GetGrid().Move(this, moveSouth);
						break;			
					case 3:
						GetGrid().Move(this, moveWest);
						break;
				}								
			}			
		}
		this.oldMoves = new Vector2[movePath.Length -  this.moveStep];
		Array.Copy(movePath, this.moveStep, this.oldMoves,  this.oldMoves.GetLowerBound(0), movePath.Length -  this.moveStep );
		this.targX = (int)targetCell[0];
		this.targY = (int)targetCell[1];	
		/*
		foreach(int number in GetGrid().obst){
			GD.Print(number);
		}*/
		

		/* old algo		
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
		}*/
	}
	
}
