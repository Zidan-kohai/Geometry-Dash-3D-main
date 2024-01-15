using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering;

namespace GamePix.Editor
{
    public partial class UnityConfigurator
    {
        private static int webGLMemorySize = 128;
        private static string platform = "WebGL";

        public static void SetWebGLSettings(bool safeMode = false, bool changeResources = true)
        {
            SetPlayerSettings(safeMode);
            SetEditorUserBuildSettings();
            SetWebGLMemorySize(webGLMemorySize);
            Emscripten.SetArguments();
            if (changeResources)
            {
                SetTexturesWebGLSettings();
                SetAtlasWebGLSettings();
                SetSoundsWebGLSettings();
            }
            DisableUnityAds();
        }

        public static void SetWebGLMemorySize(int memorySize)
        {
            var currentMemorySize = PlayerSettings.WebGL.memorySize;
            if (currentMemorySize > 0 && !memorySize.Equals(currentMemorySize))
            {
                Debug.Log(String.Format("WebGL memory size:\n{0}\nreplaced to:\n{1}", currentMemorySize, memorySize));
            }
            PlayerSettings.WebGL.memorySize = memorySize;

#if UNITY_2022_1_OR_NEWER     
            PlayerSettings.WebGL.initialMemorySize = memorySize;
            PlayerSettings.WebGL.memoryGrowthMode = WebGLMemoryGrowthMode.Geometric;
            PlayerSettings.WebGL.geometricMemoryGrowthStep = 0.2f;
            PlayerSettings.WebGL.memoryGeometricGrowthCap = 96;
            PlayerSettings.WebGL.maximumMemorySize = 2048;
            PlayerSettings.WebGL.powerPreference = WebGLPowerPreference.HighPerformance;
#endif
            Debug.Log(String.Format("Set WebGL memorySize: {0}.", PlayerSettings.WebGL.memorySize));
        }

        public static void SetPlayerSettings(bool safeMode)
        {
            // Resolution and Presentation
            PlayerSettings.runInBackground = true;
            PlayerSettings.WebGL.template = "APPLICATION:Minimal";

            // Splash Image
            PlayerSettings.SplashScreen.show = false;

            // Other settings
#if UNITY_2022
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
            PlayerSettings.SetGraphicsAPIs(
                BuildTarget.WebGL, 
                new [] {
                    GraphicsDeviceType.OpenGLES3,
                    GraphicsDeviceType.OpenGLES2 
            });
#else
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, true);
#endif

#if UNITY_2023_1_OR_NEWER
            PlayerSettings.SetIl2CppCompilerConfiguration(NamedBuildTarget.WebGL, Il2CppCompilerConfiguration.Master);
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.High);
#else
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.WebGL, Il2CppCompilerConfiguration.Master);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.High);
            
            // WebGL 1 support only gamma color space
            PlayerSettings.colorSpace = ColorSpace.Gamma;
#endif
           
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.stripUnusedMeshComponents = true;
            PlayerSettings.bakeCollisionMeshes = true;

#if UNITY_2020_1_OR_NEWER
            PlayerSettings.mipStripping = false;
#endif
            var settingsContent = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0];
            var serializedManager = new SerializedObject(settingsContent);
            serializedManager.FindProperty("submitAnalytics").boolValue = false;
            serializedManager.FindProperty("keepLoadedShadersAlive").boolValue = true;
            serializedManager.FindProperty("VertexChannelCompressionMask").intValue = -1;
            serializedManager.ApplyModifiedProperties();

            // WebGL
            PlayerSettings.WebGL.exceptionSupport = safeMode 
                ? WebGLExceptionSupport.FullWithoutStacktrace
                : WebGLExceptionSupport.None;
            PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
            PlayerSettings.WebGL.dataCaching = false;
#if UNITY_2021_2_OR_NEWER 
            PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.External;
#else
            PlayerSettings.WebGL.debugSymbols = true;
#endif
            PlayerSettings.WebGL.nameFilesAsHashes = false;
            PlayerSettings.WebGL.threadsSupport = false;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
#if UNITY_2019
            PlayerSettings.WebGL.wasmStreaming = false;
#endif
#if UNITY_2020_1_OR_NEWER
            PlayerSettings.WebGL.decompressionFallback = false;
#endif
            Debug.Log("Set player settings");
        }

        public static void SetEditorUserBuildSettings()
        {
#if UNITY_2021_2_OR_NEWER
            EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.Generic;
#endif
            EditorUserBuildSettings.development = false;
#if UNITY_2021_2 || UNITY_2021_3
            EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
#elif UNITY_2022_1_OR_NEWER
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSize);
#endif
#if UNITY_2020_2_OR_NEWER
            EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", "size");
