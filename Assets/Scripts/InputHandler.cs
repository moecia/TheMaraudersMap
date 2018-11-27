using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class InputHandler : MonoBehaviour {

    [SerializeField] private bool useViewportPos = true;
    [SerializeField] private Vector3 newPos;
    private Vector3 lastPos;
    private Vector3 direction;

    private bool startMove = false;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private Transform[] moverReference = new Transform[2];
    [SerializeField] public int currentPlayer = 0;

    [SerializeField] private GameObject footStep_L;
    [SerializeField] private GameObject footStep_R;
    [SerializeField] private float generateInteval = 5.0f;
    private float timer;
    private int leftOrRight = 0;    // 0: L, 1: R.

    private List<GameObject> footStepCache = new List<GameObject>();

    [SerializeField] public Animator[] footstepMarker = new Animator[2];

    [SerializeField] private Vector3 testPos;

    // Use this for initialization
    void Start () 
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (startMove)
            GenerateFootstep();
    }

    public void SetInitialPos(string s)
    {
        int posX, posY;
        DecodePositionString(s, out posX, out posY);

        if (useViewportPos)
            newPos = Camera.main.ScreenToWorldPoint(new Vector3(posX, posY));
        newPos.z = 0;
        moverReference[currentPlayer].position = newPos;
    }

    public void ActivatePlayer(int index)
    {
        currentPlayer = index;
        startMove = false;
        if (index == 0)
        {
            moverReference[0].gameObject.SetActive(true);
            moverReference[1].gameObject.SetActive(false);
        }
        else if (index == 1)
        {
            moverReference[1].gameObject.SetActive(true);
            moverReference[0].gameObject.SetActive(false);
        }
    }

    public void SetNewPosition(string pos)
    {
        int posX, posY;
        DecodePositionString(pos, out posX, out posY);

        if (footStepCache.Count == 0 || footStepCache[footStepCache.Count - 1].name == "Footstep_R(Clone)")
            leftOrRight = 0;
        else if (footStepCache[footStepCache.Count - 1].name == "Footstep_L(Clone)")
            leftOrRight = 1;

        if (footStepCache.Count > 20)
        {
            foreach (GameObject obj in footStepCache)
                Destroy(obj);
            footStepCache.Clear();
        }

        lastPos = moverReference[currentPlayer].position;
        if (useViewportPos)
            newPos = Camera.main.ScreenToWorldPoint(new Vector3(posX, posY));
        newPos.z = 0;
        direction = (newPos - lastPos).normalized;

        timer = generateInteval;

        if (!startMove)
            footstepMarker[currentPlayer].SetTrigger("Disappear");

        StartCoroutine(DelayStart());
    }

    private static void DecodePositionString(string pos, out int posX, out int posY)
    {
        string x = "";
        string y = "";
        int j = 0;
        for (int i = 0; i < pos.Length; i++)
        {
            if (pos[i] != ',')
                x += pos[i];
            else
            {
                j = i + 1;
                break;
            }
        }
        while (j < pos.Length)
            y += pos[j++];
        posX = int.Parse(x);
        posY = int.Parse(y);
    }

    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(1.0f);
        startMove = true;
    }

    private void GenerateFootstep()
    {
        if (Mathf.Abs(moverReference[currentPlayer].position.x - newPos.x) <= .1f
         && Mathf.Abs(moverReference[currentPlayer].position.y - newPos.y) <= .1f)
        {
            footstepMarker[currentPlayer].SetTrigger("Appear");
            print("Arrival: " + System.DateTime.Now.ToString());
            lastPos = newPos;
            startMove = false;
        }
        else
        {
            moverReference[currentPlayer].transform.position += moveSpeed * direction * Time.deltaTime;
            if (timer < generateInteval)
                timer += Time.deltaTime;
            else
            {
                var dir = newPos - lastPos;
                float angle = -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
                Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);

                footstepMarker[currentPlayer].transform.rotation = newRot;

                if (leftOrRight == 0)
                {
                    GameObject temp = Instantiate(footStep_L,
                        moverReference[currentPlayer].transform.position,
                        newRot);
                    footStepCache.Add(temp);
                    leftOrRight = 1;
                }
                else
                {
                    GameObject temp = Instantiate(footStep_R,
                        moverReference[currentPlayer].transform.position,
                        newRot);
                    footStepCache.Add(temp);
                    leftOrRight = 0;
                }
                timer = 0;
            }
        }
    }

    // Deleted the following in build ver.
    //[CustomEditor(typeof(InputHandler))]
    //public class InputHandlerEditor : Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        DrawDefaultInspector();

    //        InputHandler ih = (InputHandler)target;
    //        if (GUILayout.Button("Set Position"))
    //        {
    //            ih.SetNewPosition(ih.testPos.x.ToString() + "," + ih.testPos.y.ToString());
    //        }
    //    }
    //}
}
