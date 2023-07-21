//Author : https://github.com/seekeroftheball   https://gist.github.com/seekeroftheball
//Version : 1.2
//Updated : March 2023

using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace NewFolderWizard
{
    /// <summary>
    /// Editor window data for New Folder Wizard
    /// </summary>
    public class NewFolderWizardSetupWindow : EditorWindow
    {
        /// <summary>
        /// Properties defining the scale of the editor window.
        /// </summary>
        private struct WindowBounds
        {
            public const float WindowWidth = 300;
            public const float WindowHeight = 350;

            public static Vector2 WindowSize = new(WindowWidth, WindowHeight);

            public static Vector2 ScrollPosition;
            public const float ScrollViewHeight = 200;
        }

        string ExhibitCode;

        private static bool SelectAll = true;

        /// <summary>
        /// Constructor to set the window scale to the WindowBounds properties
        /// </summary>
        private NewFolderWizardSetupWindow()
        {
            minSize = WindowBounds.WindowSize;
            maxSize = WindowBounds.WindowSize;
        }

        /// <summary>
        /// Load the root directory when the editor window is opened.
        /// </summary>
        private void Awake()
        {
            NewFoldersSetupWizard.LoadDirectories(); 
        }

        static void Init()
        {
            //Get the open window
            //NewFolderWizardSetupWindow window = (NewFolderWizardSetupWindow)EditorWindow.GetWindow(typeof(NewFolderWizardSetupWindow));
        }

        // Draw the editor window
        private void OnGUI() => DrawWindow();
        private void OnInspectorUpdate() => Repaint();

        /// <summary>
		/// Layout the editor window
		/// </summary>
        private void DrawWindow()
        {
            GUILayout.Space(4);

            GUILayout.Label("Enter exhibit code");

            GUILayout.Space(4);

            NewFoldersSetupWizard.ExhibitCode = EditorGUILayout.TextField("Exhibit Code:", NewFoldersSetupWizard.ExhibitCode);

            Debug.Log((NewFoldersSetupWizard.ExhibitCode));

            GUILayout.Space(10);

            GUILayout.Label("Included Folders");

            GUILayout.Space(10);

            // Select all
            /*bool selectAllCache = SelectAll;
            SelectAll = GUILayout.Toggle(SelectAll, "Select All");
            if (!selectAllCache.Equals(SelectAll))
                NewFoldersSetupWizard.SelectAllChanged(SelectAll);
*/
            // Horizontal Line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Scroll
            WindowBounds.ScrollPosition = GUILayout.BeginScrollView(WindowBounds.ScrollPosition, true, true, GUILayout.Width(WindowBounds.WindowWidth), GUILayout.Height(WindowBounds.ScrollViewHeight));

            // Draw folder list
            NewFoldersSetupWizard.ParseRootDirectory();
            
            GUILayout.Space(2);
            GUILayout.EndScrollView();
            GUILayout.Space(2);

  
            // Create Folders button
            if (GUILayout.Button("Set Project"))
            {
                ProjectManager manager = GameObject.FindAnyObjectByType<ProjectManager>();
                CreateFolderHierarchy.CreateFolders(manager);
                Close();
            }
        }

        /// <summary>
        /// Menu item to display editor window.
        /// </summary>
        [MenuItem(FilePaths.MenuItemPath + "Folder Selection")]
        [MenuItem("Set ADM Project/Project Window")]
        public static void DisplayNewFolderWizardWindow()
        {
            NewFolderWizardSetupWindow popupModal = (NewFolderWizardSetupWindow)GetWindow(typeof(NewFolderWizardSetupWindow), true, "New Folder Wizard", true);
            popupModal.ShowModalUtility();
            popupModal.Focus();
        }
    }    
}