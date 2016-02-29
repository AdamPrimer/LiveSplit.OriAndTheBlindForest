using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using LiveSplit.OriAndTheBlindForest;
using LiveSplit.OriAndTheBlindForest.Debugging;

namespace LiveSplit.OriAndTheBlindForest.State
{
    public class OriTriggers
    {
        public static Dictionary<string, string> defaultSplits = new Dictionary<string, string>() 
        {
            {"Iceless",                  "344.752, -109.065, 3.981, 4.363"},
            {"Gumo Fall Trap (Fall)",    "480.171, -241.454, 3.206, 4.153"},
            {"Gumo Fall Trap (Land)",    "469.506, -388.384, 17.010, 0.937"},
            {"Kuro Cutscene",            "297.528, -110.975, 9.909, 14.488"},
            {"End of Forlorn Escape",    "-1162.265, -221.822, 7.031, 3.334"},
            {"Enter Wind Valley",        "-490.097, 181.214, 13.063, 5.933"},
            {"End of Horu Escape",       "162.890, 577.337, 5.216, 14.574"},
            {"Valley All Cells",         "-324.301, -156.883, 6.507, 7.570"},
            {"Glades All Cells",         "126.987, -195.261, 9.002, 7.149"},
            {"Grotto All Cells",         "540.557, -186.335, 6.818, 6.410"},
            {"Swamp All Cells",          "405.508, -30.681, 8.459, 7.104"},
            {"Valley 100%",              "-324.301, -156.883, 6.507, 7.570"},
            {"Glades 100%",              "270.133, -210.397, 9.559, 7.444"},
            {"Grotto 100%",              "417.221, -102.266, 18.204, 9.143"},
            {"Swamp 100%",               "398.133, -54.553, 17.584, 13.973"},
            {"L1 Switch",                "-95.843, 380.564, 6.184, 5.030"},
            {"L1 Entrance",              "14.055, 376.350, 12.762, 7.139"},
            {"L2 Rock",                  "-50.096, 279.890, 37.405, 5.431"},
            {"L2 Entrance",              "9.486, 298.079, 9.761, 6.807"},
            {"R3 Rock",                  "295.834, 311.853, 11.000, 9.193"},
            {"Horu Entrance",            "59.881, 175.917, 15.222, 8.895"},
            {"R4 Lasers",                "257.678, 187.658, 12.156, 16.600"},
            {"R4 Entrance",              "118.095, 202.512, 11.743, 8.431"},
        };

