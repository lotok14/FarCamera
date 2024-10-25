using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using FarCamera.Patches;
using System.Reflection;
using UnityEngine;
using BepInEx.Configuration;

namespace FarCamera
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class FarCameraMod : BasePlugin
    {
        public static ConfigEntry<float> LookAhead;
        public static ConfigEntry<float> CameraOffsetY;
        public static ConfigEntry<float> CameraOffsetZ;

        public override void Load()
        {
            LookAhead = Config.Bind("Direction", "Look ahead", 12f, "increase to make the camera look ahead more (recommended 20)");
            CameraOffsetY = Config.Bind("Offset", "Camera offset Y", 40f, "increase to make the camera go up (recommended 40)");
            CameraOffsetZ = Config.Bind("Offset", "Camera offset Z", -40f, "decrease to make the camera go back (recommended -60)");

            // Plugin startup logic
            Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded!");
            // patch with harmony
            startPatching();
            Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} finished patching");
        }

        private void startPatching()
        {
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            MethodInfo original;
            MethodInfo patch;

            try
            {
                if (FarCameraMod.CameraOffsetY.Value != 40f || FarCameraMod.CameraOffsetZ.Value != -40f || FarCameraMod.LookAhead.Value != 12f)
                {
                    // =====================================<FarCameraPatch>=====================================
                    // (FarCameraPatch) RaceCameraController.Awake() postfix
                    // change the offset and lookAhead value of the camera
                    original = AccessTools.Method(typeof(RaceCameraController), "Awake");
                    patch = AccessTools.Method(typeof(FarCameraPatch), "RaceCameraControllerAwakePostfix");
                    PatchMethod(harmony, original, patch, "postfix");
                }
                else
                {
                    Log.LogWarning("FarCameraPatch not patched because the config has not been changed");
                }
            }
            catch (Exception e)
            {
                Log.LogError($"stopped patching because of error:\n{e}");
            }
        }

        // patch a method with a specified patch
        private void PatchMethod(Harmony harmony, MethodInfo original, MethodInfo patch, string patchType)
        {
            string patchName = $"({patch.DeclaringType.Name}) {original.DeclaringType}.{original.Name}() {patchType}";
            try
            {
                switch (patchType)
                {
                    case "prefix":
                        harmony.Patch(original, prefix: new HarmonyMethod(patch));
                        break;
                    case "postfix":
                        harmony.Patch(original, postfix: new HarmonyMethod(patch));
                        break;
                    case "transpiler":
                        harmony.Patch(original, transpiler: new HarmonyMethod(patch));
                        break;
                    default:
                        throw new Exception($"no patch of type {patchType} exists");
                }
                Log.LogInfo($"{patchName} patched successfully");
            }
            catch (Exception e)
            {
                Log.LogError($"{patchName} not patched because of error:\n{e}");
            }
        }
    }
}
