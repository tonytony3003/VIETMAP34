using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SurvivalManager : MonoBehaviour
{
    public static SurvivalManager Instance;

    [Header("Data Repositories")]
    public List<ProvinceData> allDataMode1;
    public List<BorderData> allDataMode2;
    public List<ProvinceData> allDataMode3;


    [Header("Timer & UI")]
    public Text timeText;
    public Text comboText; // Kéo thả UI Text hiển thị Combo vào đây
    public GameObject gameOverPanel;
    public GameObject gameOverText;
    public GameObject hetTimeText;
    public GameObject thuongNgocPanel;
    public Text tongKetText;
    public GameObject winBackground;
    public GameObject marking;

    private float remainingTime = 300f;
    private bool isTimerRunning = false;
    private int currentCombo = 0; // Biến lưu số câu đúng liên tiếp
    [Header("Mode Objects")]
    public GameObject gameObjectMode1;
    public GameObject gameObjectMode2;
    public GameObject gameObjectMode3;
    public GameObject vnSumMode1;
    public GameObject vnSumMode2;

    private Dictionary<int, bool> answeredQuestions = new Dictionary<int, bool>();
    private const int MAX_QUESTIONS = 45;

    private ProvinceData targetProvince;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() => StartSurvival();

    void Update()
    {
        if (isTimerRunning)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay(remainingTime);
            }
            else
            {
                remainingTime = 0;
                isTimerRunning = false;
                EndGame(); // Hết giờ thì kết thúc
            }
        }
    }
    public void StartSurvival()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM_Main14();
        answeredQuestions.Clear();
        gameOverPanel.SetActive(false);
        winBackground.SetActive(false);
        remainingTime = 300f; // Reset về 5 phút
        currentCombo = 0; // Reset combo khi bắt đầu
        UpdateComboUI();
        isTimerRunning = true;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        NextQuestion();
    }
    public void AddCombo()
    {
        currentCombo++;
        UpdateComboUI();
    }
    public void ResetCombo()
    {
        currentCombo = 0;
        UpdateComboUI();
    }
    void UpdateComboUI()
    {
        //if (comboText == null) return;

        //if (currentCombo > 0)
        //{
        //    comboText.text = "Combo x " + currentCombo;
        //    // Có thể thêm hiệu ứng scale nhẹ ở đây để làm combo nổi bật
        //}
        //else
        //{
        //    comboText.text = ""; // Ẩn khi không có combo
        //}
        if (comboText == null) return;

        if (currentCombo > 0)
        {
            comboText.text = "Combo x " + currentCombo;

            // --- HIỆU ỨNG DOTWEEN BẮT ĐẦU TỪ ĐÂY ---

            // 1. Dừng các animation đang chạy dở (để tránh lỗi nếu người chơi trả lời quá nhanh liên tục)
            comboText.transform.DOKill();

            // 2. Trả Text về trạng thái gốc trước khi nảy
            comboText.transform.localScale = Vector3.one;

            // 3. Hiệu ứng NẢY TO RA (Punch Scale)
            // new Vector3(0.5f, 0.5f, 0f) là độ to phóng ra thêm. 
            // 0.3f là thời gian nảy (tính bằng giây).
            comboText.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0f), 0.3f, 10, 1f);

            // 4. Hiệu ứng MÀU SẮC (Sáng rực lên rồi từ từ nguội lại)
            // Đổi màu ngay lập tức sang Vàng Cam
            comboText.color = new Color(1f, 0.7f, 0f);
            // Rồi từ từ (trong 0.5 giây) phai dần về màu trắng ban đầu
            comboText.DOColor(Color.green, 0.5f);

            // --- KẾT THÚC HIỆU ỨNG ---
        }
        else
        {
            comboText.text = ""; // Ẩn đi khi mất combo
        }
    }
    void UpdateTimerDisplay(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (timeText) timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void NextQuestion()
    {
        if (answeredQuestions.Count >= MAX_QUESTIONS)
        {
            EndGame();
            return;
        }

        // 1. Tìm Index ngẫu nhiên (0-59) chưa sử dụng
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, 60);
        } while (answeredQuestions.ContainsKey(randomIndex));
        Debug.Log("Chọn câu hỏi ngẫu nhiên: " + randomIndex+" Tong cau da tra loi "+answeredQuestions.Count);
        answeredQuestions.Add(randomIndex, true);

        // 3. Bật Object và Setup theo khoảng randomIndex
        if (randomIndex >= 0 && randomIndex <= 33) // 1 đến 34
        {
            if (gameObjectMode1) gameObjectMode1.SetActive(true);
            SetupMode1(randomIndex);
        }
        else if (randomIndex >= 34 && randomIndex <= 36) // 35 đến 37
        {
            if (gameObjectMode2) gameObjectMode2.SetActive(true);
            SetupMode2(randomIndex);
        }
        else // 38 đến 60
        {
            if (gameObjectMode3) gameObjectMode3.SetActive(true);
            SetupMode3(randomIndex);
        }
    }
    public void ExitToMainMenu()
    {
        // Đảm bảo dừng thời gian và giải phóng bộ nhớ nếu cần trước khi thoát
        isTimerRunning = false;
        SceneManager.LoadScene("MainUI");
    }
    void SetupMode1(int index)
    {
        QuizManager.Instance.currentTarget = allDataMode1[index];
        QuizManager.Instance.SetNewQuestion();
    }
    void SetupMode2(int index)
    {
        BorderQuizManager.Instance.currentTarget = allDataMode2[index - 34];
        vnSumMode2.SetActive(true);
        vnSumMode1.SetActive(false);
        BorderQuizManager.Instance.SetNewQuestion();
    }

    void SetupMode3(int index)
    {
        QuizManagerMode3.Instance.currentTarget = allDataMode3[index - 37];
        QuizManagerMode3.Instance.SetNewQuestion();
    }
    public void CloseAndNext()
    {
        if (gameObjectMode1.activeSelf == true)
        {
            UIManager.Instance.CloseDetailPageAndNext();
            gameObjectMode1.SetActive(false);
        }
        else if (gameObjectMode3.activeSelf == true)
        {
            UIManagerMode3.Instance.CloseDetailPageAndNext();
            gameObjectMode3.SetActive(false);
        }
        else if (gameObjectMode2.activeSelf == true)
        {
            BorderQuizManager.Instance.NextQuestion();
            vnSumMode2.SetActive(false);
            vnSumMode1.SetActive(true);
            gameObjectMode2.SetActive(false);
        }
        NextQuestion();
    }
    public void CloseRewardPanel()
    {
        thuongNgocPanel.SetActive(false);
        if (SoundManager.Instance != null) SoundManager.Instance.ResumeBGM();
    }
    void EndGame()
    {

        isTimerRunning = false;
        //if(marking.activeSelf == true) marking.SetActive(false);
        if (vnSumMode1.activeSelf == true) vnSumMode1.SetActive(false);
        if (vnSumMode2.activeSelf == true) vnSumMode2.SetActive(false);
        if (gameObjectMode1.activeSelf == true) gameObjectMode1.SetActive(false);
        if (gameObjectMode2.activeSelf == true) gameObjectMode2.SetActive(false);
        if (gameObjectMode3.activeSelf == true) gameObjectMode3.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (answeredQuestions.Count < MAX_QUESTIONS)
        {
            tongKetText.text = "Bạn đã trả lời đúng " + answeredQuestions.Count + "/"+MAX_QUESTIONS+" câu hỏi!\nHãy cố gắng hơn ở lần sau nhé!";
            if (SoundManager.Instance != null) SoundManager.Instance.PlayLoseSound();
            hetTimeText.SetActive(true);
        }
        else
        {
            winBackground.SetActive(true);
            tongKetText.text = "Bạn đã trả lời đúng toàn bộ câu hỏi trong thời gian quy định, xin chúc mừng!";
            Debug.Log("tong cau tra loi la"+answeredQuestions.Count);
            // Replace this line in EndGame():
            SoundManager.Instance.PlayCompleteSound();
            thuongNgocPanel.SetActive(true);
            marking.SetActive(false);
            // Lấy số ngọc hiện tại, cộng thêm 1 và lưu lại
            int currentToanTri = PlayerPrefs.GetInt("Gem_ToanTri", 0);
            PlayerPrefs.SetInt("Gem_ToanTri", currentToanTri + 1);
            PlayerPrefs.Save();

            Debug.Log("Đã thêm 1 Ngọc Toàn Tri! Tổng cộng: " + (currentToanTri + 1));
            // gameOverText.text = "";
            if (SoundManager.Instance != null) SoundManager.Instance.PlayCompleteSound();
            // With the following code to correctly clear the text of the Text component attached to the gameOverText GameObject:
            var gameOverTextComponent = gameOverText.GetComponent<Text>();
            if (gameOverTextComponent != null)
            {
                gameOverTextComponent.text = "BẠN ĐÃ CHIẾN THẮNG";
                gameOverTextComponent.color = Color.green; // Đổi màu chữ thành xanh lá để nổi bật hơn
            }
            
        }
        Debug.Log("Đã hoàn thành chuỗi 60 câu hỏi.");
    }
}