using System.Reflection;
using System.Collections.Generic;
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

namespace autoturret
{
    internal class Reflection
    {

        internal static IEnumerator<float> Npcshoot(Npc turret)
        {

            if (turret == null || !turret.IsAlive || turret.CurrentItem == null)
            {
                Log.Info("炮塔已被摧毁或断开连接或其武器为空，无法射击");
                yield break;
            }

            if (turret.CurrentItem is Firearm firearm)
            {

                if (!(firearm.Base is AutosyncItem autosyncItem))
                {

                    yield break;
                }

                DummyKeyEmulator DKE = autosyncItem.DummyEmulator;


                if (DKE != null)
                {

                    DKE.AddEntry(ActionName.Shoot, false);

                    yield return Timing.WaitForSeconds(1f);

                    DKE.RemoveEntry(ActionName.Shoot);

                }

            }

        }

    }
}
