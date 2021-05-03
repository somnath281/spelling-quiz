using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Net;
/// <summary>
/// Sample
/// </summary>
public class DriveTest : MonoBehaviour
{
    public static DriveTest instance;
    //public Transform cube = null;

    public bool timeUp, timerRunning;
    public bool isGameStarted;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //StartCoroutine(InitGoogleDrive());
#if UNITY_STANDALONE
        WebClient wb = new WebClient();
        wb.DownloadFile("https://drive.google.com/uc?export=download&id=1ll7-9bbx0ThDXqT1JPoXhjb_xC5FSyiC", Application.streamingAssetsPath + "/spelling.txt");
        StartCoroutine(ReadFile(Application.streamingAssetsPath + "/spelling.txt", 1));
        //titleText.GetComponent<TextMeshProUGUI>().text = "Pick the correct spelling"+ "\n"+ "0 / " + totalQuestions.ToString();
#elif UNITY_ANDROID
        WebClient wb = new WebClient();
        print(Application.persistentDataPath);
        wb.DownloadFile("https://drive.google.com/uc?export=download&id=1ll7-9bbx0ThDXqT1JPoXhjb_xC5FSyiC", Application.persistentDataPath + "/spelling.txt");
        StartCoroutine(ReadFile(Application.persistentDataPath + "/spelling.txt", 1));
#endif
    }

    void Update()
    {
        //if (cube != null)
        //    cube.RotateAround(Vector3.up, Time.deltaTime);

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        //if (Input.GetKey(KeyCode.A))
        //    StartCoroutine(UploadText());
    }
    

    bool initInProgress = false;

    
    private string[] TotalLines;
    int count = 0;
    public List<string> questionList = new List<string>();
    IEnumerator ReadFile(string filename, float time)
    {
        yield return new WaitForSeconds(time);
        TotalLines = File.ReadAllLines(filename);
        for (int i = 0; i < TotalLines.Length; i++)
        {
            //print(TotalLines[i]);
            questionList.Add(TotalLines[i]);
        }
        totalQuestions = questionList.Count;// *10;
        //print("totalQuestions " + questionList.Count);
        ShowNextOptions();
    }
    public Button option1, option2, option3, option4, next;
    public string[] splitQuestionAnswers;
    [HideInInspector]
    public Button correctAns;
    public GameObject timerGO, titleText, scoreText;
    public int score, questionIndex, totalQuestions;
    public GameObject scores;
    public void ShowNextOptions()
    {
        TimerScript.instance.cameFrom = "";
        if (TimerScript.instance.NextQuestionCor != null)
        {
            StopCoroutine(TimerScript.instance.AutoplayNext());
            TimerScript.instance.NextQuestionCor = null;
        }
        if (questionList.Count > 0)
        {

            int rand = UnityEngine.Random.Range(0, questionList.Count - 1);
            splitQuestionAnswers = questionList[rand].Split('|');
            string[] splitAnswers = splitQuestionAnswers[1].Split(',');

            //option1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = splitAnswers[0];
            //option2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = splitAnswers[1];
            //option3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = splitAnswers[2];
            //option4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = splitAnswers[3];
            int option1Rand = UnityEngine.Random.Range(0, splitAnswers.Length - 1);
            option1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = splitAnswers[option1Rand];
            splitAnswers = splitAnswers.Where(val => val != splitAnswers[option1Rand]).ToArray();

            int option2Rand = UnityEngine.Random.Range(0, splitAnswers.Length - 1);
            option2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = splitAnswers[option2Rand];
            splitAnswers = splitAnswers.Where(val => val != splitAnswers[option2Rand]).ToArray();


            int option3Rand = UnityEngine.Random.Range(0, splitAnswers.Length - 1);
            option3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = splitAnswers[option3Rand];
            splitAnswers = splitAnswers.Where(val => val != splitAnswers[option3Rand]).ToArray();

            option4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = splitAnswers[0];

            if (splitQuestionAnswers[0] == option1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
            {
                correctAns = option1;
            }
            else if (splitQuestionAnswers[0] == option2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
            {
                correctAns = option2;
            }
            else if (splitQuestionAnswers[0] == option3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
            {
                correctAns = option3;
            }
            else if (splitQuestionAnswers[0] == option4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
            {
                correctAns = option4;
            }
            option1.interactable = true;
            option2.interactable = true;
            option3.interactable = true;
            option4.interactable = true;
            Color myColor = new Color(219 / 255, 80 / 255, 203 / 255, 255 / 255);
            option1.GetComponent<Image>().color = myColor;
            option2.GetComponent<Image>().color = myColor;
            option3.GetComponent<Image>().color = myColor;
            option4.GetComponent<Image>().color = myColor;



            questionList.RemoveAt(rand);
            next.gameObject.SetActive(false);
            questionIndex = questionIndex + 1;
            titleText.GetComponent<TextMeshProUGUI>().text = "Pick the correct spelling" + "\n" + questionIndex + "/" + totalQuestions.ToString();
            if (PhotonForSpellingQuiz.instance.isSubscribed)
            {
                print("here we should call StartGame()");
                StartGame();
            }
        }
        else
        {
            
            titleText.GetComponent<TextMeshProUGUI>().text = "Game Over";
            option1.gameObject.SetActive(false);
            option2.gameObject.SetActive(false);
            option3.gameObject.SetActive(false);
            option4.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
        }
    }
    public void SendMessageToAllAndStartGame()
    {
        if (PhotonForSpellingQuiz.instance.lobbyPanel.activeSelf)
        {
            PhotonForSpellingQuiz.instance.lobbyPanel.SetActive(false);
        }
        if (!isGameStarted)
        {
            PhotonForSpellingQuiz.instance.SendMessageToAll(PhotonForSpellingQuiz.instance.UsernameTMP.text+ " started the game!!");
            StartCoroutine(PhotonForSpellingQuiz.instance.HideDebugText());
        }
        StartGame();
    }
    public void StartGame()
    {
        PhotonForSpellingQuiz.instance.debugText.GetComponent<TextMeshProUGUI>().text = "";
        isGameStarted = true;
        if (!option1.GetComponent<EasyTween>().IsObjectOpened())
        {
            option1.GetComponent<EasyTween>().OpenCloseObjectAnimation();
        }
        if (!option2.GetComponent<EasyTween>().IsObjectOpened())
        {
            option2.GetComponent<EasyTween>().OpenCloseObjectAnimation();
        }
        if (!option3.GetComponent<EasyTween>().IsObjectOpened())
        {
            option3.GetComponent<EasyTween>().OpenCloseObjectAnimation();
        }
        if (!option4.GetComponent<EasyTween>().IsObjectOpened())
        {
            option4.GetComponent<EasyTween>().OpenCloseObjectAnimation();
        }
        if (!timerGO.GetComponent<EasyTween>().IsObjectOpened())
        {
            timerGO.GetComponent<EasyTween>().OpenCloseObjectAnimation();
        }
        if (!titleText.GetComponent<EasyTween>().IsObjectOpened())
        {
            titleText.GetComponent<EasyTween>().OpenCloseObjectAnimation();
        }
        if (!scoreText.GetComponent<EasyTween>().IsObjectOpened())
        {
            scoreText.GetComponent<TextMeshProUGUI>().text = "Score : 0/" + totalQuestions.ToString();
            scoreText.GetComponent<EasyTween>().OpenCloseObjectAnimation();
        }
        TimerScript.instance.currentAmount = 10;
        timeUp = false;
        timerRunning = true;
    }

    

    private void OnApplicationQuit()
    {
        //StartCoroutine(Revoke());
    }

   
}
