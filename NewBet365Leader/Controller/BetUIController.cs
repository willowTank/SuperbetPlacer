using FirefoxBet365Placer.Constants;
using FirefoxBet365Placer.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Text;
using WindowsInput;
using WindowsInput.Native;

namespace FirefoxBet365Placer.Controller
{
    public class BetUIController : BetController
    {
        private decimal _X_stake = 0;
        private decimal _Y_stake = 0;
        private JObject betPosition;
        private BetItem _prevItem = new BetItem();
        public string QRGetRefreshToken = "";
        public DateTime timeToSendQRGetRequest = DateTime.MaxValue;

        public BetUIController(onWriteStatusEvent _handlerWriteStatus, onWriteLogEvent _handlerWriteLog, SocketConnector _socketConnector) :
            base(_handlerWriteStatus, _handlerWriteLog, _socketConnector)
        {

        }

        public bool DoSearchEventPage(BetItem betitem)
        {
            m_handlerWriteStatus("DoSearchEventPage");
            try
            {
                string result = string.Empty;
                decimal x, y;
                dynamic betPosition;
                string isAlreadySearchPage = ExecuteScript("location.href.includes('#/AX')?'true':'false'", true);
                if(isAlreadySearchPage == "false")
                {
                    result = ExecuteScript("findSearchButton()", true);
                    m_handlerWriteStatus($"findSearchButton() => {result}");
                    if (result.ToLower() == "false" || result.ToLower() == "")
                    {
                        NavigateInvoke(Setting.instance.bet365Domain + "/#/AX/");
                    }
                    else
                    {
                        betPosition = JsonConvert.DeserializeObject<JObject>(result);
                        x = decimal.Parse(betPosition.SelectToken("x").ToString());
                        y = decimal.Parse(betPosition.SelectToken("y").ToString());
                        if (x == 0 && y == 0)
                        {
                            NavigateInvoke(Setting.instance.bet365Domain + "/#/AX/");
                        }
                        FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                        FakeUserAction.Intance.SimClick();
                    }
                    Thread.Sleep(300);
                }
                else
                {
                    FakeUserAction.Intance.SimMouseMoveTo(new Point(390 + Utils.GetRandValue(15), 150 + Utils.GetRandValue(5)));
                    FakeUserAction.Intance.SimClick();
                    Thread.Sleep(500);
                }

                result = ExecuteScript("findSearchCloseButton()", true);
                m_handlerWriteStatus($"findSearchCloseButton() => {result}");
                if (result.ToLower() != "false" && result.ToLower() != "")
                {
                    betPosition = JsonConvert.DeserializeObject<JObject>(result);
                    x = decimal.Parse(betPosition.SelectToken("x").ToString());
                    y = decimal.Parse(betPosition.SelectToken("y").ToString());
                    if (x != 0 || y != 0)
                    {
                        FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                        FakeUserAction.Intance.SimClick();
                        Thread.Sleep(500);
                        FakeUserAction.Intance.SimMouseMoveTo(new Point(x - 200 - Utils.GetRandValue(15), y));
                        FakeUserAction.Intance.SimClick();
                    }
                }

                m_handlerWriteStatus(betitem.match);
                InputSimulator inputSim = new InputSimulator();
                inputSim.Keyboard.TextEntry(betitem.match);
                
                Thread.Sleep(400);
                if (betitem.source == SOURCE.BASHING)
                {
                    result = ExecuteScript("findHorseWithName()", true);
                    m_handlerWriteStatus($"findHorseWithName() => {result}");
                    int retryCount = 10;
                    while ((result.ToLower() == "false" || result.ToLower() == "") && --retryCount > 0)
                    {
                        Thread.Sleep(100);
                        result = ExecuteScript("findHorseWithName()", true);
                    }

                    if (result.ToLower() == "false" || result.ToLower() == "")
                    {
                        return false;
                    }
                }
                else 
                {
                    result = ExecuteScript("findEventWithName()", true);
                    m_handlerWriteStatus($"findEventWithName() => {result}");
                    int retryCount = 10;
                    while ((result.ToLower() == "false" || result.ToLower() == "") && --retryCount > 0)
                    {
                        Thread.Sleep(100);
                        result = ExecuteScript("findEventWithName()", true);
                    }

                    if (result.ToLower() == "false" || result.ToLower() == "")
                    {
                        return false;
                    }
                }
                

                betPosition = JsonConvert.DeserializeObject<JObject>(result);
                betPosition = betPosition.position;
                x = decimal.Parse(betPosition.SelectToken("x").ToString());
                y = decimal.Parse(betPosition.SelectToken("y").ToString());
                if (x == 0 && y == 0)
                {
                    m_handlerWriteStatus("Wrong position detected");
                    return false;
                }

                FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                FakeUserAction.Intance.SimClick();
                return true;
            }
            catch(Exception ex) 
            {
                m_handlerWriteStatus("Exception in DoSearchEventPage : " + ex.ToString());
            }
            return false;
        }

        public bool DoVisitEventPage(BetItem betitem)
        {
            bool bSearch = false;
            string visitUrl = Setting.instance.bet365Domain + (string.IsNullOrEmpty(betitem.PD) ? "" : betitem.PD);
            if (betitem.PD.Contains("EV") && Setting.instance.placingMode != PLACING_MODE.SEARCH)
            {
                visitUrl = Setting.instance.bet365Domain + (betitem.PD.Contains("IP")? betitem.PD: "/#/IP/" + betitem.PD);
                bSearch = true;
                NavigateInvoke(visitUrl);
                return bSearch;
            }
            if(betitem.tipster == "betspan")
            {
                visitUrl = Setting.instance.bet365Domain + betitem.PD1;
            }
            visitUrl = visitUrl.Replace("///", "/");
            visitUrl = visitUrl.Split(new string[] { "/P" }, StringSplitOptions.None)[0];
            visitUrl = visitUrl.Replace("|", "");
            m_handlerWriteStatus(string.Format("DoVisitEventPage: {0}", visitUrl));

            if (betitem.source == SOURCE.DOGWIN)
            {
                if (betitem.tipster == "dog")
                {
                    DoVisitOtherPage(3);
                }
                else
                {
                    DoVisitOtherPage(2);
                }
                bSearch = true;
            }
            else if (betitem.source == SOURCE.DOG_DOG)
            {
                visitUrl = Setting.instance.bet365Domain + "/#/IP/EV" + betitem.PD;
                NavigateInvoke(visitUrl);
                bSearch = true;
            }
            else if (betitem.source == SOURCE.TRADEMATE)
            {
                bSearch = true;
                NavigateInvoke(visitUrl);
            }
            else if (betitem.source == SOURCE.DOMBETTING)
            {
                visitUrl = Setting.instance.bet365Domain + "/#/IP/B1";
                NavigateInvoke(visitUrl);
            }
            else if (betitem.source == SOURCE.BASHING)
            {
                if (Setting.instance.placingMode == PLACING_MODE.SEARCH || Setting.instance.bet365Domain.Contains("es"))
                {
                    bSearch = DoSearchEventPage(betitem);
                    if (!bSearch)
                    {
                    }
                }
                else
                {
                    NavigateInvoke(betitem.bs);
                }

            }
            else if (betitem.source == SOURCE.TIPSTER)
            {
                if(betitem.isLive == true)
                {
                    string currentMatchTitle = ExecuteScript("getMatchOfCurPage()", true);
                    if (currentMatchTitle != betitem.match)
                    {
                        if (Setting.instance.placingMode == PLACING_MODE.SEARCH)
                        {
                            bSearch = DoSearchEventPage(betitem);
                        }
                        else if (Setting.instance.placingMode == PLACING_MODE.FAST)
                        {
                            NavigateInvoke(visitUrl);
                        }
                        else
                        {
                            ExecuteScript($"location.href.includes('{"/IP"}')?'':location.href='https://www.{visitUrl}'", true);
                            m_handlerWriteStatus($"location.href.includes('{"/IP"}')?'':location.href='https://www.{visitUrl}'");
                            visitUrl = string.Format("{0}{1}", Setting.instance.bet365Domain, betitem.PD.Replace("|", ""));

                            string result = ExecuteScript($"findEventItemByText('{betitem.match}')", true);
                            m_handlerWriteStatus($"findEventItemByText('{betitem.match}')=> {result}");
                            if (result.ToLower() == "false" || result.ToLower() == "")
                            {
                                NavigateInvoke(visitUrl);
                            }
                            else
                            {
                                betPosition = JsonConvert.DeserializeObject<JObject>(result);
                                decimal x = decimal.Parse(betPosition.SelectToken("x").ToString());
                                decimal y = decimal.Parse(betPosition.SelectToken("y").ToString());
                                if (x == 0 && y == 0)
                                {
                                    m_handlerWriteStatus("Wrong position detected");
                                    NavigateInvoke(visitUrl);
                                }
                                else
                                {
                                    FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                                    FakeUserAction.Intance.SimClick();
                                    Thread.Sleep(200);
                                    ExecuteScript($"location.href.includes('{betitem.PD.Replace("|", "")}')?'':location.href='https://www.{visitUrl}'", true);
                                    bSearch = true;
                                }
                            }
                        }
                    }
                }
                else if (Setting.instance.placingMode == PLACING_MODE.SEARCH)
                {
                    bSearch = DoSearchEventPage(betitem);
                }
                else if(string.IsNullOrEmpty(betitem.PD) == false)
                {
                    NavigateInvoke(visitUrl);
                    bSearch = true;
                }
            }
            else if (Setting.instance.placingMode == PLACING_MODE.SEARCH)
            {
                bSearch = DoSearchEventPage(betitem);
            }
            else
            {
                NavigateInvoke(visitUrl);
            }

            if (betitem.source == SOURCE.BFLIVE)
            {
                bSearch = true;
            }
            return bSearch;
        }

