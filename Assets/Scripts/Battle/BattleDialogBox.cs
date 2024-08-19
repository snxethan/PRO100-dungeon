using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] int lettersPerSecond; //stores the letters per second
    [SerializeField] Color highlightedColor = Color.red; //stores the highlighted color
    [SerializeField] Text dialogText; //stores the dialog text
    [SerializeField] GameObject actionSelector; //stores the action selector
    [SerializeField] GameObject itemSelector; //stores the move selector
    [SerializeField] GameObject itemDetails; //stores the move details

    [SerializeField] List<Text> actionTexts; //stores the action texts
    [SerializeField] List<Text> itemTexts; //stores the move texts

    [SerializeField] private Text itemDetailsText; //stores the move details text
    [SerializeField] private Text itemTypeText; //stores the move type text


    public IEnumerator TypeDialog(string dialog)
    {
        Debug.Log($"Full dialog to display: {dialog}");

        dialogText.text = ""; // Clear the dialog text
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(1f / lettersPerSecond); // Control typing speed
        }
        Debug.Log($"Current text: {dialogText.text}");

        Debug.Log("TypeDialog Coroutine Completed");
        yield return new WaitForSeconds(2f); // Extended wait to ensure text is visible
    }

    public void EnableDialogText(bool enabled) //enables the dialog text
    {
        dialogText.enabled = enabled; //sets the dialog text to enabled
    }

    public void EnableActionSelector(bool enabled) //enables the dialog text
    {
        actionSelector.SetActive(enabled); //sets the dialog text to enabled
    }

    public void EnableItemSelector(bool enabled) //enables the dialog text
    {
        itemSelector.SetActive(enabled); //sets the dialog text to enabled
        itemDetails.SetActive(enabled); //sets the dialog text to disabled
    }

    public void UpdateActionSelection(int selectedAction) //updates the action selection
    {
        for (int i = 0; i < actionTexts.Count; i++) //iterates through each action text
        {
            if (i == selectedAction) //checks if the action is selected
            {
                actionTexts[i].color = highlightedColor; //sets the color to red
            }
            else
            {
                actionTexts[i].color = Color.black; //sets the color to black
            }
        }
        
    }

    public void setItemNames(List<ItemBase> items) //sets the item names
    {
        for (int i = 0; i < itemTexts.Count; i++) //iterates through each item text
        {
            if (i < items.Count) //checks if the item is in the list
            {
                itemTexts[i].text = items[i].Name; //sets the item name
            }
            else
            {
                itemTexts[i].text = "-"; //clears the item name
            }
        }
    }
}
