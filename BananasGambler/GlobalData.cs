﻿using BananasGambler.DTO;

namespace BananasGambler
{
    internal static class GlobalData
    {
        internal static LoginResponseDto UserData { get; set; } = new LoginResponseDto() { Login = "", Password = "" };
        internal static bool GameStarted { get; set; } = false;
        internal static int GameBid { get; set; } = 0;

        internal static bool IsPass = false;
    }
}
