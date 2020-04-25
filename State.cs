using System;
using Godot;
using Godot.Collections;
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
  private static Dictionary<string, object> Savestate()
  {
	return new Dictionary<string, object>()
	{
	  {"maxLevel", maxLevel},
	  {"date", DateTime.Now}
	};
  }

  public static bool Save()
  {
	try {
	  File saveFile = new File();
	  saveFile.Open("sav.sd", File.ModeFlags.Write);

	  Dictionary<string, object> saveData = Savestate();

	  saveFile.StoreLine(JSON.Print(saveData));

	  saveFile.Close();
	} catch (System.Exception e) {
	  GD.Print(e.ToString());
	  return false;
	}
	return true;
  }

  public static bool Load()
  {try{
	var saveFile = new File();
	if (!saveFile.FileExists("sav.sd"))
	  return false;

	saveFile.Open("sav.sd", File.ModeFlags.Read);

	while (!saveFile.EofReached())
	{	  
	GD.Print(JSON.Parse(saveFile.GetLine()).Result.GetType());

	  var currentLine = (Dictionary<object, object>)JSON.Parse(saveFile.GetLine()).Result;
	  if (currentLine == null)
		continue;

	  // Now we set the remaining variables.
	  foreach (System.Collections.Generic.KeyValuePair<object, object> entry in currentLine)
	  {
		string key = entry.Key.ToString();
		switch (key)
		{
		case "maxLevel":
		  maxLevel = (int)entry.Value;
		  currentLevel = maxLevel;
		  break;
		}
	  }
	}
	saveFile.Close();
	return true;
  } catch (System.Exception e) {
	GD.Print(e.ToString());
	return false;
  }}
}
