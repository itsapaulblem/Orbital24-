using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager 
{
    public static string User = "base";

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
        return PlayerPrefs.GetInt(User + "_cutscene", 0) < curr;
    }

    /// <summary>
    /// Sets the current cutscene number.
    /// </summary>
    /// <param name="cutscene">The current cutscene number</param>
    public static void SetCutscene(int cutscene)
    {
        PlayerPrefs.SetInt(User + "_cutscene", cutscene);
    }

    /// <summary>
    /// Loads the last scene the player was in.
    /// </summary>
    /// <returns>The name of the last scene, or "Room" if no scene is saved</returns>
    public static string LoadLastScene()
    {
        return PlayerPrefs.GetString(User + "_lastScene", "Room");
    }

    /// <summary>
    /// Sets the last scene the player was in.
    /// </summary>
    /// <param name="scene">The name of the last scene</param>
    public static void SetLastScene(string scene)
    {
        PlayerPrefs.SetString(User + "_lastScene", scene);
    }

    /// <summary>
    /// Loads and sets the player's coordinates based on saved data.
    /// </summary>
    public static void LoadCoords()
    {
        float xCoord = PlayerPrefs.GetFloat(User + "_xCoord", float.PositiveInfinity);
        float yCoord = PlayerPrefs.GetFloat(User + "_yCoord", float.PositiveInfinity);
        PlayerController.SetCoords(xCoord, yCoord);
    }

    /// <summary>
    /// Sets the player's coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate</param>
    /// <param name="y">The y-coordinate</param>
    public static void SetCoords(float x, float y)
    {
        PlayerPrefs.SetFloat(User + "_xCoord", x);
        PlayerPrefs.SetFloat(User + "_yCoord", y);
    }

    /// <summary>
    /// Loads the player's kill count.
    /// </summary>
    /// <returns>The player's kill count, or 0 if no count is saved</returns>
    public static int LoadKills()
    {
        return PlayerPrefs.GetInt(User + "_kills", 0);
    }

    /// <summary>
    /// Sets the player's kill count.
    /// </summary>
    /// <param name="kills">The player's kill count</param>
    public static void SetKills(int kills)
    {
        PlayerPrefs.SetInt(User + "_kills", kills);
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
        return PlayerPrefs.GetInt(User + "_progress", 0) < curr;
    }

    public static int GetDialogueBlock(string key) {
        return PlayerPrefs.GetInt(User + "_" + key, 1);
    }

    public static void IncrDialogueBlock(string key) {
        PlayerPrefs.SetInt(User + "_" + key, PlayerPrefs.GetInt(User + "_" + key, 1) + 1);
    }
    public static void DecrDialogueBlock(string key) {
        PlayerPrefs.SetInt(User + "_" + key, PlayerPrefs.GetInt(User + "_" + key, 1) - 1);
    }

    public static bool CheckItem(string key) {
        return PlayerPrefs.GetInt(User + "_" + key, 0) == 0;
    }

    public static void Complete() {
        PlayerPrefs.SetInt(User + "_Game", 1);
        PlayerPrefs.SetInt(User + "_Ezekil", 0);
    }

    public static bool isComplete() {
        return PlayerPrefs.GetInt(User + "_Game", 0) == 1;
    }

    public static void EzekilDie() {
        PlayerPrefs.SetInt(User + "_Ezekil", 1);
    }

    public static bool isEzekilDead() {
        return PlayerPrefs.GetInt(User + "_Ezekil", 0) == 1;
    }
}