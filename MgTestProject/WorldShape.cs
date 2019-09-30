using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MgTestProject
{
    public class WorldShape
    {
        public WorldShape(WorldShapeGeometry geometry, GraphicsDevice graphicsDevice)
        {
            Geometry = geometry;

            // Create texture
            Texture = new Texture2D(graphicsDevice, Geometry.Width, Geometry.Height);

            Color[] colorData = new Color[Geometry.Width * Geometry.Height];
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = geometry.DrawColor;
            }

            Texture.SetData(colorData);
        }

        public Texture2D Texture { get; private set; }
        public WorldShapeGeometry Geometry { get; private set; }
    }
}
