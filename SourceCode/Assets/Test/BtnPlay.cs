using UnityEngine;
using System.Collections;
[AddComponentMenu("Button/Play")]
public class BtnPlay : MonoBehaviour
{
    void OnClick()
    {
        Application.LoadLevel("Level");
    }
}
