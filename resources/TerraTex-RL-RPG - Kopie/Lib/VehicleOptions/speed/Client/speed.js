var res_X = API.getScreenResolutionMaintainRatio().Width;

API.onUpdate.connect(function() {
	var player = API.getLocalPlayer();
	var inVeh = API.isPlayerInAnyVehicle(player);
	

	if (inVeh) {
		var car = API.getPlayerVehicle(player);
		var health = API.getVehicleHealth(car);
		var rpm = API.getVehicleRPM(car);
		var velocity = API.getEntityVelocity(car);
		var speed = Math.sqrt(
			velocity.X * velocity.X +
			velocity.Y * velocity.Y +
			velocity.Z * velocity.Z
			);
	}

	if (inVeh) {
		API.dxDrawTexture("Lib/VehicleOptions/images/vehiclespeedicon.png", new Point(res_X -160, 500), new Size(50, 50), 0.0);
		API.drawText("" + (speed * 3.6).toFixed() +" km/h", res_X - 15, 500, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
	}
	
});