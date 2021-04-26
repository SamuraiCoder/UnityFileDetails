using System.Collections.Generic;
using System.Reflection;
using Editor.UnityFileDetails.FileDetails;
using UnityEditor;
using UnityEngine;

namespace Editor.UnityFileDetails
{
    [InitializeOnLoad]
    public class UnityFileDetails
    {
        private static Color defaultColor = new Color32(127, 127, 127, 160);
        private static Color highlightColor = new Color32(255, 255, 255, 255);
        
        public enum FileInfoType
        {
            FILE_SIZE,
            FILE_EXTENSION
        }
        
        private static string selectedGuid;
        private static List<IDetails> _fileDetailsList;
        private static Dictionary<FileInfoType, string> _fileDetailsListResult;
    
        static UnityFileDetails()
        {
            _fileDetailsList = new List<IDetails>();
            _fileDetailsListResult = new Dictionary<FileInfoType, string>();
            
            AddFileDetails(new FileExtension());
            AddFileDetails(new FileSize());
        
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowCallback;
        
            Selection.selectionChanged += () =>
            {
                if (Selection.activeObject != null)
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Selection.activeObject, out selectedGuid, out long id);
            };
        }

        private static void AddFileDetails(IDetails detail)
        {
            detail.EnableDetailsExtension();
            _fileDetailsList.Add(detail);
        }
    
        private static void OnProjectWindowCallback(string guid, Rect selectionRect)
        {
            if (Application.isPlaying || Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (IsViewProjectInTwoColumnsMode())
            {
                return;
            }
        
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                return;
            }

            //Iterate over all extensions for each file
            foreach (var detail in _fileDetailsList)
            {
                if (!detail.IsExtensionEnabled())
                {
                    continue;
                }

                _fileDetailsListResult[detail.GetExtensionName()] = detail.GetDetailExtension(assetPath);
            }

            var selected = string.CompareOrdinal(guid, selectedGuid) == 0;

            var extensionFileName = _fileDetailsListResult[FileInfoType.FILE_EXTENSION];
            var extensionFileSize = _fileDetailsListResult[FileInfoType.FILE_SIZE];

            CreateLabelFileAndSize(selectionRect, extensionFileName, extensionFileSize, selected);
        }

        private static void CreateLabelFileAndSize(Rect selectionRect, string extensionFileName, string extensionFileSize, bool selected)
        {
            var style = new GUIStyle(EditorStyles.label)
            {
                normal = {textColor = selected? highlightColor: defaultColor},
                fontSize = 10
            };

            //LabelField size
            var resultSizeStr = $"{extensionFileSize}";
            var resultTextSize = style.CalcSize(new GUIContent(resultSizeStr));
            selectionRect.x -= resultTextSize.x;
            var offsetRect = new Rect(selectionRect.position, selectionRect.size);
            EditorGUI.LabelField(offsetRect, resultSizeStr, style);
            
            //LabelField after
            //
            // var result1 = $"{extensionFileSize}";
            // var resultTextSize1 = style.CalcSize(new GUIContent(result1));
            // selectionRect.x -= resultTextSize1.x;
            // var offsetRect1 = new Rect(selectionRect.position, selectionRect.size);
            // EditorGUI.LabelField(offsetRect1, result1, style);
        }

        private static bool IsViewProjectInTwoColumnsMode()
        {
            var projectWindowType = Assembly.Load("UnityEditor").GetType("UnityEditor.ProjectBrowser");
            var projectWindow = EditorWindow.GetWindow(projectWindowType, false, "Project", false);
        
            var serializedObject = new SerializedObject(projectWindow);
            return serializedObject.FindProperty("m_ViewMode").enumValueIndex == 1;
        }
    }
}