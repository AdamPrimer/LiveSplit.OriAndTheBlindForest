﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using Devil;
namespace LiveSplit.OriAndTheBlindForest
{
    public partial class MainWindow : Window
    {
        private OriMemory reader;
        private Rectangle rect;
        private Vector2 start;
        public Vector4 lastHitbox;

        public delegate void OnNewHitboxHandler(object sender, EventArgs e);
        public event OnNewHitboxHandler OnNewHitbox;

        public MainWindow(OriState state) {
            try {
                InitializeComponent();
                reader = state.oriMemory;
                GamesPlayed.Visibility = Visibility.Visible;

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
            MainWindow wind = new MainWindow(new OriState());
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
            var rect = reader.GetWindowBounds();
            ShowOverlay(reader.IsInForeground());

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
                GamesPlayed.Visibility = Visibility.Hidden;
            } else if (enable && !Topmost) {
                Topmost = true;
                GamesPlayed.Visibility = Visibility.Visible;
            }
        }
        public void Update() {
            UpdatePosition();
            double fontSize = Math.Max((double)Height / 50, (double)6);
            double fontSizeSmall = Math.Max((double)Height / 70, (double)6);
            Vector4 hitbox = new Vector4(0, 0, 0, 0);
            var mouse = System.Windows.Forms.Form.MousePosition;
            if (mouse.X >= Left && mouse.X < Left + Width && mouse.Y >= Top && mouse.Y < Top + Height && System.Windows.Forms.Form.MouseButtons == System.Windows.Forms.MouseButtons.Right) {
                if (rect == null) {
                    rect = new Rectangle {
                        Stroke = Brushes.Red,
                        StrokeThickness = 4
                    };
                    CanvasInfo.Children.Add(rect);
                    start = reader.ScreenToGame(new Vector2(mouse.X, mouse.Y));
                }

                Vector2 startScreen = reader.GameToScreen(start);
                if (mouse.X < startScreen.X) {
                    Canvas.SetLeft(rect, mouse.X - Left);
                    hitbox.X = mouse.X;
                } else {
                    Canvas.SetLeft(rect, startScreen.X - Left);
                    hitbox.X = startScreen.X;
                }
                if (mouse.Y < startScreen.Y) {
                    Canvas.SetTop(rect, mouse.Y - Top);
                    hitbox.Y = mouse.Y;
                } else {
                    Canvas.SetTop(rect, startScreen.Y - Top);
                    hitbox.Y = startScreen.Y;
                }
                rect.Width = Math.Abs(mouse.X - startScreen.X);
                rect.Height = Math.Abs(mouse.Y - startScreen.Y);
                hitbox.W = (float)rect.Width;
                hitbox.H = (float)rect.Height;
            } else if (rect != null) {
                CanvasInfo.Children.Remove(rect);
                rect = null;
            }
            GamesPlayed.FontSize = fontSize;
            Vector2 pos = reader.ScreenToGame(new Vector2(mouse.X, mouse.Y));
            Vector2 oripos = reader.GetPosition();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Ori: " + oripos.ToString()).AppendLine("Mouse: " + pos.ToString());
            if (rect != null) {
                Vector4 gameHitbox = reader.ScreenToGame(hitbox);
                sb.AppendLine("Hitbox: " + gameHitbox.ToString());
                lastHitbox = gameHitbox;
                if (OnNewHitbox != null) {
                    OnNewHitbox(this, new EventArgs());
                }
            }

            GamesPlayed.Text = sb.ToString();

            Canvas.SetLeft(GamesPlayed, Width - GamesPlayed.ActualWidth * 1.2);
            Canvas.SetTop(GamesPlayed, Height - GamesPlayed.ActualHeight * 1.3);
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