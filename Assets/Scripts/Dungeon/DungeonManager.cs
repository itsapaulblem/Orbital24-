using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DungeonManager : MonoBehaviour
{
    private string[,] Rooms; // row, col
    private List<Coords> Ends;
    private Coords PlayerRoom = new Coords(10,5);
    private GameObject DungeonMap;
    private GameObject BossRoom;

    Dictionary<string,Vector3> SceneMap = new Dictionary<string,Vector3>(){
        {"TBLR", new Vector3(42.25f, -50f, 0f)},
        {"TBL", new Vector3(0f, -50f, 0f)}, {"TBR", new Vector3(-42.125f, -50f, 0f)}, 
        {"TLR", new Vector3(42.25f, -25f, 0f)}, {"BLR", new Vector3(0f, -25f, 0f)},
        {"TB", new Vector3(-42.125f, -25f, 0f)}, {"LR", new Vector3(42.25f, 0f, 0f)}, 
        {"TL", new Vector3(0f, 0f, 0f)}, {"TR", new Vector3(-42.125f, 0f, 0f)}, 
        {"BL", new Vector3(42.25f, 25f, 0f)}, {"BR", new Vector3(0f, 25f, 0f)},
        {"T", new Vector3(-42.125f, 25f, 0f)}, {"B", new Vector3(42.25f, 50f, 0f)}, 
        {"L", new Vector3(0f, 50f, 0f)}, {"R", new Vector3(-42.125f, 50f, 0f)},
        {"Boss", new Vector3(0f, 0f, 0f)}, {"Empty", new Vector3(200f, 200f, 0f)}
    };

    Dictionary<string,Vector3> EnterPos = new Dictionary<string,Vector3>(){
        {"T", new Vector3(0f, -9f, 0f)}, {"B", new Vector3(0f, 9f, 0f)}, 
        {"L", new Vector3(16f, 0f, 0f)}, {"R", new Vector3(-16f, 0f, 0f)},
        {"Boss", new Vector3(0f, -20f, 0f)}
    };

    Dictionary<char, int> rm = new Dictionary<char, int>(){ {'T',-1}, {'B',1}, {'L',0}, {'R',0} };
    Dictionary<char, int> cm = new Dictionary<char, int>(){ {'L',-1}, {'R',1}, {'T',0}, {'B',0} };
    Dictionary<char, char> checkroom = new Dictionary<char, char>(){ {'L','R'}, {'R','L'}, {'T','B'}, {'B','T'} };
    private int started = 0;
    private int terminated = 0;


    // Initialise and Generate map
    void Start()
    {
        DungeonMap = GameObject.Find("Dungeon Rooms");
        BossRoom = GameObject.Find("Boss Room");

        // Generate and Load scene.
        GenerateDungeon();

        // Iterate and fix any invalid outward path missed during generation.
        IEnumerator FixAnyWhenReady() {
            while (terminated < started) {
                yield return null;
            }
            for (int r = 0; r < 11; r++) {
                for (int c = 0; c < 11; c++) {
                    FixRoom(r,c);
                }
            }
        }
        StartCoroutine(FixAnyWhenReady());

        LoadRoom();
        GameObject.Find("Player").transform.position = EnterPos["T"];

        IEnumerator PrintWhenReady() { // TODO: For Debugging, can remove afterwards
            while (terminated < started) {
                yield return null;
            }
            string table = "";
            for (int y = 0; y < 11; y++) {
                string row = "";
                for (int x = 0; x < 11; x++) {
                    string t = Rooms[y,x] == null? "---  ":Rooms[y,x].PadRight(5,' ');
                    row = row + t;
                }
                table = table + row + "\n";
            }
            Debug.Log(table);
        }
        StartCoroutine(PrintWhenReady());
    }

    // Reset and Initialise
    private void InitRoom(int row, int col)
    {
        Rooms = new string[row, col];
        Ends = new List<Coords>();
        // Place starting room
        Rooms[10, 5] = "TLR"; // Assume hardcoded 11,11 
    }

    public void GenerateDungeon()
    {
        // Initialise room map
        InitRoom(11,11);

        // Generate Map Async
        StartCoroutine(GenerateDungeonRecursive(10, 4, 0, 4)); // left
        StartCoroutine(GenerateDungeonRecursive(9, 5, 0, 6)); // top
        StartCoroutine(GenerateDungeonRecursive(10, 6, 0, 4)); // right

        // Select Farthest endpoint to be Boss room. 
        IEnumerator SelectBossWhenReady() {
            while (terminated < started) {
                yield return null;
            } // wait until ready
            Coords farthest = null;
            float magnitude = 0;
            foreach (Coords c in Ends) {
                float tmp = (float)Math.Pow(Math.Pow(10-c.row, 2) + Math.Pow(5-c.col, 2), 0.5f);
                if (tmp > magnitude) {
                    magnitude = tmp;
                    farthest = c;
                }
            }
            Rooms[farthest.row, farthest.col] = "Boss";
        }
        StartCoroutine(SelectBossWhenReady());
    }

    // Tuple of coordinates
    private class Coords {
        Dictionary<string, int> rm = new Dictionary<string, int>(){ {"T",-1}, {"B",1}, {"L",0}, {"R",0} };
        Dictionary<string, int> cm = new Dictionary<string, int>(){ {"L",-1}, {"R",1}, {"T",0}, {"B",0} };
        public int row;
        public int col;
        public Coords(int r, int c) { row = r; col = c; }
        public void Move(string direction) {
            row += rm[direction];
            col += cm[direction];
        }
    }

    private IEnumerator GenerateDungeonRecursive(int r, int c, float endProb, int minRm)
    {
        if (Rooms[r,c] != null) { yield break; }
        Rooms[r,c] = "-";
        started += 1;
        string curr = "";
        string must = "";
        string cannot = "";

        // Identify compulsory and invalid paths
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
                curr = must[UnityEngine.Random.Range(0,must.Length)] + "";
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

        // Reorder char in string to match format "TBLR"
        string ordered = "";
        foreach (char _ in "TBLR") {
            if (curr.Contains(_)) { ordered = ordered + _; }
        }
        Rooms[r, c] = ordered;

        // GenerateRecursive Async
        foreach (char _ in Rooms[r, c]) {
            StartCoroutine(GenerateDungeonRecursive(r+rm[_], c+cm[_], endProb + 0.15f, minRm - 1));
        }

        // Store Endpoints
        if (curr.Length == 1) {
            Ends.Add(new Coords(r,c));
        }

        terminated += 1;
    }

    // Check valid for outward pointing paths only. (not inwards)
    private bool CheckValid(int r, int c, string room) {
        if (room == "") { return false; }
        if (room == "Boss") { return true; }
        foreach (char d in room) {
            if (r+rm[d] < 0 || r+rm[d] > 10 || c+cm[d] < 0 || c+cm[d] > 10) { return false; }
            if (Rooms[r+rm[d], c+cm[d]] != null && !Rooms[r+rm[d], c+cm[d]].Contains(checkroom[d])) { return false; }
        }
        return true;
    }

    // Fix invalid outward paths.
    private void FixRoom(int r, int c) {
        string room = Rooms[r,c];
        if (room == null || room == "Boss") { return; }
        string valid = "";
        foreach (char d in room) {
            if (r+rm[d] < 0 || r+rm[d] > 10 || c+cm[d] < 0 || c+cm[d] > 10) { continue; }
            if (Rooms[r+rm[d], c+cm[d]] != null && !Rooms[r+rm[d], c+cm[d]].Contains(checkroom[d])) { continue; }
            valid = valid + d;
        }
        Rooms[r,c] = valid;
    }

    public void ChangeRoom(string direction) {
        if (Rooms[PlayerRoom.row, PlayerRoom.col] == "Boss") { return; }
        PlayerRoom.Move(direction);
        LoadRoom();
        if (Rooms[PlayerRoom.row, PlayerRoom.col] == "Boss") {
            direction = "Boss";
        }
        GameObject.Find("Player").transform.position = EnterPos[direction];
    }

    public void LoadRoom() {
        string room = Rooms[PlayerRoom.row, PlayerRoom.col];
        if (room == "Boss") {
            BossRoom.transform.position = SceneMap[room];
            DungeonMap.transform.position = SceneMap["Empty"];

            GameObject.Find("Virtual Camera")
                        .GetComponent<Cinemachine.CinemachineConfiner>()
                        .m_BoundingShape2D = gameObject.transform.Find("Boss Confiner").GetComponent<PolygonCollider2D>();

            GameObject.Find("Virtual Camera").GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize = 15;
        } else {
            DungeonMap.transform.position = SceneMap[room];
            BossRoom.transform.position = SceneMap["Empty"];
        }
    }


    // TODO: For Debugging purpose, can remove afterwards
    public bool run = false;
    public bool indiv = false;
    public string[] testing = {"TBLR", "TBL", "TBR", "TLR", "BLR", "TB", "LR", "TL", 
                               "TR", "BL", "BR", "T", "B", "L", "R", "Boss"};
    public int ir = 0;
    public int ic = 0;
    void Update()
    {
        if (run && !indiv) {
            run = false;
            string table = "";
            for (int y = 0; y < 11; y++) {
                string row = "";
                for (int x = 0; x < 11; x++) {
                    if (Rooms[y,x] != null && !CheckValid(y,x,Rooms[y,x])) { Debug.Log(y + " " + x); }
                    string t = Rooms[y,x] == null? "---  ":Rooms[y,x].PadRight(5,' ');
                    row = row + t;
                }
                table = table + row + "\n";
            }
            Debug.Log(table);
        } else if (run && indiv) {
            Debug.Log(Rooms[ir,ic] + " " + CheckValid(ir,ic,Rooms[ir,ic]));
        }
    }

    
}


/*

            IEnumerator WaitForAll() {
                while (terminated < started) {
                    yield return null;
                }
                string table = "";
                for (int y = 0; y < 11; y++) {
                    string row = "";
                    for (int x = 0; x < 11; x++) {
                        string t = Rooms[y,x] == null? "---  ":Rooms[y,x].PadRight(5,' ');
                        row = row + t;
                    }
                    table = table + row + "\n";
                }
                Debug.Log(table);
            }
            StartCoroutine(WaitForAll());
*/