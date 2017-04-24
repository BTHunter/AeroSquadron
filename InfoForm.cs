using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace AeroSquadron
{
	/// <summary>
	/// Summary description for InfoForm.
	/// </summary>
	public class InfoForm : System.Windows.Forms.Form
	{
        private System.Windows.Forms.PictureBox LogoBox;
        private System.Windows.Forms.Label ProductLabel;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label CopyrightLabel;
        private System.Windows.Forms.Label ContactLabel;
        private System.Windows.Forms.Label EmailLabel;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.LinkLabel WebsiteLabel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private ResourceManager oResourceManager;

        public ResourceManager LocalisationResourceManager
        {
            set
            {
                oResourceManager = value;
            }
        }

		public InfoForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InfoForm));
            this.LogoBox = new System.Windows.Forms.PictureBox();
            this.ProductLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.CopyrightLabel = new System.Windows.Forms.Label();
            this.ContactLabel = new System.Windows.Forms.Label();
            this.EmailLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.WebsiteLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // LogoBox
            // 
            this.LogoBox.Image = ((System.Drawing.Bitmap)(resources.GetObject("LogoBox.Image")));
            this.LogoBox.Location = new System.Drawing.Point(8, 8);
            this.LogoBox.Name = "LogoBox";
            this.LogoBox.Size = new System.Drawing.Size(208, 56);
            this.LogoBox.TabIndex = 0;
            this.LogoBox.TabStop = false;
            // 
            // ProductLabel
            // 
            this.ProductLabel.AutoSize = true;
            this.ProductLabel.Location = new System.Drawing.Point(8, 64);
            this.ProductLabel.Name = "ProductLabel";
            this.ProductLabel.Size = new System.Drawing.Size(77, 13);
            this.ProductLabel.TabIndex = 1;
            this.ProductLabel.Text = "AeroSquadron";
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(8, 80);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(43, 13);
            this.VersionLabel.TabIndex = 2;
            this.VersionLabel.Text = "Version";
            // 
            // CopyrightLabel
            // 
            this.CopyrightLabel.AutoSize = true;
            this.CopyrightLabel.Location = new System.Drawing.Point(8, 96);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(184, 13);
            this.CopyrightLabel.TabIndex = 3;
            this.CopyrightLabel.Text = "Copyright (c) 2005 by Dennis Müller";
            // 
            // ContactLabel
            // 
            this.ContactLabel.AutoSize = true;
            this.ContactLabel.Location = new System.Drawing.Point(8, 136);
            this.ContactLabel.Name = "ContactLabel";
            this.ContactLabel.Size = new System.Drawing.Size(43, 13);
            this.ContactLabel.TabIndex = 5;
            this.ContactLabel.Text = "Contact";
            // 
            // EmailLabel
            // 
            this.EmailLabel.AutoSize = true;
            this.EmailLabel.Location = new System.Drawing.Point(16, 152);
            this.EmailLabel.Name = "EmailLabel";
            this.EmailLabel.Size = new System.Drawing.Size(60, 13);
            this.EmailLabel.TabIndex = 6;
            this.EmailLabel.Text = "EmailLabel";
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(136, 184);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(80, 24);
            this.CloseButton.TabIndex = 8;
            this.CloseButton.Text = "Close";
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // WebsiteLabel
            // 
            this.WebsiteLabel.AutoSize = true;
            this.WebsiteLabel.Location = new System.Drawing.Point(16, 168);
            this.WebsiteLabel.Name = "WebsiteLabel";
            this.WebsiteLabel.Size = new System.Drawing.Size(45, 13);
            this.WebsiteLabel.TabIndex = 9;
            this.WebsiteLabel.TabStop = true;
            this.WebsiteLabel.Text = "Website";
            this.WebsiteLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebsiteLabel_LinkClicked);
            // 
            // InfoForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(224, 214);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.WebsiteLabel,
                                                                          this.CloseButton,
                                                                          this.EmailLabel,
                                                                          this.ContactLabel,
                                                                          this.CopyrightLabel,
                                                                          this.VersionLabel,
                                                                          this.ProductLabel,
                                                                          this.LogoBox});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InfoForm";
            this.Load += new System.EventHandler(this.InfoForm_Load);
            this.ResumeLayout(false);

        }
		#endregion

        private void InfoForm_Load(object sender, System.EventArgs e)
        {
            this.ProductLabel.Text = oResourceManager.GetString("InfoProduct");
            this.VersionLabel.Text = oResourceManager.GetString("InfoVersionText") + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.CopyrightLabel.Text = oResourceManager.GetString("InfoCopyright");
            this.ContactLabel.Text = oResourceManager.GetString("InfoContact");
            this.EmailLabel.Text = oResourceManager.GetString("InfoEmail");
            this.WebsiteLabel.Text = oResourceManager.GetString("InfoWebsiteText");
            this.WebsiteLabel.Links.Add(0,this.WebsiteLabel.Text.Length,oResourceManager.GetString("InfoWebsiteLink"));
            this.CloseButton.Text = oResourceManager.GetString("ButtonClose");
            this.Text = oResourceManager.GetString("InfoProduct");
        }

        private void CloseButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void WebsiteLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            WebsiteLabel.Links[WebsiteLabel.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

	}
}
