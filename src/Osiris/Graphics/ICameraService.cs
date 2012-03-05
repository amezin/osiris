using Microsoft.Xna.Framework;

namespace Osiris.Graphics
{
	public interface ICameraService
	{
		Matrix Projection { get; }
		Matrix View { get; }
        BoundingFrustum Frustum { get; }

		float ProjectionNear
		{
			get;
		}

		float ProjectionTop
		{
			get;
		}

		Vector3 Position
		{
			get;
		}
	}
}