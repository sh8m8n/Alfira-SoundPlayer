using NAudio.Gui;
using System.Windows;
using System.Windows.Input;

namespace Alfira.MVVM.View
{
    public partial class SoundCreatingWindow : Window
    {
        public bool Success { get; private set; }

        public string SoundName { get; set; }
        public Key key { get; private set; }
        public ModifierKeys modifiers { get; private set; }
        public int Volume { get; private set; }

        private bool HotKeyRegisteringState = false;

        public SoundCreatingWindow(Window ownerWindow)
        {
            PreviewKeyDown += OnPreviewKeyDown;
            Owner = ownerWindow;
            InitializeComponent();
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
            SoundName = NameTextBox.Text;
            Volume = (int)VolumeSlider.Value;
            Success = true;

            Close();
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
    }
}
