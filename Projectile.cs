using Godot;
using System;

namespace shipClass
{
    public class Projectile : Node2D
    {
        public string name = "";//the name of the projectile
        public int firepower {get; set;} = 0;//the firepower of the projectile
        public int penetration = 0;//the penetration multiplier of the projectile
        public int accuracy = 0;//the accuracy multiplier of the projectile
        public int range = 0;//the range of the weapon in tiles
        public int APcost = 0;//the action point cost
        public string type = "";//the type of weapon it is, some types of weapons will be more effective against certain foes
		
		//constructor with parameters
        public Projectile(string nam, int fp, int pen, int acc, int ran, int ap, string typ)
        {
            name = nam;
            firepower = fp;
            penetration = pen;
            accuracy = acc;
            range = ran;
            APcost = ap;
            type = typ;
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
            return APcost;
        }
		public string getName()
        {
            return name;
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

//I tried to define a class of weapons but it will not let me reference them at all. I hope to properly define the weapons as described here
    public class Weapons : Node2D
    {
        public Projectile Gun = new Projectile("Gun", 1, 2, 2, 8, 1, "normal");
		public Projectile Missile = new Projectile("Missile", 2, 3, 2, 10, 2, "solid");
        public Projectile Laser = new Projectile("Laser", 2, 2, 3, 10, 2, "shiny");
        public Projectile Bomb = new Projectile("Bomb", 3, 3, 1, 5, 3, "solid");
		public Projectile None = new Projectile("None", 0, 0, 0, 0, 0, "nothing");

        public Projectile getGun()
        {
            return Gun;
        }
        public Projectile getMissile()
        {
            return Missile;
        }
        public Projectile getLaser()
        {
            return Laser;
        }
        public Projectile getNone()
        {
            return None;
        }
		// Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {

        }
    }

    
    
    
}