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

            bool bChanged;
            do
            {
                bChanged = false;

                foreach (Patch pPatch in _visiblePatches)
                {
                    int nLevel = pPatch.ActiveLevel;
                    int nLeft = pPatch.LeftActiveLevel;
                    int nRight = pPatch.RightActiveLevel;
                    int nTop = pPatch.TopActiveLevel;
                    int nBottom = pPatch.BottomActiveLevel;

                    int nMinimumNeighbouringLevel = Math.Min(Math.Min(nLeft, nRight), Math.Min(nTop, nBottom));

                    if (nLevel > nMinimumNeighbouringLevel + 1)
                    {
                        pPatch.ActiveLevel = nMinimumNeighbouringLevel + 1;
                        bChanged = true;
                    }
                }
            }
            while (bChanged);
            
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