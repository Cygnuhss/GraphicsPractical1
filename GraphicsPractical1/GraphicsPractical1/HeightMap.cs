using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsPractical1
{
    class HeightMap
    {
        // The dimensions of the world.
        private int width;
        private int height;

        // The heightmap is represented by an array of values ranging from 0 to 255,
        // where 0 is lowest and 255 is highest.
        private byte[,] heightData;

        public HeightMap(Texture2D heightMap)
        {
            this.width = heightMap.Width;
            this.height = heightMap.Height;

            this.LoadHeightData(heightMap);
        }

        public HeightMap(byte[,] heightMap)
        {
            this.width = heightMap.GetLength(0);
            this.height = heightMap.GetLength(1);

            this.heightData = heightMap;
        }

        private void LoadHeightData(Texture2D heightMap)
        {
            this.heightData = new byte[this.width, this.height];

            Color[] colorData = new Color[this.width * this.height];
            heightMap.GetData(colorData);

            for (int x = 0; x < this.width; x++)
                for (int y = 0; y < this.height; y++)
                    // The red value is used arbitrarily. In grayscale, it is equal to the blue and green values.
                    this.heightData[x, y] = colorData[x + y * this.width].R;
        }

        public byte this[int x, int y]
        {
            get { return this.heightData[x, y]; }
            set { this.heightData[x, y] = value; }
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }
    }
}
