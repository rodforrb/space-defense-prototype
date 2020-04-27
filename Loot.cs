using Godot;
using System;

namespace Loot
{
	public static class Loot
	{
		private static int currency = 0;

		public static int getValue(){
			return currency;
		}

		public static void giveCurrency(int value){
			currency += value;
		}
	}
}


