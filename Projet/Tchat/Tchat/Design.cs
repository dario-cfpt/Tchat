/* Project name : Tchat
 * Description : Chat online in Windows Form C#. Users can chat privately or they can chat in groups in "rooms"
 * Class : Desing - Contains all method which manage the interface
 * Author : GENGA Dario
 * Last update : 2017.11.09
 */

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Tchat
{
    class Design
    {
        /// <summary>
        /// Arrondi les bords d'un contrôle
        /// </summary>
        /// <param name="control">Le contrôle dont on veut arrondir les bords</param>
        /// <param name="x">Coordonnée x de l'angle supérieur gauche du rectangle englobant qui définit l'ellipse</param>
        /// <param name="y">Coordonnée y de l'angle supérieur gauche du rectangle englobant qui définit l'ellipse</param>
        /// <param name="width">La largeur du rectangle englobant qui définit l’ellipse</param>
        /// <param name="height">Hauteur du rectangle englobant qui définit l’ellipse</param>
        public static void RoundControl(Control control, int x, int y, int width, int height)
        {
            // Crée ellipse qui sera la nouvelle bordure du contrôle
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(x, y, width, height);
            // Redéfinit la zone du bouton avec l'ellipse précédemment créée
            Region rg = new Region(gp);
            control.Region = rg;
        }

        /// <summary>
        /// Ajoute et retourne un bouton qui permettra d'éditer le contrôle auquel il est lié. Si le bouton existe déjà alors on l'affiche et returne sans le recréer
        /// </summary>
        /// <param name="control">Le control qui sera lié au bouton</param>
        /// <param name="gbx">Le groupbox qui contient le control</param>
        /// <param name="name">La propriété name du bouton</param>
        /// <param name="familyName">La police à du text du bouton</param>
        /// <param name="text">Le text a afficher dans le bouton (correspondant à la police)</param>
        /// <param name="eventHandler">L'event appelé par le bouton lors du click</param>
        /// <param name="tag">Le tag du bouton (chaîne vide par défaut)</param>
        /// <returns>Retourne le (nouveau) bouton</returns>
        public static Button AddEditButtonForControl(Control control, GroupBox gbx, string name, string familyName, string text, EventHandler eventHandler, string tag = "")
        {
            // Vérifie si le bouton n'a pas déjà été créé.
            // Si il n'existe pas alors on le crée
            if (gbx.Controls.Find(name, false).Length == 0)
            {
                // Initialisation des paramètres du boutons
                #region InitBtn
                int btnWidth = 20;
                int btnHeight = 20;
                // Calcul la position X et Y du bouton pour qu'il se place à droite du control
                int btnLocationX = control.Location.X + control.Width;
                int btnLocationY = control.Location.Y;
                #endregion InitBtn

                // Création du bouton
                Button btn = new Button
                {
                    Name = name,
                    Font = new Font(familyName, 9),
                    Text = text,
                    Location = new Point(btnLocationX, btnLocationY),
                    Width = btnWidth,
                    Height = btnHeight,
                    Tag = tag // Permettra de savoir que le bouton ne sert que pour l'édition
                };

                btn.Click += eventHandler; // Délègue l'event click à l'EventHandler reçu en paramètre

                // Gestion de l'affichage du bouton
                gbx.Controls.Add(btn);
                btn.BringToFront(); // Met le bouton au premier plan (à faire une fois le bouton ajouter au groupbox)

                return btn;
            }
            else
            {
                gbx.Controls.Find(name, false)[0].Visible = true; // Sinon on affiche le contrôle

                return (Button)gbx.Controls.Find(name, false)[0];
            }   
        }

        /// <summary>
        /// Dessine une bordure en traitillé pour le contrôle
        /// </summary>
        /// <param name="control">Le contrôle qui veut une bordure en traitillé</param>
        /// <param name="e">Les données de l'événément du contrôle</param>
        public static void DrawDashedBorder(Control control, PaintEventArgs e)
        {
            // REMARK :  Cette methode est-elle vraiment nécessaire (DrawDashedBorder) ?
            ControlPaint.DrawBorder(e.Graphics, control.ClientRectangle, Color.Black, ButtonBorderStyle.Dashed);
        }


    }
}
