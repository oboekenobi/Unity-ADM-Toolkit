//Author : https://github.com/seekeroftheball   https://gist.github.com/seekeroftheball
//Version : 1.2
//Updated : March 2023
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace NewFolderWizard
{
    /// <summary>
    /// Parses directory data and handles the actual creation of the folder hierarchies.
    /// </summary>
    public class CreateFolderHierarchy
    {
        public static string ParentFolder;

        /// <summary>
        /// Loads root directory then loops through, parsing children.
        /// </summary>
        /// 
        public static void CreateFolders(ProjectManager manager)
        {

            CreateParentFolder();

            DirectoryData rootDirectory = Resources.Load<DirectoryData>(FilePaths.ResourceFolderRelativePathToRootDirectory);

            //NewFoldersSetupWizard.SetAllCheckValues(true, rootDirectory, ParentFolder);

            ParseFolders(rootDirectory, ParentFolder);

            manager.currentProject = ParentFolder;

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ParentFolder + "/Scenes/" + NewFoldersSetupWizard.ExhibitCode + "_Scene_01.unity");

            Debug.Log("Folder Creation Successful!");
        }

        public static void CreateParentFolder()
        {
            AssetDatabase.CreateFolder("Assets", NewFoldersSetupWizard.ExhibitCode + "_UnityAssets");
            ParentFolder = "Assets/" + NewFoldersSetupWizard.ExhibitCode + "_UnityAssets";

            //DirectoryData rootDirectory = Resources.Load<DirectoryData>(FilePaths.ResourceFolderRelativePathToRootDirectory);
        }

        /// <summary>
        /// Loops through dictionary, creates individual folders then parses children.
        /// </summary>
        /// <param name="directory">Directory to parse.</param>
        /// <param name="parentPath">Parent of directory to parse.</param>
        private static void ParseFolders(DirectoryData directory, string parentPath)
        {
            foreach (var keyValuePair in directory.Folders)
            {                
                bool folderIsEnabled = keyValuePair.Value.IsEnabled;

                //if (!folderIsEnabled)
                    //continue;                

                string guid = AssetDatabase.CreateFolder(parentPath, keyValuePair.Key);
                string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);

                FolderProperties folderProperties = keyValuePair.Value;

                if (folderProperties.ChildDirectoryData && folderProperties.ChildDirectoryData != directory)
                    ParseFolders(folderProperties.ChildDirectoryData, newFolderPath);
            }
        }
    }
}
#endif