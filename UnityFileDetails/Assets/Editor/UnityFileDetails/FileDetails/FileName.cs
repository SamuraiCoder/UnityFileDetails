using System.IO;

namespace Editor.UnityFileDetails.FileDetails
{
    public class FileName : IDetails
    {
        private bool _extensionEnabled;
        
        public void EnableDetailsExtension()
        {
            _extensionEnabled = true;
        }

        public string GetDetailExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public bool IsExtensionEnabled() => _extensionEnabled;
        
        public UnityFileDetails.FileInfoType GetExtensionName() => UnityFileDetails.FileInfoType.FILE_NAME;
    }
}
