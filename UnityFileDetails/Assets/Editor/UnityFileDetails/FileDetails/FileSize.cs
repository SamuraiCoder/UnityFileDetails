using UnityEditor;

namespace Editor.UnityFileDetails.FileDetails
{
    public class FileSize : IDetails
    {
        private bool _extensionEnabled;
        
        public void EnableDetailsExtension()
        {
            _extensionEnabled = true;
        }

        public string GetDetailExtension(string path)
        {
            var fileSizeBytes = GetFileSizeBytes(path);
            if (fileSizeBytes == long.MinValue)
            {
                return string.Empty;
            }
            
            return EditorUtility.FormatBytes(fileSizeBytes);
        }
        
        private long GetFileSizeBytes(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return long.MinValue;
            }
            
            return new System.IO.FileInfo(path).Length;
        }
        
        public bool IsExtensionEnabled() => _extensionEnabled;
        
        public UnityFileDetails.FileInfoType GetExtensionName() => UnityFileDetails.FileInfoType.FILE_SIZE;
    }
}
