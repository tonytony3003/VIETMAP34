using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject quizPanel;      // Panel chứa câu hỏi ban đầu
    public GameObject winPanel;       // Panel báo ĐÚNG (bạn muốn giữ lại)
    public GameObject detailPanel;    // Panel chi tiết màu xám bên phải
    public GameObject thuongNgocPanel;
    public GameObject lostPanel;      // Panel báo SAI
    public GameObject mapObject;      // Panel phóng to ảnh
    public GameObject zoomPanel;
    public GameObject mapLineConnection;// Panel phóng to ảnh
    public GameObject marking;

    [Header("Detail UI Elements")]
    public Text detProvinceNameText;
    public Image detProvinceImage1;
    public Image detProvinceImage2;
    public Image detProvinceImage3;
    public Image detProvinceImageAvatar;
    public Image zoomDisplayImage;
    public Text detDescriptionText;
    public ScrollRect descriptionScrollRect;
    public RectTransform detailPanelAnchor;

    [Header("Line Settings")]
    public LineRenderer lineRenderer;
    public float lineZOffset = -1f;

    public Transform gridContainer; // GameObject chứa Grid Layout Group
    public Image[] availableImages;
    void Awake()
    {

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        //if (!QuizManager.Instance.isMode3)
        //{
        //	gridContainer = null;
        //	availableImages = null;
        //}
        //else
        //{
        List<Image> tempLinks = new List<Image>(gridContainer.GetComponentsInChildren<Image>(true));

        // Nếu GridContainer có Image, nó thường nằm ở index 0, ta xóa nó đi
        if (gridContainer.GetComponent<Image>() != null)
        {
            tempLinks.Remove(gridContainer.GetComponent<Image>());
        }

        availableImages = tempLinks.ToArray();
        //}
    }

    void Start()
    {
        CloseAllPanels();
        quizPanel.SetActive(true);
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    // --- HÀM SHOW WIN (Giữ nguyên như bạn muốn) ---
    public void ShowWin(ProvinceData data)
    {
        winPanel.SetActive(true);
        ProvinceNameManager.Instance.ShowName(data.provinceID);
        // Có thể ẩn quizPanel đi để bớt rối mắt
        quizPanel.SetActive(false);
    }

    // --- HÀM HIỆN UI CHI TIẾT (Đè lên phía trên) ---
    public void ShowDetailPage(ProvinceData data, Vector3 provinceGlobalPos)
    {
        // Hiện bảng chi tiết đè lên
        detailPanel.SetActive(true);

        // Cập nhật dữ liệu cho bảng chi tiết
        if (!QuizManager.Instance.isMode3)
        {

            detProvinceNameText.text = data.provinceName;
            detProvinceImage1.sprite = data.provinceImage1;
            detProvinceImage2.sprite = data.provinceImage2;
            detProvinceImage3.sprite = data.provinceImage3;
            detProvinceImageAvatar.sprite = data.provinceImage;
            detDescriptionText.text = data.description;
        }
        else
        {
            detProvinceNameText.text = data.provinceName + " sau sáp nhập";
            detProvinceImage1.sprite = data.MergerProvince;
            detDescriptionText.text = data.desSatNhap;
            detProvinceImageAvatar.sprite = data.provinceImage;
        }


        // Reset thanh cuộn
        Canvas.ForceUpdateCanvases();
        if (descriptionScrollRect != null)
            descriptionScrollRect.verticalNormalizedPosition = 1f;

        // Vẽ đường Line
        DrawConnectionLine(provinceGlobalPos);
    }
    public void UpdateQuestionText(string text)
    {
        if (quizPanel != null && quizPanel.GetComponentInChildren<Text>() != null)
        {
            quizPanel.GetComponentInChildren<Text>().text = text;
        }
    }
    public void ShowGallery(ProvinceData data)
    {
        if (data == null || data.photoGallery == null)
        {
            ClearGallery();
            return;
        }
        // Duyệt qua danh sách Image có sẵn trong Grid
        for (int i = 0; i < 3; i++)
        {
            if (i < data.photoGallery.Count)
            {
                availableImages[i].gameObject.SetActive(true);
                availableImages[i].sprite = data.photoGallery[i];
                availableImages[i].preserveAspect = true;
            }
            else
            {
                // Nếu số lượng ảnh trong Data ít hơn số lượng Image có sẵn
                // thì ẩn các Image thừa đi
                availableImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void ClearGallery()
    {
        if (availableImages == null) return;

        foreach (Image img in availableImages)
        {
            if (img != null) img.gameObject.SetActive(false);
        }
    }
    private void DrawConnectionLine(Vector3 provinceWorldPos)
    {
        if (lineRenderer == null || detailPanelAnchor == null) return;

        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;

        // 1. Điểm đầu: Vị trí của tỉnh (Bản đồ)
        Vector3 startPos = new Vector3(provinceWorldPos.x, provinceWorldPos.y, lineZOffset);
        lineRenderer.SetPosition(0, startPos);

        // 2. Điểm cuối: Chuyển tọa độ UI sang World Space
        Vector3 worldEndPos;

        // Kiểm tra chế độ Canvas để dùng hàm phù hợp
        Canvas canvas = detailPanelAnchor.GetComponentInParent<Canvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Nếu dùng Overlay, ta phải chuyển từ tọa độ Screen sang World thông qua Camera
            Vector3 screenPoint = detailPanelAnchor.position;
            screenPoint.z = Mathf.Abs(Camera.main.transform.position.z - lineZOffset);
            worldEndPos = Camera.main.ScreenToWorldPoint(screenPoint);
        }
        else
        {
            // Nếu dùng Screen Space - Camera, ta lấy trực tiếp vị trí World của Anchor
            worldEndPos = detailPanelAnchor.position;
        }

        // Ép Z để line không bị chéo sâu vào màn hình
        worldEndPos.z = lineZOffset;
        lineRenderer.SetPosition(1, worldEndPos);
    }

    public void ShowLost()
    {
        quizPanel.SetActive(false);

        lostPanel.SetActive(true);
    }

    // Hàm gọi khi Click bên ngoài vùng xám để tiếp tục
    public void CloseDetailPageAndNext()
    {
        // Chỉ chạy nếu Panel đang hiện
        if (detailPanel.activeSelf)
        {
            detailPanel.SetActive(false);
            winPanel.SetActive(false);
            lineRenderer.enabled = false;

            quizPanel.SetActive(true);
            QuizManager.Instance.NextQuestion();
        }
    }

    public void CloseLostAndTryAgain()
    {
        Debug.Log("in");
        lostPanel.SetActive(false);
        quizPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        //if (thuongNgocPanel != null) thuongNgocPanel.SetActive(false);
        if (quizPanel != null) quizPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (detailPanel != null) detailPanel.SetActive(false);
        if (lostPanel != null) lostPanel.SetActive(false);
        if (lineRenderer != null) lineRenderer.enabled = false;
    }
    public void OpenPhotoZoom(Image clickedImage)
    {
        if (clickedImage.sprite == null) return;

        // 1. Lấy sprite từ ảnh nhỏ gán vào ảnh to
        zoomDisplayImage.sprite = clickedImage.sprite;

        // 2. Hiện Panel phóng to
        zoomPanel.SetActive(true);
        mapLineConnection.SetActive(false); // Ẩn đường line khi phóng to ảnh

        // 3. Ẩn bản đồ và bảng chi tiết cho sạch màn hình
        if (mapObject != null) mapObject.SetActive(false);
        if(marking != null) marking.SetActive(false);
        detailPanel.SetActive(false);
    }
    public void CloseRewardPanel()
    {
        thuongNgocPanel.SetActive(false);
        if (SoundManager.Instance != null) SoundManager.Instance.ResumeBGM();
    }
    public void ClosePhotoZoom()
    {
        // 1. Tắt Panel phóng to
        zoomPanel.SetActive(false);
        mapLineConnection.SetActive(true); // Hiện lại đường line khi đóng ảnh phóng to

        // 2. Hiện lại bản đồ và bảng chi tiết như cũ
        if (mapObject != null) mapObject.SetActive(true);
        if(marking != null) marking.SetActive(true);
        detailPanel.SetActive(true);
    }
    public void ExitToMainMenu()
    {
        // Đảm bảo dừng thời gian và giải phóng bộ nhớ nếu cần trước khi thoát
        if (SoundManager.Instance != null&&QuizManager.Instance.isMode3)
        {
            SoundManager.Instance.PlayBGM_Main14();
        }
        SceneManager.LoadScene("MainUI");
    }
}