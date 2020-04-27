using Godot;
using System;

public enum ProjectileType
{
	None,
	Default,
	Laser,
	Missile,
	Bomb,
	Gun
}

public class Projectile : Node2D
{
	[Export]
	public ProjectileType proj {get; set;} = ProjectileType.Default;//the name of the projectile
	[Export]
	public int firepower {get; set;} = 0;//the firepower of the projectile
	[Export]
	public int penetration {get; set;} = 0;//the penetration multiplier of the projectile
	public int accuracy {get; set;} = 0;//the accuracy multiplier of the projectile
	public int range {get; set;} = 0;//the range of the weapon in tiles
	
	[Export]
	public int cost {get; set;} = 0;//the action point cost
	public string type {get; set;} = "";//the type of weapon it is, some types of weapons will be more effective against certain foes
	

	//constructor with parameters
	public Projectile(ProjectileType p, int fp, int pen, int acc, int ran, int ap, string typ)
	{
		proj = p;
		firepower = fp;
		penetration = pen;
		accuracy = acc;
		range = ran;
		cost = ap;
		type = typ;
	}

	// constructor copy for self-filling constructor below
	public void SetProjectile(ProjectileType p, int fp, int pen, int acc, int ran, int ap, string typ)
	{
		proj = p;
		firepower = fp;
		penetration = pen;
		accuracy = acc;
		range = ran;
		cost = ap;
		type = typ;
	}

	public Projectile(ProjectileType type)
	{
		switch (type)
		{
			case ProjectileType.Gun:
				SetProjectile(ProjectileType.Gun, 1, 2, 2, 8, 1, "normal");
				break;
			case ProjectileType.Missile:
				SetProjectile(ProjectileType.Missile, 2, 3, 2, 10, 2, "solid");
				break;
			case ProjectileType.Laser:
				SetProjectile(ProjectileType.Laser, 2, 2, 3, 10, 2, "shiny");
				break;
			case ProjectileType.Bomb:
				SetProjectile(ProjectileType.Bomb, 3, 3, 1, 5, 3, "solid");
				break;
			default:
				SetProjectile(ProjectileType.None, 0, 0, 0, 0, 0, "nothing");
				break;

		}
	}
	
	//constructor without parameters
	public Projectile()
	{
		
	}
	

	public int getFP()
	{
		return firepower;
	}
	public void setFP(int fp)
	{
		firepower = fp;
	}
	public int getPen()
	{
		return penetration;
	}
	public int getAcc()
	{
		return accuracy;
	}
	public int getRange()
	{
		return range;
	}
	public int getAPCost()
	{
		return cost;
	}
	public ProjectileType getProjectileType()
	{
		return proj;
	}
	public string getType()
	{
		return type;
	}
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}


}
