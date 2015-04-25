using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using LiveSplit.OriAndTheBlindForest;

namespace Devil
{
    public class OriTriggers
    {
        public static Dictionary<string, string> defaultSplits = new Dictionary<string, string>() 
        {
            {"Iceless",                  "345.559, -109.268, 2.260, 4.781"},
            {"Gumo Fall Trap (Fall)",    "480.171, -241.454, 3.206, 4.153"},
            {"Gumo Fall Trap (Land)",    "469.506, -388.384, 17.010, 0.937"},
            {"End of Forlorn Escape",    "-1162.265, -221.822, 7.031, 3.334"},
            {"End of Horu Escape",       "162.890, 577.337, 5.216, 14.574"},
        };

        public static Dictionary<string, string> availableSplits = new Dictionary<string, string>() 
        {
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
            {"Glide",                    "Boolean"}, // Kuro's Feather
            {"Climb",                    "Boolean"},
            {"Mist Lifted",              "Boolean"}, // After Seal Reveal Cutscene
            {"Gumon Seal",               "Boolean"}, // Picked Up Seal
            {"Forlorn Ruins Entered",    "Boolean"},
            {"Forlorn Restored",         "Boolean"}, // Deposit Nightberry
            {"Wind Released",            "Boolean"}, // Start of Escape
            {"End of Forlorn Escape",    "Hitbox"},  // End of Escape
            {"Charge Jump",              "Boolean"},
            {"Sunstone",                 "Boolean"},
            {"Mount Horu Entered",       "Boolean"},
            {"R1 Into Horu Escape",      "Boolean"},
            {"Warmth Returned",          "Boolean"}, // Start of Escape
            {"End of Horu Escape",       "Hitbox"},  // End of Escape
            {"End",                      "Boolean"},

            //{"Gumo Free",                "Boolean"},

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

        public int currentSplitIdx = 0;
        public Split currentSplit;
        public Split[] sSplits;

        public event OnSplitHandler OnSplit;

        public OriTriggers() { }

        public void SetSplits(List<Split> splits) {
            sSplits = splits.ToArray();
            ResetAll();
        }

        public Split GoToNextSplit() {
            if (currentSplitIdx + 1 >= sSplits.Length) return currentSplit;

            currentSplitIdx++;
            currentSplit = sSplits[currentSplitIdx];

            return currentSplit;
        }

        public void SplitEventHandler(string name, string value) {
            if (currentSplit.name == name && currentSplit.value == value) {
                write("Trigger Function Called.");
                if (OnSplit != null) {
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

        public void OnInGameChange(bool val) { }

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
            write(string.Format("Map: {0}%", sMapCompletion));
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
                            case "mountHoruHubBottom":
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

                    write(string.Format("{0} Scene: {1} {2}", state, scene.name, scene.hasStartBeenCalled));
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
                        //Console.WriteLine("{0}: {1}", name, IGT);
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
            }
        }

        public int IncrementCounter(string name) {
            if (!counters.ContainsKey(name)) counters[name] = 0;

            counters[name]++;
            write(string.Format("Counter: {0} {1}", name, counters[name]));
            SplitEventHandler(name, counters[name].ToString());

            return counters[name];
        }

        public int SetCounter(string name, int val) {
            if (counters.ContainsKey(name) && counters[name] == val) return val;

            counters[name] = val;
            write(string.Format("Counter: {0} {1}", name, counters[name]));
            SplitEventHandler(name, val.ToString());

            return counters[name];
        }

        public void TriggerHitbox(string name, string val) {
            write(string.Format("TriggerHitbox(): {0} {1}", name, val));
            if (events.ContainsKey(name)) {
                write(string.Format("Existing: {0} {1}", name, events[name]));
                if (events[name]) return;
            }

            events[name] = true;
            write(string.Format("Current Split: {0} {1}", currentSplit.name, currentSplit.value));
            write(string.Format("Hitbox Trigger: {0} {1}", name, val));

            SplitEventHandler(name, val.ToString());
        }

        public bool WillTriggerEvent(string name, bool val) {
            return (!(events.ContainsKey(name) && events[name] == val));
        }

        public void TriggerEvent(string name, bool val) {
            write(string.Format("TriggerEvent(): {0} {1}", name, val));
            if (events.ContainsKey(name)) {
                write(string.Format("Existing: {0} {1}", name, events[name]));
            }
            if (!WillTriggerEvent(name, val)) return;

            events[name] = val;
            write(string.Format("Current Split: {0} {1}", currentSplit.name, currentSplit.value));
            write(string.Format("Event Trigger: {0} {1}", name, val));

            SplitEventHandler(name, val.ToString());
        }

        private void write(string str) {
            #if DEBUG
            StreamWriter wr = new StreamWriter("_oriauto.log", true);
            wr.WriteLine("[" + DateTime.Now + "] " + str);
            wr.Close();
            #endif
        }
    }
}
