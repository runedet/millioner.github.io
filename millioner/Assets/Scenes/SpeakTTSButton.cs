using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeastSquares.Overtone
{
    public class SpeakTTSButton : MonoBehaviour
    {
        public TTSPlayer _player;
        public SaveAudioButton saveAudio;
        private bool _isSpeaking;
        public Text inputText;
        private string previousText = "";
        private bool isInitialized = false;
        private float speakCooldown = 0.5f; // Задержка между попытками озвучки
        private float lastSpeakTime;

        private async void Start()
        {
            try
            {
                if (_player == null)
                {
                    Debug.LogError("TTSPlayer reference is missing!");
                    return;
                }

                // Ждем инициализации движка
                int attempts = 0;
                while (!_player.Engine.Loaded && attempts < 50) // максимум 5 секунд ожидания
                {
                    await Task.Delay(100);
                    attempts++;
                    if (!this.gameObject.activeInHierarchy) return;
                }

                if (!_player.Engine.Loaded)
                {
                    Debug.LogError("TTS Engine failed to initialize after 5 seconds!");
                    return;
                }

                isInitialized = true;
                
                if (inputText != null)
                {
                    StartCoroutine(WatchTextChanges());
                }
                else
                {
                    Debug.LogError("Input Text reference is missing!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during initialization: {e}");
            }
        }

        private System.Collections.IEnumerator WatchTextChanges()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            
            while (true)
            {
                try
                {
                    if (inputText != null && 
                        inputText.text != previousText && 
                        !string.IsNullOrEmpty(inputText.text) &&
                        Time.time - lastSpeakTime >= speakCooldown)
                    {
                        RequestSpeak(inputText.text);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in text watch: {e}");
                }
                yield return delay;
            }
        }

        private async void RequestSpeak(string newText)
        {
            if (!CanSpeak(newText)) return;

            try
            {
                _isSpeaking = true;
                lastSpeakTime = Time.time;
                previousText = newText;

                // Создаем отмену операции через 10 секунд
                using (var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        await SpeakWithTimeout(newText, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.LogWarning("TTS operation timed out");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during TTS processing: {e}");
            }
            finally
            {
                _isSpeaking = false;
            }
        }

        private async Task SpeakWithTimeout(string text, System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                var speakTask = _player.Speak(text);
                await Task.WhenAny(speakTask, Task.Delay(-1, cancellationToken));

                if (speakTask.IsCompleted)
                {
                    await speakTask; // Получаем результат или исключение

                    if (_player.source != null && _player.source.clip != null)
                    {
                        var audioClip = _player.source.clip;
                        if (saveAudio != null)
                        {
                            saveAudio.audioClip = audioClip;
                        }
                        await Task.Delay((int)(1000 * audioClip.length));
                    }
                }
                else
                {
                    throw new TimeoutException("TTS operation timed out");
                }
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException || e is OperationCanceledException)
                {
                    Debug.LogWarning("TTS operation was cancelled");
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CanSpeak(string text)
        {
            if (!isInitialized || _isSpeaking)
            {
                return false;
            }

            if (_player == null || _player.Engine == null || !_player.Engine.Loaded)
            {
                Debug.LogWarning("TTS system is not ready");
                return false;
            }

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            return true;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void ResetSpeaker()
        {
            _isSpeaking = false;
            previousText = "";
            lastSpeakTime = 0;
        }
    }
}