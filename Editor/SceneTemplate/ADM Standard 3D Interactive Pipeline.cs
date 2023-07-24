#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEditor;

public class NewSceneTemplatePipeline : ISceneTemplatePipeline
{
    public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
    {
        return true;
    }

    public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
    {
        
    }

    public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName)
    {
        EditorApplication.ExecuteMenuItem("Set ADM Project/Project Window");



    }
}
#endif
