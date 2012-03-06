using System;
using Microsoft.Xna.Framework.Graphics;

namespace Osiris.Graphics.Terrain
{
	/// <summary>
	/// Summary description for Level.
	/// </summary>
	public class Level
	{
		#region Fields

		private readonly IndexBuffer[] _indexBuffers;
		
		#endregion

		#region Properties

		public int NeighboursCode { get; set; }

		#endregion

		#region Constructors

		#region Instance constructor

		internal Level(IndexBuffer[] indexBuffers)
		{
			_indexBuffers = indexBuffers;
		}

		#endregion

		#endregion

		#region Methods

		public void Draw(int numVertices)
		{
			IndexBuffer indexBuffer = _indexBuffers[NeighboursCode];
			indexBuffer.GraphicsDevice.Indices = indexBuffer;

			int primitiveCount = indexBuffer.IndexCount - 2;
			indexBuffer.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip,
				0,               // base vertex
				0,               // min vertex index
				numVertices,     // total num vertices - note that is NOT just vertices that are indexed, but all vertices
				0,               // start index
				primitiveCount); // primitive count
		}


		#endregion
	}
}
