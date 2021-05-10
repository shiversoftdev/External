using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExternalTestingUtility.Cheats.BlackOps3.Constants;

namespace ExternalTestingUtility.Cheats
{
    public static class BlackOps3
    {
        public static class Constants
        {
            public const ulong OFF_CBUF_ADDTEXT = 0x20EC8B0;
        }


        private static ProcessEx __game;
        public static ProcessEx Game
        {
            get
            {
                if(__game == null || __game.BaseProcess.HasExited)
                {
                    __game = "blackops3";
                    if(__game == null || __game.BaseProcess.HasExited)
                    {
                        throw new Exception("Black Ops 3 process not found.");
                    }
                }
                if(!__game.Handle)
                {
                    __game.OpenHandle(ProcessEx.PROCESS_ACCESS, true);
                }
                return __game;
            }
        }

        public static void Cbuf_AddText(string serverText, int client = 0)
        {
            if (serverText == null) return;
            Game.Call<ulong>(Game[OFF_CBUF_ADDTEXT], client, serverText, 0L);
        }
    }
}
