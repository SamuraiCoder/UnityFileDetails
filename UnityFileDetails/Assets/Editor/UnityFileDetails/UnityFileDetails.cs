using System;
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
        private static float iconSeparator = 25f;
        
        public enum FileInfoType
        {
            FILE_SIZE,
            FILE_EXTENSION,
            FILE_NAME
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
            AddFileDetails(new FileName());
        
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

            foreach (var fileDetailsList in _fileDetailsListResult)
            {
                switch (fileDetailsList.Key)
                {
                    case FileInfoType.FILE_SIZE:
                    {
                        var fileSizeStr = _fileDetailsListResult[FileInfoType.FILE_SIZE];
                        CreateLabelFileSize(selectionRect, fileSizeStr, selected);
                        break;
                    }
                    case FileInfoType.FILE_EXTENSION:
                    {
                        var fileExtensionStr = _fileDetailsListResult[FileInfoType.FILE_EXTENSION];
                        var fileNameStr = _fileDetailsListResult[FileInfoType.FILE_NAME];
                        CreateLabelFileExtension(selectionRect, fileNameStr, fileExtensionStr, selected);
                        break;
                    }
                    case FileInfoType.FILE_NAME:
                    {
                        //Nothing
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(string.Empty, "Argument not in the file list result!");
                }
            }
        }

        private static void CreateLabelFileSize(Rect selectionRect, string fileSizeString, bool selected)
        {
            var style = new GUIStyle(EditorStyles.label)
            {
                normal = {textColor = selected? highlightColor: defaultColor},
                fontSize = 10
            };
            
            var resultSizeStr = $"{fileSizeString}";
            var resultTextSize = style.CalcSize(new GUIContent(resultSizeStr));
            selectionRect.x -= resultTextSize.x;
            var offsetRect = new Rect(selectionRect.position, selectionRect.size);
            EditorGUI.LabelField(offsetRect, resultSizeStr, style);
        }
        
        private static void CreateLabelFileExtension(Rect selectionRect, string fileNameString, string fileExtensionStr, bool selected)
        {
            var style = new GUIStyle(EditorStyles.label)
            {
                normal = {textColor = selected? highlightColor: defaultColor},
                fontSize = 11
            };
            
            //Note: we calculate how much is the size of the current element + the icon offset. 
            var resultNameStr = $"{fileNameString}";
            var resultTextSize = style.CalcSize(new GUIContent(resultNameStr));
            selectionRect.x += iconSeparator + resultTextSize.x;
            var offsetRect = new Rect(selectionRect.position, selectionRect.size);
            EditorGUI.LabelField(offsetRect, fileExtensionStr, style);
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