using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Osiris.Graphics.Terrain
{
	/// <summary>
	/// Summary description for Map.
	/// </summary>
	public class TerrainComponent : DrawableGameComponent
	{
		#region Variables

		private readonly string _terrainModelAssetName;
		private ICameraService _camera;
		private TerrainModel _terrainModel;

		#endregion

		#region Properties

		public TerrainModel Model
		{
			get { return _terrainModel; }
		}

		#endregion

		#region Constructors

		public TerrainComponent(Game game, string terrainModelAssetName)
			: base(game)
		{
			_terrainModelAssetName = terrainModelAssetName;
		}

		#endregion

		#region Methods

		public override void Initialize()
		{
			_camera = (ICameraService) Game.Services.GetService(typeof (ICameraService));
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_terrainModel = Game.Content.Load<TerrainModel>(_terrainModelAssetName);

			base.LoadContent();
		}

		public override void Update(GameTime gameTime)
		{
			_terrainModel.Update(_camera.Position, _camera.Frustum);
		}

		public override void Draw(GameTime gameTime)
		{
			if (_terrainModel.Effect is IEffectMatrices)
			{
				IEffectMatrices effectMatrices = (IEffectMatrices) _terrainModel.Effect;
				effectMatrices.World = Matrix.Identity;
				effectMatrices.View = _camera.View;
				effectMatrices.Projection = _camera.Projection;
			}
			_terrainModel.Draw();
		}

		#endregion
	}
}
