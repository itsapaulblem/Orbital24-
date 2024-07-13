using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Cinemachine;

public class DungeonManager : MonoBehaviour
{
    private string[,] Rooms; // row, col
    private List<GameObject>[,] Entities;
    private int row; private int col;
    private List<Coords> Ends;

    // Spawn management
    Dictionary<string, Func<GameObject>> Spawn;

    // Scene management
    private Coords PlayerRoom;
    private GameObject DungeonMap;
    private GameObject BossRoom;
    private GameObject Ritual;
    private GameObject Dialogue;

    // Scene coords for rooms
    Dictionary<string,Vector3> SceneMap = new Dictionary<string,Vector3>(){
        {"TBLR", new Vector3(42.25f, -50f, 0f)},
        {"TBL", new Vector3(0f, -50f, 0f)}, {"TBR", new Vector3(-42.125f, -50f, 0f)}, 
        {"TLR", new Vector3(42.25f, -25f, 0f)}, {"BLR", new Vector3(0f, -25f, 0f)},
        {"TB", new Vector3(-42.125f, -25f, 0f)}, {"LR", new Vector3(42.25f, 0f, 0f)}, 
        {"TL", new Vector3(0f, 0f, 0f)}, {"TR", new Vector3(-42.125f, 0f, 0f)}, 
        {"BL", new Vector3(42.25f, 25f, 0f)}, {"BR", new Vector3(0f, 25f, 0f)},
        {"T", new Vector3(-42.125f, 25f, 0f)}, {"B", new Vector3(42.25f, 50f, 0f)}, 
        {"L", new Vector3(0f, 50f, 0f)}, {"R", new Vector3(-42.125f, 50f, 0f)},
        {"Boss", new Vector3(0f, 10.675f, 0f)}, {"Empty", new Vector3(200f, 200f, 0f)}
    };

    // Player coords for entering room
    Dictionary<string,Vector3> EnterPos = new Dictionary<string,Vector3>(){
        {"T", new Vector3(0f, -8f, 0f)}, {"B", new Vector3(0f, 8.5f, 0f)}, 
        {"L", new Vector3(15.5f, 0f, 0f)}, {"R", new Vector3(-15.5f, 0f, 0f)},
        {"Boss", new Vector3(0f, -9f, 0f)}
    };

    // Helpers
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
        Ritual = GameObject.Find("Ritual").gameObject;
        Ritual.SetActive(false);
        Dialogue = GameObject.Find("BossDialogue");
        

        // Init Spawners
        Spawn = new Dictionary<string, Func<GameObject>>() {
            {"Coin", () => {
                GameObject coinPrefab = Resources.Load<GameObject>("Prefab/Coin");
                return Instantiate(coinPrefab, new Vector3(UnityEngine.Random.Range(-14f,14f), 
                        UnityEngine.Random.Range(-7f,7.5f), 0), Quaternion.identity); 
            }},
            {"Melee", () => {
                GameObject meleePrefab = Resources.Load<GameObject>("Prefab/Enemy");
                GameObject enemy = Instantiate(meleePrefab, new Vector3(UnityEngine.Random.Range(-14f,14f), 
                        UnityEngine.Random.Range(-7f,7.5f), 0), Quaternion.identity); 
                SpriteLibrary sl = enemy.GetComponent<SpriteLibrary>();
                sl.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("Sprites/Enemies/Side Animation/m_enemy_" + UnityEngine.Random.Range(1,2)); 
                enemy.GetComponent<EnemyAI>().sight = 100f;
                return enemy;
            }},
            {"Ranged", () => {
                GameObject rangedPrefab = Resources.Load<GameObject>("Prefab/RangedEnemy");
                GameObject enemy = Instantiate(rangedPrefab, new Vector3(UnityEngine.Random.Range(-14f,14f), 
                        UnityEngine.Random.Range(-7f,7.5f), 0), Quaternion.identity); 
                SpriteLibrary sl = enemy.GetComponent<SpriteLibrary>();
                sl.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>("Sprites/Enemies/Side Animation/r_enemy_" + UnityEngine.Random.Range(1,2)); 
                enemy.GetComponent<EnemyAI>().sight = 100f;
                return enemy;
            }},
            {"Mixed", () => {
                if (UnityEngine.Random.value < 0.5) { return Spawn["Melee"](); }
                else { return Spawn["Ranged"](); }
            }},
            {"Boss", () => {
                GameObject bossPrefab = Resources.Load<GameObject>("Prefab/FinalBoss");
                GameObject enemy = Instantiate(bossPrefab, new Vector3(0, 16f, 0), Quaternion.identity); 
                enemy.GetComponent<EnemyAI>().sight = 35f;
                return enemy;
            }}
        };

        // Generate and Load scene.
        GenerateDungeon();

