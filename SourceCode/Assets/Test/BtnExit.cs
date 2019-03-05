using UnityEngine;
using System.Collections;

[AddComponentMenu("Button/Exit")]

public class BtnExit : MonoBehaviour
{
    void OnClick()
    {
        Application.Quit();
    }
}
