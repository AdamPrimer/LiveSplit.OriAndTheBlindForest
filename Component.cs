using LiveSplit.Model;
using LiveSplit.OriAndTheBlindForest.Debugging;
using LiveSplit.OriAndTheBlindForest.Settings;
using LiveSplit.OriAndTheBlindForest.State;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.OriAndTheBlindForest
{
    public class OriComponent : IComponent
    {
        public OriAndTheBlindForestSettings Settings { get; set; }
        private InfoTextComponent textInfo;

        public string ComponentName {
            get { return "Ori and the Blind Forest Autosplitter"; }
        }

        public OriState oriState;

        protected TimerModel Model { get; set; }

        public float PaddingTop { get { return Settings.showMapDisplay ? textInfo.PaddingTop : 0; } }
        public float PaddingLeft { get { return Settings.showMapDisplay ? textInfo.PaddingLeft : 0; } }
        public float PaddingBottom { get { return Settings.showMapDisplay ? textInfo.PaddingBottom : 0; } }
        public float PaddingRight { get { return Settings.showMapDisplay ? textInfo.PaddingRight : 0; } }

        public IDictionary<string, Action> ContextMenuControls { get { return null; } }

        public OriComponent() {
            textInfo = new InfoTextComponent("0%", "Swamp 0.00%");
            oriState = new OriState();
            Settings = new OriAndTheBlindForestSettings(this);
            oriState.oriTriggers.OnSplit += OnSplitTriggered;
        }

        private void PrepareDraw(LiveSplitState state, LayoutMode mode) {
            textInfo.DisplayTwoRows = true;

            textInfo.NameLabel.HasShadow = textInfo.ValueLabel.HasShadow = state.LayoutSettings.DropShadows;
            textInfo.NameLabel.HorizontalAlignment = StringAlignment.Far;
            textInfo.ValueLabel.HorizontalAlignment = StringAlignment.Far;
            textInfo.NameLabel.VerticalAlignment = StringAlignment.Near;
            textInfo.ValueLabel.VerticalAlignment = StringAlignment.Near;
            textInfo.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            textInfo.ValueLabel.ForeColor = state.LayoutSettings.TextColor;
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) {
            if (Settings.showMapDisplay) {
                if (state.LayoutSettings.BackgroundColor.ToArgb() != Color.Transparent.ToArgb()) {
                    g.FillRectangle(new SolidBrush(state.LayoutSettings.BackgroundColor), 0, 0, width, VerticalHeight);
                }
                PrepareDraw(state, LayoutMode.Vertical);
                textInfo.DrawVertical(g, state, width, clipRegion);
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) {
            if (Settings.showMapDisplay) {
                if (state.LayoutSettings.BackgroundColor.ToArgb() != Color.Transparent.ToArgb()) {
                    g.FillRectangle(new SolidBrush(state.LayoutSettings.BackgroundColor), 0, 0, HorizontalWidth, height);
                }
                PrepareDraw(state, LayoutMode.Horizontal);
                textInfo.DrawHorizontal(g, state, height, clipRegion);
            }
        }

        public float VerticalHeight {
            get { return Settings.showMapDisplay ? textInfo.VerticalHeight : 0; }
        }

        public float MinimumWidth {
            get { return Settings.showMapDisplay ? textInfo.MinimumWidth : 0; }
        }

        public float HorizontalWidth {
            get { return Settings.showMapDisplay ? textInfo.HorizontalWidth : 0; }
        }

        public float MinimumHeight {
            get { return Settings.showMapDisplay ? textInfo.MinimumHeight : 0; }
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {
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

            if (Settings.showMapDisplay) {
                textInfo.InformationName = "Total Map: " + oriState.sMapCompletion.ToString("0.00") + "%";
                textInfo.InformationValue = oriState.sCurrentArea.ToString();
                textInfo.LongestString = "Valley Of The Wind - 100.00%";
                textInfo.Update(invalidator, state, width, height, mode);
                if (invalidator != null) {
                    invalidator.Invalidate(0, 0, width, height);
                }
            }
        }

        public void Dispose() {
            Settings.CloseDisplay();
        }

        public void OnSplitTriggered(object sender, OriTriggers.SplitEventArgs e) {
            if (e.name == "Start") {
                if (!oriState.oriTriggers.timerRunning && oriState.oriTriggers.autoReset) {
                    Model.Reset();
                }
                LogWriter.WriteLine("[OriSplitter] Start.");
                Model.Start();
            } else if (e.name == "End") {
                LogWriter.WriteLine("[OriSplitter] Final Split.");
                Model.Split();
            } else {
                LogWriter.WriteLine("[OriSplitter] Split.");
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
            LogWriter.WriteLine("[LiveSplit] Reset.");
            oriState.Reset();
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public void OnResume(object sender, EventArgs e) {
            LogWriter.WriteLine("[LiveSplit] Resume.");
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public void OnPause(object sender, EventArgs e) {
            LogWriter.WriteLine("[LiveSplit] Pause.");
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public void OnStart(object sender, EventArgs e) {
            LogWriter.WriteLine("[LiveSplit] Start.");
            oriState.oriTriggers.timerRunning = (Model.CurrentState.CurrentPhase == TimerPhase.Running);
        }

        public void OnUndoSplit(object sender, EventArgs e) {
            LogWriter.WriteLine("[LiveSplit] Undo Split.");
        }

        public void OnSkipSplit(object sender, EventArgs e) {
            LogWriter.WriteLine("[LiveSplit] Skip Split.");
        }

        public void OnSplit(object sender, EventArgs e) {
            LogWriter.WriteLine("[LiveSplit] Split.");
        }

        public Control GetSettingsControl(LayoutMode mode) {
            return Settings;
        }

        public void SetSettings(XmlNode settings) {
            Settings.SetSettings(settings);
            oriState.UpdateSplits(Settings.splitsState);
        }

        public XmlNode GetSettings(XmlDocument document) {
            return Settings.GetSettings(document);
        }
    }
}
