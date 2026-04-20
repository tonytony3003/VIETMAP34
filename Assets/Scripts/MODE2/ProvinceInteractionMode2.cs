using UnityEngine;

public class ProvinceInteractionMode2 : MonoBehaviour
{
    public string provinceID;
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f); // Màu khi di chuột qua
    public Color selectedColor = Color.cyan;             // Màu khi đã tích chọn

    private Color originalColor;
    private SpriteRenderer sr;
    private bool isSelected = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        provinceID = gameObject.name;
    }

    // --- HIỆU ỨNG HOVER ---
    void OnMouseEnter()
    {
        // Nếu UI đang hiện hoặc tỉnh đã được chọn thì không đổi màu hover
        if (UIManagerMode2.Instance.IsUIPanelActive() || isSelected) return;
        sr.color = hoverColor;
    }

    void OnMouseExit()
    {
        // Trả về màu gốc nếu tỉnh chưa được chọn
        if (isSelected) return;
        sr.color = originalColor;
    }

    // --- LOGIC CLICK CHỌN ---
    void OnMouseDown()
    {
        if (UIManagerMode2.Instance.IsUIPanelActive()) return;

        isSelected = !isSelected;

        // Ưu tiên hiển thị màu Selected, nếu bỏ chọn thì về màu Original
        sr.color = isSelected ? selectedColor : originalColor;

        BorderQuizManager.Instance.OnProvinceClick(this);
    }

    public void ResetProvince()
    {
        isSelected = false;
        sr.color = originalColor;
    }

    public bool IsSelected() => isSelected;
}