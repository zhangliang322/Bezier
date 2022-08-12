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
    
    //The position of arrows emitter point��ʼ��
    private RectTransform origin;


    //list of arrow nodes' transform �м����ӵ�λ��
    private List<RectTransform> arrowNodes = new List<RectTransform>();


    //list of control points �����м�㳯��
    private List<Vector2> controlPoints = new List<Vector2>();


    //determine the position of control point P1-P2λ�������P0 P3��λ������ ֱ�Ӹ�����������
    private readonly List<Vector2> controlPointFactors = new List<Vector2> { new Vector2(-0.3f, 0.8f), new Vector2(0.1f, 1.4f) };

    #endregion


    #region Private Method

    private void Awake()
    {
        //get position of emitter point ��ʼ��
        this.origin = this.GetComponent<RectTransform>();

        //instantiates the nodes and head; �����������м�ڵ�
        for(int i=0;i<this.arrowNodeNum;++i)
        {
            this.arrowNodes.Add(Instantiate(this.ArrowNodePrefab, this.transform).GetComponent<RectTransform>());
        }
        //β�ͽڵ�
        this.arrowNodes.Add(Instantiate(this.ArrowHeadPrefab, this.transform).GetComponent<RectTransform>());

        //4 control pint �����ĸ����������Ƶ�
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
        //P0 emitter point ��һ�������
        this.controlPoints[0] = new Vector2(this.origin.position.x, this.origin.position.y);

        //P3 mouse position ����յ�λ��
        this.controlPoints[3] = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        //P1 P2position �������ױ��������߹�ʽ����P0��P3�����м�� ����м����������� ���ƽڵ��λ��
        // p1 = p0+(p3-p0) *vector2(-0.3f,0.8f)
        //p2 = p0+(p3-p0) * vector2(0.1f,1.4f)
        this.controlPoints[1] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[0];
        this.controlPoints[2] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[1];

        for (int i = 0; i < this.arrowNodes.Count; ++i)
        {
            //calculates t ���ݽڵ�����ȷ��t����������������
            var t = Mathf.Log(1f * i / (this.arrowNodes.Count - 1) + 1f, 2f);

            //��������ʽ
            //B(t) = (1-t)^3 * p0 + 3*(1-t)^2 *t *p1 +3*(1-t) * t^2 * p2 +t^3 *p3;
            this.arrowNodes[i].position =
                 Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 0) * this.controlPoints[0] +
             3 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 1) * this.controlPoints[1] +
             3 * Mathf.Pow(1 - t, 1) * Mathf.Pow(t, 2) * this.controlPoints[2] +
                 Mathf.Pow(1 - t, 0) * Mathf.Pow(t, 3) * this.controlPoints[3];

            //cal rotation for each arrow node ����ÿ���ڵ㷽��
            if (i > 0)
            {
                var euler = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, this.arrowNodes[i].position - this.arrowNodes[i - 1].position));
                this.arrowNodes[i].rotation = Quaternion.Euler(euler);
            }

            //�������ţ����С��β��
            var scale = this.scaleFactor * (1f - 0.03f * (this.arrowNodes.Count - 1 - i));
            this.arrowNodes[i].localScale = new Vector3(scale, scale, 1f);


        }

        //��ʼ�㷽��
        this.arrowNodes[0].transform.rotation = this.arrowNodes[1].transform.rotation;

    }
    #endregion
}
