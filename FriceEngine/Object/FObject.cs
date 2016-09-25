﻿using System.Collections.Generic;
using FriceEngine.Animation;
using FriceEngine.Resource;
using FriceEngine.Utils.Graphics;

namespace FriceEngine.Object
{
    public interface IAbstractObject
    {
        double X { get; set; }
        double Y { get; set; }

        double Rotate { get; set; }
    }

    public class AbstractObject : IAbstractObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Rotate { get; set; }

        public AbstractObject(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public interface IFContainer
    {
        double Width { get; set; }
        double Height { get; set; }
    }

    public interface ICollideBox
    {
        bool IsCollide(ICollideBox other);
    }

    public abstract class PhysicalObject : IAbstractObject, IFContainer
    {
        public virtual double X { get; set; }
        public virtual double Y { get; set; }

        public virtual double Width { get; set; }
        public virtual double Height { get; set; }

        public double Rotate { get; set; } = 0;

        public bool Died { get; set; } = false;

        private double _mass = 1;

        public double Mass
        {
            get { return _mass; }
            set { _mass = value <= 0 ? 0.001 : value; }
        }
    }

    public abstract class FObject : PhysicalObject
    {
        protected FObject()
        {
            MoveList = new List<MoveAnim>();
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public List<MoveAnim> MoveList { get; private set; }

        public void Move(double x, double y)
        {
            X += x;
            Y += y;
        }

        public void Move(DoublePair p) => Move(p.X, p.Y);

        public void HandleAnims()
        {
            foreach (var anim in MoveList) Move(anim.GetDelta());
        }

        public bool ContainsPoint(double px, double py) => px >= X && px <= X + Width && py >= Y && py <= Y + Height;
        public bool ContainsPoint(int px, int py) => px >= X && px <= X + Width && py >= Y && py <= Y + Height;
    }

    public sealed class ShapeObject : FObject
    {
        public IFShape Shape;
        public ColorResource ColorResource;

        public override double Width
        {
            get { return Shape.Width; }
            set { Shape.Width = value; }
        }

        public override double Height
        {
            get { return Shape.Height; }
            set { Shape.Height = value; }
        }

        public ShapeObject(ColorResource colorResource, IFShape shape, double x, double y) : base()
        {
            ColorResource = colorResource;
            Shape = shape;
            X = x;
            Y = y;
        }
    }

    public class ImageObject : FObject
    {
    }

    public class DoublePair
    {
        public double X;
        public double Y;

        public DoublePair(double y, double x)
        {
            Y = y;
            X = x;
        }

        public static DoublePair From1000(double x, double y) => new DoublePair(x/1000.0, y/1000.0);
    }
}