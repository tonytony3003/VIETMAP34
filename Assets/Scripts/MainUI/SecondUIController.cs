using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondUIController : MonoBehaviour
{
    public List<GameObject> gameObjectsUI = new List<GameObject>();
    void Start()
    {
		if (ScreenSizeChecker.Instance.AspectRatio >= 1.4f && ScreenSizeChecker.Instance.AspectRatio <= 1.55f)
        {
            gameObjectsUI[0].SetActive(true);
        }
        else if (ScreenSizeChecker.Instance.AspectRatio >= 1.58f && ScreenSizeChecker.Instance.AspectRatio < 1.63f)
        {
            gameObjectsUI[1].SetActive(true);
        }
        else if (ScreenSizeChecker.Instance.AspectRatio >= 1.7f && ScreenSizeChecker.Instance.AspectRatio < 2.3f)
        {
            gameObjectsUI[2].SetActive(true);
        }
        else if (ScreenSizeChecker.Instance.AspectRatio >= 2.3f && ScreenSizeChecker.Instance.AspectRatio < 3.5f)
        {
            gameObjectsUI[3].SetActive(true);
        }
        else if (ScreenSizeChecker.Instance.AspectRatio >= 3.5f)
        {
            gameObjectsUI[4].SetActive(true);
        }
    }
}
