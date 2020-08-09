using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System.Collections.Generic;
using System.Linq;

namespace GTAVModMenuPW
{
    public class Class1 : Script
    {
        Vehicle Car;
        MenuPool modMenuPool = new MenuPool();
        UIMenu MainMenu = new UIMenu("Main Menu", "Select an Option");
        UIMenu PlayerSettingsMenu;
        UIMenu TeleportSettingsMenu;
        UIMenu WeaponSettingsMenu;
        UIMenu VehicleSettingsMenu;
        UIMenu WorldSettingsMenu;
        UIMenu CameraSettingsMenu;

        bool Invincible = false;
        bool NeverWanted = false;
        bool InfiniteNoReload = false;
        bool HonkBoosting = false;
        bool AutoTuning = false;
        bool AimbotPeds = false;
        bool TimeSync = false;
        bool VehiclesHurry = false;
        bool Seatbelt = false;
        public Class1()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;

            modMenuPool.Add(MainMenu);
            PlayerSettingsMenu = modMenuPool.AddSubMenu(MainMenu, "Player");
            TeleportSettingsMenu = modMenuPool.AddSubMenu(MainMenu, "Teleport");
            WeaponSettingsMenu = modMenuPool.AddSubMenu(MainMenu, "Weapons");
            VehicleSettingsMenu = modMenuPool.AddSubMenu(MainMenu, "Vehicles");
            WorldSettingsMenu = modMenuPool.AddSubMenu(MainMenu, "World");
            CameraSettingsMenu = modMenuPool.AddSubMenu(MainMenu, "Camera");

            SetupPlayerMenu();
            SetupTeleportMenu();
            SetupWeaponsMenu();
            SetupVehiclesMenu();
            SetupWorldMenu();
            SetupCameraMenu();
        }

        public void SetupPlayerMenu()
        {
            UIMenuItem KillPlayer = new UIMenuItem("Kill The Player");
            UIMenuCheckboxItem UnlimitedHealthUI = new UIMenuCheckboxItem("Is Invincible", false);
            UIMenuCheckboxItem NeverWantedUI = new UIMenuCheckboxItem("Never Wanted", false);
            UIMenuListItem SetWantedLevel = new UIMenuListItem("Set Wanted Level: ", new List<object>() { 0, 1, 2, 3, 4, 5}, 0);
            UIMenuItem MaxSkills = new UIMenuItem("Max Skills");
            UIMenuItem GiveCashButton = new UIMenuItem("Add Random Amount Of Cash");

            PlayerSettingsMenu.AddItem(KillPlayer);
            PlayerSettingsMenu.AddItem(UnlimitedHealthUI);
            PlayerSettingsMenu.AddItem(NeverWantedUI);
            PlayerSettingsMenu.AddItem(SetWantedLevel);
            PlayerSettingsMenu.AddItem(MaxSkills);
            PlayerSettingsMenu.AddItem(GiveCashButton);
            

            PlayerSettingsMenu.OnItemSelect += (sender, item, index) =>
            {
                if(item == GiveCashButton)
                {
                    Game.Player.Money += new Random().Next(1, 1000000);
                }

                if(item == MaxSkills)
                {
                    Game.Player.Character.Accuracy = 100;
                }

                if(item == KillPlayer)
                {
                    Game.Player.Character.ApplyDamage(Game.Player.Character.MaxHealth);
                }
            };

            PlayerSettingsMenu.OnCheckboxChange += (sender, checkbox, isChecked) =>
            {
                if (checkbox == NeverWantedUI)
                {
                    NeverWanted = isChecked;
                }
                else if (checkbox == UnlimitedHealthUI)
                {
                    Invincible = isChecked;
                }
            };

            PlayerSettingsMenu.OnListChange += (sender, list, index) =>
            {
                if(list == SetWantedLevel)
                {
                    Game.Player.WantedLevel = index;
                }
            };
        }

        public void SetupTeleportMenu()
        {
            UIMenuItem Marker = new UIMenuItem("To Marker");

            TeleportSettingsMenu.AddItem(Marker);

            TeleportSettingsMenu.OnItemSelect += (sender, item, index) =>
            {
                if(item == Marker)
                {
                    Game.Player.Character.Position = World.GetWaypointPosition();
                }
            };
        }

