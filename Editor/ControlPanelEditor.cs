using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(MenuBinding))]
public class ControlPanelEditor : PropertyDrawer
{

    /*private VisualElement root { get; set; }

    public VisualTreeAsset m_UXML;

    public static SerializedProperty controlPanelSerializedProperty;


    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        InitializeEditor();



        var PanelTitle = new PropertyField(property.FindPropertyRelative("ControlPanelTitle"));
        //var Bindings = new PropertyField(property.FindPropertyRelative("MenuBindings"));

        SerializedProperty menuBindingsProperty = property.FindPropertyRelative("menuBindings");

        for (int i = 0; i < menuBindingsProperty.arraySize; i++)
        {
            SerializedProperty menuBindingProperty = menuBindingsProperty.GetArrayElementAtIndex(i);
            var menuBindingField = new PropertyField(menuBindingProperty);
            root.Add(menuBindingField);
        }

        //root.Add(menuBindingsProperty);
        root.Add(PanelTitle);

        //m_UXML.CloneTree(root);

        // Add custom inspector elements for ControlPanel here

        //controlPanelSerializedProperty = property;
        return root;
    }

    private void InitializeEditor()
    {
        root = new VisualElement();
    }
*/



    private VisualElement root { get; set; }

    public VisualTreeAsset m_UXML;

    public static SerializedProperty controlPanelSerializedProperty;


    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        InitializeEditor();

        var title = new PropertyField(property.FindPropertyRelative("ControlTitle"));

        var type = new PropertyField(property.FindPropertyRelative("selectedType"));

        var boolean = new PropertyField(property.FindPropertyRelative("InitialBooleanValue"));

        var gameObject = new PropertyField(property.FindPropertyRelative("GameObjectReference"));

        var section = new PropertyField(property.FindPropertyRelative("SectionReference"));


        root.Add(type);
        root.Add(title);
        root.Add(gameObject);
        root.Add(boolean);
        root.Add(section);

        //m_UXML.CloneTree(root);

        // Add custom inspector elements for ControlPanel here

        //controlPanelSerializedProperty = property;
        return root;
    }

    private void InitializeEditor()
    {
        root = new VisualElement();
    }


}/*[CustomPropertyDrawer(typeof(MenuBinding))]
public class MenuBindingEditor : PropertyDrawer
{

    private VisualElement root { get; set; }

    public VisualTreeAsset m_UXML;

    public static SerializedProperty controlPanelSerializedProperty;


    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        InitializeEditor();

        var title = new PropertyField(property.FindPropertyRelative("ControlTitle"));

        var type = new PropertyField(property.FindPropertyRelative("selectedType"));

        var boolean = new PropertyField(property.FindPropertyRelative("InitialBooleanValue"));

        var gameObject = new PropertyField(property.FindPropertyRelative("GameObjectReference"));

        var section = new PropertyField(property.FindPropertyRelative("SectionReference"));


        root.Add(type);
        root.Add(title);
        root.Add(gameObject);
        root.Add(boolean);
        root.Add(section);

        //m_UXML.CloneTree(root);

        // Add custom inspector elements for ControlPanel here

        //controlPanelSerializedProperty = property;
        return root;
    }

    private void InitializeEditor()
    {
        root = new VisualElement();
    }


}*/