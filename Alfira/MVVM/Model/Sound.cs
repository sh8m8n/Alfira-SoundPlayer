using GlobalHotKey;
using System.Windows.Input;

namespace Alfira.MVVM.Model
{
    public class Sound : HotKey
    {
        public string Name { get; set; }
        public string Hotkey => $" {Modifiers}+{Key} ";
        public int Volume { get; set; }
        public string FilePath { get; }

        public Sound(string filePath, string name, Key key, ModifierKeys modifiers, int volume)
            : base(key, modifiers)
        {
            Name = name;
            FilePath = filePath;
            Volume = volume;
        }
    }
}
