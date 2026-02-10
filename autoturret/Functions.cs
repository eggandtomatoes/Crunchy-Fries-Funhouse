using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Exiled;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using InventorySystem.Items;
using NetworkManagerUtils.Dummies;
using Exiled.API.Features.Items.FirearmModules;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Autosync;
using MEC;

namespace AutoTurretPlugin
{
    internal static class Functions
    {
        internal static void RefreshAIM(Npc turret, Player target)
        {

            if (turret != null && target.IsAlive && target != null)
            {
                Vector3 DirctionToTarget = target.CameraTransform.position - turret.CameraTransform.position;

                if (DirctionToTarget.sqrMagnitude > 0.01f)
                {

                    Quaternion TargetRotation = Quaternion.LookRotation(DirctionToTarget);

                    turret.Rotation = TargetRotation;
                }

            }

            Log.Info($"炮塔{turret.Nickname}正在瞄准{target.Nickname}");

            return;
        }

        internal static bool IfInDistance(Player A, Player B) {

            Log.Info($"射程为{TurrletPlugin.Instance.Config.Range}");

            return (float)(A.Position - B.Position).sqrMagnitude <= TurrletPlugin.Instance.Config.Range; 
        }


        internal static Player FindClosestPlayer(Npc turret)
        {
            Player closestPlayer = null;

            float closestDistance = float.MaxValue;

            foreach (Player x in Player.List)
            {


                if (x != null && x.IsAlive && x.IsScp && !TurrletPlugin.turretTasks.ContainsKey(x) && IfInDistance(turret, x) && XRay.IsInLineOfSight(turret, x))
                {

                    float Xdistance = (turret.Position - x.Position).sqrMagnitude;

                    if (Xdistance < closestDistance)
                    {
                        closestDistance = Xdistance;
                        closestPlayer = x;
                    }

                }
            }

            return closestPlayer;
        }

        internal static bool WeponaryCheck(Npc turret, Firearm turretgun)
        {
            if (turretgun == null)
            {
                Log.Error($"{turret.Nickname}的当前武器不是火器，无法射击");

                return false;
            }
            if (turretgun.MagazineAmmo <= 0)
            {
                turret.Kill( DamageType.Unknown );

                Log.Info($"{turret.Nickname}的弹药耗尽，已自毁");
                return false;
            }
            return true;
        }

        

    }
}