        public void SetupWeaponsMenu()
        {
            UIMenuItem GiveAllWeapons = new UIMenuItem("Give All Weapons");
            UIMenuCheckboxItem InfiniteNoReloadUI = new UIMenuCheckboxItem("Infinite Ammo / No Reload", false);

            WeaponSettingsMenu.AddItem(GiveAllWeapons);
            WeaponSettingsMenu.AddItem(InfiniteNoReloadUI);

            WeaponSettingsMenu.OnItemSelect += (sender, item, index) =>
            {
                if(item == GiveAllWeapons)
                {
                    foreach(WeaponHash wh in Enum.GetValues(typeof(WeaponHash)))
                    {
                        if(!Game.Player.Character.Weapons.HasWeapon(wh))
                        {
                            Game.Player.Character.Weapons.Give(wh, 9999, false, true);
                        }
                }
                }
            };

            WeaponSettingsMenu.OnCheckboxChange += (sender, checkbox, isChecked) =>
            {
                if(checkbox == InfiniteNoReloadUI)
                {
                    InfiniteNoReload = isChecked;
                }
            };
        }

        public void SetupVehiclesMenu()
        {
            List<dynamic> CarsInList = new List<dynamic>();
            foreach (VehicleHash vh in Enum.GetValues(typeof(VehicleHash)))
            {
                CarsInList.Add(vh);
            }

            List<dynamic> VehicleColors = new List<dynamic>();
            foreach (VehicleColor vc in Enum.GetValues(typeof(VehicleColor)))
            {
                VehicleColors.Add(vc);
            }

            UIMenu VehicleUpgrades = modMenuPool.AddSubMenu(VehicleSettingsMenu, "Vehicle Upgrades");

            UIMenuItem MaxUpdates = new UIMenuItem("Add Max Upgrades");
            UIMenuCheckboxItem AutoTune = new UIMenuCheckboxItem("Auto Tune Vehicle", false);
            UIMenuItem Repair = new UIMenuItem("Repair");
            UIMenuListItem PrimairyColorList = new UIMenuListItem("Primary Color: ", VehicleColors, 0);
            UIMenuListItem SecondairyColorList = new UIMenuListItem("Secondairy Color: ", VehicleColors, 0);
            UIMenuItem ApplyColors = new UIMenuItem("Apply Colors to Current Vehicle");
            UIMenuCheckboxItem HonkBoostingUI = new UIMenuCheckboxItem("Honkboosting", false);
            UIMenuListItem CarList = new UIMenuListItem("Car List: ", CarsInList, 0);
            UIMenuItem CarSpawnButton = new UIMenuItem("Spawn Selected Car");
            UIMenuItem KillEngine = new UIMenuItem("Kill Engine of Current Vehicle");
            UIMenuCheckboxItem SeatBeltUI = new UIMenuCheckboxItem("Seat Belt: ", false);

            VehicleUpgrades.AddItem(MaxUpdates);
            VehicleUpgrades.AddItem(AutoTune);
            VehicleUpgrades.AddItem(PrimairyColorList);
            VehicleUpgrades.AddItem(SecondairyColorList);
            VehicleUpgrades.AddItem(ApplyColors);
            VehicleSettingsMenu.AddItem(Repair);
            VehicleSettingsMenu.AddItem(HonkBoostingUI);
            VehicleSettingsMenu.AddItem(CarList);
            VehicleSettingsMenu.AddItem(CarSpawnButton);
            VehicleSettingsMenu.AddItem(KillEngine);
            VehicleSettingsMenu.AddItem(SeatBeltUI);

            VehicleUpgrades.OnItemSelect += (sender, item, index) =>
            {
                if (item == MaxUpdates)
                {
                    if (Game.Player.Character.IsInVehicle())
                    {
                        Vehicle CurCar = Game.Player.Character.CurrentVehicle;
                        Function.Call(Hash.SET_VEHICLE_MOD_KIT, CurCar.Handle, 0);

                        foreach (VehicleMod vm in Enum.GetValues(typeof(VehicleMod)))
                        {
                            CurCar.SetMod(vm, CurCar.GetModCount(vm) - 1, true);
                        }

                        foreach(VehicleNeonLight vnl in Enum.GetValues(typeof(VehicleNeonLight)))
                        {
                            CurCar.SetNeonLightsOn(vnl, true);
                        }
                        Car.NeonLightsColor = new Random().Next(0, 2) == 0 ? Color.Red : new Random().Next(0, 2) == 0 ? Color.Blue : Color.Green;
                        Car.TireSmokeColor = Color.Black;
                    }
                    else
                    {
                        UI.ShowSubtitle("Player Isn't in a Vehicle!");
                    }
                }

                if (item == ApplyColors)
                {
                    if (Game.Player.Character.IsInVehicle())
                    {
                        Vehicle CurCar = Game.Player.Character.CurrentVehicle;
                        CurCar.PrimaryColor = VehicleColors[PrimairyColorList.Index];
                        CurCar.SecondaryColor = VehicleColors[SecondairyColorList.Index];
                    }
                    else
                    {
                        UI.ShowSubtitle("Player Isn't in a Vehicle!");
                    }
                }
            };

            VehicleSettingsMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == CarSpawnButton)
                {
                    World.CreateVehicle(CarsInList[CarList.Index], Game.Player.Character.Position + Game.Player.Character.ForwardVector * 5);
                }