        public static Dictionary<string, string> availableSplits = new Dictionary<string, string>() 
        {
            {"In Game",                  "Boolean"},
            {"In Menu",                  "Boolean"},
            {"Map %",                     "Value"},
            {"Hitbox",                   "Hitbox"}, 
            {"Start",                    "Boolean"},
            {"Soul Flame",               "Boolean"},
            {"Spirit Flame",             "Boolean"},
            {"Wall Jump",                "Boolean"},
            {"Spirit Tree Reached",      "Boolean"},
            {"Iceless",                  "Hitbox"},
            {"Charge Flame",             "Boolean"},
            {"Gumo Fall Trap (Fall)",    "Hitbox"},
            {"Gumo Fall Trap (Land)",    "Hitbox"},
            {"Double Jump",              "Boolean"},
            {"Water Vein",               "Boolean"},
            {"Ginso Tree Entered",       "Boolean"},
            {"Bash",                     "Boolean"},
            {"Clean Water",              "Boolean"}, // End of Escape
            {"Stomp",                    "Boolean"},
            {"Kuro Cutscene",            "Hitbox"},  // Skippable cutscene near Iceless 
            {"Glide",                    "Boolean"}, // Kuro's Feather
            {"Climb",                    "Boolean"},
            {"Mist Lifted",              "Boolean"}, // After Seal Reveal Cutscene
            {"Gumon Seal",               "Boolean"}, // Picked Up Seal
            {"Forlorn Ruins Entered",    "Boolean"},
            {"Forlorn Restored",         "Boolean"}, // Deposit Nightberry
            {"Wind Released",            "Boolean"}, // Start of Escape
            {"End of Forlorn Escape",    "Hitbox"},  // End of Escape
            {"Enter Wind Valley",        "Hitbox"},  // Entering Wind Valley
            {"Charge Jump",              "Boolean"},
            {"Sunstone",                 "Boolean"},
            {"Mount Horu Entered",       "Boolean"},
            {"R1 Into Horu Escape",      "Boolean"},
            {"Warmth Returned",          "Boolean"}, // Start of Escape
            {"End of Horu Escape",       "Hitbox"},  // End of Escape
            {"End",                      "Boolean"},

            {"Health Cells",             "Value"},
            {"Energy Cells",             "Value"},
            {"Ability Cells",            "Value"},
            {"Level",                    "Value"},
            {"Key Stones",               "Value"},

            {"Magnet",                   "Boolean"},
            {"Ultra Magnet",             "Boolean"},
            {"Rapid Fire",               "Boolean"},
            {"Soul Efficiency",          "Boolean"},
            {"Water Breath",             "Boolean"},
            {"Charge Flame Blast",       "Boolean"},
            {"Charge Flame Burn",        "Boolean"}, 
            {"Double Jump Upgrade",      "Boolean"},
            {"Bash Upgrade",             "Boolean"},
            {"Ultra Defense",            "Boolean"},
            {"Health Efficiency",        "Boolean"},
            {"Sense",                    "Boolean"},
            {"Stomp Upgrade",            "Boolean"},
            {"Quick Flame",              "Boolean"},
            {"Map Markers",              "Boolean"},
            {"Energy Efficiency",        "Boolean"},
            {"Health Markers",           "Boolean"},
            {"Energy Markers",           "Boolean"},
            {"Ability Markers",          "Boolean"},
            {"Rekindle",                 "Boolean"}, 
            {"Regroup",                  "Boolean"}, 
            {"Charge Flame Efficiency",  "Boolean"},
            {"Ultra Soul Flame",         "Boolean"},
            {"Soul Flame Efficiency",    "Boolean"},
            {"Split Flame",              "Boolean"},
            {"Spark Flame",              "Boolean"},
            {"Cinder Flame",             "Boolean"},
            {"Ultra Split Flame",        "Boolean"},
            {"Valley All Cells",         "Hitbox"},
            {"Glades All Cells",         "Hitbox"},
            {"Grotto All Cells",         "Hitbox"},
            {"Swamp All Cells",          "Hitbox"},
            {"Valley 100%",              "Hitbox"},
            {"Glades 100%",              "Hitbox"},
            {"Grotto 100%",              "Hitbox"},
            {"Swamp 100%",               "Hitbox"},
            {"L1 Switch",                "Hitbox"},
            {"L1 Entrance",              "Hitbox"},
            {"L2 Rock",                  "Hitbox"},
            {"L2 Entrance",              "Hitbox"},
            {"R3 Rock",                  "Hitbox"},
            {"Horu Entrance",            "Hitbox"},
            {"R4 Lasers",                "Hitbox"},
            {"R4 Entrance",              "Hitbox"},
        };

        public class SplitEventArgs : EventArgs
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public delegate void OnSplitHandler(object sender, SplitEventArgs e);

        public Decimal mapCompletion = 0;
        public string inGameTime = "";
        public Dictionary<string, int> counters = new Dictionary<string, int>();
        public Dictionary<string, bool> events = new Dictionary<string, bool>();

        public bool autoStart = false;
        public bool autoReset = false;
        public bool timerRunning = false;
        public int currentSplitIdx = 0;
        public Split currentSplit;
        public Split[] sSplits;

        public event OnSplitHandler OnSplit;

        public OriTriggers() { }

        public void SetSplits(List<Split> splits) {
            sSplits = splits.ToArray();
            ResetAll();
        }

        public void SetAutoStart(bool autoStart) {
            this.autoStart = autoStart;
        }

        public void SetAutoReset(bool autoReset) {
            this.autoReset = autoReset;
        }

        public Split GoToNextSplit() {
            if (currentSplitIdx + 1 >= sSplits.Length) return currentSplit;

            currentSplitIdx++;
            currentSplit = sSplits[currentSplitIdx];

            return currentSplit;
        }

