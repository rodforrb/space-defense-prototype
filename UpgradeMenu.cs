using Godot;
using System;

public class UpgradeMenu : WindowDialog
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	private Ship1 currentShip;
	private Label curr;

	private Label mhp;
	private Label atk;
	private Label range;
	private int ATKcost = 1;
	private int HPcost = 1;
	private int RANGEcost = 3;
	private TileMap grid;
	private bool visible = false;
	public override void _Ready()
	{
		//All initial nodes obtained, to be used later
		curr = (Label) GetNode("Currency");
		mhp = (Label) GetNode("currentHEALTH");
		atk = (Label) GetNode("currentATK");
		range = (Label) GetNode("currentRANGE");
		grid = (TileMap) GetNode("../Grid");
	}

	//called fromt he grid, shows the menu and updates all information
	public void showMenu(Ship1 ship)
	{
		Show();
		currentShip = ship;
		mhp.Text = "Current HP: " + currentShip.maxHP;
		atk.Text = "Current HP: " + currentShip.firepower;
		range.Text = "Current HP: " + currentShip.maxRange;
		curr.Text = "Currency: " + Loot.Loot.getValue();
		
	}

	//runs on the attack increse button pressed
	private void _on_ATK_pressed()
	{
		if (Loot.Loot.getValue() >= ATKcost)
		{
			currentShip.firepower +=1;
			atk.Text = "Current Attack: " + currentShip.firepower;
			Loot.Loot.spendCurrency(ATKcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested +=ATKcost;
		}
	}

	//runs when the attack decrease button is pressed
	private void _on_ATKM_pressed()
	{
		if (currentShip.firepower > 5)
		{
			currentShip.firepower -=1;
			atk.Text = "Current Attack: " + currentShip.firepower;
			Loot.Loot.giveCurrency(ATKcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested -=ATKcost;
		}
	}

	//runs when the range increase button is pressed
	private void _on_RANGE_pressed()
	{
		if (Loot.Loot.getValue() >= RANGEcost)
		{
			currentShip.maxRange +=1;
			currentShip.range = currentShip.maxRange;
			range.Text = "Current Range: " + currentShip.maxRange;
			Loot.Loot.spendCurrency(RANGEcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested += RANGEcost;
		}
	}

	//runs when the range decrease button is pressed
	private void _on_RANGEM_pressed()
	{
		if (currentShip.maxRange > 3)
		{
			currentShip.maxRange -=1;
			currentShip.range = currentShip.maxRange;
			range.Text = "Current Range: " + currentShip.maxRange;
			Loot.Loot.giveCurrency(RANGEcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested -=RANGEcost;
		}
	}

	//runs when the health increase button is pressed
	private void _on_HEALTH_pressed()
	{
		if (Loot.Loot.getValue() >= HPcost)
		{
			currentShip.maxHP +=10;
			currentShip.HP = currentShip.maxHP;
			mhp.Text = "Current HP: " + currentShip.maxHP;
			Loot.Loot.spendCurrency(HPcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested += HPcost;
		}
	}

	//runs when the health decrease button is pressed
	private void _on_HEALTHM_pressed()
	{
		if (currentShip.maxHP >= 20)
		{
			currentShip.maxHP -=10;
			currentShip.HP = currentShip.maxHP;
			mhp.Text = "Current HP: " + currentShip.maxHP;
			Loot.Loot.giveCurrency(HPcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested -= HPcost;
		}
	}

	//runs when the menu is made visible or invisible
	private void _on_UpgradeMenu_visibility_changed()
	{
		//stops input from reaching the grid when visible
		//allows input when closed
		grid.SetProcessInput(visible);
		visible = !visible;
	}

	// private void _on_UpgradeMenu_hide()
	// {
	//     grid.SetProcessInput(true);
	// }

	// private void _on_UpgradeMenu_about_to_show()
	// {
	//     grid.SetProcessInput(false);
	// }

	


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
