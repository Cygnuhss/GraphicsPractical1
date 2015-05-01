using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphicsPractical1
{
    // http://devmag.org.za/2009/04/25/perlin-noise/
    class PerlinNoise
    {
        static float[][] GenerateWhiteNoise(int width, int height)
        {
            Random random = new Random();
            float[][] noise = GetEmptyArray<float>(width, height);
 
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i][j] = (float)random.NextDouble() % 1;
                }
            }
 
            return noise;
        }

        public static T[][] GetEmptyArray<T>(int width, int height)
        {
            T[][] image = new T[width][];

            for (int i = 0; i < width; i++)
            {
                image[i] = new T[height];
            }

            return image;
        }


        static float[][] GenerateSmoothNoise(float[][] baseNoise, int octave)
        {
           int width = baseNoise.Length;
           int height = baseNoise[0].Length;
 
           float[][] smoothNoise = GetEmptyArray<float>(width, height);
 
           int samplePeriod = 1 << octave; // calculates 2 ^ k
           float sampleFrequency = 1.0f / samplePeriod;
 
           for (int i = 0; i < width; i++)
           {
              // Calculate the horizontal sampling indices.
              int sample_i0 = (i / samplePeriod) * samplePeriod;
              int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
              float horizontal_blend = (i - sample_i0) * sampleFrequency;
 
              for (int j = 0; j < height; j++)
              {
                 // Calculate the vertical sampling indices.
                 int sample_j0 = (j / samplePeriod) * samplePeriod;
                 int sample_j1 = (sample_j0 + samplePeriod) % height; //wrap around
                 float vertical_blend = (j - sample_j0) * sampleFrequency;
 
                 // Blend the two top corners
                 float top = Interpolate(baseNoise[sample_i0][sample_j0],
                    baseNoise[sample_i1][sample_j0], horizontal_blend);
 
                 // Blend the two bottom corners.
                 float bottom = Interpolate(baseNoise[sample_i0][sample_j1],
                    baseNoise[sample_i1][sample_j1], horizontal_blend);
 
                 // Final blending.
                 smoothNoise[i][j] = Interpolate(top, bottom, vertical_blend);
              }
           }
 
           return smoothNoise;
        }

        public static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        public static float[][] GeneratePerlinNoise(float[][] baseNoise, int octaveCount)
        {
            int width = baseNoise.Length;
            int height = baseNoise[0].Length;

            float[][][] smoothNoise = new float[octaveCount][][];

            float persistance = 0.6f;

            // Generate smooth noise.
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
            }

            float[][] perlinNoise = GetEmptyArray<float>(width, height);

            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            // Blend the noises.
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        perlinNoise[i][j] += smoothNoise[octave][i][j] * amplitude;
                    }
                }
            }

            // Normalisation of the noise.
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i][j] /= totalAmplitude;
                }
            }

            return perlinNoise;
        }

        public static float[][] GeneratePerlinNoise(int width, int height, int octaveCount)
        {
            float[][] baseNoise = GenerateWhiteNoise(width, height);

            return GeneratePerlinNoise(baseNoise, octaveCount);
        }

        // Convert the range from 0-1 float to 0-255 byte greyscale values.
        public static byte[][] MapToGrey(float[][] greyValues)
        {
            int width = greyValues.Length;
            int height = greyValues[0].Length;

            byte[][] image = GetEmptyArray<byte>(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    byte grey = (byte) (256 * greyValues[i][j]);
                    image[i][j] = grey;
                }
            }

            return image;
        }
    }
}
