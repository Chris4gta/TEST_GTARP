var currentVehicleHP;
var player ;
var inVeh;
var veh;
var res_X = API.getScreenResolutionMaintainRatio().Width;
var res_Y = API.getScreenResolutionMaintainRatio().Hight;

API.onVehicleHealthChange.connect(function (sender, olValue) {
	
	API.triggerServerEvent("update_vehhp_server" );
});

API.onUpdate.connect(function (sender, e) {
	
	player = API.getLocalPlayer();
	inVeh = API.isPlayerInAnyVehicle(player);
	veh = API.getPlayerVehicle(player);
	currentVehicleHP = API.getVehicleHealth(veh) / 1000 * 100;
	if (currentVehicleHP != null && inVeh) {
		if(currentVehicleHP < 0){
			currentVehicleHP = 0;
		}
		API.dxDrawTexture("Lib/VehicleOptions/images/vehicledmgicon.png", new Point(res_X -160, 600), new Size(50, 50), 0.0);
        API.drawText("" + (currentVehicleHP).toFixed() +"%", res_X - 15, 600, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
			
    }
});

