using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;

namespace Devil
{
    public struct Scene
    {
        public string name { get; set; }
        public bool hasStartBeenCalled { get; set; }
        public int state { get; set; }
    }

    public struct Area
    {
        public string name { get; set; }
        public Decimal progress { get; set; }
    }

    public class OriMemory
    {
        private Dictionary<string, string> funcPatterns = new Dictionary<string, string>()
        {
            // GameStateMachine::Awake()
            // Hooked to enable knowing when the player is in the game, essential to know when other memory is available to access
            {"GameStateMachine",      "558BEC5783EC048B7D08B8????????8938E8????????83EC0868????????50E8????????83C41085C0740CC7471400000000E94D000000E8????????83EC0868????????50E8????????|-63"},
            
            // GameController::Awake(): Unavailable until in actual game.
            // Hooked to enable differentiation between transitioning into a game from the save screen, and actually being in a game.
            {"GameController",        "83EC74C745AC00000000C745B000000000C745B400000000C745B8????????C745BC00000000C745A800000000C745A4000000008B05????????83EC086A0050|-10"},
            
            // Found This By Watching Accesses: Unavailable until in actual game.
            // Hooked to obtain information about World Events and Keys
            {"WorldEvents",           "57568B7D088B771083FE0D0F83F10000008BCEC1E102B8????????03C18B00FFE00FB605????????0FB64F143BC10F94C00FB6C0E9CB0000000FB605????????0FB64F143BC10F94C00FB6C0E9B30000000FB605????????0FB64F143BC10F94C00FB6C0E99B0000000FB605????????0FB64F143BC1|-82"},
            
            // SeinCharacter::Awake(): Unavailable until in actual game.
            // Hooked to know everything about Ori (internally named Sein, don't ask)
            {"SeinCharacter",         "558BEC5783EC048B7D08B8????????8938B8????????893883EC0C68????????E8????????83C41083EC08578945F850E8????????83C4108B45F889473483EC0C57E8????????83C41083EC085057E8????????83C4108D65FC5FC9C3|-82"},
            
            // ScenesManager::Awake()
            // Hooked to know what scenes are loaded
            {"ScenesManager",         "558BEC535783EC208B7D08B8????????893883EC0C57E8????????83C4108B05????????8B40208945F48B40308945E485FF|-38"},
            
            // SeinDeathCounter::get_Count()
            // Hooked to get death counter, for some reason this is a global singleton...
            {"SeinDeathCounter",      "83C41085C0740433C0EB098B05????????8B4014C9C3|-9"},
            
            // Hooked to know Ori's position, used to trigger splits based on location.
            {"Position",              "05480000008B08894DE88B4804894DEC8B40088945F08B05"},

            // Hooked to get Map%
            {"GameWorld",             "558BEC53575683EC0C8B7D08B8????????89388B47|-8"},
        };

        public Dictionary<string, int> funcPointers = new Dictionary<string, int>() { };
        public Dictionary<string, int> pCache = new Dictionary<string, int>() { };

        public Process proc;
        public bool isHooked = false;
        public DateTime hookedTime;

        public bool HookProcess() {
            if (proc == null || proc.HasExited) {
                // Initialize/Reset all the pointers to 0
                foreach (var pair in funcPatterns) {
                    funcPointers[pair.Key] = 0;
                }

                Process[] processes = Process.GetProcessesByName("ori");
                if (processes.Length == 0) {
                    isHooked = false;
                    return isHooked;
                }

                proc = processes[0];
                if (proc.HasExited) {
                    isHooked = false;
                    return isHooked;
                }

                isHooked = true;
                hookedTime = DateTime.Now;
            }

            return isHooked;
        }

        public int GetBasePointer(string name) {
            if (!funcPointers.ContainsKey(name) || !funcPatterns.ContainsKey(name)) {
                return 0;
            }

            if (funcPointers[name] == 0) {
                int[] addrs = Memory.FindMemorySignatures(proc, funcPatterns[name]);
                funcPointers[name] = addrs[0];
            }

            return funcPointers[name];
        }

