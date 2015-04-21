using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Devil
{
    public class OriTriggers
    {
        public delegate void OnSplitTriggered(string key);

        public Decimal mapCompletion = 0;
        public string inGameTime = "";
        public Dictionary<string, int> counters = new Dictionary<string, int>();
        public Dictionary<string, bool> events = new Dictionary<string, bool>()
        {
            {"Start", false},
            {"Soul Flame", false},
            {"Spirit Flame", false},
            {"Wall Jump", false},
            {"Charge Flame", false},
            {"Double Jump", false},
            {"Gumo Free", false},
            {"Water Vein", false},
            {"Ginso Tree Entered", false},
            {"Bash", false},
            {"Clean Water", false},
            {"Stomp", false},
            {"Glide", false},
            {"Sunstone", false},
            {"Mount Horu Entered", false},
            {"Warmth Returned", false},
            {"End", false},

            {"Charge Jump", false},
            {"Climb", false},
            {"Spirit Tree Reached", false},
            {"Gumon Seal", false},
            {"Mist Lifted", false},
            {"Forlorn Restored", false},
            {"Wind Released", false},
            {"Double Jump Upgrade", false},
        };

        public int currentSplitIdx = 0;
        public Split currentSplit;
        public Split[] sSplits;

        public OnSplitTriggered triggerFunc;

        public OriTriggers(List<Split> splits, OnSplitTriggered func) {
            write("Loaded Triggers");
            sSplits = splits.ToArray();
            if (sSplits.Length > 0) {
                currentSplit = sSplits[currentSplitIdx];
            }
            foreach (var split in splits) {
                write(split.name);
            }

            triggerFunc = func;
        }

        public Split GoToNextSplit() {
            if (currentSplitIdx + 1 >= sSplits.Length) return currentSplit;

            currentSplitIdx++;
            currentSplit = sSplits[currentSplitIdx];

            return currentSplit;
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

            Console.WriteLine("All Triggers Reset.");
        }

        public void OnStartGame(bool val) {
            if (WillTriggerEvent("Start", val)) {
                ResetAll();
                TriggerEvent("Start", val);
            }
        }

        public void OnInGameChange(bool val) { }
        public void OnPositionChange(float posX, float posY) { }

        public void OnKeyChange(string key, bool val) {
            TriggerEvent(key, val);
        }

        public void OnWorldEventChange(string key, bool val) {
            TriggerEvent(key, val);
        }

        public void OnAbilityChange(string key, bool val) {
            switch (key) {
                case "Bash":
                case "Stomp":
                case "Charge Jump":
                case "Double Jump":
                case "Charge Flame":
                case "Wall Jump":
                case "Climb":
                case "Glide":
                case "Spirit Flame":
                case "Double Jump Upgrade":
                    TriggerEvent(key, val);
                    break;
            }
        }

        public void OnMapCompletionChange(Area[] val, Decimal sMapCompletion) {
            mapCompletion = sMapCompletion;
            Console.WriteLine("Map: {0}%", sMapCompletion);
        }

        public void OnActiveScenesChange(Scene[] val, Scene[] old) {
            foreach (var scene in val) {
                if (old == null || !Array.Exists(old, delegate(Scene s) { return s.name.Equals(scene.name) && s.state == scene.state; })) {
                    string state = "";
                    if ((SceneState)scene.state == SceneState.Loaded) {
                        state = "Loaded";
                        switch (scene.name) {
                            case "creditsScreen":
                                TriggerEvent("End", true);
                                break;
                            case "ginsoEntranceIntro":
                                TriggerEvent("Ginso Tree Entered", true);
                                break;
                            case "mountHoruHubBottom":
                                TriggerEvent("Mount Horu Entered", true);
                                break;
                        }
                    } else if ((SceneState)scene.state == SceneState.Disabling) {
                        state = "Disabling";
                    } else if ((SceneState)scene.state == SceneState.Loading) {
                        state = "Loading";
                    } else if ((SceneState)scene.state == SceneState.LoadingCancelled) {
                        state = "Loading Cancelled";
                    } else if ((SceneState)scene.state == SceneState.Disabled) {
                        state = "Disabled";
                    }

                    Console.WriteLine("{0} Scene: {1}", state, scene.name);
                }
            }
        }

        public void OnGenericChange(string name, object val, object old) {
            switch (name) {
                case "Soul Flame":
                    TriggerEvent("Soul Flame", (bool)val);
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

                case "Energy Max":
                    SetCounter("Energy Cells", (int)Math.Floor((float)val));
                    SetCounter("Energy Max", (int)Math.Floor((float)val));
                    break;

                case "Health Max":
                    SetCounter("Health Max", (int)val);
                    SetCounter("Health Cells", (int)val / 4 - 3);
                    break;

                case "Ability Cells":
                    SetCounter(name, (int)val);
                    break;

                case "Level":
                    SetCounter(name, (int)Math.Floor((float)val));
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

                case "Energy Current":
                case "Health Current":
                case "Skill Points":
                case "Experience":
                case "Key Stones":
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
            if (!counters.ContainsKey(name)) {
                counters[name] = 0;
            }
            counters[name]++;
            Console.WriteLine("Counter: {0} {1}", name, counters[name]);
            return counters[name];
        }

        public int SetCounter(string name, int val) {
            if (counters.ContainsKey(name) && counters[name] == val) return val;

            counters[name] = val;
            Console.WriteLine("Counter: {0} {1}", name, counters[name]);
            return counters[name];
        }

        public bool WillTriggerEvent(string name, bool val) {
            return (!(events.ContainsKey(name) && events[name] == val));
        }

        public void TriggerEvent(string name, bool val) {
            if (!WillTriggerEvent(name, val)) return;

            if (currentSplit.name == name && currentSplit.value == val.ToString()) {
                write("Splitable Event!");
                triggerFunc(name);
                GoToNextSplit();
            }

            events[name] = val;
            write(string.Format("Current Split: {0} {1}", currentSplit.name, currentSplit.value));
            write(string.Format("Event Trigger: {0} {1}", name, val));
        }

        private void write(string str) {
            StreamWriter wr = new StreamWriter("test.log", true);
            wr.WriteLine("[" + DateTime.Now + "] " + str);
            wr.Close();
        }
    }
}
