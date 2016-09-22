﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Frice_dotNet.Properties.FriceEngine.Object;
using Frice_dotNet.Properties.FriceEngine.Resource;
using Frice_dotNet.Properties.FriceEngine.Utils.Graphics;
using Frice_dotNet.Properties.FriceEngine.Utils.Message;
using Frice_dotNet.Properties.FriceEngine.Utils.Time;

namespace Frice_dotNet.Properties.FriceEngine
{
    public class AbstractGame : Form
    {
        internal readonly IList<IAbstractObject> Objects;
        internal readonly IList<IAbstractObject> ObjectAddBuffer;
        internal readonly IList<IAbstractObject> ObjectDeleteBuffer;

        internal readonly IList<FText> Texts;
        internal readonly IList<FText> TextAddBuffer;
        internal readonly IList<FText> TextDeleteBuffer;

        internal readonly IList<FTimeListener> FTimeListeners;
        internal readonly IList<FTimeListener> FTimeListenerAddBuffer;
        internal readonly IList<FTimeListener> FTimeListenerDeleteBuffer;

        internal AbstractGame()
        {
            SetBounds(100, 100, 500, 500);
            Objects = new List<IAbstractObject>();
            ObjectAddBuffer = new List<IAbstractObject>();
            ObjectDeleteBuffer = new List<IAbstractObject>();

            Texts = new List<FText>();
            TextAddBuffer = new List<FText>();
            TextDeleteBuffer = new List<FText>();

            FTimeListeners = new List<FTimeListener>();
            FTimeListenerAddBuffer = new List<FTimeListener>();
            FTimeListenerDeleteBuffer = new List<FTimeListener>();
            ShowDialog();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ProcessBuffer();
            foreach (var o in Objects) (o as FObject)?.HandleAnims();

            var g = e.Graphics;
            foreach (var o in Objects)
                if (o is ShapeObject)
                {
                    var brush = new SolidBrush((o as ShapeObject).ColorResource.Color);
                    if ((o as ShapeObject).Shape is FRectangle)
                        g.FillRectangle(brush,
                            (float) o.X,
                            (float) o.Y,
                            (float) (o as ShapeObject).Width,
                            (float) (o as ShapeObject).Height
                        );
                    else if ((o as ShapeObject).Shape is FOval)
                        g.FillEllipse(brush,
                            (float) o.X,
                            (float) o.Y,
                            (float) (o as ShapeObject).Width,
                            (float) (o as ShapeObject).Height
                        );
                }
                else if (o is ImageObject)
                {
                }
            foreach (var t in Texts)
            {
            }
            base.OnPaint(e);
        }

        private void ProcessBuffer()
        {
            foreach (var o in ObjectAddBuffer) Objects.Add(o);
            foreach (var o in ObjectDeleteBuffer) Objects.Remove(o);
            ObjectAddBuffer.Clear();
            ObjectDeleteBuffer.Clear();

            foreach (var t in TextAddBuffer) Texts.Add(t);
            foreach (var t in TextDeleteBuffer) Texts.Remove(t);
            TextAddBuffer.Clear();
            TextDeleteBuffer.Clear();

            foreach (var t in FTimeListenerAddBuffer) FTimeListeners.Add(t);
            foreach (var t in FTimeListenerDeleteBuffer) FTimeListeners.Remove(t);
            FTimeListenerAddBuffer.Clear();
            FTimeListenerDeleteBuffer.Clear();
        }
    }

    public class Game
    {
        public Game()
        {
            _timer = new FTimer(10);
            Form = new AbstractGame();
            Form.ShowDialog();
            // ReSharper disable VirtualMemberCallInConstructor
            OnInit();
            Run();
        }

        protected readonly AbstractGame Form;

        private readonly FTimer _timer;

//        private readonly Graphics _gameScene;
//        private readonly Bitmap _screenCut;

        public Point MousePosition() => Control.MousePosition;

        /// <summary>
        /// add an object or text to screen.
        /// </summary>
        /// <param name="o">the object or text to be added.</param>
        public void AddObject(IAbstractObject o)
        {
            if (o == null) return;
            if (o is FText) Form.TextAddBuffer.Add((FText) o);
            else Form.ObjectAddBuffer.Add(o);
        }

        /// <summary>
        /// remove an object or text from screen.
        /// </summary>
        /// <param name="o">the object or text to be removed.</param>
        public void RemoveObject(IAbstractObject o)
        {
            if (o == null) return;
            if (o is FText) Form.TextDeleteBuffer.Add((FText) o);
            else Form.ObjectDeleteBuffer.Add(o);
        }

        /// <summary>
        /// clear all objects and texts
        /// </summary>
        public void ClearObjects()
        {
            foreach (var o in Form.Objects) Form.ObjectDeleteBuffer.Add(o);
            foreach (var o in Form.Texts) Form.TextDeleteBuffer.Add(o);
        }

        /// <summary>
        /// add a timerListener
        /// </summary>
        /// <param name="t">the timeListener to be added.</param>
        public void AddTimeListener(FTimeListener t) => Form.FTimeListenerAddBuffer.Add(t);

        /// <summary>
        /// remove a timeListener
        /// </summary>
        /// <param name="t">the timeListener to be removed.</param>
        public void RemoveTimeListener(FTimeListener t) => Form.FTimeListenerDeleteBuffer.Add(t);

        /// <summary>
        /// clear the timeListeners.
        /// </summary>
        public void ClearTimeListeners()
        {
            foreach (var l in Form.FTimeListeners) Form.FTimeListenerDeleteBuffer.Add(l);
        }

        public virtual void OnInit()
        {
        }

        public virtual void OnRefresh()
        {
        }

        public virtual void OnClick()
        {
        }

        private int _counter;

        private void Run()
        {
            _counter = 0;
            while (true)
                if (_timer.Ended())
                {
                    ++_counter;
                    OnRefresh();
                    Form.Refresh();
                    FLog.Info("repaint" + _counter);
                }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}