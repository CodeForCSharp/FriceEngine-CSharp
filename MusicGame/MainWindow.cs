using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriceEngine;
using FriceEngine.Object;
using FriceEngine.Resource;
using FriceEngine.Utils.Message;
using FriceEngine.Utils.Time;

namespace MusicGame
{
    class MainWindow:WpfGame
    {
        public override void OnInit()
        {
            SetSize(600,600);
            Title = "Music Game";
            AddObject(new ImageObject(ImageResource.FromFile("Res/Img/BG.jpg"),0,0));
            AddTimeListener(new FTimeListener(3000, () => { }, true));
        }
        private FTimer _seconds = new FTimer(1000);
        private int _count = 2;
        public override void OnLastInit()
        {
            var r = new FDialog(this).Confirm("Are you player?", "Confirm",FDialogOptions.YesNo);
            switch (r)
            {
                case FDialogResults.No:
                    break;
                case FDialogResults.Yes:
                    break;
                    default:
                        break;
            }
        }
    }
}