        public bool MakeBet(List<BetItem> betitems)
        {
            bool bRet = false;
            try
            {
                Removebet();
                dynamic processResponse = new JObject();
                processResponse.username = Setting.instance.betUsername;
                processResponse.stake = betitems[0].stake;
                processResponse.message = "***EMPTY***";
                int retryCount = 2;
                string betRespCode = string.Empty;
                string addbetResponse = string.Empty;
                dynamic jsonSlipResponse = new JObject();
                double stake = betitems[0].stake;
                double newOdds = -1, oldOdds = -1, currentOdds = -1;
                double newHandicap = 0, oldHandicap = 0;
                m_handlerWriteStatus("");
                m_handlerWriteStatus("Starting Combo : ");
                for (int i = 0; i < betitems.Count; i++)
                {
                    BetItem betitem = betitems[i];
                    string visitUrl = Setting.instance.bet365Domain + (string.IsNullOrEmpty(betitem.PD) ? "" : betitem.PD);
                    visitUrl = visitUrl.Replace("///", "/");
                    visitUrl = visitUrl.Split(new string[] { "/P" }, StringSplitOptions.None)[0];
                    visitUrl = visitUrl.Replace("|", "");
                    if (betitem.PD.StartsWith("EV"))
                        visitUrl = Setting.instance.bet365Domain + "/#/IP/" + betitem.PD;
                    if (!CheckIfLogged() && i == 0)
                    {
                        m_handlerWriteStatus("Session is logged out");
                        if (!DoLogin(visitUrl))
                        {
                            return false;
                        }
                        string jsResult = ExecuteScript("flashvars.USER_NAME", true);
                        m_handlerWriteStatus(jsResult);
                        Thread.Sleep(500);
                    }
                    string betexpression = string.Empty;
                    betexpression = string.Format("{0}|{1}|{2}|@{3}|{4}% - {5}", betitem.sport, betitem.match, betitem.outcome, betitem.odds, betitem.arbPercent, m_socketConnector.getRetriedCount(betitem));
                    m_handlerWriteStatus(betexpression);
                }
                for (int i = 0; i < betitems.Count; i++)
                {
                    BetItem betitem = betitems[i];
                    string visitUrl = Setting.instance.bet365Domain + (string.IsNullOrEmpty(betitem.PD) ? "" : betitem.PD);
                    visitUrl = visitUrl.Replace("///", "/");
                    visitUrl = visitUrl.Split(new string[] { "/P" }, StringSplitOptions.None)[0];
                    visitUrl = visitUrl.Replace("|", "");
                    if (betitem.PD.StartsWith("EV"))
                        visitUrl = Setting.instance.bet365Domain + "/#/IP/" + betitem.PD;

                    m_handlerWriteStatus(betitem.source.ToString() + string.Format(" : Stake = {0}", betitem.stake));
                    GetWidthHeight();
                    PlacingBet = true;

                    bool bSearch = DoVisitEventPage(betitem);
                    _prevItem = betitem;
                    m_handlerWriteStatus(string.Format("bSearch={0}", bSearch));

                    Thread.Sleep(1000);
                    bool bFound = DoAddBetUI(betitem);
                    if (bFound == false)
                        bFound = DoAddBetUIbyTrack(betitem);

                    if (bFound == false)
                    {
                        ExecuteScript("location.reload();");
                        Thread.Sleep(5000);
                        if (!CheckIfLogged())
                        {
                            m_handlerWriteStatus("Session is logged out");
                            if (!DoLogin(visitUrl))
                            {
                                return false;
                            }
                            string jsResult = ExecuteScript("flashvars.USER_NAME", true);
                            m_handlerWriteStatus(jsResult);
                            Thread.Sleep(500);
                        }
                        bFound = DoAddBetUI(betitem);
                    }
                    if (bFound == false)
                    {
                        return false;
                    }
                    m_handlerWriteStatus(RespBody.ToString());
                    jsonSlipResponse = JsonConvert.DeserializeObject<dynamic>(RespBody);
                    betRespCode = jsonSlipResponse.sr.ToString();
                    
                    if (handleOddsChangeUI(ref betitem, jsonSlipResponse, ref newOdds, ref oldOdds, ref newHandicap, ref oldHandicap) == false)
                    {
                        m_handlerWriteStatus("Odds or Line is changed!");
                        Removebet();
                        betitem.retryCount = 1000;
                        return false;
                    }
                    else
                    {
                        foreach (dynamic item in jsonSlipResponse.bt)
                        {
                            try
                            {
                                if (item.ToString().Contains(betitem.runnerId) == false) continue;
                                string strSU = item["su"].ToString();
                                if (strSU.ToLower() == "true")
                                {
                                    Removebet();
                                    betitem.retryCount = 1000;
                                    return false;
                                }
                                break;
                            }
                            catch
                            {
                            }
                        }
                        betitem.retryCount = -1;
                    }
                }

                if (jsonSlipResponse.bg == null || jsonSlipResponse.bg.ToString() == "")
                    return false;
                Thread.Sleep(300);

                string strPlaceBetResp = string.Empty;
                while (--retryCount > 0)
                {
                    string ns = string.Empty;
                    string ms = string.Empty;

                    dynamic jsonContent = string.Empty;
                    string strLimitStake = string.Empty;
                    string strCurOdds = string.Empty;


                    string betUser = Setting.instance.betUsername;
                    string betPass = Setting.instance.betPassword;

                    strPlaceBetResp = DoPlacebetUI(betitems[0]);

                    if (string.IsNullOrEmpty(strPlaceBetResp))
                    {
                        m_handlerWriteStatus("BetResponse is Null!");
                        processResponse.message = string.Format("[{0}:{1}] BetResponse is Null!!", betUser, betPass);
                        Thread.Sleep(6 * 1000);
                        continue;
                    }

                    jsonContent = JsonConvert.DeserializeObject<dynamic>(strPlaceBetResp);
                    m_handlerWriteStatus(jsonContent.ToString());
                    betRespCode = jsonContent.sr.ToString();
                   
                    if (betRespCode == "0")
                    {
                        double placedStake = Utils.ParseToDouble(jsonContent.ts.ToString());
                        processResponse.message = string.Format("[{0}:{1}] SUCCESS. Stake:{2} Odds:{3}", betUser, betPass, placedStake, Utils.GetComboOdds(betitems));
                        m_socketConnector.addPlacedbet(betitems);
                        bRet = true;
                        Removebet();
                        break;
                        //return BETRESP_CODE.SUCCESS;
                    }
                    else if (jsonContent.ToString().Contains("Session Locked") ||
                        betRespCode == "-2")
                    {
                        processResponse.message = string.Format("[{0}:{1}] Session Locked!! ", betUser, betPass);
                        Thread.Sleep(5000);
                        continue;
                    }
                    else if (betRespCode == "9")
                    {
                        m_handlerWriteStatus(ns);
                        m_handlerWriteStatus(ms);
                    }
                    else if (betRespCode == "11")
                    {
                        //Maxstake
                        if (jsonContent["bt"][0]["ms"].ToString() == "0")
                        {
                            processResponse.message = "Market suspended: max stake is 0.";
                            Thread.Sleep(_TIMEOUT_ZERO_MS);
                            continue;
                        }
                        else
                        {
                            string strBetMaxAmount = jsonContent["bt"][0]["rs"].ToString();
                            if (string.IsNullOrEmpty(strBetMaxAmount))
                            {
                                processResponse.message = string.Format("Stake is wrong: {0}", jsonContent.ToString());
                                Thread.Sleep(_TIMEOUT_ZERO_MS);
                                continue;
                            }

                            double maxLimitStake = Utils.ParseToDouble(strBetMaxAmount);
                            maxLimitStake = Math.Floor(maxLimitStake);
                            stake = maxLimitStake;
                            maxLimitStake = Math.Floor(maxLimitStake);
                            _lastMaxStake = maxLimitStake;
                            betitems[0].stake = maxLimitStake;
                            retryCount++;
                            Thread.Sleep(_TIMEOUT_ZERO_MS);
                        }
                    }
                    else if (betRespCode == "14")
                    {

                        processResponse.message = string.Format("[{0}:{1}] Odds changed {2} => {3}.", betUser, betPass, oldOdds, newOdds);
                        break;
                    }
                    else
                    {
                        m_handlerWriteStatus("Place bet failed .." + jsonContent.ToString());
                        processResponse.message = string.Format("[{0}:{1}] Place bet failed {2} ", betUser, betPass, jsonContent.ToString());
                        break;
                    }
                }

                processResponse.message = processResponse.message.ToString();
                m_handlerWriteStatus(processResponse.message.ToString());
                processResponse.balance = 0;
                processResponse.success = 0;
                m_socketConnector.SendData("betResult", processResponse);
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in MakeBet: " + ex.ToString());
            }
            PlacingBet = false;
            return bRet;
        }

