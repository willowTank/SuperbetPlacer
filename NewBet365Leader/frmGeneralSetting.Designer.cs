namespace FirefoxBet365Placer
{
    partial class frmGeneralSetting
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.numStake = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.dmChooseMode = new System.Windows.Forms.DomainUpDown();
            this.txtOwner = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numHeightDiff = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAnydesk = new System.Windows.Forms.TextBox();
            this.chkWorkingAtDayTime = new System.Windows.Forms.CheckBox();
            this.chkRecordResult = new System.Windows.Forms.CheckBox();
            this.chkComplex = new System.Windows.Forms.CheckBox();
            this.chkFakeBet = new System.Windows.Forms.CheckBox();
            this.chkKeepSession = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLicense = new System.Windows.Forms.TextBox();
            this.txtCountry = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServerURL = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.timeStop = new System.Windows.Forms.DateTimePicker();
            this.timeStart = new System.Windows.Forms.DateTimePicker();
            this.chkUseUILogin = new System.Windows.Forms.CheckBox();
            this.numDelayBetweenBets = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numDelayBetweenRetries = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numDelayAfterRefresh = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.numDelayAfterLogin = new System.Windows.Forms.NumericUpDown();
            this.numDelayLoadandLogin = new System.Windows.Forms.NumericUpDown();
            this.numDelayBetweenStartandLoad = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkEdge = new System.Windows.Forms.CheckBox();
            this.btnSelectEdge = new System.Windows.Forms.Button();
            this.txtEdgePath = new System.Windows.Forms.TextBox();
            this.chkFirefox = new System.Windows.Forms.CheckBox();
            this.chkChrome = new System.Windows.Forms.CheckBox();
            this.btnSelectFirefox = new System.Windows.Forms.Button();
            this.txtFirefoxPath = new System.Windows.Forms.TextBox();
            this.btnSelectChrome = new System.Windows.Forms.Button();
            this.txtChromePath = new System.Windows.Forms.TextBox();
            this.chkDouble = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStake)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeightDiff)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayBetweenBets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayBetweenRetries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayAfterRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayAfterLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayLoadandLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayBetweenStartandLoad)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(144, 427);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(258, 427);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(8, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(462, 409);
            this.tabControl1.TabIndex = 62;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chkDouble);
            this.tabPage1.Controls.Add(this.numStake);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.dmChooseMode);
            this.tabPage1.Controls.Add(this.txtOwner);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.numHeightDiff);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.txtAnydesk);
            this.tabPage1.Controls.Add(this.chkWorkingAtDayTime);
            this.tabPage1.Controls.Add(this.chkRecordResult);
            this.tabPage1.Controls.Add(this.chkComplex);
            this.tabPage1.Controls.Add(this.chkFakeBet);
            this.tabPage1.Controls.Add(this.chkKeepSession);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.txtLicense);
            this.tabPage1.Controls.Add(this.txtCountry);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtServerURL);
            this.tabPage1.Controls.Add(this.txtPassword);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtUsername);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(454, 383);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // numStake
            // 
            this.numStake.Location = new System.Drawing.Point(111, 240);
            this.numStake.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numStake.Name = "numStake";
            this.numStake.Size = new System.Drawing.Size(98, 20);
            this.numStake.TabIndex = 86;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 247);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 85;
            this.label4.Text = "Stake:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // dmChooseMode
            // 
            this.dmChooseMode.Items.Add("Normal Mode");
            this.dmChooseMode.Items.Add("Fast Mode");
            this.dmChooseMode.Items.Add("Search Mode");
            this.dmChooseMode.Location = new System.Drawing.Point(26, 318);
            this.dmChooseMode.Name = "dmChooseMode";
            this.dmChooseMode.Size = new System.Drawing.Size(117, 20);
            this.dmChooseMode.TabIndex = 84;
            this.dmChooseMode.Text = "Choose Mode";
            // 
            // txtOwner
            // 
            this.txtOwner.Location = new System.Drawing.Point(238, 59);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(127, 20);
            this.txtOwner.TabIndex = 80;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(192, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 81;
            this.label7.Text = "Owner:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // numHeightDiff
            // 
            this.numHeightDiff.Location = new System.Drawing.Point(111, 171);
            this.numHeightDiff.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numHeightDiff.Name = "numHeightDiff";
            this.numHeightDiff.Size = new System.Drawing.Size(98, 20);
            this.numHeightDiff.TabIndex = 79;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 178);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(38, 13);
            this.label14.TabIndex = 78;
            this.label14.Text = "H. diff:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 210);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(51, 13);
            this.label12.TabIndex = 77;
            this.label12.Text = "Anydesk:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtAnydesk
            // 
            this.txtAnydesk.Location = new System.Drawing.Point(111, 203);
            this.txtAnydesk.Name = "txtAnydesk";
            this.txtAnydesk.Size = new System.Drawing.Size(254, 20);
            this.txtAnydesk.TabIndex = 76;
            // 
            // chkWorkingAtDayTime
            // 
            this.chkWorkingAtDayTime.AutoSize = true;
            this.chkWorkingAtDayTime.Location = new System.Drawing.Point(325, 319);
            this.chkWorkingAtDayTime.Name = "chkWorkingAtDayTime";
            this.chkWorkingAtDayTime.Size = new System.Drawing.Size(103, 17);
            this.chkWorkingAtDayTime.TabIndex = 67;
            this.chkWorkingAtDayTime.Text = "Work at daytime";
            this.chkWorkingAtDayTime.UseVisualStyleBackColor = true;
            // 
            // chkRecordResult
            // 
            this.chkRecordResult.AutoSize = true;
            this.chkRecordResult.Location = new System.Drawing.Point(324, 291);
            this.chkRecordResult.Name = "chkRecordResult";
            this.chkRecordResult.Size = new System.Drawing.Size(94, 17);
            this.chkRecordResult.TabIndex = 66;
            this.chkRecordResult.Text = "Record Result";
            this.chkRecordResult.UseVisualStyleBackColor = true;
            // 
            // chkComplex
            // 
            this.chkComplex.AutoSize = true;
            this.chkComplex.Location = new System.Drawing.Point(239, 291);
            this.chkComplex.Name = "chkComplex";
            this.chkComplex.Size = new System.Drawing.Size(79, 17);
            this.chkComplex.TabIndex = 65;
            this.chkComplex.Text = "Keep log in";
            this.chkComplex.UseVisualStyleBackColor = true;
            // 
            // chkFakeBet
            // 
            this.chkFakeBet.AutoSize = true;
            this.chkFakeBet.Location = new System.Drawing.Point(143, 291);
            this.chkFakeBet.Name = "chkFakeBet";
            this.chkFakeBet.Size = new System.Drawing.Size(69, 17);
            this.chkFakeBet.TabIndex = 64;
            this.chkFakeBet.Text = "Fake Bet";
            this.chkFakeBet.UseVisualStyleBackColor = true;
            // 
            // chkKeepSession
            // 
            this.chkKeepSession.AutoSize = true;
            this.chkKeepSession.Location = new System.Drawing.Point(26, 291);
            this.chkKeepSession.Name = "chkKeepSession";
            this.chkKeepSession.Size = new System.Drawing.Size(117, 17);
            this.chkKeepSession.TabIndex = 61;
            this.chkKeepSession.Text = "Keep Session Alive";
            this.chkKeepSession.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 60;
            this.label3.Text = "License:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Server URL:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtLicense
            // 
            this.txtLicense.Location = new System.Drawing.Point(111, 97);
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.Size = new System.Drawing.Size(254, 20);
            this.txtLicense.TabIndex = 59;
            // 
            // txtCountry
            // 
            this.txtCountry.Location = new System.Drawing.Point(111, 59);
            this.txtCountry.Name = "txtCountry";
            this.txtCountry.Size = new System.Drawing.Size(59, 20);
            this.txtCountry.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Country:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtServerURL
            // 
            this.txtServerURL.Location = new System.Drawing.Point(111, 21);
            this.txtServerURL.Name = "txtServerURL";
            this.txtServerURL.Size = new System.Drawing.Size(254, 20);
            this.txtServerURL.TabIndex = 31;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(238, 137);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(127, 20);
            this.txtPassword.TabIndex = 40;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Account:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(111, 137);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(118, 20);
            this.txtUsername.TabIndex = 39;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.timeStop);
            this.tabPage2.Controls.Add(this.timeStart);
            this.tabPage2.Controls.Add(this.chkUseUILogin);
            this.tabPage2.Controls.Add(this.numDelayBetweenBets);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.numDelayBetweenRetries);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.numDelayAfterRefresh);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.numDelayAfterLogin);
            this.tabPage2.Controls.Add(this.numDelayLoadandLogin);
            this.tabPage2.Controls.Add(this.numDelayBetweenStartandLoad);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(454, 383);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Filter";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(224, 40);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(20, 13);
            this.label17.TabIndex = 89;
            this.label17.Text = "To";
            this.label17.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(36, 40);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 88;
            this.label16.Text = "From";
            this.label16.Visible = false;
            // 
            // timeStop
            // 
            this.timeStop.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeStop.Location = new System.Drawing.Point(260, 34);
            this.timeStop.Name = "timeStop";
            this.timeStop.ShowUpDown = true;
            this.timeStop.Size = new System.Drawing.Size(129, 20);
            this.timeStop.TabIndex = 87;
            // 
            // timeStart
            // 
            this.timeStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeStart.Location = new System.Drawing.Point(72, 34);
            this.timeStart.Name = "timeStart";
            this.timeStart.ShowUpDown = true;
            this.timeStart.Size = new System.Drawing.Size(129, 20);
            this.timeStart.TabIndex = 86;
            // 
            // chkUseUILogin
            // 
            this.chkUseUILogin.AutoSize = true;
            this.chkUseUILogin.Location = new System.Drawing.Point(39, 391);
            this.chkUseUILogin.Name = "chkUseUILogin";
            this.chkUseUILogin.Size = new System.Drawing.Size(88, 17);
            this.chkUseUILogin.TabIndex = 85;
            this.chkUseUILogin.Text = "Use UI Login";
            this.chkUseUILogin.UseVisualStyleBackColor = true;
            this.chkUseUILogin.Visible = false;
            // 
            // numDelayBetweenBets
            // 
            this.numDelayBetweenBets.DecimalPlaces = 1;
            this.numDelayBetweenBets.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDelayBetweenBets.Location = new System.Drawing.Point(291, 345);
            this.numDelayBetweenBets.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numDelayBetweenBets.Name = "numDelayBetweenBets";
            this.numDelayBetweenBets.Size = new System.Drawing.Size(98, 20);
            this.numDelayBetweenBets.TabIndex = 84;
            this.numDelayBetweenBets.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numDelayBetweenBets.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(36, 347);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 13);
            this.label10.TabIndex = 83;
            this.label10.Text = "Delay between bets:";
            this.label10.Visible = false;
            // 
            // numDelayBetweenRetries
            // 
            this.numDelayBetweenRetries.DecimalPlaces = 1;
            this.numDelayBetweenRetries.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDelayBetweenRetries.Location = new System.Drawing.Point(291, 303);
            this.numDelayBetweenRetries.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numDelayBetweenRetries.Name = "numDelayBetweenRetries";
            this.numDelayBetweenRetries.Size = new System.Drawing.Size(98, 20);
            this.numDelayBetweenRetries.TabIndex = 82;
            this.numDelayBetweenRetries.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numDelayBetweenRetries.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(36, 305);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(115, 13);
            this.label9.TabIndex = 81;
            this.label9.Text = "Delay between retries :";
            this.label9.Visible = false;
            // 
            // numDelayAfterRefresh
            // 
            this.numDelayAfterRefresh.DecimalPlaces = 1;
            this.numDelayAfterRefresh.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDelayAfterRefresh.Location = new System.Drawing.Point(291, 260);
            this.numDelayAfterRefresh.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numDelayAfterRefresh.Name = "numDelayAfterRefresh";
            this.numDelayAfterRefresh.Size = new System.Drawing.Size(98, 20);
            this.numDelayAfterRefresh.TabIndex = 80;
            this.numDelayAfterRefresh.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numDelayAfterRefresh.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(36, 262);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(174, 13);
            this.label8.TabIndex = 79;
            this.label8.Text = "Delay before addbet after referesh :";
            this.label8.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 223);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 13);
            this.label6.TabIndex = 78;
            this.label6.Text = "Delay after successful login :";
            this.label6.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(36, 140);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(167, 13);
            this.label13.TabIndex = 66;
            this.label13.Text = "Between start and load 365 page:";
            this.label13.Visible = false;
            // 
            // numDelayAfterLogin
            // 
            this.numDelayAfterLogin.DecimalPlaces = 1;
            this.numDelayAfterLogin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDelayAfterLogin.Location = new System.Drawing.Point(291, 216);
            this.numDelayAfterLogin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numDelayAfterLogin.Name = "numDelayAfterLogin";
            this.numDelayAfterLogin.Size = new System.Drawing.Size(98, 20);
            this.numDelayAfterLogin.TabIndex = 74;
            this.numDelayAfterLogin.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numDelayAfterLogin.Visible = false;
            // 
            // numDelayLoadandLogin
            // 
            this.numDelayLoadandLogin.DecimalPlaces = 2;
            this.numDelayLoadandLogin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numDelayLoadandLogin.Location = new System.Drawing.Point(291, 173);
            this.numDelayLoadandLogin.Name = "numDelayLoadandLogin";
            this.numDelayLoadandLogin.Size = new System.Drawing.Size(98, 20);
            this.numDelayLoadandLogin.TabIndex = 71;
            this.numDelayLoadandLogin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDelayLoadandLogin.Visible = false;
            // 
            // numDelayBetweenStartandLoad
            // 
            this.numDelayBetweenStartandLoad.DecimalPlaces = 2;
            this.numDelayBetweenStartandLoad.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numDelayBetweenStartandLoad.Location = new System.Drawing.Point(291, 133);
            this.numDelayBetweenStartandLoad.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDelayBetweenStartandLoad.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numDelayBetweenStartandLoad.Name = "numDelayBetweenStartandLoad";
            this.numDelayBetweenStartandLoad.Size = new System.Drawing.Size(98, 20);
            this.numDelayBetweenStartandLoad.TabIndex = 67;
            this.numDelayBetweenStartandLoad.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(36, 180);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(174, 13);
            this.label15.TabIndex = 70;
            this.label15.Text = "Between page load and try to login:";
            this.label15.Visible = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkEdge);
            this.tabPage3.Controls.Add(this.btnSelectEdge);
            this.tabPage3.Controls.Add(this.txtEdgePath);
            this.tabPage3.Controls.Add(this.chkFirefox);
            this.tabPage3.Controls.Add(this.chkChrome);
            this.tabPage3.Controls.Add(this.btnSelectFirefox);
            this.tabPage3.Controls.Add(this.txtFirefoxPath);
            this.tabPage3.Controls.Add(this.btnSelectChrome);
            this.tabPage3.Controls.Add(this.txtChromePath);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(454, 383);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Browser";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkEdge
            // 
            this.chkEdge.AutoSize = true;
            this.chkEdge.Location = new System.Drawing.Point(6, 129);
            this.chkEdge.Name = "chkEdge";
            this.chkEdge.Size = new System.Drawing.Size(51, 17);
            this.chkEdge.TabIndex = 10;
            this.chkEdge.Text = "Edge";
            this.chkEdge.UseVisualStyleBackColor = true;
            // 
            // btnSelectEdge
            // 
            this.btnSelectEdge.Location = new System.Drawing.Point(403, 123);
            this.btnSelectEdge.Name = "btnSelectEdge";
            this.btnSelectEdge.Size = new System.Drawing.Size(29, 23);
            this.btnSelectEdge.TabIndex = 9;
            this.btnSelectEdge.Text = "...";
            this.btnSelectEdge.UseVisualStyleBackColor = true;
            this.btnSelectEdge.Click += new System.EventHandler(this.btnSelectEdge_Click);
            // 
            // txtEdgePath
            // 
            this.txtEdgePath.Location = new System.Drawing.Point(82, 125);
            this.txtEdgePath.Name = "txtEdgePath";
            this.txtEdgePath.Size = new System.Drawing.Size(319, 20);
            this.txtEdgePath.TabIndex = 8;
            // 
            // chkFirefox
            // 
            this.chkFirefox.AutoSize = true;
            this.chkFirefox.Location = new System.Drawing.Point(6, 87);
            this.chkFirefox.Name = "chkFirefox";
            this.chkFirefox.Size = new System.Drawing.Size(57, 17);
            this.chkFirefox.TabIndex = 7;
            this.chkFirefox.Text = "Firefox";
            this.chkFirefox.UseVisualStyleBackColor = true;
            // 
            // chkChrome
            // 
            this.chkChrome.AutoSize = true;
            this.chkChrome.Location = new System.Drawing.Point(6, 43);
            this.chkChrome.Name = "chkChrome";
            this.chkChrome.Size = new System.Drawing.Size(62, 17);
            this.chkChrome.TabIndex = 6;
            this.chkChrome.Text = "Chrome";
            this.chkChrome.UseVisualStyleBackColor = true;
            // 
            // btnSelectFirefox
            // 
            this.btnSelectFirefox.Location = new System.Drawing.Point(403, 81);
            this.btnSelectFirefox.Name = "btnSelectFirefox";
            this.btnSelectFirefox.Size = new System.Drawing.Size(29, 23);
            this.btnSelectFirefox.TabIndex = 5;
            this.btnSelectFirefox.Text = "...";
            this.btnSelectFirefox.UseVisualStyleBackColor = true;
            this.btnSelectFirefox.Click += new System.EventHandler(this.btnSelectFirefox_Click);
            // 
            // txtFirefoxPath
            // 
            this.txtFirefoxPath.Location = new System.Drawing.Point(82, 83);
            this.txtFirefoxPath.Name = "txtFirefoxPath";
            this.txtFirefoxPath.Size = new System.Drawing.Size(319, 20);
            this.txtFirefoxPath.TabIndex = 4;
            // 
            // btnSelectChrome
            // 
            this.btnSelectChrome.Location = new System.Drawing.Point(403, 38);
            this.btnSelectChrome.Name = "btnSelectChrome";
            this.btnSelectChrome.Size = new System.Drawing.Size(29, 23);
            this.btnSelectChrome.TabIndex = 2;
            this.btnSelectChrome.Text = "...";
            this.btnSelectChrome.UseVisualStyleBackColor = true;
            this.btnSelectChrome.Click += new System.EventHandler(this.btnSelectChrome_Click);
            // 
            // txtChromePath
            // 
            this.txtChromePath.Location = new System.Drawing.Point(82, 40);
            this.txtChromePath.Name = "txtChromePath";
            this.txtChromePath.Size = new System.Drawing.Size(319, 20);
            this.txtChromePath.TabIndex = 1;
            // 
            // chkDouble
            // 
            this.chkDouble.AutoSize = true;
            this.chkDouble.Location = new System.Drawing.Point(238, 319);
            this.chkDouble.Name = "chkDouble";
            this.chkDouble.Size = new System.Drawing.Size(60, 17);
            this.chkDouble.TabIndex = 87;
            this.chkDouble.Text = "Double";
            this.chkDouble.UseVisualStyleBackColor = true;
            // 
            // frmGeneralSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(479, 460);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmGeneralSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Setting";
            this.Load += new System.EventHandler(this.frmSetting_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStake)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeightDiff)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayBetweenBets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayBetweenRetries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayAfterRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayAfterLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayLoadandLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayBetweenStartandLoad)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox chkKeepSession;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLicense;
        private System.Windows.Forms.TextBox txtCountry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServerURL;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.CheckBox chkFakeBet;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox chkComplex;
        private System.Windows.Forms.CheckBox chkRecordResult;
        private System.Windows.Forms.CheckBox chkWorkingAtDayTime;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox chkFirefox;
        private System.Windows.Forms.CheckBox chkChrome;
        private System.Windows.Forms.Button btnSelectFirefox;
        private System.Windows.Forms.TextBox txtFirefoxPath;
        private System.Windows.Forms.Button btnSelectChrome;
        private System.Windows.Forms.TextBox txtChromePath;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtAnydesk;
        private System.Windows.Forms.NumericUpDown numHeightDiff;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtOwner;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkEdge;
        private System.Windows.Forms.Button btnSelectEdge;
        private System.Windows.Forms.TextBox txtEdgePath;
        private System.Windows.Forms.DomainUpDown dmChooseMode;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DateTimePicker timeStop;
        private System.Windows.Forms.DateTimePicker timeStart;
        private System.Windows.Forms.CheckBox chkUseUILogin;
        private System.Windows.Forms.NumericUpDown numDelayBetweenBets;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numDelayBetweenRetries;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numDelayAfterRefresh;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numDelayAfterLogin;
        private System.Windows.Forms.NumericUpDown numDelayLoadandLogin;
        private System.Windows.Forms.NumericUpDown numDelayBetweenStartandLoad;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numStake;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDouble;
    }
}