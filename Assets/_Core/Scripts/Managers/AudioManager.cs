using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private List<BGAudio> bgAudioList;
    [SerializeField] private List<KratosSfx> kratosAudioList;
    [SerializeField] private List<TrollSfx> trollAudioList;

    [SerializeField] private AudioSource bgSource;
    [SerializeField] private GameObject oneshotAudioPrefab;
    [SerializeField] private AudioSource btnClickSource;

    // Private Variable
    private ObjectPool<OneShotAudio> audioPool;
    private OneShotAudio tempOneShotAudio;

    // Properties
    public AudioSource BGAudioSource { get { return bgSource; } }
    public AudioSource BtnClickSource { get { return btnClickSource; } }

    private void Awake()
    {
        // make singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        // create sfx audio pool
        audioPool = new ObjectPool<OneShotAudio>(
            () =>
            {
                Instantiate(oneshotAudioPrefab, transform.position, Quaternion.identity, transform).TryGetComponent(out tempOneShotAudio);
                return tempOneShotAudio;
            },
            (OneShotAudio audio) => { audio.gameObject.SetActive(true); },
            (OneShotAudio audio) => { audio.gameObject.SetActive(false); },
            (OneShotAudio audio) => { Destroy(audio.gameObject); },
            false, 5, 10
            );

        // start BG Audio
        PlayBGAudio(BGAudio.Name.Mainmenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) PlayBGAudio(BGAudio.Name.TrollBattle);
        else if (Input.GetKeyDown(KeyCode.M)) PlayBGAudio(BGAudio.Name.Mainmenu);
    }

    public void PlayBGAudio(BGAudio.Name name)
    {
        for (int i = 0; i < bgAudioList.Count; i++)
        {
            if (!bgAudioList[i].name.Equals(name)) continue;
            //if (bgAudioList[i].clip.Equals(bgSorce.clip)) continue;

            // play new bg
            bgSource.clip = bgAudioList[i].clip;
            bgSource.volume = bgAudioList[i].volume;
            bgSource.loop = bgAudioList[i].isLoop;
            bgSource.Play();
            break;
        }
    }

    public void PlayKratosAudioAtPoint(KratosSfx.Name name, Vector3 pos)
    {
        tempOneShotAudio = audioPool.Get();
        tempOneShotAudio.transform.position = pos;

        for (int i = 0; i < kratosAudioList.Count; i++)
        {
            if (kratosAudioList[i].name != name) continue;

            tempOneShotAudio.Source.clip = kratosAudioList[i].clip;
            tempOneShotAudio.Source.volume = kratosAudioList[i].volume;
            tempOneShotAudio.Source.pitch = kratosAudioList[i].pitch;
            tempOneShotAudio.PlayAudio();
        }
    }

    public void PlayTrollAudioAtPoint(TrollSfx.Name name, Vector3 pos)
    {
        tempOneShotAudio = audioPool.Get();
        tempOneShotAudio.transform.position = pos;

        for (int i = 0; i < trollAudioList.Count; i++)
        {
            if (trollAudioList[i].name != name) continue;

            tempOneShotAudio.Source.clip = trollAudioList[i].clip;
            tempOneShotAudio.Source.volume = trollAudioList[i].volume;
            tempOneShotAudio.Source.pitch = trollAudioList[i].pitch;
            tempOneShotAudio.PlayAudio();
        }
    }

    public void ReturnAudioToPool(OneShotAudio audio)
    {
        if (!audio) return;
        audioPool.Release(audio);
    }
}

public class BaseAudio
{
    public AudioClip clip;
    [Range(0.0f, 1.0f)] public float volume = 0.1f;
}

[System.Serializable]
public class BGAudio : BaseAudio
{
    public enum Name
    {
        Mainmenu, TrollBattle, Victory
    }
    public Name name;

    public bool isLoop = true;
}

[System.Serializable]
public class KratosSfx : BaseAudio
{
    public enum Name
    {
        ShieldOpen, ShieldClose, ShieldBlock, AxePickup, Heal, AxeThrow, AxeReturned, LAttack1, LAttack2, LAttack3, LAttack4, HAttack1, HAttack2, Dodge
    }
    public Name name;
    [Range(0.0f, 3.0f)] public float pitch = 1;
}

[System.Serializable]
public class TrollSfx : BaseAudio
{
    public enum Name
    {
        Scream, StoneHit, Kick, StoneDrop
    }
    public Name name;
    [Range(0.0f, 3.0f)] public float pitch = 1;
}

