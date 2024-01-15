using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GamePix.Editor
{
    public enum ResourcesFormat
    {
        Default,
        Gpx
    }

    public static class ApplicationInfo
    {
        private static readonly string jsonTemplate = "\"{0}\": \"{1}\",";
        public static string pluginVersion => "5.11";
        public static string companyName => Application.companyName.ToSafeGpxString();
        public static string productName => Application.productName.ToSafeGpxString();
        public static string productVersion => Application.version;
        public static string unityVersion => Application.unityVersion;
        public static string operationSystem => Environment.OSVersion.ToString();
        public static string exceptionSupport => PlayerSettings.WebGL.exceptionSupport.ToString();
        private static int defaultWebScreenWidth => PlayerSettings.defaultWebScreenWidth;
        private static int defaultWebScreenHeight => PlayerSettings.defaultWebScreenHeight;
        public static ResourcesFormat resourcesFormat { get; set; }

        public static string ToJson()
        {
            var json = new StringBuilder("{");
            json.AppendFormat(jsonTemplate, "pluginVersion", pluginVersion);
            json.AppendFormat(jsonTemplate, "emscripten", Emscripten.editorVersion);
            json.AppendFormat(jsonTemplate, "companyName", companyName);
            json.AppendFormat(jsonTemplate, "productName", productName);
            json.AppendFormat(jsonTemplate, "productVersion", productVersion);
            json.AppendFormat(jsonTemplate, "unityVersion", unityVersion);
            json.AppendFormat(jsonTemplate, "operationSystem", operationSystem);
            json.AppendFormat(jsonTemplate, "exceptions",  exceptionSupport);
            var resourcesFormatName = Enum.GetName(typeof(ResourcesFormat), resourcesFormat);
            json.AppendFormat(jsonTemplate, "resourcesFormat", resourcesFormatName);
            json.AppendFormat(jsonTemplate, "width",  defaultWebScreenWidth);
            json.AppendFormat(jsonTemplate, "height",  defaultWebScreenHeight);
            json.Replace(',', '}', json.Length - 1, 1);
            return json.ToString();
        }
    }
}