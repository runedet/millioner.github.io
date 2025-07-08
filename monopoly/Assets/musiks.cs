using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class musiks : MonoBehaviour
{
    public AudioMixer NewAudioMixer;
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "merrycraisler.txt");
    }

    void Start()
    {
        LoadVolume();
    }

    void LoadVolume()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string volumeStr = File.ReadAllText(savePath);
                float volume = float.Parse(volumeStr);
                
                // Если значение 0, устанавливаем -80 дБ, иначе используем lerp от -20 до 20
                float decibelValue = volume == 0 ? -80f : Mathf.Lerp(-20f, 20f, volume);
                NewAudioMixer.SetFloat("volume", decibelValue);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при чтении файла громкости: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Файл громкости не найден: " + savePath);
        }
    }

    void Update()
    {
        
    }
}