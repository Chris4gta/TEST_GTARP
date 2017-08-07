﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace GTFuel
{
    public class Fuel : VehicleFuelScript
    {

        public static string fuelLevelPropertyName = "_Fuel_Level";
        public static string manualRefuelAnimDict = "weapon@w_sp_jerrycan";

        public static string[] tankBones = new string[] {
        "petrolcap",
        "petroltank",
        "petroltank_r",
        "petroltank_l",
        "wheel_lr",
        };


        protected Blip[] blips;
        protected Pickup[] pickups;

        protected float fuelTankCapacity = 65f;

        protected float fuelAccelerationImpact = 0.0002f;
        protected float fuelTractionImpact = 0.0001f;
        protected float fuelRPMImpact = 0.0005f;

        public float showMarkerInRangeSquared = 250f;


        public Random random = new Random();

        private int currentGasStationIndex;
        private Vehicle lastVehicle;

        protected bool currentVehicleFuelLevelInitialized = false;
        protected bool hudActive = false;
        protected bool refuelAllowed = true;
        protected float addedFuelCapacitor = 0f;


        protected Vehicle LastVehicle { get => lastVehicle; set => lastVehicle = value; }

        public Fuel()
        {
            API.onClientEventTrigger += OnClientEvent;


            blips = new Blip[GasStations.positions.Length];
            pickups = new Pickup[GasStations.positions.Length];

            try
            {
                CreateBlips();
                API.sendChatMessageToAll("Blips erzeugt");
            }
            catch
            {
                // nothing
            }

        }



        public void OnClientEvent(Client player, string eventName, params object[] args ) //arguments param can contain multiple params
        {
            NetHandle vehicle = API.getPlayerVehicle(player);
            if (eventName == "CAR_IS_REFUELING")
            {
                FuelList.Set(vehicle, VehicleMaxFuelLevel(player.vehicle));
                API.triggerClientEvent(player, "update_fuel_client", FuelList.Get(vehicle));
            }

            if (eventName == "UPDATE_FUEL")
            {
                float CurrentTank;

                for (int i = FuelList.Count - 1; i >= 0; i--)
                {
                    var item = FuelList.ElementAt(i);
                    var itemKey = item.Key;
                    var itemValue = item.Value;
                    bool EngineStatus = API.getVehicleEngineStatus(itemKey);
                    API.sendChatMessageToAll("Motor an " + EngineStatus);
                    if (EngineStatus)
                    {
                        CurrentTank = FuelList.Get(itemKey);
                        CurrentTank = CurrentTank - 1f;

                        if (CurrentTank <= 0f)
                        {
                            CurrentTank = 0f;
                            API.setVehicleEngineStatus(itemKey, false);
                        }
                        API.sendChatMessageToAll("FahrzeugID " + itemKey + " Tank " + itemValue);
                        FuelList.Set(itemKey, CurrentTank);

                        if (API.isPlayerInAnyVehicle(player))
                        {
                            API.triggerClientEvent(player, "update_fuel_client", FuelList.Get(vehicle));
                        }
                    }
                }
            }

            if (eventName == "GET_VEHICLE_FUEL")
            {
                API.triggerClientEvent(player, "SEND_VEHICLE_FUEL", FuelList.Get(vehicle));
            }
            if (eventName == "GET_RPM")
            {
                //float vehicleRPM = (float)args[0];
                //int vehicleSpeed = (int)args[1];
                bool isJumpPressed = (bool)args[2];
                RenderUI(player);

                //API.sendChatMessageToAll(""+ vehicleSpeed + "");
                ConsumeFuel(player.vehicle, 1f, 1f, isJumpPressed, player, API.getPlayerVehicle(player));
                
            }


        }
        /// <summary>
        /// 
        /// </summary>
        public void CreateBlips()
        {
            for (int i = 0; i < GasStations.positions.Length; i++)
            {
                var blip = API.createBlip(GasStations.positions[i]);
                blip.sprite = 361;
                blip.color = 72;
                blip.scale = 1f;
                blip.name = "Gas Station";

                blips[i] = blip;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rangeSquared"></param>
        /// <returns></returns>
        public int GetGasStationIndexInRange(Vector3 pos, float rangeSquared)
        {
            for (int i = 0; i < blips.Length; i++)
            {
                Blip blip = blips[i];

                if (Vector3.DistanceSquared(GasStations.positions[i], pos) < rangeSquared)
                {
                    return i;
                }
            }

            return -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerPed"></param>
        public void RenderUI(Client playerPed)
        {

            //hud.RenderBar(playerPed.CurrentVehicle.FuelLevel, fuelTankCapacity);

            var gasStationIndex = GetGasStationIndexInRange(playerPed.position, showMarkerInRangeSquared);

            if (gasStationIndex != -1)
            {
                if (gasStationIndex != currentGasStationIndex)
                {
                    // Found gas station in range
                    API.sendChatMessageToAll("GasStation Found");
                    currentGasStationIndex = gasStationIndex;
                }
            }
            else
            {
                if (currentGasStationIndex != -1)
                {
                    // Lost gas station in range
                    currentGasStationIndex = -1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public Vector3 GetVehicleTankPos(Vehicle vehicle)
        {
            return vehicle.position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="player"></param>
        /// <param name="isJumpPressed"></param>
        /// <returns></returns>
        public bool IsVehicleNearAnyPump(Vehicle vehicle, Client player, bool isJumpPressed)
        {
            var fuelTankPos = GetVehicleTankPos(vehicle);

            foreach (var pump in GasStations.pumps[currentGasStationIndex])
            {
                if (Vector3.DistanceSquared(pump, fuelTankPos) <= 20f)
                {
                    if (isJumpPressed)
                    {
                        API.triggerClientEvent(player, "TRIGGER_REFUEL_MENU");
                    }
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vehicleRPM"></param>
        /// <param name="vehicleSpeed"></param>
        /// <param name="isJumpPressed"></param>
        /// <param name="player"></param>
        /// <param name="NetVehicle"></param>
        public void ConsumeFuel(Vehicle vehicle, float vehicleRPM, float vehicleSpeed, bool isJumpPressed, Client player, NetHandle NetVehicle)
        {

            float fuel = FuelList.Get(NetVehicle);
            float VehConsum = GetVehicleConsume(vehicle);
            
            // Consuming
            if (fuel > 0 && vehicle.engineStatus)
            {
                float normalizedRPMValue = (float)Math.Pow(VehConsum, 1.5);
                
                fuel -= normalizedRPMValue * fuelRPMImpact;
                //fuel -= vehicleSpeed * fuelAccelerationImpact;
                fuel -= vehicle.maxTraction * fuelTractionImpact;
                
                fuel = fuel < 0f ? 0f : fuel;
                
                FuelList.Set(NetVehicle, fuel);

                if (fuel <= 0f)
                {
                    fuel = 0f;
                    API.setVehicleEngineStatus(NetVehicle, false);
                }

                API.triggerClientEvent(player, "update_fuel_client", FuelList.Get(player.vehicle));
            }
           
            // Refueling at gas station
            if (
              // If we have gas station near us
              currentGasStationIndex != -1 &&
              // And near any pump
              IsVehicleNearAnyPump(vehicle, player, isJumpPressed)
            )
            {
                if (vehicleSpeed < 0.1f)
                {
                    //ControlEngine(vehicle);
                }

                if (vehicle.engineStatus)
                {
                    //hud.InstructTurnOffEngine();
                }
                else
                {
                    //hud.InstructRefuelOrTurnOnEngine();

                    if (refuelAllowed)
                    {
                        if (isJumpPressed)
                        {
                            /* Tank aufüllen
                            if (fuel < fuelTankCapacity)
                            {
                                fuel += 0.1f;
                                addedFuelCapacitor += 0.1f;
                            }
                            */
                        }

                        if (!isJumpPressed && addedFuelCapacitor > 0f)
                        {
                            //TriggerEvent("frfuel:fuelAdded", addedFuelCapacitor);
                            //TriggerServerEvent("frfuel:fuelAdded", addedFuelCapacitor);
                            addedFuelCapacitor = 0f;
                        }
                    }
                }

                //hud.RenderInstructions();
                //PlayHUDAppearSound();
                //hudActive = true;
            }
            else
            {
                /*
                if (fuel != 0f && !vehicle.IsEngineRunning)
                {
                    vehicle.IsEngineRunning = true;
                }

                hudActive = false;
                */
            }

            VehicleSetFuelLevel(vehicle, fuel, NetVehicle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="fuelLevel"></param>
        /// <param name="NetVehicle"></param>
        public void VehicleSetFuelLevel(Vehicle vehicle, float fuelLevel, NetHandle NetVehicle)
        {
            float max = VehicleMaxFuelLevel(vehicle);

            if (fuelLevel > max)
            {
                fuelLevel = max;
            }

            FuelList.Set(NetVehicle, fuelLevel);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public float VehicleMaxFuelLevel(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                if (VehiclesPetrolTanks.Has(vehicle))
                {
                    return VehiclesPetrolTanks.Get(vehicle);
                }
                else
                {
                    return 65f;
                }
            }

            API.sendChatMessageToAll("Vehicle is null");
            return 65f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public float GetVehicleConsume(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                if (VehiclesPetrolTanks.Has(vehicle))
                {
                    return VehiclesPetrolTanks.Get(vehicle);
                }
                else
                {
                    return 5f;
                }
            }

            return 5f;
        }

    }
}
