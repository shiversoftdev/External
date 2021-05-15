using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExternalTestingUtility.Cheats.BlackOps3.Constants;
using static System.ExCallThreadType;

namespace ExternalTestingUtility.Cheats
{
    internal static class BlackOps3
    {
        internal static class Constants
        {
            public const ulong OFF_CBUF_ADDTEXT = 0x20EC8B0;
            public const ulong OFF_DVAR_SETFROMSTRINGBYNAME = 0x22C7F60;
            public const ulong OFF_BG_UNLOCKABLESSETCLASSSETITEM = 0x26AE260;
            public const ulong OFF_LiveStats_Loadouts_GetCACRoot = 0x1EAF030;
            public const ulong OFF_BG_UnlockablesSetBubbleGumBuff = 0x26AE000;
        }


        private static ProcessEx __game;
        internal static ProcessEx Game
        {
            get
            {
                if (__game == null || __game.BaseProcess.HasExited)
                {
                    __game = "blackops3";
                    if (__game == null || __game.BaseProcess.HasExited)
                    {
                        throw new Exception("Black Ops 3 process not found.");
                    }
                    __game.SetDefaultCallType(XCTT_RIPHijack);
                }
                if (!__game.Handle)
                {
                    __game.OpenHandle(ProcessEx.PROCESS_ACCESS, true);
                }
                return __game;
            }
        }

        internal static void Cbuf_AddText(string serverText, int client = 0)
        {
            if (serverText == null) return;
            Game.Call<ulong>(Game[OFF_CBUF_ADDTEXT], client, serverText, 0L);
        }

        internal static void Dvar_SetFromStringByName(string dvarName, string _value, bool CreateIfMissing = true)
        {
            if (dvarName == null || _value == null) return;
            Game.Call<ulong>(Game[OFF_DVAR_SETFROMSTRINGBYNAME], dvarName, _value, CreateIfMissing);
        }

        internal static void ApplyHostDvars()
        {
            Dvar_SetFromStringByName("party_minPlayers", "1");
            Dvar_SetFromStringByName("lobbyDedicatedSearchSkip", "1");
            Dvar_SetFromStringByName("lobbySearchTeamSize", "1");
            Dvar_SetFromStringByName("lobbySearchSkip", "1");
            Dvar_SetFromStringByName("lobbyMergeDedicatedEnabled", "0");
            Dvar_SetFromStringByName("lobbyMergeEnabled", "0");
        }

        internal static bool BG_UnlockablesSetClassSetItem(ClassSetType classSetType, int classSetIndex, loadoutClass_t customClass, string slotName, int itemIndex, int controllerIndex = 0)
        {
            if (slotName == null) return false;
            var f = Game[OFF_BG_UNLOCKABLESSETCLASSSETITEM];
            return Game.Call<bool>(f, controllerIndex, classSetType, classSetIndex, customClass, slotName, itemIndex);
        }

        internal static void SetBubbleGumSlot(int packIndex, int buffIndex, int itemIndex)
        {
            LiveStats_Loadouts_GetCACRoot(out CACRoot cacroot, 0, CACType.CAC_TYPE_ZM_ONLINE);
            BG_UnlockablesSetBubbleGumBuff(cacroot, packIndex, buffIndex, itemIndex);
        }

        internal static void SetZMWeaponLoadout(int WeaponIndex, ZMLoadoutData LoadoutInfo)
        {
            for(int i = 0; i < 5; i++)
            {
                SetBubbleGumSlot(0, i, 252);
            }
        }

        internal static void BG_UnlockablesSetBubbleGumBuff(CACRoot cacRoot, int packIndex, int buffIndex, int itemIndex)
        {
            if (cacRoot.ddlContext == 0 || cacRoot.rootState == 0) return;
            var f = Game[OFF_BG_UnlockablesSetBubbleGumBuff];
            Game.Call<long>(f, cacRoot, packIndex, buffIndex, itemIndex);
        }

        internal static void LiveStats_Loadouts_GetCACRoot(out CACRoot outroot, uint controllerindex, CACType cacType)
        {
            var f = Game[OFF_LiveStats_Loadouts_GetCACRoot];
            object[] args = new object[] { new CACRoot(), controllerindex, cacType };
            Game.CallRef<bool>(f, ref args);
            outroot = (CACRoot)args[0];
        }

        #region typedef
        internal enum ClassSetType : int
        {
            CLASS_SET_TYPE_INVALID = -1,
            CLASS_SET_TYPE_MP_PUBLIC = 0x0,
            CLASS_SET_TYPE_MP_CUSTOM = 0x1,
            CLASS_SET_TYPE_MP_ARENA = 0x2,
            CLASS_SET_TYPE_COUNT = 0x3,
        };

        internal enum loadoutClass_t : int
        {
            CLASS_INVALID = -1,
            CUSTOM_CLASS_1 = 0x0,
            CUSTOM_CLASS_FIRST = 0x0,
            CUSTOM_CLASS_2 = 0x1,
            CUSTOM_CLASS_3 = 0x2,
            CUSTOM_CLASS_4 = 0x3,
            CUSTOM_CLASS_5 = 0x4,
            BASIC_CUSTOM_CLASS_COUNT = 0x5,
            CUSTOM_CLASS_6 = 0x5,
            FIELD_OPS_CLASS_FIRST = 0x5,
            CUSTOM_CLASS_7 = 0x6,
            CUSTOM_CLASS_8 = 0x7,
            CUSTOM_CLASS_9 = 0x8,
            FIELD_OPS_CLASS_LAST = 0x8,
            CUSTOM_CLASS_10 = 0x9,
            CUSTOM_CLASS_COUNT = 0xA,
            DEFAULT_CLASS_FIRST = 0xA,
            DEFAULT_CLASS_SMG = 0xA,
            DEFAULT_CLASS_CQB = 0xB,
            DEFAULT_CLASS_ASSAULT = 0xC,
            DEFAULT_CLASS_LMG = 0xD,
            DEFAULT_CLASS_SNIPER = 0xE,
            DEFAULT_CLASS_LAST = 0xE,
            TOTAL_CLASS_COUNT = 0xF,
        };

        internal enum CACType : int
        {
            CAC_TYPE_INVALID = -1,
            CAC_TYPE_MP_PUBLIC = 0x0,
            CAC_TYPE_MP_CUSTOM = 0x1,
            CAC_TYPE_MP_ARENA = 0x2,
            CAC_TYPE_MP_OFFLINE = 0x3,
            CAC_TYPE_FR = 0x4,
            CAC_TYPE_FR_OFFLINE = 0x5,
            CAC_TYPE_CP_ONLINE = 0x6,
            CAC_TYPE_CP_OFFLINE = 0x7,
            CAC_TYPE_ZM_ONLINE = 0x8,
            CAC_TYPE_ZM_OFFLINE = 0x9,
            CAC_TYPE_COUNT = 0xA,
        };

        internal struct CACRoot
        {
            public long ddlContext;
            public long rootState;
        };

        internal struct ZMLoadoutData
        {
            public int CamoIndex;
            public int Attachment1;
            public int Attachment2;
            public int Attachment3;
            public int Attachment4;
            public int Attachment5;
            public int Attachment6;
            public int Attachment7;
            public int Attachment8;
        }
        #endregion
    }
}
