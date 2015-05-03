using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsPractical1
{
    class Terrain
    {
        // The dimensions of the world.
        private int width;
        private int height;
        private int minHeight;
        private int maxHeight;

        private VertexPositionColorNormal[] vertices;
        private short[] indices;

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        // This constructor is used when a heightmap image is provided.
        public Terrain(HeightMap heightMap, float heightScale, GraphicsDevice device)
        {
            this.width = heightMap.Width;
            this.height = heightMap.Height;

            this.minHeight = CalculateMinHeight(heightMap);
            this.maxHeight = CalculateMaxHeight(heightMap);

            this.vertices = this.LoadVertices(heightMap, heightScale);
            this.SetupIndices();
            this.CalculateNormals();

            this.CopyToBuffers(device);
        }

        // This constructor is used when the heightmap will be generated on the go.
        public Terrain(int width, int height, int octaveCount, float heightScale, GraphicsDevice device)
        {
            this.width = width;
            this.height = height;

            float[][] tempMap = PerlinNoise.GeneratePerlinNoise(width, height, octaveCount);
            // Convert from 0-1 float values to 0-255 byte greyscale values.
            byte[][] byteMap = PerlinNoise.MapToGray(tempMap);

            // Convert from jagged array to 2D array.
            byte[,] byteMap2D = new byte[byteMap.Length, byteMap.Max(x => x.Length)];

            for (var i = 0; i < byteMap.Length; i++)
            {
                for (var j = 0; j < byteMap[i].Length; j++)
                {
                    byteMap2D[i, j] = (byteMap[i][j]);
                }
            }
            // Create a heightmap from the byte data.
            HeightMap heightMap = new HeightMap(byteMap2D);

            this.minHeight = CalculateMinHeight(heightMap);
            this.maxHeight = CalculateMaxHeight(heightMap);

            this.vertices = this.LoadVertices(heightMap, heightScale);
            this.SetupIndices();
            this.CalculateNormals();

            this.CopyToBuffers(device);
        }

        private VertexPositionColorNormal[] LoadVertices(HeightMap heightMap, float heightScale)
        {
            VertexPositionColorNormal[] vertices = new VertexPositionColorNormal[this.width * this.height];

            for (int x = 0; x < this.width; ++x)
                for (int y = 0; y < this.height; ++y)
                {
                    int v = x + y * this.width;
                    float h = heightMap[x, y] * heightScale;
                    vertices[v].Position = new Vector3(x, h, -y);
                    vertices[v].Color = this.CalculateTerrainColor(h, heightScale);
                }
            
            return vertices;
        }

        private int CalculateMinHeight(HeightMap heightMap)
        {
            // Begin with the highest height possible.
            int minHeight = 255;

            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    if (heightMap[i, j] < minHeight)
                        minHeight = heightMap[i, j];
                }
            }

            return minHeight;
        }

        private int CalculateMaxHeight(HeightMap heightMap)
        {
            // Begin with the lowest height possible.
            int maxHeight = 0;

            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    if ((int)heightMap[i, j] > maxHeight)
                        maxHeight = (int)heightMap[i, j];
                }
            }

            return maxHeight;
        }

        private Color CalculateTerrainColor(float oldHeight, float heightScale)
        {
            Color color;
            // The maximum height is rescaled to 255.
            int maxHeight = 255;
            int terrainTypes = 5;
            int height = this.ConvertToHeightRange((int)oldHeight, heightScale);

            // Give the terrain a different color, from top to bottom.
            // Snow on the mountains in the 5/5.
            color = Color.White;
            // The mountains in the 4/5.
            if (height < (maxHeight / ((float)terrainTypes / (terrainTypes - 1))))
                color = Color.Gray;
            // The hills in the 3/5.
            if (height < (maxHeight / ((float)terrainTypes / (terrainTypes - 2))))
                color = Color.Brown;
            // The grass in the 2/5.
            if (height < (maxHeight / ((float)terrainTypes / (terrainTypes - 3))))
                color = Color.Green;
            // The lakes and seas in the 1/5.
            if (height < (maxHeight / terrainTypes))
                color = Color.Blue;

            return color;
        }

        private int ConvertToHeightRange(int value, float heightScale)
        {
            // Spread out the old minimum-maximum range.
            int oldStart = (int) (this.minHeight * heightScale);
            int oldEnd = (int) (this.maxHeight * heightScale);
            int oldRange = oldEnd - oldStart;
            // Rescale to a 0 to 255 scale.
            int newStart = 0;
            int newEnd = 255;
            int newRange = newEnd - newStart;

            float scale = (float) newRange / oldRange;
            int converted = (int) (newStart + ((value - oldStart) * scale));

            return converted;
        }

        private void SetupIndices()
        {
            this.indices = new short[(this.width - 1) * (this.height - 1) * 6];
            
            int counter = 0;
            for (int x = 0; x < this.width - 1; x++)
                for (int y = 0; y < this.height - 1; y++)
                {
                    int lowerLeft = x + y * this.width;
                    int lowerRight = (x + 1) + y * this.width;
                    int topLeft = x + (y + 1) * this.width;
                    int topRight = (x + 1) + (y + 1) * this.width;
                    
                    this.indices[counter++] = (short)topLeft;
                    this.indices[counter++] = (short)lowerRight;
                    this.indices[counter++] = (short)lowerLeft;
                    
                    this.indices[counter++] = (short)topLeft;
                    this.indices[counter++] = (short)topRight;
                    this.indices[counter++] = (short)lowerRight;
                }
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < this.indices.Length / 3; i++)
            {
                short i1 = this.indices[i * 3];
                short i2 = this.indices[i * 3 + 1];
                short i3 = this.indices[i * 3 + 2];
                
                Vector3 side1 = this.vertices[i3].Position - this.vertices[i1].Position;
                Vector3 side2 = this.vertices[i2].Position - this.vertices[i1].Position;
                Vector3 normal = Vector3.Cross(side1, side2);
                normal.Normalize();
                
                this.vertices[i1].Normal += normal;
                this.vertices[i2].Normal += normal;
                this.vertices[i3].Normal += normal;
            }

            for (int i = 0; i < this.vertices.Length; i++)
                this.vertices[i].Normal.Normalize();
        }

        private void CopyToBuffers(GraphicsDevice device)
        {
            this.vertexBuffer = new VertexBuffer(device, VertexPositionColorNormal.VertexDeclaration,
                this.vertices.Length, BufferUsage.WriteOnly);
            this.vertexBuffer.SetData(this.vertices);

            this.indexBuffer = new IndexBuffer(device, typeof(short), this.indices.Length,
                BufferUsage.WriteOnly);
            this.indexBuffer.SetData(this.indices);

            device.Indices = this.indexBuffer;
            device.SetVertexBuffer(this.vertexBuffer);
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertices.Length, 0,
                this.indices.Length / 3);
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