        public void SplitEventHandler(string name, string value) {
            if ((currentSplit.name == name && currentSplit.value == value) ||
                    (this.autoReset && !this.timerRunning &&
                    sSplits.Length > 0 &&
                    sSplits[0].name == name && sSplits[0].value == value)) {
                LogWriter.WriteLine("Trigger Function Called.");
                if (OnSplit != null && currentSplit.doSplit) {
                    SplitEventArgs e = new SplitEventArgs();
                    e.name = name;
                    e.value = value;

                    OnSplit(this, e);
                }
                GoToNextSplit();
            }
        }

        public bool HasSplit(string name) {
            foreach (var split in sSplits) {
                if (split.name == name) {
                    return true;
                }
            }
            return false;
        }

        public void ResetAll() {
            List<string> keys;

            keys = new List<string>(counters.Keys);
            foreach (var key in keys) {
                counters[key] = 0;
            }

            keys = new List<string>(events.Keys);
            foreach (var key in keys) {
                events[key] = false;
            }

            if (sSplits.Length > 0) {
                currentSplitIdx = 0;
                currentSplit = sSplits[currentSplitIdx];
            }
        }

        public void OnStartGame(bool val) {
            if (WillTriggerEvent("Start", val)) {
                ResetAll();
                TriggerEvent("Start", val);
            }
        }

        public void OnInGameChange(bool val) {
            TriggerEvent("In Game", val);
            TriggerEvent("In Menu", !val);
        }

        public void OnPositionChange(Vector2 pos) {
            if (!availableSplits.ContainsKey(currentSplit.name) || availableSplits[currentSplit.name] != "Hitbox") {
                return;
            }

            if (new Vector4(pos, 0.68f, 1.15f).Intersects(new Vector4(currentSplit.value))) {
                TriggerHitbox(currentSplit.name, currentSplit.value);
            }
        }

        public void OnKeyChange(string key, bool val) {
            TriggerEvent(key, val);
        }

        public void OnWorldEventChange(string key, bool val) {
            TriggerEvent(key, val);
        }

        public void OnAbilityChange(string key, bool val) {
            TriggerEvent(key, val);
        }

        public void OnMapCompletionChange(Area[] val, Decimal sMapCompletion) {
            mapCompletion = sMapCompletion;
        }

        public void OnActiveScenesChange(Scene[] val, Scene[] old) {
            foreach (var scene in val) {
                if (old == null || !Array.Exists(old, delegate(Scene s) { return s.name.Equals(scene.name) && s.state == scene.state && s.hasStartBeenCalled == scene.hasStartBeenCalled; })) {
                    string state = "";
                    if ((SceneState)scene.state == SceneState.Loaded) {
                        state = "Loaded";
                        switch (scene.name) {
                            case "ginsoEntranceIntro":
                                TriggerEvent("Ginso Tree Entered", true);
                                break;
                            case "forlornRuinsGetNightberry":
                                TriggerEvent("Forlorn Ruins Entered", true);
                                break;
                            case "mountHoruHubMid":
                                TriggerEvent("Mount Horu Entered", true);
                                break;
                            case "catAndMouseMid":
                                TriggerEvent("R1 Into Horu Escape", true);
                                break;
                        }
                    } else if ((SceneState)scene.state == SceneState.Disabling) {
                        state = "Disabling";
                    } else if ((SceneState)scene.state == SceneState.Loading) {
                        state = "Loading";
                        switch (scene.name) {
                            case "creditsScreen":
                                TriggerEvent("End", true);
                                break;
                        }
                    } else if ((SceneState)scene.state == SceneState.LoadingCancelled) {
                        state = "Loading Cancelled";
                    } else if ((SceneState)scene.state == SceneState.Disabled) {
                        state = "Disabled";
                    }

                    LogWriter.WriteLine("{0} Scene: {1} {2}", state, scene.name, scene.hasStartBeenCalled);
                }
            }
        }

