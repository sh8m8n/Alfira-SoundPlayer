using System.IO;
using GlobalHotKey;

namespace Alfira.MVVM.Model
{
    public class SoundInfo
    {
        public string FilePath { get; private set; }

        public string Name { get; private set; }
        public HotKey HotKey { get; private set; }
        public int Volume { get; private set; }

        public SoundInfo(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            SoundFileConverter.DecodeFile(path, out string name, out HotKey hotKey, out int volume);
            Name = name;
            HotKey = hotKey;
            Volume = volume;
        }

        /// <summary>
        /// Создает закодированное название файла из параметров звука 
        /// </summary>
        /// <param name="name">Название (без расширения)</param>
        /// <param name="hotkey"></param>
        /// <param name="Volume"></param>
        /// <returns></returns>
        public static string Code(string name, HotKey hotkey, int Volume)
        {
            return $"{name}-{(int)hotkey.Key}-{(int)hotkey.Modifiers}-{Volume}";
        }
    }
}
