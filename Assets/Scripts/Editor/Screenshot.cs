#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor {
    public static class Screenshot {
        private static int id = 0;

        [MenuItem("Screenshot/Grab")]
        public static void Grab() {
            id++;
            ScreenCapture.CaptureScreenshot("Screenshot" + id + ".png", ScreenCapture.StereoScreenCaptureMode.BothEyes);
        }
    }
}

#endif