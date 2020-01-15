using Godot;
using System;

public class Ship1 : Node2D
{
    public const int maxHP = 50;//maximum hp
    public int HP { get; set;} = 50;//current hp

    public int firepower { get; set; } = 5;//the ships firepower multiplier
    public int penetration { get; set; } = 5;//the ships ability to ignore armour
    public int armour { get; set; } = 5;//the ships resistance to damage
    public int accuracy { get; set; } = 5;//odds of hitting an opponent
    public int evasion { get; set; } = 5;//odds of dodging an attack
	
	
    private int range = 5;

    private int attackRange = 1;

    public int getAttackRange()
    {
        return attackRange;
    }

    public int GetRange()
    {
        return range;
    }

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
