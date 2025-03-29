using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.U2D.PSD;

namespace DominoGames.Editor
{
    // Latest updated on 2025.02.22.
    // Author JYS
    // Version 1.1.0

    // 스프라이트, PSD(PSB) import 시 자동으로 세팅 설정

    public class SpriteImportSettings : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            // Apply settings only if the texture is imported as a sprite
            if (importer.textureType == TextureImporterType.Sprite)
            {
                importer.spritePixelsPerUnit = 32;  // Set PPU to 32
                importer.filterMode = FilterMode.Point;  // Set to Point (No Filter)
                importer.textureCompression = TextureImporterCompression.Uncompressed;  // No compression
                importer.mipmapEnabled = false;  // Disable mipmaps for pixel art
            }
        }

        private void OnPreprocessAsset()
        {
            // 현재 가져오는 파일 경로
            string extension = Path.GetExtension(assetPath).ToLower();

            // PSB/PSD 파일인지 확인
            if (extension == ".psb")
            {
                Debug.Log("yeah");
                PSDImporter importer = (PSDImporter)assetImporter;

                importer.spritePixelsPerUnit = 32;  // Set PPU to 32
                importer.filterMode = FilterMode.Point;  // Set to Point (No Filter)
                importer.mipmapEnabled = false;  // Disable mipmaps for pixel art
            }
        }
    }
}