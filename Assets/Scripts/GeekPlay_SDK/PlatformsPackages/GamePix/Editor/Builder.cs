using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using ICSharpCode.SharpZipLib.Zip;

namespace GamePix.Editor
{
    public class Builder
    {
        private static string buildRoot = "html5";
        private static string buildDirectory = "Build";
        private static string applicationInfoJson =  "appinfo.json";
        private static string symbolsFileName = "MethodMap.tsv";
        private static string[] symbolsPaths = {
            Path.Combine("Library", "Bee", "artifacts", "WebGL", "il2cppOutput", "cpp", "Symbols", symbolsFileName),
            Path.Combine("Temp", "StagingArea", "Data", "il2cppOutput", "Symbols", symbolsFileName),
            Path.Combine("Temp", "EditorBuildOutput", symbolsFileName)
        };

#if UNITY_2021_2_OR_NEWER 
        private static string gpxBinaryFileName = "libunity-gpx.a";
#else
        private static string gpxBinaryFileName = "libunity-gpx.bc";
#endif

        private static string gpxBinaryPath = 
            Path.Combine("Assets", "Plugins", "GamePix", "impl", gpxBinaryFileName);
        private static string logsDirectory = "Logs";
        private static string editorLogFileName = "Editor.log";
        private static string projectSettingsFileName = "ProjectSettings.asset";
        
#if UNITY_2022_2_OR_NEWER
        private static string emccArgsPrefix = "Player";
        private static string emccArgsExtension =  ".dag.json" ;
        private static string emccArgsPath = Path.Combine("Library", "Bee");
#elif UNITY_2021_2_OR_NEWER
        private static string emccArgsPrefix = string.Empty;
        private static string emccArgsExtension = ".rsp";
        private static string emccArgsPath = Path.Combine("Library", "Bee", "artifacts", "rsp");
#else 
        private static string emccArgsPrefix = string.Empty;
        private static string emccArgsExtension = ".resp";
        private static string emccArgsPath = "Temp";
#endif

        private static string packMetadata = "gmpx.v3";
    
        public static void BuildArchive(string root, bool changedResources = true)
        {
            buildRoot = root;
            if (Directory.Exists(buildRoot))
            {
                Directory.Delete(buildRoot, true);
            }
            if (Build())
            {
                CopyGamepixBinary();
                SaveApplicationInfo(changedResources);
                CopyDebugSymbols();
                CopyLogs();
                Archive();
            }
        }

        public static void BuildGame(string root, CompressOptions compressOptions = CompressOptions.Uncompressed, bool changedResources = true)
        {
            buildRoot = root;
            if (Builder.Build(compressOptions))
            {
                Builder.CopyGamepixBinary();
                Builder.SaveApplicationInfo(changedResources);
            }
        }

        private static bool Build(CompressOptions compressOptions = CompressOptions.Uncompressed)
        {
            Debug.Log("Start build");
            DateTime buildStart = DateTime.Now;
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetBuildScenes().Where(s => s.enabled).Select(c => c.path).ToArray();
            buildPlayerOptions.locationPathName = buildRoot;
            Debug.Log(string.Format("Build directory: {0}", buildPlayerOptions.locationPathName));
            buildPlayerOptions.target = BuildTarget.WebGL;
            buildPlayerOptions.options = BuildOptions.AutoRunPlayer;
            
            switch (compressOptions)
            {
                case CompressOptions.StandardCompression:
                    buildPlayerOptions.options = buildPlayerOptions.options | BuildOptions.CompressWithLz4;
                    break;
                case CompressOptions.HighCompression:
                    buildPlayerOptions.options = buildPlayerOptions.options | BuildOptions.CompressWithLz4HC;
                    break;
                default:
                    buildPlayerOptions.options = buildPlayerOptions.options | BuildOptions.UncompressedAssetBundle;
                    break;
            }
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                var duration = DateTime.Now - buildStart;
                Debug.Log(String.Format("Build succeeded. Size: {0:0.00} Mb. Duration: {1:0.00} min.", 
                                        (double)summary.totalSize/1024/1024,
                                        duration.TotalMinutes));
                return true;
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
            return false;
        }

        public static EditorBuildSettingsScene[] GetBuildScenes()
        {
            return EditorBuildSettings.scenes;
        }

        public static void SaveBuildScene(EditorBuildSettingsScene scene)
        {
            var scenes = GetBuildScenes()
                                .Where(s => s.guid.Equals(scene.guid) || s.path.Equals(scene.path))
                                .Select(s => {
                                            s.enabled = scene.enabled; 
                                            return s;
                                        })
                                .ToArray();
            EditorBuildSettings.scenes = scenes;
        }

        private static void SaveApplicationInfo(bool changedResources)
        {
            var jsonFile = new FileInfo(Path.Combine(buildRoot, buildDirectory, applicationInfoJson));
            using (StreamWriter stream = jsonFile.CreateText())
            {
                ApplicationInfo.resourcesFormat = (changedResources) ? ResourcesFormat.Gpx : ResourcesFormat.Default;
                stream.Write(ApplicationInfo.ToJson());
            }
        }

