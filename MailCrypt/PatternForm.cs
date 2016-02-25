using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailCrypt
{
    public partial class PatternForm : Form
    {
        int downX, downY, moveX, moveY, gridX = 0, gridY = 0, keyX, keyY, keyNum, curI = 0, curJ = 0, tryCount = 0;
        Random rand = new Random();
        int[,] randNums = new int[50, 50];
        bool firstPaint = true, keyFlag = false, drag = false, appBlocked = false;
        String selectedPic;

        static byte[] keyString = ASCIIEncoding.ASCII.GetBytes("z9!y8@x7");

        public PatternForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            CheckTime();
        }

        private void CheckTime()
        {
            try
            {
                String lockTimeString = File.ReadAllText(Environment.CurrentDirectory + "\\data.mfa");
                DateTime lockTime = Convert.ToDateTime(lockTimeString);
                TimeSpan timeDiff = new TimeSpan(0, 0, 0);
                timeDiff = (DateTime.Now.ToLocalTime() - lockTime);
                if (timeDiff < new TimeSpan(0, 15, 0))
                {
                    messageLabel.Text = "Try After " + (new TimeSpan(0, 15, 0).Minutes - timeDiff.Minutes).ToString() + " Minutes";
                    messageLabel.Visible = true;
                    appBlocked = true;
                    statusLabel.Text = "Application Blocked";
                    this.Invalidate();
                }
                else
                {
                    statusLabel.Text = "Picture Password";
                    appBlocked = false;
                    messageLabel.Visible = false;
                    LoadValues();
                    CreateGrid();
                }
            }
            catch
            {
                statusLabel.Text = "Picture Password";
                appBlocked = false;
                messageLabel.Visible = false;
                LoadValues();
                CreateGrid();
            }

        }

        private void LoadValues()
        {
            String[] picData;
            char[] dataDivider = { ';' };
            String decryptedString;
            String cryptedString = File.ReadAllText(Environment.CurrentDirectory + "\\config.mfa");

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(keyString, keyString), CryptoStreamMode.Read);
            StreamReader readerStream = new StreamReader(cryptoStream);
            decryptedString = readerStream.ReadToEnd();

            picData = decryptedString.Split(dataDivider);
            selectedPic = picData[0];
            keyX = Convert.ToInt32(picData[1]);
            keyY = Convert.ToInt32(picData[2]);
            keyNum = Convert.ToInt32(picData[3]);

            switch (selectedPic)
            {
                case "PIC_1":
                    this.BackgroundImage = MailCrypt.Properties.Resources.PIC_1;
                    break;
                case "PIC_2":
                    this.BackgroundImage = MailCrypt.Properties.Resources.PIC_2;
                    break;
                case "PIC_3":
                    this.BackgroundImage = MailCrypt.Properties.Resources.PIC_3;
                    break;
                case "PIC_4":
                    this.BackgroundImage = MailCrypt.Properties.Resources.PIC_4;
                    break;
                case "PIC_5":
                    this.BackgroundImage = MailCrypt.Properties.Resources.PIC_5;
                    break;
            }
        }

        private void CreateGrid()
        {
            int i, j, keyCount = 0;
            for (i = 0; i < 50; i++)
            {
                keyCount = 0;
                for (j = 0; j < 50; j++)
                {
                    randNums[i, j] = rand.Next(10);
                    if (randNums[i, j] == keyNum)
                        keyCount++;
                }
                if (keyCount < 1)
                    j--;
            }
        }

        public void MouseisDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !appBlocked)
            {
                drag = true;
                downX = e.X;
                downY = e.Y;
            }
        }

        public void MouseisUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!appBlocked)
            {
                drag = false;
                if (keyFlag)
                {
                    this.Dispose();
                }
                else
                {
                    tryCount++;
                    statusLabel.Text = "Chances Left : " + (3 - tryCount);
                    if (tryCount >= 3)
                    {
                        statusLabel.Text = "Application Blocked";
                        File.WriteAllText(Environment.CurrentDirectory + "\\data.mfa", DateTime.Now.ToLocalTime().ToString());
                        CheckTime();
                    }
                }
            }
        }

        public void MouseMoves(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!appBlocked)
            {
                if (drag)
                {
                    moveX = e.X;
                    moveY = e.Y;
                    this.Invalidate();
                }
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            int tempX, tempY;
            if (firstPaint && !appBlocked)
            {
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        tempX = gridX + ((i - 25) * 75);
                        tempY = gridY + ((j - 25) * 75);
                        e.Graphics.DrawString(randNums[i, j].ToString(), new Font("Segoe UI", 20),
                            new SolidBrush(Color.White), new PointF(tempX, tempY));
                    }
                }
                firstPaint = false;
            }
            else if (!appBlocked)
            {
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        tempX = moveX - downX + ((i - 25) * 75);
                        tempY = moveY - downY + ((j - 25) * 75);
                        e.Graphics.DrawString(randNums[i, j].ToString(), new Font("Segoe UI", 18),
                            new SolidBrush(Color.White), new PointF(tempX, tempY));

                        if (randNums[i, j] == keyNum)
                        {
                            if ((Math.Abs(keyX - tempX) < 10) && (Math.Abs(keyY - tempY) < 10))
                            {
                                keyFlag = true;
                                curI = i;
                                curJ = j;
                            }
                            else
                            {
                                tempX = moveX - downX + ((curI - 25) * 75);
                                tempY = moveY - downY + ((curJ - 25) * 75);
                                if (!(Math.Abs(keyX - tempX) < 10 && Math.Abs(keyY - tempY) < 10))
                                {
                                    keyFlag = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PatternForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!keyFlag)
                Environment.Exit(0);
        }

        private void checkTimeTimer_Tick(object sender, EventArgs e)
        {
            if (appBlocked)
            {
                CheckTime();
            }
        }
    }
}
