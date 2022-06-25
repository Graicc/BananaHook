using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace BananaHook.HookAndPatch
{
    public class OnPlayerTaggedByPlayerHook
    {
        static public void OnEvent(Player tagger, Player victim)
        {
            bool isTagging = Utils.Room.IsTagging();
            Events.OnPlayerTagPlayer?.SafeInvoke(null, new PlayerTaggedPlayerArgs
            {
                tagger = tagger,
                victim = victim,
                isTagging = isTagging
            });

            if (victim == PhotonNetwork.LocalPlayer)
            {
                Events.OnLocalPlayerTag?.SafeInvoke(null, new PlayerTaggedPlayerArgs
                {
                    tagger = tagger,
                    victim = victim,
                    isTagging = isTagging
                });
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPrefs))]
    [HarmonyPatch("SetString", MethodType.Normal)]
    public class OnNicknameChange
    {
        private static void Postfix(string key, string value)
        {
            if (key == "playerName")
            {
                Events.OnLocalNicknameChange?.SafeInvoke(null, new PlayerNicknameArgs
                {
                    oldNickName = PhotonNetwork.LocalPlayer.NickName,
                    newNickName = value
                });
            }
        }
    }

    [HarmonyPatch(typeof(GorillaNetworking.GorillaComputer))]
    [HarmonyPatch("InitializeNameState", MethodType.Normal)]
    public class OnComputerGotOurName
    {
        private static void Postfix(GorillaNetworking.GorillaComputer __instance)
        {
            Events.OnLocalNicknameChange?.SafeInvoke(null, new PlayerNicknameArgs
            {
                oldNickName = "gorilla",
                newNickName = __instance.currentName
            });
        }
    }

    /* Stuff for MasterClient is below */
    /* Stuff for MasterClient is below */
    /* Stuff for MasterClient is below */

    [HarmonyPatch(typeof(GorillaTagManager))]
    [HarmonyPatch("ReportTag", MethodType.Normal)]
    internal class OnPlayerTagged_MasterClient
    {
        internal static void Prefix(GorillaTagManager __instance, Player taggedPlayer, Player taggingPlayer)
        {
            if(__instance.photonView.IsMine && __instance.IsGameModeTag())
            {
                if(__instance.isCurrentlyTag)
                {
                    if (taggingPlayer == __instance.currentIt && taggingPlayer != taggedPlayer && Time.time > __instance.lastTag + __instance.tagCoolDown) goto TAGEVENT;
                }
                else if (__instance.currentInfected.Contains(taggingPlayer) && !__instance.currentInfected.Contains(taggedPlayer) && Time.time > __instance.lastTag + __instance.tagCoolDown) goto TAGEVENT;
            }
            return;
        TAGEVENT:
            try
            {
                OnPlayerTaggedByPlayerHook.OnEvent(taggingPlayer, taggedPlayer);
            }
            catch (Exception e) { BananaHook.Log("ReportTag Exception: " + e.Message + "\n" + e.StackTrace); }
        }
    }

    [HarmonyPatch(typeof(GorillaHuntManager))]
    [HarmonyPatch("ReportTag", MethodType.Normal)]
    internal class OnPlayerHunted_MasterClient
    {
        internal static void Prefix(GorillaHuntManager __instance, Player taggedPlayer, Player taggingPlayer)
        {
            if ((__instance.currentHunted.Contains(taggingPlayer) || !__instance.currentTarget.Contains(taggingPlayer)) && !__instance.currentHunted.Contains(taggedPlayer) && __instance.currentTarget.Contains(taggedPlayer))
            {
                goto TAGEVENT;
            }
            else
            {
                if (__instance.IsTargetOf(taggingPlayer, taggedPlayer)) goto TAGEVENT;
            }
            return;
        TAGEVENT:
            try
            {
                OnPlayerTaggedByPlayerHook.OnEvent(taggingPlayer, taggedPlayer);
            }
            catch (Exception e) { BananaHook.Log("ReportTag Exception: " + e.Message + "\n" + e.StackTrace); }
        }
    }
}