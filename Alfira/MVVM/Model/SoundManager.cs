using GlobalHotKey;
using NAudio.Wave;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Input;

namespace Alfira.MVVM.Model
{
    public class SoundManager : IDisposable
    {
        private WaveOutEvent outputDevice = new WaveOutEvent() { DeviceNumber = -1 };
        private AudioFileReader currentAudioFile;

        private DirectoryInfo soundDirectory;

        private Dictionary<HotKey, string> sounds = new Dictionary<HotKey, string>();
        private HotKeyManager hotKeyManager;

        public SoundManager()
        {
            // Возня с проводником
            soundDirectory = new DirectoryInfo("Sounds");
            if (!soundDirectory.Exists )
                soundDirectory.Create();

            //Хоткеи
            hotKeyManager = new HotKeyManager();
            hotKeyManager.KeyPressed += HotKeyManagerPressed;
            
            //Naudio
            outputDevice.PlaybackStopped += OnPlaybackStopped;

            SetOutputDevice(6); //Удалить этот костыль по возможности

            LoadAllSounds();
        }


        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            if (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
            }

            string path = sounds[e.HotKey];
            int volume = SoundFileConverter.GetVolume(path);
            currentAudioFile = new AudioFileReader(path) { Volume = volume };
            outputDevice.Init(currentAudioFile);
            outputDevice.Play();
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            currentAudioFile.Dispose();
        }

        /// <summary>
        /// Загружает все файлы из проводника в хеш таблицу
        /// </summary>
        private void LoadAllSounds()
        {
            FileInfo[] soundFiles = soundDirectory.GetFiles();
            foreach (FileInfo file in soundFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                SoundFileConverter.DecodeFile(fileName, out HotKey hotKey);
                hotKeyManager.Register(hotKey);
                sounds.Add(hotKey, file.FullName);
            }
        }
        
        /// <summary>
        /// Копирует файл по указанному пути и добавляет в коллекцию звуков
        /// </summary>
        /// <param name="path">Существующий файл</param>
        /// <param name="name">Название</param>
        /// <param name="key"></param>
        /// <param name="modKeys"></param>
        /// <param name="Volume"></param>
        public void AddSound(string path, string name, Key key, ModifierKeys modKeys, int Volume)
        {
            HotKey hotKey = hotKeyManager.Register(key, modKeys);

            //Сохранение в проводнике
            FileInfo oldFile = new FileInfo(path);
            string ext = oldFile.Extension;
            string newPath = Path.Combine(soundDirectory.FullName,
                SoundFileConverter.CodeSoundFile(name, hotKey, Volume)+ext);
            oldFile.CopyTo(newPath, true);

            sounds.Add(hotKey, newPath);
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
            foreach (var sound in sounds)
            {
                hotKeyManager.Unregister(sound.Key);
            }

            outputDevice?.Dispose();
            currentAudioFile?.Dispose();

            hotKeyManager.Dispose();
        }
    }
}
