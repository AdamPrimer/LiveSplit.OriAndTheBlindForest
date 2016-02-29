using System;
using System.Collections.Generic;
using System.IO;
using LiveSplit.OriAndTheBlindForest;
using LiveSplit.OriAndTheBlindForest.Memory;
using LiveSplit.OriAndTheBlindForest.Debugging;

namespace LiveSplit.OriAndTheBlindForest.State
{
    public enum SceneState
    {
        Disabling,
        Disabled,
        Loading,
        LoadingCancelled,
        Loaded
    }

    public enum GameState
    {
        Logos,
        StartScreen,
        TitleScreen,
        InGame,
        WatchCutscenes,
        TrialEnd,
        Prologue
    }

    public enum SwimState
    {
        Out,
        Surface,
        UnderMoving,
        UnderIdle
    }

    public enum RunState
    {
        Run,
        Jog,
        Walk
    }

    public struct Split
    {
        public string name;
        public string value;
        public bool doSplit;
        public Split(string name, string value, bool doSplit = true) {
            this.name = name;
            this.value = value;
            this.doSplit = doSplit;
        }
    }

    public struct Scene
    {
        public string name { get; set; }
        public bool hasStartBeenCalled { get; set; }
        public int state { get; set; }

        public override string ToString() {
            return name + " - " + state;
        }
    }

    public struct Area
    {
        public string name { get; set; }
        public decimal progress { get; set; }
        public bool current { get; set; }

        public override string ToString() {
            return (string.IsNullOrEmpty(name) ? "N/A" : name) + " - " + progress.ToString("0.00") + "%";
        }
    }

    public class OriState
    {
        public OriMemory oriMemory;
        public OriTriggers oriTriggers;

        public float posX = 0;
        public float posY = 0;

        public bool isOpen = false;
        public bool inGame = false;

        public DateTime lostSein = DateTime.Now;
        public Decimal sMapCompletion = 0;
        public List<Area> sAreas = new List<Area>();
        public Area sCurrentArea;
        public Scene[] sActiveScenes = new Scene[0];
        public Dictionary<string, object> sState = new Dictionary<string, object>();
        public Dictionary<string, bool> sKeys = new Dictionary<string, bool>();
        public Dictionary<string, bool> sEvents = new Dictionary<string, bool>();
        public Dictionary<string, bool> sAbilities = new Dictionary<string, bool>();

        public delegate Dictionary<string, bool> GetGeneric(Dictionary<string, int> inbound);
        public delegate void OnChangeGeneric(string key, bool val);

        public int exceptionsCaught = 0;
        public int totalExceptionsCaught = 0;

        public OriState() {
            oriMemory = new OriMemory();
            oriTriggers = new OriTriggers();
        }

        public void Reset() {
            List<string> keys;

            this.posX = 0;
            this.posY = 0;
            this.sMapCompletion = 0;
            sAreas.Clear();
            this.exceptionsCaught = 0;
            this.totalExceptionsCaught = 0;

            this.lostSein = DateTime.Now;
            this.sActiveScenes = new Scene[0];

            keys = new List<string>(sKeys.Keys);
            foreach (var key in keys) { sKeys[key] = false; }
            keys = new List<string>(sEvents.Keys);
            foreach (var key in keys) { sEvents[key] = false; }
            keys = new List<string>(sAbilities.Keys);
            foreach (var key in keys) { sAbilities[key] = false; }

            oriTriggers.ResetAll();
        }

        public void Dispose() {
            // Unhook from reading the game memory
            oriMemory.Dispose();
        }

        public void UpdateSplits(List<Split> splits) {
            oriTriggers.SetSplits(splits);
        }

        public void UpdateAutoStart(bool autoStart) {
            oriTriggers.SetAutoStart(autoStart);
        }

        public void UpdateAutoReset(bool autoReset) {
            oriTriggers.SetAutoReset(autoReset);
        }

