//Author : https://github.com/seekeroftheball   https://gist.github.com/seekeroftheball
//Version : 1.2
//Updated : March 2023
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

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
            public const float WindowWidth = 500;
            public const float WindowHeight = 190;

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

            GUIStyle TitleStyle = new GUIStyle(GUI.skin.label);
            TitleStyle.fontStyle = FontStyle.Bold;
            TitleStyle.alignment = TextAnchor.MiddleCenter;
            TitleStyle.fontSize = 16;

            GUILayout.Label("Please enter the project code before proceeding!", TitleStyle);

            GUILayout.Space(20);

            NewFoldersSetupWizard.ExhibitCode = EditorGUILayout.TextField("Exhibit Code:", NewFoldersSetupWizard.ExhibitCode);

            Debug.Log((NewFoldersSetupWizard.ExhibitCode));

            GUILayout.Space(20);

            GUIStyle DescStyle = new GUIStyle(GUI.skin.label);
            DescStyle.fontStyle = FontStyle.Normal;
            DescStyle.alignment = TextAnchor.MiddleLeft;
            DescStyle.fixedWidth = WindowBounds.WindowWidth;
            DescStyle.clipping = TextClipping.Clip;
            DescStyle.wordWrap= true;
            DescStyle.fontSize = 12;
            // Scroll


            // Draw folder list
            //NewFoldersSetupWizard.ParseRootDirectory();

            
            GUILayout.Label("(Example: GOL-001) This will create the necessary project folders under your Assets folder. It will be labeled  ''your project code" + " + " + "_Assets''. You will place all your project files for that case into that folder hierarchy.", DescStyle);
            GUILayout.Space(20);
            /*GUILayout.Label("Included Folders");

            GUILayout.Space(10);

            // Select all
            *//*bool selectAllCache = SelectAll;
            SelectAll = GUILayout.Toggle(SelectAll, "Select All");
            if (!selectAllCache.Equals(SelectAll))
                NewFoldersSetupWizard.SelectAllChanged(SelectAll);
*/
            // Horizontal Line
            //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

           /* // Scroll
            WindowBounds.ScrollPosition = GUILayout.BeginScrollView(WindowBounds.ScrollPosition, true, true, GUILayout.Width(WindowBounds.WindowWidth), GUILayout.Height(WindowBounds.ScrollViewHeight));

            // Draw folder list
            //NewFoldersSetupWizard.ParseRootDirectory();

            GUILayout.Space(2);
            GUILayout.EndScrollView();
            GUILayout.Space(2);*/


            // Create Folders button
            if (GUILayout.Button("Set Project"))
            {
                ProjectManager manager = GameObject.FindAnyObjectByType<ProjectManager>();
                CreateFolderHierarchy.CreateFolders(manager);
                Close();
            }

            GUILayout.Space(4);
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
#endif