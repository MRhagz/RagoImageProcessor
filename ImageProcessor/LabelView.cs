using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessor
{
    internal class LabelView : UserControl
    {
        GroupBox group;
        Label title;
        Label subtitle;

        public LabelView(string stringTitle, string stringSubtitle)
        {
            group = new GroupBox
            {
                Text = "",
                AutoSize = true,
                Width = 250,
                Padding = new Padding(10),
            };

            title = new Label
            {
                Text = stringTitle,
                AutoSize = false,
                Width = 250,
                Height = 30,
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            subtitle = new Label
            {
                Text = stringSubtitle,
                AutoSize = false,
                Width = 250,
                Height = 20,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            title.Location = new System.Drawing.Point(0, 10);
            subtitle.Location = new System.Drawing.Point(0, title.Bottom + 5);

            group.Controls.Add(title);
            group.Controls.Add(subtitle);

            this.Controls.Add(group);
            this.Width = group.Width;
            this.Height = group.Height;
        }
    }
}
