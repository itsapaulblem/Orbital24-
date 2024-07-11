using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager 
{
    /// <summary>
    /// Checks if the player has viewed a cutscene prior to the current one.
    /// Returns true if the saved cutscene is smaller than the current cutscene.
    /// </summary>
    /// <param name="curr">The current cutscene number</param>
    /// <returns>true if the player has viewed a prior cutscene, false otherwise</returns>
    /// <example>
    /// If the player has viewed cutscene 2 and the current cutscene is 3, this method will return true.
    /// </example>
    public static bool CheckCutscene(int curr)
    {
        return PlayerPrefs.GetInt("cutscene", 0) < curr;
    }

    /// <summary>
    /// Sets the current cutscene number.
    /// </summary>
    /// <param name="cutscene">The current cutscene number</param>
    public static void SetCutscene(int cutscene)
    {
        PlayerPrefs.SetInt("cutscene", cutscene);
    }

    /// <summary>
    /// Loads the last scene the player was in.
    /// </summary>
    /// <returns>The name of the last scene, or "Room" if no scene is saved</returns>
    public static string LoadLastScene()
    {
        return PlayerPrefs.GetString("lastScene", "Room");
    }

    /// <summary>
    /// Sets the last scene the player was in.
    /// </summary>
    /// <param name="scene">The name of the last scene</param>
    public static void SetLastScene(string scene)
    {
        PlayerPrefs.SetString("lastScene", scene);
    }

    /// <summary>
    /// Loads and sets the player's coordinates based on saved data.
    /// </summary>
    public static void LoadCoords()
    {
        float xCoord = PlayerPrefs.GetFloat("xCoord", float.PositiveInfinity);
        float yCoord = PlayerPrefs.GetFloat("yCoord", float.PositiveInfinity);
        PlayerController.SetCoords(xCoord, yCoord);
    }

    /// <summary>
    /// Sets the player's coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate</param>
    /// <param name="y">The y-coordinate</param>
    public static void SetCoords(float x, float y)
    {
        PlayerPrefs.SetFloat("xCoord", x);
        PlayerPrefs.SetFloat("yCoord", y);
    }

    /// <summary>
    /// Loads the player's kill count.
    /// </summary>
    /// <returns>The player's kill count, or 0 if no count is saved</returns>
    public static int LoadKills()
    {
        return PlayerPrefs.GetInt("kills", 0);
    }

    /// <summary>
    /// Sets the player's kill count.
    /// </summary>
    /// <param name="kills">The player's kill count</param>
    public static void SetKills(int kills)
    {
        PlayerPrefs.SetInt("kills", kills);
    }

    /// <summary>
    /// Checks if the player has made progress prior to the current point.
    /// Returns true if the saved progress is smaller than the current progress.
    /// </summary>
    /// <param name="curr">The current progress point</param>
    /// <returns>true if the player has made prior progress, false otherwise</returns>
    /// <example>
    /// If the player has made progress to point 2 and the current progress point is 3, this method will return true.
    /// </example>
    public static bool CheckProgress(int curr)
    {
        return PlayerPrefs.GetInt("progress", 0) < curr;
    }

    public static int GetDialogueBlock(string key) {
        return PlayerPrefs.GetInt(key, 1);
    }

    public static void IncrDialogueBlock(string key) {
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key, 1) + 1);
    }
    public static void DecrDialogueBlock(string key) {
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key, 1) - 1);
    }

    public static bool CheckItem(string key) {
        return PlayerPrefs.GetInt(key, 0) == 0;
    }
}