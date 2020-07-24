using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using PathCreation.Examples;
using PathCreation;
using UnityEngine.SceneManagement;
public class PathLine : MonoBehaviour
{
    public LayerMask _layerMask;
    public Text LineCountText;
    public Text RestartText;
    int LineCount;
    int a;
    public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate { };
    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> pointsFriend = new List<Vector3>();

    public PathCreator pathCreator_Player;
    public PathCreator pathCreator_Friend;
    private BezierPath bezierPath;
    private BezierPath bezierPathFriend;
    public LineRenderer lineRendererPlayer;
    public LineRenderer lineRendererFriend;
    public LineRenderer lineRenderer;

    public GameObject friend;
    public GameObject player;
    public GameObject pathcreatorplayer;
    public GameObject pathcreatorfriend;

    public GameObject target;
    public float movingCharacterPointThreshold;
    public float targetPointThreshold;

    public Boolean camerafollow;

    Animator animatorPlayer;
    Animator animatorFriend;
    

    public void Start()
    {
        animatorPlayer = player.transform.GetChild(0).GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name == "Level6" || SceneManager.GetActiveScene().name == "Level7" || SceneManager.GetActiveScene().name == "Level10")
        {
            animatorFriend = friend.transform.GetChild(0).GetComponent<Animator>();
        }
        player.GetComponent<PathFollower>().enabled = false;
        friend.GetComponent<PathFollower>().enabled = false;
        LineCount = 0;
        LineCountText.text = "Line Count: " + LineCount.ToString();
        RestartText.text = "";
    }

    private void Awake()
    {
        lineRendererPlayer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if (hit)
                {
                    Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                    if (hitInfo.transform.gameObject.tag == "Friend")
                    {
                        lineRendererPlayer.enabled = false;
                        lineRendererFriend.enabled = true;
                        pathcreatorplayer.SetActive(false);
                        pathcreatorfriend.SetActive(true);
                    }
                    
                    if (hitInfo.transform.gameObject.tag == "Player")
                    {
                        lineRendererPlayer.enabled = true;
                        lineRendererFriend.enabled = false;
                        pathcreatorplayer.SetActive(true);
                        pathcreatorfriend.SetActive(false);
                    }
                    
                    if (hitInfo.transform.gameObject.tag == "Ground")
                    {
                        lineRendererPlayer.enabled = false;
                        lineRendererFriend.enabled = false;
                        pathcreatorplayer.SetActive(false);
                        pathcreatorfriend.SetActive(false);
                    }
                }    
        }
        
        if (LineCount == 0)
        {
            drawPathLine();
            LineCountText.text = "Line Count: " + LineCount.ToString();
        }
        
        else if (LineCount == 1)
        {
            drawPathLine();
            LineCountText.text = "Line Count: " + LineCount.ToString();
        }
        
        else if (LineCount == 2)
        {
            LineCountText.text = "You can not draw any more paths";
            RestartText.text = "Press r to restart";
        }
    }

    private string check()
    {
        string ret = "";
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hit)
        {
            if (hitInfo.transform.gameObject.tag == "Friend")
            {
                ret = "Friend";
            }
            
            if (hitInfo.transform.gameObject.tag == "Player")
            {
                ret = "Player";
            }
            
            if (hitInfo.transform.gameObject.tag == "Ground")
            {
                ret = "Ground";
            }
        }
        return ret;
    } 

    private void input()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);      
        }
    }


    private List<Vector3> CreatePath(Transform movingCharacter, Transform target, LineRenderer lineRenderer)
    {
        var points = new List<Vector3>();
        points.Add(movingCharacter.position);
        var linePoints = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePoints);
        foreach (var point in linePoints)
        {
            if (point.z >= movingCharacterPointThreshold && point.z <= targetPointThreshold)
            {
                points.Add(point);
            }
        }
        if (points[points.Count - 1].z <= targetPointThreshold - 2f)
        {
            return null;
        }
        points.Add(target.position);
        return points;
    }

    private void drawPathLine()     //inputu aldığımız kısım (çizim yapılan kısım)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (check() == "Player")
            {
                a = 1;
            }

            else if (check() == "Friend")
            {
                a = 2;
            }

            else if (check() == "Ground")
            {
                a = 3;
            }
            
            points.Clear();
        }

        if (Input.GetButton("Fire1"))
        {
            if (Input.GetButton("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _layerMask, QueryTriggerInteraction.Ignore))
                {
                    if (a == 1)
                    {
                        if (DistanceToLastPoint(hitInfo.point) > 1f)
                        {
                            var point = hitInfo.point;
                            point.y = 0.6f;
                            points.Add(point);
                            lineRenderer.positionCount = points.Count;
                            lineRenderer.SetPositions(points.ToArray());
                        }
                    }

                    if (a == 2)
                    {
                        if (DistanceToLastPoint(hitInfo.point) > 1f)
                        {
                            pointsFriend.Add(hitInfo.point);
                            lineRenderer.positionCount = pointsFriend.Count;
                            lineRenderer.SetPositions(pointsFriend.ToArray());
                        }
                    }
                }
            }
        }

        else if (Input.GetButtonUp("Fire1"))
        {
            {
                if (a == 1) //player
                {
                    camerafollow=true;
                    var pathPoints = CreatePath(player.transform, target.transform, lineRenderer);
                    if (pathPoints != null)
                    {
                        animatorPlayer.SetBool("walk", true);
                        LineCount++;
                        bezierPath = new BezierPath(pathPoints, false, PathSpace.xyz);
                        pathCreator_Player.GetComponent<PathCreator>().bezierPath = bezierPath;
                        bezierPath.GlobalNormalsAngle = 90;
                        player.GetComponent<PathFollower>().enabled = true;
                        friend.GetComponent<PathFollower>().enabled = false;
                    }
                    else
                    {
                        lineRenderer.positionCount = 0;
                        animatorPlayer.SetBool("walk", false);
                    }
                }

                if (a == 2) //friend
                {
                    animatorFriend.SetBool("walk2", true);
                    bezierPathFriend = new BezierPath(pointsFriend, false, PathSpace.xyz);
                    pathcreatorfriend.GetComponent<PathCreator>().bezierPath = bezierPathFriend;
                    bezierPathFriend.GlobalNormalsAngle = 90;
                    player.GetComponent<PathFollower>().enabled = false;
                    friend.GetComponent<PathFollower>().enabled = true;
                    LineCount++;
                }
                else
                {
                    animatorFriend.SetBool("walk2", false);
                }

                if (a == 3) //ground
                {
                    player.GetComponent<PathFollower>().enabled = false;
                    friend.GetComponent<PathFollower>().enabled = false;
                }
     
            }
        }
    }

        private float DistanceToLastPoint(Vector3 point)
        {
            if (a == 1)
            {
                if (!points.Any())
                    return Mathf.Infinity;
                return Vector3.Distance(points.Last(), point);
            }
            if (a == 2)
            {
                if (!pointsFriend.Any())
                    return Mathf.Infinity;
                return Vector3.Distance(pointsFriend.Last(), point);
            }
            return 0;
        }
    
}