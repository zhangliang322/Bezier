using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BezierArrows : MonoBehaviour
{
    #region Public Fields
    [Tooltip("The Prefab of arrow head;")]
    public GameObject ArrowHeadPrefab;

    [Tooltip("The Prefab of arrow node;")]
    public GameObject ArrowNodePrefab;

    [Tooltip("The number of arrow nodes;")]
    public int arrowNodeNum;

    [Tooltip("The scale multiplier for arrow nodes;")]
    public float scaleFactor = 1f;

    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Private Fields
    
    //The position of arrows emitter point初始点
    private RectTransform origin;


    //list of arrow nodes' transform 中间链接点位置
    private List<RectTransform> arrowNodes = new List<RectTransform>();


    //list of control points 控制中间点朝向
    private List<Vector2> controlPoints = new List<Vector2>();


    //determine the position of control point P1-P2位置相对于P0 P3的位置向量 直接给定向量因子
    private readonly List<Vector2> controlPointFactors = new List<Vector2> { new Vector2(-0.3f, 0.8f), new Vector2(0.1f, 1.4f) };

    #endregion


    #region Private Method

    private void Awake()
    {
        //get position of emitter point 起始点
        this.origin = this.GetComponent<RectTransform>();

        //instantiates the nodes and head; 按数量加入中间节点
        for(int i=0;i<this.arrowNodeNum;++i)
        {
            this.arrowNodes.Add(Instantiate(this.ArrowNodePrefab, this.transform).GetComponent<RectTransform>());
        }
        //尾巴节点
        this.arrowNodes.Add(Instantiate(this.ArrowHeadPrefab, this.transform).GetComponent<RectTransform>());

        //4 control pint 设置四个贝塞尔控制点
        for (int i = 0; i < 4; ++i)
        {
            this.controlPoints.Add(Vector2.zero);
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
        //P0 emitter point 第一个发射点
        this.controlPoints[0] = new Vector2(this.origin.position.x, this.origin.position.y);

        //P3 mouse position 鼠标终点位置
        this.controlPoints[3] = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        //P1 P2position 根据三阶贝塞尔曲线公式，由P0和P3计算中间点 获得中间两个贝塞尔 控制节点的位置
        // p1 = p0+(p3-p0) *vector2(-0.3f,0.8f)
        //p2 = p0+(p3-p0) * vector2(0.1f,1.4f)
        this.controlPoints[1] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[0];
        this.controlPoints[2] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[1];

        for (int i = 0; i < this.arrowNodes.Count; ++i)
        {
            //calculates t 根据节点数量确定t，并呈现疏密区别
            var t = Mathf.Log(1f * i / (this.arrowNodes.Count - 1) + 1f, 2f);

            //贝塞尔公式
            //B(t) = (1-t)^3 * p0 + 3*(1-t)^2 *t *p1 +3*(1-t) * t^2 * p2 +t^3 *p3;
            this.arrowNodes[i].position =
                 Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 0) * this.controlPoints[0] +
             3 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 1) * this.controlPoints[1] +
             3 * Mathf.Pow(1 - t, 1) * Mathf.Pow(t, 2) * this.controlPoints[2] +
                 Mathf.Pow(1 - t, 0) * Mathf.Pow(t, 3) * this.controlPoints[3];

            //cal rotation for each arrow node 计算每个节点方向
            if (i > 0)
            {
                var euler = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, this.arrowNodes[i].position - this.arrowNodes[i - 1].position));
                this.arrowNodes[i].rotation = Quaternion.Euler(euler);
            }

            //设置缩放，起点小结尾大
            var scale = this.scaleFactor * (1f - 0.03f * (this.arrowNodes.Count - 1 - i));
            this.arrowNodes[i].localScale = new Vector3(scale, scale, 1f);


        }

        //初始点方向
        this.arrowNodes[0].transform.rotation = this.arrowNodes[1].transform.rotation;

    }
    #endregion
}
