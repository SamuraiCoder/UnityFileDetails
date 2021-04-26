namespace Editor.UnityFileDetails.FileDetails
{
    public interface IDetails
    {
        void EnableDetailsExtension();
        string GetDetailExtension(string path);
        bool IsExtensionEnabled();
        UnityFileDetails.FileInfoType GetExtensionName();
    }
}
