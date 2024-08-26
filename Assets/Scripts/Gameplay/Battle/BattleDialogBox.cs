using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    #region fields
    [Header("UI Components")]
    [SerializeField] int lettersPerSecond; // Letters per second for the dialog box
    [SerializeField] Color highlightedColor = Color.red; // Highlighted color for the dialog box
    [SerializeField] Text dialogText; // Dialog text for the dialog box
    [SerializeField] GameObject actionSelector; // Action selector for the dialog box
    [SerializeField] GameObject itemSelector; // Item selector for the dialog box
    [SerializeField] GameObject itemDetails; // Item details for the dialog box

    [SerializeField] List<Text> actionTexts; // List of action texts for the dialog box
    [SerializeField] List<Text> itemTexts; // List of item texts for the dialog box

    [SerializeField] private Text itemDetailsText; // Item details text for the dialog box
    [SerializeField] private Text itemTypeText; // Item type text for the dialog box
    #endregion

    #region update text 
    private Coroutine currentDialogCoroutine; // Current dialog coroutine for the dialog box

    public IEnumerator TypeDialog(string dialog) // Coroutine to type the dialog
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true); // Ensure the dialog box is active
        }

        if (currentDialogCoroutine != null) // If there is a current dialog coroutine
        {
            StopCoroutine(currentDialogCoroutine); // Stop the current dialog coroutine 
        }

        dialogText.text = "";
        currentDialogCoroutine = StartCoroutine(TypeDialogCoroutine(dialog)); // Start the dialog coroutine

        yield return currentDialogCoroutine;
    }


    private IEnumerator TypeDialogCoroutine(string dialog)
    {
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(2f);
        currentDialogCoroutine = null;
    }
    
    public void SetItemNames(List<ItemBase> items)
    {
        for (int i = 0; i < itemTexts.Count; i++)
        {
            if (i < items.Count && items[i] != null && (items[i].Uses > 0 || items[i].UnlimetedUse))
            {
                itemTexts[i].text = items[i].Name;
            }
            else
            {
                itemTexts[i].text = "-";
            }
        }
    }
    public void ChangeActionText(string action1, string action2)
    {
        actionTexts[0].text = action1;
        actionTexts[1].text = action2;
    }
    #endregion
    
    #region enable/disable dialog components
    public void ToggleDialogText(bool enabled) 
    {
        dialogText.enabled = enabled;
    }

    public void ToggleActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void ToggleItemSelector(bool enabled)
    {
        itemSelector.SetActive(enabled);
        itemDetails.SetActive(enabled);
        dialogText.enabled = !enabled; // Hide dialog text when item selector is shown
    }
    #endregion
    
    #region update dialog components
    public void UpdateActionSelection(int selectedAction)
    {
        //update the action (FIGHT / RUN)
        for (int i = 0; i < actionTexts.Count; i++)
        {
            // Set the color of the action text based on the selection
            actionTexts[i].color = (i == selectedAction) ? highlightedColor : Color.black; 
        }
    }
    public void UpdateItemSelection(int selectedItem, ItemBase item, int playerLevel)
    {
        for (int i = 0; i < itemTexts.Count; i++)
        {
            itemTexts[i].color = (i == selectedItem) ? highlightedColor : Color.black;
        }

        if (item != null && (item.Uses > 0 || item.UnlimetedUse))
        {
            itemTypeText.text = item.GetItemTypeStr(playerLevel);
            itemDetailsText.text = item.GetItemDetails();//playerLevel was passed as a parameter for some reason
        }
        else
        {
            itemTypeText.text = "No Item";
            itemDetailsText.text = "No details available";
        }
    }
    #endregion

}
