using SaverMaui.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaverMaui.ViewModels
{
    internal class ProfileViewModel : BaseViewModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public bool WeAreOnline { get; set; }

        public Color OnlineButtonBackgroundColor { get; set; } = new Color(255, 160, 122);

        public ProfileViewModel()
        {
            this.WeAreOnline = IsOnlineHelper.IsOnline;

            if (this.WeAreOnline) 
            {
                this.OnlineButtonBackgroundColor = new Color(124, 252, 0);
            }
        }
    }
}
