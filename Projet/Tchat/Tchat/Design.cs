/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : Desing - Contains all method which manage the interface
 * Author : GENGA Dario
 * Last update : 2017.12.17 (yyyy-MM-dd)
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Tchat
{
    /// <summary>
    /// Contains all method which manage the interface
    /// </summary>
    class Design
    {
        /// <summary>
        /// Round the edges of a control
        /// </summary>
        /// <param name="control">The control that we want to round the edges</param>
        /// <param name="x">X coordinate of the upper left corner of the bounding rectangle that defines the ellipse</param>
        /// <param name="y">Y coordinate of the upper left corner of the bounding rectangle that defines the ellipse</param>
        /// <param name="width">The width of the bounding rectangle that defines the ellipse</param>
        /// <param name="height">The height of the bounding rectangle that defines the ellipse</param>
        public static void RoundControl(Control control, int x, int y, int width, int height)
        {
            // Create an ellipse that will be the now border of the control
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(x, y, width, height);
            // Redefines the button area with the previously created ellipse
            Region rg = new Region(gp);
            control.Region = rg;
        }

        /// <summary>
        /// Adds and returns a button that will edit the control to which it is linked. If the button already exists then it is displayed and re-edited without recreating it
        /// </summary>
        /// <param name="control">The Control that will be linked to the button</param>
        /// <param name="gbx">The GroupBox that will be linked to the button</param>
        /// <param name="name">The name property of the button</param>
        /// <param name="familyName">Font for the text of the button</param>
        /// <param name="text">The text to display in the button (corresponding to the font)</param>
        /// <param name="eventHandler">The event called by the button when clicked</param>
        /// <param name="tag">The tag of the button (empty string by default)</param>
        /// <returns>Return the (new) button</returns>
        public static Button AddEditButtonForControl(Control control, GroupBox gbx, string name, string familyName, string text, EventHandler eventHandler, string tag = "")
        {
            // Check if the button has not already been created
            // If not we create it
            if (gbx.Controls.Find(name, false).Length == 0)
            {
                // Initializing button settings
                #region InitBtn
                int btnWidth = 20;
                int btnHeight = 20;
                // Calculates the position X and Y of the button so that it is placed to the right of the control
                int btnLocationX = control.Location.X + control.Width;
                int btnLocationY = control.Location.Y;
                #endregion InitBtn
                
                // Creation of the button
                Button btn = new Button
                {
                    Name = name,
                    Font = new Font(familyName, 9),
                    Text = text,
                    Location = new Point(btnLocationX, btnLocationY),
                    Width = btnWidth,
                    Height = btnHeight,
                    Tag = tag // Will let us know that the button is only for editing
                };

                btn.Click += eventHandler; // Delegate the event click to the EventHandler received as parameter

                // Management of the button display
                gbx.Controls.Add(btn);
                btn.BringToFront(); // Put the button in the foreground (to do once the button added to the groupbox)

                return btn;
            }
            else
            {
                gbx.Controls.Find(name, false)[0].Visible = true; // Else we show the control

                return (Button)gbx.Controls.Find(name, false)[0];
            }   
        }

        /// <summary>
        /// Draw a dashed border for control
        /// </summary>
        /// <param name="control">The control that wants a dashed border</param>
        /// <param name="e">The data of the control event</param>
        public static void DrawDashedBorder(Control control, PaintEventArgs e)
        {
            // REMARK :  Is this method (DrawDashedBorder) really necessary ?
            ControlPaint.DrawBorder(e.Graphics, control.ClientRectangle, Color.Black, ButtonBorderStyle.Dashed);
        }


    }
}
