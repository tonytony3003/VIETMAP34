using System.Collections.Generic;
using UnityEngine;

public class QuizManagerMode3 : MonoBehaviour
{
    public static QuizManagerMode3 Instance;

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
        if (isMode4)
        {
            return;
        }
        allProvinces.Shuffle(); // Xáo trộn danh sách tỉnh để mỗi lần chơi sẽ có thứ tự khác nhau
        SetNewQuestion();
    }
    public void SetNewQuestion()
    {
        if (isMode4)
        {
            if (!isMode3)
            {
                Debug.Log("Đang ở Mode 1, cập nhật câu hỏi sát nhập");
                UIManagerMode3.Instance.UpdateQuestionText("Đâu là " + currentTarget.provinceName + "?");
            }
            else
            {
                UIManagerMode3.Instance.UpdateQuestionText("Các tỉnh dưới đây sát nhâp thành tỉnh mới nào ?");
                UIManagerMode3.Instance.ShowGallery(currentTarget);
            }
        }
        else
        {
            if (currentIndex < allProvinces.Count)
            {
                currentTarget = allProvinces[currentIndex];
                // Cập nhật câu hỏi lên UI
                if (UIManager.Instance != null)
                {
                    if (!isMode3)
                    {
                        UIManagerMode3.Instance.UpdateQuestionText("Đâu là " + currentTarget.provinceName + "?");
                    }
                    else
                    {
                        UIManagerMode3.Instance.UpdateQuestionText("Các tỉnh dưới đây sáp nhâp thành tỉnh mới nào ?");
                        UIManagerMode3.Instance.ShowGallery(currentTarget);
                    }

                }
            }
            else
            {
                if (UIManagerMode3.Instance != null)
                {
                    UIManagerMode3.Instance.UpdateQuestionText("Chúc mừng! Bạn đã hoàn thành bản đồ!");
                    if (SoundManager.Instance != null) SoundManager.Instance.PlayCompleteSound();
                }
            }
        }
    }

    // CẬP NHẬT: Thêm tham số Vector3 để lấy vị trí tỉnh
    public void CheckAnswer(string clickedID, Vector3 clickedPosition)
    {
        // Kiểm tra nếu bất kỳ Panel nào đang mở (Detail hoặc Lost) thì không cho tương tác tiếp
        if (UIManagerMode3.Instance.detailPanel.activeSelf || UIManagerMode3.Instance.lostPanel.activeSelf)
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
            UIManagerMode3.Instance.ShowWin(currentTarget);

            // 3. Hiện bảng thông tin chi tiết đè lên và vẽ đường kẻ
            UIManagerMode3.Instance.ShowDetailPage(currentTarget, clickedPosition);
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
            UIManagerMode3.Instance.ShowLost();
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
        UIManagerMode3.Instance.CloseLostAndTryAgain();
        SetNewQuestion();
    }
}