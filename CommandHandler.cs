using CommandSystem;
using CustomRendering;
using Exiled;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEC;
using System.Threading;
using CASSIE = Exiled.API.Features.Cassie;

namespace Blizzard
{
    internal class CommandHandler
    {
        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        [CommandHandler(typeof(GameConsoleCommandHandler))]
        [CommandHandler(typeof(ClientCommandHandler))]

        public class BlizzardCommand : ICommand
        {
            public string Command => "Weather.Blizzard";
            public string[] Aliases => new string[] { "bliz" };
            public string Description => "Toggles the blizzard effect.";
            
            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {

                string executer = "Remote/Console";

                Player player = Player.Get(sender); if (player != null) executer = player.Nickname;

                if( player != null && player.CheckPermission(BlizzardPlugin.Instance.Config.BlizzardPermission) == false)
                {
                    response = "You do not have permission to use this command.";

                    return false;
                }

                try
                {

                    string fArg = arguments.FirstOrDefault()?.ToLower();

                    if( fArg == "on")
                    {

                        bool BlizzardCanRun = ( BlizzardPlugin.Instance.ctsBlz == null ) || ( BlizzardPlugin.Instance.ctsBlz.Token.IsCancellationRequested );

                        if (BlizzardCanRun)
                        { 

                            BlizzardPlugin.Instance.ctsBlz?.Cancel();
                            BlizzardPlugin.Instance.ctsBlz?.Dispose();
                            BlizzardPlugin.Instance.ctsBlz = null;


                            BlizzardPlugin.Instance.ctsBlz = new CancellationTokenSource();

                            _ = new WeatherHandler().Bliz(BlizzardPlugin.Instance.ctsBlz.Token);

                            response = "Blizzard effect has been toggled, it'll probably last for 10m if not manually shutted.";

                        }
                        else
                        {
                            response = "Blizzard effect is already running.";

                        }

                    }
                    else if( fArg == "off")
                    {

                        bool BlizzardIsRunning = ( BlizzardPlugin.Instance.ctsBlz != null ) && ( !BlizzardPlugin.Instance.ctsBlz.Token.IsCancellationRequested );

                        if ( BlizzardIsRunning )
                        {

                            new WeatherHandler().BlizEnd();

                            BlizzardPlugin.Instance.ctsBlz?.Cancel();
                            BlizzardPlugin.Instance.ctsBlz?.Dispose();
                            BlizzardPlugin.Instance.ctsBlz = null;



                            response = "Blizzard effect has been manually shutted.";

                        }

                        else
                        {
                            response = "Blizzard effect is not running.";
                        }

                    }
                    else
                    {
                        response = "Invalid argument. Use 'on' to start the blizzard effect or 'off' to stop it.";
                    }

                    return true;

                }
                catch ( Exception e )
                {

                    response = $"An error occurred while executing blizzard: {e.Message} ";

                    Log.Info( $"[Blizzard] An error occurred while {executer} was executing blizzard: {e}" );

                    return false;

                }
            }

        }

    }
}
