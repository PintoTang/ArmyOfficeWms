using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.Infrastructrue.UserCtrl.Model;
using MaterialDesignThemes.Wpf;

namespace CL.WCS.WPF
{
    public class MessageViewModel
    {
        public SnackbarMessageQueue MessageQueue { get { return SnackbarQueue.MessageQueue; } }

    }
}