        public void ClearPointerCache() {
            List<string> keys = new List<string>(pCache.Keys);
            foreach (var key in keys) {
                pCache[key] = 0;
            }
        }

        public int GetCachedPointer(string name) {
            if (!pCache.ContainsKey(name) || pCache[name] == 0) {
                int pointer = GetBasePointer(name);
                int start = Memory.GetInt32(proc, pointer);    // Pointer to Object
                int result = Memory.GetInt32(proc, start);     // Head of Object
                pCache[name] = result;
                return result;
            }
            return pCache[name];
        }

        public int GetCachedAddress(string name, int addr) {
            if (!pCache.ContainsKey(name) || pCache[name] == 0) {
                int result = Memory.GetInt32(proc, addr);
                pCache[name] = result;
                return result;
            }
            return pCache[name];
        }

        public float GetPosition(int axis_id) {
            // Resolved mono + 0x001F42C4 [0x50, 0x688, 0x7F0, 0x478, 0x750]
            // Found assembly that accessed it.

            int pPosition = GetBasePointer("Position");
            int start = Memory.GetInt32(proc, pPosition - 0x1C);
            int path0 = Memory.GetInt32(proc, start);
            return Memory.GetFloat(proc, path0 + 0x48 + (axis_id * 4));
        }

        public Dictionary<string, bool> GetEvents(Dictionary<string, int> abilities) {
            int pEvents = GetBasePointer("WorldEvents");
            int start = Memory.GetInt32(proc, pEvents) - 0x2;

            Dictionary<string, bool> results = new Dictionary<string, bool>();
            foreach (var pair in abilities) {
                results[pair.Key] = Memory.GetBool(proc, start + pair.Value);
            }
            return results;
        }

        public Dictionary<string, bool> GetKeys(Dictionary<string, int> abilities) {
            int pEvents = GetBasePointer("WorldEvents");
            int start = Memory.GetInt32(proc, pEvents) - 0x2;

            Dictionary<string, bool> results = new Dictionary<string, bool>();
            foreach (var pair in abilities) {
                results[pair.Key] = Memory.GetBool(proc, start - 0x40 + pair.Value);
            }
            return results;
        }

        public Dictionary<string, bool> GetAbilities(Dictionary<string, int> abilities) {
            int path0 = GetSein();
            int path1 = Memory.GetInt32(proc, path0 + 0x4C);                 // PlayerAbilities
            Dictionary<string, bool> results = new Dictionary<string, bool>();
            foreach (var pair in abilities) {
                int path2 = Memory.GetInt32(proc, path1 + (pair.Value * 4)); // CharacterAbility
                results[pair.Key] = Memory.GetBool(proc, path2 + 0x8);
            }
            return results;
        }

        public int GetGame() {
            return GetCachedPointer("GameController");
        }

        public int GetScenesManager() {
            return GetCachedPointer("ScenesManager");
        }

        public int GetDeathCounter() {
            return GetCachedPointer("SeinDeathCounter");
        }

        public int GetSein() {
            return GetCachedPointer("SeinCharacter");
        }

        public int GetGameStateMachine() {
            return GetCachedPointer("GameStateMachine");
        }

        public int GetGameWorld() {
            return GetCachedPointer("GameWorld");
        }

        public int GetGameState() {
            int path0 = GetGameStateMachine();
            return Memory.GetInt32(proc, path0 + 0x14);    // GameStateMachine.CurrentState
        }

        public int GetDeathsCount() {
            int start = GetDeathCounter();
            return Memory.GetInt32(proc, start + 0x14);   // SeinDeathCounter.m_deathCounter
        }

