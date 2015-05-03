using Devil;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.OriAndTheBlindForest
{
    public class OriComponent : LogicComponent
    {
        public OriAndTheBlindForestSettings Settings { get; set; }

        public override string ComponentName {
            get { return "Ori and the Blind Forest Autosplitter"; }
        }

        public OriState oriState;

        protected TimerModel Model { get; set; }

        public OriComponent() {
            oriState = new OriState();
            Settings = new OriAndTheBlindForestSettings(this);
            oriState.oriTriggers.OnSplit += OnSplitTriggered;
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {
            if (Model == null) {
                Model = new TimerModel() { CurrentState = state };
                state.OnReset += OnReset;
                state.OnPause += OnPause;
                state.OnResume += OnResume;
                state.OnStart += OnStart;
                state.OnSplit += OnSplit;
                state.OnUndoSplit += OnUndoSplit;
                state.OnSkipSplit += OnSkipSplit;
            }

            oriState.Loop();
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public override void Dispose() {
            Settings.CloseDisplay();
        }

        public void OnSplitTriggered(object sender, OriTriggers.SplitEventArgs e) {
            if (e.name == "Start") {
                if (!oriState.oriTriggers.timerRunning && oriState.oriTriggers.autoReset) {
                    Model.Reset();
                }
                write("[OriSplitter] Start.");
                Model.Start();
            } else if (e.name == "End") {
                write("[OriSplitter] Final Split.");
                Model.Split();
            } else {
                write("[OriSplitter] Split.");
                if (oriState.oriTriggers.autoStart && !oriState.oriTriggers.timerRunning) {
                    if (oriState.oriTriggers.autoReset) {
                        Model.Reset();
                    }
                    Model.Start();
                } else { 
                    Model.Split();
                }
            }
        }

        public void OnReset(object sender, TimerPhase e) {
            write("[LiveSplit] Reset.");
            oriState.Reset();
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public void OnResume(object sender, EventArgs e) {
            write("[LiveSplit] Resume.");
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public void OnPause(object sender, EventArgs e) {
            write("[LiveSplit] Pause.");
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public void OnStart(object sender, EventArgs e) {
            write("[LiveSplit] Start.");
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public void OnUndoSplit(object sender, EventArgs e) {
            write("[LiveSplit] Undo Split.");
        }

        public void OnSkipSplit(object sender, EventArgs e) {
            write("[LiveSplit] Skip Split.");
        }

        public void OnSplit(object sender, EventArgs e) {
            write("[LiveSplit] Split.");
        }

        public override Control GetSettingsControl(LayoutMode mode) {
            return Settings;
        }

        public override void SetSettings(XmlNode settings) {
            Settings.SetSettings(settings);
            oriState.UpdateSplits(Settings.splitsState);
        }

        public override XmlNode GetSettings(XmlDocument document) {
            return Settings.GetSettings(document);
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
