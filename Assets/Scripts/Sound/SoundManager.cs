using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Background Music Clips")]
    public AudioClip bgmMain_Mode1_Mode4; // Nhạc chung cho Main UI, Mode 1, Mode 4
    public AudioClip bgmMode2;            // Nhạc riêng cho Mode 2
    public AudioClip bgmMode3;            // Nhạc riêng cho Mode 3

    [Header("SFX Clips (Kéo thả file âm thanh vào đây)")]
    public AudioClip correctClip;   // Âm thanh chọn ĐÚNG
    public AudioClip wrongClip;     // Âm thanh chọn SAI
    public AudioClip completeClip;  // Âm thanh HOÀN THÀNH mode
    public AudioClip loseClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolume();

        // Không tự động Play ở đây nữa, ta sẽ dùng lệnh gọi từ các Manager
    }

    // --- QUẢN LÝ VOLUME MẶC ĐỊNH CỦA BẠN ---
    public void LoadVolume()
    {
        if (bgmSource != null) bgmSource.volume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        if (sfxSource != null) sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    // --- QUẢN LÝ NHẠC NỀN (BGM) ---
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;

        // Nếu bài nhạc yêu cầu ĐÚNG LÀ bài đang được gắn vào source
        if (bgmSource.clip == clip)
        {
            // Nếu nó đang bị pause (ví dụ vừa đóng bảng Win), thì cho hát tiếp
            if (!bgmSource.isPlaying) bgmSource.UnPause();
            // Nếu đang hát rồi thì return, giúp nhạc chuyển scene LIỀN MẠCH
            return;
        }

        // Nếu là bài khác thì đổi bài và hát từ đầu
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // CÁC HÀM GỌI NHANH BGM CHO TỪNG MODE
    public void PlayBGM_Main14() => PlayBGM(bgmMain_Mode1_Mode4);
    public void PlayBGM_Mode2() => PlayBGM(bgmMode2);
    public void PlayBGM_Mode3() => PlayBGM(bgmMode3);

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void PauseBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying) bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        // Dừng nhạc Win (nếu nó còn đang kêu dở)
        if (sfxSource != null)
        {
            sfxSource.Stop();
            sfxSource.loop = false; // QUAN TRỌNG: Tắt loop đi để các tiếng Đúng/Sai sau này không bị lặp lại
            sfxSource.clip = null;  // Xóa clip để dọn dẹp
        }

        // Phát tiếp nhạc nền
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.UnPause();
        }
    }

    // --- HÀM PHÁT HIỆU ỨNG CHUNG ---
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // --- CÁC HÀM GỌI SFX ---
    public void PlayCorrectSound() => PlaySFX(correctClip);
    public void PlayWrongSound() => PlaySFX(wrongClip);
    public void PlayLoseSound() => PlaySFX(loseClip);

    // ĐÃ SỬA: Phát nhạc Win và TẠM DỪNG nhạc nền
    public void PlayCompleteSound()
    {
        PauseBGM(); // Ẩn nhạc nền đi

        if (sfxSource != null && completeClip != null)
        {
            sfxSource.clip = completeClip;
            sfxSource.loop = true; // BẬT lặp lại cho riêng nhạc Win
            sfxSource.Play();      // Dùng Play() bình thường để nó nhận lệnh Loop
        }
    }
}