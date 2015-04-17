using LiveSplit.OriAndTheBlindForest;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.OriAndTheBlindForest
{
    class OriComponent : LogicComponent
    {
        public override string ComponentName
        {
            get { return "Ori and the Blind Forest Auto Splitter"; }
        }

        //private int pulseSpeedMilliseconds = 100;

        private bool useInGame = true;
        private bool usePosition = true;
        private bool useEvents = false;  // Not recommended. Do not appear until after save, also slow to scan signature.
        
        public float posX = 0;
        public float posY = 0;
        public bool allowStart = true;
        public bool inLoading = true;
        public bool inGame = false;
        
        public OriMemory oriMemory;

        protected TimerModel Model { get; set; }

        public OriComponent()
        {
            oriMemory = new OriMemory();
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (Model == null)
            {
                Model = new TimerModel() { CurrentState = state };
                state.OnStart += OnStart;
                state.OnReset += OnReset;
                state.OnPause += OnPause;
            }

            oriMemory.HookProcess();

            if ((posX == 0 && posY == 1 && !inLoading) || !oriMemory.isHooked || oriMemory.isExited || oriMemory.proc.HasExited)
            {
                write("No Ori.exe");
                inLoading = true;
                inGame = false;
                return;
            }

            Pulse();
        }

        private void write(string str)
        {
            StreamWriter wr = new StreamWriter("test.log", true);
            wr.WriteLine("[" + DateTime.Now + "] " + str);
            wr.Close();
        }

        public void OnPause(object sender, EventArgs e)
        {
            write("[LiveSplit] Pause.");
        }

        public void OnStart(object sender, EventArgs e)
        {
            write("[LiveSplit] Start.");
            inGame = true;
            inLoading = false;
            allowStart = false;
        }

        public void OnReset(object sender, TimerPhase e)
        {
            write("[LiveSplit] Reset.");
            allowStart = true;
        }

        public void Pulse()
        {
            bool inGame = false;
            if (useInGame)
            {
                bool isInGame = oriMemory.GetInGame();
                write(isInGame.ToString() + " " + oriMemory.proc.Id + " " + oriMemory.proc.HasExited);
                if (inLoading && isInGame == false) inLoading = false;

                //
                // The InGame == True event will trigger on Game Exit, as it unloads the memory used to 
                // check if the save slot screen is active. It is recommended that splits are never reset 
                // automatically,  and that the state of this autosplitter remain fixed until a reset event 
                // is called manually, and only then resuming autosplitting duties.
                //
                // There may be room for autosplitting if the END_GAME trigger is never met.
                //

                if (!inLoading)
                {
                    inGame = isInGame;
                    UpdateInGame(inGame);
                }
            }

            if (Model.CurrentState.CurrentPhase == TimerPhase.Running && !useInGame || inGame)
            {
                if (usePosition)
                {
                    float posX = oriMemory.GetPosition(0);
                    float posY = oriMemory.GetPosition(1);
                    UpdatePosition(posX, posY);
                }

                if (useEvents)
                {
                    //oriMemory.PrintEvents();
                }
            }
        }

        public void UpdatePosition(float x, float y)
        {
            if (x != posX || y != posY)
            {
                posX = x;
                posY = y;
                OnPositionChange();
            }
        }

        public void UpdateInGame(bool newInGame)
        {
            if (newInGame != inGame)
            {
                inGame = newInGame;
                OnInGameChange();
            }
        }

        public void OnPositionChange()
        {
            write("[OriSplitter] Position: (" + posX.ToString() + "," + posY.ToString() + ")");
        }

        public void OnInGameChange()
        {
            if (inGame && allowStart)
            {
                write("[OriSplitter] Start timing.");
                allowStart = false;
                Model.Start();
            }
        }

        public void OnSplitConditionMet()
        {
            write("[OriSplitter] Split.");
            Model.Split();
        }

        public void OnGameEnd()
        {
            write("[OriSplitter] Final Split.");
            Model.Split();
        }

        public Dictionary<string, int> axes = new Dictionary<string, int>()
        {
            {"x",   0},
            {"y",   1},
        };

        public Dictionary<string, int> keys = new Dictionary<string, int>()
        {
            {"Water Vein",   0},
            {"Gumon Seal",   1},
            {"Sunstone",     2},
        };

        public Dictionary<string, int> events = new Dictionary<string, int>()
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

        /*
        public Dictionary<string, int> abilities = new Dictionary<string, int>()
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
        */

        public override void Dispose()
        {

        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return null;
        }

        public override void SetSettings(XmlNode settings)
        {
           
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return document.CreateElement("Settings");
        }
    }
}
