/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmRegister - This form is used to create a new account
 * Class : Placeholder - Create a placeholder for a TextBox in C# Windows Form
 * Author : GENGA Dario
 * Last update : 2017.12.17 (yyyy-MM-dd)
 */
using System;
using System.Windows.Forms;

/// <summary>
/// Create a placeholder for a TextBox in C# Windows Form
/// </summary>
public class Placeholder
{
    private TextBox _tbx;
    private string _textPlaceholder;
    private bool _placerholderIsDisabled;

    /// <summary>
    /// Instantiate the placeholder for the textbox
    /// </summary>
    /// <param name="tbx">The textbox who gets the placeholder</param>
    /// <param name="textPlaceholder">The text of the placeholder</param>
    public Placeholder(TextBox tbx, string textPlaceholder)
    {
        PlacerholderIsDisabled = false;

        Tbx = tbx;
        TextPlaceholder = textPlaceholder;

        // Create placeholder events
        tbx.TextChanged += Tbx_TextChanged;
        tbx.GotFocus += Tbx_GotFocus;
        tbx.LostFocus += Tbx_LostFocus;
    }

    /// <summary>
    /// The textbox who gets the placeholder
    /// </summary>
    public TextBox Tbx
    {
        get => _tbx;
        set => _tbx = value;
    }

    /// <summary>
    /// The text of the placeholder
    /// </summary>
    public string TextPlaceholder
    {
        get => _textPlaceholder;
        set => _textPlaceholder = value;
    }
    
    /// <summary>
    /// State of the placeholder (disabled = true, enabled = false)
    /// </summary>
    private bool PlacerholderIsDisabled { get => _placerholderIsDisabled; set => _placerholderIsDisabled = value; }


    /// <summary>
    /// When the text of the textbox change and if the textbox is focused then the placeholder is disabled
    /// </summary>
    private void Tbx_TextChanged(object sender, EventArgs e)
    {
        // We must specify that the textbox has to be focused
        // so the system cannot disabled the placeholder at our place
        if (Tbx.Focused)
        {
            PlacerholderIsDisabled = true;
        }
    }

    /// <summary>
    /// When the textbox got the focus and if the placeholder isn't disabled then the text of the textbox is cleared
    /// </summary>
    private void Tbx_GotFocus(object sender, EventArgs e)
    {
        if (!PlacerholderIsDisabled)
        {
            Tbx.Text = "";
        }
    }

    /// <summary>
    /// When the textbox lost the focus and if his text is not null then the placeholder is enabled
    /// </summary>
    private void Tbx_LostFocus(object sender, EventArgs e)
    {
        if (String.IsNullOrWhiteSpace(Tbx.Text))
        {
            Tbx.Text = TextPlaceholder;
            PlacerholderIsDisabled = false;
        }
    }

}

