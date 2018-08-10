/***************************************************/
/***  INCLUDE               ************************/
/***************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

/***************************************************/
/***  THE CLASS             ************************/
/***************************************************/
public class SettingsGenerator :
    MonoBehaviour
{
    #region Constants
    /***************************************************/
    /***  CONSTANTS             ************************/
    /***************************************************/

    public const string PATH = "Assets/Texts/";
    public const string FILE_NAME = "Settings.json";

    #endregion
    #region Methods
    /***************************************************/
    /***  METHODS               ************************/
    /***************************************************/

    [MenuItem("Assets/Generate\\Update settings file")]
    private static void GenerateSettingsFile()
    {
        if (File.Exists(PATH + FILE_NAME) && ! EditorUtility.DisplayDialog("Are you sure ?", "This action will overwrite the actual settings saved.\n Do you still want to do it ?", "Yes", "Oh God ! No !!"))
            return;

        // Create directory
        if (!Directory.Exists(PATH))
            Directory.CreateDirectory(PATH);

        // Create/Open file
        StreamWriter sw;
        if (!File.Exists(PATH + FILE_NAME))
            sw = File.CreateText(PATH + FILE_NAME);
        else
            sw = new StreamWriter(PATH + FILE_NAME);

        sw.Write(JsonUtility.ToJson(new SettingsManager.Settings(), true));
        sw.Close();

        Debug.Log("Settings file generated at: " + PATH + FILE_NAME);
    }

    #endregion
}