        public void Loop() {
            try {
                bool isNowOpen = (oriMemory.HookProcess() && !oriMemory.proc.HasExited);

                if (isNowOpen != isOpen) {
                    if (!isNowOpen) {
                        inGame = false;
                        LogWriter.WriteLine("ori.exe is unavailable.");
                    } else {
                        LogWriter.WriteLine("ori.exe is available.");
                    }
                    isOpen = isNowOpen;
                }

                if (isOpen) Pulse();
            } catch (Exception e) {
                if (this.exceptionsCaught < 10 && this.totalExceptionsCaught < 30) {
                    this.exceptionsCaught++;
                    this.totalExceptionsCaught++;
                    LogWriter.WriteLine("Exception #{0}: {1}", this.exceptionsCaught, e.ToString());
                } else if (this.totalExceptionsCaught < 30) {
                    LogWriter.WriteLine("Too many exceptions, rebooting autosplitter.");
                    this.oriMemory.Dispose();
                    this.oriMemory = new OriMemory();
                    this.exceptionsCaught = 0;
                } else if (this.totalExceptionsCaught == 30) {
                    LogWriter.WriteLine("Too many total exceptions, no longer logging them.");
                    this.totalExceptionsCaught++;
                }
            }
        }

        public void Pulse() {
            GameState state = (GameState)oriMemory.GetGameState();

            bool isInGame = CheckInGame(state);
            bool isInGameWorld = CheckInGameWorld(state);
            bool isStartingGame = CheckStartingNewGame(state);

            UpdateInGame(isInGame);
            UpdateStartGame(isStartingGame);
            UpdateScenes();

            if (isInGameWorld) {
                UpdateMap();
                int pSein = oriMemory.GetSein();
                if (pSein != 0 && (DateTime.Now >= lostSein)) {
                    UpdatePosition();
                    UpdateEvents();
                    UpdateAbilities();
                    UpdateSein();
                } else if (pSein == 0) {
                    lostSein = DateTime.Now.AddMilliseconds(500);
                }
            }

            oriMemory.ClearPointerCache();
        }

        public void UpdateGeneric(Dictionary<string, int> inbound, Dictionary<string, bool> outbound, GetGeneric getFunc, OnChangeGeneric changeFunc) {
            Dictionary<string, bool> results = getFunc(inbound);
            foreach (var pair in results) {
                bool old_val = false;
                bool val = pair.Value;

                if (outbound.ContainsKey(pair.Key)) {
                    old_val = outbound[pair.Key];
                }

                if (val != old_val) {
                    outbound[pair.Key] = val;
                    changeFunc(pair.Key, val);
                }
            }
        }

        public void UpdateSein() {
            Dictionary<string, object> n = new Dictionary<string, object>() { };

            n["Energy Max"] = oriMemory.GetSeinEnergy("Max");
            n["Health Max"] = oriMemory.GetSeinHealth<int>("Max");
            n["Soul Flame"] = oriMemory.GetSeinHasSoulFlame();
            n["Ability Cells"] = oriMemory.GetSeinInventory("Skill Points");
            n["Deaths"] = oriMemory.GetDeathsCount();
            n["Level"] = oriMemory.GetSeinLevel("Current");
            n["Key Stones"] = oriMemory.GetSeinInventory("Keystones");
            n["Map %"] = oriMemory.GetTotalMapCompletion();

            //n["Soul Flames Cast"]  = oriMemory.GetSeinSoulFlame<Int32>("Number of Soul Flames Cast");

            //n["Is Gliding"]        = oriMemory.GetSeinAbilityState<Boolean>("Is Gliding");
            //n["Is Grabbing Block"] = oriMemory.GetSeinAbilityState<Boolean>("Is Grabbing Block");
            //n["Is Grabbing Wall"]  = oriMemory.GetSeinAbilityState<Boolean>("Is Grabbing Wall");
            //n["Is Crouching"]      = oriMemory.GetSeinAbilityState<Boolean>("Is Crouching");
            //n["Is Bashing"]        = oriMemory.GetSeinAbilityState<Boolean>("Is Bashing");

            //n["Map Stones"]        = oriMemory.GetSeinInventory("Map Stones");

            //n["Skill Points"]      = oriMemory.GetSeinLevel("Skill Points");
            //n["Experience"]        = oriMemory.GetSeinLevel("Experience");

            //n["Has Died"]          = oriMemory.GetSeinDamage<Boolean>("Died");
            //n["Immortal"]          = oriMemory.GetSeinDamage<Boolean>("Is Immortal");

            //n["Swim State"]        = oriMemory.GetSeinAbilityState<Int32>("Swim State");
            //n["Run State"]         = oriMemory.GetSeinAbilityState<Int32>("Run State");

            //n["In Game Time"]      = oriMemory.GetGameTime();

            //n["Position X"]        = oriMemory.GetPosition(0);
            //n["Position Y"]        = oriMemory.GetPosition(1);

            //n["Energy Current"]    = oriMemory.GetSeinEnergy("Current");
            //n["Health Current"]    = oriMemory.GetSeinHealth<Single>("Current");

            foreach (var pair in n) {
                if (!sState.ContainsKey(pair.Key)) {
                    sState[pair.Key] = pair.Value;
                }

                if (!sState[pair.Key].Equals(n[pair.Key])) {
                    oriTriggers.OnGenericChange(pair.Key, pair.Value, sState[pair.Key]);
                    sState[pair.Key] = pair.Value;
                }
            }
        }

