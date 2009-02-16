namespace tracm
{
    partial class MainForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabWelcome = new System.Windows.Forms.TabPage();
            this.tabUpload = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.Length = new System.Windows.Forms.TextBox();
            this.Cue = new System.Windows.Forms.TextBox();
            this.Description = new System.Windows.Forms.TextBox();
            this.Producer = new System.Windows.Forms.TextBox();
            this.Genre = new System.Windows.Forms.TextBox();
            this.Subject = new System.Windows.Forms.TextBox();
            this.Title = new System.Windows.Forms.TextBox();
            this.Inentifier = new System.Windows.Forms.TextBox();
            this.VideoFileButton = new System.Windows.Forms.Button();
            this.VideoFile = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelVideoFile = new System.Windows.Forms.Label();
            this.tabQueue = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.groupBoxFiles = new System.Windows.Forms.GroupBox();
            this.FileBrowse = new System.Windows.Forms.Button();
            this.DownloadPath = new System.Windows.Forms.TextBox();
            this.labelDownloadFolder = new System.Windows.Forms.Label();
            this.groupBoxCablecast = new System.Windows.Forms.GroupBox();
            this.CablecastServer = new System.Windows.Forms.TextBox();
            this.CablecastPassword = new System.Windows.Forms.TextBox();
            this.labelCablecastServer = new System.Windows.Forms.Label();
            this.CablecastUsername = new System.Windows.Forms.TextBox();
            this.labelCablecastUsername = new System.Windows.Forms.Label();
            this.labelCablecastPassword = new System.Windows.Forms.Label();
            this.groupBoxACM = new System.Windows.Forms.GroupBox();
            this.ACMServer = new System.Windows.Forms.TextBox();
            this.ACMPassword = new System.Windows.Forms.TextBox();
            this.labelACMServer = new System.Windows.Forms.Label();
            this.ACMUsername = new System.Windows.Forms.TextBox();
            this.labelACMUsername = new System.Windows.Forms.Label();
            this.labelACMPassword = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabUpload.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupBoxFiles.SuspendLayout();
            this.groupBoxCablecast.SuspendLayout();
            this.groupBoxACM.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabWelcome);
            this.tabControl1.Controls.Add(this.tabUpload);
            this.tabControl1.Controls.Add(this.tabQueue);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(456, 354);
            this.tabControl1.TabIndex = 0;
            // 
            // tabWelcome
            // 
            this.tabWelcome.Location = new System.Drawing.Point(4, 22);
            this.tabWelcome.Name = "tabWelcome";
            this.tabWelcome.Padding = new System.Windows.Forms.Padding(3);
            this.tabWelcome.Size = new System.Drawing.Size(448, 328);
            this.tabWelcome.TabIndex = 0;
            this.tabWelcome.Text = "Welcome";
            this.tabWelcome.UseVisualStyleBackColor = true;
            // 
            // tabUpload
            // 
            this.tabUpload.Controls.Add(this.button2);
            this.tabUpload.Controls.Add(this.Length);
            this.tabUpload.Controls.Add(this.Cue);
            this.tabUpload.Controls.Add(this.Description);
            this.tabUpload.Controls.Add(this.Producer);
            this.tabUpload.Controls.Add(this.Genre);
            this.tabUpload.Controls.Add(this.Subject);
            this.tabUpload.Controls.Add(this.Title);
            this.tabUpload.Controls.Add(this.Inentifier);
            this.tabUpload.Controls.Add(this.VideoFileButton);
            this.tabUpload.Controls.Add(this.VideoFile);
            this.tabUpload.Controls.Add(this.label8);
            this.tabUpload.Controls.Add(this.label7);
            this.tabUpload.Controls.Add(this.label6);
            this.tabUpload.Controls.Add(this.label5);
            this.tabUpload.Controls.Add(this.label4);
            this.tabUpload.Controls.Add(this.label3);
            this.tabUpload.Controls.Add(this.label2);
            this.tabUpload.Controls.Add(this.label1);
            this.tabUpload.Controls.Add(this.labelVideoFile);
            this.tabUpload.Location = new System.Drawing.Point(4, 22);
            this.tabUpload.Name = "tabUpload";
            this.tabUpload.Padding = new System.Windows.Forms.Padding(3);
            this.tabUpload.Size = new System.Drawing.Size(448, 328);
            this.tabUpload.TabIndex = 1;
            this.tabUpload.Text = "Upload";
            this.tabUpload.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(364, 294);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Queue";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Length
            // 
            this.Length.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Length.Location = new System.Drawing.Point(72, 267);
            this.Length.Name = "Length";
            this.Length.Size = new System.Drawing.Size(112, 20);
            this.Length.TabIndex = 18;
            // 
            // Cue
            // 
            this.Cue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Cue.Location = new System.Drawing.Point(72, 241);
            this.Cue.Name = "Cue";
            this.Cue.Size = new System.Drawing.Size(112, 20);
            this.Cue.TabIndex = 17;
            // 
            // Description
            // 
            this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Description.Location = new System.Drawing.Point(72, 175);
            this.Description.Multiline = true;
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(368, 60);
            this.Description.TabIndex = 16;
            // 
            // Producer
            // 
            this.Producer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Producer.Location = new System.Drawing.Point(72, 146);
            this.Producer.Name = "Producer";
            this.Producer.Size = new System.Drawing.Size(368, 20);
            this.Producer.TabIndex = 15;
            // 
            // Genre
            // 
            this.Genre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Genre.Location = new System.Drawing.Point(72, 120);
            this.Genre.Name = "Genre";
            this.Genre.Size = new System.Drawing.Size(368, 20);
            this.Genre.TabIndex = 14;
            // 
            // Subject
            // 
            this.Subject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Subject.Location = new System.Drawing.Point(72, 94);
            this.Subject.Name = "Subject";
            this.Subject.Size = new System.Drawing.Size(368, 20);
            this.Subject.TabIndex = 13;
            // 
            // Title
            // 
            this.Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Title.Location = new System.Drawing.Point(72, 68);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(368, 20);
            this.Title.TabIndex = 12;
            // 
            // Inentifier
            // 
            this.Inentifier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Inentifier.Location = new System.Drawing.Point(72, 42);
            this.Inentifier.Name = "Inentifier";
            this.Inentifier.Size = new System.Drawing.Size(112, 20);
            this.Inentifier.TabIndex = 11;
            // 
            // VideoFileButton
            // 
            this.VideoFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VideoFileButton.Location = new System.Drawing.Point(365, 14);
            this.VideoFileButton.Name = "VideoFileButton";
            this.VideoFileButton.Size = new System.Drawing.Size(75, 23);
            this.VideoFileButton.TabIndex = 10;
            this.VideoFileButton.Text = "Browse...";
            this.VideoFileButton.UseVisualStyleBackColor = true;
            // 
            // VideoFile
            // 
            this.VideoFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.VideoFile.Location = new System.Drawing.Point(72, 16);
            this.VideoFile.Name = "VideoFile";
            this.VideoFile.Size = new System.Drawing.Size(287, 20);
            this.VideoFile.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 270);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Length:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 244);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Cue:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 175);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Description:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Producer:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Genre:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Subject:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Title:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ShowID:";
            // 
            // labelVideoFile
            // 
            this.labelVideoFile.AutoSize = true;
            this.labelVideoFile.Location = new System.Drawing.Point(8, 19);
            this.labelVideoFile.Name = "labelVideoFile";
            this.labelVideoFile.Size = new System.Drawing.Size(56, 13);
            this.labelVideoFile.TabIndex = 0;
            this.labelVideoFile.Text = "Video File:";
            // 
            // tabQueue
            // 
            this.tabQueue.Location = new System.Drawing.Point(4, 22);
            this.tabQueue.Name = "tabQueue";
            this.tabQueue.Size = new System.Drawing.Size(448, 328);
            this.tabQueue.TabIndex = 2;
            this.tabQueue.Text = "Queue";
            this.tabQueue.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.groupBoxFiles);
            this.tabSettings.Controls.Add(this.groupBoxCablecast);
            this.tabSettings.Controls.Add(this.groupBoxACM);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(448, 328);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxFiles
            // 
            this.groupBoxFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFiles.Controls.Add(this.FileBrowse);
            this.groupBoxFiles.Controls.Add(this.DownloadPath);
            this.groupBoxFiles.Controls.Add(this.labelDownloadFolder);
            this.groupBoxFiles.Location = new System.Drawing.Point(6, 226);
            this.groupBoxFiles.Name = "groupBoxFiles";
            this.groupBoxFiles.Size = new System.Drawing.Size(434, 48);
            this.groupBoxFiles.TabIndex = 8;
            this.groupBoxFiles.TabStop = false;
            this.groupBoxFiles.Text = "Files";
            // 
            // FileBrowse
            // 
            this.FileBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FileBrowse.Location = new System.Drawing.Point(353, 16);
            this.FileBrowse.Name = "FileBrowse";
            this.FileBrowse.Size = new System.Drawing.Size(75, 23);
            this.FileBrowse.TabIndex = 4;
            this.FileBrowse.Text = "Browse...";
            this.FileBrowse.UseVisualStyleBackColor = true;
            this.FileBrowse.Click += new System.EventHandler(this.FileBrowse_Click);
            // 
            // DownloadPath
            // 
            this.DownloadPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DownloadPath.Location = new System.Drawing.Point(105, 19);
            this.DownloadPath.Name = "DownloadPath";
            this.DownloadPath.Size = new System.Drawing.Size(241, 20);
            this.DownloadPath.TabIndex = 3;
            // 
            // labelDownloadFolder
            // 
            this.labelDownloadFolder.AutoSize = true;
            this.labelDownloadFolder.Location = new System.Drawing.Point(9, 22);
            this.labelDownloadFolder.Name = "labelDownloadFolder";
            this.labelDownloadFolder.Size = new System.Drawing.Size(90, 13);
            this.labelDownloadFolder.TabIndex = 0;
            this.labelDownloadFolder.Text = "Download Folder:";
            // 
            // groupBoxCablecast
            // 
            this.groupBoxCablecast.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCablecast.Controls.Add(this.CablecastServer);
            this.groupBoxCablecast.Controls.Add(this.CablecastPassword);
            this.groupBoxCablecast.Controls.Add(this.labelCablecastServer);
            this.groupBoxCablecast.Controls.Add(this.CablecastUsername);
            this.groupBoxCablecast.Controls.Add(this.labelCablecastUsername);
            this.groupBoxCablecast.Controls.Add(this.labelCablecastPassword);
            this.groupBoxCablecast.Location = new System.Drawing.Point(6, 119);
            this.groupBoxCablecast.Name = "groupBoxCablecast";
            this.groupBoxCablecast.Size = new System.Drawing.Size(434, 101);
            this.groupBoxCablecast.TabIndex = 7;
            this.groupBoxCablecast.TabStop = false;
            this.groupBoxCablecast.Text = "Cablecast";
            // 
            // CablecastServer
            // 
            this.CablecastServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CablecastServer.Location = new System.Drawing.Point(105, 19);
            this.CablecastServer.Name = "CablecastServer";
            this.CablecastServer.Size = new System.Drawing.Size(322, 20);
            this.CablecastServer.TabIndex = 3;
            // 
            // CablecastPassword
            // 
            this.CablecastPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CablecastPassword.Location = new System.Drawing.Point(105, 71);
            this.CablecastPassword.Name = "CablecastPassword";
            this.CablecastPassword.Size = new System.Drawing.Size(322, 20);
            this.CablecastPassword.TabIndex = 5;
            // 
            // labelCablecastServer
            // 
            this.labelCablecastServer.AutoSize = true;
            this.labelCablecastServer.Location = new System.Drawing.Point(9, 22);
            this.labelCablecastServer.Name = "labelCablecastServer";
            this.labelCablecastServer.Size = new System.Drawing.Size(82, 13);
            this.labelCablecastServer.TabIndex = 0;
            this.labelCablecastServer.Text = "Server Address:";
            // 
            // CablecastUsername
            // 
            this.CablecastUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CablecastUsername.Location = new System.Drawing.Point(105, 45);
            this.CablecastUsername.Name = "CablecastUsername";
            this.CablecastUsername.Size = new System.Drawing.Size(322, 20);
            this.CablecastUsername.TabIndex = 4;
            // 
            // labelCablecastUsername
            // 
            this.labelCablecastUsername.AutoSize = true;
            this.labelCablecastUsername.Location = new System.Drawing.Point(9, 48);
            this.labelCablecastUsername.Name = "labelCablecastUsername";
            this.labelCablecastUsername.Size = new System.Drawing.Size(58, 13);
            this.labelCablecastUsername.TabIndex = 1;
            this.labelCablecastUsername.Text = "Username:";
            // 
            // labelCablecastPassword
            // 
            this.labelCablecastPassword.AutoSize = true;
            this.labelCablecastPassword.Location = new System.Drawing.Point(9, 74);
            this.labelCablecastPassword.Name = "labelCablecastPassword";
            this.labelCablecastPassword.Size = new System.Drawing.Size(56, 13);
            this.labelCablecastPassword.TabIndex = 2;
            this.labelCablecastPassword.Text = "Password:";
            // 
            // groupBoxACM
            // 
            this.groupBoxACM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxACM.Controls.Add(this.ACMServer);
            this.groupBoxACM.Controls.Add(this.ACMPassword);
            this.groupBoxACM.Controls.Add(this.labelACMServer);
            this.groupBoxACM.Controls.Add(this.ACMUsername);
            this.groupBoxACM.Controls.Add(this.labelACMUsername);
            this.groupBoxACM.Controls.Add(this.labelACMPassword);
            this.groupBoxACM.Location = new System.Drawing.Point(6, 12);
            this.groupBoxACM.Name = "groupBoxACM";
            this.groupBoxACM.Size = new System.Drawing.Size(434, 101);
            this.groupBoxACM.TabIndex = 6;
            this.groupBoxACM.TabStop = false;
            this.groupBoxACM.Text = "ACM SCS";
            // 
            // ACMServer
            // 
            this.ACMServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ACMServer.Location = new System.Drawing.Point(105, 19);
            this.ACMServer.Name = "ACMServer";
            this.ACMServer.Size = new System.Drawing.Size(322, 20);
            this.ACMServer.TabIndex = 3;
            // 
            // ACMPassword
            // 
            this.ACMPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ACMPassword.Location = new System.Drawing.Point(105, 71);
            this.ACMPassword.Name = "ACMPassword";
            this.ACMPassword.PasswordChar = '*';
            this.ACMPassword.Size = new System.Drawing.Size(322, 20);
            this.ACMPassword.TabIndex = 5;
            // 
            // labelACMServer
            // 
            this.labelACMServer.AutoSize = true;
            this.labelACMServer.Location = new System.Drawing.Point(9, 22);
            this.labelACMServer.Name = "labelACMServer";
            this.labelACMServer.Size = new System.Drawing.Size(81, 13);
            this.labelACMServer.TabIndex = 0;
            this.labelACMServer.Text = "Server address:";
            // 
            // ACMUsername
            // 
            this.ACMUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ACMUsername.Location = new System.Drawing.Point(105, 45);
            this.ACMUsername.Name = "ACMUsername";
            this.ACMUsername.Size = new System.Drawing.Size(322, 20);
            this.ACMUsername.TabIndex = 4;
            // 
            // labelACMUsername
            // 
            this.labelACMUsername.AutoSize = true;
            this.labelACMUsername.Location = new System.Drawing.Point(9, 48);
            this.labelACMUsername.Name = "labelACMUsername";
            this.labelACMUsername.Size = new System.Drawing.Size(58, 13);
            this.labelACMUsername.TabIndex = 1;
            this.labelACMUsername.Text = "Username:";
            // 
            // labelACMPassword
            // 
            this.labelACMPassword.AutoSize = true;
            this.labelACMPassword.Location = new System.Drawing.Point(9, 74);
            this.labelACMPassword.Name = "labelACMPassword";
            this.labelACMPassword.Size = new System.Drawing.Size(56, 13);
            this.labelACMPassword.TabIndex = 2;
            this.labelACMPassword.Text = "Password:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 354);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "tracm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabUpload.ResumeLayout(false);
            this.tabUpload.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.groupBoxFiles.ResumeLayout(false);
            this.groupBoxFiles.PerformLayout();
            this.groupBoxCablecast.ResumeLayout(false);
            this.groupBoxCablecast.PerformLayout();
            this.groupBoxACM.ResumeLayout(false);
            this.groupBoxACM.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabWelcome;
        private System.Windows.Forms.TabPage tabUpload;
        private System.Windows.Forms.TabPage tabQueue;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.Label labelACMServer;
        private System.Windows.Forms.GroupBox groupBoxACM;
        private System.Windows.Forms.TextBox ACMPassword;
        private System.Windows.Forms.TextBox ACMUsername;
        private System.Windows.Forms.TextBox ACMServer;
        private System.Windows.Forms.Label labelACMPassword;
        private System.Windows.Forms.Label labelACMUsername;
        private System.Windows.Forms.GroupBox groupBoxCablecast;
        private System.Windows.Forms.TextBox CablecastServer;
        private System.Windows.Forms.TextBox CablecastPassword;
        private System.Windows.Forms.Label labelCablecastServer;
        private System.Windows.Forms.TextBox CablecastUsername;
        private System.Windows.Forms.Label labelCablecastUsername;
        private System.Windows.Forms.Label labelCablecastPassword;
        private System.Windows.Forms.GroupBox groupBoxFiles;
        private System.Windows.Forms.TextBox DownloadPath;
        private System.Windows.Forms.Label labelDownloadFolder;
        private System.Windows.Forms.Button FileBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelVideoFile;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Length;
        private System.Windows.Forms.TextBox Cue;
        private System.Windows.Forms.TextBox Description;
        private System.Windows.Forms.TextBox Producer;
        private System.Windows.Forms.TextBox Genre;
        private System.Windows.Forms.TextBox Subject;
        private System.Windows.Forms.TextBox Title;
        private System.Windows.Forms.TextBox Inentifier;
        private System.Windows.Forms.Button VideoFileButton;
        private System.Windows.Forms.TextBox VideoFile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;

    }
}

