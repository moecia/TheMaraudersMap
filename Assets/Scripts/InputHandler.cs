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
    [SerializeField] private Transform moverReference;

    [SerializeField] private GameObject footStep_L;
    [SerializeField] private GameObject footStep_R;
    [SerializeField] private float generateInteval = 5.0f;
    private float timer;
    private int leftOrRight = 0;    // 0: L, 1: R.

    private List<GameObject> footStepCache = new List<GameObject>();

    [SerializeField] public Animator footstepMarker;

    [SerializeField] private Vector3 testPos;

    // Use this for initialization
    void Start () 
    {
        lastPos = moverReference.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // print("Mouse Pos: " + Input.mousePosition);
        // print("Convert to World Pos: " + Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (startMove)
            GenerateFootstep();
    }

    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(1.0f);
        startMove = true;
    }

    private void GenerateFootstep()
    {
        if (Mathf.Abs(moverReference.position.x - newPos.x) <= .1f
         && Mathf.Abs(moverReference.position.y - newPos.y) <= .1f)
        {
            footstepMarker.SetTrigger("Appear");
            print("Arrival: " + System.DateTime.Now.ToString());
            lastPos = newPos;
            startMove = false;
        }
        else
        {
            moverReference.transform.position += moveSpeed * direction * Time.deltaTime;
            if (timer < generateInteval)
                timer += Time.deltaTime;
            else
            {
                var dir = newPos - lastPos;
                float angle = -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
                Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);

                footstepMarker.transform.rotation = newRot;

                if (leftOrRight == 0)
                {
                    GameObject temp = Instantiate(footStep_L,
                        moverReference.transform.position,
                        newRot);
                    footStepCache.Add(temp);
                    leftOrRight = 1;
                }
                else
                {
                    GameObject temp = Instantiate(footStep_R,
                        moverReference.transform.position,
                        newRot);
                    footStepCache.Add(temp);
                    leftOrRight = 0;
                }
                timer = 0;
            }
        }
    }

    public void SetNewPosition(string pos)
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
        int posX = int.Parse(x);
        int posY = int.Parse(y);

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

        if (useViewportPos)
            newPos = Camera.main.ScreenToWorldPoint(new Vector3(posX, posY));            
        newPos.z = 0;
        direction = (newPos - lastPos).normalized;    
        
        timer = generateInteval;

        if (!startMove)
            footstepMarker.SetTrigger("Disappear");

        StartCoroutine(DelayStart());
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
