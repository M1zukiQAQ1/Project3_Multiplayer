using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IndicationText : MonoBehaviour
{
    [SerializeField] private float indicationTextHoldTime = 2f;
    [SerializeField] private float indicationTextFadeTime = 0.5f;
    [SerializeField] private int maximumTextAllowedOnScreen = 3;
    [SerializeField] private TMP_Text indicationTextPrefab;

    private Queue<string> textsToDisplay = new();
    private int totCurrentText = 0;
    
    public void DisplayHintTextOnUI(string textToDisplay)
    {
        textsToDisplay.Enqueue(textToDisplay);
        Debug.Log($"IndicationText: text {textToDisplay} added to the queue");
    }

    private IEnumerator IEFadeAwayAndDestroyText(TMP_Text textObj)
    {
        float timer = 0;
        yield return new WaitForSecondsRealtime(indicationTextHoldTime);
        while(timer < indicationTextFadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / indicationTextFadeTime);
            textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, alpha);
            yield return null;
        }
        textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 0);
        Destroy(textObj.gameObject);
        totCurrentText--;
    }

    // Update is called once per frame
    void Update()
    {
        if(totCurrentText < maximumTextAllowedOnScreen && textsToDisplay.Count > 0)
        {
            var newText = Instantiate(indicationTextPrefab, transform).GetComponent<TMP_Text>();
            newText.text = textsToDisplay.Dequeue();
            totCurrentText++;
            StartCoroutine(IEFadeAwayAndDestroyText(newText));
        }
    }
}
