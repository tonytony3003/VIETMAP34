using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProvince", menuName = "MapGame/ProvinceData")]
public class ProvinceData : ScriptableObject
{
	public string provinceName; // Tên hiển thị (ví dụ: Hà Giang)
	public string provinceID;   // Phải khớp với tên GameObject tỉnh (ví dụ: Tinh_1)
    public Sprite provinceImage; // Ảnh đại diện của tỉnh (có thể dùng để hiển thị trên bản đồ hoặc trong bảng chi tiết)
    [Header("Ảnh sát nhập")]
    public List<Sprite> photoGallery; // Danh sách các ảnh nhỏ hơn (có thể dùng để hiển thị trong bảng chi tiết hoặc khi người chơi nhấp vào tỉnh)
    public Sprite MergerProvince; // Ảnh sau sát nhập 
    [Header("Nội dung hiển thị")]
    public Sprite provinceImage1; // Ảnh đại diện của tỉnh
    public Sprite provinceImage2;
    public Sprite provinceImage3;
    [TextArea(5, 20)]            // Cho phép nhập văn bản dài trong Inspector
    public string description;  // Đoạn văn mô tả chi tiết
    [TextArea(5, 20)]
    public string desSatNhap;
}