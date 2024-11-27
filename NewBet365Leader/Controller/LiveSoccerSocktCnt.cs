using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using HtmlAgilityPack;
using FirefoxBet365Placer.Json;
using FirefoxBet365Placer.Constants;
using System.Windows.Forms;
using H.Socket.IO;

namespace FirefoxBet365Placer.Controller
{

    public class LiveSoccerSocktCnt
    {
        private SocketIoClient _socket = null;
        private onWriteStatusEvent m_handlerWriteStatus;
        private onProcNewTipEvent m_handlerNewTip;

        private string _KEY = "BCDE000019940000010900000ABCD000"; //replace with your key
        private string _IV = "1994A0B0C0D0E109"; //replace with your IV


        public LiveSoccerSocktCnt(onWriteStatusEvent onWriteStatus, onProcNewTipEvent onProcNewTip)
        {
            m_handlerWriteStatus = onWriteStatus;
            m_handlerNewTip = onProcNewTip;
        }

        public void CloseSocket()
        {
            _socket.DisconnectAsync();
        }

        async public void startListening()
        {
            Setting setting = Setting.instance;
            if (_socket != null)
            {
                await _socket.DisconnectAsync();
                _socket = null;
            }

            _socket = new SocketIoClient();
            _socket.Connected +=(sender, e) =>
            {
                m_handlerWriteStatus("Live soccer socket has been connected!");
                _socket.Emit("loginRequest", "MG5py9Uf+D3duMTqjFxbpuNyu0V2gxZ+WTMNK3I7h4D2+9wbpzb1S/eoe5nUkUX1He4I2HMzQm8t8ENU7ash8vuuO5VBz9PZrmuyf4myXWGieJkni3pgj5m6ZZp5w1zHXINIoHy0psM/R5W+FE27iUT0OlRAPOARbToS/J6/aqgpsPYmMBsVNC0Uv4nTNwel1JIREP8YpTgF9ieEqxue6IACznmL9D46OGvKdaNsftuD1KTt+0pOGc/OzbzYuVedi9SZ5QpkXmq70mLY2HGQavNETl2GV4+0P3GJ7Uit5Ufz8MVnZU/MQ80quTkDIvWJqGa40ouT57jda6t+ljWh9A==");
            };


            _socket.On("candidate", (data) =>
            {
                try
                {
                    if (GlobalConstants.state != State.Running) return;
                    if (GlobalConstants.validationState != ValidationState.SUCCESS) return;
                    if (GetBoolVal("tipster.enabled") != true) return;

                    List<BetItem> betList = new List<BetItem>();
                    string receivedContent = data.ToString();
                    receivedContent = Utils.DecryptMessage(receivedContent, _KEY, _IV);
                    //m_handlerWriteStatus(receivedContent);
                    PlaceBetCandidate[] candidates = JsonConvert.DeserializeObject<PlaceBetCandidate[]>(receivedContent);
                    for (int i = 0; i < candidates.Length; i++)
                    {
                        if (candidates[i].strSlaveCode != "B365" && candidates[i].strMasterCode != "B365") continue;
                        BetData bet365Data = new BetData();
                        BetData otherData = new BetData();
                        if (candidates[i].strSlaveCode == "B365")
                        {
                            bet365Data = candidates[i].betSlave;
                            otherData = candidates[i].betMaster;
                        }
                        else if (candidates[i].strMasterCode == "B365")
                        {
                            bet365Data = candidates[i].betMaster;
                            otherData = candidates[i].betSlave;
                        }
                        if (bet365Data.dOdds <= otherData.dReverseOdds) continue;

                        BetItem betitem = new BetItem();
                        betitem.match = candidates[i].home + " - " + candidates[i].away;
                        betitem.league = candidates[i].league;
                        betitem.source = SOURCE.TIPSTER;
                        betitem.tipster = "jlivesoccer";
                        betitem.Leader = "jlivesoccer";
                        betitem.value = 100 * (bet365Data.dOdds - otherData.dReverseOdds) / otherData.dReverseOdds;

                        betitem.pick = bet365Data.strMarketName;
                        betitem.softbookie = otherData.siteCode;
                        betitem.handicap = bet365Data.getCorrectLine();
                        betitem.pin_odds = otherData.dOdds;
                        betitem.pin_reverseodds = otherData.dReverseOdds;
                        betitem.strScore = candidates[i].strScore;
                        betitem.strTime = candidates[i].strTime;
                        betitem.dprofit = candidates[i].dProfit;

                        string strBS = string.Format("pt=N#o={0}#f={1}#fp={2}#so=0#c={3}#sa=SA_STR#oto=2#st=#ust=#fb=0.00#tr=#||", Utils.ToFractions(bet365Data.dOdds), bet365Data.eventId, bet365Data.sectionId, 1);
                        if (string.IsNullOrEmpty(bet365Data.strHandicap))
                        {
                            strBS = string.Format("pt=N#o={0}#f={1}#fp={2}#so=0#c={3}#ln={4}#sa=SA_STR#oto=2#st=#ust=#fb=0.00#tr=#||", Utils.ToFractions(bet365Data.dOdds), bet365Data.eventId, bet365Data.sectionId, 1, bet365Data.getCorrectLine());
                        }
                        betitem.bs = strBS;
                        betitem.runnerId = bet365Data.sectionId;
                        betitem.odds = bet365Data.dOdds;



                        string strTipsterSetting = GetStringVal("tipster.leader");
                        if (!strTipsterSetting.ToLower().Contains(betitem.Leader.ToLower()) && !strTipsterSetting.Contains("all")) continue;
                        if (betitem.odds < GetDoubleVal("tipster.odds.min")) continue;
                        double stakePercent = GetDoubleVal("tipster.stakepercent");

                        double maxStake = GetDoubleVal("tipster.maxstake");
                        if (betitem.value < maxStake) continue;
                        double myStake = stakePercent;
                        if (GetBoolVal("tipster.stake.fixed"))
                            myStake = stakePercent;
                        else
                            myStake = betitem.stake * stakePercent / 100;

                        foreach (string tipster in strTipsterSetting.ToLower().Split(','))
                        {
                            try
                            {
                                if (tipster.Contains(betitem.Leader.ToLower()))
                                {
                                    string stakePart = tipster.Split(':')[1].Trim();
                                    stakePercent = Utils.ParseToDouble(stakePart.Substring(0, stakePart.Length - 1));
                                    if (stakePart.EndsWith("$"))
                                        myStake = stakePercent;
                                    else
                                        myStake = betitem.stake * stakePercent / 100;
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }
                        betitem.stake = myStake;

                        betList.Add(betitem);
                        //m_handlerWriteStatus("new bet has been detected");
                    }
                    m_handlerNewTip(betList);
                }
                catch (Exception ex)
                {
                    m_handlerWriteStatus("Exception in candidate message: " + ex.ToString());
                }
            });

            _socket.ConnectAsync(new Uri("https://www.duoduo66888.com:5002"));
        }


        public string GetStringVal(string keyName)
        {
            try
            {
                string ketValue = Setting.instance.BotSetting[keyName].ToString();
                return ketValue;
            }
            catch
            {
            }
            return string.Empty;
        }

        public double GetDoubleVal(string keyName)
        {
            try
            {
                string ketValue = Setting.instance.BotSetting[keyName].ToString();
                return Utils.ParseToDouble(ketValue);
            }
            catch
            {
            }
            return -1;
        }

        public bool GetBoolVal(string keyName)
        {
            try
            {
                return Setting.instance.BotSetting[keyName] == true;
            }
            catch
            {
            }
            return false;
        }
    }
}
