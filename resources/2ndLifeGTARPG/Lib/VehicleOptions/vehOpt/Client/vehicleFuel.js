
var currentVehicleFuel;
var player ;
var inVeh;
var veh;
var res_X = API.getScreenResolutionMaintainRatio().Width;
var res_Y = API.getScreenResolutionMaintainRatio().Hight;

var date = new Date();
var seconds = date.getSeconds();
var TimeMatch = seconds - 1;

API.onServerEventTrigger.connect(function (eventName, args) {
	if (eventName == "update_fuel_client"){
		currentVehicleFuel = args[0];
	}
	  
});


API.onUpdate.connect(function (sender, e) {
	var date = new Date();
	seconds = date.getSeconds();
	player = API.getLocalPlayer();
	inVeh = API.isPlayerInAnyVehicle(player);
	veh = API.getPlayerVehicle(player);
	
	if(TimeMatch != seconds){
		TimeMatch = seconds;
		API.triggerServerEvent("UPDATE_FUEL");
	}
	


	if (currentVehicleFuel != null && inVeh) {
		API.dxDrawTexture("Lib/VehicleOptions/images/vehiclefuelicon.png", new Point(res_X -160, 550), new Size(50, 50), 0.0);
        API.drawText("" + (currentVehicleFuel).toFixed(2) +" L", res_X - 15, 550, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);


    }
});




