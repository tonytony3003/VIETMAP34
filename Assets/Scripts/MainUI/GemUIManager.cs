using UnityEngine;
using UnityEngine.UI;

public class GemUIManager : MonoBehaviour
{
    [Header("Text Hiển Thị Số Ngọc")]
    public Text txtGemDinhVi;      // Ngọc Định Vị (Mode 1)
    public Text txtGemTranAi;      // Ngọc Trấn Ải (Mode 2)
    public Text txtGemTaiCauTruc;  // Ngọc Tái Cấu Trúc (Mode 3)
    public Text txtGemToanTri;     // Ngọc Toàn Tri (Mode 4)

    void Start()
    {
        // Cập nhật giao diện ngọc ngay khi vào màn hình chính
        UpdateGemDisplay();
    }

    public void UpdateGemDisplay()
    {
        // Đọc dữ liệu từ PlayerPrefs, mặc định là 0 nếu chưa có
        int gemDinhVi = PlayerPrefs.GetInt("Gem_DinhVi", 0);
        int gemTranAi = PlayerPrefs.GetInt("Gem_TranAi", 0);
        int gemTaiCauTruc = PlayerPrefs.GetInt("Gem_TaiCauTruc", 0);
        int gemToanTri = PlayerPrefs.GetInt("Gem_ToanTri", 0);

        // Hiển thị lên UI
        if (txtGemDinhVi != null) txtGemDinhVi.text = "x" + gemDinhVi;
        if (txtGemTranAi != null) txtGemTranAi.text = "x" + gemTranAi;
        if (txtGemTaiCauTruc != null) txtGemTaiCauTruc.text = "x" + gemTaiCauTruc;
        if (txtGemToanTri != null) txtGemToanTri.text = "x" + gemToanTri;
    }

    // Hàm bổ trợ dùng để Test (Bạn có thể gắn vào 1 nút ẩn để test game)
    public void ResetAllGems()
    {
        PlayerPrefs.SetInt("Gem_DinhVi", 0);
        PlayerPrefs.SetInt("Gem_TranAi", 0);
        PlayerPrefs.SetInt("Gem_TaiCauTruc", 0);
        PlayerPrefs.SetInt("Gem_ToanTri", 0);
        PlayerPrefs.Save();
        UpdateGemDisplay();
        Debug.Log("Đã reset toàn bộ ngọc!");
    }
}