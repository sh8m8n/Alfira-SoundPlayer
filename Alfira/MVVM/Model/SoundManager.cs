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
        /// Сохраняет файлы и настройки в проводник
        /// </summary>
        private void SaveData()
        {
            File.WriteAllText(soundsData.FullName, JsonSerializer.Serialize(sounds));
        }
        
        /// <summary>
        /// Копирует файл из указанного пути и добавляет в коллекцию звуков
        /// </summary>
        /// <param name="path">Существующий файл</param>
        /// <param name="name">Название</param>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <param name="volume"></param>
        public void AddSound(string path, string name, Key key, ModifierKeys modifiers, int volume)
        {
            FileInfo soundFile = new FileInfo(path);
            string newPath = Path.Combine(soundsDirectory.FullName, name + soundFile.Extension);
            soundFile.CopyTo(newPath);

            Sound sound = new Sound(newPath, name, key, modifiers, volume);
            hotKeyManager.Register(sound);
            sounds.Add(sound);
        }

        /// <summary>
        /// Удаляет звук из коллекции
        /// </summary>
        /// <param name="sound"></param>
        public void RemoveSound(Sound sound)
        {
            File.Delete(sound.FilePath);
            sounds.Remove(sound);
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
