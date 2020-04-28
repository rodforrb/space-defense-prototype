using Godot;
using System;

public enum Team
{
	Player,
	Computer
}

public enum Type
{
	Medium,
	Lite,
	Heavy,
	Destroyer,
	Sniper,
	Support
}

public class Ship1 : Node2D
{
	
	[Export]
	public int maxHP = 10;//maximum hp
	
	[Export]
	public int HP = 10;//current hp

	[Export]
	public Team team {get;} = Team.Player;
	
	[Export]
	public Type type {get; set; } = Type.Medium;
   
	[Export]
	public int firepower = 5;//the ships firepower multiplier
	[Export]
	public int penetration = 5;
	[Export]
	public int armour = 5;
	
	public string name {
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
				return 7; 
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
	*/
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
	
	[Export]
	public int AP { get; set; } = 2;//The current action points of a ship, how many times it may use it's weapons or skill in a turn
	
	[Export]
	public int maxAP = 2;//the maximum action points of a ship, it will reset to this value at the start of every turn
	
	[Export]
	public int maxRange = 3;//the range it can move
	
	[Export]
	public int range {get; set;} = 3;
	
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
	}

	public Projectile weapon1 { 
		get{
			if (type == Type.Destroyer) {
				Projectile wep = new Projectile(ProjectileType.Gun, 1, 2, 2, 8, 1, "missile");
				return wep;
			}
			else { 
				Projectile wep = new Projectile(ProjectileType.Gun, 1, 2, 2, 8, 1, "laser");
				return wep; 
			}
		} 
		set{
			this.weapon1 = value;
		}
	}//the first weapon that the ship has
	//public shipClass.Projectile weapon0 = shipClass.Weapons.getGun();//the first weapon that the ship has
	public Projectile weapon2 { get; set;} = new Projectile(ProjectileType.Missile, 2, 3, 2, 10, 2, "solid");//the second weapon a ship has
	public Projectile weapon3 { get; set;} = new Projectile(ProjectileType.Laser, 2, 2, 3, 10, 2, "shiny");//the third weapon a ship has
																									 //int yes = shipClass.Projectile.firepower;
//	public (PackedScene) var bullet;
//	public var bullet_conatianer = GetNode("bullet_container");
//	public PackedScene bullet = ResourceLoader.Load("Bullets.tscn") as PackedScene;
	public PackedScene laser = ResourceLoader.Load("Laser.tscn") as PackedScene;
	

	//constructor with parameters
	public Ship1(int Hpp, int fp, int pen, int arm, int acc, int eva, int ran, int ap, Projectile w1, Projectile w2, Projectile w3)
	{
		HP = Hpp;
		maxHP = Hpp;
		firepower = fp;
		penetration = pen;
		armour = arm;
		accuracy = acc;
		evasion = eva;
		range = ran;
		AP = ap;
		maxAP = ap;
		weapon1 = w1;
		weapon2 = w2;
		weapon3 = w3; 
		team = Team.Player;
	}

	//constructor without parameters
	public Ship1()
	{
		team = Team.Player;
	}

	public Projectile getWeapon1()
	{
		return weapon1;
	}

	private int attackRange = 3;

	public int getAttackRange()
	{
		return attackRange;
	}

	public int GetRange()
	{
		return range;
	}
	
	//TODO: add proper getter and setter functions for each of the ships variables
	//TODO: add the ability to select weapons when attacking
	//TODO: make the calculation in take_damage more complex, factoring in the defenders armour/evasion and the attackers penetration/accuracy
	//TODO: the calculation in take_damage should be affected by what weapon the attacker uses. Perhaps this will be calculated elsewhere in Grid.cs

	public int getFirepower()
	{
		return this.firepower;
	}

	//these values serve as base values for weapons and skills
	//we will most likely change every value here later on

	//special setters for HP
	public void take_damage(int fp, int pen, int acc)
	{
		float hits = (float)acc / (float)(acc + evasion);
		int chance = (int) (hits * 100);
		Random random = new Random();
		int result = random.Next(0, 100);
		
		
		
		var hpb = (TextureProgress)GetNode("HPbar");
		
		// we can remove randomness if we want to, I just left it in to test the results
		if (result <= chance)
		{
			HP = Math.Max(0, HP - ( (fp) / (1 + Math.Max(0, ((armour * 2) - pen) ) )) );
		}
		
		//first calculate the actual hp
		//HP = Math.Max(0, HP - ( (fp) / (1 + Math.Max(0, ((armour * 2) - pen) ) )) );
		//then calculate it as a percentage for the HPbar
		double fraction = ((double) HP / maxHP) * 100.0;
		hpb.Value = (int)(fraction);

		GD.Print(HP);
		GD.Print(hpb.Value);
	 
		// ship is removed by Grid if dead
		// the hpbar naturally is removed too since it is a child node 
	}

	public void take_hit(int damage, int pen)
	{
		//HP = Math.Max(0, HP-damage);
		HP = Math.Max(0, HP - ( Math.Max(1, (damage) / ((Math.Max(1, ((armour) - pen) ) ))) ));
		var bar = (TextureProgress)GetNode("HPbar");

		bar.Value = (int)((double) HP / maxHP * 100.0);
	}



	public void heal_damage(int heal)
	{
		HP = Math.Min(maxHP, heal + HP);
	}

	//special setters for AP
	public void spend_AP(int amount)
	{
		//when a ship attacks, it spends ap. If the AP is zero then the ship is unable to attack
		//If a ship tries to use a move that costs more AP than it currently has, it will not attack and therefore won't spend AP
		if ((AP - amount) >= 0){
			AP = AP - amount;
		}
	}
	public void ResetPoints()
	{
		//when it is the ship's turn again it will regain all of it's action points
		AP = maxAP;
		range = maxRange;
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

	/*public void shoot(Vector2 st, Vector2 ed, int ev)
	{
		
		//_bullet.Bulle(WorldToMap(attacker.Position), WorldToMap(defender.Position), f, p, a, defender.evasion);
		var bullet_instance = bullet.Instance() as Area2D;
		AddChild(bullet_instance);
		//bullet_instance.SetPosition(attackNode.Position);
		//bullet_instance.start_at(st, ed, firepower, penetration, accuracy);
		bullet_instance = new Bullets(st, ed, firepower, penetration, accuracy);
		
		bullet_instance.Connect("hit_target", this.GetParent(), "attackhits" );
	}*/

	public Texture GetTexture()
	{
		return GetChild<Sprite>(0).Texture;
	}

}
