using UnityEngine;
using System.Collections;

public class EnemySpawn : MonoBehaviour
{
    #region 字段
    //敌人的Prefab
    public Transform m_enemy;
    //生成的敌人数量
    public int m_enemyCount = 0;
    //敌人的最大生成数量
    public int m_maxEnemy = 3;
    //生成敌人的时间间隔
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
        //如果生成敌人的数量超过最大值，停止生成敌人
        if (m_enemyCount >= m_maxEnemy) return;

        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
        {
            //重设计时器
            m_timer = Random.value * 15.0f;
            if (m_timer < 5)
            {
                m_timer = 5;
            }

            //生成敌人
            //随机生成位置
            Vector3 position = m_transform.position;
            //float x = Random.value * 15.0f;
            //float z = Random.value * 15.0f;
            //position.x += x;
            //position.z += z;

            Transform obj = (Transform)Instantiate(m_enemy, position, m_enemy.rotation);

            //获取敌人的脚本
            Enemy enemy = obj.GetComponent<Enemy>();

            //初始化敌人
            enemy.Init(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Spawn.tif", true);
    }
}
