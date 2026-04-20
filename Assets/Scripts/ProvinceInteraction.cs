using UnityEngine;
using UnityEngine.UIElements;

public class ProvinceInteraction : MonoBehaviour
{
	private SpriteRenderer sr;
	private Color originalColor;

	[Header("Settings")]
	public Color hoverColor = Color.white;
	public Color correctColor = Color.green; // Màu khi đã trả lời đúng

	private bool isCorrected = false; // Đánh dấu tỉnh này đã được giải xong

	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		if (sr != null)
		{
			originalColor = sr.color;
		}
	}

	void OnMouseEnter()
	{
		// Nếu đã đúng rồi hoặc UI đang mở thì không đổi màu hover
		if (isCorrected || IsUIOpen()) return;
		sr.color = hoverColor;
	}

	void OnMouseExit()
	{
		if (isCorrected) return;
		sr.color = originalColor;
	}

	void OnMouseDown()
	{
		if (SurvivalManager.Instance)
		{
			if (SurvivalManager.Instance.gameObjectMode1.activeSelf)
			{
				// Không cho bấm nếu đã trả lời đúng hoặc đang hiện bảng thông báo
				if (isCorrected || IsUIOpen()) return;

				if (QuizManager.Instance != null)
				{
					// TRUYỀN THÊM: transform.position để QuizManager biết điểm bắt đầu của đường Line
					QuizManager.Instance.CheckAnswer(gameObject.name, transform.position);
				}
			}
			else if (SurvivalManager.Instance.gameObjectMode3.activeSelf)
			{
				Debug.Log("INNNNNNNNNNNNNN");
				// Không cho bấm nếu đã trả lời đúng hoặc đang hiện bảng thông báo
				if (isCorrected || IsUIOpen()) return;
				if (QuizManagerMode3.Instance != null)
				{
					// TRUYỀN THÊM: transform.position để QuizManager biết điểm bắt đầu của đường Line
					QuizManagerMode3.Instance.CheckAnswer(gameObject.name, transform.position);
				}
			}
		}
		else
		{
			if (isCorrected || IsUIOpen()) return;

			if (QuizManager.Instance != null)
			{
				// TRUYỀN THÊM: transform.position để QuizManager biết điểm bắt đầu của đường Line
				QuizManager.Instance.CheckAnswer(gameObject.name, transform.position);
			}
		}
	}

	// Hàm kiểm tra xem có Panel nào đang hiện không để chặn bấm xuyên thấu
	private bool IsUIOpen()
	{
		if (UIManager.Instance == null) return false;

		// Chặn tương tác nếu bảng chi tiết hoặc bảng thua đang hiện
		return UIManager.Instance.detailPanel.activeSelf || UIManager.Instance.lostPanel.activeSelf;
	}

	// Hàm này sẽ được gọi từ QuizManager khi người chơi chọn đúng
	public void MarkAsCorrect()
	{
		if (!SurvivalManager.Instance)
		{
			isCorrected = true;
			if (sr != null)
			{
				sr.color = correctColor;
			}
		}

		// Thường sẽ giữ Collider để người chơi vẫn có thể click xem lại thông tin nếu muốn, 
		// nhưng ở đây ta đã chặn logic CheckAnswer rồi.
	}
}