        // Iterate and fix any invalid outward path missed during generation.
        IEnumerator FixAnyWhenReady() {
            while (terminated < started) {
                yield return null;
            }
            for (int r = 0; r < row; r++) {
                for (int c = 0; c < col; c++) {
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
            for (int r = 0; r < row; r++) {
                string printrow = "";
                for (int c = 0; c < col; c++) {
                    string t = Rooms[r,c] == null? "---  ":Rooms[r,c].PadRight(5,' ');
                    printrow = printrow + t;
                }
                table = table + printrow + "\n";
            }
            Debug.Log(table);
        }
        StartCoroutine(PrintWhenReady());
    }

    // Reset and Initialise
    private void InitRoom(int r, int c)
    {
        row = r;
        col = c;
        Rooms = new string[row, col];
        Entities = new List<GameObject>[row, col];
        Ends = new List<Coords>();

        // Place starting room
        Rooms[row-1, col/2] = "TLR"; 
        Entities[row-1, col/2] = new List<GameObject>();
        PlayerRoom = new Coords(row-1, col/2);
    }

    public void GenerateDungeon()
    {
        // Initialise room map
        InitRoom(5,7);

        // Generate Map Async
        StartCoroutine(GenerateDungeonRecursive(row-2, col/2, 0, 3)); // top
        StartCoroutine(GenerateDungeonRecursive(row-1, col/2-1, 0, 2)); // left
        StartCoroutine(GenerateDungeonRecursive(row-1, col/2+1, 0, 2)); // right

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
            if (r+rm[_] < 0 || r+rm[_] >= row || c+cm[_] < 0 || c+cm[_] >= col) {
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
            StartCoroutine(GenerateDungeonRecursive(r+rm[_], c+cm[_], endProb + 0.2f, minRm - 1));
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
            if (r+rm[d] < 0 || r+rm[d] >= row || c+cm[d] < 0 || c+cm[d] >= col) { return false; }
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
            if (r+rm[d] < 0 || r+rm[d] >= row || c+cm[d] < 0 || c+cm[d] >= col) { continue; }
            string tmp = Rooms[r+rm[d], c+cm[d]];
            if (tmp != null && !tmp.Contains(checkroom[d]) && tmp != "Boss") { continue; }
            valid = valid + d;
        }
        Rooms[r,c] = valid;
    }

    public void ChangeRoom(string direction) {
        if (Rooms[PlayerRoom.row, PlayerRoom.col] == "Boss") { return; }
        DeactivateRoom();
        PlayerRoom.Move(direction);
        LoadRoom();
        if (Rooms[PlayerRoom.row, PlayerRoom.col] == "Boss") {
            direction = "Boss";
        }
        GameObject.Find("Player").transform.position = EnterPos[direction];
    }

    private void LoadRoom() {
        string room = Rooms[PlayerRoom.row, PlayerRoom.col];
        if (room == "Boss") {
            BossRoom.transform.position = SceneMap[room];
            DungeonMap.transform.position = SceneMap["Empty"];

            GameObject.Find("Virtual Camera")
                        .GetComponent<Cinemachine.CinemachineConfiner>()
                        .m_BoundingShape2D = gameObject.transform.Find("Boss Confiner").GetComponent<PolygonCollider2D>();

            GameObject.Find("Virtual Camera").GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize = 15;

            GameObject.Find("Top").SetActive(false);
            GameObject.Find("Bottom").SetActive(false);
            GameObject.Find("Left").SetActive(false);
            GameObject.Find("Right").SetActive(false);

            ActivateRoom(true);
        } else {
            DungeonMap.transform.position = SceneMap[room];
            BossRoom.transform.position = SceneMap["Empty"];
            ActivateRoom();
        }
    }

    // Activate entities in room if exist, otherwise spawn entities
    private void ActivateRoom(bool isBoss = false) {
        List<GameObject> ent = Entities[PlayerRoom.row, PlayerRoom.col];
        if (isBoss) {
            GameObject boss = Spawn["Boss"]();
            Ritual.SetActive(true);
            // Add listener for Boss defeat
            NPC npc = Dialogue.GetComponent<NPC>();
            boss.GetComponent<EnemyAI>().EnemyDied += npc.FinalBossDiedHandler;
            IEnumerator AfterDialogue() {
                while (!npc.dialogueShown) {
                    yield return null;
                }
                SpriteRenderer bsr = boss.GetComponent<SpriteRenderer>();
                float rate = 1.0f/ 0.5f;
                float progress = 0.0f; 
                Color tmp = bsr.color;

                while (progress < 2.0f){
                    tmp.a = Mathf.Lerp(1, 0 , progress);
                    bsr.color = tmp;
                    progress += rate * Time.deltaTime;
                    yield return null; 
                }
                Destroy(boss);

                // TODO: Spawn sword, then to cutscene in room
            }
            StartCoroutine(AfterDialogue());
        } else if (ent != null) {
            ent.RemoveAll(e => e == null);
            foreach (GameObject g in ent) {
                g.SetActive(true);
            }
        } else {
            ent = new List<GameObject>();
            // Types: Coin(.2), Melee(.3), Ranged(.3), MixedEnemies(.2)
            float rand = UnityEngine.Random.value;
            string type = rand<0.2 ? "Coin" : (rand<0.5 ? "Melee" : (rand<0.8 ? "Ranged" : "Mixed"));
            for (int _ = 0; _ < 3; _++) {
                ent.Add(Spawn[type]());
            }
            Entities[PlayerRoom.row, PlayerRoom.col] = ent;
        }
    }

    // Deactivate entities in room
    private void DeactivateRoom() {
        List<GameObject> ent = Entities[PlayerRoom.row, PlayerRoom.col];

        // Store coins as part of room
        Coin[] newCoins = UnityEngine.Object.FindObjectsOfType<Coin>();
        foreach (Coin c in newCoins) {
            ent.Add(c.gameObject);
        }
        ent.RemoveAll(e => e == null); // Remove destroyed gameObjects
        foreach (GameObject g in ent) {
            g.SetActive(false);
        }
        
        // Destroy all bullets
        Bullet[] existingBullet = UnityEngine.Object.FindObjectsOfType<Bullet>();
        foreach (Bullet b in existingBullet) {
            Destroy(b.gameObject);
        }
    }
}