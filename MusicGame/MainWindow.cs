using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriceEngine;
using FriceEngine.Object;
using FriceEngine.Resource;
using FriceEngine.Utils.Data;
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
            AddObject(new ImageObject(ImageResource.FromPath(@".\Res\Img\BG.jpg"),0,0));
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
                    var json = new JsonPreference(@"./data.json");
                    AddKeyListener(pressed: key =>
                    {
                        switch (key)
                        {
                            case Key.A:
                            case Key.S:
                            case Key.D:
                            case Key.F:
                            case Key.J:
                            case Key.L:
                            case Key.K:
                                json.Insert($"{Clock.Current-3000}",$"{(int)key}");
                                break;
                        }
                        
                    });
                    break;
                case FDialogResults.Yes:
                    break;
                    default:
                        break;
            }
        }
    }
}
