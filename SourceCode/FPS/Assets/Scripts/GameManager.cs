using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region 字段
    public static GameManager instance = null;
    //游戏得分
    int m_score = 0;
    int m_scoreSP = 0;
    //游戏最高得分
    static int m_hiscore = 0;
    static int m_hiscoreSP = 0;
    //子弹数
    int m_ammo = 100;
    //游戏主角
    Player m_player;
    public Transform m_door;
    //GUIText
    GUIText txt_ammo;
    GUIText txt_life;
    GUIText txt_hiscore;
    GUIText txt_score;
    public bool pause = false;
    bool flag = false;
    public int CurrentSceneNumber;
    #endregion

    // Use this for initialization
    void Start()
    {
        if (CurrentSceneNumber == 2)
        {
            m_score += 2100;
        }
        instance = this;
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        txt_ammo = this.transform.FindChild("txt_ammo").GetComponent<GUIText>();
        txt_hiscore = this.transform.FindChild("txt_hiscore").GetComponent<GUIText>();
        txt_score = this.transform.FindChild("txt_score").GetComponent<GUIText>();
        txt_life = this.transform.FindChild("txt_life").GetComponent<GUIText>();
        if (CurrentSceneNumber == 0)
        {
            txt_score.text = "Score  " + m_scoreSP;
            txt_hiscore.text = "High Score  " + m_hiscoreSP;
        }
        else
        {
            txt_score.text = "Score  " + m_score;
            txt_hiscore.text = "High Score  " + m_hiscore;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (pause) return;
        if (CurrentSceneNumber == 1)
        {
            if (m_score >= 2100 && !flag)
            {
                flag = true;
                Destroy(GameObject.FindGameObjectWithTag("Block"));
                m_door.GetComponent<Animation>().Play("open");
            }
        }
    }

    /// <summary>
    /// 更新分数
    /// </summary>
    /// <param name="score">增加的分数数值</param>
    public void SetScore(int score)
    {

        if (CurrentSceneNumber == 0)
        {
            m_scoreSP += score;
            if (m_scoreSP > m_hiscoreSP)
            {
                m_hiscoreSP = m_scoreSP;
            }
            txt_score.text = "Score  " + m_scoreSP;
            txt_hiscore.text = "High Score  " + m_hiscoreSP;
        }
        else
        {
            m_score += score;
            if (m_score > m_hiscore)
            {
                m_hiscore = m_score;
            }
            txt_score.text = "Score  " + m_score;
            txt_hiscore.text = "High Score  " + m_hiscore;
        }
    }

    /// <summary>
    /// 更新弹药
    /// </summary>
    /// <param name="ammo">减少的弹药数量</param>
    public void SetAmmo(int ammo)
    {
        m_ammo -= ammo;
        if (m_ammo <= 0)
        {
            m_ammo = 100 + m_ammo;
        }
        txt_ammo.text = m_ammo.ToString() + "/100";
    }

    /// <summary>
    /// 更新生命
    /// </summary>
    /// <param name="life">当前生命数值</param>
    public void SetLife(int life)
    {
        if (win)
            return;
        txt_life.text = life.ToString();
    }

    public Texture m_TryAgain;
    public Texture m_Title;
    public bool win = false;
    void OnGUI()
    {
        //居中显示文字
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        //改变文字大小
        GUI.skin.label.fontSize = 40;

        if (CurrentSceneNumber == 1)
        {
            if (m_score >= 2100)
            {
                //显示通关
                GUI.Label(new Rect(Screen.width * 0.5f - 300, Screen.height * 0.4f, 600, 45), "恭喜！第一关已通过！");
                GUI.Label(new Rect(Screen.width * 0.5f - 300, Screen.height * 0.5f, 600, 45), "请通过最初的门进入第二关！");
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            pause = true;
            //释放鼠标
            Screen.lockCursor = false;
            //显示重新开始按钮
            if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.70f, 300, 45), m_TryAgain))
            {
                if (CurrentSceneNumber == 0)
                {
                    Application.LoadLevel("LevelSP");
                }
                else
                {
                    Application.LoadLevel("Level");
                }

                pause = false;
            }

            //显示回到标题按钮
            if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.85f, 300, 45), m_Title))
            {
                Application.LoadLevel("Title");
                pause = false;
            }
        }
        else if (m_player.m_life <= 0 && !win)
        {
            pause = true;
            //显示Game Over
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Game Over");

            //释放鼠标
            Screen.lockCursor = false;

            //显示重新开始按钮
            if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.70f, 300, 45), m_TryAgain))
            {
                if (CurrentSceneNumber == 0)
                {
                    Application.LoadLevel("LevelSP");
                }
                else
                {
                    Application.LoadLevel("Level");
                }
                pause = false;
            }

            //显示回到标题按钮
            if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.85f, 300, 45), m_Title))
            {
                Application.LoadLevel("Title");
                pause = false;
            }
        }
        else if (win)
        {
            pause = true;
            //显示Win
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "You Win!");

            //释放鼠标
            Screen.lockCursor = false;

            //显示重新开始按钮
            if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.70f, 300, 45), m_TryAgain))
            {
                Application.LoadLevel("Level");
                pause = false;
            }

            //显示回到标题按钮
            if (GUI.Button(new Rect(Screen.width * 0.5f - 150, Screen.height * 0.85f, 300, 45), m_Title))
            {
                Application.LoadLevel("Title");
                pause = false;
            }
        }
        else
        {
            pause = false;
            Screen.lockCursor = true;
        }
    }


}

