using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ReturnButtonName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReturnName()
    {
        DriveTest.instance.option1.interactable = false;
        DriveTest.instance.option2.interactable = false;
        DriveTest.instance.option3.interactable = false;
        DriveTest.instance.option4.interactable = false;
        //print(DriveTest.instance.splitQuestionAnswers[0]);
        if (transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == DriveTest.instance.splitQuestionAnswers[0])
        {
            //print("this is correct ans");
            transform.GetComponent<Image>().color = Color.green;
            DriveTest.instance.score = DriveTest.instance.score + 1;
            DriveTest.instance.scoreText.GetComponent<TextMeshProUGUI>().text = "Score : "+ DriveTest.instance.score +"/"+ DriveTest.instance.totalQuestions.ToString();
            //PhotonForSpellingQuiz.instance.SendMessageToAll(DriveTest.instance.scoreText.GetComponent<TextMeshProUGUI>().text);
        }
        else
        {
            //print("this is wrong ans");
            transform.GetComponent<Image>().color = Color.red;
            DriveTest.instance.correctAns.GetComponent<Image>().color = Color.green;
        }
        if (DriveTest.instance.questionList.Count > 0)
        {
            DriveTest.instance.next.gameObject.SetActive(true);
        }
        DriveTest.instance.timerRunning = false;
        if(TimerScript.instance.NextQuestionCor == null)
        {
            TimerScript.instance.cameFrom = "ReturnButtonName.cs";
            //TimerScript.instance.NextQuestionCor = StartCoroutine(TimerScript.instance.AutoplayNext());
        }
        if (!DriveTest.instance.scores.activeSelf)
        {
            DriveTest.instance.scores.SetActive(true);
        }
        PhotonForSpellingQuiz.instance.SendMessageToAll(DriveTest.instance.score.ToString()+"/"+ DriveTest.instance.totalQuestions.ToString());
    }
}
