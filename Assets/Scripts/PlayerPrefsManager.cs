using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager //: MonoBehaviour
{
    /// Returns true if player viewed cutscene if prior to current cutscene.
    ///   That is, if savedCutsene smaller than current cutscene, return true.
    public static bool CheckCutscene(int curr) {
        return PlayerPrefs.GetInt("cutscene", 0) < curr;
    }

    public static void SetCutscene(int cutscene) {
        PlayerPrefs.SetInt("cutscene", cutscene);
    }

    public static string LoadLastScene() {
        return PlayerPrefs.GetString("lastScene", "Cutscene1");
    }

    public static void SetLastScene(string scene) {
        PlayerPrefs.SetString("lastScene", scene);
    }
    
    /// Load and set coords based on saved coord data
    public static void LoadCoords() {
        float xCoord = PlayerPrefs.GetFloat("xCoord", 0);
        float yCoord = PlayerPrefs.GetFloat("yCoord", 0);
        PlayerController.SetCoords(xCoord, yCoord);
    }

    public static void SetCoords(float x, float y) {
        PlayerPrefs.SetFloat("xCoord", x);
        PlayerPrefs.SetFloat("yCoord", y);
    }

    public static int LoadKills() {
        return PlayerPrefs.GetInt("kills", 0);
    }

    public static void SetKills(int kills) {
        PlayerPrefs.SetInt("kills", kills);
    }

    public static bool CheckProgress(int curr) {
        return PlayerPrefs.GetInt("progress", 0) < curr;
    }
}
