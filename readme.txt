Graphics Practical 1
06-05-2015

---
Jelmer van Nuss - 4058925


Bonus assignments:

--- Terrain color based on height ---
The terrain is split in 5 categories. Each has a range of 1/5 of the height.
- Water (blue) is in the lower 1/5.
- Grass (green) is in the 2/5.
- Hills (brown) are in the 3/5.
- Mountains (gray) are in the 4/5.
- Snow (white) is in the upper 5/5.
The color range is rescaled to the minimum and maximum height. As a result,
there will always be an even spread of all the colors.

Things to try:
Terrain types can be added or removed, while the spread will still be even.
With k terrain types, every type will color 1/k part.

Methods involved:	(in the Terrain class)
calculateTerrainColor, calculateMinHeight, calculateMaxHeight, convertToHeightRange

--- Terrain generation with Perlin noise ---
The height of the terrain is based on random values between 0 and 255.
0 being lowest and 255 being highest.
These values are generated with Perlin noise.
First, an array with random 0-1 values is generated.
Second, for a certain amount of layers of smooth noise (octaves), a second
array will be generated. This array will take some of the values of the first
array, and interpolate the others. This results in a smoother noise.
Lastly, all the octaves are blended into one last array. Each of the octaves have
a weight (persistance) that will decrease with each successive octave. The values of
the octaves are added, and lastly normalized so that they fall in the range of 0-1.
This 0-1 range is now converted to a 0-255 byte range.

Things to try:
To disable, comment out the Perlin noise generation in Game1.LoadContent,
and enable the load from heightmap image.
For different kind of terrains, change the octave count or persistance.
The octave count can be changed in Game1.LoadContent.
A lower value will generate a 'jagged' terrain, a higher value will generate a smoother terrain.
Default: 8
The persistance can be changed in PerlinNoise.GeneratePerlinNoise.
A higher value will generate a 'jagged' terrain, a lower value will generate a smoother terrain.
Default: 0.6

Classes involved:
PerlinNoise, Game1 (LoadContent)

