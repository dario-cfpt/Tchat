/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Form : FrmDisconnect - Custom MessageBox for the disconnect
 * Author : GENGA Dario
 * Last update : 2017.12.18 (yyyy-MM-dd)
 */
using System.Drawing;
using System.Windows.Forms;

namespace Tchat
{
    /// <summary>
    /// Custom MessageBox for the disconnect
    /// </summary>
    public partial class FrmDisconnect : Form
    {
        /// <summary>
        /// Create a MessageBox for the disconnect with an specifical icon
        /// </summary>
        /// <param name="icon"></param>
        public FrmDisconnect(MessageBoxIcon icon)
        {
            InitializeComponent();

            // Define the icone of the picturebox according to the MessageBoxIcon
            Bitmap bitmap = null;
            switch (icon)
            {
                case MessageBoxIcon.Hand: // Or .Stop
                    bitmap = SystemIcons.Hand.ToBitmap();
                    break;
                case MessageBoxIcon.Question: // Or .Error
                    bitmap = SystemIcons.Question.ToBitmap();
                    break;
                case MessageBoxIcon.Exclamation: // Or .Warning
                    bitmap = SystemIcons.Warning.ToBitmap();
                    break;
                case MessageBoxIcon.Asterisk: // Or .Information
                    break;
                default:
                    break;
            }
            pbxIcon.Image = bitmap;
        }
    }
}
