using UnityEngine;
using UnityEditor;

public class NoiseToolks : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Noise/AllTheNoise")]
    public static void CreateAllTheNoise()
    {
        int n = 128;
        {
            var noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise.SetFrequency(0.0181f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(2);
            noise.SetFractalLacunarity(2);
            noise.SetFractalGain(.5f);
            AssetDatabase.CreateAsset(GenerateNoiseTexture3D(noise, n), "Assets/Materials/Sky/WorleyEroderNoise.asset");
        }
        {
            // TODO funker ikke
            var noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            noise.SetFrequency(0.321f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(5);
            noise.SetFractalLacunarity(2);
            noise.SetFractalGain(.5f);
            noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
            noise.SetCellularJitter(1);
            noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);
            AssetDatabase.CreateAsset(GenerateNoiseTexture3D(noise, n), "Assets/Materials/Sky/WorleyNoise.asset");
        }
        {
            var noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFrequency(0.0409f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(5);
            noise.SetFractalLacunarity(2);
            noise.SetFractalGain(.5f);
            AssetDatabase.CreateAsset(GenerateNoiseTexture3D(noise, n), "Assets/Materials/Sky/PerlinNoise.asset");
        }
    }

    public static Texture3D GenerateNoiseTexture3D(FastNoiseLite noise, int n)
    {
        // TODO consider dropping 3-channel lol
        var texture = new Texture3D(n, n, n, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Repeat;
        var colors = new Color[n * n * n];
        for (int z = 0; z < n; z++)
        {
            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    var value = noise.GetNoise(x, y, z);
                    colors[x + y * n + z * n * n] = new Color(value, value, value);
                }
            }
        }
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
#endif
}
