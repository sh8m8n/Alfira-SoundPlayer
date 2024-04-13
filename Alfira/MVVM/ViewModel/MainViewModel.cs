using Alfira.MVVM.Model;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Alfira.MVVM.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private SoundManager soundManager = new SoundManager();

        public ReadOnlyObservableCollection<Sound> Sounds => soundManager.Sounds;

        public RelayCommand CloseCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand<object> DeleteCommand { get; set; }

        public MainViewModel()
        {
            CloseCommand = new RelayCommand(OnApplicationClose);
            DeleteCommand = new RelayCommand<object>(DeleteSound);  
        }

        private void OnApplicationClose()
        {
            soundManager.Dispose();
            Application.Current.Shutdown();
        }

        public void AddSound()
        {

        }

        private void DeleteSound(object parameter)
        {
            soundManager.RemoveSound(parameter as Sound);
        }
    }
}
