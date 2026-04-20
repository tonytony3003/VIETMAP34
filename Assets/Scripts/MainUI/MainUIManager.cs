using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Tên chính xác của Scene Mode 1 trong Build Settings")]
    public string mode1SceneName = "Mode1";

    [Tooltip("Tên chính xác của Scene Mode 2 trong Build Settings")]
    public string mode2SceneName = "Mode2";

    [Tooltip("Tên chính xác của Scene Mode 3 trong Build Settings")]
    public string mode3SceneName = "Mode3";

    [Tooltip("Tên chính xác của Scene Sinh Tồn")]
    public string survivalSceneName = "Mode4";
    [Header("Buttons")]
    public Button btnMode1;
    public Button btnMode2;
    public Button btnMode3;
    public Button btnMode4;

    [Header("Lock Icons (Tùy chọn)")]
    public GameObject lockIconMode2;
    public GameObject lockIconMode3;
    public GameObject lockIconMode4;
    /// <summary>
    /// Chế độ 1: Nhận biết tỉnh mới (sau sát nhập)
    /// </summary>
    public void OpenMode1()
    {
        LoadTargetScene(mode1SceneName);
    }

    /// <summary>
    /// Chế độ 2: Địa giới tiếp giáp
    /// </summary>
    public void OpenMode2()
    {
        LoadTargetScene(mode2SceneName);
    }

    /// <summary>
    /// Chế độ 3: Hợp nhất tỉnh cũ (truy tìm nguồn gốc)
    /// </summary>
    public void OpenMode3()
    {
        LoadTargetScene(mode3SceneName);
    }

    /// <summary>
    /// Chế độ 4: Chế độ Sinh Tồn (Tổng hợp các mode)
    /// </summary>
    public void OpenSurvivalMode()
    {
        LoadTargetScene(survivalSceneName);
    }

    /// <summary>
    /// Thoát ứng dụng
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Đang thoát trò chơi...");
        Application.Quit();
    }

    // Hàm bổ trợ để kiểm tra tên scene trước khi load
    private void LoadTargetScene(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            SceneManager.LoadScene(name);
        }
        else
        {
            Debug.LogError("Chưa nhập tên Scene trong Inspector!");
        }
    }
    void Start()
    {
        // DÒNG NÀY SẼ XÓA SẠCH DỮ LIỆU CŨ MỖI KHI BẬT GAME
        //PlayerPrefs.DeleteKey("UnlockedMode");

        // Sau khi xóa xong thì mới chạy lệnh kiểm tra UI
        CheckUnlockedModes();
    }

    public void CheckUnlockedModes()
    {
        // Mặc định biến "UnlockedMode" sẽ là 1 (chỉ mở Mode 1) nếu người chơi mới vào lần đầu
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedMode", 1);

        // Mode 1 luôn mở
        btnMode1.interactable = true;

        // Bật/Tắt các nút dựa trên tiến trình đã lưu
        btnMode2.interactable = unlockedLevel >= 2;
        btnMode3.interactable = unlockedLevel >= 3;
        btnMode4.interactable = unlockedLevel >= 4;

        // Tùy chọn: Ẩn/hiện icon Ổ Khóa đè lên nút
        if (lockIconMode2) lockIconMode2.SetActive(unlockedLevel < 2);
        if (lockIconMode3) lockIconMode3.SetActive(unlockedLevel < 3);
        if (lockIconMode4) lockIconMode4.SetActive(unlockedLevel < 4);
    }

    // Nút dùng để test game (Reset dữ liệu)
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("UnlockedMode");
        CheckUnlockedModes(); // Cập nhật lại UI ngay lập tức
        Debug.Log("Đã xóa toàn bộ tiến trình!");
    }
}