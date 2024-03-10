using GlobalHotKey;
using NAudio.Wave;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Input;
using System.Collections.Specialized;

namespace Alfira.MVVM.Model
{
    public class SoundManager : IDisposable
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader currentAudioFile;

        private DirectoryInfo soundDirectory;

        private Dictionary<HotKey, string> sounds;
        private HotKeyManager hotKeyManager;

        public SoundManager()
        {
            soundDirectory = new DirectoryInfo("Sounds");
            if (!soundDirectory.Exists )
                soundDirectory.Create();

            sounds = new Dictionary<HotKey, string>();

            hotKeyManager = new HotKeyManager();
            hotKeyManager.KeyPressed += HotKeyManagerPressed;

            outputDevice = new WaveOutEvent() { DeviceNumber = 6 };
            outputDevice.PlaybackStopped += OnPlaybackStopped;

            RefreshAllSounds();

            //AddSound("C:\\Users\\Шаман\\Desktop\\sound.wav", "Баребух", 
            //Key.F6, ModifierKeys.Alt | ModifierKeys.Shift,100);
            AddSound("C:\\Users\\Шаман\\Desktop\\sad.wav", "Баребух",
                Key.S, ModifierKeys.Alt | ModifierKeys.Shift,100);
            AddSound("C:\\Users\\Шаман\\Desktop\\fortnitedeath.wav", "Баребух",
                Key.D, ModifierKeys.Alt | ModifierKeys.Shift, 100);
            AddSound("C:\\Users\\Шаман\\Desktop\\aaa.wav", "Баребух",
                Key.A, ModifierKeys.Alt | ModifierKeys.Shift, 100);
            AddSound("C:\\Users\\Шаман\\Desktop\\potion.wav", "Баребух",
                Key.P, ModifierKeys.Alt | ModifierKeys.Shift, 100);
            AddSound("C:\\Users\\Шаман\\Desktop\\Freddy.wav", "Баребух",
                Key.F, ModifierKeys.Alt | ModifierKeys.Shift, 100);
            AddSound("C:\\Users\\Шаман\\Desktop\\kratos.wav", "Баребух",
                Key.K, ModifierKeys.Alt | ModifierKeys.Shift, 100);
            AddSound("C:\\Users\\Шаман\\Desktop\\hui.wav", "Баребух",
                Key.H, ModifierKeys.Alt | ModifierKeys.Shift, 100);
        }


        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            if (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
            }

            currentAudioFile = new AudioFileReader(sounds[e.HotKey]);
            outputDevice.Init(currentAudioFile);
            outputDevice.Play();
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            currentAudioFile.Dispose();
        }

        private void RefreshAllSounds()
        {
            sounds.Clear();
            FileInfo[] soundFiles = soundDirectory.GetFiles();
            foreach (FileInfo file in soundFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                SoundFileConverter.DecodeFile(fileName, out HotKey hotKey);
                hotKeyManager.Register(hotKey);
                sounds.Add(hotKey, file.FullName);
            }
        }

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
        /// Sets output device
        /// </summary>
        /// <param name="outputDeviceNumber"></param>
        public void SetOutputDevice(int outputDeviceNumber)
        {
            if (outputDeviceNumber < WaveOut.DeviceCount && outputDeviceNumber >= 0)
                outputDevice = new WaveOutEvent() { DeviceNumber = outputDeviceNumber };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Output devices names</returns>
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
