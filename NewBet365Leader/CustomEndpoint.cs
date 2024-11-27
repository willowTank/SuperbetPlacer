using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FirefoxBet365Placer.Json;
using System.Threading;
using System.IO;
using FirefoxBet365Placer.Constants;
using System.Net.Http.Headers;

namespace FirefoxBet365Placer
{
    public class CustomEndpoint
    {

        public static BrowserProfile getBrowserProfile(string username)
        {
            BrowserProfile profile = new BrowserProfile();
            try
            {
                HttpClient httpClient = getHttpClient();
                string url = string.Format("http://37.187.91.64:5002/interface/getProfile/{0}", Math.Abs(username.GetHashCode()));
                HttpResponseMessage ipResponse = httpClient.GetAsync(url).Result;
                string strProfile = ipResponse.Content.ReadAsStringAsync().Result;
                profile = JsonConvert.DeserializeObject<BrowserProfile>(strProfile);
                return profile;
            }
            catch
            {
            }
            return null;
        }

        public static void UpdateProxySetting(int proxyPort, string countryCode)
        {
            try
            {
                if (string.IsNullOrEmpty(Setting.instance.proxyServer)) return ;
                HttpClient httpClient = getHttpClient();
                dynamic jsonObject = new JObject();
                jsonObject.proxy = new JObject();
                jsonObject.proxy.carrier = "";
                jsonObject.proxy.city = "";
                jsonObject.proxy.country = countryCode.ToLower();
                jsonObject.proxy.state = "";
                jsonObject.proxy.preset = "session_long";

                var payloadPost = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                string url = string.Format("http://{0}:22999/api/proxies/{1}", Setting.instance.ProxyUrl, proxyPort);
                HttpResponseMessage ipResponse = httpClient.PutAsync(url, payloadPost).Result;
                //ipResponse.EnsureSuccessStatusCode();
            }
            catch
            {
            }
        }

        public static void RefreshProxyIP(int proxyPort)
        {
            try
            {
                if (string.IsNullOrEmpty(Setting.instance.proxyServer)) return;
                HttpClient httpClient = getHttpClient();
                string url = string.Format("http://{0}:22999/api/refresh_sessions/{1}", Setting.instance.ProxyUrl, proxyPort);
                HttpResponseMessage ipResponse = httpClient.PostAsync(url, null).Result;
                //ipResponse.EnsureSuccessStatusCode();
            }
            catch
            {
            }
        }

