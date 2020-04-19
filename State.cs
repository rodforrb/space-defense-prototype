using System;
using Godot;
/* Static variables and functions needed globally.
 * Godot classes must be instantiated to be used.
 * New members should still be added sparingly.
*/
public class State : Node
{
	public static int maxLevel {get; set;} = 1;
  public static int currentLevel {get; set;} = 1;
  public static void nextLevel()
  {
	// increment state variables to unlock next level
	State.maxLevel = Math.Max(State.maxLevel, State.currentLevel + 1);
	State.currentLevel += 1;
  }
}
