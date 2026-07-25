using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MovementGlyphRenderer : GlyphRenderer
{
    private bool leftPressed, rightPressed, upPressed, downPressed;

    protected override void Render()
    {
        if (!text.enabled) return;

        string theString = glyphString;
        theString = theString.Replace("W", $"<color={(upPressed ? "#808080" : "#FFFFFF")}><sprite=\"{(zxc ? "glyph_up" : "glyph_w")}\" index=0 tint=1>");
        theString = theString.Replace("A", $"<color={(leftPressed ? "#808080" : "#FFFFFF")}><sprite=\"{(zxc ? "glyph_left" : "glyph_a")}\" index=0 tint=1>");
        theString = theString.Replace("S", $"<color={(downPressed ? "#808080" : "#FFFFFF")}><sprite=\"{(zxc ? "glyph_down" : "glyph_s")}\" index=0 tint=1>");
        theString = theString.Replace("D", $"<color={(rightPressed ? "#808080" : "#FFFFFF")}><sprite=\"{(zxc ? "glyph_right" : "glyph_d")}\" index=0 tint=1>");
        text.text = theString;

        if (leftPressed && rightPressed && upPressed && downPressed)
        {
            text.enabled = false;
        }
    }

    public void SendInput(Vector2 movement)
    {
        if (movement.x < 0) leftPressed = true;
        if (movement.x > 0) rightPressed = true;
        if (movement.y < 0) downPressed = true;
        if (movement.y > 0) upPressed = true;
        if (movement.magnitude != 0) Render();
    }
}
