using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    #region 主角字段
    //组件
    public Transform m_transform;
    CharacterController m_ch;
    //角色移动速度
    public float m_movSpeed = 30.0f;
    //生命值
    public int m_life = 5;
    ////重力
    //public float m_gravity = 0.02f;
    #endregion
    #region 相机字段
    //摄像机Transform
    Transform m_camTransform;
    //摄像机旋转角度
    Vector3 m_camRot;
    //摄像机高度
    float m_camHeight = 1.4f;
    #endregion
    #region 射击相关字段
    //枪口transform
    Transform m_muzzlepoint;
    //射击可以碰到的碰撞层次
    public LayerMask m_layer;
    //射击时的音效
    public AudioClip m_audio;
    //射击时的粒子效果
    public Transform m_fx;
    //射击间隔计时器
    float m_shootTimer = 0;
    bool isJump = false;
    #endregion

    // Use this for initialization
    void Start()
    {
        //获取组件
        m_transform = this.transform;
        m_ch = this.GetComponent<CharacterController>();

        //获取摄像机
        m_camTransform = Camera.main.transform;
        //设置摄像机初始位置
        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;

        m_camTransform.rotation = m_transform.rotation;
        m_camRot = m_camTransform.eulerAngles;  //eulerAngles:欧拉角度

        //射击相关
        m_muzzlepoint = m_camTransform.FindChild("M16/muzzlepoint").transform;

        ////锁定鼠标
        //Screen.lockCursor = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.pause) return;
        float xm = 0, ym = 0, zm = 0;

        if (m_transform.position.y > 10)
        {
            ym -= Time.deltaTime * 50;
        }
        else
        {
            isJump = false;
        }

        //获得鼠标移动距离
        float rh = Input.GetAxis("Mouse X");    //horizontal 水平方向上
        float rv = Input.GetAxis("Mouse Y");    //vertical 竖直方向上

        //旋转摄像机
        m_camRot.x -= rv * 2;
        m_camRot.y += rh * 2;
        m_camTransform.eulerAngles = m_camRot;

        //使主角的面向与摄像机一致
        Vector3 camRot = m_camTransform.eulerAngles;
        camRot.x = 0;
        camRot.z = 0;
        m_transform.eulerAngles = camRot;

        #region 接受用户键盘操作
        //上下
        if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
        {
            zm += m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
        {
            zm -= m_movSpeed * Time.deltaTime;
        }
        //左右
        if (Input.GetKey("left") || Input.GetKey(KeyCode.A))
        {
            xm -= m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey("right") || Input.GetKey(KeyCode.D))
        {
            xm += m_movSpeed * Time.deltaTime;
        }
        //空格
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            isJump = true;
            ym += 15.0f;
        }
        #endregion

        //移动
        m_ch.Move(m_transform.TransformDirection(new Vector3(xm, ym, zm)));

        //使摄像机位置与主角一致
        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;

        //射击相关
        //更新射击间隔时间
        m_shootTimer -= Time.deltaTime;
        //鼠标左键射击
        if (Input.GetMouseButton(0) && m_shootTimer <= 0)
        {
            m_shootTimer = 0.1f;
            //播放射击音效
            this.GetComponent<AudioSource>().PlayOneShot(m_audio);
            //减少弹药，更新弹药UI
            GameManager.instance.SetAmmo(1);
            //RaycastHit用来保存射线的探测结果
            RaycastHit info;
            bool hit = Physics.Raycast(m_muzzlepoint.position, m_camTransform.TransformDirection(Vector3.forward), out info, 100, m_layer);
            //Debug.Log(m_muzzlepoint.position);
            if (hit)
            {
                //如果射中了Tag为Enemy的游戏体
                if (info.transform.tag.CompareTo("Enemy") == 0)
                {
                    Enemy enemy = info.transform.GetComponent<Enemy>();
                    //敌人减少生命
                    enemy.OnDamage(1);

                }
                //在射中的地方释放一个粒子效果
                Instantiate(m_fx, info.point, transform.rotation);
                //Vector3 fwd = m_camTransform.TransformDirection(Vector3.forward);
                //n.rigidbody.AddForce(fwd * 1000);

            }
        }
    }

    /// <summary>
    /// 绘制一个图标在场景中表示当前对象，方便观察用。
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
    }

    /// <summary>
    /// 减少主角生命，要传递给Enemy脚本调用
    /// </summary>
    /// <param name="damage">生命值减少的数值</param>
    public void OnDamage(int damage)
    {
        if (GameManager.instance.pause) return;
        m_life -= damage;
        GameManager.instance.SetLife(m_life);
        if (m_life <= 0)
        {
            Screen.lockCursor = false;
        }
    }

    /// <summary>
    /// 回复主角生命，要传递给Pill脚本调用
    /// </summary>

    public AudioClip m_audioPickUp;
    public void CellPickUp()
    {
        if (GameManager.instance.pause) return;
        m_life += 1;
        this.GetComponent<AudioSource>().PlayOneShot(m_audioPickUp);
        GameManager.instance.SetLife(m_life);
    }
}