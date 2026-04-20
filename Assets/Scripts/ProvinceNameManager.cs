using UnityEngine;
using System.Collections.Generic;

public class ProvinceNameManager : MonoBehaviour
{
	public static ProvinceNameManager Instance;

	// Dictionary để tra cứu nhanh: Key là tên tỉnh, Value là GameObject chứa Text
	public Dictionary<string, GameObject> provinceTextDic = new Dictionary<string, GameObject>();

	void Awake()
	{
		Instance = this;
		// Tự động quét tất cả các UI Text con bên dưới nó và đưa vào Dictionary
		foreach (Transform child in transform)
		{
			// Giả sử bạn đặt tên UI Text trùng với ID tỉnh (ví dụ: "HaNoi", "BacGiang")
			if (!provinceTextDic.ContainsKey(child.name))
			{
				provinceTextDic.Add(child.name, child.gameObject);
			}
			// Mặc định ẩn hết đi khi bắt đầu
			child.gameObject.SetActive(false);
		}
	}

	// Hàm để bật tên tỉnh
	public void ShowName(string provinceID)
	{
		if (provinceTextDic.ContainsKey(provinceID))
		{
			provinceTextDic[provinceID].SetActive(true);
		}
		else
		{
			Debug.LogWarning("Không tìm thấy UI Text nào có tên: " + provinceID);
		}
	}

	// Hàm ẩn tất cả (dùng khi chơi lại hoặc qua màn)
	public void HideAllNames()
	{
		foreach (var item in provinceTextDic.Values)
		{
			item.SetActive(false);
		}
	}
}