using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayFromFirstScene
{
    private static string previousScenePath;

    static PlayFromFirstScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            var scenes = EditorBuildSettings.scenes;

            if (scenes.Length > 0)
            {
                var firstScene = scenes[0].path;
                var currentScene = EditorSceneManager.GetActiveScene().path;

                if (currentScene != firstScene)
                {
                    previousScenePath = currentScene;
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(firstScene);
                }
                else
                {
                    previousScenePath = null;
                }
            }
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (!string.IsNullOrEmpty(previousScenePath))
            {
                var scenePathToRestore = previousScenePath;
                previousScenePath = null;
                EditorSceneManager.OpenScene(scenePathToRestore);
            }
        }
    }
}