        public Area[] GetMapCompletion() {
            int start = GetGameWorld();
            int path1 = Memory.GetInt32(proc, start + 0x18);   // Head of RuntimeAreas List`1
            int path2 = Memory.GetInt32(proc, path1 + 0x08);   // Head of List`1._items
            int size = Memory.GetInt32(proc, path1 + 0x0C);    // Size of the Array

            List<Area> areas = new List<Area>();
            for (var i = 0; i < size; i++) {
                int path3 = Memory.GetInt32(proc, path2 + 0x10 + (i * 0x04)); // Head of RuntimeGameWorldArea

                float completionAmount = Memory.GetFloat(proc, path3 + 0x14); // RuntimeGameWorldArea.m_completionAmount 
                bool completionIsDirty = Memory.GetBool(proc, path3 + 0x18);  // RuntimeGameWorldArea.m_dirtyCompletionAmount

                int path4 = Memory.GetInt32(proc, path3 + 0x08);              // Head of RuntimeGameWorldArea
                int path5 = Memory.GetInt32(proc, path4 + 0x1C);              // Head of String object
                int len = Memory.GetInt32(proc, path5 + 0x08);                // Length of String

                string areaName = System.Text.Encoding.Unicode.GetString(
                    Memory.GetBytes(proc, path5 + 0x0C, 2 * len));            // RuntimeGameWorldArea.AreaNameString

                Decimal completionRounded = Math.Round((Decimal)completionAmount * 100, 2, MidpointRounding.AwayFromZero);

                Area area = new Area();
                area.name = areaName;
                area.progress = completionRounded;
                areas.Add(area);
            }

            return areas.ToArray();
        }

        public Scene[] GetScenes() {
            int start = GetScenesManager();
            int path1 = Memory.GetInt32(proc, start + 0x14);   // Head of ActiveScenes List`1
            int path2 = Memory.GetInt32(proc, path1 + 0x08);   // Head of List`1._items
            int size = Memory.GetInt32(proc, path1 + 0x0C);    // Size of the Array

            List<Scene> scenes = new List<Scene>();
            for (var i = 0; i < size; i++) {
                int path3 = Memory.GetInt32(proc, path2 + 0x10 + (i * 0x04)); // Head of SceneManagerScene

                bool hasStartBeenCalled = Memory.GetBool(proc, path3 + 0x10); // SceneManagetScene.HasStartBeenCalled
                int currentState = Memory.GetInt32(proc, path3 + 0x14);       // SceneManagerScene.CurrentState

                int path4 = Memory.GetInt32(proc, path3 + 0x0C);              // Head of RuntimeSceneMetadata
                int path5 = Memory.GetInt32(proc, path4 + 0x08);              // Head of String object
                int len = Memory.GetInt32(proc, path5 + 0x08);                // Length of String

                string sceneName = System.Text.Encoding.Unicode.GetString(
                    Memory.GetBytes(proc, path5 + 0x0C, 2 * len));            // RuntimeSceneMetadata.Scene

                Scene scene = new Scene();
                scene.name = sceneName;
                scene.hasStartBeenCalled = hasStartBeenCalled;
                scene.state = currentState;
                scenes.Add(scene);
            }

            return scenes.ToArray();
        }

        public bool GetGameInfo(string field) {
            int start = GetGame();
            return Memory.GetBool(proc, start + gameControllerFields[field]);
        }

        public float GetGameTime() {
            int start = GetGame();
            int path0 = GetCachedAddress("GameTimer", start + 0x14); // GameTimer
            return Memory.GetFloat(proc, path0 + 0x1c);              // GameTimer.CurrentTime
        }

        public bool GetSeinHasSoulFlame() {
            return (GetSeinEnergy("Max") > 0);
        }

        public int GetSeinInventory(string field) {
            int start = GetSein();
            int path0 = GetCachedAddress("SeinInventory", start + 0x2C); // SeinInventory
            return Memory.GetInt32(proc, path0 + seinInventoryFields[field]);
        }

        public float GetSeinLevel(string field) {
            int start = GetSein();
            int path0 = GetCachedAddress("SeinLevel", start + 0x38); // SeinLevel
            return Memory.GetInt32(proc, path0 + seinLevelFields[field]);
        }

        public float GetSeinEnergy(string field) {
            int start = GetSein();
            int path0 = GetCachedAddress("SeinEnergy", start + 0x3C); // SeinEnergy
            return Memory.GetFloat(proc, path0 + seinEnergyFields[field]);
        }

