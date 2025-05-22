using UnityEngine;
using UnityEditor;

public class PNGImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.EndsWith(".png"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 32; // ajustar para coincidir con nuestro juego
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
        }
    }
}
