using Alfira.MVVM.Model;
using Alfira.MVVM.View;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

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

            //Выбор файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = ".wav only | *.wav";

            bool? success = openFileDialog.ShowDialog();
            if (success == true)
                filepath = openFileDialog.FileName;
            else
                return;

            //Конфигурация звука

            SoundCreatingWindow creatingwindow = new SoundCreatingWindow(window);
            creatingwindow.ShowDialog();

            if(creatingwindow.Success == true)
            {
                name = creatingwindow.Name;
                key = creatingwindow.key;
                modifiers = creatingwindow.modifiers;
            }

            //soundManager.AddSound(filepath, name, key, modifiers, volume);
        }

        private void DeleteSound(object parameter)
        {
            soundManager.RemoveSound(parameter as Sound);
        }
    }
}
