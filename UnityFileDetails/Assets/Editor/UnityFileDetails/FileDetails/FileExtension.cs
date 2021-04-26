using System.Collections.Generic;
using System.IO;

namespace Editor.UnityFileDetails.FileDetails
{
    public class FileExtension : IDetails
    {
        private Dictionary<string, string> _extensionDict;
        private bool _extensionEnabled;
    
        public void EnableDetailsExtension()
        {
            _extensionDict = new Dictionary<string, string>
            {
                [".psd"] = "psd",
                [".png"] = "png",
                [".jpg"] = "tga",
                [".fbx"] = "fbx",
                [".cs"] = "cs",
                [".prefab"] = "prf",
            };

            _extensionEnabled = true;
        }

        public string GetDetailExtension(string path)
        {
            var extensionPath = Path.GetExtension(path);
            if (!_extensionDict.ContainsKey(extensionPath))
            {
                return string.Empty;
            }

            return _extensionDict[extensionPath];
        }

        public bool IsExtensionEnabled() => _extensionEnabled;
        
        public UnityFileDetails.FileInfoType GetExtensionName() => UnityFileDetails.FileInfoType.FILE_EXTENSION;
    }
}
