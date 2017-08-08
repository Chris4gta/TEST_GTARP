var g_menu = API.createMenu("Tanken", "Kraftstoff", 0, 0, 6);
g_menu.ResetKey(menuControl.Back);

g_menu.AddItem(API.createMenuItem("Diesel ", "Hier tankst du Diesel."));
g_menu.AddItem(API.createMenuItem("Benzin", "Hier tankst du Benzin."));
g_menu.AddItem(API.createMenuItem("Strom", "Hier tankst du Strom."));

g_menu.OnItemSelect.connect(function(sender, item, index) {
	API.sendChatMessage("You selected: ~g~" + item.Text);

	API.triggerServerEvent("CAR_IS_REFUELING");
	API.showCursor(false);
	g_menu.Visible = false;
});

API.onServerEventTrigger.connect(function(name, args) {
	if (name == "TRIGGER_REFUEL_MENU") {
		API.showCursor(false);
		g_menu.Visible = true;
	}
});

API.onUpdate.connect(function() {
	API.drawMenu(g_menu);
});

API.onUpdate.connect(function (sender, e) {
	
	player = API.getLocalPlayer();
	inVeh = API.isPlayerInAnyVehicle(player);
	veh = API.getPlayerVehicle(player);

	if(inVeh){
		
	}

});