using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FriceEngine.Utils.Message
{
    public class FDialog
    {
        private readonly WpfGame _game;
        public FDialog(WpfGame game)
        {
            _game = game;
        }

        public void Show(string msg)
        {
            MessageBox.Show(_game.Window,msg);
        }

        public FDialogResults Confirm(string msg)
        {
            return Confirm(msg,"");
        }

        public FDialogResults Confirm(string msg,string title)
        {
            return Confirm(msg,title,FDialogOptions.YesNoCancel);
        }

        public FDialogResults Confirm(string msg,string title,FDialogOptions options)
        {
            Enum.TryParse(options.ToString(),out MessageBoxButton e);
            var result  = MessageBox.Show(_game.Window, msg, title,e);
            Enum.TryParse(result.ToString(), out FDialogResults r);
            return r;
        }
    }

    public enum FDialogOptions
    {
         YesNoCancel = MessageBoxButton.YesNoCancel,
         YesNo = MessageBoxButton.YesNo,
         Ok = MessageBoxButton.OK,
         OkCancel = MessageBoxButton.OKCancel
    }

    public enum FDialogResults
    {
        Ok = MessageBoxResult.OK,
        No = MessageBoxResult.No,
        None = MessageBoxResult.None,
        Cancel = MessageBoxResult.Cancel,
        Yes = MessageBoxResult.Yes
    }
}
