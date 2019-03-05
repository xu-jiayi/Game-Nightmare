using UnityEngine;
using System.Collections;

public class PillSpawn : MonoBehaviour
{
    #region 字段
    //药品的prefeb
    public Transform m_pill;
    //生成的药品的数量
    public int m_pillCount = 0;
    //生成药品的时间间隔
    float m_timer = 0;

    protected Transform m_transform;
    #endregion

    // Use this for initialization
    void Start()
    {
        m_transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.pause) return;
        if (m_pillCount == 1) return;

        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
        {
            //重设计时器
            m_timer = Random.value * 15.0f;
            if (m_timer < 5)
            {
                m_timer = 5;
            }

            //生成药品
            Transform obj = (Transform)Instantiate(m_pill, m_transform.position, m_pill.rotation);

            //获取药品的脚本
            Pill pill = obj.GetComponent<Pill>();

            //初始化药品
            pill.Init(this);
        }
    }
}
