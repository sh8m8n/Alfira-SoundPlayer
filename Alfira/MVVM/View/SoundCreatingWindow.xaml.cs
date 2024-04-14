using NAudio.Gui;
using System.Windows;
using System.Windows.Input;

namespace Alfira.MVVM.View
{
    public partial class SoundCreatingWindow : Window
    {
        public bool Success { get; private set; }

        public string Name { get; set; }
        public Key key { get; private set; }
        public ModifierKeys modifiers { get; private set; }
        public int Volume { get; private set; }
        public string StringHotKey { get; set; } = "Press me";

        private bool HotKeyRegisteringState = false;

        public SoundCreatingWindow(Window ownerWindow)
        {
            PreviewKeyDown += OnPreviewKeyDown;
            Owner = ownerWindow;
            InitializeComponent();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(HotKeyRegisteringState)
            {
                key = e.Key;
                modifiers = Keyboard.Modifiers;

                HotKeyRegisteringState = !HotKeyRegisteringState;
                btn_Hotkey.Content = "Reregister HotKey";
            }
        }

        private void btn_Hotkey_Click(object sender, RoutedEventArgs e)
        {
            HotKeyRegisteringState = !HotKeyRegisteringState;

            if (HotKeyRegisteringState)
                btn_Hotkey.Content = "Press HotKey";
            else
                btn_Hotkey.Content = "Reregister HotKey";
        }

        private void btn_CreateSound_Click(object sender, RoutedEventArgs e)
        {
            Name = NameTextBox.Text;
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
