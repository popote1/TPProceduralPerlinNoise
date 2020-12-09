using UnityEngine;

namespace Utils {
    public static class PerlinNoise {
    
        public static float[,] Cave(float[,] map, float modifier, bool edgesAreWalls,Vector2 offset) {
            for (int x = 0; x < map.GetLength(0); x++) {
                for (int y = 0; y < map.GetLength(1); y++) {
                    if (edgesAreWalls && (x == 0 || y == 0 || x == map.GetLength(0)-1  || y == map.GetLength(1)-1 )) {
                        map[x, y] = 1; //Keep the edges as walls
                    }
                    else {
                        //Generate a new point using Perlin noise, then round it to a value of either 0 or 1
                        map[x, y] = Mathf.PerlinNoise(x * modifier+offset.x
                            , y * modifier+offset.y);
                    }
                }
            }
            return map;
        }

    }
}
