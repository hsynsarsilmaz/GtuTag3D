using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinToGameButton : MonoBehaviour
{

    public TMPro.TMP_InputField inputField;
    public TMPro.TMP_InputField inputField2;
    public TMPro.TextMeshProUGUI text;
    public TMPro.TextMeshProUGUI text2;
    private Color defaultColor = new Color(0.8823f,0.6941f,0.1725f);
    private Color disabledColor = new Color(0.4431f,0.5019f,0.5764f,0.2196f);
    private Color defaultColor2 = new Color(0.2666f, 0.7411f, 0.1960f);
    // Update is called once per frame
    void Update()
    {
        if (inputField2 == null)
        {
            if (inputField.text != "")
            {
                text.color = defaultColor;
            }
            else
            {
                text.color = disabledColor;
            }
        }
        else
        {
            if (inputField.text != "" && inputField2.text != "")
            {
                text.color = defaultColor;
                text2.color = defaultColor2;
            }
            else
            {
                text.color = disabledColor;
                text2.color = disabledColor;
            }
        }

    }
}
