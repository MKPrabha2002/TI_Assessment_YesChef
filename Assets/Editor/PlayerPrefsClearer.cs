#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class PlayerPrefsClearer
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[PlayerPrefsClearer] All local PlayerPrefs have been completely wiped.");
    }
}
#endif
