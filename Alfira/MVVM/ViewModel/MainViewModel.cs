using Alfira.MVVM.Model;
using Alfira.MVVM.View;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NAudio.Wave;
using NAudio.WaveFormRenderer;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Alfira.MVVM.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private Window window;

        private SoundManager soundManager = new SoundManager();

        public ReadOnlyObservableCollection<Sound> Sounds => soundManager.Sounds;

        public RelayCommand CloseCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand<object> DeleteCommand { get; set; }

        public MainViewModel(Window window)
        {
            this.window = window;

            CloseCommand = new RelayCommand(OnApplicationClose);
            AddCommand = new RelayCommand(AddSound);
            DeleteCommand = new RelayCommand<object>(DeleteSound);  
        }

        private void OnApplicationClose()
        {
            soundManager.Dispose();
            Application.Current.Shutdown();
        }

        public void AddSound()
        {
            string filepath;
            string name;
            Key key;
            ModifierKeys modifiers;
            int volume;

            //Выбор Файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = ".mp3 | *.mp3; | .wav | *.wav";

            bool? success = openFileDialog.ShowDialog();
            if (success == true)
                filepath = openFileDialog.FileName;
            else
                return;

            //Рендер графического представления звука
            WaveFormRenderer renderer = new WaveFormRenderer();
            AudioFileReader audioFileReader = new AudioFileReader(filepath);

            var settings = new StandardWaveFormRendererSettings()
            {
                BackgroundColor = Color.Transparent,
                Width = 330,
                TopHeight = 18,
                BottomHeight = 18,
                BottomPeakPen = new Pen(Color.White),
            };

            Bitmap SoundRenderBitmap = new Bitmap(renderer.Render(audioFileReader, settings));


            BitmapSource soundRenderBitMap = Imaging.CreateBitmapSourceFromHBitmap(SoundRenderBitmap.GetHbitmap(),
                System.IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            int soundLength = (int)audioFileReader.TotalTime.TotalMilliseconds;

            //Конфигурация звука
            SoundCreatingWindow creatingwindow = new SoundCreatingWindow(window, soundRenderBitMap, soundLength);
            creatingwindow.ShowDialog();

            if(creatingwindow.Success == true)
            {
                name = creatingwindow.SoundName;
                key = creatingwindow.key;
                modifiers = creatingwindow.modifiers;
                volume = creatingwindow.Volume;

                if( creatingwindow.StartTrim == 0 &&  creatingwindow.EndTrim == soundLength )
                {
                    soundManager.AddSound(filepath, name, key, modifiers, volume);
                }
                else
                {
                    soundManager.AddSound(filepath, name, key, modifiers, volume, TimeSpan.FromMilliseconds(creatingwindow.StartTrim),
                        TimeSpan.FromMilliseconds(creatingwindow.EndTrim));
                }
            }
        }

        private void DeleteSound(object parameter)
        {
                soundManager.RemoveSound(parameter as Sound);
        }

        private void SwitchTheme(string themeName)
        {

            ResourceDictionary newTheme = new ResourceDictionary();
            newTheme.Source = new Uri($".//Themes/{themeName}Theme.xaml", UriKind.Relative);

            ResourceDictionary themeManager = Application.Current.Resources["ThemeManager"] as ResourceDictionary;
            themeManager.MergedDictionaries.Clear();
            themeManager.MergedDictionaries.Add(newTheme);
        }
    }
}
