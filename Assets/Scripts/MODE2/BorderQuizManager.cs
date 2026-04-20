using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BorderQuizManager : MonoBehaviour
{
	public static BorderQuizManager Instance;

	[Header("Data")]
	public List<BorderData> allQuests;
	public BorderData currentTarget;
	private int currentIndex = 0;

	[Header("Border Lines")]
	public GameObject lineLao;
	public GameObject lineCampuchia;
	public GameObject lineTrungQuoc;
	public bool isMode4 = false; // Biến để xác định chế độ chơi (1 hay 3)

	private List<ProvinceInteractionMode2> selectedProvinces = new List<ProvinceInteractionMode2>();
	public Transform borderAnchorPoint;

	void Awake() => Instance = this;

	void Start()
	{
		HideAllLines();
		if (isMode4)
		{
			return;
		}
        allQuests.Shuffle(); // Xáo trộn danh sách câu hỏi để mỗi lần chơi sẽ có thứ tự khác nhau
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM_Mode2();
        SetNewQuestion();
	}

	public void SetNewQuestion()
	{
		StopAllCoroutines();
		HideAllLines();
		ResetProvincesUI();

		if (currentIndex < allQuests.Count)
		{
			currentTarget = allQuests[currentIndex];
			UIManagerMode2.Instance.SetQuizUI(
				"Các tỉnh nào tiếp giáp với " + currentTarget.countryName + "?",
				"Chọn các tỉnh trên bản đồ rồi nhấn Xác nhận"
			);
		}
		else
		{
			if(!isMode4) UIManagerMode2.Instance.ShowFinishGame();
		}
	}

	public void ConfirmSelection()
	{
		if (currentTarget == null || UIManagerMode2.Instance.IsUIPanelActive()) return;

		List<string> selectedIDs = selectedProvinces.Select(p => p.provinceID.Trim()).ToList();
		List<string> correctIDs = currentTarget.borderProvinceIDs.Select(id => id.Trim()).ToList();
		selectedIDs.Sort();
		correctIDs.Sort();

		if (selectedIDs.SequenceEqual(correctIDs))
		{
            if (SoundManager.Instance != null)
            {
                Debug.Log("Đang phát âm thanh đúng");
                SoundManager.Instance.PlayCorrectSound();
            }
            if (isMode4)
            {
                SurvivalManager.Instance.AddCombo();
            }
            // 1. Nhấp nháy biên giới ngay lập tức
            GameObject targetLine = GetTargetLine();
			if (targetLine != null) StartCoroutine(BlinkRoutine(targetLine));
			// Truyền vị trí của targetLine sang hàm ShowDetail
			UIManagerMode2.Instance.ShowDetail(targetLine.transform.position, false);
			UIManagerMode2.Instance.ShowWin();
			//Hiển thị tên các tỉnh
			foreach (string id in correctIDs)
			{
				ProvinceNameManager.Instance.ShowName(id);
			}
		}
		else
		{
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWrongSound();
                Debug.Log("Đang phát âm thanh sai");
            }
            if (isMode4)
            {
                SurvivalManager.Instance.ResetCombo();
            }
            UIManagerMode2.Instance.ShowLost();
		}
	}

	public void NextQuestion()
	{
		currentIndex++;
		UIManagerMode2.Instance.CloseAllPanels();
		SetNewQuestion();
	}

	private GameObject GetTargetLine()
	{
		if (currentTarget.countryName.Contains("Lào")) return lineLao;
		if (currentTarget.countryName.Contains("Campuchia")) return lineCampuchia;
		if (currentTarget.countryName.Contains("Trung Quốc")) return lineTrungQuoc;
		return null;
	}

	public void OnProvinceClick(ProvinceInteractionMode2 province)
	{
		if (UIManagerMode2.Instance.IsUIPanelActive()) return;

		if (province.IsSelected())
		{
			if (!selectedProvinces.Contains(province)) selectedProvinces.Add(province);
		}
		else
		{
			selectedProvinces.Remove(province);
		}
	}

	private void ResetProvincesUI()
	{
		foreach (var p in selectedProvinces) p.ResetProvince();
		selectedProvinces.Clear();
	}

	private void HideAllLines()
	{
		if (lineLao) lineLao.SetActive(false);
		if (lineCampuchia) lineCampuchia.SetActive(false);
		if (lineTrungQuoc) lineTrungQuoc.SetActive(false);
	}

	private IEnumerator BlinkRoutine(GameObject target)
	{
		target.SetActive(true);
		Renderer rd = target.GetComponent<Renderer>();
		while (true)
		{
			if (rd != null) rd.enabled = !rd.enabled;
			else target.SetActive(!target.activeSelf);
			yield return new WaitForSeconds(0.4f);
		}
	}

	public void TryAgain()
	{
		ResetProvincesUI();
		UIManagerMode2.Instance.CloseAllPanels();
		SetNewQuestion();
	}
}