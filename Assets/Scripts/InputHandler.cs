using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputHandler : MonoBehaviour {

    [SerializeField] private bool useViewportPos = true;
    [SerializeField] private Vector3 newPos;
    private Vector3 lastPos = new Vector3(0, 0);
    private Vector3 direction;

    private bool startMove = false;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private Transform moverReference;

    [SerializeField] private GameObject footStep_L;
    [SerializeField] private GameObject footStep_R;
    [SerializeField] private float generateInteval = 5.0f;
    private float timer;
    private int leftOrRight = 1;    // 0: L, 1: R.

    private List<GameObject> footStepCache = new List<GameObject>();

	// Use this for initialization
	void Start () 
    {
        moverReference.position = lastPos;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //print("Mouse Pos: " + Input.mousePosition);
        //print("Convert to World Pos: " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (startMove)
            GenerateFootstep();
    }

    private void GenerateFootstep()
    {
        if (Mathf.Abs(moverReference.position.x - newPos.x) <= .1f
         && Mathf.Abs(moverReference.position.y - newPos.y) <= .1f)
        {
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
                if (leftOrRight == 0)
                {
                    GameObject temp = Instantiate(footStep_L,
                        moverReference.transform.position,
                        newRot);
                    print(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
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

    /// <summary>
    /// Needs enter newPos parameter.
    /// </summary>
    //public void SetNewPosition(Vector3 newPos)
    //{
    //    foreach (GameObject obj in footStepCache)
    //        Destroy(obj);
    //    footStepCache.Clear();
    //    if (useViewportPos)
    //    {
    //        this.newPos = Camera.main.ScreenToWorldPoint(newPos);
    //        this.newPos.z = 0;
    //    }
    //    else
    //    {
    //        this.newPos = newPos;
    //        this.newPos.z = 0;
    //    }
    //    direction = (newPos - lastPos).normalized;
    //    timer = generateInteval;
    //    startMove = true;
    //}

    /// <summary>
    /// Don't need enter newPos parameter.
    /// </summary>
    public void SetNewPosition()
    {
        foreach (GameObject obj in footStepCache)
            Destroy(obj);
        footStepCache.Clear();
        if (useViewportPos)
            newPos = Camera.main.ScreenToWorldPoint(newPos);            
        newPos.z = 0;
        direction = (newPos - lastPos).normalized;
        print(direction);
        timer = generateInteval;
        startMove = true;
    }

    // Deleted the following in build ver.
    [CustomEditor(typeof(InputHandler))]
    public class InputHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InputHandler ih = (InputHandler)target;
            if (GUILayout.Button("Set Position"))
            {
                ih.SetNewPosition();
            }
        }
    }
}
