namespace FormsApp_OidcClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BtLogin = new Button();
            TbResponse = new TextBox();
            getCccfToken = new Button();
            TbApiResponse = new TextBox();
            SuspendLayout();
            // 
            // BtLogin
            // 
            BtLogin.Location = new Point(4, 6);
            BtLogin.Name = "BtLogin";
            BtLogin.Size = new Size(185, 28);
            BtLogin.TabIndex = 0;
            BtLogin.Text = "Resource Owner Password Flow";
            BtLogin.UseVisualStyleBackColor = true;
            BtLogin.Click += BtLogin_Click;
            // 
            // TbResponse
            // 
            TbResponse.Location = new Point(4, 39);
            TbResponse.Multiline = true;
            TbResponse.Name = "TbResponse";
            TbResponse.Size = new Size(792, 180);
            TbResponse.TabIndex = 1;
            TbResponse.Text = "Login response will be shown here";
            // 
            // getCccfToken
            // 
            getCccfToken.Location = new Point(231, 6);
            getCccfToken.Name = "getCccfToken";
            getCccfToken.Size = new Size(176, 27);
            getCccfToken.TabIndex = 2;
            getCccfToken.Text = "Client Credentials Flow";
            getCccfToken.UseVisualStyleBackColor = true;
            getCccfToken.Click += getCccfToken_Click;
            // 
            // TbApiResponse
            // 
            TbApiResponse.Location = new Point(4, 225);
            TbApiResponse.Multiline = true;
            TbApiResponse.Name = "TbApiResponse";
            TbApiResponse.Size = new Size(792, 226);
            TbApiResponse.TabIndex = 3;
            TbApiResponse.Text = "API response will be shown here";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(TbApiResponse);
            Controls.Add(getCccfToken);
            Controls.Add(TbResponse);
            Controls.Add(BtLogin);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtLogin;
        private TextBox TbResponse;
        private Button getCccfToken;
        private TextBox TbApiResponse;
    }
}