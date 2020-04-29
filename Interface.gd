extends Panel

# called to update the interface
# @param selected, the selected user ship, can be passed an alternate ship
func update(selected = get_node("../Grid").selectedShip):
	# collect everything to be updated
	var sprite = get_node("PanelContainer/PanelSprite")

	var hp_bar = get_node("HPLabel/HPBar")
	var hp = get_node("HPLabel/HP")
	var hp0 = get_node("HPLabel/HPMax")
	var name = get_node("Stats/Name")

	var range_bar = get_node("RangeLabel/RangeBar")
	var rng = get_node("RangeLabel/Range")
	var range0 = get_node("RangeLabel/MaxRange")

	var ap_bar = get_node("APLabel/APBar")
	var ap = get_node("APLabel/AP")
	var ap0 = get_node("APLabel/APMax")

	var alignment = get_node("Stats/L1")
	var atkRange = get_node("Stats/L2")
	var damage = get_node("Stats/L3")
	var armour = get_node("Stats/L4")
	var penetration = get_node("Stats/L5")

	var upgrade_button = get_node("OpenUpgrade")

	# no ship selected; clear interface
	if selected == null:
		sprite.texture = null

		upgrade_button.disabled = true

		name.text = ""
		hp_bar.value = 0
		hp.text = "  0"
		hp0.text = "0"

		range_bar.value = 0
		rng.text = "  0"
		range0.text = "0"

		ap_bar.value = 0
		ap.text = "  0"
		ap0.text = "0"

		alignment.text = ""
		atkRange.text = ""
		damage.text = ""
		armour.text = ""
		penetration.text = ""

	# draw the selected ship
	else:
		sprite.texture = selected.get_node("Sprite").texture
		upgrade_button.disabled = false

		name.text = selected.name
		# label if the ship is selected by click (vs by hover)
		if selected == get_node("../Grid").selectedShip:
			name.text += " (selected)"

		hp_bar.value = (100*selected.HP)/selected.maxHP
		if selected.HP < 10: hp.text = "  "
		else: hp.text = ""
		hp.text += str(selected.HP)
		hp0.text = str(selected.maxHP)
		
		range_bar.value = (100*selected.range)/selected.maxRange
		if selected.range < 10: rng.text = "  "
		else: rng.text = ""
		rng.text += str(selected.range)
		range0.text = str(selected.maxRange)

		ap_bar.value = (100*selected.AP)/selected.maxAP
		if selected.AP < 10: ap.text = "  "
		else: ap.text = ""
		ap.text += str(selected.AP)
		ap0.text = str(selected.maxAP)

		if selected.team == 0:
			alignment.text = "Friendly"
		else:
			alignment.text = "Hostile"

		atkRange.text = str(selected.atkRange)

		damage.text = str(selected.firepower)

		armour.text = str(selected.armour)

		penetration.text = str(selected.penetration)


