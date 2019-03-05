using UnityEngine;
using System.Collections;

public class TitleUI : MonoBehaviour
{
    public Texture[] m_button;

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.55f, 300, 45), m_button[0]))
        {
            Application.LoadLevel("Level");
        }
        if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.70f, 300, 45), m_button[1]))
        {
            Application.LoadLevel("LevelSP");
        }
        if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.85f, 300, 45), m_button[2]))
        {
            Application.Quit();
        }
    }
}
