using NAudio.Gui;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Alfira.MVVM.View
{
    public partial class SoundCreatingWindow : Window
    {
        public bool Success { get; private set; }

        public string SoundName { get; set; }
        public Key key { get; private set; }
        public ModifierKeys modifiers { get; private set; }
        public int Volume { get; private set; }
        public int StartTrim { get; private set; }
        public int EndTrim { get; private set; }

        private bool HotKeyRegisteringState = false;
        private double soundLength;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ownerWindow"></param>
        /// <param name="soundRender">sound picture</param>
        /// <param name="soundLength">milliseconds</param>
        public SoundCreatingWindow(Window ownerWindow, BitmapSource soundRender, int soundLength)
        {
            PreviewKeyDown += OnPreviewKeyDown;
            Owner = ownerWindow;
            this.soundLength = soundLength;
            EndTrim = soundLength;

            InitializeComponent();

            Image_soundRender.Source = soundRender;
            VolumeSlider.Value = 100;
            StartSlider.Maximum = soundLength;
            FinishSLider.Maximum = soundLength;
            FinishSLider.Value = soundLength;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                modifiers = modifiers | ModifierKeys.Alt;
            }

            else if(HotKeyRegisteringState
                    && e.Key != Key.System
                    && e.Key != Key.LWin && e.Key != Key.RWin
                    && e.Key != Key.LeftAlt && e.Key != Key.RightAlt
                    && e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl
                    && e.Key != Key.LeftShift && e.Key != Key.RightShift)
            {
                key = e.Key;
                modifiers = modifiers | Keyboard.Modifiers;

                HotKeyRegisteringState = !HotKeyRegisteringState;
                btn_Hotkey.Content = "Reregister HotKey";
                TextBlock_HotKey.Text = $"{modifiers} + {key}";
            }
        }

        private void btn_Hotkey_Click(object sender, RoutedEventArgs e)
        {
            key = Key.None;
            modifiers = ModifierKeys.None;

            HotKeyRegisteringState = !HotKeyRegisteringState;

            if (HotKeyRegisteringState)
                btn_Hotkey.Content = "Press HotKey";
            else
                btn_Hotkey.Content = "Reregister HotKey";

            TextBlock_HotKey.Text = "???";
        }

        private void btn_CreateSound_Click(object sender, RoutedEventArgs e)
        {
            if(!ValidateData())
                return;

            SoundName = NameTextBox.Text;
            Volume = (int)VolumeSlider.Value;
            Success = true;

            Close();
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(NameTextBox.Text)) return false;
            if (key == Key.None) return false;
            if (modifiers == ModifierKeys.None) return false;

            return true;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = (int)VolumeSlider.Value;
            VolumeTextBox.Text = value.ToString();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void StartSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(StartSlider.Value >= FinishSLider.Value)
            {
                StartSlider.Value = FinishSLider.Value - 1;
            }

            LeftBorder.Width = StartSlider.Value * Image_soundRender.ActualWidth / soundLength;
            StartTrim = (int)StartSlider.Value;
        }

        private void FinishSLider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(FinishSLider.Value <= StartSlider.Value)
            {
                FinishSLider.Value = StartSlider.Value + 1;
            }

            RightBorder.Width = (soundLength - FinishSLider.Value) * Image_soundRender.ActualWidth / soundLength;
            EndTrim = (int)FinishSLider.Value;
        }
    }
}
