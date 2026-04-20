using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance;

    [Header("Data")]
    public List<ProvinceData> allProvinces;
    public ProvinceData currentTarget;
    private int currentIndex = 0;

    public bool isMode3 = false; // Biến để xác định chế độ chơi (1 hay 3)
    public bool isMode4 = false; // Biến để xác định chế độ chơi (1 hay 3)
    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }
    void Start()
    {
        /*if (isMode4)
        {
            return;
        }
        */
        if (SoundManager.Instance != null && (isMode4||!isMode3)) SoundManager.Instance.PlayBGM_Main14();
        if (SoundManager.Instance != null && isMode3) SoundManager.Instance.PlayBGM_Mode3();
        allProvinces.Shuffle(); // Xáo trộn danh sách tỉnh để mỗi lần chơi sẽ có thứ tự khác nhau
        SetNewQuestion();
    }
    public void SetNewQuestion()
    {
        if (isMode4)
        {
            if (!isMode3)
            {
                Debug.Log("Đang ở Mode 1, cập nhật câu hỏi sáp nhập");
                UIManager.Instance.UpdateQuestionText("Đâu là " + currentTarget.provinceName + "?");
            }
            else
            {
                UIManager.Instance.UpdateQuestionText("Các tỉnh dưới đây sáp nhập thành tỉnh mới nào ?");
                UIManager.Instance.ShowGallery(currentTarget);
            }
        }
        else
        {
        if (currentIndex < allProvinces.Count)
        {
            Debug.Log("currentIndex=" + currentIndex + "allProvinces.Count=" + allProvinces.Count);
            currentTarget = allProvinces[currentIndex];
            // Cập nhật câu hỏi lên UI
            if (UIManager.Instance != null)
            {
                if (!isMode3)
                {
                    UIManager.Instance.UpdateQuestionText("Đâu là " + currentTarget.provinceName + "?");
                }
                else
                {
                    UIManager.Instance.UpdateQuestionText("Các tỉnh dưới đây sáp nhập thành tỉnh mới nào ?");
                    UIManager.Instance.ShowGallery(currentTarget);
                }

            }
        }
        else
        {
            Debug.Log("currentIndex=" + currentIndex + "allProvinces.Count=" + allProvinces.Count);
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateQuestionText("Chúc mừng! Bạn đã hoàn thành bản đồ!");
                if (isMode3)
                {
                    // Lấy số ngọc hiện tại, cộng thêm 1 và lưu lại
                    int currentTaiCauTruc = PlayerPrefs.GetInt("Gem_TaiCauTruc", 0);
                    PlayerPrefs.SetInt("Gem_TaiCauTruc", currentTaiCauTruc + 1);
                    PlayerPrefs.Save();

                    Debug.Log("Đã thêm 1 Ngọc Tái Cấu Trúc! Tổng cộng: " + (currentTaiCauTruc + 1));
                }
                else
                {
                    if (!isMode4)
                    {
                        // Lấy số ngọc hiện tại, cộng thêm 1 và lưu lại
                        int currentDinhVi = PlayerPrefs.GetInt("Gem_DinhVi", 0);
                        PlayerPrefs.SetInt("Gem_DinhVi", currentDinhVi + 1);
                        PlayerPrefs.Save();

                        Debug.Log("Đã thêm 1 Ngọc Định Vị! Tổng cộng: " + (currentDinhVi + 1));
                    }
                }
                SoundManager.Instance.PlayCompleteSound();
                UIManager.Instance.thuongNgocPanel.SetActive(true);

                if (SoundManager.Instance != null) SoundManager.Instance.PlayCompleteSound();
                UIManager.Instance.availableImages[0].gameObject.SetActive(false);
                UIManager.Instance.availableImages[1].gameObject.SetActive(false);
                UIManager.Instance.availableImages[2].gameObject.SetActive(false);
                if (!isMode3)
                {
                    if (PlayerPrefs.GetInt("UnlockedMode", 1) < 2)
                    {
                        PlayerPrefs.SetInt("UnlockedMode", 2);
                        PlayerPrefs.Save(); // Lưu ngay lập tức
                        Debug.Log("Đã mở khóa Mode 2!");
                    }
                }
                // NẾU LÀ MODE 3 -> MỞ KHÓA MODE 4
                else
                {
                    if (PlayerPrefs.GetInt("UnlockedMode", 1) < 4)
                    {
                        PlayerPrefs.SetInt("UnlockedMode", 4);
                        PlayerPrefs.Save();
                        Debug.Log("Đã mở khóa Mode 4!");
                    }
                }
            }
        }
        }
    }

    // CẬP NHẬT: Thêm tham số Vector3 để lấy vị trí tỉnh
    public void CheckAnswer(string clickedID, Vector3 clickedPosition)
    {
        // Kiểm tra nếu bất kỳ Panel nào đang mở (Detail hoặc Lost) thì không cho tương tác tiếp
        if (UIManager.Instance.detailPanel.activeSelf || UIManager.Instance.lostPanel.activeSelf)
            return;

        if (clickedID == currentTarget.provinceID)
        {
            Debug.Log("Đúng!");
            if (SoundManager.Instance != null)
            {
                Debug.Log("Đang phát âm thanh đúng");
                SoundManager.Instance.PlayCorrectSound();
            }
            if (isMode4)
            {
                SurvivalManager.Instance.AddCombo();
            }
            // 1. Đánh dấu tỉnh đúng (đổi màu xanh)
            GameObject.Find(clickedID).GetComponent<ProvinceInteraction>()?.MarkAsCorrect();

            // 2. Hiện thông báo Win (như yêu cầu của bạn)
            UIManager.Instance.ShowWin(currentTarget);

            // 3. Hiện bảng thông tin chi tiết đè lên và vẽ đường kẻ
            UIManager.Instance.ShowDetailPage(currentTarget, clickedPosition);
        }
        else
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWrongSound();
                Debug.Log("Đang phát âm thanh sai");
            }
            if (isMode4)
            {
                SurvivalManager.Instance.ResetCombo();
            }
            UIManager.Instance.ShowLost();
        }
    }

    // Hàm này sẽ được gọi từ UIManager.CloseDetailPageAndNext()
    public void NextQuestion()
    {
        currentIndex++;
        SetNewQuestion();
    }

    // Được gọi khi bấm nút thử lại trên LostPanel
    public void TryAgain()
    {
        Debug.Log("inreal");
        UIManager.Instance.CloseLostAndTryAgain();
        SetNewQuestion();
    }
}