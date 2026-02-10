using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using System.ComponentModel;
using Exiled;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Exiled.API;
using Exiled.API.Features.Items;
using Exiled.API.Features.Items.FirearmModules;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;

namespace AutoTurretPlugin
{
    internal class Commands
    {

        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        [CommandHandler(typeof(GameConsoleCommandHandler))]
        [CommandHandler(typeof(ClientCommandHandler))]

        public class SpawnTurretCommand : ICommand
        {

            public string Command => "autoturret.spawn";

            public string[] Aliases => new string[] { };

            public string Description => "Spawns an auto turret at your position.";

            public string Permission => TurrletPlugin.Instance.Config.SpawnTurretPermissions;

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player TurretSpawner = Player.Get(sender);

                if (!sender.CheckPermission( Permission ) )
                {
                    response = "You do not have permission to use this command.";

                    return false;
                }
                if (TurretSpawner == null )
                {
                    response = "Can only be used by players, can't be used by off-game remotes";

                    return false;

                }

                try
                {

                    _ = new TurretSpawning().SpawnTurret(TurretSpawner.Position, TurretSpawner.Rotation , TurretSpawner );

                    response = "Turret spawned.";

                    return true;
                }
                catch (Exception e)
                {
                    response = $"An error occurred while spawning the turret:{e.Message}";

                    Log.Error($"An error:{e} occured while spawning a turret ");

                    return false;
                    

                }

            }
        }
    }
}
