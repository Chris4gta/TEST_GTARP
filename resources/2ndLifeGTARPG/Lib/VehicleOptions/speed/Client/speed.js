var res_X = API.getScreenResolutionMaintainRatio().Width;

var date = new Date();
var seconds = date.getSeconds();
var TimeMatch = seconds - 1;
var test = 0.434;
API.onUpdate.connect(function (s, e) {
	var date = new Date();
	seconds = date.getSeconds();
	var player = API.getLocalPlayer();
	var inVeh = API.isPlayerInAnyVehicle(player);
	

	if(TimeMatch != seconds){
		TimeMatch = seconds;
	}


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

		API.dxDrawTexture("Lib/VehicleOptions/images/vehiclespeedicon.png", new Point(res_X -160, 500), new Size(50, 50), 0.0);
		API.drawText("" + (speed * 3.6).toFixed() +" km/h", res_X - 15, 500, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
		speed = speed * 3.6;
		//Ist notwendig damit Werte an Server geschickt werden käönnen ka warum
		rpm = rpm + 0.002;
		speed = speed + 0.002;
	
		API.triggerServerEvent("GET_RPM", rpm, speed, API.isControlPressed(22), TimeMatch,seconds );
	}	
});