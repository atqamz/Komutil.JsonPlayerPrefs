using UnityEditor;

namespace Komutil.JsonPlayerPrefs.Editor
{
    public class JsonPlayerPrefsEditor : EditorWindow
    {
        [MenuItem("Edit/Clear All JsonPlayerPrefs")]
        public static void ClearAllJsonPlayerPrefs()
        {
            JsonPlayerPrefs.DeleteAll();
        }
    }
}