using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Created using this tutorial: https://youtu.be/P8hx343kIGg?si=-5OC9V8HhHH2GBeJ

/// <summary>
/// Class <c>ButtonNavDropdown</c> allows for a dropdown menu to be navigated using a controller or keyboard.
/// This component should be added to the Item object inside of Dropdown->Template->Viewport->Content
/// </summary>
public class ButtonNavDropdown : MonoBehaviour, ISelectHandler
{
    private ScrollRect scrollRect;
    private float scrollPosition = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>(true);

        int itemCount = scrollRect.content.transform.childCount - 1; //subtract 1 since Unity creates an extra Item that is not one of the options
        int index = transform.GetSiblingIndex(); //the index of this transform, relative to its siblings

        index = index < ((float)itemCount / 2f) ? index - 1 : index; //to improve appearance

        scrollPosition = 1 - ((float)index / itemCount);
    }

    //Any time the Item is selected, this will be executed.
    public void OnSelect(BaseEventData eventData)
    {
        if (scrollRect)
        {
            scrollRect.verticalScrollbar.value = scrollPosition;
        }   
    }
}
