using Exiled;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Items.FirearmModules;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Enums;
using RemoteAdmin;
using static PlayerList;
using static UnityEngine.GraphicsBuffer;
using InventorySystem.Items.Firearms.Modules.Misc;
using Exiled.API.Structs;
using CameraShaking;

namespace AutoTurretPlugin
{
    public class TurretSpawning
    {
        public async Task SpawnTurret(Vector3 position, Quaternion rotation ,Player Spawner )
        {
            Statics._cnt++;
            Npc turretNpc = Npc.Spawn($"turret{Statics._cnt}", RoleTypeId.Tutorial, position );

            if (turretNpc == null)
            {
                Log.Error("failed to spawn turret!");
                return;
            }

            turretNpc.Rotation = rotation;

            Vector3 SpPosition = position;

            turretNpc.DisplayNickname = $"自动炮塔id{Statics._cnt}";
            turretNpc.Health = TurrletPlugin.Instance.Config.StandardHealth;

            await Task.Delay(100);

            Item Logicergun = Item.Create(ItemType.GunLogicer);

            if ( Logicergun is Firearm turretgun)
            {

                turretNpc.ClearInventory();

                await Task.Delay(500);

                Log.Info("Trying to prep turrets with guns.");

                turretgun.MaxMagazineAmmo = TurrletPlugin.Instance.Config.MagazineCapacity;
                turretgun.AddAttachment(AttachmentIdentifier.Get(FirearmType.Logicer, InventorySystem.Items.Firearms.Attachments.AttachmentName.Flashlight));
                turretgun.MagazineAmmo = turretgun.MaxMagazineAmmo;
                turretgun.Inaccuracy = 0;
                turretNpc.Health = TurrletPlugin.Instance.Config.StandardHealth;
                turretNpc.AddItem(ItemType.ArmorHeavy);
                turretNpc.AddItem(turretgun);
                turretNpc.CurrentItem = turretgun;


                Log.Info($"武器是否能被使用？{turretgun.IsUsable}");

                if (!turretNpc.TryGetItem( turretgun.Serial , out _ ) )
                {
                    Log.Info("failed to give turret gun to turret npc!");
                    return;
                }

                if( turretNpc.CurrentItem != turretgun)
                {
                    Log.Info("failed to give turret gun to turret npc!");
                    return;
                }

                Log.Info(" Succesfully spawned ");

                var cts = new CancellationTokenSource();
                TurrletPlugin.turretTasks.Add(turretNpc, cts);
                _ = ShootingLogic.AimAndShootLoop(turretNpc, cts.Token, SpPosition);

            }
            else
            {
                Log.Info("failed to create turret gun!");
                turretNpc.Destroy();
                return;
            }

        }

    }
}
