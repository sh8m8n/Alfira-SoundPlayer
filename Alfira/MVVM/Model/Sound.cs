using GlobalHotKey;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Alfira.MVVM.Model
{
    public class Sound : HotKey
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public int Volume { get; set; }

        /// <summary>
        /// Используется для создания нового звука
        /// </summary>
        /// <param name="path">Путь существующего звукового файла</param>
        /// <param name="soundsFolder">Папка в котрую будет закодирован звук</param>
        /// <param name="name">Название звука</param>
        /// <param name="volume">Громкость</param>
        public Sound(string path, DirectoryInfo soundsFolder, string name, Key key, ModifierKeys modifiers, int volume)
        {
            // : base(key, modifiers) заменитель
            Key = key;
            Modifiers = modifiers;

            Name = name;
            Volume = volume;

            //Расширение чтобы потом добавить обработку mp3
            FileInfo oldFile = new FileInfo(path);
            string ext = oldFile.Extension;

            FilePath = Path.Combine(soundsFolder.FullName, CodeSoundFile() + ext);
        }

        /// <summary>
        /// Используется для уже созданных ранее звуков
        /// </summary>
        /// <param name="file">Путь файла (с уже закодированным именем)</param>
        public Sound(FileInfo file)
        {
            string[] soundProps = Path.GetFileNameWithoutExtension(file.Name).Split('-');

            // : base(key, modifiers) заменитель
            Key = (Key)int.Parse(soundProps[1]);
            Modifiers = (ModifierKeys)int.Parse(soundProps[2]);

            Name = soundProps[0];
            FilePath = file.FullName;
            Volume = int.Parse(soundProps[3]);
        }

        /// <summary>
        /// Создает закодированное название файла из параметров звука 
        /// </summary>
        /// <returns></returns>
        private string CodeSoundFile()
        {
            return $"{Name}-{(int)Key}-{(int)Modifiers}-{Volume}";
        }
    }
}
