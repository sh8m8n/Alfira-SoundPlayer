using GlobalHotKey;
using System.Windows.Input;
using System.IO;

namespace Alfira.MVVM.Model
{
    public static class SoundFileConverter
    {
        /// <summary>
        /// Создает закодированное название файла из параметров звука 
        /// </summary>
        /// <param name="name">Название (без расширения)</param>
        /// <param name="hotkey"></param>
        /// <param name="Volume"></param>
        /// <returns></returns>
        public static string CodeSoundFile(string name, HotKey hotkey, int Volume)
        {
            return $"{name}-{(int)hotkey.Key}-{(int)hotkey.Modifiers}-{Volume}";
        }

        /// <summary>
        /// Декодирует параметры звука из закодированного названия
        /// </summary>
        /// <param name="fileName">Закодированное название (без расширения)</param>
        /// <param name="name">Название звука</param>
        /// <param name="hotkey"></param>
        /// <param name="Volume"></param>
        public static void DecodeFile(string fileName, out string name,
            out Key key, out ModifierKeys modKeys, out int Volume)
        {
            string[] file = fileName.Split('-');

            name = file[0];

            key = (Key)int.Parse(file[1]);
            modKeys = (ModifierKeys)int.Parse(file[2]);

            Volume = int.Parse(file[3]);
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
        public static void DecodeFile(string fileName, out int volume)
        {
            string[] file = fileName.Split('-');

            volume = int.Parse(file[3]);
        }
    }
}
