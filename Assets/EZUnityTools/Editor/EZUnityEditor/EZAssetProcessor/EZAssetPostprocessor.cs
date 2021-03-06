/*
 * Author:      熊哲
 * CreateTime:  7/17/2017 2:15:07 PM
 * Description:
 * 
*/
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EZUnityEditor
{
    public class EZAssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        string dirPath { get { return Path.GetDirectoryName(assetPath); } }
        string dirName { get { return dirPath.Substring(dirPath.LastIndexOf("/") + 1); } }
        string assetName { get { return Path.GetFileName(assetPath); } }

        void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            if (assetName.ToLower().StartsWith("sprite"))
            {
                textureImporter.mipmapEnabled = false;
                if (dirName.StartsWith("[") && dirName.EndsWith("]"))
                {
                    textureImporter.spritePackingTag = dirName.Substring(1, dirName.Length - 2);
                }
                // sprite_spriteName
                if (assetName.ToLower().StartsWith("sprite_"))
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                }
                // sprite@_spriteName
                else if (assetName.ToLower().StartsWith("sprite@_"))
                {
                    //这句会造成npot的设置被还原
                    //textureImporter.textureType = TextureImporterType.Sprite;
                }
            }
            if (assetName.ToLower().StartsWith("tex2D_"))
            {
                textureImporter.textureType = TextureImporterType.Default;
                textureImporter.mipmapEnabled = false;
            }
            // textureName_normalMap
            if (assetPath.ToLower().Contains("normalmap"))
            {
                textureImporter.textureType = TextureImporterType.NormalMap;
            }
            // textureName_bumpMap
            else if (assetPath.ToLower().Contains("bumpmap"))
            {
                textureImporter.textureType = TextureImporterType.NormalMap;
                textureImporter.convertToNormalmap = true;
            }
        }
        void OnPostprocessTexture(Texture2D texture)
        {

        }
        void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {

        }

        void OnPreprocessModel()
        {
            ModelImporter modelImporter = (ModelImporter)assetImporter;
            modelImporter.importMaterials = false;
            modelImporter.animationType = ModelImporterAnimationType.None;
            // modelName@animationName
            if (assetPath.Contains("@"))
            {
                modelImporter.importAnimation = true;
                modelImporter.animationType = ModelImporterAnimationType.Generic;
            }
            // modelName_collider
            if (assetPath.ToLower().Contains("collider"))
            {
                modelImporter.addCollider = true;
                modelImporter.animationType = ModelImporterAnimationType.None;
            }
        }
        void OnPostprocessModel(GameObject gameObject)
        {
            if (gameObject.name.ToLower().Contains("collider"))
            {
                gameObject.AddComponent<MeshCollider>();
                MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.enabled = false;
                }
            }
            foreach (Transform tr in gameObject.transform)
            {
                OnPostprocessModel(tr.gameObject);
            }
        }
        Material OnAssignMaterialModel(Material material, Renderer renderer)
        {
            if (material.name == "" // 未指定材质
                    || material.name == ("Mat") || material.name.StartsWith("Mat.") // C4D默认材质
                    || material.name.StartsWith("lambert")  // Maya默认材质
                    || material.name.EndsWith(" - Default") // 3dMax默认材质
            )
            {
                Debug.Log("Invalid Material Name: " + material.name);
                return EZAssetGenerator.GenerateMaterial();
            }
            return null;
        }

        void OnPostprocessAudio(AudioClip audioClip)
        {
            AudioImporter audioImporter = (AudioImporter)assetImporter;
            // audioName_mono
            if (assetPath.ToLower().Contains("_mono"))
            {
                audioImporter.forceToMono = true;
            }
        }
    }
}