﻿using Entoarox.FurnitureAnywhere;
using Harmony;
using StardewValley;
using Entoarox.Framework.Events;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.Locations;
using System.Reflection;
using System;

namespace CustomFurnitureAnywhere
{

    [HarmonyPatch(typeof(FurnitureAnywhereMod), "InitSpecialObject")]
    public class FurnitureAnywhereModFix
    {
        internal static bool Prefix(Item i)
        {
            if (i is CustomFurniture.CustomFurniture)
            {
                for (int c = 0; c < Game1.player.items.Count; c++)
                    if (Game1.player.items[c] != null && Game1.player.items[c].Equals(i))
                        Game1.player.items[c] = new AnywhereCustomFurniture(Game1.player.items[c] as CustomFurniture.CustomFurniture);

                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FurnitureAnywhereMod), "MoreEvents_ActiveItemChanged")]
    public class FurnitureAnywhereModFix2
    {
        internal static bool Prefix(object s, EventArgsActiveItemChanged e, FurnitureAnywhereMod __instance)
        {
 
                if (e.OldItem != null && e.OldItem is AnywhereCustomFurniture)
                {
                    CustomFurnitureAnywhereMod.modhelper.Reflection.GetPrivateMethod(__instance, "RestoreVanillaObjects").Invoke();
                    return false;
                }
            
            return true;
        }
    }

    [HarmonyPatch(typeof(FurnitureAnywhereMod), "RestoreVanillaObjects")]
    public class FurnitureAnywhereModFix3
    {
        internal static bool Prefix()
        {
            for (int c = 0; c < Game1.player.items.Count; c++)
                if (Game1.player.items[c] != null && Game1.player.items[c] is AnywhereCustomFurniture)
                {
                    Game1.player.items[c] = (Game1.player.items[c] as AnywhereCustomFurniture).Revert();
                    return false;
                }
            return true;
        }
    }

    [HarmonyPatch]
    public class FurnitureAnywhereModFix4
    {
        internal static MethodInfo TargetMethod()
        {
            if (Type.GetType("StardewValley.GameLocation, Stardew Valley") != null)
                return AccessTools.Method(Type.GetType("StardewValley.GameLocation, Stardew Valley"), "isCollidingPosition", new[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character) });
            else
                return AccessTools.Method(Type.GetType("StardewValley.GameLocation, StardewValley"), "isCollidingPosition", new[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character) });
        }

        internal static void Postfix(ref bool __result, GameLocation __instance, Rectangle position)
        {
            SerializableDictionary<Vector2, StardewValley.Object> objects = __instance.objects;

            if (__instance is DecoratableLocation)
                return;

            foreach (Vector2 k in objects.Keys)
                if (objects[k] is Furniture f && f.furniture_type != Furniture.rug && f.boundingBox.Intersects(position))
                {
                    __result = true;
                    return;
                }
        }
    }

    
    [HarmonyPatch]
    public class FurnitureAnywhereModFix5
    {
        internal static MethodInfo TargetMethod()
        {
            if (Type.GetType("StardewValley.Object, Stardew Valley") != null)
                return AccessTools.Method(Type.GetType("StardewValley.Object, Stardew Valley"), "clicked");
            else
                return AccessTools.Method(Type.GetType("StardewValley.Object, StardewValley"), "clicked");
        }

        internal static void Postfix(StardewValley.Object __instance, StardewValley.Farmer who, ref bool __result)
        {
            if (!(Game1.currentLocation is DecoratableLocation) && __instance is Furniture)
                __result = __instance.clicked(who);
        }
    }
}
    
