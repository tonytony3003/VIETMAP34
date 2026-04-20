using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManagerMode2 : MonoBehaviour
{
    public static UIManagerMode2 Instance;
    public int count = 0;
    [Header("Panels")]
    public GameObject winPanel;
    public GameObject lostPanel;
    public GameObject finishPanel;
    public GameObject quizPanel;
    public GameObject thuongNgocPanel;
    public GameObject huongDan;
    public GameObject detailPanel;
    public GameObject confirmBtn;
    public List<GameObject> molds;
    public Text titleBorderText;
    public RectTransform detailPanelAnchor;
    [Header("Line Settings")]
    public LineRenderer lineRenderer;
    public float lineZOffset = -1f;

    [Header("Texts")]
    public Text questionText;
    public Text instructionText;
    public Text descriptionText;
    public ScrollRect descriptionScrollRect;
    public float cntx = 0;
    public float cnty = 0;
    void Awake()
    {
        // Safe singleton pattern to avoid accidental duplicate instances
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CloseAllPanels();
        if (quizPanel != null) quizPanel.SetActive(true);
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    public void SetQuizUI(string q, string i)
    {
        confirmBtn.SetActive(true);
        huongDan.SetActive(true);
        quizPanel.SetActive(true);
        questionText.text = q;
        instructionText.text = i;
    }

    // Centralized show/hide for win panel with debug logging
    public void ShowWin()
    {
        confirmBtn.SetActive(false);
        huongDan.SetActive(false);
        quizPanel.SetActive(false);
        // Reset thanh cuộn
        Canvas.ForceUpdateCanvases();
        if (descriptionScrollRect != null)
            descriptionScrollRect.verticalNormalizedPosition = 1f;
        //
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        if (detailPanel != null)
        {
            molds[0].gameObject.SetActive(false);
            molds[1].gameObject.SetActive(false);
            molds[2].gameObject.SetActive(false);
            if (BorderQuizManager.Instance.currentTarget.countryName.Contains("Lào")) molds[0].gameObject.SetActive(true);
            if (BorderQuizManager.Instance.currentTarget.countryName.Contains("Campuchia")) molds[1].gameObject.SetActive(true);
            if (BorderQuizManager.Instance.currentTarget.countryName.Contains("Trung Quốc")) molds[2].gameObject.SetActive(true);
            detailPanel.SetActive(true);
        }
        if (titleBorderText != null)
        {
            titleBorderText.text = "Đường Biên Giới Việt - " + BorderQuizManager.Instance.currentTarget.countryName;
        }
        if (descriptionText != null)
            descriptionText.text = BorderQuizManager.Instance.currentTarget.countryDescription;
    }

    public void HideWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    public void ShowLost()
    {
        confirmBtn.SetActive(false);
        huongDan.SetActive(false);
        quizPanel.SetActive(false);
        if (lostPanel != null)
        {
            lostPanel.SetActive(true);
        }
    }
    public void ShowDetail(Vector3 borderPivotPos, bool hideWin = false)
    {
        if (detailPanel != null)
        {
            detailPanel.SetActive(true);
            // Vẽ đường nối
            DrawConnectionLine(borderPivotPos);
        }
        if (hideWin) HideWin();
    }
    public void ShowFinishGame()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(true);
        }
        if (SoundManager.Instance != null) SoundManager.Instance.PlayCompleteSound();
        questionText.text = "";
        instructionText.text = "";
        SoundManager.Instance.PlayCompleteSound();
        thuongNgocPanel.SetActive(true);
        // Lấy số ngọc hiện tại, cộng thêm 1 và lưu lại
        int currentTranAi = PlayerPrefs.GetInt("Gem_TranAi", 0);
        PlayerPrefs.SetInt("Gem_TranAi", currentTranAi + 1);
        PlayerPrefs.Save();

        Debug.Log("Đã thêm 1 Ngọc Trấn Ải! Tổng cộng: " + (currentTranAi + 1));
        if (PlayerPrefs.GetInt("UnlockedMode", 1) < 3)
        {
            PlayerPrefs.SetInt("UnlockedMode", 3);
            PlayerPrefs.Save();
            Debug.Log("Đã mở khóa Mode 3!");
        }
    }

    public bool IsUIPanelActive()
    {
        return (winPanel != null && winPanel.activeSelf)
            || (lostPanel != null && lostPanel.activeSelf)
            || (finishPanel != null && finishPanel.activeSelf)
            || (detailPanel != null && detailPanel.activeSelf);
    }

    public void CloseAllPanels()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (detailPanel != null) detailPanel.SetActive(false);
        if (lostPanel != null) lostPanel.SetActive(false);
        if (finishPanel != null) finishPanel.SetActive(false);
        if (lineRenderer != null) lineRenderer.enabled = false;
        //ProvinceNameManager.Instance.HideAllNames();
	}
    private void DrawConnectionLine(Vector3 worldPos)
    {
        if (lineRenderer == null || detailPanelAnchor == null) return;

        lineRenderer.enabled = true;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;

        // Cấu hình màu đỏ cho Line (Nếu bạn chưa chỉnh trong Inspector)
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        // Điểm đầu: Pivot của targetLine (Biên giới)
        if (BorderQuizManager.Instance.currentTarget.countryName.Contains("Campuchia")) cntx = 0.9f;
        if (BorderQuizManager.Instance.currentTarget.countryName.Contains("Trung Quốc")) cnty=0.55f;
        lineRenderer.SetPosition(0, new Vector3(worldPos.x+cntx, worldPos.y+cnty, lineZOffset));
        cntx = 0f;
        cnty = 0f;

        // Điểm cuối: Chuyển vị trí của LineAnchor (UI) sang World Space
        Vector3 worldEndPos;
        Canvas canvas = detailPanelAnchor.GetComponentInParent<Canvas>();

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector3 screenPoint = detailPanelAnchor.position;
            screenPoint.z = Mathf.Abs(Camera.main.transform.position.z - lineZOffset);
            worldEndPos = Camera.main.ScreenToWorldPoint(screenPoint);
        }
        else
        {
            worldEndPos = detailPanelAnchor.position;
        }

        worldEndPos.z = lineZOffset;
        lineRenderer.SetPosition(1, worldEndPos);
    }
    public void ExitToMainMenu()
    {
        // Đảm bảo dừng thời gian và giải phóng bộ nhớ nếu cần trước khi thoát
        SceneManager.LoadScene("MainUI");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM_Main14();
        }
    }
    public void CloseRewardPanel()
    {
        thuongNgocPanel.SetActive(false);
        if (SoundManager.Instance != null) SoundManager.Instance.ResumeBGM();
    }
}