        public bool MakeBet(BetItem betitem)
        {
            if (betitem.match.Contains("U19") && Setting.instance.bet365Domain.Contains("es")) return true;
            string visitUrl = Setting.instance.bet365Domain + (string.IsNullOrEmpty(betitem.PD) ? "" : betitem.PD);
            visitUrl = visitUrl.Replace("///", "/");
            visitUrl = visitUrl.Split(new string[] { "/P" }, StringSplitOptions.None)[0];
            visitUrl = visitUrl.Replace("|", "");
            if (betitem.PD.StartsWith("EV"))
                visitUrl = Setting.instance.bet365Domain + "/#/IP/" + betitem.PD;
            if (betitem.tipster == "betspan")
                visitUrl = Setting.instance.bet365Domain + betitem.PD1;
            string betexpression = string.Empty;
            if (betitem.source == SOURCE.TIPSTER)
            {
                betexpression = string.Format("New Bet {0} {1} {2} @{3} Double:{4} EW:{5}", betitem.Leader, betitem.match, betitem.pick, betitem.odds, betitem.isDouble, betitem.bEW);
                m_handlerWriteStatus(betexpression);
            }
            else
            {
                betexpression = string.Format("{0}|{1}|{2}|@{3}|{4}% - {5}", betitem.sport, betitem.match, betitem.outcome, betitem.odds, betitem.arbPercent, m_socketConnector.getRetriedCount(betitem));
                m_handlerWriteStatus(betexpression);
            }

            bool bRet = false;
            m_handlerWriteStatus(betitem.source.ToString() + string.Format(" : Stake = {0}", betitem.stake));
            GetWidthHeight();
            PlacingBet = true;
            try
            {
                double stake = betitem.stake;
                dynamic processResponse = new JObject();
                processResponse.username = Setting.instance.betUsername;
                processResponse.stake = stake;
                processResponse.message = "***EMPTY***";

                if (!CheckIfLogged())
                {
                    m_handlerWriteStatus("Session is logged out");
                    if (!DoLogin(visitUrl))
                    {
                        return false;
                    }
                    string jsResult = ExecuteScript("flashvars.USER_NAME", true);
                    m_handlerWriteStatus(jsResult);
                    Thread.Sleep(500);
                }

                //ExecuteScript($"location.href.includes('{betitem.PD.Replace("|", "")}')?'':location.href='{visitUrl}'", true);
                //if (_prevItem.PD != betitem.PD) 

                bool bSearch = DoVisitEventPage(betitem);
                _prevItem = betitem;
                m_handlerWriteStatus(string.Format("bSearch={0}", bSearch));
                Removebet();
                string addbetResponse = string.Empty;
                dynamic jsonSlipResponse = new JObject();
                int retryCount = 2;

                if (betitem.source == SOURCE.BASHING && Setting.instance.bet365Domain.Contains("es") == false)
                {
                    Thread.Sleep(100);
                }
                else
                {

                    if ((betitem.source == SOURCE.TIPSTER && betitem.tipster == "messi")
                        || Setting.instance.placingMode == PLACING_MODE.FAST
                        || bSearch == false)
                    {
                        bool bFound = DoAddBetUIbyTrack(betitem);
                        if (bFound == false)
                            bFound = DoAddBetUIbyTrack(betitem);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        bool bFound = DoAddBetUI(betitem);
                        if (bFound == false)
                            bFound = DoAddBetUIbyTrack(betitem);

                        if (bFound == false)
                        {
                            ExecuteScript("location.reload();");
                            Thread.Sleep(5000);
                            if (!CheckIfLogged())
                            {
                                m_handlerWriteStatus("Session is logged out");
                                if (!DoLogin(visitUrl))
                                {
                                    return false;
                                }
                                string jsResult = ExecuteScript("flashvars.USER_NAME", true);
                                m_handlerWriteStatus(jsResult);
                                Thread.Sleep(500);
                            }
                            bFound = DoAddBetUI(betitem);
                        }
                        if (bFound == false)
                        {
                            return false;
                        }
                    }
                }


                CheckEWBox(betitem);

                jsonSlipResponse = JsonConvert.DeserializeObject<dynamic>(RespBody);
                //// Check if wrong bet
                //try
                //{
                //    string strSU = jsonSlipResponse.bt[0].su.ToString();
                //    if (strSU.ToLower() == "true")
                //    {
                //        m_socketConnector.m_placedBetList.Add(betitem);
                //        PlacingBet = false;
                //        return true;
                //    }
                //    string strMD = jsonSlipResponse.bt[0].pt[0].md.ToString();
                //    if (strMD.ToLower() == "")
                //    {
                //        m_socketConnector.m_placedBetList.Add(betitem);
                //        PlacingBet = false;
                //        return true;
                //    }
                //}
                //catch
                //{
                //}
                string betRespCode = jsonSlipResponse.sr.ToString();
                m_handlerWriteStatus(jsonSlipResponse.ToString());
                double newOdds = -1, oldOdds = -1, currentOdds = -1;
                double newHandicap = 0, oldHandicap = 0;
                if (handleOddsChangeUI(ref betitem, jsonSlipResponse, ref newOdds, ref oldOdds, ref newHandicap, ref oldHandicap) == false)
                {
                    Removebet();
                    betitem.retryCount = 1000;
                    return false;
                }
                // If bet is suspended, we will skip it 
                foreach (dynamic item in jsonSlipResponse.bt)
                {
                    try
                    {
                        if (item.ToString().Contains(betitem.runnerId) == false) continue;
                        string strSU = item["su"].ToString();
                        if (strSU.ToLower() == "true")
                        {
                            Removebet();
                            betitem.retryCount = 1000;
                            return false;
                        }
                        break;
                    }
                    catch
                    {
                    }
                }

                if (jsonSlipResponse.bg == null || jsonSlipResponse.bg.ToString() == "")
                    return false;
                Thread.Sleep(300);

                string strPlaceBetResp = string.Empty;
                while (--retryCount > 0)
                {
                    if (betitem.bEW) stake = stake / 2;

                    string ns = string.Empty;
                    string ms = string.Empty;

                    dynamic jsonContent = string.Empty;
                    string strLimitStake = string.Empty;
                    string strCurOdds = string.Empty;


                    string betUser = Setting.instance.betUsername;
                    string betPass = Setting.instance.betPassword;

                    strPlaceBetResp = DoPlacebetUI(betitem);

                    if (string.IsNullOrEmpty(strPlaceBetResp))
                    {
                        m_handlerWriteStatus("BetResponse is Null!");
                        processResponse.message = string.Format("[{0}:{1}] BetResponse is Null!!", betUser, betPass);
                        Thread.Sleep(6 * 1000);
                        continue;
                    }

                    jsonContent = JsonConvert.DeserializeObject<dynamic>(strPlaceBetResp);
                    m_handlerWriteStatus(jsonContent.ToString());
                    betRespCode = jsonContent.sr.ToString();
                    // Get Max stake if available
                    try
                    {
                        if (jsonContent["bt"][0]["rs"] != null)
                        {
                            strLimitStake = jsonContent["bt"][0]["rs"].ToString();
                            double maxLimitStake = Utils.ParseToDouble(strLimitStake);
                            if (maxLimitStake > 0)
                                _lastMaxStake = Math.Floor(maxLimitStake);
                        }

                        if (jsonContent["bt"][0]["ms"] != null)
                        {
                            strLimitStake = jsonContent["bt"][0]["ms"].ToString();
                            double maxLimitStake = Utils.ParseToDouble(strLimitStake);
                            if (maxLimitStake > 0)
                                _lastMaxStake = Math.Floor(maxLimitStake);
                            else if (maxLimitStake == 0)
                            {
                                m_handlerWriteStatus("Max stake is zero.");
                                bRet = true;
                                m_socketConnector.m_placedBetList.Add(betitem);
                                PlacingBet = false;
                                break;
                            }
                        }
                    }
                    catch
                    {

                    }
                    if (betRespCode == "0")
                    {
                        string betId = jsonContent.bt[0].tr.ToString();
                        betitem.betId = betId;
                        betitem.placedDate = DateTime.Now;
                        PostPlacedBet(betitem);
                        double placedStake = Utils.ParseToDouble(jsonContent.ts.ToString());
                        processResponse.message = string.Format("[{0}:{1}] SUCCESS. Stake:{2} Odds:{3}", betUser, betPass, placedStake, betitem.odds);
                        m_socketConnector.m_placedBetList.Add(betitem);
                        if (betitem.source == SOURCE.BASHING) m_socketConnector.countOfTodayBashBet++;
                        bRet = true;
                        Removebet();
                        break;
                        //return BETRESP_CODE.SUCCESS;
                    }
                    else if (jsonContent.ToString().Contains("Session Locked") ||
                        betRespCode == "-2")
                    {
                        processResponse.message = string.Format("[{0}:{1}] Session Locked!! ", betUser, betPass);
                        Thread.Sleep(5000);
                        continue;
                    }
                    else if (betRespCode == "9")
                    {
                        m_handlerWriteStatus(ns);
                        m_handlerWriteStatus(ms);
                    }
                    else if (betRespCode == "19")
                    {
                        //Log out
                        DoAddBetUI(betitem);
                        Thread.Sleep(1000);
                        continue;
                    }
                    else if (betRespCode == "10")
                    {
                        //Not Enough Balance: Put all Balance
                        processResponse.message = string.Format("[{0}:{1}] Balance is small ", betUser, betPass);
                    }
                    else if (betRespCode == "11")
                    {
                        //Maxstake
                        if (jsonContent["bt"][0]["ms"].ToString() == "0")
                        {
                            processResponse.message = "Market suspended: max stake is 0.";
                            Thread.Sleep(_TIMEOUT_ZERO_MS);
                            continue;
                        }
                        else
                        {
                            string strBetMaxAmount = jsonContent["bt"][0]["rs"].ToString();
                            if (string.IsNullOrEmpty(strBetMaxAmount))
                            {
                                processResponse.message = string.Format("Stake is wrong: {0}", jsonContent.ToString());
                                Thread.Sleep(_TIMEOUT_ZERO_MS);
                                continue;
                            }

                            double maxLimitStake = Utils.ParseToDouble(strBetMaxAmount);
                            maxLimitStake = Math.Floor(maxLimitStake);
                            stake = maxLimitStake;
                            if (Setting.instance.isComplex)
                            {
                                maxLimitStake = Math.Floor(maxLimitStake) - 1;
                                //maxLimitStake = Math.Floor(maxLimitStake) + 0.88;
                            }
                            maxLimitStake = Math.Floor(maxLimitStake);
                            _lastMaxStake = maxLimitStake;
                            betitem.stake = maxLimitStake;
                            retryCount++;
                            Thread.Sleep(_TIMEOUT_ZERO_MS);
                        }
                    }
                    else if (betRespCode == "14")
                    {

                        if (betitem.source == SOURCE.DOGWIN || betitem.source == SOURCE.DOG_PREMATCH)
                        {
                            dynamic tmpSU = jsonContent["bt"][0]["su"];
                            if (tmpSU != null)
                            {
                                string strSU = tmpSU.ToString();
                                if (strSU.ToLower() == "true")
                                {
                                    betitem.bSuspended = true;
                                    processResponse.message = string.Format("[{0}:{1}] Suspended {2} => {3}.", betUser, betPass, oldOdds, newOdds);
                                    break;
                                }
                            }
                        }
                        processResponse.message = string.Format("[{0}:{1}] Odds changed {2} => {3}.", betUser, betPass, oldOdds, newOdds);
                        break;
                    }
                    else if (betRespCode == "15")
                    {
                        Setting.instance.writeRestartLog(string.Format("[{0}] Reason: placebet response code => {1}", DateTime.Now, betRespCode));
                        //m_handlerWriteStatus("It will be restarted after 2 mins");
                        //Thread.Sleep(1000 * 60 * 2);
                        //Setting.instance.WriteRegistry("bAutoStart", "1");
                        //Process.Start(Application.ExecutablePath);
                        //Process.GetCurrentProcess().Kill();
                        //bRet = true;
                        break;
                    }
                    else if (betRespCode == "41")
                    {
                        processResponse.message = string.Format("[{0}:{1}] Allow login others", Setting.instance.betUsername, Setting.instance.betPassword);
                        break;
                    }
                    else if (betRespCode == "24")
                    {
                        try
                        {
                            string[] oldPTList = ns.Split(new string[] { "pt=N" }, StringSplitOptions.None);
                            foreach (var item in jsonContent.bt)
                            {
                                for (int i = 0; i < oldPTList.Length; i++)
                                {
                                    string pt = oldPTList[i];
                                    if (pt.Contains(item.fi.ToString()))
                                    {
                                        //pt = changeStakePart(pt, item.ra.ToString());
                                        double maxLimitStake = Utils.ParseToDouble(item.ms.ToString());
                                        maxLimitStake = Math.Floor(maxLimitStake) - 1;
                                        betitem.stake = maxLimitStake;
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                        Thread.Sleep(2300);
                    }
                    else
                    {
                        m_handlerWriteStatus("Place bet failed .." + jsonContent.ToString());
                        processResponse.message = string.Format("[{0}:{1}] Place bet failed {2} ", betUser, betPass, jsonContent.ToString());
                        break;
                    }
                }

                processResponse.message = processResponse.message.ToString() + " - " + betexpression + " | " + betitem.runnerId;
                m_handlerWriteStatus(processResponse.message.ToString());
                processResponse.dbID = betitem.dbId;
                processResponse.betId = betitem.betId;
                processResponse.balance = 0;
                processResponse.success = 0;
                m_socketConnector.SendData("betResult", processResponse);
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in MakeBet: " + ex.ToString());
            }
            PlacingBet = false;
            return bRet;
        }

        private void CheckEWBox(BetItem betitem)
        {
            try
            {
                if (betitem.bEW || betitem.source == SOURCE.BASHING)
                {
                    string jsResult = ExecuteScript("findEWCheckBox()", true);
                    m_handlerWriteStatus(string.Format("findEWCheckBox() = {0}", jsResult));
                    if (jsResult == "false")
                    {
                        return;
                    }
                    betPosition = JsonConvert.DeserializeObject<JObject>(jsResult);
                    decimal x = decimal.Parse(betPosition.SelectToken("x").ToString());
                    decimal y = decimal.Parse(betPosition.SelectToken("y").ToString());

                    if (x != 0 && y != 0)
                    {
                        FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                        FakeUserAction.Intance.SimClick();
                    }
                    else
                    {
                        string jsCode = "BetSlipLocator.betSlipManager.betslip.activeModule.slip.bets[0].ewexCheckboxChecked()";
                        m_handlerWriteStatus(jsCode);
                        ExecuteScript(jsCode);

                        jsCode = "BetSlipLocator.betSlipManager.betslip.activeModule.slip.bets[0].eachwayCheckboxChecked()";
                        m_handlerWriteStatus(jsCode);
                        ExecuteScript(jsCode);
                    }
                }
            }
            catch
            {
            }
        }

        private void PostPlacedBet(BetItem betitem)
        {
            if (!Setting.instance.isRecordResult) return;
            try
            {
                if (betitem.source == SOURCE.BETBURGER)
                {
                    string strRaceItem = JsonConvert.SerializeObject(betitem);
                    CustomEndpoint.postRequest(Setting.instance.serverAddr + "/history/registerBbBet", strRaceItem);
                }
                else if (betitem.source == SOURCE.TIPSTER && betitem.tipster == "jlivesoccer")
                {
                    SoccerHistory raceItem = new SoccerHistory();
                    raceItem.league = betitem.league;
                    raceItem.match = betitem.match;
                    raceItem.pick = betitem.pick;
                    raceItem.handicap = betitem.handicap;
                    raceItem.odds = betitem.odds;
                    raceItem.placed = DateTime.Now;
                    raceItem.dprofit = betitem.dprofit;
                    raceItem.sharpbookie = betitem.softbookie;
                    raceItem.pin_odds = betitem.pin_odds;
                    raceItem.pin_reverseodds = betitem.pin_reverseodds;
                    raceItem.h_score = betitem.strScore.Split('-')[0];
                    raceItem.a_score = betitem.strScore.Split('-')[1];
                    raceItem.mins = betitem.strTime;
                    raceItem.bet_id = betitem.FI;
                    string strRaceItem = JsonConvert.SerializeObject(raceItem);
                    CustomEndpoint.postRequest(Setting.instance.serverAddr + "/history/registerLiveSoccerBet", strRaceItem);
                }
            }
            catch
            {

            }
        }

        public void GetWidthHeight()
        {
            string clientWidth = ExecuteScript("document.body.clientWidth", true);
            string clientHeight = ExecuteScript("document.body.clientHeight", true);
            int intWidth = 0, intHeight = 0;
            if (int.TryParse(clientHeight, out intWidth)) Setting.instance.innerWidth = intWidth;
            if (int.TryParse(clientHeight, out intHeight)) Setting.instance.innerHeight = intHeight;
        }

        public void DoTest()
        {
            CancellationTokenSource canceller = new CancellationTokenSource();
            var cancellerToken = canceller.Token;
            Task t = Task.Run(() =>
            {
                try
                {
                    int retryCounter = 0;
                    while (!cancellerToken.IsCancellationRequested)
                    {
                        if (retryCounter % 5 == 0)
                        {
                            // Check if bet button is available again and click
                            FakeUserAction.Intance.SimMouseMoveTo(new FirefoxBet365Placer.Json.Point(100, 100));
                            FakeUserAction.Intance.SimClick();
                        }
                        Thread.Sleep(Utils.GetRandValue(90, 120));
                        retryCounter++;
                    }
                    m_handlerWriteStatus("test is done");
                }
                catch (Exception ex)
                {
                }
            }, cancellerToken);

            TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 30);
            if (!t.Wait(ts))
            {
                canceller.Cancel();
                m_handlerWriteStatus("The timeout interval elapsed in DoPlaceBet of BetController.");
            }
        }
        async public Task<bool> MakeRandomBet()
        {
            try
            {
                string result = ExecuteScript("GetPosOfFakeBet()", true);
                m_handlerWriteStatus($"GetPosOfFakeBet() => {result}");
                if (result.ToLower() == "false" || result.ToLower() == "")
                {
                    return false;
                }

                dynamic betPosition = JsonConvert.DeserializeObject<JObject>(result);
                decimal x = decimal.Parse(betPosition.SelectToken("x").ToString());
                decimal y = decimal.Parse(betPosition.SelectToken("y").ToString());
                if (x == 0 && y == 0)
                {
                    m_handlerWriteStatus("Wrong position detected");
                    return false;
                }
                FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                FakeUserAction.Intance.SimClick();
                Thread.Sleep(1000);

                BetItem betitem = new BetItem();
                betitem.stake = 0.25;
                if(Setting.instance.countryCode == "br")
                    betitem.stake = 0.75;
                betitem.bEW = false;
                DoPlacebetUI(betitem);
            }
            catch
            {
            }
            return true;
        }

        public string DoPlacebetUI(BetItem betitem)
        {
            m_handlerWriteStatus("DoPlacebetUI");
            ClickBetPushAlert();
            RespBody = string.Empty;
            WaitingForAPI = true;
            CancellationTokenSource canceller = new CancellationTokenSource();
            var cancellerToken = canceller.Token;
            Task t = Task.Run(async () =>
            {
                try
                {
                    string result = ExecuteScript("JSON.stringify(BetSlipLocator.betSlipManager.betslip.getBetslipElement().getBoundingClientRect())", true);
                    m_handlerWriteStatus("betslip pos => " + result);
                    if (!string.IsNullOrEmpty(result)) 
                    {
                        betPosition = JsonConvert.DeserializeObject<JObject>(result);
                    }
                    decimal x      = decimal.Parse(betPosition.SelectToken("x").ToString());
                    decimal y      = decimal.Parse(betPosition.SelectToken("y").ToString());
                    decimal width  = decimal.Parse(betPosition.SelectToken("width").ToString());
                    decimal height = decimal.Parse(betPosition.SelectToken("height").ToString());

                    _X_stake = x + width / 4 + Utils.GetRandValue(0, 15, true);
                    _Y_stake = y + height - 15 + Utils.GetRandValue(0, 10, true);
                    betitem.stake = Math.Floor(betitem.stake * 10) / 10;
                    if (betitem.stake > 1)
                        betitem.stake = Math.Floor(betitem.stake);
                    if (betitem.bEW)
                    {
                        betitem.stake = betitem.stake / 2;
                    }
                    m_handlerWriteStatus(string.Format("Stake pos: ({0}, {1}) stake = {2}", _X_stake, _Y_stake, betitem.stake));
                    FakeUserAction.Intance.SimMouseMoveTo(new Point(_X_stake, _Y_stake));
                    FakeUserAction.Intance.SimClick();
                    Thread.Sleep(Utils.GetRandValue(100, 600));
                    // Send Stake
                    
                    InputSimulator inputSim = new InputSimulator();
                    foreach (char digit in betitem.stake.ToString().ToCharArray()) 
                    {
                        inputSim.Keyboard.KeyPress((VirtualKeyCode)digit);
                        Thread.Sleep(Utils.GetRandValue(10, 100));
                    }
                    //m_handlerWriteStatus(string.Format("SetStake('{0}')", betitem.stake));
                    //ExecuteScript(string.Format("SetStake('{0}')", betitem.stake).Replace(",", "."));
                    CheckEWBox(betitem);
                    decimal xBtn = x + width * 3/ 4 + Utils.GetRandValue(-10, 10, true);
                    decimal yBtn = _Y_stake + Utils.GetRandValue(-5, 5, true); ;
                    Thread.Sleep(Utils.GetRandValue(1500, 2500));
                    m_handlerWriteStatus(string.Format("Bet pos: ({0}, {1})", xBtn, yBtn));
                    FakeUserAction.Intance.SimMouseMoveTo(new Point(xBtn, yBtn));
                    FakeUserAction.Intance.SimClick();
                    ResolveQRCode();
                    _lastCallPlaceBet = DateTime.Now;
                    int retryCounter = -1;
                    while (WaitingForAPI && !cancellerToken.IsCancellationRequested)
                    {
                        if (retryCounter % 10 == 0) 
                        {
                            if (!CheckIfLogged()) 
                            {
                                break;
                            }
                            // Check if bet button is available again and click
                            decimal xTemp = xBtn + Utils.GetRandValue(0, 15, true);
                            decimal yTemp = yBtn + Utils.GetRandValue(0, 15, true); ;
                            FakeUserAction.Intance.SimMouseMoveTo(new Point(xTemp, yTemp));
                            FakeUserAction.Intance.SimClick();
                        }
                        Thread.Sleep(Utils.GetRandValue(130, 200));
                        retryCounter++;
                        if (retryCounter > 50) break;
                    }

                    if (!string.IsNullOrEmpty(RespBody))
                    {
                        // If stake is above max stake, odds or line has been changed!>
                        dynamic jsonContent = JsonConvert.DeserializeObject<dynamic>(RespBody);
                        string betRespCode = jsonContent.sr.ToString();
                        m_handlerWriteStatus($"betRespCode = {betRespCode}");
                        if (betRespCode == "11")
                        {
                            retryCounter = 0;
                            WaitingForAPI = true;
                            while (WaitingForAPI && !cancellerToken.IsCancellationRequested)
                            {
                                if (retryCounter % 10 == 0)
                                {
                                    // Check if bet button is available again and click
                                    ResolveQRCode();
                                    decimal xTemp = xBtn + Utils.GetRandValue(0, 15, true);
                                    decimal yTemp = yBtn + Utils.GetRandValue(0, 15, true); ;
                                    FakeUserAction.Intance.SimMouseMoveTo(new Point(xTemp, yTemp));
                                    FakeUserAction.Intance.SimClick();
                                }
                                Thread.Sleep(Utils.GetRandValue(130, 200));
                                retryCounter++;
                            }
                        }
                        else if (betRespCode == "14")
                        {
                            ResolveQRCode();
                            double newOdds = -1, oldOdds = -1, currentOdds = -1;
                            double newHandicap = 0, oldHandicap = 0;
                            bool bRet = handleOddsChangeUI(ref betitem, jsonContent, ref newOdds, ref oldOdds, ref newHandicap, ref oldHandicap);
                            if (bRet == false) 
                            {
                                betitem.retryCount = 1000;
                                return;
                            } 
                            while (!cancellerToken.IsCancellationRequested)
                            {
                                retryCounter = 0;
                                WaitingForAPI = true;
                                while (WaitingForAPI && !cancellerToken.IsCancellationRequested)
                                {
                                    if (retryCounter % 10 == 0)
                                    {
                                        // Check if bet button is available again and click
                                        decimal xTemp = xBtn + Utils.GetRandValue(0, 15, true);
                                        decimal yTemp = yBtn + Utils.GetRandValue(0, 15, true); ;
                                        FakeUserAction.Intance.SimMouseMoveTo(new Point(xTemp, yTemp));
                                        FakeUserAction.Intance.SimClick();
                                    }
                                    Thread.Sleep(Utils.GetRandValue(130, 200));
                                    retryCounter++;
                                }
                                jsonContent = JsonConvert.DeserializeObject<dynamic>(RespBody);
                                betRespCode = jsonContent.sr.ToString();
                                if (betRespCode != "14") break;
                            }
                        }
                        else if (betRespCode == "70")
                        {
                            m_handlerWriteStatus("QR code scanning geo_complyfailed");
                            timeToSendQRGetRequest = DateTime.MaxValue;
                            ResolveQRCode();
                            while (!cancellerToken.IsCancellationRequested)
                            {
                                retryCounter = 0;
                                WaitingForAPI = true;
                                RespBody = string.Empty;
                                while (WaitingForAPI && !cancellerToken.IsCancellationRequested)
                                {
                                    if (retryCounter % 10 == 0)
                                    {
                                        // Check if bet button is available again and click
                                        decimal xTemp = xBtn + Utils.GetRandValue(0, 15, true);
                                        decimal yTemp = yBtn + Utils.GetRandValue(0, 15, true); ;
                                        FakeUserAction.Intance.SimMouseMoveTo(new Point(xTemp, yTemp));
                                        FakeUserAction.Intance.SimClick();
                                    }
                                    Thread.Sleep(Utils.GetRandValue(130, 200));
                                    retryCounter++;
                                }
                                jsonContent = JsonConvert.DeserializeObject<dynamic>(RespBody);
                                betRespCode = jsonContent.sr.ToString();
                                if (betRespCode != "86") break;
                            }

                        }
                        else if (betRespCode == "86")
                        {
                            m_handlerWriteStatus("QR code scanning ,Trying to betting again!");
                            ResolveQRCode();
                            while (!cancellerToken.IsCancellationRequested)
                            {
                                retryCounter = 0;
                                WaitingForAPI = true;
                                RespBody = string.Empty;
                                while (WaitingForAPI && !cancellerToken.IsCancellationRequested)
                                {
                                    if (retryCounter % 10 == 0)
                                    {
                                        // Check if bet button is available again and click
                                        decimal xTemp = xBtn + Utils.GetRandValue(0, 15, true);
                                        decimal yTemp = yBtn + Utils.GetRandValue(0, 15, true); ;
                                        FakeUserAction.Intance.SimMouseMoveTo(new Point(xTemp, yTemp));
                                        FakeUserAction.Intance.SimClick();
                                    }
                                    Thread.Sleep(Utils.GetRandValue(130, 200));
                                    retryCounter++;
                                }
                                jsonContent = JsonConvert.DeserializeObject<dynamic>(RespBody);
                                betRespCode = jsonContent.sr.ToString();
                                if (betRespCode != "86") break;
                            }

                        }

                        // Remove placed bets
                        FakeUserAction.Intance.SimMouseMoveTo(new Point(xBtn, y - 100));
                        FakeUserAction.Intance.SimClick();
                    }
                }
                catch (Exception e)
                {
                }
            }, canceller.Token);

            TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 25);
            if (!t.Wait(ts))
            {
                canceller.Cancel();
                m_handlerWriteStatus("The timeout interval elapsed in DoPlaceBet of BetController.");
            }

            return RespBody;
        }

        async private Task<Point> GetPositionBet()
        {
            try
            {
                string result = ExecuteScript($"getPostionBet()");
                if (result.ToLower() == "false" || string.IsNullOrEmpty(result)) return null;
                m_handlerWriteStatus("getPostionBet() => " + result);

                dynamic betPosition = JsonConvert.DeserializeObject<JObject>(result);
                decimal x = decimal.Parse(betPosition.SelectToken("x").ToString());
                decimal y = decimal.Parse(betPosition.SelectToken("y").ToString());

                if (x == 0 || y == 0)
                {

                    return null;
                }
                return new Point(x, y);
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in GetPositionBet : " + ex.ToString());
            }
            return null;
        }

        async private Task<bool> ExpandMarketGroupAndSearch(string runnerId)
        {
            try
            {
                //m_handlerWriteStatus("ExpandMarketGroupAndSearch");
                //ElementHandle[] closedMarkets = m_browser.QuerySelectorAllAsync("div[class='gl-MarketGroupButton ']").Result;
                //for (int i = 0; i < closedMarkets.Length; i++)
                //{
                //    ElementHandle eh = closedMarkets[closedMarkets.Length - i - 1];
                //    BoundingBox rect = await eh.BoundingBoxAsync();
                //    Point endPos = new Point(rect.X, rect.Y);
                //    await FakeUserAction.Intance.AdjustElementPositionWithMouse(endPos);
                //    await FakeUserAction.Intance.SimClickElement(eh);

                //    string result = await ExecuteScript($"scrollBetIntoView('{runnerId}')");
                //    m_handlerWriteStatus($"scrollBetIntoView('{runnerId}') => {result}");
                //    if (result.ToLower() == "true")
                //    {
                //        return true;
                //    }
                //}
            }
            catch
            {

            }
            return false;
        }


        public bool DoAddBetUIbyTrack(BetItem betitem)
        {
            m_handlerWriteStatus("DoAddBetUIbyTrack");
            //string visitUrl = Setting.instance.bet365Domain + "/#/IP/B1";
            //NavigateInvoke(visitUrl);
            Thread.Sleep(700);
            RespBody = string.Empty;
            WaitingForAPI = true;
            bool isBetSlipLoaded = false;
            CancellationTokenSource canceller = new CancellationTokenSource();
            var cancellerToken = canceller.Token;
            Task t = Task.Run(async () =>
            {
                try
                {
                    string ns = betitem.bs;
                    string FI = Utils.Between(ns, "#f=", "#");
                    double odds = betitem.odds;
                    string PI = betitem.runnerId;
                    string jsCode = string.Format("AddBetScript('{0}', '{1}', '{2}', '', '%s')", ns, FI, betitem.odds, betitem.runnerId, FI, betitem.runnerId);
                    m_handlerWriteStatus($"AddBetScript('{ns}','{FI}','{odds}','{PI}','{FI}-{PI}')");
                    ExecuteScript($"AddBetScript('{ns}','{FI}','{odds}','{PI}','{FI}-{PI}')", false);
                    string checkSlipCode = "BetSlipLocator.betSlipManager.betslip.activeModule.slip.footer.stakeBox._active_element.getBoundingClientRect().bottom <= window.innerHeight";

                    checkSlipCode = "BetSlipLocator.betSlipManager.betslip.activeModule.slip.bets.length > 0";
                    bool isAdded = ExecuteScript(checkSlipCode, true).ToLower() == "true";

                    //while (!isAdded && !cancellerToken.IsCancellationRequested && WaitingForAPI)
                    while (!cancellerToken.IsCancellationRequested && WaitingForAPI)
                    {
                        //isAdded = ExecuteScript(checkSlipCode, true).ToLower() == "true";
                        Thread.Sleep(500);
                    }
                    m_handlerWriteStatus("Betslip is added");
                    isBetSlipLoaded = true;
                }
                catch (Exception) { }
            }, cancellerToken);

            TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 5);
            if (!t.Wait(ts))
            {
                m_handlerWriteStatus("The timeout interval elapsed in DoUIAddBet of BetController.");
                canceller.Cancel();
            }
            return isBetSlipLoaded;
        }

        public bool DoAddBetUI(BetItem betitem)
        {
            m_handlerWriteStatus("DoAddBetUI");
            RespBody = string.Empty;
            WaitingForAPI = true;
            bool isBetSlipLoaded = false;
            CancellationTokenSource canceller = new CancellationTokenSource();
            var cancellerToken = canceller.Token;
            Task t = Task.Run(async () =>
            {
                try
                {
                    ClickBetPushAlert();
                    string result = string.Empty;
                    decimal x = 0, y = 0;
                    JObject betPosition = null;
                    // Scroll Element into View and Get position
                    
                    string runnerId = betitem.runnerId.Replace("|", "");
                    m_handlerWriteStatus($"scrollBetIntoView('{runnerId}') => started");
                    result = ExecuteScript($"scrollBetIntoView('{runnerId}')", true);
                    m_handlerWriteStatus($"scrollBetIntoView('{runnerId}') => {result}");

                    if (result.ToLower() == "false")
                    {
                        //ExecuteScript("expandAllMarketGroup()", false);
                        ExecuteScript("ExpandAllMarketGroup()", true);
                        Thread.Sleep(1000);
                        result = ExecuteScript($"scrollBetIntoView('{betitem.runnerId}')", true);
                        m_handlerWriteStatus($"scrollBetIntoView('{betitem.runnerId}') => {result}");
                    }

                    if (result.ToLower() == "false") 
                    {
                        m_handlerWriteStatus("Bet is not found!");
                        return;
                    }
                    else if (result.ToLower() == "suspended")
                    {
                        m_handlerWriteStatus("Bet is suspended!");
                        return;
                    }

                    betPosition = JsonConvert.DeserializeObject<JObject>(result);
                    x = decimal.Parse(betPosition.SelectToken("x").ToString());
                    y = decimal.Parse(betPosition.SelectToken("y").ToString());

                    if(x == 0 && y == 0)
                    {
                        m_handlerWriteStatus("Wrong position detected and loadSlipByScript()");
                        FakeUserAction.Intance.SimRandomMouseMove(false);
                        result = ExecuteScript("loadSlipByScript();", true);
                    }
                    else
                    {
                        m_handlerWriteStatus(string.Format("Setting.instance.innerHeight = {0}", Setting.instance.innerHeight));
                        for(int i =0; i < 10; i++)
                        {
                            if (y > Setting.instance.innerHeight - 300 || y < 0)
                            {
                                decimal wheelOffset = 0;
                                if (y < 0)
                                {
                                    wheelOffset = y - Setting.instance.innerHeight / 2;
                                }
                                else
                                {
                                    wheelOffset = y - Setting.instance.innerHeight + Setting.instance.innerHeight / 3;
                                }
                                m_handlerWriteStatus(string.Format("SimWheel = {0}", wheelOffset));
                                FakeUserAction.Intance.SimMouseMoveTo(new Point((decimal)(Setting.instance.innerWidth * 0.45), 200));
                                FakeUserAction.Intance.SimWheel(wheelOffset);
                                Thread.Sleep(500);
                                result = ExecuteScript($"scrollBetIntoView('{runnerId}')", true);
                                m_handlerWriteStatus($"scrollBetIntoView('{runnerId}') => {result}");

                                betPosition = JsonConvert.DeserializeObject<JObject>(result);
                                x = decimal.Parse(betPosition.SelectToken("x").ToString());
                                y = decimal.Parse(betPosition.SelectToken("y").ToString());
                            }
                            else
                            {
                                FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                                FakeUserAction.Intance.SimClick();
                                break;
                            }
                        }
                    }


                    /*
                    Point betPos = await GetPositionBet();
                    if (await FakeUserAction.Intance.AdjustElementPositionWithMouse(betPos))
                    {
                        betPos = await GetPositionBet();
                        await FakeUserAction.Intance.SimMouseMoveTo(betPos, 10, 1);
                    }
                    else
                    {
                        await FakeUserAction.Intance.SimMouseMoveTo(betPos, 10, 1);
                    }

                    RespBody = string.Empty;
                    while (WaitingForAPI) Thread.Sleep(100);
                    dynamic jsonSlipResponse = JsonConvert.DeserializeObject<dynamic>(RespBody);
                    m_handlerWriteStatus(jsonSlipResponse.ToString());
                    */

                    string checkSlipCode = "BetSlipLocator.betSlipManager.betslip.activeModule.slip.footer.stakeBox._active_element.getBoundingClientRect().bottom <= window.innerHeight";
                    //bool isAdded = ExecuteScript("BetSlipLocator.betSlipManager.betslip.getBetCount()", true) != "0";
                    checkSlipCode = "BetSlipLocator.betSlipManager.betslip.activeModule.slip.bets.length > 0";
                    bool isAdded = ExecuteScript(checkSlipCode, true).ToLower() == "true";
                    if (Setting.instance.isDouble)
                    {
                        while (!cancellerToken.IsCancellationRequested && WaitingForAPI)
                        {
                            isAdded = ExecuteScript(checkSlipCode, true).ToLower() == "true";
                            Thread.Sleep(500);
                        }
                    }
                    else
                    {
                        while (!isAdded && !cancellerToken.IsCancellationRequested && WaitingForAPI)
                        {
                            isAdded = ExecuteScript(checkSlipCode, true).ToLower() == "true";
                            Thread.Sleep(500);
                        }
                    }
                    ClickBetPushAlert();
                    m_handlerWriteStatus("Betslip is added");
                    isBetSlipLoaded = true;
                }
                catch (Exception) { }
            }, cancellerToken);

            TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 10);
            if (!t.Wait(ts))
            {
                m_handlerWriteStatus("The timeout interval elapsed in DoUIAddBet of BetController.");
                canceller.Cancel();
            }
            return isBetSlipLoaded;
        }
        public void ClickBetPushAlert()
        {
            try
            {
                if (Setting.instance.countryCode.Contains("it"))
                {
                    while (true)
                    {
                        Thread.Sleep(100);
                        string result = ExecuteScript("getPosHandicapAlert();", true);
                        m_handlerWriteStatus("getPosHandicapAlert() = " + result);
                        if (!result.Contains("false"))
                        {
                            betPosition = JsonConvert.DeserializeObject<JObject>(result);
                            decimal x = decimal.Parse(betPosition.SelectToken("x").ToString());
                            decimal y = decimal.Parse(betPosition.SelectToken("y").ToString());
                            if (x > 0 && y > 0)
                            {
                                FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
                                FakeUserAction.Intance.SimClick();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }
            }
            catch
            {
            }
        }
        public bool Removebet()
        {
            m_handlerWriteStatus("Removebet - started!");
            ExecuteScript("deleteAllBets()");
            ClickBetPushAlert();
            m_handlerWriteStatus("Removebet - finished!");
            return true;
        }

        async public Task<bool> DoUILogin(string visitUrl)
        {
            try
            {
                //DoLoad365Page(visitUrl);
                //m_handlerWriteStatus("DoUILogin");
                //Thread.Sleep(10000);
                ////DoLoad365Page();
                //// string loginScript = File.ReadAllText("loginscript.js");

                //BoundingBox rects = null;
                //LOGGED = false;
                //WaitingForLogin = true;
                //ElementHandle inputNameDiv = m_browser.QuerySelectorAsync("input[class='lms-StandardLogin_Username ']").Result;
                //if (inputNameDiv == null)
                //{
                //    ElementHandle loginDiv = m_browser.QuerySelectorAsync("div[class='hm-MainHeaderRHSLoggedOutWide_Login ']").Result;
                //    if (loginDiv == null)
                //        loginDiv = m_browser.QuerySelectorAsync("div[class='lms-LoginButton ']").Result;
                //    await FakeUserAction.Intance.SimClickElement(loginDiv);
                //    //m_browser.Mouse.ClickAsync(1560 , 66);
                //    m_handlerWriteStatus("Clicked Login Button");
                //    Thread.Sleep(3000);
                //}
                //else
                //{
                //    LOGGED = false;
                //    WaitingForLogin = false;
                //    return false;

                //}

                //inputNameDiv = m_browser.QuerySelectorAsync("input[class='lms-StandardLogin_Username ']").Result;
                //var nameEle = inputNameDiv.GetPropertyAsync("value").Result;
                //string username = nameEle.RemoteObject.Value.ToString();
                //if (string.IsNullOrEmpty(username))
                //{
                //    rects = inputNameDiv.BoundingBoxAsync().Result;
                //    await FakeUserAction.Intance.SimClickElement(inputNameDiv);
                //    await m_browser.Keyboard.TypeAsync(Setting.instance.betUsername);
                //}

                //Thread.Sleep(1000);
                //ElementHandle passDiv = m_browser.QuerySelectorAsync("input[class='lms-StandardLogin_Password ']").Result;
                //var passEle = passDiv.GetPropertyAsync("value").Result;
                //string pass = "";
                //if (string.IsNullOrEmpty(pass))
                //{
                //    await FakeUserAction.Intance.SimClickElement(passDiv);
                //    await m_browser.Keyboard.TypeAsync(Setting.instance.betPassword);
                //    Thread.Sleep(3000);
                //}


                //ElementHandle loginBtn = m_browser.QuerySelectorAsync("div[class='lms-LoginButton ']").Result;
                //rects = loginBtn.BoundingBoxAsync().Result;
                //await FakeUserAction.Intance.SimClickElement(loginBtn);
                //Task t = Task.Run(async () => {
                //    try
                //    {
                //        while (WaitingForLogin) Thread.Sleep(500);
                //    }
                //    catch (Exception) { }
                //});

                //TimeSpan ts = TimeSpan.FromMilliseconds(1000 * 25);
                //if (!t.Wait(ts))
                //{
                //    m_handlerWriteStatus("The timeout interval elapsed in DoUILogin of BetController.");
                //    return false;
                //}
                //Thread.Sleep(1000);
                //m_handlerWriteStatus("Login Success!");
                //m_handlerWriteStatus("Page is loaded and waiting for 5 seconds!");
                //Thread.Sleep((int)5 * 1000);
                //await DoClickDlgBox();
            }
            catch
            {

            }
            WaitingForLogin = false;
            return true;
        }

        private void ResolveQRCode()
        {
            m_handlerWriteStatus("ResolveQRCode()");
            for (int i = 0; i < 5; i++)
            {
                bool isShowQRcode = CheckQRCode(1000);
                if (!isShowQRcode)
                {
                    break;
                }
                else
                {
                    ScanQRcode();
                    isShowQRcode = CheckQRCode(1500);
                    if (!isShowQRcode)
                    {
                        Thread.Sleep(2000);
                        break;
                    }
                    else
                        ScanQRcode();
                }
            }
        }

        private bool CheckQRCode(int delay = 100)
        {
            bool bShow = false;
            try
            {
                Thread.Sleep(delay);
                string isFound = ExecuteScript("document.getElementsByClassName(\"atm-AuthenticatorModule\").length", true);
                m_handlerWriteStatus("CheckQRCode() = " + isFound);
                if (int.Parse(isFound) > 0)
                    bShow = true;
            }
            catch { }
            return bShow;
        }

        private  void ScanQRcode()
        {
            try
            {
                Thread.Sleep(2000);
                bool isFound = CheckQRCode(0);
                if (!isFound)
                    return;

                double diffSecs = (timeToSendQRGetRequest - DateTime.Now).TotalSeconds;
                //if (diffSecs < 60 && !string.IsNullOrEmpty(QRGetRefreshToken)) SendQRApi(1, null);

                string result = string.Empty;
                string xcftToken = string.Empty;
                int retryCnt = 5;
                ExecuteScript("getCurrentLocation()", true);
                ExecuteScript("getXcft()", true);
                while (retryCnt-- > 0)
                {
                    Thread.Sleep(500);
                    result = ExecuteScript("JSON.stringify({latitude, longitude})", true);
                    xcftToken = ExecuteScript("xcftRes", true);
                    if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(xcftToken))
                        break;
                }

                JObject betData = new JObject();
                JObject geo = JObject.Parse(result);

                betData["xcft"] = xcftToken;
                betData["lat"] = geo["latitude"].ToString().Replace(",", ".");
                betData["lon"] = geo["longitude"].ToString().Replace(",", ".");
                betData["countryCode"] = Setting.instance.countryCode.ToUpper();
                m_handlerWriteStatus($"latitude : {betData["lat"].ToString()} , longitude : {betData["lon"].ToString()} , countryCode : {Setting.instance.countryCode}");
                string placebetPostContent = JsonConvert.SerializeObject(betData);
                SendQRApi(0, placebetPostContent);
                Thread.Sleep(5000);
            }
            catch { }
        }

        public void SendQRApi(int nMode, string postData)
        {
            try
            {
                if (nMode == 0)
                {
                    JObject betData = JsonConvert.DeserializeObject<JObject>(postData);
                    betData["apiKey"] = Setting.instance.qrAPI;
                    betData["proxy"] = Setting.instance.proxyServer;
                    betData["bet365Username"] = Setting.instance.betUsername;
                    if (betData["countryCode"].ToString() == "ES")
                        betData["host"] = "www.bet365.es";
                    else if (betData["countryCode"].ToString() == "BG")
                        betData["host"] = "www.bet365.com";
                    else if (betData["countryCode"].ToString() == "IT")
                        betData["host"] = "www.bet365.it";
                    else if (betData["countryCode"].ToString() == "BD")
                        betData["host"] = "www.3256871.com";
                    else
                        betData["host"] = "www.bet365.com";


                    string placebetPostContent = JsonConvert.SerializeObject(betData);
                    m_handlerWriteStatus($"QR Post Request: {placebetPostContent}");
                    using (HttpClient newHttpClient = new HttpClient())
                    {
                        HttpResponseMessage qrBet365Response = newHttpClient.PostAsync("https://qrresolver.bettingco.ru/api/QrResolver/GetQrBet365", new StringContent(placebetPostContent, Encoding.UTF8, "application/json")).Result;
                        m_handlerWriteStatus($"QR Post Response StatusCode: {qrBet365Response.StatusCode}");
                        try
                        {
                            string strContent = qrBet365Response.Content.ReadAsStringAsync().Result;
                            m_handlerWriteStatus($"QR Post Response: {strContent}");
                            var qrPostResponse = JsonConvert.DeserializeObject<dynamic>(strContent);
                            if (!string.IsNullOrEmpty(qrPostResponse.refreshToken.ToString()))
                            {
                                QRGetRefreshToken = qrPostResponse.refreshToken.ToString();
                                timeToSendQRGetRequest = DateTime.Now.AddSeconds(40);
                            }
                        }
                        catch { }
                    }
                }
                else if (nMode == 1)
                {
                    using (HttpClient newHttpClient = new HttpClient())
                    {
                        Random rnd = new Random();
                        HttpResponseMessage resp = newHttpClient.GetAsync($"https://qrresolver.bettingco.ru/api/QrResolver/RefreshSession?refreshToken={QRGetRefreshToken}&forcedResolve=false").Result;
                        HttpStatusCode StatusCode = resp.StatusCode;
                        m_handlerWriteStatus($"QRGetRefreshTokene: {QRGetRefreshToken}");

                        string strContent = resp.Content.ReadAsStringAsync().Result;
                        var responseJson = JsonConvert.DeserializeObject<dynamic>(strContent);
                        if (responseJson.status.ToString() == "-1")
                        {
                            m_handlerWriteStatus("QR code session Expired!");
                            timeToSendQRGetRequest = DateTime.MaxValue;
                        }
                        else
                            timeToSendQRGetRequest = DateTime.Now.AddSeconds(rnd.Next(30, 60));
                    }
                }
            }
            catch { }
        }

    }
}
