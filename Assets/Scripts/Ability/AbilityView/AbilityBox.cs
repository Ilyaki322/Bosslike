using UnityEngine;
using UnityEngine.UIElements;

public class AbilityBox : VisualElement
{
    //public Image Icon;
    //public Sprite AbilitySprite;

    public AbilityBox(Sprite abilitySprite, Sprite frameSprite)
    {
        var icon = this.CreateChild<Image>("tileIcon");
        icon.image = abilitySprite.texture;

        var frame = this.CreateChild("tileFrame");
        frame.style.backgroundImage = frameSprite.texture;
    }
}
