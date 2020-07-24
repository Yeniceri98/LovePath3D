using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using PathCreation;
using PathCreation.Examples;
using UnityEngine.SceneManagement;    //oyun bittikten sonra başa dönmemizi sağlayan fonksiyonun çalışması için bu kütüphaneye ihtiyaç var

public class PlayerController : MonoBehaviour
{
    
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private int count;          //count tutmak için
    public Text countText;
    public Text InformativeText;

    private bool fly;
    private string checkenemy;
    Vector3 myVectorthis, myVectorenemy = new Vector3(0.0f, 1.0f, 0.0f);
    private float i, j, k;
    public GameObject enemy1, enemy2, enemy3, enemy4;
    public GameObject player, target, friend;

    Animator animatorPlayer;
    Animator animatorFriend;
    Animator animatorTarget;


    private void Start()
    {
        animatorPlayer = player.transform.GetChild(0).GetComponent<Animator>();
        animatorTarget = target.transform.GetChild(0).GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name == "Level6" || SceneManager.GetActiveScene().name == "Level7" || SceneManager.GetActiveScene().name == "Level10")
        {
            animatorFriend = friend.transform.GetChild(0).GetComponent<Animator>();
        }
        count = 0;
        setCountText();
        InformativeText.text = "";
    }

    private void Awake()
    {
        FindObjectOfType<PathLine>().OnNewPathCreated += SetPoints;
    }

    private void SetPoints(IEnumerable<Vector3> points)
    {
        pathPoints = new Queue<Vector3>(points);
    }


    void Update()
    {
        if (fly)
        {
            if (checkenemy == "Enemy1")
            {
                i += 0.1f;
                myVectorthis = new Vector3(0.0f, i, 0.0f);
                myVectorenemy = new Vector3(0.0f, i, 0.0f);
                if (enemy1.GetComponent<PathFollower>() != null)
                {
                    enemy1.GetComponent<PathFollower>().enabled = false;
                }
                this.gameObject.transform.Translate(myVectorthis);
                enemy1.gameObject.transform.Translate(myVectorenemy);
            }

            if (checkenemy == "Enemy2")
            {
                j += 0.1f;
                myVectorthis = new Vector3(0.0f, j, 0.0f);
                myVectorenemy = new Vector3(0.0f, j, 0.0f);
                this.gameObject.transform.Translate(myVectorthis);
                enemy2.gameObject.transform.Translate(myVectorenemy);
            }

            if (checkenemy == "Enemy3")
            {
                k += 0.1f;
                myVectorthis = new Vector3(0.0f, k, 0.0f);
                myVectorenemy = new Vector3(0.0f, k, 0.0f);
                this.gameObject.transform.Translate(myVectorthis);
                enemy3.gameObject.transform.Translate(myVectorenemy);
            }
        }

        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }   
    }


    void OnTriggerEnter(Collider other)     //player-pick up etkileşimi
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;    //player pick upları topladıkça count artmış olacak
            setCountText();
        }

        if (other.gameObject.CompareTag("Target"))
        {
            
            InformativeText.text = "Bravo. You fallen in love!";
            Invoke("LoadNextScene", 3);
            
            animatorPlayer.SetBool("walk", false);
            animatorPlayer.Play("kiss");
            animatorTarget.Play("kiss");
            //other.transform.GetChild(0).GetComponent<Animator>().Play("kiss");

        }


        if (other.gameObject.CompareTag("Enemy1"))
        {
            if (this.CompareTag("Friend"))
            {
                //other.gameObject.SetActive(false);  (nesnenin yok olmasını sağlar)
                animatorFriend.SetBool("walk2", false);
                this.GetComponent<PathFollower>().enabled = false;
                fly = true;
                checkenemy = "Enemy1";
            }
            else
            {
                player.GetComponent<PathFollower>().enabled = false;
                animatorPlayer.SetBool("walk", false);
                animatorPlayer.Play("death");
                InformativeText.text = "It's over :(";
                Invoke("Restart", 2);
            }
        }

        if (other.gameObject.CompareTag("Enemy2"))
        {
            if (this.CompareTag("Friend"))
            {
                animatorFriend.SetBool("walk2", false);
                this.GetComponent<PathFollower>().enabled = false;
                fly = true;
                checkenemy = "Enemy2";
            }
            else
            {
                player.GetComponent<PathFollower>().enabled = false;
                animatorPlayer.SetBool("walk", false);
                animatorPlayer.Play("death");
                InformativeText.text = "It's over :(";
                Invoke("Restart", 2);
            }
        }

        if (other.gameObject.CompareTag("Enemy3"))
        {
            if (this.CompareTag("Friend"))
            {
                animatorFriend.SetBool("walk2", false);
                this.GetComponent<PathFollower>().enabled = false;
                fly = true;
                checkenemy = "Enemy3";
            }
            else
            {
                player.GetComponent<PathFollower>().enabled = false;
                animatorPlayer.SetBool("walk", false);
                animatorPlayer.Play("death");
                InformativeText.text = "It's over :(";
                Invoke("Restart", 2);
            }
        }

        if (other.gameObject.CompareTag("Enemy4"))
        {
            if (this.CompareTag("Friend"))
            {
                animatorFriend.SetBool("walk2", false);
                this.GetComponent<PathFollower>().enabled = false;
                fly = true;
                checkenemy = "Enemy3";
            }
            else
            {
                player.GetComponent<PathFollower>().enabled = false;
                animatorPlayer.SetBool("walk", false);
                animatorPlayer.Play("death");
                InformativeText.text = "It's over :(";
                Invoke("Restart", 2);
            }
        }

    }

     private void LoadNextScene()
     {
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
     }

     private void Restart()      //Yandıktan sonra sahnenin tekrardan yüklenmesi
     {
         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
     }

     void setCountText()
     {
         countText.text = "Count: " + count.ToString();
     }
}


