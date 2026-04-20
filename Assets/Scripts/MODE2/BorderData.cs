using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBorderData", menuName = "MapGame/BorderData")]
public class BorderData : ScriptableObject
{
    public string countryName;
    public List<string> borderProvinceIDs; // Nhập ID các tỉnh đúng vào đây
    public string borderObjectName;
    [TextArea(5, 20)] // Tạo ô nhập liệu rộng trong Inspector
    public string countryDescription;
}