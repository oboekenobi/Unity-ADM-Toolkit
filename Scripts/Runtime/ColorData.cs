using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using ADM.UISystem;

namespace MyNamespace
{
    [CreateAssetMenu(fileName = "New Color Data", menuName = "Color Data")]
    public class ColorData : ScriptableObject
    {
        string ThemeName;

        [Tooltip("The color to be applied to the root classes")]
        public Color PrimaryBaseColor;
        public Color SecondaryBaseColor;
        public Color TertiaryBaseColor;
        public Color HighlightColor;
        public Color Text1Color;
        public Color Text2Color;
        public Color BorderColor;
    }
}