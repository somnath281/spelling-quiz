using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TimerScript : MonoBehaviour
{
    public static TimerScript instance;
    public Transform loadingBar;
    public Transform textIndicator;
    public float currentAmount;
    [SerializeField] private float speed;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (DriveTest.instance.isGameStarted && DriveTest.instance.timerRunning)
        {
            if (currentAmount > 0)
            {
                currentAmount -= speed * Time.deltaTime;
                textIndicator.GetComponent<TextMeshProUGUI>().text = ((int)currentAmount).ToString();
            }
            loadingBar.GetComponent<Image>().fillAmount = currentAmount / 10;
            if (loadingBar.GetComponent<Image>().fillAmount == 0 && !DriveTest.instance.timeUp)
            {
                DriveTest.instance.timeUp = true;
                DriveTest.instance.option1.interactable = false;
                DriveTest.instance.option2.interactable = false;
                DriveTest.instance.option3.interactable = false;
                DriveTest.instance.option4.interactable = false;
                DriveTest.instance.correctAns.GetComponent<Image>().color = Color.green;
                DriveTest.instance.next.gameObject.SetActive(true);
                DriveTest.instance.timerRunning = false;
                if (NextQuestionCor == null)
                {
                    cameFrom = "TimeScript.cs";
                    NextQuestionCor = StartCoroutine(AutoplayNext());
                }
            }
        }
    }
    public string cameFrom = "";
    public Coroutine NextQuestionCor;
    public IEnumerator AutoplayNext()
    {
        yield return new WaitForSeconds(5);
        print("cameFrom " + cameFrom);
        if (!DriveTest.instance.timerRunning && cameFrom != "" && NextQuestionCor != null)
        {
            //DriveTest.instance.ShowNextOptions();
        }
    }
}
