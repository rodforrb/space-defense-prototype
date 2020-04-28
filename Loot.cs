using Godot;
using System;

namespace Loot
{
    //Static class to hold information about a players current loot value
    public static class Loot
    {
        private static int currency = 0;

        //Retrieves the current loot value
        public static int getValue(){
            return currency;
        }

        //Adds a supplied value to the players current loot
        public static void giveCurrency(int value){
            currency += value;
        }

        //Removes a given value from the current loot
        public static void spendCurrency(int value){
            currency -= value;
        }
    }
}


