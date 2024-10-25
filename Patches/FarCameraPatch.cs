using HarmonyLib;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using System;
using System.Reflection;

namespace FarCamera.Patches
{
    public class FarCameraPatch
    {
        public static void RaceCameraControllerAwakePostfix(RaceCameraController __instance)
        {
            typeof(RaceCameraController).GetProperty("lookAheadGroupDistanceMin").SetValue(__instance, FarCameraMod.LookAhead.Value);
            typeof(RaceCameraController).GetProperty("cameraNearOffset").SetValue(__instance, new Vector3(0, FarCameraMod.CameraOffsetY.Value, FarCameraMod.CameraOffsetZ.Value));
        }
    }

}