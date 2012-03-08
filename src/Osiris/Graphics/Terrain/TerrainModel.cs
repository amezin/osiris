using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Osiris.Graphics.Terrain
{
	public class TerrainModel
	{
		private readonly int _numPatchesX;
		private readonly int _numPatchesY;
        private readonly List<Patch> _patches, _visiblePatches;
        private int _numLevels;

		public HeightMap HeightMap { get; private set; }

		public Effect Effect { get; set; }

        public int[] MaxPatchesAtLevel { get; private set; }
        public int Levels
        {
            get { return _numLevels; }
        }

		internal TerrainModel(int numPatchesX, int numPatchesY, Patch[,] patches, HeightMap heightMap, Effect effect)
		{
			HeightMap = heightMap;
			_numPatchesX = numPatchesX;
			_numPatchesY = numPatchesY;
            _patches = new List<Patch>(_numPatchesX * _numPatchesY);
            _numLevels = 0;
            for (int x = 0; x < _numPatchesX; x++)
                for (int y = 0; y < _numPatchesY; y++)
                {
                    _patches.Add(patches[x, y]);
                    _numLevels = Math.Max(_numLevels, patches[x, y].Levels);
                }
            _visiblePatches = new List<Patch>(_patches.Count);
            MaxPatchesAtLevel = new int[_numLevels - 1];
            MaxPatchesAtLevel[0] = 4;
            for (int i = 1; i < _numLevels - 1; i++)
            {
                MaxPatchesAtLevel[i] = MaxPatchesAtLevel[i - 1] * 2;
            }

			Effect = effect;
		}

		public void Update(Vector3 cameraPosition, BoundingFrustum cameraFrustum)
        {
            // Frustum culling
            _visiblePatches.Clear();
            foreach (Patch p in _patches)
            {
                p.Visible = cameraFrustum.Intersects(p.BoundingBox);
                if (p.Visible)
                {
                    _visiblePatches.Add(p);
                    p.UpdateDistanceFromCamera(cameraPosition);
                }
                else
                {
                    p.ActiveLevel = 0;
                }
            }

            _visiblePatches.Sort();

            int minLevel = 0;
            int processedPatches = 0;
            foreach (Patch p in _visiblePatches)
            {
                while (minLevel < _numLevels - 1 && processedPatches >= MaxPatchesAtLevel[minLevel]) 
                {
                    minLevel++;
                }
                p.ActiveLevel = minLevel;
                processedPatches++;
            }

            bool changed;
            do
            {
                changed = false;
                foreach (Patch p in _visiblePatches)
                {
                    int maxActiveLevel = p.ActiveLevel;
                    if (p.TopActiveLevel != int.MaxValue) maxActiveLevel = Math.Max(maxActiveLevel, p.TopActiveLevel);
                    if (p.BottomActiveLevel != int.MaxValue) maxActiveLevel = Math.Max(maxActiveLevel, p.BottomActiveLevel);
                    if (p.LeftActiveLevel != int.MaxValue) maxActiveLevel = Math.Max(maxActiveLevel, p.LeftActiveLevel);
                    if (p.RightActiveLevel != int.MaxValue) maxActiveLevel = Math.Max(maxActiveLevel, p.RightActiveLevel);
                    if (p.ActiveLevel < maxActiveLevel - 1)
                    {
                        changed = true;
                        p.ActiveLevel = maxActiveLevel - 1;
                    }
                }
            } while (changed);
            
            foreach (Patch p in _visiblePatches)
            {
                p.UpdateTessellation();
            }
		}

        public void Draw()
		{
            // start effect rendering
            foreach (Patch p in _visiblePatches)
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    p.Draw();
			    }
		}
	}
}