        public T GetSeinDamage<T>(string field) {
            int start = GetSein();
            int path0 = GetCachedAddress("SeinMortality", start + 0x40);      // SeinMortality
            int path1 = GetCachedAddress("SeinDamageReciever", path0 + 0x08); // SeinDamageReciever
            int path2 = path1 + seinDamageFields[field];

            if (typeof(T) == typeof(Boolean)) {
                return (T)(object)Memory.GetBool(proc, path2);
            } else {
                return (T)(object)Memory.GetFloat(proc, path2);
            }
        }

        public T GetSeinHealth<T>(string field) {
            int start = GetSein();
            int path0 = GetCachedAddress("SeinMortality", start + 0x40);        // SeinMortality
            int path1 = GetCachedAddress("SeinHealthController", path0 + 0x0C); // SeinHealthController
            int path2 = path1 + seinHealthFields[field];

            if (typeof(T) == typeof(Single)) {
                return (T)(object)Memory.GetFloat(proc, path2);
            } else {
                return (T)(object)Memory.GetInt32(proc, path2);
            }
        }

        public T GetSeinAbilityState<T>(string field) {
            int start = GetSein();
            int path0 = GetCachedAddress("SeinAbilities", start + 0x10); // SeinAbilities
            int[] path = seinAbilityStateFields[field];

            int path1 = Memory.GetInt32(proc, path0 + path[0]);

            if (typeof(T) == typeof(Boolean)) {
                return (T)(object)Memory.GetBool(proc, path1 + path[1]);
            } else {
                return (T)(object)Memory.GetInt32(proc, path1 + path[1]);
            }
        }

        public T GetSeinSoulFlame<T>(string field) {
            int start = GetSein();
            int path0 = GetCachedAddress("SeinSoulFlame", start + 0x28); // SeinSoulFlame
            int path1 = path0 + seinSoulFlameFields[field];

            if (typeof(T) == typeof(Single)) {
                return (T)(object)Memory.GetFloat(proc, path1);
            } else if (typeof(T) == typeof(Boolean)) {
                return (T)(object)Memory.GetBool(proc, path1);
            } else {
                return (T)(object)Memory.GetInt32(proc, path1);
            }
        }

        private void write(string str) {
            StreamWriter wr = new StreamWriter("test.log", true);
            wr.WriteLine("[" + DateTime.Now + "] " + str);
            wr.Close();
        }

        public static Dictionary<string, int> seinInventoryFields = new Dictionary<string, int>()
        {
            {"Keystones",         0x1C}, // Int32
            {"Map Stones",        0x20}, // Int32
            {"Skill Points",      0x24}, // Int32
        };

        public static Dictionary<string, int> seinSoulFlameFields = new Dictionary<string, int>()
        {
            {"Number of Soul Flames Cast",         0x90}, // Int32
            {"Time Held Down",                     0x94}, // Single
            {"Hold Down Duration",                 0x98}, // Single
            {"Is Locked",                          0xA4}, // Boolean
            {"Cooldown Duration",                  0xA8}, // Single
            {"Rekindle Cooldown Duration",         0xAC}, // Single
            {"Cooldown Remaining",                 0xB0}, // Single
            {"Is Casting",                         0xBC}, // Boolean
        };

        public static Dictionary<string, int> seinLevelFields = new Dictionary<string, int>()
        {
            {"Current",         0x28}, // Single
            {"Skill Points",    0x24}, // Single
            {"Experience",      0x2C}, // Single
        };

        public static Dictionary<string, int> seinHealthFields = new Dictionary<string, int>()
        {
            {"Current",         0x1c}, // Single
            {"Max",             0x20}, // Int32
        };

        public static Dictionary<string, int> seinEnergyFields = new Dictionary<string, int>()
        {
            {"Current",         0x20}, // Single
            {"Max",             0x24}, // Single
        };