        public void OnGenericChange(string name, object val, object old) {
            switch (name) {
                case "Soul Flame":
                    TriggerEvent("Soul Flame", (bool)val);
                    break;

                case "Energy Max":
                    SetCounter("Energy Cells", (int)Math.Floor((float)val));
                    SetCounter("Energy Max", (int)Math.Floor((float)val));
                    break;

                case "Health Max":
                    SetCounter("Health Max", (int)val);
                    SetCounter("Health Cells", (int)val / 4 - 3);
                    break;

                case "Ability Cells":
                case "Key Stones":
                    SetCounter(name, (int)val);
                    break;

                case "Level":
                    SetCounter(name, (int)val);
                    break;

                case "In Game Time":
                    int seconds = (int)Math.Floor((float)val % 60f);
                    int minutes = (int)Math.Floor((float)val / 60f);
                    int hours = (int)Math.Floor(minutes / 60f);

                    string sHours = hours.ToString().PadLeft(2, '0');
                    string sMinutes = (minutes - hours * 60).ToString().PadLeft(2, '0');
                    string sSeconds = seconds.ToString().PadLeft(2, '0');

                    string IGT = string.Format("{0}:{1}:{2}", sHours, sMinutes, sSeconds);

                    if (IGT != inGameTime) {
                        //LogWriter.WriteLine("{0}: {1}", name, IGT);
                        inGameTime = IGT;
                    }

                    break;

                case "Is Gliding":
                case "Is Grabbing Block":
                case "Is Grabbing Wall":
                case "Is Crouching":
                case "Is Bashing":
                    if ((bool)val == true) {
                        int count = IncrementCounter(name);
                    }
                    break;

                case "Soul Flames Cast":
                case "Deaths":
                    if ((int)val > (int)old) {
                        int count = IncrementCounter(name);
                    }
                    break;

                case "Energy Current":
                case "Health Current":
                case "Skill Points":
                case "Experience":
                case "Map Stones":
                case "Immortal":
                case "Swim State":
                    //Console.WriteLine("{0}: {1} was {2}", name, val, old);
                    break;

                case "Position X":
                case "Position Y":
                case "Has Died":
                case "Run State":
                    break;

                case "Map %":
                    SplitEventHandler("Map %", val.ToString());
                    break;
            }
        }

        public int IncrementCounter(string name) {
            if (!counters.ContainsKey(name)) counters[name] = 0;

            counters[name]++;
            LogWriter.WriteLine("Counter: {0} {1}", name, counters[name]);
            SplitEventHandler(name, counters[name].ToString());

            return counters[name];
        }

        public int SetCounter(string name, int val) {
            if (counters.ContainsKey(name) && counters[name] == val) return val;

            counters[name] = val;
            LogWriter.WriteLine("Counter: {0} {1}", name, counters[name]);
            SplitEventHandler(name, val.ToString());

            return counters[name];
        }

        public void TriggerHitbox(string name, string val) {
            LogWriter.WriteLine("TriggerHitbox(): {0} {1}", name, val);
            if (events.ContainsKey(name)) {
                LogWriter.WriteLine("Existing: {0} {1}", val, events[val]);
                if (events[val]) return;
            }

            events[val] = true;
            LogWriter.WriteLine("Current Split: {0} {1}", currentSplit.name, currentSplit.value);
            LogWriter.WriteLine("Hitbox Trigger: {0} {1}", name, val);

            SplitEventHandler(name, val.ToString());
        }

        public bool WillTriggerEvent(string name, bool val) {
            return (!(events.ContainsKey(name) && events[name] == val) ||
                    (this.autoReset && !this.timerRunning &&
                     sSplits.Length > 0 && sSplits[0].name == name));
        }

        public void TriggerEvent(string name, bool val) {
            LogWriter.WriteLine("TriggerEvent(): {0} {1}", name, val);
            if (events.ContainsKey(name)) {
                LogWriter.WriteLine("Existing: {0} {1}", name, events[name]);
            }
            if (!WillTriggerEvent(name, val)) return;

            events[name] = val;
            LogWriter.WriteLine("Current Split: {0} {1}", currentSplit.name, currentSplit.value);
            LogWriter.WriteLine("Event Trigger: {0} {1}", name, val);

            SplitEventHandler(name, val.ToString());
        }
    }
}
