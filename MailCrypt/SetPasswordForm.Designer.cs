namespace MailCrypt
{
    partial class SetPasswordForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.noteLabel = new System.Windows.Forms.Label();
            this.errorLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.acceptButton = new System.Windows.Forms.Button();
            this.picBox_5 = new System.Windows.Forms.PictureBox();
            this.picBox_4 = new System.Windows.Forms.PictureBox();
            this.picBox_3 = new System.Windows.Forms.PictureBox();
            this.picBox_2 = new System.Windows.Forms.PictureBox();
            this.picBox_1 = new System.Windows.Forms.PictureBox();
            this.selectedPicPanel = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedPicPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // noteLabel
            // 
            this.noteLabel.Font = new System.Drawing.Font("Segoe UI Light", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noteLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.noteLabel.Location = new System.Drawing.Point(624, 12);
            this.noteLabel.Name = "noteLabel";
            this.noteLabel.Size = new System.Drawing.Size(234, 118);
            this.noteLabel.TabIndex = 6;
            this.noteLabel.Text = "Click a picture, then type a number and drag it to a desired location on the pict" +
    "ure.";
            this.noteLabel.UseCompatibleTextRendering = true;
            // 
            // errorLabel
            // 
            this.errorLabel.Font = new System.Drawing.Font("Segoe WP Light", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorLabel.ForeColor = System.Drawing.Color.Maroon;
            this.errorLabel.Location = new System.Drawing.Point(624, 341);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(234, 96);
            this.errorLabel.TabIndex = 12;
            this.errorLabel.UseCompatibleTextRendering = true;
            // 
            // cancelButton
            // 
            this.cancelButton.AutoSize = true;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Image = global::MailCrypt.Properties.Resources.cross;
            this.cancelButton.Location = new System.Drawing.Point(624, 440);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 72);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // acceptButton
            // 
            this.acceptButton.AutoSize = true;
            this.acceptButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.acceptButton.Font = new System.Drawing.Font("Segoe UI Light", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acceptButton.Image = global::MailCrypt.Properties.Resources.tick;
            this.acceptButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.acceptButton.Location = new System.Drawing.Point(705, 440);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(153, 72);
            this.acceptButton.TabIndex = 10;
            this.acceptButton.Text = "Accept";
            this.acceptButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.acceptButton.UseCompatibleTextRendering = true;
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // picBox_5
            // 
            this.picBox_5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox_5.Image = global::MailCrypt.Properties.Resources.PIC_5;
            this.picBox_5.Location = new System.Drawing.Point(518, 412);
            this.picBox_5.Name = "picBox_5";
            this.picBox_5.Size = new System.Drawing.Size(100, 100);
            this.picBox_5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBox_5.TabIndex = 5;
            this.picBox_5.TabStop = false;
            this.picBox_5.Click += new System.EventHandler(this.picBox_5_Click);
            // 
            // picBox_4
            // 
            this.picBox_4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox_4.Image = global::MailCrypt.Properties.Resources.PIC_4;
            this.picBox_4.Location = new System.Drawing.Point(518, 312);
            this.picBox_4.Name = "picBox_4";
            this.picBox_4.Size = new System.Drawing.Size(100, 100);
            this.picBox_4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBox_4.TabIndex = 4;
            this.picBox_4.TabStop = false;
            this.picBox_4.Click += new System.EventHandler(this.picBox_4_Click);
            // 
            // picBox_3
            // 
            this.picBox_3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox_3.Image = global::MailCrypt.Properties.Resources.PIC_3;
            this.picBox_3.Location = new System.Drawing.Point(518, 212);
            this.picBox_3.Name = "picBox_3";
            this.picBox_3.Size = new System.Drawing.Size(100, 100);
            this.picBox_3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBox_3.TabIndex = 3;
            this.picBox_3.TabStop = false;
            this.picBox_3.Click += new System.EventHandler(this.picBox_3_Click);
            // 
            // picBox_2
            // 
            this.picBox_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox_2.Image = global::MailCrypt.Properties.Resources.PIC_2;
            this.picBox_2.Location = new System.Drawing.Point(518, 112);
            this.picBox_2.Name = "picBox_2";
            this.picBox_2.Size = new System.Drawing.Size(100, 100);
            this.picBox_2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBox_2.TabIndex = 2;
            this.picBox_2.TabStop = false;
            this.picBox_2.Click += new System.EventHandler(this.picBox_2_Click);
            // 
            // picBox_1
            // 
            this.picBox_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox_1.Image = global::MailCrypt.Properties.Resources.PIC_1;
            this.picBox_1.Location = new System.Drawing.Point(518, 12);
            this.picBox_1.Name = "picBox_1";
            this.picBox_1.Size = new System.Drawing.Size(100, 100);
            this.picBox_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBox_1.TabIndex = 1;
            this.picBox_1.TabStop = false;
            this.picBox_1.Click += new System.EventHandler(this.picBox_1_Click);
            // 
            // selectedPicPanel
            // 
            this.selectedPicPanel.Image = global::MailCrypt.Properties.Resources.PIC_5;
            this.selectedPicPanel.Location = new System.Drawing.Point(12, 12);
            this.selectedPicPanel.Name = "selectedPicPanel";
            this.selectedPicPanel.Size = new System.Drawing.Size(500, 500);
            this.selectedPicPanel.TabIndex = 0;
            this.selectedPicPanel.TabStop = false;
            this.selectedPicPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.selectedPicPanel_Paint);
            this.selectedPicPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.selectedPicPanel_MouseDown);
            this.selectedPicPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.selectedPicPanel_MouseMove);
            this.selectedPicPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.selectedPicPanel_MouseUp);
            // 
            // SetPasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(868, 526);
            this.ControlBox = false;
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.acceptButton);
            this.Controls.Add(this.noteLabel);
            this.Controls.Add(this.picBox_5);
            this.Controls.Add(this.picBox_4);
            this.Controls.Add(this.picBox_3);
            this.Controls.Add(this.picBox_2);
            this.Controls.Add(this.picBox_1);
            this.Controls.Add(this.selectedPicPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "SetPasswordForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Picture Password";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SetPasswordForm_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.picBox_5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.selectedPicPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox selectedPicPanel;
        private System.Windows.Forms.PictureBox picBox_1;
        private System.Windows.Forms.PictureBox picBox_2;
        private System.Windows.Forms.PictureBox picBox_3;
        private System.Windows.Forms.PictureBox picBox_4;
        private System.Windows.Forms.PictureBox picBox_5;
        private System.Windows.Forms.Label noteLabel;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label errorLabel;


    }
}