        public static Dictionary<string, int> seinDamageFields = new Dictionary<string, int>()
        {
            {"Is Immortal",                             0x4c}, // Bool
            {"Died",                                    0x5c}, // Bool
            {"Invinsible Time Remaining",               0x54}, // Single
            {"Invinsible To Enemies Time Remaining",    0x58}, // Single
            {"Hurt Time Remaining",                     0x70}, // Single
        };

        public static Dictionary<string, int> gameControllerFields = new Dictionary<string, int>()
        {
            {"Is Closing",                  0x05}, // Boolean
            {"Is Focused",                  0x06}, // Boolean
            {"Is Restarting",               0x69}, // Boolean
            {"Is Loading New",              0x6A}, // Boolean (RequiresIntialValues)
            {"Is Loading Old",              0x6B}, // Boolean (IsLoading)
            {"Menu Can Be Opened",          0x7C}, // Boolean
            {"Gameplay Suspended",          0x7D}, // Boolean
            {"Gameplay Suspended For UI",   0x7E}, // Boolean
            {"Lock Input By Action",        0x7F}, // Boolean
            {"Lock Input",                  0x80}, // Boolean
        };

        public static Dictionary<string, int[]> seinAbilityStateFields = new Dictionary<string, int[]>()
        {
            {"Is Gliding",                  new int[]{0x24, 0x47}}, // Boolean
            {"Is Grabbing Block",           new int[]{0x28, 0x79}}, // Boolean
            {"Is Grabbing Wall",            new int[]{0x30, 0x6C}}, // Boolean
            {"Is Crouching",                new int[]{0x2C, 0x28}}, // Boolean
            {"Swim State",                  new int[]{0x3C, 0x74}}, // Int32 (Out, Surface, Under Moving, Under Idle)
            {"Run State",                   new int[]{0x44, 0x40}}, // Int32 (Run, Jog, Walk)
            {"Is Bashing",                  new int[]{0x50, 0xD0}}, // Boolean
        };

        public static Dictionary<string, int> axes = new Dictionary<string, int>()
        {
            {"x",   0},
            {"y",   1},
        };

        public static Dictionary<string, int> keys = new Dictionary<string, int>()
        {
            {"Water Vein",   0},
            {"Gumon Seal",   1},
            {"Sunstone",     2},
        };

        public static Dictionary<string, int> events = new Dictionary<string, int>()
        {
            {"Ginso Tree Entered",   0},
            {"Mist Lifted",          1},
            {"Clean Water",          2},
            {"Wind Released",        3},
            {"Gumo Free",            4},
            {"Forlorn Restored",     5},
            {"Spirit Tree Reached",  6},
            {"Warmth Returned",      7},
        };

        public static Dictionary<string, int> abilities = new Dictionary<string, int>()
        {
            {"Bash",                     5},
            {"Charge Flame",             6},
            {"Wall Jump",                7},
            {"Stomp",                    8},
            {"Double Jump",              9},
            {"Charge Jump",              10},
            {"Magnet",                   11},
            {"Ultra Magnet",             12},
            {"Climb",                    13},
            {"Glide",                    14},
            {"Spirit Flame",             15},
            {"Rapid Fire",               16},
            {"Soul Efficiency",          17},
            {"Water Breath",             18},
            {"Charge Flame Blast",       19},
            {"Charge Flame Burn",        20}, 
            {"Double Jump Upgrade",      21},
            {"Bash Upgrade",             22},
            {"Ultra Defense",            23},
            {"Health Efficiency",        24},
            {"Sense",                    25},
            {"Stomp Upgrade",            26},
            {"Quick Flame",              27},
            {"Map Markers",              28},
            {"Energy Efficiency",        29},
            {"Health Markers",           30},
            {"Energy Markers",           31},
            {"Ability Markers",          32},
            {"Rekindle",                 33}, 
            {"Regroup",                  34}, 
            {"Charge Flame Efficiency",  35},
            {"Ultra Soul Flame",         36},
            {"Soul Flame Efficiency",    37},
            {"Split Flame",              38},
            {"Spark Flame",              39},
            {"Cinder Flame",             40},
            {"Ultra Split Flame",        41},
        };
    }
}