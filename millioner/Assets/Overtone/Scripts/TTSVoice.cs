using LeastSquares.Overtone;
using UnityEngine;
using System.IO;

namespace Assets.Overtone.Scripts
{
    public class TTSVoice : MonoBehaviour
    {
        public string voiceName;
        public int speakerId;
        private string oldVoiceName;
        private int oldSpeakerId;
        public TTSVoiceNative VoiceModel { get; private set; }
        private float checkInterval = 0.1f; // Check every 0.1 seconds
        private float nextCheckTime = 0f;
        
        void Update()
        {
            // Check voice.txt file frequently
            if (Time.time >= nextCheckTime)
            {
                CheckVoiceFile();
                nextCheckTime = Time.time + checkInterval;
            }

            if (voiceName != oldVoiceName)
            {
                oldVoiceName = voiceName;
                VoiceModel?.Dispose();
                VoiceModel = TTSVoiceNative.LoadVoiceFromResources(voiceName);
            }

            if (speakerId != oldSpeakerId)
            {
                oldSpeakerId = speakerId;
                VoiceModel.SetSpeakerId(speakerId);
            }
        }

        private void CheckVoiceFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "voice.txt");
            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath).Trim();
                if (!string.IsNullOrEmpty(fileContent))
                {
                    voiceName = fileContent;
                }
            }
        }

        void OnDestroy()
        {
            VoiceModel?.Dispose();
        }
    }
}