using Godot;
using System;

namespace shipClass
{
    public class Projectile : Node2D
    {
        public string name { get; set; } = "";//the name of the projectile
        public int firepower { get; set; } = 0;//the firepower of the projectile
        public int penetration { get; set; } = 0;//the penetration multiplier of the projectile
        public int accuracy { get; set; } = 0;//the accuracy multiplier of the projectile
        public int range { get; set; } = 0;//the range of the weapon in tiles
        public int APcost { get; set; } = 0;//the action point cost
        public string type { get; set; } = "";//the type of weapon it is
		
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

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {

        }


    }

/*    public class Weapons : Node2D
    {
        public Projectile Gun = new Projectile("Gun", 1, 2, 2, 8, 1, "normal");
		public Projectile Missile = new Projectile("Missile", 2, 3, 2, 10, 2, "solid");
        public Projectile Laser = new Projectile("Laser", 2, 2, 3, 10, 2, "shiny");
        public Projectile Bomb = new Projectile("Bomb", 3, 3, 1, 5, 3, "solid");

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
		// Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {

        }
    }
*/
    
    
    
}