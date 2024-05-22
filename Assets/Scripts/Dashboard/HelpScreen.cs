using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpScreen : MonoBehaviour
{
    [SerializeField] private DashboardUIController dashboardUIController;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private List<Sprite> imageList;
    private int currentIndex = 0;

    private void OnEnable()
    {
        tutorialImage.sprite = imageList[0];
    }

    public void tutorialNavigation(bool isLeft)//Changes the images on the tutoria page based on the arrow you press.
    {
        if(isLeft)
        {
            currentIndex = (currentIndex - 1 + imageList.Count) % imageList.Count;
        }
        else
        {
            currentIndex = (currentIndex + 1) % imageList.Count;
        }
        tutorialImage.sprite = imageList[currentIndex];
    }

}
