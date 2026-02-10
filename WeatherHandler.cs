using CustomPlayerEffects;
using CustomRendering;
using Exiled;
using Exiled.API.Enums;
using Exiled.API.Features;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Player = Exiled.API.Features.Player;
using Mirror;
using System.Reflection;

namespace Blizzard
{

    internal class WeatherHandler
    {

        internal async Task Bliz( CancellationToken Token )
        {

            DateTime StartTime = DateTime.UtcNow;

            try
            {
                while (!Token.IsCancellationRequested)
                {

                    if( DateTime.UtcNow - StartTime > BlizzardPlugin.Instance.Config.BlizzardDurationTime ) BlizzardPlugin.Instance.ctsBlz.Cancel();

                    foreach (Player player in Player.List)
                    {

                        if (player.IsDead) continue;

                        lock (BlizzardPlugin.Instance._dictLock)
                        {

                            bool IsBlizzarded = BlizzardPlugin.Instance.InBlizzard[player.Id];
                            DateTime LHeat = BlizzardPlugin.Instance.LastHeat[player.Id] ;

                            if (IsBlizzarded == false && InZone(player) == true)
                            {

                                SpawnFog(player);

                                IsBlizzarded = BlizzardPlugin.Instance.InBlizzard[player.Id] = true;
                            }
                            else if (IsBlizzarded == true && InZone(player) == false)
                            {

                                DisableBliz(player);

                                IsBlizzarded = BlizzardPlugin.Instance.InBlizzard[player.Id] = false;

                            }

                            if (IsBlizzarded && DateTime.UtcNow - LHeat >= BlizzardPlugin.Instance.Config.MedkitHeatTime) player.EnableEffect<Hypothermia>(BlizzardPlugin.Instance.Config.BlizStrongness, 9999f, false);
                            else if (IsBlizzarded && DateTime.UtcNow - LHeat < BlizzardPlugin.Instance.Config.MedkitHeatTime && player.TryGetEffect<Hypothermia>( out var garb ) ) player.DisableEffect<Hypothermia>();

                        }

                    }

                    await Task.Delay(500 ,Token );
                }
            }

            catch (OperationCanceledException)
            {

                BlizEnd();

                Log.Info($"[Blizzard] Blizzard effect has ended.");
            }

            catch (Exception e)
            {
                
                Log.Info($"[Blizzard] An error occurred in the blizzard effect handler: {e}");
            }

        }
        internal void SpawnFog( Player player )
        {

            player.EnableEffect<FogControl>();

            FogControl FogEffect = player.GetEffect<FogControl>();

            if (FogEffect == null)
            {

                Log.Warn($"Failed to get FogControl effect for player {player.Nickname}");

                return;

            }

            FogEffect.SetFogType(FogType.Scp244);

            FogEffect.Intensity = 255 ;

        }
        internal void DisableBliz( Player player )
        {

            player.DisableEffect<FogControl>();
            player.DisableEffect<Hypothermia>();


        }

        internal bool InZone( Player player)
        {

            float Ypos = player.Position.y;

            if ( Ypos >= BlizzardPlugin.Instance.Config.BlizzardZoneY )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void BlizEnd()
        {

            lock (BlizzardPlugin.Instance._dictLock)
            {

                foreach (var player in Player.List.ToList())
                {

                    if (BlizzardPlugin.Instance.InBlizzard[player.Id]) DisableBliz(player);

                    BlizzardPlugin.Instance.InBlizzard[player.Id] = false;
                    BlizzardPlugin.Instance.LastHeat[player.Id] = DateTime.MinValue;
                }

            }

        }

        
    }

}