using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip musicClip;

    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private int defaultPoolSize = 16;
    [SerializeField] private int maxPoolSize = 64;

    [SerializeField] AudioClip[] deathSounds;
    [SerializeField] AudioClip hitSound;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private float sfxSound = 1f;

    private ObjectPool<AudioSource> sfxPool;

    private void Awake()
    {
          instance = this;
          InitializePool();

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);

        ChangeMusicVolume(musicSlider.value);
        ChangeSFXVolume(sfxSlider.value);
    }
    public void ChangeMusicVolume(float volume) 
    {
        musicSource.volume = volume;
    }
    public void ChangeSFXVolume(float volume)
    {
        sfxSound = volume;
    }

    private void InitializePool()
    {
        sfxPool = new ObjectPool<AudioSource>(
            createFunc: CreatePooledAudioSource,
            actionOnGet: OnGetAudioSource,
            actionOnRelease: OnReleaseAudioSource,
            actionOnDestroy: OnDestroyAudioSource,
            collectionCheck: true,
            defaultCapacity: defaultPoolSize,
            maxSize: maxPoolSize
        );
        PlayMusic(musicClip, 0.2f);
    }

    private AudioSource CreatePooledAudioSource()
    {
        GameObject go;

        if (audioSourcePrefab != null)
        {
            go = Instantiate(audioSourcePrefab, transform);
        }
        else
        {
            go = new GameObject("PooledAudioSource");
            go.transform.SetParent(transform);
            go.AddComponent<AudioSource>();
        }

        var src = go.GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f; 
        go.SetActive(false);
        return src;
    }

    private void OnGetAudioSource(AudioSource src)
    {
        src.gameObject.SetActive(true);
    }

    private void OnReleaseAudioSource(AudioSource src)
    {
        src.Stop();
        src.clip = null;
        src.gameObject.SetActive(false);
    }

    private void OnDestroyAudioSource(AudioSource src)
    {
        if (src != null)
            Destroy(src.gameObject);
    }


    public void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
        musicSource.clip = null;
    }


    
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        var src = sfxPool.Get();
        StartCoroutine(PlayAndRelease(src, clip, null, volume, pitch, spatial: 0f));
    }
    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, float spatialBlend = 1f)
    {
        if (clip == null) return;
        var src = sfxPool.Get();
        StartCoroutine(PlayAndRelease(src, clip, position, volume, pitch, spatialBlend));
    }

    public void PlayHitSound(float volume = 1f, float pitch = 1f)
    {
        var src = sfxPool.Get();
        StartCoroutine(PlayAndRelease(src, hitSound, null, volume, pitch, spatial: 0f));

    }
    public void PlayRandomDeathSound(float volume = 1f, float pitch = 1f)
    {
        var src = sfxPool.Get();
        StartCoroutine(PlayAndRelease(src, deathSounds[Random.Range(0,deathSounds.Length)], null, volume, pitch, spatial: 0f));

    }
    private IEnumerator PlayAndRelease(AudioSource src, AudioClip clip, Vector3? position, float volume, float pitch, float spatial)
    {
        src.clip = clip;
        src.volume = Mathf.Clamp01(volume * sfxSound);
        src.pitch = Mathf.Max(0.01f, pitch);
        src.spatialBlend = Mathf.Clamp01(spatial);

        if (position.HasValue)
        {
            src.transform.position = position.Value;
        }
        else
        {
            src.transform.localPosition = Vector3.zero;
        }

        src.Play();

        float waitTime = (clip.length / src.pitch) + 0.02f;
        yield return new WaitForSeconds(waitTime);

        if (sfxPool != null)
            sfxPool.Release(src);
    }

    public AudioSource PlayLoopingWhile(AudioClip clip, Func<bool> condition, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || condition == null) return null;

        var src = sfxPool.Get();
        StartCoroutine(LoopWhileCondition(src, clip, condition, volume, pitch));
        return src;
    }

    private IEnumerator LoopWhileCondition(AudioSource src, AudioClip clip, Func<bool> condition, float volume, float pitch)
    {
        src.clip = clip;
        src.volume = Mathf.Clamp01(volume);
        src.pitch = Mathf.Max(0.01f, pitch);
        src.loop = true;

        src.Play();

        while (condition())
        {
            yield return null;
        }

        src.loop = false;
        src.Stop();

        if (sfxPool != null)
            sfxPool.Release(src);
    }
}
