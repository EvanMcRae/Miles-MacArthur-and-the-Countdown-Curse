using TMPro;
using UnityEngine;

// FORMAT:
// @ = item name
// % = item use ability
// & = item use glyph
// * = item drop glyph
[RequireComponent(typeof(TextMeshProUGUI))]
public class ItemGlyphRenderer : GlyphRenderer
{
    protected override void Render()
    {
        if (!text.enabled || Player.instance.heldItem == null) return;

        string theString = glyphString;
        theString = theString.Replace("@", Player.instance.heldItem.name);
        theString = theString.Replace("%", Player.instance.heldItem.GetUseText());
        theString = theString.Replace("&", $"<sprite=\"{(zxc ? "glyph_x" : "glyph_e")}\" index=0 tint=1>");
        theString = theString.Replace("*", $"<sprite=\"{(zxc ? "glyph_z" : "glyph_q")}\" index=0 tint=1>");
        text.text = theString;
    }

    // Lazy yes but screw it dude
    public void Update()
    {
        if (Player.instance.heldItem == null && text.enabled)
        {
            Deactivate();
            return;
        }
        else if (Player.instance.heldItem != null && !text.enabled)
        {
            Activate();
            Render();
        }
    }
}
