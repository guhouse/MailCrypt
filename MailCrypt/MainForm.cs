using ActiveUp.Net.Mail;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Sockets;

namespace MailCrypt
{
    public partial class MainForm : Form
    {
        static Imap4Client imapClient = new Imap4Client();
        static System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
        static bool loggedIn = false, isMailEncrypted = false, isLocked = true, isFocused = true, newsUpdating = true,
            isDeleting = false, isFetching = false, isDrag = false;
        static String curUserId, curPassword, inboxPath, outboxPath, mailInfoPath, curMailId = "NoData",
            curMailbox = "NoData", curMailStatus, curToAddress, filePass = "z9!y8@x7", mailPass = "7x@8y!9z",
            toAddress, subText, mailBody;
        static Point downPoint;
        static DateTime statusSetTime = new DateTime();
        static DateTime lastActiveTime = new DateTime();
        static Point lastMousePos;
        static TimeSpan timeDiff = new TimeSpan(0, 0, 5);
        static List<String> inMailIDs = new List<string>();
        static List<String> outMailIDs = new List<string>();

        private XmlTextReader rssReader;
        private XmlDocument rssDoc;
        private XmlNode nodeRss;
        private XmlNode nodeChannel;
        private XmlNode nodeItem;
        private int newsIndex = 0;

        public MainForm()
        {
            InitializeComponent();

            lastActiveTime = DateTime.Now.ToLocalTime();
            lastMousePos = Cursor.Position;

            this.SetStyle(ControlStyles.ResizeRedraw, true);

            if (File.Exists(Environment.CurrentDirectory + "\\config.mfa"))
            {
                PatternForm welcomeForm = new PatternForm();
                welcomeForm.ShowDialog(this);
            }
            SetStatus("Welcome.");
            isLocked = false;
            SetComponents();
            new Thread(GetRssFeeds).Start();
        }

        private void SetComponents()
        {
            loginPanel.Visible = true;
            mailComposePanel.Visible = false;
            inboxPanel.Visible = false;
            logoutPanel.Visible = false;
            mailOpenPanel.Visible = false;

            viewLoginButton.BackColor = Color.FromArgb(255, 80, 89, 123);
            composeMailButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            inboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            outboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            setPicPassButton.BackColor = Color.FromArgb(255, 57, 66, 100);

            inboxTable.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            outboxTable.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            rssTable.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
        }

        private void ConnectMail()
        {
            String username = "", password = "";
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    loginButton.Enabled = false;
                    username = usernameFeild.Text;
                    password = passwordFeild.Text;
                });

                //IMAP Connection
                imapClient.ConnectSsl("imap.gmail.com", 993);
                imapClient.LoginFast(username, password);
                imapClient.Command("capability");

                //SMTP Connection
                smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
                smtpClient.EnableSsl = true;

                curUserId = username;
                curPassword = password;

