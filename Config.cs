using CommandSystem;
using Exiled;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Items.FirearmModules;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static PlayerList;
using static UnityEngine.GraphicsBuffer;

namespace AutoTurretPlugin
{

    public class Config : IConfig
    {

        [Description("IsDebugOn")]

        public bool Debug { get; set; } = false;

        [Description("启用自动炮塔")]
        public bool IsEnabled { get; set; } = true;

        [Description("射程")]
        public float Range { get ; set ; } = 1000 ;

        [Description("初始血量")]

        public int StandardHealth { get; set; } = 300;

        [Description("炮塔生成所需权限")]

        public string SpawnTurretPermissions { get; set; } = "autoturret.spawn";

        [Description("弹匣容量")]

        public int MagazineCapacity { get; set; } = 1500;


    }
    public class TurrletPlugin : Plugin<Config>
    {
        public static TurrletPlugin Instance { get; private set; }

        public override string Name => "autoTurlet";

        public override string Author => "site27-WhiteDoor | killjsj";

        public override Version Version => new Version(1, 2, 1);

        public static readonly Dictionary<Player, CancellationTokenSource> turretTasks = new Dictionary<Player, CancellationTokenSource>();
        public override void OnEnabled()
        {
            Instance = this;

            Statics._cnt = 0;

            Exiled.Events.Handlers.Player.Dying += OnTurretDying;
            Exiled.Events.Handlers.Player.Jumping += ListCheck;

            Log.Info("autoturrlets are set!");

            base.OnEnabled();
        }

        public override void OnDisabled()
        {

            Exiled.Events.Handlers.Player.Dying -= OnTurretDying;
            Exiled.Events.Handlers.Player.Jumping -= ListCheck;

            foreach ( var cts in turretTasks.Values)
            {

                cts.Cancel();
                cts.Dispose();

            }
            turretTasks.Clear();

            Instance = null;

            Log.Info("autoturrlets are now banned!");

            base.OnDisabled();
        }

        private void OnTurretDying(DyingEventArgs ev)
        {

            Log.Info("checking if a turret is downed");
            if (turretTasks.ContainsKey( ev.Player ))
            {

                Log.Info("a turret has been downed");
                
                Npc turret = ev.Player as Npc;

                turret.ExplodeEffect(ProjectileType.FragGrenade);

                turret.ClearInventory();

                turret.Destroy();

            }
        }

        private void ListCheck(JumpingEventArgs ev)
        {
            
            foreach (var key in turretTasks.Keys)
            {
                Log.Info($"当前字典内的炮塔有：{key.Nickname}");
            }

        }


    }
}