        public static string getPlaceHeaders()
        {
            try
            {
                int retry = 5;
                while (--retry > 0)
                {
                    HttpClient client = getHttpClient();
                    string urlForToken = "http://89.40.6.53:6002/interface/placeheaders";
                    HttpResponseMessage ipResponse = client.GetAsync(urlForToken).Result;
                    ipResponse.EnsureSuccessStatusCode();
                    string strContent = ipResponse.Content.ReadAsStringAsync().Result;
                    dynamic jsContent = JsonConvert.DeserializeObject<dynamic>(strContent);
                    return strContent;
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public static List<MatchUpInfo> getMatchUpData()
        {
            try
            {
                int retry = 5;
                while (--retry > 0)
                {
                    HttpClient client = getHttpClient();
                    string urlForToken = "http://23.19.58.112:8888/interface/bet365matchup";
                    HttpResponseMessage ipResponse = client.GetAsync(urlForToken).Result;
                    ipResponse.EnsureSuccessStatusCode();
                    string strContent = ipResponse.Content.ReadAsStringAsync().Result;
                    MatchUpInfo[] raceArray = JsonConvert.DeserializeObject<MatchUpInfo[]>(strContent);
                    return raceArray.ToList();
                }
            }
            catch (Exception)
            {
            }
            return new List<MatchUpInfo>();
        }

        public static List<RaceItem> getDogRaceData()
        {
            try
            {
                int retry = 5;
                while (--retry > 0)
                {
                    HttpClient client = getHttpClient();
                    string urlForToken = "http://37.187.91.64:5002/interface/bet365DogData";
                    HttpResponseMessage ipResponse = client.GetAsync(urlForToken).Result;
                    ipResponse.EnsureSuccessStatusCode();
                    string strContent = ipResponse.Content.ReadAsStringAsync().Result;
                    RaceItem[] raceArray = JsonConvert.DeserializeObject<RaceItem[]>(strContent);
                    return raceArray.ToList();
                }
            }
            catch (Exception)
            {
            }
            return new List<RaceItem>();
        }

        public static string getRequest(string url)
        {
            try
            {
                HttpClient client = getHttpClient();
                HttpResponseMessage ipResponse = client.GetAsync(url).Result;
                ipResponse.EnsureSuccessStatusCode();
                string content = ipResponse.Content.ReadAsStringAsync().Result;
                return content;
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        public static string postRequest(string url, string payload)
        {
            try
            {
                HttpClient client = getHttpClient();
                var payloadPost = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage ipResponse = client.PostAsync(url, payloadPost).Result;
                ipResponse.EnsureSuccessStatusCode();
                string content = ipResponse.Content.ReadAsStringAsync().Result;
                return content;
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        public static string getEventId(string eventName)
        {
            try
            {
                HttpClient client = getHttpClient();
                dynamic payload = new JObject();
                payload.eventName = eventName;
                var payloadPost = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage ipResponse = client.PostAsync("http://91.121.70.201:9002/interface/getEventId", payloadPost).Result;
                ipResponse.EnsureSuccessStatusCode();
                string content = ipResponse.Content.ReadAsStringAsync().Result;
                return content;
            }
            catch (Exception ex)
            {
                int a = 1;
            }
            return "";
        }
        public static Account getAllocatedUser(int botIndex)
        {
            try
            {
                int retryCount = 5;
                while (--retryCount > 0)
                {
                    try
                    {
                        HttpClient client = getHttpClient();
                        HttpResponseMessage ipResponse = client.GetAsync(string.Format("{1}/admin/account/getAllocatedUser/{0}", botIndex, Setting.instance.serverAddr)).Result;
                        ipResponse.EnsureSuccessStatusCode();
                        string content = ipResponse.Content.ReadAsStringAsync().Result;
                        Account myAccount = JsonConvert.DeserializeObject<Account>(content);
                        return myAccount;
                    }
                    catch (Exception ex)
                    {
                        int a = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                int a = 1;
            }
            return null;
        }

        public static void sendNewUserTerm(string serverUrl, string payload)
        {
            try
            {
                HttpClient client = getHttpClient();
                var payloadPost = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage ipResponse = client.PostAsync(serverUrl + "/interface/userterm", payloadPost).Result;
                ipResponse.EnsureSuccessStatusCode();
                string content = ipResponse.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                int a = 1;
            }
        }


        public static string getProxyURL(string browserId)
        {
            string proxyURL = string.Empty;
            try
            {
                HttpClient client = getHttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://api.gologin.com/browser/" + browserId),
                    Headers =
                        {
                            { "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MWZiYmQ0MmYwYWI3YjBlMmFjY2JkYjIiLCJ0eXBlIjoiZGV2Iiwiand0aWQiOiI2MWZiYmZhNzVlZjRmMDU1ZDAyMjNjYzUifQ.AuvRnCRzhgobHSWegGjE_BaUX1vhjWTRjqcRp7zSAfE" },
                            { "Accept", "application/json" },
                            { "User-Agent", "insomnia/2021.7.2" },
                            { "Host", "api.gologin.com" }
                        }
                };
                var response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                string content = response.Content.ReadAsStringAsync().Result;
                dynamic profileData = JsonConvert.DeserializeObject<dynamic>(content);
                string proxyEnabled = profileData.proxyEnabled.ToString();
                if(proxyEnabled.ToLower() == "true")
                {
                    string mode = profileData.proxy.mode.ToString();
                    string host = profileData.proxy.host.ToString();
                    string port = profileData.proxy.port.ToString();
                    proxyURL = string.Format("{0}://{1}:{2}", mode, host, port);
                }
            }
            catch
            {

            }
            return proxyURL;
        }

        public static string LaunchBrowser(onWriteStatusEvent onWriteStatus)
        {
            try
            {
                HttpClient httpClient = getHttpClient();
                HttpResponseMessage httpResp = null;
                string puppeteerUrl = string.Empty;
                if (Setting.instance.solutionType == (int)SOLUTION.MULTILOGIN)
                {
                    httpResp = httpClient.GetAsync("http://127.0.0.1:13028/api/v1/profile/start?automation=true&puppeteer=true&profileId=" + Setting.instance.profileId).Result;
                    string strContent = httpResp.Content.ReadAsStringAsync().Result;
                    dynamic jsContent = JsonConvert.DeserializeObject<dynamic>(strContent);
                    puppeteerUrl = jsContent.value.ToString();
                }
                else if(Setting.instance.solutionType == (int)SOLUTION.VMLOGIN)
                {
                    httpResp = httpClient.GetAsync("http://127.0.0.1:35000/api/v1/profile/start?automation=true&profileId=" + Setting.instance.profileId).Result;
                    string strContent = httpResp.Content.ReadAsStringAsync().Result;
                    onWriteStatus(strContent);
                    dynamic jsContent = JsonConvert.DeserializeObject<dynamic>(strContent);
                    puppeteerUrl = jsContent.value.ToString();
                    Thread.Sleep(10 * 1000);
                }
                else if (Setting.instance.solutionType == (int)SOLUTION.GOLOGIN)
                {
                    string urlStartProfile = string.Format("http://localhost:{0}/interface/start/{1}", Setting.instance.numAgentPort, Setting.instance.profileId);
                    onWriteStatus(urlStartProfile);
                    httpResp = httpClient.GetAsync(urlStartProfile).Result;
                    string strContent = httpResp.Content.ReadAsStringAsync().Result;
                    onWriteStatus(strContent);
                    dynamic jsContent = JsonConvert.DeserializeObject<dynamic>(strContent);
                    puppeteerUrl = jsContent.wsUrl.ToString();
                    Thread.Sleep(2 * 1000);
                }
                else
                {
                    httpResp = httpClient.GetAsync("http://localhost:35000/automation/launch/puppeteer/" + Setting.instance.profileId).Result;
                    string strContent = httpResp.Content.ReadAsStringAsync().Result;
                    dynamic jsContent = JsonConvert.DeserializeObject<dynamic>(strContent);
                    puppeteerUrl = jsContent.puppeteerUrl.ToString();
                }
                return puppeteerUrl;
            }
            catch
            {

            }
            return string.Empty;
        }

        public static void ExitAgent()
        {
            try
            {
                HttpClient client = CustomEndpoint.getHttpClient();
                HttpResponseMessage respMessage = client.GetAsync(string.Format("http://localhost:{0}/interface/exit", Setting.instance.numAgentPort)).Result;
            }
            catch
            {

            }
        }

        public static string UpdateLang()
        {
            try
            {
                HttpClient client = getHttpClient();
                // Update language
                var request = new HttpRequestMessage
                {
                    Method = new HttpMethod("PATCH"),
                    RequestUri = new Uri("https://api.gologin.com/browser/" + Setting.instance.profileId + "/language"),
                    Headers =
                        {
                            { "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqd3RpZCI6IjYyMWUyMzAyM2RlZjhjMTJiZmMxMDk3ZiIsInR5cGUiOiJ1c2VyIiwic3ViIjoiNjFmYmJkNDJmMGFiN2IwZTJhY2NiZGIyIn0.gEegoTWo1LdQ_iHJwRNntfzB6CpF6q3NvA-9SSTJr4E" },
                        },
                    Content = new StringContent("{\"language\":\"es-ES,es;q=0.9\"}")
                    {
                        Headers =
                            {
                                ContentType = new MediaTypeHeaderValue("application/json")
                            }
                    }
                };
                HttpResponseMessage response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                string body = response.Content.ReadAsStringAsync().Result;
                return body;
            }
            catch
            {

            }
            return string.Empty;
        }

        public static string UpdateProfile()
        {
            try
            {
                HttpClient client = getHttpClient();
                if (Setting.instance.solutionType == (int)SOLUTION.GOLOGIN)
                {
                    var request = new HttpRequestMessage
                    {
                        Method = new HttpMethod("PATCH"),
                        RequestUri = new Uri("https://api.gologin.com/browser/fingerprints"),
                        Headers =
                        {
                            { "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqd3RpZCI6IjYyMWUyMzAyM2RlZjhjMTJiZmMxMDk3ZiIsInR5cGUiOiJ1c2VyIiwic3ViIjoiNjFmYmJkNDJmMGFiN2IwZTJhY2NiZGIyIn0.gEegoTWo1LdQ_iHJwRNntfzB6CpF6q3NvA-9SSTJr4E" },
                        },
                        Content = new StringContent("{\"browsersIds\":[\"" + Setting.instance.profileId + "\"]}")
                        {
                            Headers =
                            {
                                ContentType = new MediaTypeHeaderValue("application/json")
                            }
                        }
                    };
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    string body = response.Content.ReadAsStringAsync().Result;
                    
                    // Update language
                    request = new HttpRequestMessage
                    {
                        Method = new HttpMethod("PATCH"),
                        RequestUri = new Uri("https://api.gologin.com/browser/" + Setting.instance.profileId + "/language"),
                        Headers =
                        {
                            { "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqd3RpZCI6IjYyMWUyMzAyM2RlZjhjMTJiZmMxMDk3ZiIsInR5cGUiOiJ1c2VyIiwic3ViIjoiNjFmYmJkNDJmMGFiN2IwZTJhY2NiZGIyIn0.gEegoTWo1LdQ_iHJwRNntfzB6CpF6q3NvA-9SSTJr4E" },
                        },
                        Content = new StringContent("{\"language\":\"es-ES,es;q=0.9\"}")
                        {
                            Headers =
                            {
                                ContentType = new MediaTypeHeaderValue("application/json")
                            }
                        }
                    };
                    response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    body = response.Content.ReadAsStringAsync().Result;

                    // Update Canvas Noise
                    request = new HttpRequestMessage
                    {
                        Method = new HttpMethod("GET"),
                        RequestUri = new Uri("https://api.gologin.com/browser/" + Setting.instance.profileId),
                        Headers =
                        {
                            { "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqd3RpZCI6IjYyMWUyMzAyM2RlZjhjMTJiZmMxMDk3ZiIsInR5cGUiOiJ1c2VyIiwic3ViIjoiNjFmYmJkNDJmMGFiN2IwZTJhY2NiZGIyIn0.gEegoTWo1LdQ_iHJwRNntfzB6CpF6q3NvA-9SSTJr4E" },
                        },
                    };
                    response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    string profileBody = response.Content.ReadAsStringAsync().Result;

                    dynamic jsonProfile =  JsonConvert.DeserializeObject<dynamic>(profileBody);
                    jsonProfile.canvas.mode = "noise";
                    request = new HttpRequestMessage
                    {
                        Method = new HttpMethod("PUT"),
                        RequestUri = new Uri("https://api.gologin.com/browser/" + Setting.instance.profileId + "?updateNoises=false"),
                        Headers =
                        {
                            { "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqd3RpZCI6IjYyMWUyMzAyM2RlZjhjMTJiZmMxMDk3ZiIsInR5cGUiOiJ1c2VyIiwic3ViIjoiNjFmYmJkNDJmMGFiN2IwZTJhY2NiZGIyIn0.gEegoTWo1LdQ_iHJwRNntfzB6CpF6q3NvA-9SSTJr4E" },
                        },
                        Content = new StringContent(jsonProfile.ToString())
                        {
                            Headers =
                            {
                                ContentType = new MediaTypeHeaderValue("application/json")
                            }
                        }
                    };
                    response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();

                    return body;
                }

                if (Setting.instance.solutionType != (int)SOLUTION.INCOGNITION) return string.Empty;
                HttpResponseMessage ipResponse = client.GetAsync("http://localhost:35000/profile/all").Result;
                ipResponse.EnsureSuccessStatusCode();
                string content = ipResponse.Content.ReadAsStringAsync().Result;
                dynamic profileData = JsonConvert.DeserializeObject<dynamic>(content);

                JObject profileItem = null;
                foreach (var item in profileData.profileData)
                {
                    string tmpBrowserId = item["general_profile_information"]["browser_id"].ToString();
                    if (tmpBrowserId == Setting.instance.profileId)
                        profileItem = item;
                }

                string browser_id = profileItem["general_profile_information"]["browser_id"].ToString();
                string profile_name = profileItem["general_profile_information"]["profile_name"].ToString();

                Random rndObject = new Random();
                int randomIndex = rndObject.Next(0, 100);
                bool isMac = false;
                string userAgent = Constants.Constants.macUserAgents[0];
                if (randomIndex > 50)
                {
                    isMac = true;
                    userAgent = Constants.Constants.macUserAgents[randomIndex % Constants.Constants.macUserAgents.Length];
                }
                else
                {
                    userAgent = Constants.Constants.winUserAgents[randomIndex % Constants.Constants.winUserAgents.Length];
                }
                userAgent = Constants.Constants.macUserAgents[randomIndex % Constants.Constants.macUserAgents.Length];
                isMac = true;
                string screenResolution = Constants.Constants.screenResolutions[randomIndex % Constants.Constants.screenResolutions.Length];
                // Update Profile with proxy
                profileData = new JObject();
                profileData["profile_browser_id"] = browser_id;
                profileData["general_profile_information"] = profileItem["general_profile_information"];
                profileData["general_profile_information"]["simulated_operating_system"] = isMac ? "Mac" : "Windows";
                profileData["Navigator"] = new JObject();
                profileData["Navigator"]["user_agent"] = userAgent;
                profileData["Navigator"]["screen_resolution"] = screenResolution;
                profileData["Navigator"]["platform"] = isMac ? "MacIntel" : "Win32";

                string strProfileData = profileData.ToString();
                strProfileData = strProfileData.Replace("\n", "").Replace("\r", "").Replace("\"", "\\\"");
                var payloadPost = new StringContent("{\"profileData\":\"" + strProfileData + "\"}", Encoding.UTF8, "application/json");
                ipResponse = client.PostAsync("http://localhost:35000/profile/update", payloadPost).Result;
                ipResponse.EnsureSuccessStatusCode();
                string strUpdateResult = ipResponse.Content.ReadAsStringAsync().Result;
            }
            catch
            {

            }
            return string.Empty;
        }

        public static string allocateProxy()
        {
            if (Setting.instance.solutionType != (int)SOLUTION.INCOGNITION) return string.Empty;
            try
            {
                try
                {
                    HttpClient client = getHttpClient();
                    HttpResponseMessage ipResponse = client.GetAsync("http://localhost:35000/profile/all").Result;
                    ipResponse.EnsureSuccessStatusCode();
                    string content = ipResponse.Content.ReadAsStringAsync().Result;
                    dynamic profileData = JsonConvert.DeserializeObject<dynamic>(content);

                    JObject profileItem = null;
                    foreach (var item in profileData.profileData)
                    {
                        string tmpBrowserId = item["general_profile_information"]["browser_id"].ToString();
                        if(tmpBrowserId == Setting.instance.profileId)
                            profileItem = item;
                    }
                    
                    if(profileItem == null) return string.Empty;

                    string browser_id   = profileItem["general_profile_information"]["browser_id"].ToString();
                    string profile_name = profileItem["general_profile_information"]["profile_name"].ToString();

                    profileData = new JObject();
                    profileData["profile_browser_id"] = browser_id;
                    profileData["general_profile_information"] = profileItem["general_profile_information"];
                    
                    if(!string.IsNullOrEmpty(Setting.instance.proxyServer))
                    {
                        profileData["Proxy"] = new JObject();
                        profileData["Proxy"]["connection_type"] = "HTTP proxy";
                        profileData["Proxy"]["proxy_url"] = Setting.instance.proxyServer;
                        profileData["Proxy"]["proxy_rotating"] = "0";
                    }

                    if(!string.IsNullOrEmpty(Setting.instance.proxyUser) && !string.IsNullOrEmpty(Setting.instance.proxyPass))
                    {
                        profileData["Proxy"]["proxy_username"] = Setting.instance.proxyUser;
                        profileData["Proxy"]["proxy_password"] = Setting.instance.proxyPass;
                    }

                    profileData["WebRTC"] = new JObject();
                    profileData["WebRTC"]["local_ip"] = "";
                    profileData["WebRTC"]["public_ip"] = "";
                    profileData["WebRTC"]["behavior"] = "Real";
                    profileData["WebRTC"]["set_external_ip"] = true;

                    string strProfileData = profileData.ToString();
                    strProfileData = strProfileData.Replace("\n", "").Replace("\r", "").Replace("\"", "\\\"");
                    var payloadPost = new StringContent("{\"profileData\":\"" + strProfileData + "\"}", Encoding.UTF8, "application/json");
                    ipResponse = client.PostAsync("http://localhost:35000/profile/update", payloadPost).Result;
                    ipResponse.EnsureSuccessStatusCode();
                    string strUpdateResult = ipResponse.Content.ReadAsStringAsync().Result;
                    return strUpdateResult + " : " + profile_name;
                }
                catch (Exception ex)
                {
                    int a = 1;
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }


        public static HttpClient getHttpClient(string proxy = "")
        {
            HttpClientHandler handler;

            if (!string.IsNullOrEmpty(proxy) && proxy != null)
            {
                var proxyURI = new Uri(string.Format("{0}{1}", "http://", proxy));
                WebProxy proxyItem;
                var useAuth = false;
                /*
                var credentials = new NetworkCredential(Setting.instance.proxyUsername, Setting.instance.proxyPassword);
                proxyItem = new WebProxy(proxyURI, true, null, credentials);
                useAuth = true;
                */
                proxyItem = new WebProxy(proxyURI, false);
                handler = new HttpClientHandler()
                {
                    Proxy = proxyItem,
                    UseProxy = true,
                    PreAuthenticate = useAuth,
                    UseDefaultCredentials = !useAuth,
                };
            }
            else
            {
                handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
            }
            HttpClient httpClientEx = new HttpClient(handler);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            /*
            ServicePointManager.DefaultConnectionLimit = 200;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            */
            httpClientEx.Timeout = new TimeSpan(0, 0, 360);
            httpClientEx.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html, application/xhtml+xml, application/xml; q=0.9, image/webp, */*; q=0.8");
            httpClientEx.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36");
            httpClientEx.DefaultRequestHeaders.ExpectContinue = false;
            return httpClientEx;
        }
    }
}