        public void UpdateScenes() {
            Scene[] arr1 = oriMemory.GetScenes();
            Scene[] arr2 = sActiveScenes;

            if (arr1.Length != arr2.Length) {
                oriTriggers.OnActiveScenesChange(arr1, arr2);
                sActiveScenes = arr1;
                return;
            }

            for (var i = 0; i < arr1.Length; i++) {
                if (!(arr1[i].name == arr2[i].name && arr1[i].state == arr2[i].state)) {
                    oriTriggers.OnActiveScenesChange(arr1, arr2);
                    sActiveScenes = arr1;
                    break;
                }
            }
        }

        public void UpdateMap() {
            Area[] areas = oriMemory.GetMapCompletion();
            if (areas.Length == 0) return;

            List<Area> newAreas = new List<Area>();
            decimal mapCompletion = 0;
            foreach (var area in areas) {
                mapCompletion += area.progress;
                newAreas.Add(area);
                if (area.current) {
                    sCurrentArea = area;
                }
            }
            sAreas = newAreas;

            mapCompletion = Math.Round((decimal)mapCompletion / areas.Length, 2, MidpointRounding.AwayFromZero);

            if (mapCompletion != sMapCompletion) {
                oriTriggers.OnMapCompletionChange(areas, sMapCompletion);
                sMapCompletion = mapCompletion;
            }
        }

        public void UpdateEvents() {
            UpdateGeneric(OriMemory.keys, sKeys, oriMemory.GetKeys, oriTriggers.OnKeyChange);
            UpdateGeneric(OriMemory.events, sEvents, oriMemory.GetEvents, oriTriggers.OnWorldEventChange);
        }

        public void UpdateAbilities() {
            UpdateGeneric(OriMemory.abilities, sAbilities, oriMemory.GetAbilities, oriTriggers.OnAbilityChange);
        }

        public void UpdatePosition() {
            Vector2 pos = oriMemory.GetCameraTargetPosition();
            if (pos.X != posX || pos.Y != posY) {
                posX = pos.X;
                posY = pos.Y;
                oriTriggers.OnPositionChange(pos);
            }
        }

        public void UpdateInGame(bool newInGame) {
            if (newInGame != inGame) {
                inGame = newInGame;
                oriTriggers.OnInGameChange(inGame);
            }
        }

        public void UpdateStartGame(bool isStartingGame) {
            if (isStartingGame) {
                oriTriggers.OnStartGame(isStartingGame);
                sMapCompletion = 0;
                sCurrentArea = default(Area);
                sAreas.Clear();
            }
        }

        public bool CheckInGame(GameState state) {
            return (state != GameState.Logos && state != GameState.StartScreen && state != GameState.TitleScreen);
        }

        public bool CheckInGameWorld(GameState state) {
            return (CheckInGame(state) && !CheckEnteringGame() && state != GameState.Prologue);
        }

        public bool CheckStartingNewGame(GameState state) {
            return (state == GameState.Prologue);
        }

        public bool CheckEnteringGame() {
            // One of these two flags will be true during the transition from the Save Slot menu into the actual game.
            return oriMemory.GetGameInfo("Is Loading Old") || oriMemory.GetGameInfo("Is Loading New");
        }
    }
}
