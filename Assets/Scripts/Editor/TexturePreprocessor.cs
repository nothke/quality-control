using UnityEngine;
using UnityEditor;

class TexturePreprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // if (assetPath.Contains("_bumpmap"))
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.filterMode = FilterMode.Point;
    }
}