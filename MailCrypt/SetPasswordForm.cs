using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailCrypt
{
    public partial class SetPasswordForm : Form
    {

        String selectedImage = "PIC_5", selectedNum = "NoKey";
        int selectedX = 250, selectedY = 250;
        bool firstPaint = true, drag = false;
        int downX, downY, moveX, moveY;
        static byte[] keyString;

        public SetPasswordForm()
        {
            InitializeComponent();
        }

        private void SavePassword()
        {
            keyString = ASCIIEncoding.ASCII.GetBytes("z9!y8@x7");
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(keyString, keyString), CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cryptoStream);

            streamWriter.Write(selectedImage + ";" + selectedX + ";" + selectedY + ";" + selectedNum);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();
            streamWriter.Flush();
            File.WriteAllText(Environment.CurrentDirectory + "\\config.mfa", Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));           
        }

        private void picBox_1_Click(object sender, EventArgs e)
        {
            selectedPicPanel.Image = picBox_1.Image;
            selectedImage = "PIC_1";
        }

        private void picBox_2_Click(object sender, EventArgs e)
        {
            selectedPicPanel.Image = picBox_2.Image;
            selectedImage = "PIC_2";
        }

        private void picBox_3_Click(object sender, EventArgs e)
        {
            selectedPicPanel.Image = picBox_3.Image;
            selectedImage = "PIC_3";
        }

        private void picBox_4_Click(object sender, EventArgs e)
        {
            selectedPicPanel.Image = picBox_4.Image;
            selectedImage = "PIC_4";
        }

        private void picBox_5_Click(object sender, EventArgs e)
        {
            selectedPicPanel.Image = picBox_5.Image;
            selectedImage = "PIC_5";
        }

        private void SetPasswordForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar))
            {
                selectedNum = e.KeyChar.ToString();
                errorLabel.Text = "";
                selectedPicPanel.Invalidate();
            }
            else
            {
                errorLabel.Text = "Your password should be a number.";
            }
        }

        private void selectedPicPanel_Paint(object sender, PaintEventArgs e)
        {
            int tempX, tempY;
            if (firstPaint && !selectedNum.Equals("NoKey"))
            {
                e.Graphics.DrawString(selectedNum.ToString(), new Font("Segoe UI", 18),
                                new SolidBrush(Color.White), new PointF(250, 250));
            }
            else
            {
                if (!selectedNum.Equals("NoKey"))
                {
                    tempX = moveX - 20;
                    tempY = moveY - 25;
                    e.Graphics.DrawString(selectedNum, new Font("Segoe UI", 18),
                        new SolidBrush(Color.White), new PointF(tempX, tempY));
                }
            }
        }

        private void selectedPicPanel_MouseDown(object sender, MouseEventArgs e)
        {
            firstPaint = false;
            if (e.Button == MouseButtons.Left)
            {
                drag = true;
                downX = e.X;
                downY = e.Y;
            }
        }

        private void selectedPicPanel_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
            selectedX = moveX - 20;
            selectedY = moveY - 25;
        }

        private void selectedPicPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                if (e.X >= 20 && e.X < 500)
                    moveX = e.X;
                if (e.Y >= 25 && e.Y < 490)
                    moveY = e.Y;
                selectedPicPanel.Invalidate();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            if (selectedNum.Equals("NoKey"))
            {
                errorLabel.Text = "Select a number as your password.";
            }
            else
            {
                SavePassword();
                this.Dispose();
            }
        }
    }
}
