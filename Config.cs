using CommandSystem;
using Exiled;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Items.FirearmModules;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using LabApi.Events.Handlers;
using PlayerRoles;
using System;
using Exiled.Events;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Blizzard
{
    public class Config : IConfig
    {

        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        public string BlizzardPermission { get; set; } = "Weather.Blizzard";

        public TimeSpan BlizzardDurationTime { get; set; } = TimeSpan.FromMinutes(10);

        public TimeSpan MedkitHeatTime { get; set; } = TimeSpan.FromSeconds(30);

        public byte BlizStrongness { get; set; } = 100 ;

        public float BlizzardZoneY { get; set; } = 270f;

    }

    public class BlizzardPlugin : Plugin<Config>
    {

        public static BlizzardPlugin Instance { get; private set; }
        public override string Name => "Blizzard";

        public override string Author => "site27-whitedoor";

        public override Version Version => new Version(1, 0, 0);

        internal Dictionary<Int64, DateTime> LastHeat;

        internal Dictionary<Int64, bool> InBlizzard;

        internal CancellationTokenSource ctsBlz;

        internal readonly object _dictLock = new object();

        public override void OnEnabled()
        {
            Log.Info("[Blizzard] Plugin Is Preped");

            Instance = this;

            ctsBlz = null;

            LastHeat = new Dictionary<Int64, DateTime>();
            InBlizzard = new Dictionary<Int64, bool>();

            Exiled.Events.Handlers.Player.UsedItem += OnUsingItem;
            Exiled.Events.Handlers.Player.Joined += OnPlayerJoined;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;

            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Log.Info("[Blizzard] Plugin Is No More Avaliable");

            ctsBlz?.Cancel();
            ctsBlz?.Dispose();
            ctsBlz = null;

            LastHeat.Clear();
            InBlizzard.Clear();

            Exiled.Events.Handlers.Player.UsedItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.Joined -= OnPlayerJoined;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;

            Instance = null;

            base.OnDisabled();
        }

        private void OnUsingItem(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Medkit)
            {

                lock (_dictLock)
                {

                    if (LastHeat.ContainsKey(ev.Player.Id))
                    {
                        LastHeat[ev.Player.Id] = DateTime.UtcNow;
                    }
                    else
                    {
                        LastHeat.Add(ev.Player.Id, DateTime.UtcNow);
                    }

                }

            }
        }

        private void OnPlayerJoined(JoinedEventArgs ev)
        {
            lock (_dictLock)
            {

                if (!LastHeat.ContainsKey(ev.Player.Id))
                {
                    LastHeat.Add(ev.Player.Id, DateTime.MinValue);
                }
                if (!InBlizzard.ContainsKey(ev.Player.Id))
                {
                    InBlizzard.Add(ev.Player.Id, false);
                }

            }
        }

        private void OnPlayerLeft(LeftEventArgs ev)
        {

            lock (_dictLock)
            {

                if (LastHeat.ContainsKey(ev.Player.Id))
                {
                    LastHeat.Remove(ev.Player.Id);
                }
                if (InBlizzard.ContainsKey(ev.Player.Id))
                {
                    InBlizzard.Remove(ev.Player.Id);
                }

            }
        }

        private void OnPlayerDied(DiedEventArgs ev)
        {
            Player player = ev.Player;

            lock (_dictLock)
            {
                if (LastHeat.ContainsKey(ev.Player.Id))
                {
                    LastHeat[player.Id] = DateTime.MinValue;
                }
                if (InBlizzard.ContainsKey(ev.Player.Id))
                {
                    InBlizzard[player.Id] = false;
                }
            }

        }
    }
}
