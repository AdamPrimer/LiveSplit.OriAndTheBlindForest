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

namespace LiveSplit.UI.Components
{
    public class OriComponent : LogicComponent
    {
        public OriAndTheBlindForestSettings Settings { get; set; }

        public override string ComponentName {
            get { return "Ori and the Blind Forest Auto Splitter"; }
        }

        public Devil.OriState oriState;

        protected TimerModel Model { get; set; }

        public OriComponent() {
            Settings = new OriAndTheBlindForestSettings(this);
            oriState = new Devil.OriState();
            oriState.oriTriggers.OnSplit += OnSplit;
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {
            if (Model == null) {
                Model = new TimerModel() { CurrentState = state };
                state.OnReset += OnReset;
            }

            oriState.Loop();
        }

        public void OnReset(object sender, TimerPhase e) {
            write("[LiveSplit] Reset.");
            oriState.Reset();
        }

        // if (Model.CurrentState.CurrentPhase == TimerPhase.Running && !useInGame || inGame)

        public void OnSplit(object sender, Devil.OriTriggers.SplitEventArgs e) {
            if (e.name == "Start") {
                write("[OriSplitter] Start.");
                Model.Start();
            } else if (e.name == "End") {
                write("[OriSplitter] Final Split.");
                Model.Split();
            } else {
                write("[OriSplitter] Split.");
                Model.Split();
            }
        }

        public override void Dispose() { }

        public override Control GetSettingsControl(LayoutMode mode) {
            write("GetSettingsControl();");
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
            StreamWriter wr = new StreamWriter("test.log", true);
            wr.WriteLine("[" + DateTime.Now + "] " + str);
            wr.Close();
        }
    }
}
