using System;
using System.Windows.Forms;

namespace FirefoxBet365Placer
{
    public partial class frmGeneralSetting : Form
    {
        public string fileName = string.Empty;
        OpenFileDialog fileDlg = new OpenFileDialog();
        public frmGeneralSetting()
        {
            InitializeComponent();
        }

        private void initControls()
        {
        }

        private void initValues()
        {
            // General Setting

            txtCountry.Text = Setting.instance.countryCode;
            txtServerURL.Text = Setting.instance.serverAddr;
            txtUsername.Text = Setting.instance.betUsername;
            txtPassword.Text = Setting.instance.betPassword;
            txtLicense.Text = Setting.instance.license;

            chkKeepSession.Checked = Setting.instance.isKeepSessionAlive;
            chkFakeBet.Checked = Setting.instance.isMakeFakeBet;
            chkComplex.Checked = Setting.instance.isComplex;
            chkWorkingAtDayTime.Checked = Setting.instance.isWorkingAtDayTime;
            numStake.Value  = (decimal)Setting.instance.numStake;

            // BookieBashing
            numDelayAfterLogin.Value = (decimal)Setting.instance.numFlatStake;
            numDelayBetweenStartandLoad.Value = (decimal)Setting.instance.numBetCount;
            numDelayLoadandLogin.Value = (decimal)Setting.instance.numOddsMin;
            
            chkRecordResult.Checked = Setting.instance.isRecordResult;
            chkUseUILogin.Checked = Setting.instance.isUseUILogin;

            chkDouble.Checked = Setting.instance.isDouble;
            chkChrome.Checked = Setting.instance.isEnabledChrome;
            chkFirefox.Checked = Setting.instance.isEnabledFirefox;
            chkEdge.Checked = Setting.instance.isEnabledEdge;
            txtChromePath.Text = Setting.instance.chromePath;
            txtFirefoxPath.Text = Setting.instance.firefoxPath;
            txtEdgePath.Text = Setting.instance.edgePath;

            numHeightDiff.Value = (decimal)Setting.instance.heightDiff;
            txtAnydesk.Text = Setting.instance.anydesk;
            txtOwner.Text = Setting.instance.owner;

            numDelayBetweenStartandLoad.Value = (decimal)Setting.instance.delayStart_Load;
            numDelayLoadandLogin.Value = (decimal)Setting.instance.delayLoad_Login;
            numDelayAfterLogin.Value = (decimal)Setting.instance.delayAfterLogin ;
            numDelayAfterRefresh.Value = (decimal)Setting.instance.delayAfterRefresh;
            numDelayBetweenRetries.Value = (decimal)Setting.instance.delayBetweenRetries;
            numDelayBetweenBets.Value = (decimal)Setting.instance.delayBetweenBets;

            dmChooseMode.SelectedIndex = (int)Setting.instance.placingMode;

            timeStart.Text = Setting.instance.timeStart;
            timeStop.Text  = Setting.instance.timeStop;
        }

        private bool canSet()
        {
            if (string.IsNullOrEmpty(txtCountry.Text))
            {
                txtCountry.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtServerURL.Text))
            {
                txtCountry.Focus();
                return false;
            }

            return true;
        }
        private void setValues()
        {
            // General Setting
            Setting.instance.countryCode = txtCountry.Text;
            Setting.instance.serverAddr = txtServerURL.Text;
            Setting.instance.betUsername = txtUsername.Text;
            Setting.instance.betPassword = txtPassword.Text;
            Setting.instance.license = txtLicense.Text;

            Setting.instance.isKeepSessionAlive = chkKeepSession.Checked;
            Setting.instance.isMakeFakeBet = chkFakeBet.Checked;
            Setting.instance.isComplex = chkComplex.Checked;
            Setting.instance.isDouble = chkDouble.Checked;
            Setting.instance.isWorkingAtDayTime = chkWorkingAtDayTime.Checked;

            Setting.instance.numStake = (double)numStake.Value;
            Setting.instance.anydesk = txtAnydesk.Text;
            Setting.instance.owner = txtOwner.Text;
            // BookieBashing
            Setting.instance.numFlatStake = (double)numDelayAfterLogin.Value;
            Setting.instance.numBetCount = (double)numDelayBetweenStartandLoad.Value;
            Setting.instance.numOddsMin = (double)numDelayLoadandLogin.Value;
            Setting.instance.isRecordResult = chkRecordResult.Checked;

            Setting.instance.isEnabledChrome = chkChrome.Checked;
            Setting.instance.isEnabledFirefox = chkFirefox.Checked;
            Setting.instance.isEnabledEdge = chkEdge.Checked;

            Setting.instance.chromePath = txtChromePath.Text;
            Setting.instance.firefoxPath = txtFirefoxPath.Text;
            Setting.instance.edgePath = txtEdgePath.Text;


            Setting.instance.heightDiff = (double)numHeightDiff.Value;

            // Delay
            Setting.instance.isUseUILogin = chkUseUILogin.Checked;

            Setting.instance.delayStart_Load     = (double)numDelayBetweenStartandLoad.Value;
            Setting.instance.delayLoad_Login     = (double)numDelayLoadandLogin.Value;
            Setting.instance.delayAfterLogin     = (double)numDelayAfterLogin.Value;
            Setting.instance.delayAfterRefresh   = (double)numDelayAfterRefresh.Value;
            Setting.instance.delayBetweenRetries = (double)numDelayBetweenRetries.Value;
            Setting.instance.delayBetweenBets = (double)numDelayBetweenBets.Value;

            Setting.instance.placingMode = (PLACING_MODE)dmChooseMode.SelectedIndex;

            Setting.instance.timeStart =  timeStart.Text;
            Setting.instance.timeStop = timeStop.Text;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!canSet())
                return;

            setValues();
            Setting.instance.saveSettingInfo();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            initControls();
            initValues();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!canSet())
                return;
            setValues();
            Setting.instance.saveSettingInfo();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelectChrome_Click(object sender, EventArgs e)
        {
            if (fileDlg.ShowDialog() == DialogResult.OK) 
            {
                txtChromePath.Text = fileDlg.FileName;
            }
            
        }

        private void btnSelectFirefox_Click(object sender, EventArgs e)
        {
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                txtFirefoxPath.Text = fileDlg.FileName;
            }
        }

        private void btnSelectEdge_Click(object sender, EventArgs e)
        {
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                txtEdgePath.Text = fileDlg.FileName;
            }
        }
    }
}