                if (item == Repair)
                {
                    if (Game.Player.Character.IsInVehicle())
                    {
                        Vehicle CurCar = Game.Player.Character.CurrentVehicle;
                        CurCar.Repair();
                    }
                    else
                    {
                        UI.ShowSubtitle("Player Isn't in a Vehicle!");
                    }
                }

                if (item == KillEngine)
                {
                    if (Game.Player.Character.IsInVehicle())
                    {
                        Game.Player.Character.CurrentVehicle.EngineHealth = 0f;
                    }
                    else
                    {
                        UI.ShowSubtitle("Player Isn't in a Vehicle!");
                    }
                }
            };

            VehicleUpgrades.OnCheckboxChange += (sender, checkbox, isChecked) =>
            {
                if (checkbox == AutoTune)
                {
                    AutoTuning = isChecked;
                }
            };

            VehicleSettingsMenu.OnCheckboxChange += (sender, checkbox, isChecked) =>
            {
                if(checkbox == HonkBoostingUI)
                {
                    HonkBoosting = isChecked;
                }

                if(checkbox == SeatBeltUI)
                {
                    Seatbelt = isChecked;
                }
            };
        }

        public void SetupWorldMenu()
        {
            UIMenu Peds = modMenuPool.AddSubMenu(WorldSettingsMenu, "Peds");
            UIMenuListItem TimeList = new UIMenuListItem("Change Time: ", new List<object>() { "00:00", "01:00", "02:00",
           "03:00", "04:00", "05:00","06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00",
            "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" }, 0);
            UIMenuCheckboxItem SyncTimeRealTime = new UIMenuCheckboxItem("Sync with real time:", false);
            UIMenuItem Vehicles = new UIMenuItem("Explode Nearby Cars");
            UIMenuCheckboxItem VehiclesHurryUI = new UIMenuCheckboxItem("All Vehicles Hurry", false);
            UIMenuItem SpawnPedDriver = new UIMenuItem("Spawn Random Driver");
            UIMenuItem NearbyPedDriver = new UIMenuItem("Make Random Nearby Ped Driver");
            UIMenuItem AllPedsDance = new UIMenuItem("All Peds Dance");
            UIMenuCheckboxItem AimbotPedsUI = new UIMenuCheckboxItem("Peds Have Aimbot: ", false);
            UIMenuItem KillPeds = new UIMenuItem("Kill Nearby Peds");
            UIMenuItem TeleportPeds = new UIMenuItem("Teleport Nearby Peds To Player");
            UIMenuItem BodyGuardPeds = new UIMenuItem("Make Nearby Peds BodyGuards");

            WorldSettingsMenu.AddItem(TimeList);
            WorldSettingsMenu.AddItem(SyncTimeRealTime);
            WorldSettingsMenu.AddItem(Vehicles);
            WorldSettingsMenu.AddItem(VehiclesHurryUI);
            Peds.AddItem(SpawnPedDriver);
            Peds.AddItem(NearbyPedDriver);
            Peds.AddItem(AllPedsDance);
            Peds.AddItem(AimbotPedsUI);
            Peds.AddItem(KillPeds);
            Peds.AddItem(TeleportPeds);
            Peds.AddItem(BodyGuardPeds);

            WorldSettingsMenu.OnItemSelect += (sender, item, index) =>
            {
                if(item == Vehicles)
                {
                    foreach(Vehicle v in World.GetNearbyVehicles(Game.Player.Character, 100f))
                    {
                        v.Explode();
                    }
                }
            };

            WorldSettingsMenu.OnCheckboxChange += (sender, checkbox, isChecked) =>
            {
                if(checkbox == SyncTimeRealTime)
                {
                    TimeSync = isChecked;
                }

                if(checkbox == VehiclesHurryUI)
                {
                    VehiclesHurry = isChecked;
                }
            };

            WorldSettingsMenu.OnListChange += (sender, list, index) =>
            {
                if(list == TimeList)
                {
                    World.CurrentDayTime = new TimeSpan(index, 0, 0);
                }
            };

            Peds.OnItemSelect += (sender, item, index) =>
            {
                if (item == KillPeds)
                {
                    foreach (Ped p in World.GetNearbyPeds(Game.Player.Character, 100f))
                    {
                        p.ApplyDamage(p.MaxHealth);
                    }
                }

                if(item == NearbyPedDriver)
                {
                    if (Game.Player.Character.IsInVehicle())
                    {
                        Game.Player.Character.SetIntoVehicle(Game.Player.Character.CurrentVehicle, VehicleSeat.Passenger);
                        Ped Driver = null;

                        foreach(Ped p in World.GetNearbyPeds(Game.Player.Character.Position, 100f))
                        {
                            if(p != Game.Player.Character && p.IsAlive)
                            {
                                Driver = p;
                                Driver.AddBlip().Color = BlipColor.Blue;
                                break;
                            }
                        }

                        Driver.Task.DriveTo(Game.Player.Character.CurrentVehicle, World.GetWaypointPosition(), 5.0f, 30.0f, 7);
                        Driver.AlwaysKeepTask = true;
                        Driver.BlockPermanentEvents = true;
                    }
                    else
                    {
                        UI.ShowSubtitle("Player Isn't in a Vehicle!");
                    }
                }

                if (item == TeleportPeds)
                {
                    foreach (Ped p in World.GetNearbyPeds(Game.Player.Character, 100f))
                    {
                        if (p.IsAlive)
                        {
                            p.Position = Game.Player.Character.Position.Around(3);
                        }
                    }
                }

                if (item == BodyGuardPeds)
                {
                    foreach (Ped p in World.GetNearbyPeds(Game.Player.Character, 100f))
                    {
                        if (p.IsAlive)
                        {
                            p.Weapons.Give(WeaponHash.AssaultRifle, 9999, true, true);
                            Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, p, Game.Player.Character.CurrentPedGroup);
                            Function.Call(Hash.SET_PED_COMBAT_ABILITY, p, 100);
                        }
                    }
                }

                if (item == AllPedsDance)
                {
                    foreach (Ped p in World.GetAllPeds())
                    {
                        if (p != Game.Player.Character && p.IsAlive)
                        {
                            p.Task.ClearAll();
                            p.Task.PlayAnimation("missfbi3_sniping", "dance_m_default", -1, -1, AnimationFlags.Loop);
                        }
                    }
                }

                if(item == SpawnPedDriver)
                {

                    if(Game.Player.Character.IsInVehicle())
                    {
                        Game.Player.Character.SetIntoVehicle(Game.Player.Character.CurrentVehicle, VehicleSeat.Passenger);

                        Ped Driver = Game.Player.Character.CurrentVehicle.CreateRandomPedOnSeat(VehicleSeat.Driver);
                        Driver.Task.DriveTo(Game.Player.Character.CurrentVehicle, World.GetWaypointPosition(), 5.0f, 30.0f, 7);
                        Driver.AlwaysKeepTask = true;
                        Driver.BlockPermanentEvents = true;
                    }
                    else
                    {
                        UI.ShowSubtitle("Player Isn't in a Vehicle!");
                    }
                }
            };

            Peds.OnCheckboxChange += (sender, checkbox, isChecked) =>
            {
                if (checkbox == AimbotPedsUI)
                {
                    AimbotPeds = isChecked;
                }
            };
        }

        public void SetupCameraMenu()
        {
            UIMenuItem LookAtPlayer = new UIMenuItem("Look At Player");
            UIMenuItem FollowClosestPed = new UIMenuItem("Follow Closest Ped");

            CameraSettingsMenu.AddItem(LookAtPlayer);
            CameraSettingsMenu.AddItem(FollowClosestPed);

            CameraSettingsMenu.OnItemSelect += (sender, item, index) =>
            {
                if(item == LookAtPlayer)
                {
                    World.RenderingCamera.PointAt(Game.Player.Character.Position);
                } else if(item == FollowClosestPed)
                {
                    Ped Closest = null;

                    foreach(Ped p in World.GetNearbyPeds(Game.Player.Character.Position, 30f))
                    {
                        if(p != Game.Player.Character)
                        {
                            Closest = p;
                        }
                    }    
                    World.RenderingCamera.AttachTo(Closest, new Vector3(3, 3, 3));
                    World.RenderingCamera.PointAt(Closest.Position);
                }
            };
        }

        // Fires every frame
        public void OnTick(object sender, EventArgs e)
        {
            //UI.ShowSubtitle(World.RenderingCamera.Position.X + ", " + World.RenderingCamera.Position.Y + ", " + World.RenderingCamera.Position.Z);
            if(modMenuPool != null && modMenuPool.IsAnyMenuOpen())
            {
                modMenuPool.ProcessMenus();
            }

            if(NeverWanted)
            {
                Game.Player.WantedLevel = 0;
            }

            if(InfiniteNoReload)
            {
                //Game.Player.Character.Weapons.Current.AmmoInClip = Game.Player.Character.Weapons.Current.MaxAmmoInClip;
                Game.Player.Character.Weapons.Current.InfiniteAmmo = true;
                Game.Player.Character.Weapons.Current.InfiniteAmmoClip = true;
            }

            if(AutoTuning && Game.Player.Character.IsInVehicle())
            {
                if(Car != Game.Player.Character.CurrentVehicle)
                {
                    Car = Game.Player.Character.CurrentVehicle;
                    Function.Call(Hash.SET_VEHICLE_MOD_KIT, Car.Handle, 0);

                    foreach (VehicleMod vm in Enum.GetValues(typeof(VehicleMod)))
                    {
                        Car.SetMod(vm, Car.GetModCount(vm) - 1, true);
                    }

                    foreach (VehicleNeonLight vnl in Enum.GetValues(typeof(VehicleNeonLight)))
                    {
                        Car.SetNeonLightsOn(vnl, true);
                    }
                    Car.NeonLightsColor = new Random().Next(0, 2) == 0 ? Color.Red : new Random().Next(0, 2) == 0 ? Color.Blue : Color.Green;
                    Car.TireSmokeColor = Color.Black;
                }
            }

            if(AimbotPeds)
            {
                foreach(Ped p in World.GetAllPeds())
                {
                    p.Accuracy = 100;
                }
            }

            if(TimeSync)
            {
                DateTime Nows = DateTime.Now;
                World.CurrentDayTime = new TimeSpan(Nows.Hour, Nows.Minute, Nows.Second);
            }

            if(VehiclesHurry)
            {
                foreach(Vehicle veh in World.GetAllVehicles())
                {
                    if(veh.IsDriveable && veh.IsOnAllWheels)
                        veh.Velocity += veh.ForwardVector * 2;
                }
            }

            Game.Player.Character.IsInvincible = Invincible;
            Game.Player.Character.CanFlyThroughWindscreen = !Seatbelt;
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.H)
            {
                World.CreateVehicle(VehicleHash.Adder, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2);
            }

            if (e.KeyCode == Keys.E)
            {
                if(Game.Player.Character.IsInVehicle() && HonkBoosting)
                    Game.Player.Character.CurrentVehicle.Velocity += Game.Player.Character.CurrentVehicle.ForwardVector * 20;
            }

            if(e.KeyCode == Keys.F9)
            {
                MainMenu.Visible = !MainMenu.Visible;
            }

            if(e.KeyCode == Keys.B)
            {
                World.CreateVehicle(VehicleHash.Bus, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2);
            }

            if(e.KeyCode == Keys.T)
            {
                World.CreateVehicle(VehicleHash.TowTruck, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2);
            }
        }
    }
}
