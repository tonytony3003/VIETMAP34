using UnityEngine;

public class ScreenSizeChecker : MonoBehaviour
{
	public static ScreenSizeChecker Instance { get; private set; }

	// Biến lưu tỉ lệ màn hình dạng số thực (ví dụ: 16:9 sẽ ra khoảng 1.777)
	public float AspectRatio;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return; // Dừng chạy tiếp code bên dưới nếu object này bị hủy
		}
	}

	void Start()
	{
		int width = Screen.width;
		int height = Screen.height;

		// Ép kiểu (float) cho width để phép chia giữ được số thập phân
		AspectRatio = (float)width / height;

		Debug.Log($"Kích thước màn hình: {width} x {height}");
		Debug.Log($"Tỉ lệ màn hình (Aspect Ratio): {AspectRatio}");
	}
}