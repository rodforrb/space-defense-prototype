using Godot;
using System;

public class UpgradeMenu : WindowDialog
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	private Button atkP;
	private Button atkM;
	private Button rangeP;
	private Button rangeM;
	private Button healthP;
	private Button healthM;

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
		atkP = (Button) GetNode("ATK+");
		atkM = (Button) GetNode("ATK-");
		rangeP = (Button) GetNode("RANGE+");
		rangeM = (Button) GetNode("RANGE-");
		healthP = (Button) GetNode("HEALTH+");
		healthM = (Button) GetNode("HEALTH-");
	}

	//called from the grid, shows the menu and updates all information
	public void showMenu(Ship1 ship)
	{
		Show();
		currentShip = ship;
		mhp.Text = "Max HP: " + currentShip.maxHP;
		atk.Text = "Attack Damage: " + currentShip.firepower;
		range.Text = "Movement Range: " + currentShip.maxRange;
		curr.Text = "Currency: " + Loot.Loot.getValue();
		checkButtons();
	}

	// private void checkButtons(){
	// 	int currentCurr = Loot.Loot.getValue();
	// 	if (currentCurr < ATKcost){
	// 		atkP.Visible = false;
	// 	}else{
	// 		atkP.Visible = true;
	// 	}

	// 	if (currentCurr < RANGEcost){
	// 		rangeP.Visible = false;
	// 	}else{
	// 		rangeP.Visible = true;
	// 	}

	// 	if (currentCurr < HPcost){
	// 		healthP.Visible = false;
	// 	}else{
	// 		healthP.Visible = true;
	// 	}

	// 	if (currentShip.firepower <= 5){
	// 		atkM.Visible = false;
	// 	}else{
	// 		atkM.Visible = true;
	// 	}

	// 	if (currentShip.maxRange <= 3){
	// 		rangeM.Visible = false;
	// 	}else{
	// 		rangeM.Visible = true;
	// 	}

	// 	if (currentShip.maxHP <= 10){
	// 		healthM.Visible = false;
	// 	}else{
	// 		healthM.Visible = true;
	// 	}
	// }

	private void checkButtons(){
		int currentCurr = Loot.Loot.getValue();
		if (currentCurr < ATKcost){
			disable(atkP);
		}else{
			enable(atkP);
		}

		if (currentCurr < RANGEcost){
			disable(rangeP);
		}else{
			enable(rangeP);
		}

		if (currentCurr < HPcost){
			disable(healthP);
		}else{
			enable(healthP);
		}

		if (currentShip.firepower <= 5){
			disable(atkM);
		}else{
			enable(atkM);
		}

		if (currentShip.maxRange <= 3){
			disable(rangeM);
		}else{
			enable(rangeM);
		}

		if (currentShip.maxHP <= 10){
			disable(healthM);
		}else{
			enable(healthM);
		}
	}


	/* disables a button (greys it out)
	 * @param button, a GD Button
	*/
	private void disable(Button button)
	{
		// turn off the button
		button.Disabled = true;
		// turn it grey
		button.Modulate = new Color(.9f, .6f, .6f, 1);
	}

	/* enables a disabled button
	 * @param button, a GD Button
	*/
	private void enable(Button button)
	{
		// turn off the button
		button.Disabled = false;
		// turn it grey
		button.Modulate = new Color(1, 1, 1, 1);
	}

	//runs on the attack increse button pressed
	private void _on_ATK_pressed()
	{
		if (Loot.Loot.getValue() >= ATKcost)
		{
			currentShip.firepower +=1;
			atk.Text = "Attack Damage: " + currentShip.firepower;
			Loot.Loot.spendCurrency(ATKcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested +=ATKcost;
			checkButtons();
			
		}
	}

	//runs when the attack decrease button is pressed
	private void _on_ATKM_pressed()
	{
		if (currentShip.firepower > 5)
		{
			currentShip.firepower -=1;
			atk.Text = "Attack Damage: " + currentShip.firepower;
			Loot.Loot.giveCurrency(ATKcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested -=ATKcost;
			checkButtons();
		}
	}

	//runs when the range increase button is pressed
	private void _on_RANGE_pressed()
	{
		if (Loot.Loot.getValue() >= RANGEcost)
		{
			currentShip.maxRange +=1;
			currentShip.range = currentShip.maxRange;
			range.Text = "Movement Range: " + currentShip.maxRange;
			Loot.Loot.spendCurrency(RANGEcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested += RANGEcost;
			checkButtons();
		}
	}

	//runs when the range decrease button is pressed
	private void _on_RANGEM_pressed()
	{
		if (currentShip.maxRange > 3)
		{
			currentShip.maxRange -=1;
			currentShip.range = currentShip.maxRange;
			range.Text = "Movement Range: " + currentShip.maxRange;
			Loot.Loot.giveCurrency(RANGEcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested -=RANGEcost;
			checkButtons();
		}
	}

	//runs when the health increase button is pressed
	private void _on_HEALTH_pressed()
	{
		if (Loot.Loot.getValue() >= HPcost)
		{
			currentShip.maxHP +=10;
			currentShip.HP = currentShip.maxHP;
			mhp.Text = "Max HP: " + currentShip.maxHP;
			Loot.Loot.spendCurrency(HPcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested += HPcost;
			checkButtons();
		}
	}

	//runs when the health decrease button is pressed
	private void _on_HEALTHM_pressed()
	{
		if (currentShip.maxHP >= 20)
		{
			currentShip.maxHP -=10;
			currentShip.HP = currentShip.maxHP;
			mhp.Text = "Max HP: " + currentShip.maxHP;
			Loot.Loot.giveCurrency(HPcost);
			curr.Text = "Currency: " + Loot.Loot.getValue();
			currentShip.CurrInvested -= HPcost;
			checkButtons();
		}
	}

	//runs when the menu is made visible or invisible
	private void _on_UpgradeMenu_visibility_changed()
	{
		//stops input from reaching the grid when visible
		//allows input when closed
		grid.SetProcessInput(visible);
		visible = !visible;
		
		// grey out the background when input is disabled
		(GetNode("../Grey") as Panel).Visible = visible;
	}

	


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
