using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    #region 字段
    //Transform组件
    Transform m_transform;
    //寻路组件
    UnityEngine.AI.NavMeshAgent m_agent;
    //动画组件
    Animator m_ani;

    //移动速度
    public float m_movSpeed = 0.5f;
    //旋转速度
    public float m_rotSpeed = 120;
    //计算器
    float m_timer = 2;
    //生命值
    public int m_life = 15;
    //攻击范围
    public float m_attackRange = 40f;
    //停止距离
    public float m_stopDistance = 10.0f;
    float m_BackTimer = 0;

    //主角
    Player m_player;

    //出生点
    protected EnemySpawn m_spawn;
    //敌人生成器模式
    public int CurrentModeNumber = 1;
    #endregion

    // Use this for initialization
    void Start()
    {
        //获取组件
        m_transform = this.transform;
        m_agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_ani = this.GetComponent<Animator>();

        //获取主角
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (Vector3.Distance(m_transform.position, m_player.m_transform.position) > m_attackRange)
        {
            m_ani.SetBool("idle", true);
            return;
        }
        //设置寻路目标
        m_agent.SetDestination(m_player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.pause)
        {
            m_agent.Stop();
            m_ani.SetBool("run", false);
            m_ani.SetBool("attack", false);
            m_ani.SetBool("idle", true);
            return;
        }

        m_BackTimer += Time.deltaTime;
        //如果主角生命值为0，则什么也不做
        if (m_player.m_life <= 0) return;

        RotateTo();

        //获取当前动画状态
        AnimatorStateInfo stateInfo = m_ani.GetCurrentAnimatorStateInfo(0);

        #region 如果处于待机状态
        //m_ani.IsInTransition(0) 判断是否在动画过渡过程中
        if (stateInfo.nameHash == Animator.StringToHash("Base Layer.idle") && !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("idle", false);

            if (Vector3.Distance(m_transform.position, m_player.m_transform.position) > m_attackRange)
            {
                m_ani.SetBool("idle", true);
                return;
            }

            //待机一段时间
            m_timer -= Time.deltaTime;
            if (m_timer > 0) return;

            //如果与主角距离小于m_stopDistance米，则进入攻击状态
            if (Vector3.Distance(m_transform.position, m_player.m_transform.position) < m_stopDistance)
            {
                //进入攻击动画状态
                m_ani.SetBool("attack", true);
            }
            else
            {
                //重置计时器
                m_timer = 1;
                //设置目标点
                m_agent.SetDestination(m_player.m_transform.position);
                //进入跑步动画状态
                m_ani.SetBool("run", true);
            }
        }
        #endregion

        #region 如果处于跑步状态
        if (stateInfo.nameHash == Animator.StringToHash("Base Layer.run") && !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("run", false);

            if (Vector3.Distance(m_transform.position, m_player.m_transform.position) > m_attackRange)
            {
                m_ani.SetBool("idle", true);
                return;
            }

            //每隔一秒重新定位主角
            m_timer -= Time.deltaTime;
            if (m_timer < 0)
            {
                m_timer = 1;
                m_agent.SetDestination(m_player.m_transform.position);
            }

            //追向主角
            MoveTo();

            //如果与主角距离小于m_stopDistance米，则进入攻击状态
            if (Vector3.Distance(m_transform.position, m_player.m_transform.position) < m_stopDistance)
            {
                //停止寻路
                m_agent.ResetPath();
                m_ani.SetBool("attack", true);
            }

        }
        #endregion

        #region 如果处于攻击状态
        if (stateInfo.nameHash == Animator.StringToHash("Base Layer.attack") && !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("attack", false);
            RotateTo();

            if (Vector3.Distance(m_transform.position, m_player.m_transform.position) > m_attackRange)
            {
                m_ani.SetBool("idle", true);
                return;
            }

            ////如果动画播完，进入待机状态
            //Animator.GetCurrentAnimatorStateInfo(0).normalizedTime
            //这个返回一个float，大于等于1表示动画播放完一次
            if (stateInfo.normalizedTime >= 1.0f)
            {
                m_ani.SetBool("idle", true);
                //重置计时器
                m_timer = 2;
                //减少主角生命
                m_player.OnDamage(1);
            }
        }
        #endregion

        #region 如果处于死亡状态
        if (stateInfo.nameHash == Animator.StringToHash("Base Layer.death") && !m_ani.IsInTransition(0))
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                OnDeath();
            }
        }
        #endregion

        //m_agent.SetDestination(m_player.transform.position);
        //MoveTo();
    }

    void MoveTo()
    {
        if (GameManager.instance.pause) return;
        float speed = m_movSpeed * Time.deltaTime;
        m_agent.Move(m_transform.TransformDirection(new Vector3(0, 0, speed)));
    }

    /// <summary>
    /// 使敌人始终旋转到面向主角的角度
    /// </summary>
    void RotateTo()
    {
        if (GameManager.instance.pause) return;
        //当前角度
        Vector3 oldAngle = m_transform.eulerAngles;

        //获得面向主角的水平方向的角度(只需要在水平方向面向主角)
        m_transform.LookAt(m_player.m_transform);   //当该物体设置了LookAt并指定了目标物体时，该物体的z轴将始终指向目标物体
        float target = m_transform.eulerAngles.y;

        //转向主角
        float speed = m_rotSpeed * Time.deltaTime;
        float angle = Mathf.MoveTowardsAngle(oldAngle.y, target, speed);
        m_transform.eulerAngles = new Vector3(0, angle, 0);
    }

    /// <summary>
    /// 减少敌人生命，要传递给Player脚本调用
    /// </summary>
    /// <param name="damage">生命值减少的数值</param>
    //计算器

    public void OnDamage(int damage)
    {
        if (GameManager.instance.pause) return;
        m_life -= damage;
        if (m_BackTimer >= 2)
        {
            Vector3 position = transform.forward;
            //Debug.Log(position);
            this.transform.position -= position * 3;
            m_BackTimer = 0;
        }

        if (m_life <= 0)
        {
            m_ani.SetBool("death", true);
        }
    }

    public void Init(EnemySpawn spawn)
    {
        if (GameManager.instance.pause) return;
        m_spawn = spawn;
        m_spawn.m_enemyCount++;
    }

    public int score = 100;

    public void OnDeath()
    {
        if (GameManager.instance.pause) return;
        if (CurrentModeNumber == 2)
        {
            //更新敌人数量
            m_spawn.m_enemyCount--;
        }

        //加分
        GameManager.instance.SetScore(score);
        //摧毁自身
        Destroy(this.gameObject);

        if (CurrentModeNumber == 3)
        {
            GameManager.instance.win = true;
        }
    }

    //红色血条贴图
    public Texture2D blood_red;
    //黑色血条贴图
    public Texture2D blood_black;
    void OnGUI()
    {
        if (CurrentModeNumber == 3)
        {
            if (m_life <= 0) return;

            //计算出血条的宽高
            Vector2 bloodSize = GUI.skin.label.CalcSize(new GUIContent(blood_red));
            //通过血值计算红色血条显示区域
            int blood_height = blood_red.height * m_life / 100;

            //先绘制黑色血条
            GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, bloodSize.x, bloodSize.y * 0.6f), blood_black);
            //在绘制红色血条
            GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, bloodSize.x, blood_height * 0.6f), blood_red);

        }
    }
}
