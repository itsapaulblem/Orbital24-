using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    /* Templates:
    S - ╔   ╗ (TLR)

        ╚═══╝
    
    TB - ╔   ╗  LR - ╔═══╗
         ║   ║
         ╚   ╝       ╚═══╝

    TR - ╔   ╗  TL - ╔   ╗  BR - ╔═══╗  BL - ╔═══╗
         ║               ║       ║               ║
         ╚═══╝       ╚═══╝       ╚   ╝       ╚   ╝

    T - ╔   ╗   B - ╔═══╗   R - ╔═══╗   L - ╔═══╗
        ║   ║       ║   ║       ║               ║
        ╚═══╝       ╚   ╝       ╚═══╝       ╚═══╝

    All following same naming scheme
    */

    public string[] Room = { "TBL", "TBR", "TLR", "BLR", "TBL", "TBR", "TBLR", "TB", "LR", "TL", "TR", "BL", "BR"};
    public string[] End = { "T", "B", "L", "R" };

    public int DungeonWidth { get; private set; }
    public int DungeonHeight { get; private set; }

    private string[,] Rooms; // row, col

    Dictionary<char, int> rm = new Dictionary<char, int>(){ {'T',-1}, {'B',1}, {'L',0}, {'R',0} };
    Dictionary<char, int> cm = new Dictionary<char, int>(){ {'L',-1}, {'R',1}, {'T',0}, {'B',0} };
    Dictionary<char, char> checkroom = new Dictionary<char, char>(){ {'L','R'}, {'R','L'}, {'T','B'}, {'B','T'} };
    

    private void InitRoom(int row, int col)
    {
        DungeonWidth = col;
        DungeonHeight = row;
        Rooms = new string[row, col];

        // Place starting room
        Rooms[10, 5] = "TLR"; // Assume hardcoded 11,11 
    }

    public void GenerateDungeon(int roomCount, float connectionProbability)
    {
        // Initialise room map
        InitRoom(11,11);

        // Recursively generate rooms until desired count is reached
        GenerateDungeonRecursive(10, 4, 0, 4); // left
        GenerateDungeonRecursive(9, 5, 0, 6); // top
        GenerateDungeonRecursive(10, 6, 0, 4); // right
    }


    private void GenerateDungeonRecursive(int r, int c, float endProb, int minRm)
    {
        if (Rooms[r,c] != null) { return; }
        string curr = "";
        string must = "";
        string cannot = "";

        foreach (char _ in "TBLR") {
            if (r+rm[_] < 0 || r+rm[_] > 10 || c+cm[_] < 0 || c+cm[_] > 10) {
                cannot = cannot + _;
            } else if (Rooms[r+rm[_], c+cm[_]] != null && Rooms[r+rm[_], c+cm[_]].Contains(checkroom[_])) {
                must = must + _;
            } else if (Rooms[r+rm[_], c+cm[_]] != null && !Rooms[r+rm[_], c+cm[_]].Contains(checkroom[_])) {
                cannot = cannot + _;
            }
        }

        while (!CheckValid(r, c, curr)) {
            curr = "";
            if ((minRm <= 0 && UnityEngine.Random.value < endProb) || cannot.Length == 3) { // End type
                curr = must;
            } else { // Room type
                while (curr.Length <= 1) { // Minimum of 2 doorway
                    foreach (char _ in "TBLR") {
                        if (curr.Contains(_)) { continue; }
                        else if (must.Contains(_)) { 
                            curr = curr + _; 
                            continue;
                        } else if (cannot.Contains(_)) { continue; }

                        if (UnityEngine.Random.value < 0.8) { curr = curr + _; }
                    }
                }
            }
        }
        Rooms[r, c] = curr;

        foreach (char _ in Rooms[r, c]) {
            GenerateDungeonRecursive(r+rm[_], c+cm[_], endProb + 0.02f, minRm - 1);
        }
    }

    private bool CheckValid(int r, int c, string room) {
        if (room == "") { return false; }
        foreach (char d in room) {
            if (r+rm[d] < 0 || r+rm[d] > 10 || c+cm[d] < 0 || c+cm[d] > 10) { return false; }
            if (Rooms[r+rm[d], c+cm[d]] != null && !Rooms[r+rm[d], c+cm[d]].Contains(checkroom[d])) { return false; }
        }
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
