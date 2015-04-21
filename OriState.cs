using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Devil
{
    public enum SceneState
    {
        Disabling,
        Disabled,
        Loading,
        LoadingCancelled,
        Loaded
    };

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
    }

    public class OriState
    {
        public OriMemory oriMemory;
        public OriTriggers oriTriggers;

        public float posX = 0;
        public float posY = 0;

        public bool isOpen = false;
        public bool inGame = false;

        public Decimal sMapCompletion = 0;
        public Scene[] sActiveScenes = new Scene[0];
        public Dictionary<string, object> sState = new Dictionary<string, object>();
        public Dictionary<string, bool> sKeys = new Dictionary<string, bool>();
        public Dictionary<string, bool> sEvents = new Dictionary<string, bool>();
        public Dictionary<string, bool> sAbilities = new Dictionary<string, bool>();

        public delegate Dictionary<string, bool> GetGeneric(Dictionary<string, int> inbound);
        public delegate void OnChangeGeneric(string key, bool val);

        public OriState() {
            oriMemory = new OriMemory();
        }

        public void InitializeTriggers(List<Split> splits, OriTriggers.OnSplitTriggered func) {
            oriTriggers = new OriTriggers(splits, func);
        }

        public void Loop() {
            bool isNowOpen = (oriMemory.HookProcess() && !oriMemory.proc.HasExited);

            if (isNowOpen != isOpen) {
                if (!isNowOpen) {
                    inGame = false;
                    Console.WriteLine("ori.exe is unavailable.");
                } else {
                    Console.WriteLine("ori.exe is available.");
                }
                isOpen = isNowOpen;
            }
            if (isOpen) {
                //Memory.Counter(true);
                Pulse();
                //Console.WriteLine("ReadProcessMemory Count: {0}", Memory.Counter(false));
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
                UpdateEvents();
                UpdateAbilities();
                UpdateSein();
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

            n["Energy Max"]        = oriMemory.GetSeinEnergy("Max");
            n["Health Max"]        = oriMemory.GetSeinHealth<Int32>("Max");
            n["Soul Flame"]        = oriMemory.GetSeinHasSoulFlame();
            n["Ability Cells"]     = oriMemory.GetSeinInventory("Skill Points");
            n["Deaths"]            = oriMemory.GetDeathsCount();

            //n["Soul Flames Cast"]  = oriMemory.GetSeinSoulFlame<Int32>("Number of Soul Flames Cast");

            //n["Is Gliding"]        = oriMemory.GetSeinAbilityState<Boolean>("Is Gliding");
            //n["Is Grabbing Block"] = oriMemory.GetSeinAbilityState<Boolean>("Is Grabbing Block");
            //n["Is Grabbing Wall"]  = oriMemory.GetSeinAbilityState<Boolean>("Is Grabbing Wall");
            //n["Is Crouching"]      = oriMemory.GetSeinAbilityState<Boolean>("Is Crouching");
            //n["Is Bashing"]        = oriMemory.GetSeinAbilityState<Boolean>("Is Bashing");

            //n["Key Stones"]        = oriMemory.GetSeinInventory("Keystones");
            //n["Map Stones"]        = oriMemory.GetSeinInventory("Map Stones");

            //n["Skill Points"]      = oriMemory.GetSeinLevel("Skill Points");
            //n["Experience"]        = oriMemory.GetSeinLevel("Experience");
            //n["Level"]             = oriMemory.GetSeinLevel("Current");

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
            
            Decimal mapCompletion = 0;
            foreach (var area in areas) {
                mapCompletion += area.progress;
            }
            mapCompletion = Math.Round((Decimal)mapCompletion / areas.Length, 2, MidpointRounding.AwayFromZero);
            
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

        public void UpdatePosition(float x, float y) {
            if (x != posX || y != posY) {
                posX = x;
                posY = y;
                oriTriggers.OnPositionChange(posX, posY);
            }
        }

        public void UpdateInGame(bool newInGame) {
            if (newInGame != inGame) {
                inGame = newInGame;
                oriTriggers.OnInGameChange(inGame);
            }
        }

        public void UpdateStartGame(bool isStartingGame) {
            if (isStartingGame == true) {
                oriTriggers.OnStartGame(isStartingGame);
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
