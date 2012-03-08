using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Osiris.Graphics.Terrain
{
	/// <summary>
	/// Summary description for Patch.
	/// </summary>
	public class Patch : IComparable<Patch>
	{
		#region Variables

		private readonly VertexBuffer _vertexBuffer;
		private readonly Level[] _levels;
		private readonly Vector3 _center;

		private Patch _left, _right, _top, _bottom;

        private float _distanceFromCamera;

		#endregion

		#region Properties

		public int ActiveLevel { get; set; }
        public bool Visible { get; set; }

        public int Levels
        {
            get { return _levels.Length; }
        }

        public Vector3 Center
        {
            get
            {
                return _center;
            }
        }

		private bool LeftMoreDetailed
		{
			get { return (LeftActiveLevel < ActiveLevel); }
		}

		private bool RightMoreDetailed
		{
			get { return (RightActiveLevel < ActiveLevel); }
		}

		private bool TopMoreDetailed
		{
            get { return (TopActiveLevel < ActiveLevel); }
		}

		private bool BottomMoreDetailed
		{
			get { return (BottomActiveLevel < ActiveLevel); }
		}

		public int LeftActiveLevel
		{
			get {return (_left != null && _left.Visible) ? _left.ActiveLevel : int.MaxValue;}
		}

		public int RightActiveLevel
		{
            get { return (_right != null && _right.Visible) ? _right.ActiveLevel : int.MaxValue; }
		}

		public int TopActiveLevel
		{
            get { return (_top != null && _top.Visible) ? _top.ActiveLevel : int.MaxValue; }
		}

		public int BottomActiveLevel
		{
            get { return (_bottom != null && _bottom.Visible) ? _bottom.ActiveLevel : int.MaxValue; }
		}

		public Vector2 Offset { get; private set; }
		public BoundingBox BoundingBox { get; private set; }

		#endregion

		#region Constructors

		internal Patch(VertexBuffer vertexBuffer, Level[] levels, BoundingBox boundingBox, Vector3 center, Vector2 offset)
		{
			_vertexBuffer = vertexBuffer;
			_levels = levels;
			_center = center;
			Offset = offset;
			BoundingBox = boundingBox;
            Visible = true;
		}

		#endregion

		#region Methods

		internal void SetNeighbours(Patch pLeft, Patch pRight, Patch pTop, Patch pBottom)
		{
			_left = pLeft;
			_right = pRight;
			_top = pTop;
			_bottom = pBottom;
		}

		private static int GetNeighboursCode(bool bLeft, bool bRight, bool bTop, bool bBottom)
		{
			return ((bLeft) ? 1 : 0) | ((bRight) ? 2 : 0) | ((bTop) ? 4 : 0) | ((bBottom) ? 8 : 0);
		}

        public void UpdateDistanceFromCamera(Vector3 cameraPosition)
        {
            _distanceFromCamera = (_center - cameraPosition).LengthSquared();
        }

        public void UpdateTessellation()
		{
			// work out bitmask for neighbours
			int nCode = GetNeighboursCode(LeftMoreDetailed, RightMoreDetailed, TopMoreDetailed, BottomMoreDetailed);
			_levels[ActiveLevel].NeighboursCode = nCode;
		}

		public void Draw()
		{
			_vertexBuffer.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
			_levels[ActiveLevel].Draw(_vertexBuffer.VertexCount);
		}

		#endregion

        public int CompareTo(Patch other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(other, null)) return -1;
            return (_distanceFromCamera < other._distanceFromCamera) ? -1 : 1;
        }
    }
}
