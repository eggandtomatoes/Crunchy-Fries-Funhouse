using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Items.Firearms.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using MEC;
using autoturret;

namespace AutoTurretPlugin
{
    internal static class ShootingLogic
    {

        private static readonly TimeSpan _aimingRate = TimeSpan.FromMilliseconds(700);

        internal static async Task AimAndShootLoop(Npc turret, CancellationToken cancellationToken, Vector3 SpPosition)
        {
            Player target = null;

            Firearm Turretgun = turret.CurrentItem as Firearm;

            if( Turretgun == null )
            {
                Log.Info($"{turret.Nickname}的当前武器不是火器，任务终止");
                return;
            }

            DateTime LastShot = DateTime.MinValue;

            DateTime LastAim = DateTime.MinValue;

            try
            {

                while ((!cancellationToken.IsCancellationRequested) && turret != null && turret.IsAlive && turret.IsConnected)
                {

                    turret.Position = SpPosition;


                    if (DateTime.UtcNow - LastAim >= _aimingRate)
                    {

                        bool _ifTargetStillValid = (target != null && target.IsAlive && XRay.IsInLineOfSight(turret, target) && Functions.IfInDistance(turret, target));

                        if (!_ifTargetStillValid)
                        {
                            target = Functions.FindClosestPlayer(turret);
                        }

                        if (target != null) Functions.RefreshAIM(turret, target);

                        LastAim = DateTime.UtcNow;
                    }

                    await Task.Delay(50, cancellationToken);

                    Functions.WeponaryCheck(turret, Turretgun);

                    if (target != null )
                    {

                        Timing.RunCoroutine(Reflection.Npcshoot(turret));

                    }

                    await Task.Delay(50, cancellationToken);

                }
            }

            catch (OperationCanceledException)
            {
                Log.Debug($"{turret.Nickname}的任务已被停止");
            }
            finally
            {
                if (TurrletPlugin.turretTasks.ContainsKey(turret))
                {

                    TurrletPlugin.turretTasks.Remove(turret);

                }
            }

        }
    }
}