                if (imapClient.IsConnected)
                {
                    loggedIn = true;
                    SetStatus("Login successful");
                    this.Invoke((MethodInvoker)delegate
                    {
                        logoutPanel.Visible = true;
                    });
                }
            }
            catch (SocketException se)
            {
                imapClient = new Imap4Client();
                SetStatus("No internet connection.");
                this.Invoke((MethodInvoker)delegate
                {
                    loginButton.Enabled = true;
                });
            }
            catch (Exception er)
            {
                imapClient = new Imap4Client();
                SetStatus("Invalid credentials.");
                this.Invoke((MethodInvoker)delegate
                {
                    loginButton.Enabled = true;
                    usernameFeild.Text = "";
                    passwordFeild.Text = "";
                });
            }
        }

        private void SetDirectories()
        {
            if (!System.IO.Directory.Exists(Environment.CurrentDirectory + "\\" + curUserId + "\\Inbox\\"))
                System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + curUserId + "\\Inbox\\");
            if (!System.IO.Directory.Exists(Environment.CurrentDirectory + "\\" + curUserId + "\\Outbox\\"))
                System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + curUserId + "\\Outbox\\");
            if (!System.IO.Directory.Exists(Environment.CurrentDirectory + "\\" + curUserId + "\\MailInfo\\"))
                System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\\" + curUserId + "\\MailInfo\\");

            inboxPath = Environment.CurrentDirectory + "\\" + curUserId + "\\Inbox\\";
            outboxPath = Environment.CurrentDirectory + "\\" + curUserId + "\\Outbox\\";
            mailInfoPath = Environment.CurrentDirectory + "\\" + curUserId + "\\MailInfo\\";
        }

        private void GetRssFeeds()
        {
            try
            {
                newsUpdating = true;
                rssReader = new XmlTextReader("http://rss.cnn.com/rss/edition.rss");
                rssDoc = new XmlDocument();
                rssDoc.Load(rssReader);

                for (int i = 0; i < rssDoc.ChildNodes.Count; i++)
                {
                    if (rssDoc.ChildNodes[i].Name == "rss")
                    {
                        nodeRss = rssDoc.ChildNodes[i];
                    }
                    Thread.Sleep(100);
                    if (i == 30)
                        break;
                }

                for (int i = 0; i < nodeRss.ChildNodes.Count; i++)
                {
                    if (nodeRss.ChildNodes[i].Name == "channel")
                    {
                        nodeChannel = nodeRss.ChildNodes[i];
                    }
                    Thread.Sleep(100);
                    if (i == 30)
                        break;
                }
                newsUpdating = false;
            }
            catch
            {
            }
        }

        private void GetInboxMails()
        {
            int inMailCount = 0;
            ActiveUp.Net.Mail.Message inMail = null;
            String inFilePath;
            String inMailBody;
            String mailFrom;
            String inMailSub;

            if (loggedIn && !isDeleting)
            {
                isFetching = true;
                SetDirectories();

                try
                {
                    SetStatus("Getting data from server.");
                    Mailbox myMailBox = imapClient.SelectMailbox("Inbox");
                    inMailCount = myMailBox.MessageCount;

                    for (int i = 1; i <= inMailCount; i++)
                    {
                        inMail = myMailBox.Fetch.MessageObject(i);
                        if (!File.Exists(inboxPath + inMail.MessageId))
                        {
                            //Inbox path
                            inFilePath = inboxPath + inMail.MessageId;
                            //Message parts
                            mailFrom = inMail.HeaderFields["from"];
                            inMailSub = inMail.HeaderFields["subject"];
                            inMailBody = inMail.BodyText.TextStripped;
                            inMailBody = Decrypt(inMailBody, mailPass);
                            if (inMailBody.Equals("error") || inMailBody.Equals("wrongkey"))
                            {
                                inMailBody = inMail.BodyText.TextStripped;
                            }

                            File.WriteAllText(inFilePath, Encrypt(mailFrom + "~" + inMailSub + "~" + inMailBody, filePass));
                        }
                    }
                    isFetching = false;
                    LoadInbox();
                }
                catch
                {
                    SetStatus("Wait...");
                }
            }
        }

        private void GetOutboxMails()
        {
            int outMailCount = 0;
            ActiveUp.Net.Mail.Message outMail = null;
            String outFilePath;
            String outMailBody;
            String mailTo;
            String outMailSub;

            if (loggedIn && !isDeleting)
            {
                isFetching = true;
                SetDirectories();

                try
                {
                    SetStatus("Getting data from server.");
                    Mailbox myMailBox = imapClient.SelectMailbox("[Gmail]/Sent Mail");
                    outMailCount = myMailBox.MessageCount;

                    for (int i = 1; i <= outMailCount; i++)
                    {
                        outMail = myMailBox.Fetch.MessageObject(i);
                        if (!File.Exists(outboxPath + outMail.MessageId))
                        {
                            //Outbox path
                            outFilePath = outboxPath + outMail.MessageId;

                            //Message parts
                            mailTo = outMail.HeaderFields["to"];
                            outMailSub = outMail.HeaderFields["subject"];
                            outMailBody = outMail.BodyText.TextStripped;

                            outMailBody = Decrypt(outMailBody, mailPass);
                            if (outMailBody.Equals("error") || outMailBody.Equals("wrongkey"))
                            {
                                outMailBody = outMail.BodyText.TextStripped;
                            }

                            File.WriteAllText(outFilePath, Encrypt(mailTo + "~" + outMailSub + "~" + outMailBody, filePass));
                        }
                    }
                    isFetching = false;
                    LoadOutbox();
                }
                catch
                {
                    SetStatus("Wait...");
                }
            }
        }

        private void LoadInbox()
        {
            try
            {
                SetDirectories();

                if (loggedIn && !isFetching)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        inboxTable.Controls.Clear();
                    });
                    inMailIDs.Clear();
                    String[] mails = Directory.GetFiles(inboxPath);

                    int inMailCount = mails.Length;
                    char[] lineBreak = { '~' };
                    Label snoLabel, nameLabel, subjectLabel;
                    Button deleteButton;
                    int rowCount = 0;
                    String decryptedString, cryptedString;

                    for (int i = mails.Length - 1; i >= 0; i--)
                    {
                        //Read and decrypt mails
                        cryptedString = File.ReadAllText(mails[i]);
                        decryptedString = Decrypt(cryptedString, filePass);
                        String[] headerInfo = decryptedString.Split(lineBreak, 3, StringSplitOptions.RemoveEmptyEntries);

                        //Store IDs
                        inMailIDs.Add(mails[i].Substring(mails[i].LastIndexOf("\\") + 1));

                        //Set id, name and subject
                        snoLabel = new Label();
                        snoLabel.Text = (rowCount + 1).ToString();
                        snoLabel.Font = new Font("Segoe UI Light", 14);
                        snoLabel.TextAlign = ContentAlignment.MiddleLeft;
                        snoLabel.ForeColor = Color.Black;
                        snoLabel.UseCompatibleTextRendering = true;
                        snoLabel.AutoEllipsis = true;
                        snoLabel.Dock = DockStyle.Fill;

                        nameLabel = new Label();
                        nameLabel.Text = headerInfo[0].Trim();
                        nameLabel.Font = new Font("Segoe UI Light", 14);
                        nameLabel.TextAlign = ContentAlignment.MiddleLeft;
                        nameLabel.ForeColor = Color.Black;
                        nameLabel.UseCompatibleTextRendering = true;
                        nameLabel.AutoEllipsis = true;
                        nameLabel.Dock = DockStyle.Fill;

                        subjectLabel = new Label();
                        subjectLabel.Text = headerInfo[1].Trim();
                        subjectLabel.Font = new Font("Segoe UI Light", 14);
                        subjectLabel.TextAlign = ContentAlignment.MiddleLeft;
                        subjectLabel.ForeColor = Color.Black;
                        subjectLabel.UseCompatibleTextRendering = true;
                        subjectLabel.AutoEllipsis = true;
                        subjectLabel.Dock = DockStyle.Fill;
                        subjectLabel.Cursor = Cursors.Hand;

                        deleteButton = new Button();
                        deleteButton.Text = "";
                        deleteButton.Image = global::MailCrypt.Properties.Resources.del;
                        deleteButton.ImageAlign = ContentAlignment.MiddleCenter;
                        deleteButton.AutoSize = true;
                        deleteButton.FlatStyle = FlatStyle.Flat;
                        deleteButton.Click += new EventHandler(inboxDeleteButton_Click);
                        deleteButton.Dock = DockStyle.Fill;

                        //Add to inbox list
                        this.Invoke((MethodInvoker)delegate
                        {
                            inboxTable.Controls.Add(snoLabel, 0, rowCount);
                            inboxTable.Controls.Add(nameLabel, 1, rowCount);
                            inboxTable.Controls.Add(subjectLabel, 2, rowCount);
                            inboxTable.Controls.Add(deleteButton, 3, rowCount);
                        });
                        rowCount++;

                        //Add mouse listner
                        subjectLabel.MouseDoubleClick += new MouseEventHandler(inSubjectLabel_MouseDoubleClick);
                    }
                }
            }
            catch
            {
                SetStatus("Wait...");
            }
        }

        private void LoadOutbox()
        {
            try
            {
                SetDirectories();

                if (loggedIn && !isFetching)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        outboxTable.Controls.Clear();
                    });
                    outMailIDs.Clear();

                    String[] mails = Directory.GetFiles(outboxPath);
                    int outMailCount = mails.Length;
                    char[] lineBreak = { '~' };
                    Label snoLabel, nameLabel, subjectLabel;
                    Button deleteButton;
                    int rowCount = 0;
                    String decryptedString;
                    String cryptedString;

                    for (int i = mails.Length - 1; i >= 0; i--)
                    {
                        //Read and decrypt mails
                        cryptedString = File.ReadAllText(mails[i]);
                        decryptedString = Decrypt(cryptedString, filePass);
                        String[] headerInfo = decryptedString.Split(lineBreak, 3, StringSplitOptions.RemoveEmptyEntries);

                        //Store IDs
                        outMailIDs.Add(mails[i].Substring(mails[i].LastIndexOf("\\") + 1));

                        //Set sno, name and subject
                        snoLabel = new Label();
                        snoLabel.Text = (rowCount + 1).ToString();
                        snoLabel.Font = new Font("Segoe UI Light", 14);
                        snoLabel.TextAlign = ContentAlignment.MiddleLeft;
                        snoLabel.ForeColor = Color.Black;
                        snoLabel.UseCompatibleTextRendering = true;
                        snoLabel.AutoEllipsis = true;
                        snoLabel.Dock = DockStyle.Fill;

                        nameLabel = new Label();
                        nameLabel.Text = headerInfo[0].Trim();
                        nameLabel.Font = new Font("Segoe UI Light", 14);
                        nameLabel.TextAlign = ContentAlignment.MiddleLeft;
                        nameLabel.ForeColor = Color.Black;
                        nameLabel.UseCompatibleTextRendering = true;
                        nameLabel.AutoEllipsis = true;
                        nameLabel.Dock = DockStyle.Fill;

                        subjectLabel = new Label();
                        subjectLabel.Text = headerInfo[1].Trim();
                        subjectLabel.Font = new Font("Segoe UI Light", 14);
                        subjectLabel.TextAlign = ContentAlignment.MiddleLeft;
                        subjectLabel.ForeColor = Color.Black;
                        subjectLabel.UseCompatibleTextRendering = true;
                        subjectLabel.AutoEllipsis = true;
                        subjectLabel.Dock = DockStyle.Fill;
                        subjectLabel.Cursor = Cursors.Hand;

                        deleteButton = new Button();
                        deleteButton.Text = "";
                        deleteButton.Image = global::MailCrypt.Properties.Resources.del;
                        deleteButton.ImageAlign = ContentAlignment.MiddleCenter;
                        deleteButton.AutoSize = true;
                        deleteButton.FlatStyle = FlatStyle.Flat;
                        deleteButton.Click += new EventHandler(outboxDeleteButton_Click);
                        deleteButton.Dock = DockStyle.Fill;

                        //Add to outbox list
                        this.Invoke((MethodInvoker)delegate
                        {
                            outboxTable.Controls.Add(snoLabel, 0, rowCount);
                            outboxTable.Controls.Add(nameLabel, 1, rowCount);
                            outboxTable.Controls.Add(subjectLabel, 2, rowCount);
                            outboxTable.Controls.Add(deleteButton, 3, rowCount);
                        });
                        rowCount++;

                        //Add mouse listner
                        subjectLabel.MouseDoubleClick += new MouseEventHandler(outSubjectLabel_MouseDoubleClick);
                    }
                }
            }
            catch
            {
                SetStatus("Wait...");
            }
        }

        private void ReadInMail(String mailID)
        {
            char[] lineBreak = { '~' };
            String[] mailParts;
            String decryptedString;
            String cryptedString;

            ClearMailView();

            cryptedString = File.ReadAllText(inboxPath + mailID);
            decryptedString = Decrypt(cryptedString, filePass);
            mailParts = decryptedString.Split(lineBreak, 3, StringSplitOptions.None);
            curMailId = mailID;
            curMailbox = "inbox";

            mailViewLabel_1.Text = "From :";
            curToAddress = mailViewFrom.Text = mailParts[0];
            mailViewSub.Text = mailParts[1];

            if (mailParts[2].Contains("*mfa.encrypted.mail*"))
            {
                keyLabel.Visible = true;
                keyField_1.Visible = true;
                decryptButton.Visible = true;
                decryptButton.Text = "Decrypt";
                mailViewBody.Text = mailParts[2].Remove(0, 20);
            }
            else
            {
                keyLabel.Visible = false;
                keyField_1.Visible = false;
                decryptButton.Visible = false;
                mailViewBody.Text = mailParts[2];
            }

            mailOpenPanel.Visible = true;
            loginPanel.Visible = false;
            mailComposePanel.Visible = false;
            inboxPanel.Visible = false;
            logoutPanel.Visible = false;
        }

        private void ReadOutMail(String mailID)
        {
            char[] lineBreak = { '~' };
            String[] mailParts;
            String decryptedString;
            String cryptedString;

            ClearMailView();

            cryptedString = File.ReadAllText(outboxPath + mailID);
            decryptedString = Decrypt(cryptedString, filePass);
            mailParts = decryptedString.Split(lineBreak, 3, StringSplitOptions.None);
            curMailId = mailID;
            curMailbox = "outbox";

            mailViewLabel_1.Text = "To :";
            mailViewFrom.Text = mailParts[0];
            mailViewSub.Text = mailParts[1];

            if (mailParts[2].Contains("*mfa.encrypted.mail*"))
            {
                keyLabel.Visible = true;
                keyField_1.Visible = true;
                decryptButton.Visible = true;
                decryptButton.Text = "Decrypt";
                mailViewBody.Text = mailParts[2].Remove(0, 20);
            }
            else
            {
                keyLabel.Visible = false;
                keyField_1.Visible = false;
                decryptButton.Visible = false;
                mailViewBody.Text = mailParts[2];
            }

            mailOpenPanel.Visible = true;
            loginPanel.Visible = false;
            mailComposePanel.Visible = false;
            inboxPanel.Visible = false;
            outboxPanel.Visible = false;
            logoutPanel.Visible = false;
        }

        private void DeleteMail()
        {
            if (!isDeleting)
            {
                isDeleting = true;
                if (curMailbox.Equals("inbox"))
                {
                    try
                    {
                        File.Delete(inboxPath + curMailId);
                        LoadInbox();

                        Mailbox myMailBox = imapClient.SelectMailbox("Inbox");
                        int[] ids = myMailBox.Search("ALL");

                        if (ids.Length > 0)
                        {
                            ActiveUp.Net.Mail.Message msg = null;
                            for (int i = 0; i < ids.Length; i++)
                            {
                                msg = myMailBox.Fetch.MessageObject(ids[i]);
                                if (msg.MessageId.Equals(curMailId))
                                {
                                    imapClient.Command("copy " + ids[i].ToString() + " [Gmail]/Trash");
                                    SetStatus("Mail deleted");
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        SetStatus("Wait...");
                    }
                }
                else if (curMailbox.Equals("outbox"))
                {
                    try
                    {
                        File.Delete(outboxPath + curMailId);
                        LoadOutbox();
                        Mailbox myMailBox = imapClient.SelectMailbox("[Gmail]/Sent Mail");
                        int[] ids = myMailBox.Search("ALL");

                        if (ids.Length > 0)
                        {
                            ActiveUp.Net.Mail.Message msg = null;
                            for (int i = 0; i < ids.Length; i++)
                            {
                                msg = myMailBox.Fetch.MessageObject(ids[i]);
                                if (msg.MessageId.Equals(curMailId))
                                {
                                    imapClient.Command("copy " + ids[i].ToString() + " [Gmail]/Trash");
                                    SetStatus("Mail deleted");
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        SetStatus("Wait...");
                    }
                }
                SetDirectories();
                isDeleting = false;
            }
        }

        private void SendMail()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    sendButton.Enabled = false;
                    encryptButton.Enabled = false;
                    keyField_2.Enabled = false;
                    mailBodyField.Enabled = false;
                    subFeild.Enabled = false;
                    toFeild.Enabled = false;
                });
                SetStatus("Sending...");
                MailMessage newMail = new MailMessage();
                newMail.From = new MailAddress(curUserId + "@gmail.com");
                newMail.To.Add(toAddress);
                newMail.Subject = subText;

                if (isMailEncrypted)
                {
                    newMail.Body = "*mfa.encrypted.mail*" + mailBody;
                    newMail.Body = Encrypt(newMail.Body, mailPass);
                }
                else
                    newMail.Body = mailBody;

                smtpClient.Send(newMail);
                SetStatus("Mail sent.");
                ClearCompose();
            }
            catch
            {
                this.Invoke((MethodInvoker)delegate
                {
                    sendButton.Enabled = true;
                    encryptButton.Enabled = true;
                    keyField_2.Enabled = true;
                    mailBodyField.Enabled = false;
                    subFeild.Enabled = false;
                    toFeild.Enabled = false;
                });
                SetStatus("Sending failed.");
            }
        }

        private void ClearCompose()
        {
            this.Invoke((MethodInvoker)delegate
            {
                toFeild.Text = "";
                subFeild.Text = "";
                mailBodyField.Text = "";
                keyField_2.Text = "";
                isMailEncrypted = false;
                encryptButton.Text = "Encrypt";
                encryptButton.Image = global::MailCrypt.Properties.Resources.locked;
                mailBodyField.ReadOnly = false;

                this.Invoke((MethodInvoker)delegate
                {
                    sendButton.Enabled = true;
                    encryptButton.Enabled = true;
                    keyField_2.Enabled = true;
                    mailBodyField.Enabled = true;
                    subFeild.Enabled = true;
                    toFeild.Enabled = true;
                });
            });
        }

        private void ClearMailView()
        {
            this.Invoke((MethodInvoker)delegate
            {
                mailViewFrom.Text = "";
                mailViewSub.Text = "";
                mailViewBody.Text = "";
                keyField_1.Text = "";
            });
            isMailEncrypted = false;
        }

        private void CheckTries()
        {
            try
            {
                if (!System.IO.File.Exists(mailInfoPath + curMailId))
                {
                    decryptButton.Text = "Decrypt   1/10";
                    File.WriteAllText(mailInfoPath + curMailId, "1");
                }
                else
                {
                    String info = File.ReadAllText(mailInfoPath + curMailId);
                    int decryptPinTry = Convert.ToInt32(info);
                    decryptPinTry++;
                    if (decryptPinTry < 10)
                    {
                        decryptButton.Text = "Decrypt   " + decryptPinTry + "/10";
                        File.WriteAllText(mailInfoPath + curMailId, decryptPinTry.ToString());
                    }
                    else
                    {
                        decryptButton.Text = "Decrypt   " + decryptPinTry + "/10";
                        if (curMailbox.Equals("inbox"))
                        {
                            new Thread(DeleteMail).Start();
                        }
                        else if (curMailbox.Equals("outbox"))
                        {
                            new Thread(DeleteMail).Start();
                        }
                        File.Delete(mailInfoPath + curMailId);
                        curMailStatus = "deleted";
                        new Thread(SendAlert).Start();
                        ClearMailView();
                        mailOpenPanel.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message);
            }
        }

        private void LockApp()
        {
            guiUpdateTimer.Stop();
            isLocked = true;

            viewLoginButton.BackColor = Color.FromArgb(255, 80, 89, 123);
            composeMailButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            inboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            outboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            setPicPassButton.BackColor = Color.FromArgb(255, 57, 66, 100);

            if (imapClient.IsConnected)
            {
                loginPanel.Visible = true;
                logoutPanel.Visible = true;
            }
            else
            {
                loginPanel.Visible = true;
                logoutPanel.Visible = false;
            }

            mailComposePanel.Visible = false;
            mailOpenPanel.Visible = false;
            inboxPanel.Visible = false;
            outboxPanel.Visible = false;

            PatternForm welcomeForm = new PatternForm();
            welcomeForm.ShowDialog(this);
            lastActiveTime = DateTime.Now.ToLocalTime();
            isLocked = false;
            guiUpdateTimer.Start();
        }

        private void SendAlert()
        {
            if (curMailbox.Equals("inbox"))
            {
                MailMessage newMail = new MailMessage();
                newMail.From = new MailAddress(curUserId + "@gmail.com");
                newMail.To.Add(curToAddress);
                newMail.Subject = "MailCrypt Alert";
                if (curMailStatus.Equals("decrypted"))
                    newMail.Body = curUserId + " has successfully decrypted your message.";
                else if (curMailStatus.Equals("deleted"))
                    newMail.Body = curUserId + " could not decrypt your message. Your message has been deleted.";

                smtpClient.Send(newMail);
                SetStatus("Alert sent");
            }
        }

        private void SetStatus(String status)
        {
            try
            {
                statusSetTime = DateTime.Now.ToLocalTime();
                this.Invoke((MethodInvoker)delegate
                {
                    statusLabel.Text = status;
                });
            }
            catch
            {
            }
        }

        private String Encrypt(String textString, String keyString)
        {
            try
            {
                Byte[] keyBytes = ASCIIEncoding.ASCII.GetBytes(keyString);
                for (int i = 0; i < keyBytes.Length; i++)
                {
                    keyBytes[i] = (Byte)(keyBytes[i] * keyBytes[i]);
                }
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(keyBytes, keyBytes), CryptoStreamMode.Write);
                StreamWriter streamWriter = new StreamWriter(cryptoStream);

                streamWriter.Write(textString);
                streamWriter.Flush();
                cryptoStream.FlushFinalBlock();
                streamWriter.Flush();

                String cypherText = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                return cypherText;
            }
            catch
            {
                return "error";
            }
        }

        private String Decrypt(String cypherText, String keyString)
        {
            try
            {
                Byte[] keyBytes = ASCIIEncoding.ASCII.GetBytes(keyString);
                for (int i = 0; i < keyBytes.Length; i++)
                {
                    keyBytes[i] = (Byte)(keyBytes[i] * keyBytes[i]);
                }
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cypherText));
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(keyBytes, keyBytes), CryptoStreamMode.Read);
                StreamReader readerStream = new StreamReader(cryptoStream);
                String mailText = readerStream.ReadToEnd();

                return mailText;
            }
            catch (CryptographicException)
            {
                return "wrongkey";
            }
            catch
            {
                return "error";
            }
        }

        private void inboxDeleteButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            Button delButton = (Button)sender;
            int clickedRow = inboxTable.GetRow(delButton);
            Label snoLabel = (Label)inboxTable.GetControlFromPosition(0, clickedRow);
            int index = Convert.ToInt32(snoLabel.Text.ToString()) - 1;
            curMailId = inMailIDs[index];
            curMailbox = "inbox";
            new Thread(DeleteMail).Start();
        }

        private void outboxDeleteButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            Button delButton = (Button)sender;
            int clickedRow = outboxTable.GetRow(delButton);
            Label snoLabel = (Label)outboxTable.GetControlFromPosition(0, clickedRow);
            int index = Convert.ToInt32(snoLabel.Text.ToString()) - 1;
            curMailId = outMailIDs[index];
            curMailbox = "outbox";
            new Thread(DeleteMail).Start();
        }

        private void viewLoginButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            viewLoginButton.BackColor = Color.FromArgb(255, 80, 89, 123);
            composeMailButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            inboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            outboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            setPicPassButton.BackColor = Color.FromArgb(255, 57, 66, 100);

            if (imapClient.IsConnected)
            {
                loginPanel.Visible = true;
                logoutPanel.Visible = true;
            }
            else
            {
                loginPanel.Visible = true;
                logoutPanel.Visible = false;
            }

            mailComposePanel.Visible = false;
            mailOpenPanel.Visible = false;
            inboxPanel.Visible = false;
            outboxPanel.Visible = false;
        }

        private void composeMailButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            if (loggedIn)
            {
                mailComposePanel.Visible = true;
                mailOpenPanel.Visible = false;
                inboxPanel.Visible = false;
                outboxPanel.Visible = false;
                loginPanel.Visible = false;

                viewLoginButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                composeMailButton.BackColor = Color.FromArgb(255, 80, 89, 123);
                inboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                outboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                setPicPassButton.BackColor = Color.FromArgb(255, 57, 66, 100);
            }
        }

        private void inboxButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            if (loggedIn)
            {
                if (inboxPanel.Visible)
                {
                    if (!isDeleting && !isFetching)
                        new Thread(GetInboxMails).Start();
                }
                loginPanel.Visible = false;
                mailOpenPanel.Visible = false;
                mailComposePanel.Visible = false;
                inboxPanel.Visible = true;
                outboxPanel.Visible = false;
                inboxTable.Focus();

                viewLoginButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                composeMailButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                inboxButton.BackColor = Color.FromArgb(255, 80, 89, 123);
                outboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                setPicPassButton.BackColor = Color.FromArgb(255, 57, 66, 100);

                if (!isDeleting && !isFetching)
                    new Thread(LoadInbox).Start();
            }
        }

        private void outboxButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            if (loggedIn)
            {
                if (outboxPanel.Visible)
                {
                    if (!isDeleting && !isFetching)
                        new Thread(GetOutboxMails).Start();
                }
                loginPanel.Visible = false;
                mailComposePanel.Visible = false;
                mailOpenPanel.Visible = false;
                inboxPanel.Visible = false;
                outboxPanel.Visible = true;
                outboxTable.Focus();

                viewLoginButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                composeMailButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                inboxButton.BackColor = Color.FromArgb(255, 57, 66, 100);
                outboxButton.BackColor = Color.FromArgb(255, 80, 89, 123);
                setPicPassButton.BackColor = Color.FromArgb(255, 57, 66, 100);

                if (!isDeleting && !isFetching)
                    new Thread(LoadOutbox).Start();
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            if (usernameFeild.Text.Equals("") || passwordFeild.Text.Equals(""))
            {
                SetStatus("Enter username and password.");
            }
            else
            {
                new Thread(ConnectMail).Start();
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            try
            {
                loginButton.Enabled = true;
                loggedIn = false;
                inboxTable.Controls.Clear();
                outboxTable.Controls.Clear();
                imapClient.Disconnect();
                logoutPanel.Visible = false;

                imapClient = new Imap4Client();
                SetStatus("You have logged out.");
                SetComponents();
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message);
            }
        }

        private void setPicPassButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            setPicPassButton.BackColor = Color.FromArgb(255, 80, 89, 123);

            SetPasswordForm passwordSetForm = new SetPasswordForm();
            passwordSetForm.ShowDialog(this);

            setPicPassButton.BackColor = Color.FromArgb(255, 57, 66, 100);
        }

        private void inSubjectLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            Label subLabel = (Label)sender;
            int clickedRow = inboxTable.GetRow(subLabel);
            Label snoLabel = (Label)inboxTable.GetControlFromPosition(0, clickedRow);
            int index = Convert.ToInt32(snoLabel.Text.ToString()) - 1;
            String mailID = inMailIDs[index];

            ReadInMail(mailID);
        }

        private void outSubjectLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            Label subLabel = (Label)sender;
            int clickedRow = outboxTable.GetRow(subLabel);
            Label snoLabel = (Label)outboxTable.GetControlFromPosition(0, clickedRow);
            int index = Convert.ToInt32(snoLabel.Text.ToString()) - 1;
            String mailID = outMailIDs[index];

            ReadOutMail(mailID);
        }

        private void encryptButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            if (keyField_2.Text.Equals(""))
            {
                SetStatus("Enter key first.");
            }
            else
            {
                if (isMailEncrypted)
                {
                    String mailText = Decrypt(mailBodyField.Text.ToString(), keyField_2.Text);
                    if (mailText.Equals("error"))
                    {
                        SetStatus("Error.");
                    }
                    else if (mailText.Equals("wrongkey"))
                    {
                        SetStatus("Invalid key.");
                    }
                    else
                    {
                        mailBodyField.Text = mailText;
                        mailBodyField.Font = new Font("Segoe UI Light", 14);
                        mailBodyField.ReadOnly = false;
                        keyField_2.Text = "";
                        encryptButton.Text = "Encrypt";
                        encryptButton.Image = global::MailCrypt.Properties.Resources.locked;
                        isMailEncrypted = false;
                    }
                }
                else
                {
                    String mailText = Encrypt(mailBodyField.Text.ToString(), keyField_2.Text);
                    if (mailText.Equals("error"))
                    {
                        SetStatus("Error.");
                    }
                    else
                    {
                        mailBodyField.Text = mailText;
                        mailBodyField.Font = new Font("Segoe UI Light", 14);
                        mailBodyField.ReadOnly = true;
                        keyField_2.Text = "";
                        encryptButton.Text = "Decrypt";
                        encryptButton.Image = global::MailCrypt.Properties.Resources.unlocked;
                        isMailEncrypted = true;
                    }
                }
            }
        }

        private void keyField_2_MouseClick(object sender, MouseEventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();
            keyField_2.Text = "";
        }

        private void mailBodyField_TextChanged(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();
            mailBodyField.Font = new Font("Segoe UI Light", 14);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            if (loggedIn)
            {
                if (!toFeild.Text.Contains("@") || !toFeild.Text.Contains("."))
                {
                    SetStatus("Invalid address.");
                }
                else if (toFeild.Text.Equals("") || subFeild.Text.Equals("") || mailBodyField.Text.Equals(""))
                {
                    SetStatus("Empty fields are not allowed.");
                }
                else
                {
                    toAddress = toFeild.Text.ToString();
                    subText = subFeild.Text.ToString();
                    mailBody = mailBodyField.Text.ToString();
                    new Thread(SendMail).Start();
                }
            }
            else
            {
                SetStatus("Not logged in.");
                loginPanel.Visible = true;
            }
        }

        private void decryptButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            String cypherMailText = mailViewBody.Text.ToString();
            String mailText = Decrypt(cypherMailText, keyField_1.Text.ToString());
            if (mailText.Equals("error"))
            {
                SetStatus("Error.");
            }
            else if (mailText.Equals("wrongkey"))
            {
                SetStatus("Invalid key.");
                CheckTries();
            }
            else
            {
                mailViewBody.Text = mailText;
                decryptButton.Visible = false;
                keyField_1.Visible = false;
                keyLabel.Visible = false;
                curMailStatus = "decrypted";
                new Thread(SendAlert).Start();
            }
        }

        private void keyField_1_MouseClick(object sender, MouseEventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();
            keyField_1.Text = "";
        }

        private void replyButton_Click(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();

            String replyAddress = mailViewFrom.Text.ToString();
            ClearCompose();

            mailComposePanel.Visible = true;
            encryptButton.Text = "Encrypt";
            encryptButton.Image = global::MailCrypt.Properties.Resources.locked;
            mailOpenPanel.Visible = false;
            inboxPanel.Visible = false;
            outboxPanel.Visible = false;
            loginPanel.Visible = false;

            toFeild.Text = replyAddress;
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            isFocused = true;
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            isFocused = false;
        }

        private void usernameFeild_TextChanged(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();
        }

        private void passwordFeild_TextChanged(object sender, EventArgs e)
        {
            lastActiveTime = DateTime.Now.ToLocalTime();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                inboxTable.Controls.Clear();
                outboxTable.Controls.Clear();
                imapClient.Disconnect();
                logoutPanel.Visible = false;
                loggedIn = false;
            }
            catch
            {

            }
        }

        private void guiUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (lastMousePos != Cursor.Position)
            {
                if (isFocused)
                    lastActiveTime = DateTime.Now.ToLocalTime();
            }
            lastMousePos = Cursor.Position;

            timeDiff = (DateTime.Now.ToLocalTime() - statusSetTime);
            if (timeDiff > new TimeSpan(0, 0, 5))
            {
                this.Invoke((MethodInvoker)delegate
                {
                    statusLabel.Text = "";
                });
            }

            timeDiff = (DateTime.Now.ToLocalTime() - lastActiveTime);
            if (timeDiff > new TimeSpan(0, 5, 0))
            {
                if (File.Exists(Environment.CurrentDirectory + "\\config.mfa") && !isLocked)
                {
                    LockApp();
                }
            }
        }

        private void newsTimer_Tick(object sender, EventArgs e)
        {
            Label headLabel, detailLabel;
            if (!newsUpdating)
            {
                try
                {
                    if (newsIndex == 30)
                    {
                        new Thread(GetRssFeeds).Start();
                        newsIndex = 0;
                    }
                    if (nodeChannel.ChildNodes[newsIndex].Name == "item")
                    {
                        newsTimer.Interval = 10000;
                        nodeItem = nodeChannel.ChildNodes[newsIndex];

                        headLabel = new Label();
                        headLabel.Text = nodeItem["title"].InnerText;
                        headLabel.Font = new Font("Segoe UI Light", 26);
                        headLabel.TextAlign = ContentAlignment.MiddleLeft;
                        headLabel.ForeColor = Color.RoyalBlue;
                        headLabel.UseCompatibleTextRendering = true;
                        headLabel.AutoSize = true;
                        headLabel.Dock = DockStyle.Fill;

                        detailLabel = new Label();
                        detailLabel.Text = nodeItem["description"].InnerText;
                        detailLabel.Font = new Font("Segoe UI Light", 14);
                        detailLabel.TextAlign = ContentAlignment.TopLeft;
                        detailLabel.ForeColor = Color.DimGray;
                        detailLabel.UseCompatibleTextRendering = true;
                        detailLabel.AutoSize = true;
                        detailLabel.Dock = DockStyle.Fill;

                        rssTable.Invoke((MethodInvoker)delegate
                        {
                            rssTable.Controls.Clear();
                            rssTable.Controls.Add(headLabel, 0, 0);
                            rssTable.Controls.Add(detailLabel, 0, 1);
                        });
                    }
                    else
                    {
                        newsTimer.Interval = 100;
                    }
                    newsIndex++;
                }
                catch
                {
                }
            }
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void titlePanel_MouseDown(object sender, MouseEventArgs e)
        {
            isDrag = true;
            downPoint = new Point(e.X, e.Y);
        }

        private void titlePanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDrag = false;
        }

        private void titlePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                this.Location = new Point(Cursor.Position.X - downPoint.X, Cursor.Position.Y - downPoint.Y);
                Invalidate();
            }
        }
    }
}
