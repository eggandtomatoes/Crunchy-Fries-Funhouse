using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Exiled.API.Features;

using UnityEngine;


namespace AutoTurretPlugin
{
    public static class XRay
    {
        internal static bool IsInLineOfSight(Player turret, Player target)
        {

            if (turret == null || target == null) return false;


            Vector3 start = turret.CameraTransform.position;
            Vector3 end = target.CameraTransform.position;

            bool IsHit = false;

            RaycastHit Hit_Info;

            IsHit = Physics.Linecast(start, end, out Hit_Info, -1 );

            if( !IsHit ) return false;

            if (Hit_Info.collider.gameObject.layer == 2 || Hit_Info.collider.gameObject.layer == 13)
            {
                Log.Info("hit player directly");
                return true;

            }
            else
            {
                Log.Info($"hit something else: {Hit_Info.collider.gameObject.name} , layer: {Hit_Info.collider.gameObject.layer}");
                return false;

            }

        }

    }
}
