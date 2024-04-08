using Alfira.MVVM.Model;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfira.MVVM.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private SoundManager SoundManager = new SoundManager();

        public ReadOnlyObservableCollection<Sound> Sounds => SoundManager.Sounds;

        public RelayCommand CloseCommand;

        public MainViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        private void Close()
        {
            SoundManager.Dispose();
        }
    }
}
