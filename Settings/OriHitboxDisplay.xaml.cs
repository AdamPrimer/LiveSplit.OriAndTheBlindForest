using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using LiveSplit.OriAndTheBlindForest;
using LiveSplit.OriAndTheBlindForest.State;
using LiveSplit.OriAndTheBlindForest.Memory;

namespace LiveSplit.OriAndTheBlindForest.Settings {
    public partial class OriHitboxDisplay : Window {
        private OriMemory reader;
        private Rectangle hitboxUI;
        private Vector2 start;
        public Vector4 lastHitbox = null;
        private bool isDragging;

        public delegate void OnNewHitboxHandler(object sender, EventArgs e);
        public event OnNewHitboxHandler OnNewHitbox;

        public OriHitboxDisplay(OriState state) {
            try {
                InitializeComponent();
                reader = state.oriMemory;
                OriInfo.Visibility = Visibility.Visible;

                Thread worker = new Thread(UpdateUI);
                worker.IsBackground = true;
                worker.Name = "UI Worker";
                worker.Start();
            } catch (Exception e) {
                System.Windows.MessageBox.Show("Error loading program: " + e.ToString());
                Close();
            }
        }

        [STAThread]
        public static void Main(string[] args) {
            OriHitboxDisplay wind = new OriHitboxDisplay(new OriState());
            wind.ShowDialog();
        }

        private void UpdateUI() {
            while (true) {
                try {
                    if (Visibility == System.Windows.Visibility.Visible) {
                        Dispatcher.Invoke((Action)Update);
                    }
                } catch { }
                Thread.Sleep(33);
            }
        }

        public void UpdatePosition() {
            var rect = reader.GetGameWindowBounds();
            ShowOverlay(reader.IsGameInForeground());

            //hs window has height 0 if it just launched, screwing things up if the tracker is started before hs is. 
            //this prevents that from happening. 
            if (rect.H == 0 || Visibility != Visibility.Visible) {
                SetRect(0, 0, 0, 0);
                return;
            }

            SetRect((int)rect.Y, (int)rect.X, (int)rect.W, (int)rect.H);
        }

        private bool PointInsideControl(System.Windows.Point pos, double actualWidth, double actualHeight) {
            return PointInsideControl(pos, actualWidth, actualHeight, new Thickness(0));
        }
        private bool PointInsideControl(System.Windows.Point pos, double actualWidth, double actualHeight, Thickness margin) {
            if (pos.X > 0 - margin.Left && pos.X < actualWidth + margin.Right) {
                if (pos.Y > 0 - margin.Top && pos.Y < actualHeight + margin.Bottom)
                    return true;
            }
            return false;
        }

        public void ShowOverlay(bool enable) {
            if (!enable) {
                Topmost = false;
                OriInfo.Visibility = Visibility.Hidden;
            } else if (enable && !Topmost) {
                Topmost = true;
                OriInfo.Visibility = Visibility.Visible;
            }
        }

        public void DrawRectangle(Vector4 pos) {
            if (pos.W == 0 || pos.H == 0) return;

            try {
                pos = reader.GameToScreen(pos);

                if (hitboxUI == null) {
                    hitboxUI = new Rectangle {
                        Stroke = Brushes.Red,
                        StrokeThickness = 3
                    };
                    CanvasInfo.Children.Add(hitboxUI);
                }

                hitboxUI.Visibility = OriInfo.Visibility;

                Canvas.SetLeft(hitboxUI, pos.X - Left);
                Canvas.SetTop(hitboxUI, pos.Y - Top);

                hitboxUI.Width = pos.W;
                hitboxUI.Height = pos.H;
            } catch { }
        }

        public void UndrawRectangle() {
            if (hitboxUI != null) {
                CanvasInfo.Children.Remove(hitboxUI);
                hitboxUI = null;
            }
        }

        public void Update() {
            try {
                UpdatePosition();
                double fontSize = Math.Max((double)Height / 50, (double)6);
                double fontSizeSmall = Math.Max((double)Height / 70, (double)6);
                Vector4 hitbox = new Vector4(0, 0, 0, 0);

                var mouse = System.Windows.Forms.Form.MousePosition;
                if (mouse.X >= Left && mouse.X < Left + Width && mouse.Y >= Top && mouse.Y < Top + Height &&
                        System.Windows.Forms.Form.MouseButtons == System.Windows.Forms.MouseButtons.Middle) {
                    if (isDragging == false) {
                        start = reader.ScreenToGame(new Vector2(mouse.X, mouse.Y));
                    }
                    isDragging = true;

                    Vector2 startScreen = reader.GameToScreen(start);
                    if (mouse.X < startScreen.X) {
                        hitbox.X = mouse.X;
                    } else {
                        hitbox.X = startScreen.X;
                    }
                    if (mouse.Y < startScreen.Y) {
                        hitbox.Y = mouse.Y;
                    } else {
                        hitbox.Y = startScreen.Y;
                    }

                    hitbox.W = (float)Math.Abs(mouse.X - startScreen.X);
                    hitbox.H = (float)Math.Abs(mouse.Y - startScreen.Y);

                    lastHitbox = reader.ScreenToGame(hitbox);
                    if (OnNewHitbox != null) {
                        OnNewHitbox(this, new EventArgs());
                    }
                } else {
                    isDragging = false;
                }

                DrawRectangle(lastHitbox);

                OriInfo.FontSize = fontSize;
                Vector2 pos = reader.ScreenToGame(new Vector2(mouse.X, mouse.Y));
                Vector2 oripos = reader.GetCameraTargetPosition();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Ori: " + oripos.ToString()).AppendLine("Mouse: " + pos.ToString());

                if (hitboxUI != null) {
                    sb.AppendLine("Hitbox: " + lastHitbox.ToString());
                }

                OriInfo.Text = sb.ToString();

                Canvas.SetLeft(OriInfo, Width - OriInfo.ActualWidth * 1.2);
                Canvas.SetTop(OriInfo, Height - OriInfo.ActualHeight * 1.3);
            } catch { }
        }

        private void SetRect(int top, int left, int width, int height) {
            Top = top;
            Left = left;
            Width = width;
            Height = height;
            CanvasInfo.Width = width;
            CanvasInfo.Height = height;
        }
    }
}
