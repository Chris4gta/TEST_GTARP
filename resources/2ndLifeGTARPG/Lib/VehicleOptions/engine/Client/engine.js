var g_menu = API.createMenu("Fahrzeug", "Subtitle", 0, 0, 6);
var player;
var inVeh;
var EngineStatusOn;
var currentVehicleFuel;
g_menu.AddItem(API.createMenuItem("Motor einschalten " , "Das Fahrzeug wird gestartet"));
g_menu.AddItem(API.createMenuItem("Motor ausschalten " , "Das Fahrzeug wird gestoppt"));


g_menu.OnItemSelect.connect(function(sender, item, index) {
	API.showCursor(false);	
	g_menu.Visible = true;
	var veh = API.getPlayerVehicle(player);
	var inVeh = API.isPlayerInAnyVehicle(player);
	var bEngineStatus = API.getVehicleEngineStatus(veh);
	
	
	API.triggerServerEvent("GET_VEHICLE_FUEL");
	if (inVeh){
		if (index==0 && currentVehicleFuel > 0){
			bEngineStatus = true;
			API.setVehicleEngineStatus(veh, bEngineStatus);
			g_menu.Visible = false;
		}else{
			bEngineStatus = false;
			API.setVehicleEngineStatus(veh, bEngineStatus);
			g_menu.Visible = false;
		}
	}else{
		API.sendNotification("Spieler nicht im Fahrzeug.");
		
	}
	API.triggerServerEvent("ENGINE_STATUS", bEngineStatus );
	
});

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == "SEND_VEHICLE_FUEL"){
		currentVehicleFuel = args[0];
	}
	  
});

API.onKeyDown.connect(function(sender, e) {
	var veh = API.getPlayerVehicle(player);
	var inVeh = API.isPlayerInAnyVehicle(player);
	var bEngineStatus = API.getVehicleEngineStatus(veh);
	if (inVeh){
		if (e.KeyCode === Keys.M ) {
			//g_menu.Visible = !g_menu.Visible;
			if (bEngineStatus){
				bEngineStatus = false;
				API.setVehicleEngineStatus(veh, bEngineStatus);
				API.sendNotification("Motor wurde ausgeschaltet.");
			}else{
				bEngineStatus = true;
				API.setVehicleEngineStatus(veh, bEngineStatus);
				API.sendNotification("Motor wurde eingeschaltet.");
			}
			API.triggerServerEvent("ENGINE_STATUS", bEngineStatus );
		}
	}
});

API.onUpdate.connect(function (s, e) {
	player = API.getLocalPlayer();
	inVeh = API.isPlayerInAnyVehicle(player);
	EngineStatusOn = API.getVehicleEngineStatus(API.getPlayerVehicle(player));
	API.drawMenu(g_menu);
});