        private static void CopyDebugSymbols()
        {
            string projectPath = Directory.GetCurrentDirectory();
            foreach (var symbolsPath in symbolsPaths) {
                var symbolsFile = new FileInfo(Path.Combine(projectPath, symbolsPath));
                if (symbolsFile.Exists)
                {
                    symbolsFile.CopyTo(Path.Combine(buildRoot, buildDirectory, symbolsFileName), true);
                    return;
                }
            }
            throw new Exception(String.Format("Debug symbols file {0} is not exist", symbolsFileName));
        }

        private static void CopyGamepixBinary()
        {
            string projectPath = Directory.GetCurrentDirectory();
            var binaryFile = new FileInfo(Path.Combine(projectPath, gpxBinaryPath));
            if (!binaryFile.Exists)
            {
                throw new Exception(String.Format("Gamepix binary file {0} is not exist", binaryFile.FullName));
            }
            binaryFile.CopyTo(Path.Combine(buildRoot, buildDirectory, gpxBinaryFileName), true);
        }
        
        private static void CopyLogs()
        {
  var logsPath = Path.Combine(buildRoot, logsDirectory);
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            var editorLogPaths = new FileInfo[]
            {
                new FileInfo(Path.Combine(Application.persistentDataPath, "..", "..", 
                                          editorLogFileName)),
                new FileInfo(Path.Combine(Application.persistentDataPath, "..", "..", "..", 
                                          "Local", "Unity", "Editor", editorLogFileName)),
                new FileInfo(Path.Combine(Application.persistentDataPath, "..", "..", "..", 
                                          "Logs", "Unity", "Editor", editorLogFileName)),
            };
            foreach (var path in editorLogPaths)
            {
                if (path.Exists)
                {
                    path.CopyTo(Path.Combine(logsPath, editorLogFileName), true);
                    break;
                }
            }

            var projectSettings = new FileInfo(Path.Combine("ProjectSettings", projectSettingsFileName));
            if (projectSettings.Exists)
            {
                projectSettings.CopyTo(Path.Combine(logsPath, projectSettingsFileName), true);
            }

            if (!Directory.Exists(emccArgsPath))
            {
                return;
            }

            var searchPattern = emccArgsPrefix + "*" + emccArgsExtension;
#if UNITY_2022_2_OR_NEWER
            CopyLatestFile(emccArgsPath, searchPattern, SearchOption.TopDirectoryOnly, logsPath);
#else
            CopyAllFiles(emccArgsPath, searchPattern, SearchOption.TopDirectoryOnly, logsPath);
#endif
        }
        
        private static void CopyLatestFile(string source, string searchPattern, SearchOption searchOption, string target)
        {
            var directory = new DirectoryInfo(source);
            var file = directory
               .GetFiles(searchPattern, searchOption)
               .OrderByDescending(item => item.LastWriteTime)
               .First();
            var targetFilePath = Path.Combine(target, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        private static void CopyAllFiles(string source, string searchPattern, SearchOption searchOption, string target)
        {
            foreach (var file in Directory.GetFiles(source, searchPattern, searchOption))
            {
                var targetFilePath = Path.Combine(target, Path.GetFileName(file));
                File.Copy(file, targetFilePath, true);
            }
        }

        private static void Archive()
        {
            Debug.Log("Archiving process started...");
            if (!Directory.Exists(buildRoot))
            {
                throw new Exception(String.Format("Directory with build {0} is not exist", buildRoot));
            }
            
            var archiveFileName = String.Format("{0}_{1}_{2}.gpx", 
                                        ApplicationInfo.companyName, 
                                        ApplicationInfo.productName, 
                                        PlayerSettings.bundleVersion);
            var archivePath = Path.Combine(buildRoot, archiveFileName);
            
            using (var archive = File.Create(archivePath))
            {
                using (var stream = new ZipOutputStream(archive))
                {
                    var zipEntry = new ZipEntry(archiveFileName);
                    stream.PutNextEntry(zipEntry);

                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.WriteString(packMetadata);

                        var rootLength = buildRoot.Length + 1;
                        var paths = Directory.EnumerateFiles(buildRoot, "*.*", SearchOption.AllDirectories);
                        foreach (var path in paths)
                        {
                            if (path == archivePath)
                            {
                                continue;
                            }
                            
                            var filename = path.Substring(rootLength);
                            writer.WriteString(filename);
                            
                            using (var reader = File.OpenRead(path))
                            {
                                byte[] bytes = new byte[reader.Length];
                                reader.Read(bytes, 0, bytes.Length);
                                writer.WriteBytes(bytes);
                            }
                        }
                    }
                }
            }
            
            Debug.LogFormat("Archive created successfully: {0}", archivePath);
        }
    }
}