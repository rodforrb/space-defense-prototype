using Godot;
using System;

public class Ship1 : Node2D
{
    private int maxHP = 50;//maximum hp
    public int HP { get; set;} = 50;//current hp

    //0 for firendly, 1 for enemy;
    //Do we need a name or ID?
    public int shipType = 0;

   

    public int firepower { get; set; } = 5;//the ships firepower multiplier
    public int penetration { get; set; } = 5;//the ships ability to ignore armour
    public int armour { get; set; } = 5;//the ships resistance to damage
    public int accuracy { get; set; } = 5;//odds of hitting an opponent
    public int evasion { get; set; } = 5;//odds of dodging an attack
    public int AP { get; set; } = 4;//The current action points of a ship, how many times it may use it's weapons or skill in a turn

    private int maxAP = 4;//the maximum action points of a ship, it will reset to this value at the start of every turn
    private int range = 5;//the range it can move
	
	public Projectile weapon1 { get; set; } = new Projectile(ProjectileType.Gun, 1, 2, 2, 8, 1, "normal");//the first weapon that the ship has
	//public shipClass.Projectile weapon0 = shipClass.Weapons.getGun();//the first weapon that the ship has
	public Projectile weapon2 { get; set;} = new Projectile(ProjectileType.Missile, 2, 3, 2, 10, 2, "solid");//the second weapon a ship has
	public Projectile weapon3 { get; set;} = new Projectile(ProjectileType.Laser, 2, 2, 3, 10, 2, "shiny");//the third weapon a ship has
                                                                                                     //int yes = shipClass.Projectile.firepower;

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
    }

    //constructor without parameters
    public Ship1()
    {

    }

    public Projectile getWeapon1()
	{
		return weapon1;
	}

    private int attackRange = 2;

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
    public void take_damage(int hit)
    {
        HP = Math.Max(0, HP - hit);
        GD.Print(HP);
        
        
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
	public void reset_AP()
	{
		//when it is the ship's turn again it will regain all of it's action points
		AP = maxAP;
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

}
