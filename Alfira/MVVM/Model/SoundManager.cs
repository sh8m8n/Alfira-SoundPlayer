using GlobalHotKey;
using NAudio.Wave;
using System;
using System.IO;
using System.Windows.Input;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace Alfira.MVVM.Model
{
    public class SoundManager : IDisposable
    {
        private readonly ObservableCollection<Sound> sounds =
            JsonSerializer.Deserialize<ObservableCollection<Sound>>(File.ReadAllText("sounds.json"));
        public readonly ReadOnlyObservableCollection<Sound> Sounds;

        private AudioFileReader currentAudioFile;
        private WaveOutEvent outputDevice = new WaveOutEvent() { DeviceNumber = -1 };

        private DirectoryInfo soundsDirectory = new DirectoryInfo("Sounds");

        private HotKeyManager hotKeyManager;

        private FileInfo soundsData = new FileInfo("sounds.json");

        public SoundManager()
        {
            Sounds = new ReadOnlyObservableCollection<Sound>(sounds);

            //Хоткеи
            hotKeyManager = new HotKeyManager();
            hotKeyManager.KeyPressed += OnHotKeyPressed;
            foreach (var sound in sounds)
                hotKeyManager.Register(sound);

            //Naudio
            outputDevice.PlaybackStopped += OnPlaybackStopped;

            SetOutputDevice(6); //Удалить этот костыль по возможности
        }


        private void OnHotKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
            }

            Sound sound = e.HotKey as Sound;

            currentAudioFile = new AudioFileReader(sound.FilePath) { Volume = (float)sound.Volume / 100};
            outputDevice.Init(currentAudioFile);
            outputDevice.Play();
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            currentAudioFile.Dispose();
        }

        /// <summary>
        /// Сохраняет звуки в json
        /// </summary>
        private void SaveData()
        {
            File.WriteAllText(soundsData.FullName, JsonSerializer.Serialize(sounds));
        }
        
        /// <summary>
        /// Копирует файл из указанного пути и добавляет в коллекцию звуков
        /// </summary>
        /// <param name="path">Существующий файл</param>
        /// <param name="name">Название звука</param>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <param name="volume">Громкость</param>
        public void AddSound(string path, string name, Key key, ModifierKeys modifiers, int volume)
        {
            FileInfo soundFile = new FileInfo(path);
            string newPath = Path.Combine(soundsDirectory.FullName, name + soundFile.Extension);

            soundFile.CopyTo(newPath);

            Sound sound = new Sound(newPath, name, key, modifiers, volume);
            hotKeyManager.Register(sound);
            sounds.Add(sound);

            SaveData();
        }

        /// <summary>
        /// Копирует файл из указанного пути, обрезает и добавляет в коллекцию звуков
        /// </summary>
        /// <param name="path">Существующий файл</param>
        /// <param name="name">Название звука</param>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <param name="volume">Громкость</param>
        /// <param name="startTime">Начало</param>
        /// <param name="endTime">Конец</param>
        public void AddSound(string path, string name, Key key, ModifierKeys modifiers, int volume, TimeSpan startTime, TimeSpan endTime)
        {
            FileInfo soundFile = new FileInfo(path);
            string newPath = Path.Combine(soundsDirectory.FullName, name + soundFile.Extension);

            if (soundFile.Extension == ".mp3")
            {
                TrimMp3File(path, newPath, startTime, endTime);
            }
            else
            {
                TrimWavFile(path, newPath, startTime, endTime);
            }

            Sound sound = new Sound(newPath, name, key, modifiers, volume);
            hotKeyManager.Register(sound);
            sounds.Add(sound);

            SaveData();
        }

        /// <summary>
        /// Удаляет звук из коллекции
        /// </summary>
        /// <param name="sound"></param>
        public void RemoveSound(Sound sound)
        {
            try
            {
                hotKeyManager.Unregister(sound);
                File.Delete(sound.FilePath);
                sounds.Remove(sound);
                SaveData();
            }
            catch { }
        }

        /// <summary>
        /// Обрезает .mp3 файл
        /// </summary>
        /// <param name="inputPath">Существующий файл</param>
        /// <param name="outputPath">Обрезанный файл</param>
        /// <param name="startTime">Начало</param>
        /// <param name="endTime">Конец</param>
        void TrimMp3File(string inputPath, string outputPath, TimeSpan startTime, TimeSpan endTime)
        {
            var reader = new Mp3FileReader(inputPath);
            var writer = File.Create(outputPath);
            
            Mp3Frame frame;
            while ((frame = reader.ReadNextFrame()) != null)
            {
                if (reader.CurrentTime >= startTime)
                {
                    if (reader.CurrentTime <= endTime)
                        writer.Write(frame.RawData, 0, frame.RawData.Length);
                    else break;
                }
            }

            reader.Dispose();
            writer.Dispose();
        }

        /// <summary>
        /// Обрезает .wav файл
        /// </summary>
        /// <param name="inPath">Существующий файл</param>
        /// <param name="outPath">Обрезанный файл</param>
        /// <param name="startTime">Начало</param>
        /// <param name="endTime">Конец</param>
        public void TrimWavFile(string inPath, string outPath, TimeSpan startTime, TimeSpan endTime)
        {
            var reader = new WaveFileReader(inPath);
            var writer = new WaveFileWriter(outPath, reader.WaveFormat);

            int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

            int startPos = (int)(startTime.TotalMilliseconds * bytesPerMillisecond);
            startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

            int endPos = (int)(endTime.TotalMilliseconds * bytesPerMillisecond);
            endPos = endPos - endPos % reader.WaveFormat.BlockAlign;

            reader.Position = startPos;
            byte[] buffer = new byte[endPos - startPos];
            reader.Read(buffer, 0, buffer.Length);
            writer.Write(buffer, 0, buffer.Length);

            reader.Dispose();
            writer.Dispose();
        }

        /// <summary>
        /// Устанавливает устройство вывода
        /// </summary>
        /// <param name="outputDeviceNumber"></param>
        public void SetOutputDevice(int outputDeviceNumber)
        {
            if (outputDeviceNumber < WaveOut.DeviceCount && outputDeviceNumber >= 0)
                outputDevice = new WaveOutEvent() { DeviceNumber =  outputDeviceNumber };
        }

        /// <summary>
        /// Возвращает доступные устройства вывода
        /// </summary>
        /// <returns>Устройства вывода</returns>
        public string[] GetOutputDevices()
        {
            int count = WaveOut.DeviceCount;
            string[] outputDevices = new string[count];

            for (int i = 0; i < count; i++)
                outputDevices[i] = $"{i}: {WaveOut.GetCapabilities(i).ProductName}";

            return outputDevices;
        }

        public void Dispose()
        {
            SaveData();
            outputDevice?.Dispose();
            currentAudioFile?.Dispose();

            hotKeyManager.Dispose();
        }
    }
}
