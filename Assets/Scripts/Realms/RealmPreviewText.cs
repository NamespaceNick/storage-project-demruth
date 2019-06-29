using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealmPreviewText : MonoBehaviour
{
    public string previewString = "Realm: ";
    Text previewText;

    void Start()
    {
        previewText = GetComponent<Text>();
    }

    void Update()
    {
        if (previewText.enabled)
        {
            previewText.text = previewString + RealmManager.instance.realmViewing;
        }
        
    }
}
