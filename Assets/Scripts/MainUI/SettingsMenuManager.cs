using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject settingsPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Xóa sạch các sự kiện cũ trên Inspector (nếu có) để tránh xung đột
        if (bgmSlider != null) bgmSlider.onValueChanged.RemoveAllListeners();
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();

        // Gắn sự kiện mới
        if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Đồng bộ thanh trượt với âm lượng hiện tại ngay khi vào Scene
        SyncSliders();
    }

    // --- HÀM BẬT/TẮT BẢNG MENU ---
    public void ToggleSettingsMenu()
    {
        if (settingsPanel != null)
        {
            bool willBeActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(willBeActive);

            // QUAN TRỌNG: Mỗi lần mở bảng Menu lên, ép thanh trượt chạy về đúng vị trí
            if (willBeActive)
            {
                SyncSliders();
            }
        }
    }

    // --- HÀM ĐỒNG BỘ THANH TRƯỢT VỚI ÂM THANH THỰC TẾ ---
    private void SyncSliders()
    {
        if (SoundManager.Instance == null) return;

        // 1. Tạm thời TẮT sự kiện lắng nghe để việc UI tự di chuyển không kích hoạt hàm SetVolume
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
            bgmSlider.value = SoundManager.Instance.bgmSource.volume; // Kéo thanh trượt về đúng chỗ
            bgmSlider.onValueChanged.AddListener(SetBGMVolume); // Bật lại sự kiện
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
            sfxSlider.value = SoundManager.Instance.sfxSource.volume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    // --- XỬ LÝ NHẠC NỀN ---
    public void SetBGMVolume(float volume)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBGMVolume(volume);
    }

    // --- XỬ LÝ HIỆU ỨNG ---
    public void SetSFXVolume(float volume)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(volume);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainUI");
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}