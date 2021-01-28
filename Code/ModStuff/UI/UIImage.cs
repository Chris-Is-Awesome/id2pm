using UnityEngine;

namespace ModStuff.UI
{
    public class UIImage : UIElement
    {
        public void Initialize()
        {
            //Set TextMesh
            nameTextMesh = gameObject.GetComponentInChildren<TextMesh>();
        }
    }
}
