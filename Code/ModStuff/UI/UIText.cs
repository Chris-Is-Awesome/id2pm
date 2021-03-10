using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModStuff.UI
{
    public class UIText : Singleton<UIText>
    {
        //References
        DebugMenu debugMenu;
        UIScrollBar scrollBar;

        //Debugmenu console size
        const int LINECOUNT = 24;
        const float LINELENGTH = 1600f;
        const int MAXMEMORY = 400;
        const string OUTPUT_TEXT_SEPARATOR = "----------------------------------------------------------------------------------------------------------------------------------";

        //Stored strings
        List<string> lineList;
        List<string> LineList
        {
            get
            {
                if (lineList == null) lineList = new List<string>();
                return lineList;
            }
        }

        //Awake function
        public void Awake()
        {
            OutputText("DebugMenu Initialized", true, Color.blue, false);
        }

        public static void QuickText(string text)
        {
            Instance.OutputText(text, true, Color.blue, false);
        }

        //Write on the console
        public void OutputText(string text, bool wrap, Color color, bool useSeparator)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (useSeparator) LineList.Add(OUTPUT_TEXT_SEPARATOR);
            if (wrap) text = WrapText(text, LINELENGTH, false);

            string[] lines = text.Split(new char[] { '\n' });
            bool addColor = color != Color.black;
            for (int i = 0; i < lines.Length; i++)
            {
                string textFromLine = lines[i];
                if (string.IsNullOrEmpty(textFromLine)) textFromLine = " ";
                if (addColor) textFromLine = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + textFromLine + "</color>";
                LineList.Add(textFromLine);
            }

            if (LineList.Count > MAXMEMORY) LineList.RemoveRange(0, LineList.Count - MAXMEMORY);

            SetStep();
            UpdateConsole();
        }

        //Scrolling position variables
        int scrollPos;

        //Clear console
        public void ClearConsole()
        {
            lineList = null;
            SetStep();
            UpdateConsole();
        }

        //Assemble console text
        public string GetText()
        {
            string text = "";
            for (int i = 0; i < LINECOUNT; i++)
            {
                if (i > 0) { text = "\n" + text + "\n"; }
                int currentPos = LineList.Count - LINECOUNT + i - scrollPos;
                text += (currentPos < 0) ? " " : LineList[currentPos];
            }
            return text;
        }

        //Update console
        public void UpdateConsole()
        {
            //Find the debugmenu. If it doesnt exist, exit
            if (debugMenu == null)
            {
                GameObject debug = GameObject.Find("Debug");
                if (debug == null) { return; }
                debugMenu = debug.GetComponent<DebugMenu>();
            }

            //Send text to the console
            debugMenu.OutputText(GetText());
        }

        //Configure scrollbar step and division size
        void SetStep()
        {
            scrollPos = 0;
            if (scrollBar == null) return;

            float extraRows = LineList.Count - LINECOUNT;
            if (extraRows <= 0f) //extraRows = 1f;
            {
                scrollBar.gameObject.SetActive(false);
            }
            else
            {
                scrollBar.gameObject.SetActive(true);
                scrollBar.SliderStep = 1f / extraRows;
                if (scrollBar.Value != 1f) scrollBar.Value = 1f;
            }
        }

        //Link debugmenu scrollbar to the singleton
        public void SetupScrollBar(UIScrollBar scroll)
        {
            scrollBar = scroll;
            scrollBar.onInteraction += UpdatePos;
            SetStep();
        }

        //Scrollbar function
        void UpdatePos(float pos)
        {
            int extraLines = LineList.Count - LINECOUNT;
            if (extraLines < 0) extraLines = 0;

            float scrollValue = Mathf.Clamp(pos, 0f, 1f);
            scrollPos = (int)Math.Round((1f - scrollValue) * extraLines, 0);
            UpdateConsole();
        }

        //Used to fix missing characterinfo for ludo font
        TextMesh characterFixer;
        void FixCharInfoIssue()
        {
            if (characterFixer != null) { return; }
            GameObject instantiatedButton = null;
            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {

                if (go.name == "Default" && go.transform.parent != null && go.transform.parent.name == "KeyConfig")
                {
                    instantiatedButton = GameObject.Instantiate(go);
                    characterFixer = instantiatedButton.GetComponentInChildren<TextMesh>();
                    break;
                }
            }
            if (characterFixer == null) { return; } //I hope this doesnt happen
            string magicText = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZ0123456789<>_-(){}[]+*-\\/#@|º.,´`\"' \n\t";
            characterFixer.text = magicText;
            _font = characterFixer.font;
            instantiatedButton.SetActive(false);
            _font.RequestCharactersInTexture(magicText);
        }

        //Returns a text with line jumps and vertical fix (static function)
        public static string WrapText(string text, float lineSize, bool vertFix)
        {
            return UIText.Instance.GetTextWithJumps(text, lineSize, vertFix);
        }

        //Returns a text with line jumps and vertical fix (instance function)
        string GetTextWithJumps(string text, float lineSize, bool vertFix)
        {
            if (string.IsNullOrEmpty(text)) { return null; }

            FixCharInfoIssue();
            string newText = AddLineJumps(text, lineSize);
            if (vertFix) newText = FixVerticalShift(newText);

            return newText;
        }

        //Return the maximum line size in a paragraph
        public float GetMaxLineSize(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0f;
            FixCharInfoIssue();
            float output = 0f;
            float px = 0f;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    if (px > output) output = px;
                    px = 0f;
                }
                else
                {
                    _font.GetCharacterInfo(c, out CharacterInfo characterInfo);
                    px += (float)characterInfo.advance;
                }
            }
            if (px > output) output = px;
            return output;
        }

        //Evaluates if a point in text needs a line jump
        Font _font;
        bool DoWrapHere(string text, int index, float initialLength, float wrapSize)
        {
            float px = initialLength;
            char newLetter = text[index];
            if (newLetter == '\n') { return false; }
            if (newLetter == ' ')
            {
                _font.GetCharacterInfo(newLetter, out CharacterInfo characterInfoSpace);
                px += (float)characterInfoSpace.advance;
                for (int i = index + 1; i < text.Length; i++)
                {
                    char c = text[i];
                    if (c == ' ' || c == '\n') { return px > wrapSize; }
                    _font.GetCharacterInfo(c, out CharacterInfo characterInfo);

                    px += (float)characterInfo.advance;
                    if (px > wrapSize) { return true; }
                }
                return false;
            }
            _font.GetCharacterInfo(newLetter, out CharacterInfo characterInfo2);
            px += (float)characterInfo2.advance;

            return px > wrapSize;
        }

        //Add line jumpts to blocks of texts
        string AddLineJumps(string text, float lineSize)
        {
            float lineLength = 0f;
            //float charScale = 0.03f;
            string output = "";

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    output += '\n';
                    lineLength = 0f;
                }
                else
                {
                    bool addChar = true;
                    if (DoWrapHere(text, i, lineLength, lineSize))
                    {
                        output += '\n';
                        lineLength = 0f;
                        if (c == ' ') { addChar = false; }
                    }
                    if (addChar)
                    {
                        _font.GetCharacterInfo(c, out CharacterInfo characterInfo);
                        output += c;
                        lineLength += (float)characterInfo.advance;
                    }
                }
            }
            return output;

        }

        //Used for text that move up with more lines (debug menu and bigframes)
        public string FixVerticalShift(string text)
        {
            if (text.Contains("\n"))
            {
                string str = "";
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        str += "\n";
                    }
                }
                text = str + text;
            }
            return text;
        }

        // Returns cutscene font
        public Font GetLudoFont
        {
            get
            {
                if (_font == null) FixCharInfoIssue();
                return _font;
            }
        }

        // Creates a text object
        public GameObject CreateText(string objName, string text = "", Vector3 posOffset = default(Vector3), Transform parent = null, TextAlignment alignment = TextAlignment.Left, Color color = default(Color), int fontSize = 80, Vector3 rotation = default(Vector3), int layer = 9)
        {
            // Create GameObject
            GameObject textObj = new GameObject(objName);
            if (parent == null) parent = GameObject.Find("OverlayCamera").transform;
            textObj.transform.parent = parent;
            textObj.transform.localPosition = new Vector3(UIScreenLibrary.FirstCol + posOffset.x, posOffset.y, posOffset.z);
            textObj.transform.localEulerAngles = rotation;
            textObj.layer = layer;

            // Create TextMesh
            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.font = GetLudoFont;
            textMesh.GetComponent<UnityEngine.MeshRenderer>().sharedMaterial = FontMaterialMap.LookupFontMaterial(textMesh.font);
            textMesh.alignment = alignment;
            textMesh.fontSize = fontSize;
            textMesh.characterSize = 0.018f;
            if (color.a == 0) color = Color.white;
            textMesh.color = color;
            textMesh.text = text;

            return textObj;
        }

       // Automatically add line breaks to given text; ignores rich text tags
       public static string AddLineBreaks(string text, TextMesh mesh, float maxWidthOverride = 0f)
		{
            string output = text;
            float maxWidth = maxWidthOverride > 0 ? maxWidthOverride : 920f;
			float textWidth = 0;
            int endIndexOfTag = -1;
            bool isInTag = false;

            for (int i = 0; i < output.Length - 1; i++)
			{
                char c = output[i];

                // If line break, continue and reset width (new line)
                if (c == '\n')
                {
                    textWidth = 0;
                    continue;
                }

                // Check if rich text tag
                if (c == '<')
                {
                    isInTag = true;
                }
                else if (c == '>')
                {
                    if (endIndexOfTag >= 0)
                    {
                        endIndexOfTag = text.IndexOf(c, endIndexOfTag);
                    }
                    else
                    {
                        endIndexOfTag = text.IndexOf(c);
                    }

                    isInTag = false;
                }

                if (mesh != null && mesh.font.GetCharacterInfo(c, out CharacterInfo charInfo, mesh.fontSize, mesh.fontStyle))
				{
                    if (!isInTag)
                    {
                        textWidth += charInfo.advance;
                    }
                }

                // Add new line if at max width
                if (!isInTag && textWidth > maxWidth)
                {
                    output = output.Insert(i, "\n");
                    textWidth = 0;
                }
            }

            return output;
		}
    }
}