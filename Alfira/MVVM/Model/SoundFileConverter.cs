using GlobalHotKey;
using System.Windows.Input;

namespace Alfira.MVVM.Model
{
    public static class SoundFileConverter
    {
        public static string CodeSoundFile(string name, HotKey hotkey, int Volume)
        {
            return $"{name}-{(int)hotkey.Key}-{(int)hotkey.Modifiers}-{Volume}";
        }

        public static void DecodeFile(string fileName, out string name, out HotKey hotkey, out int Volume)
        {
            string[] file = fileName.Split('-');

            name = file[0];

            Key key = (Key)int.Parse(file[1]);
            ModifierKeys modKeys = (ModifierKeys)int.Parse(file[2]);
            hotkey = new HotKey(key, modKeys);

            Volume = int.Parse(file[3]);
        }

        public static void DecodeFile(string fileName, out string name, out HotKey hotkey)
        {
            string[] file = fileName.Split('-');

            name = file[0];

            Key key = (Key)int.Parse(file[1]);
            ModifierKeys modKeys = (ModifierKeys)int.Parse(file[2]);
            hotkey = new HotKey(key, modKeys);
        }
        public static void DecodeFile(string fileName, out HotKey hotkey)
        {
            string[] file = fileName.Split('-');

            Key key = (Key)int.Parse(file[1]);
            ModifierKeys modKeys = (ModifierKeys)int.Parse(file[2]);
            hotkey = new HotKey(key, modKeys);
        }
    }
}
