using UnityEngine;

[System.Serializable]
public class NamedAudioSource
{
    public string name;
    public AudioSource audioSource;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource levelMusic, gameOverMusic, gameWinMusic, changeSceneMusic;

    public NamedAudioSource[] sfx;

    private void Awake()
    {
        instance = this;
    }

    public void PlayGameOver()
    {
        levelMusic.Stop();
        gameOverMusic.Play();
    }

    public void PlayGameWin()
    {
        levelMusic.Stop();
        gameWinMusic.Play();
    }

    public void PlayChangeScene()
    {
        levelMusic.Stop();
        changeSceneMusic.Play();
    }

    // Method to play an SFX by name
    public void PlaySFX(string sfxName)
    {
        NamedAudioSource soundEffect = System.Array.Find(sfx, s => s.name == sfxName);
        if (soundEffect != null)
        {
            //soundEffect.audioSource.Stop();
            soundEffect.audioSource.Play();
        }
        else
        {
            Debug.LogWarning("SFX with name " + sfxName + " does not exist.");
        }
    }

    // Method to stop an SFX by name
    public void StopSFX(string sfxName)
    {
        NamedAudioSource soundEffect = System.Array.Find(sfx, s => s.name == sfxName);
        if (soundEffect != null)
        {
            soundEffect.audioSource.Stop();
        }
        else
        {
            Debug.LogWarning("SFX with name " + sfxName + " does not exist.");
        }
    }
}