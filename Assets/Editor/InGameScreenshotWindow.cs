using UnityEditor;
using UnityEngine;

public class InGameScreenshotWindow : EditorWindow
{
    [MenuItem("Tools/In-Game Screenshot")]
    public static void ShowWindow()
    {
        GetWindow<InGameScreenshotWindow>("In-Game Screenshot");
    }

    private void OnGUI()
    {
        GUILayout.Label("Take In-Game Screenshot", EditorStyles.boldLabel);

        if (GUILayout.Button("Capture Screenshot"))
        {
            CaptureScreenshot();
        }
    }

    private void CaptureScreenshot()
    {
        // Ensure the game is running before capturing a screenshot.
        if (EditorApplication.isPlaying)
        {
            string screenshotName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(screenshotName);
            Debug.Log("Screenshot saved to: " + screenshotName);
        }
        else
        {
            Debug.LogWarning("Please enter play mode to take an in-game screenshot.");
        }
    }
}
