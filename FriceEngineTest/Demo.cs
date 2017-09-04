﻿using System;
using System.Windows.Forms;
using FriceEngine;
using FriceEngine.Animation;
using FriceEngine.Object;
using FriceEngine.Resource;
using FriceEngine.Utils.Graphics;
using FriceEngine.Utils.Misc;
using FriceEngine.Utils.Time;

namespace FriceEngineTest
{
	public class Demo : WpfGame
	{
		private const int H1 = 310;
		private const int H2 = -10;

		private readonly ImageObject[] _lo =
		{
			ImageObject.FromFile("lo1.png", 550, H1),
			ImageObject.FromFile("lo2.png", 550, H1),
			ImageObject.FromFile("lo3.png", 550, H1),
			ImageObject.FromFile("lo4.png", 550, H1),
			ImageObject.FromFile("lo5.png", 550, H1)
		};

		private readonly ImageObject[] _lou =
		{
			ImageObject.FromFile("lo1u.png", 550, H2),
			ImageObject.FromFile("lo2u.png", 550, H2),
			ImageObject.FromFile("lo3u.png", 550, H2),
			ImageObject.FromFile("lo4u.png", 550, H2),
			ImageObject.FromFile("lo5u.png", 550, H2)
		};

		private int _loLast, _louLast, _s;
		private FTimeListener _timer;
		private Action _lambda;
		private ImageObject _bird;
		private TextObject _score;

		public override void OnInit()
		{
			ShowFps = true;
			AddObject(ImageObject.FromFile("back.png", 0, 0));
			Height = 500;
			Width = 500;
			_bird = ImageObject.FromFile("an.png", 20, 300);
			_score = new TextObject(ColorResource.Black,
				"Click to jump.", 16, 10, 20);
			ResetGravity();
			_lambda = () =>
			{
				_bird.Y = 200;
				_bird.ClearAnims();
				ResetGravity();
				MessageBox.Show(@"GG!");
				_score.Text = "Restart!";
				_s = 0;
			};
			foreach (var o in _lo) _bird.TargetList.Add(new Pair<PhysicalObject, Action>(o, _lambda));
			foreach (var o in _lou) _bird.TargetList.Add(new Pair<PhysicalObject, Action>(o, _lambda));
			AddObject(_bird, _score);
			_timer = new FTimeListener(1700, () =>
			{
				_score.Text = "Score: " + _s++;
				_lou[_louLast].ClearAnims();
				_lo[_loLast].ClearAnims();
				_lou[_louLast].Y = H2;
				_lo[_loLast].Y = H1;
				_loLast = Random.Next(_lo.Length);
				_louLast = Random.Next(_lou.Length);
				var delta = Random.Next(50) - 50;
				_lou[_louLast].X = 550;
				_lo[_loLast].X = 550;
				_lou[_louLast].Y += delta;
				_lo[_loLast].Y += delta;
				_lou[_louLast].AddAnims(new SimpleMove(-400, 0));
				_lo[_loLast].AddAnims(new SimpleMove(-400, 0));
				AddObject(_lo[_loLast], _lou[_louLast]);
			}, true);
			AddTimeListener(_timer);
			base.OnInit();
		}

		public override void OnClick(double x, double y, int button)
		{
			_bird.ClearAnims();
			ResetGravity();
			base.OnClick(x, y, button);
		}

		public override void OnRefresh()
		{
			if (_bird.Y > Height + 50) _lambda.Invoke();
			else if (_bird.Y < 0)
			{
				_bird.Y = 0;
				_bird.ClearAnims();
				_bird.AddAnims(new AccelerateMove(0, 1800));
			}
			base.OnRefresh();
		}

		public override void OnLoseFocus() => GamePause();
		public override void OnFocus() => GameStart();
		private void ResetGravity() => _bird.AddAnims(new AccelerateMove(0, 1800), new SimpleMove(0, -500));
	}

	internal class Demo2 : WpfGame
	{
		private ShapeObject[] _objects;

		private double ShitShit => -Height;

		public override void OnClick(double x, double y, int button)
		{
			_objects.ForEach(o =>
			{
				if (o.ContainsPoint(x, y))
				{
					o.ColorResource = ColorResource.Gray;
//	                RemoveObject(o);
//					MessageBox.Show($@"x = {x}, y = {y}");
				}
			});
			base.OnClick(x, y, button);
		}

		public override void OnInit()
		{
			SetSize(500, 600);
			var t = new FTimer(1500);
		    _objects = new[]
		    {
		        new ShapeObject(ColorResource.Black, new FRectangle(Width / 4, Height / 6), 0, Height),
		        new ShapeObject(ColorResource.Black, new FRectangle(Width / 4, Height / 6), Width / 4, Height),
		        new ShapeObject(ColorResource.Black, new FRectangle(Width / 4, Height / 6), Width / 2, Height),
		        new ShapeObject(ColorResource.Black, new FRectangle(Width / 4, Height / 6), Width * 3 / 4, Height),
		        new ShapeObject(ColorResource.Black, new FRectangle(Width / 4, Height / 6), 0, Height),
		        new ShapeObject(ColorResource.Black, new FRectangle(Width / 4, Height / 6), Width / 4, Height),
		        new ShapeObject(ColorResource.Black, new FRectangle(Width / 4, Height / 6), Width / 2, Height),
		        new ShapeObject(ColorResource.Black, new FRectangle(Width / 4, Height / 6), Width * 3 / 4, Height)
		    };
		    _objects.ForEach(o =>
		    {
		        o.AddAnims(new SimpleMove(0, 250));
		        AddObject(o);
		    });
            t.Start(() =>
			{
				var a = Random.Next(_objects.Length);
				while (_objects[a].Y < Height) a = Random.Next(_objects.Length);
				_objects[a].ColorResource = ColorResource.Black;
				_objects[a].Y = ShitShit;
			});
		}

		public override void OnFocus() => GameStart();

		public override void OnLoseFocus() => GamePause();
	}

	public class Demo3 : WpfGame
	{
		private GravityMove _o;

		public override void OnInit()
		{
			SetSize(500, 600);
			_o = new GravityMove();
			var obj = new ShapeObject(ColorResource.DrakGray, new FCircle(50), 200, 200);
//			obj.AddAnims(new SimpleMove(10, 0));
			obj.AddAnims(_o);
			AddObject(obj);
		}

		public override void OnMouseMove(double x, double y)
		{
			_o.Centre.X = x;
			_o.Centre.Y = y;
		}

		internal class GravityMove : MoveAnim
		{
			public DoublePair Centre = new DoublePair(225, 225);
			public DoublePair Self = new DoublePair(200, 200);
			public DoublePair Speed = DoublePair.Empty();

			public override DoublePair Delta
			{
				get
				{
//					MessageBox.Show(@"Shit");
					Now = Clock.Current;
					var d = Math.Sqrt((Self.X - Centre.X) * (Self.X - Centre.X) +
					                  (Self.Y - Centre.Y) * (Self.Y - Centre.Y));
					Speed.X += (Now - Last)/1e7 * Math.Cos(d) *50;
					Speed.Y += (Now - Last)/1e7 * Math.Sin(d) *50;
					var ret = DoublePair.FromTicks((Now - Last) * Speed.X, (Now - Last) * Speed.Y);
					Last = Clock.Current;
					Self = Self + ret;
					return ret;
				}
			}
		}
	}
}