#endif
#if UNITY_2023_1_OR_NEWER
            PlayerSettings.SetIl2CppStacktraceInformation(NamedBuildTarget.WebGL, Il2CppStacktraceInformation.MethodOnly);
#endif
            
            Debug.Log("Set editor user build settings");
        }
        
        private static void SetTexturesWebGLSettings() 
        {
#if UNITY_2021_2_OR_NEWER
            EditorUserBuildSettings.overrideTextureCompression = OverrideTextureCompression.ForceUncompressed;
            EditorUserBuildSettings.overrideMaxTextureSize = 2048;
            AssetDatabase.Refresh();
#endif
            var textureGuids = AssetDatabase.FindAssets("t:texture");
            foreach (var guid in textureGuids) 
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (textureImporter == null)
                {
                    continue;
                } 
                
                var platformTextureSettings = textureImporter.GetPlatformTextureSettings(platform);
                platformTextureSettings.overridden = true;
                platformTextureSettings.compressionQuality = 0;
                platformTextureSettings.format = textureImporter.textureType != TextureImporterType.SingleChannel
                    ? TextureImporterFormat.RGBA32
                    : TextureImporterFormat.Alpha8;

                textureImporter.SetPlatformTextureSettings(platformTextureSettings);
                textureImporter.SaveAndReimport();
            }
            Debug.Log("Set textures settings");
        }

        private static void SetAtlasWebGLSettings() 
        {
            var atlasGuids = AssetDatabase.FindAssets("t:spriteatlas");
            foreach (var guid in atlasGuids) 
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var atlas = AssetDatabase.LoadAssetAtPath(path,  typeof(SpriteAtlas)) as SpriteAtlas;
              
                if (atlas == null)
                {
                    continue;
                } 
                
                var platformAtlasSettings = atlas.GetPlatformSettings(platform);
                platformAtlasSettings.overridden = true;
                platformAtlasSettings.compressionQuality = 0;
                platformAtlasSettings.textureCompression = TextureImporterCompression.Uncompressed;
                platformAtlasSettings.format = TextureImporterFormat.RGBA32;
                
                atlas.SetPlatformSettings(platformAtlasSettings);
            }
            Debug.Log("Set sprite atlas settings");
        }

        private static void SetSoundsWebGLSettings() 
        {
            var guids = AssetDatabase.FindAssets("t:audioClip");
            foreach (var guid in guids) 
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
                if (audioImporter == null)
                {
                    continue;
                }

                var platformAudioSettings = audioImporter.GetOverrideSampleSettings(platform);
#if UNITY_2022_2_OR_NEWER
                platformAudioSettings.preloadAudioData = false;
#else
                audioImporter.preloadAudioData = false;
#endif
                platformAudioSettings.quality = 1;
                platformAudioSettings.compressionFormat = AudioCompressionFormat.AAC;
                platformAudioSettings.loadType = AudioClipLoadType.DecompressOnLoad;
                audioImporter.SetOverrideSampleSettings(platform, platformAudioSettings);
                audioImporter.SaveAndReimport();
            }
            Debug.Log("Set audio clips settings");
        }

        private static void DisableUnityAds() 
        {
            var unityConnectContent = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/UnityConnectSettings.asset")[0];
            var serializedManager = new SerializedObject(unityConnectContent);
            
            serializedManager.FindProperty("m_Enabled").boolValue = false;
            
            var crashReportingSettings = serializedManager.FindProperty("CrashReportingSettings");
            crashReportingSettings.FindPropertyRelative("m_Enabled").boolValue = false;
            
            var purchasingSettings = serializedManager.FindProperty("UnityPurchasingSettings");
            purchasingSettings.FindPropertyRelative("m_Enabled").boolValue = false;
            
            var analyticsSettings = serializedManager.FindProperty("UnityAnalyticsSettings");
            analyticsSettings.FindPropertyRelative("m_Enabled").boolValue = false;
            
            var adsSettings = serializedManager.FindProperty("UnityAdsSettings");
            adsSettings.FindPropertyRelative("m_Enabled").boolValue = false;
            
            var performanceReportingSettings = serializedManager.FindProperty("PerformanceReportingSettings");
            performanceReportingSettings.FindPropertyRelative("m_Enabled").boolValue = false;
            
            serializedManager.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
    }
}
