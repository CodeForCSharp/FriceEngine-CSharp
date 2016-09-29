﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FriceEngine.Object;
using FriceEngine.Utils.Graphics;
using FriceEngine.Utils.Time;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace FriceEngine
{
	/// <summary>
	/// A abstract Game Class based on WPF.
	/// ifdog同学加油喔，不是很懂WPF的API所以帮不上什么忙啦。。。只能根据Google和栈溢出加一些自己能加的东西。		——ice1000
	/// 对，我现在也在面向Stackoverflow编程。 -ifdog
	/// </summary>
	///<author>ifdog</author>
	public class WpfGame
	{
		private readonly WpfWindow _window;
		private readonly List<IAbstractObject> _buffer = new List<IAbstractObject>();
		public bool ShowFps { get; set; } = true;
		protected WpfGame()
		{
			_window = new WpfWindow(ShowFps)
			{
				OnClickAction = OnClick,
				CustomDrawAction = CustomDraw
			};
			OnInit();
			Run();
			new Application().Run(_window);
		}

		private void Run()
		{
			CompositionTarget.Rendering += (sender, e) =>
			{
				OnRefresh();
				_window.Update(_buffer);
				Thread.Sleep(2);//据观察基本不影响FPS，但能稍微降低CPU占用。
			};
		}

		public virtual void OnInit()
		{
		}

		public virtual void OnRefresh()
		{
		}

		public virtual void OnExit()
		{
		}

		public virtual void CustomDraw(Canvas canvas)
		{
		}

		public virtual void OnClick(MouseButtonEventArgs e)
		{
		}

		public void AddObject(IAbstractObject obj)
		{
			_buffer.Add(obj);
		}
	}

	public class WpfWindow : Window
	{
		private readonly Canvas _canvas = new Canvas();
		private readonly Dictionary<int, FrameworkElement> _objectsDict = new Dictionary<int, FrameworkElement>();

		public Action<MouseButtonEventArgs> OnClickAction;
		public Action<Canvas> CustomDrawAction;
		private TextBlock _fpsTextBlock;
		private int _fps;
		private bool _showFps;

		public WpfWindow(bool showFps = true)
		{
			_showFps = showFps;
			Content = _canvas;
			if (_showFps)
			{
				_fpsTextBlock = new TextBlock()
				{
					Foreground = Brushes.Red,
					Background = Brushes.White,
				};
				_fpsTextBlock.SetValue(Canvas.LeftProperty, 10.0);
				_fpsTextBlock.SetValue(Canvas.RightProperty, 10.0);
				_canvas.Children.Add(_fpsTextBlock);
				new FTimer2(1000).Start(() =>
				{
					this.Dispatcher.Invoke(() =>
					{
						_fpsTextBlock.Text = $"FPS:{_fps}";
					});
					_fps = 0;
				});
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			OnClickAction?.Invoke(e);
			base.OnMouseDown(e);
		}

		public void Update(List<IAbstractObject> objects)
		{
			_objectsDict.Keys.Where(o =>
						!objects.Select(i => i.Uid).Contains(o)
			).ToList().ForEach(_onRemove);

			objects.ForEach(o =>
			{
				if (_objectsDict.ContainsKey(o.Uid))
				{
					_onChange(o);
				}
				else
				{
					_onAdd(o);
				}
			});

			CustomDrawAction?.Invoke(_canvas);
			if (_showFps)
			{
				_fps++;
			}
		}

		private void _onRemove(int uid)
		{
			_objectsDict.Remove(uid);
			_canvas.Children.Remove(_objectsDict[uid]);
		}

		private void _onAdd(IAbstractObject obj)
		{
			if (obj is ShapeObject)
			{
				var c = ((ShapeObject) obj).ColorResource.Color;
				if (((ShapeObject) obj).Shape is FRectangle)
				{
					var rect = new Rectangle
					{
						Fill = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B)),
						Width = (float) ((ShapeObject) obj).Width,
						Height = (float) ((ShapeObject) obj).Height
					};
					rect.SetValue(Canvas.LeftProperty, obj.X);
					rect.SetValue(Canvas.TopProperty, obj.Y);
					_objectsDict.Add(obj.Uid, rect);
					_canvas.Children.Add(rect);
				}
				else if (((ShapeObject) obj).Shape is FOval)
				{
					var epse = new Ellipse
					{
						Fill = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B)),
						Width = (float) ((ShapeObject) obj).Width,
						Height = (float) ((ShapeObject) obj).Height
					};
					epse.SetValue(Canvas.LeftProperty, obj.X);
					epse.SetValue(Canvas.TopProperty, obj.Y);

					_objectsDict.Add(obj.Uid, epse);
					_canvas.Children.Add(epse);
				}
			}
			else if (obj is ImageObject)
			{
				var bmp = (ImageObject) obj;
				Image img;
				using (var ms = new MemoryStream())
				{
					bmp.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					var bImage = new BitmapImage();
					bImage.BeginInit();
					bImage.StreamSource = new MemoryStream(ms.ToArray());
					bImage.EndInit();
					bmp.Bitmap.Dispose();
					img = new Image {Source = bImage};
				}
				img.SetValue(Canvas.LeftProperty, obj.X);
				img.SetValue(Canvas.TopProperty, obj.Y);
				_objectsDict.Add(obj.Uid, img);
				_canvas.Children.Add(img);
			}
			else if (obj is SimpleText)
			{
				var o = (SimpleText) obj;
				var b = new TextBlock
				{
					Foreground = new SolidColorBrush(ColorUtils.ToMediaColor(o.Color)),
					Text = o.Text
				};
				b.SetValue(Canvas.LeftProperty, o.X);
				b.SetValue(Canvas.RightProperty, o.Y);
				_canvas.Children.Add(b);
			}
		}

		private void _onChange(IAbstractObject o)
		{
			var element = _objectsDict[o.Uid];
			(o as FObject)?.RunAnims();
			element.SetValue(Canvas.LeftProperty, o.X);
			element.SetValue(Canvas.TopProperty, o.Y);
		}
	}
}
