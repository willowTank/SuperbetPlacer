using FirefoxBet365Placer.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirefoxBet365Placer.Controller
{
    public class FakeUserAction
    {
        private static FakeUserAction _instance = null;
        private static onWriteStatusEvent m_handlerWriteStatus;

        public static FakeUserAction Intance
        {
            get
            {
                return _instance;
            }
        }

        private Thread _workThread;
        private bool isRunning = false;
        private Point _mouseCurrPos;
        private bool isStopped = true;
        private int _xOffset;
        private int _yOffset;

        public FakeUserAction(onWriteStatusEvent writeStatus)
        {
            m_handlerWriteStatus = writeStatus;
            _mouseCurrPos = new Point(Utils.GetRandValue(200, 600), Utils.GetRandValue(200, 600));
        }
        public static void CreateInstance(onWriteStatusEvent writeStatus)
        {
            _instance = new FakeUserAction(writeStatus);
        }

        public bool SimClickPlaceBet()
        {
            try
            {
                Point endPos = new Point(
                                    Setting.instance.innerWidth / 2 + Utils.GetRandValue(30, 60),
                                    Setting.instance.innerHeight - Utils.GetRandValue(30, 60)
                                    );
                SimWinMouse(_mouseCurrPos, endPos);
                SimClick();
            }
            catch
            {

            }
            return false;
        }

        /*
        WindMouse algorithm.Calls the move_mouse kwarg with each new step.
        Released under the terms of the GPLv3 license.
        G_0 - magnitude of the gravitational fornce
        W_0 - magnitude of the wind force fluctuations
        M_0 - maximum step size (velocity clip threshold)
        D_0 - distance where wind behavior changes from random to damped
        */
        public bool SimWinMouse(Point sPos, Point ePos, int maxPoints = -1, int timestamp = -1, int cpDelta = 1, decimal G_0 = 9, decimal W_0 = 3, decimal M_0 = 30, decimal D_0 = 12)
        {
            try
            {
                decimal sqrt3 = (decimal)Math.Sqrt(3);
                decimal sqrt5 = (decimal)Math.Sqrt(5);
                decimal current_x = sPos.x, current_y = sPos.y;
                Random rnd = new Random();
                decimal dist, v_x = 0, v_y = 0, W_x = 0, W_y = 0;
                dist = (decimal)(Math.Sqrt(Math.Pow((double)(ePos.x - sPos.x), 2) + Math.Pow((double)(ePos.y - sPos.y), 2)));
                while (dist >= 1)
                {

                    decimal W_mag = Math.Min(W_0, dist);
                    if (dist >= D_0)
                    {
                        W_x = W_x / sqrt3 + (decimal)(2 * rnd.NextDouble() - 1) * W_mag / sqrt5;
                        W_y = W_y / sqrt3 + (decimal)(2 * rnd.NextDouble() - 1) * W_mag / sqrt5;
                    }
                    else
                    {
                        W_x /= sqrt3;
                        W_y /= sqrt3;
                        if (M_0 < 3)
                            M_0 = (decimal)(rnd.NextDouble() * 3 + 3);
                        else
                            M_0 = M_0 / sqrt5;
                    }
                    v_x += W_x + G_0 * (ePos.x - sPos.x) / dist;
                    v_y += W_y + G_0 * (ePos.y - sPos.y) / dist;
                    decimal v_mag = (decimal)(Math.Sqrt(Math.Pow((double)(v_x), 2) + Math.Pow((double)(v_y), 2)));
                    if (v_mag > M_0)
                    {
                        decimal v_clip = M_0 / 2 + (decimal)rnd.NextDouble() * M_0 / 2;
                        v_x = (v_x / v_mag) * v_clip;
                        v_y = (v_y / v_mag) * v_clip;
                    }
                    sPos.x += v_x;
                    sPos.y += v_y;
                    int move_x = (int)(Math.Round(sPos.x));
                    int move_y = (int)(Math.Round(sPos.y));
                    if (current_x != move_x || current_y != move_y)
                    {
                        Point currentPos = new Point(move_x, move_y);
                        MoveCursor(currentPos, Utils.GetRandValue(1, 2));
                    }
                    dist = (decimal)(Math.Sqrt(Math.Pow((double)(ePos.x - sPos.x), 2) + Math.Pow((double)(ePos.y - sPos.y), 2)));

                }
            }
            catch (Exception ex)
            {
                int a = 1;
            }
            return true;
        }


        public bool SimClick(bool pauseAfterMouseUp = true)
        {
            m_handlerWriteStatus(string.Format("Clicked ({0}, {1})", _mouseCurrPos.x, _mouseCurrPos.y));
            Win32.MouseEvent(MouseEventFlags.LeftDown);
            Thread.Sleep(Utils.GetRandValue(20, 60));
            Win32.MouseEvent(MouseEventFlags.LeftUp);
            if (pauseAfterMouseUp) Thread.Sleep(100);
            return true;
        }

        public bool SimWheel(decimal delta)
        {
            if(delta > 0)
            {
                decimal deltaStep = 30;
                while (delta > deltaStep)
                {
                    Win32.MouseEvent(MouseEventFlags.Wheel, -deltaStep);
                    delta -= 30;
                }
                Win32.MouseEvent(MouseEventFlags.Wheel, -deltaStep + delta);
            }
            else
            {
                delta = Math.Abs(delta);
                decimal deltaStep = 30;
                while (delta > deltaStep)
                {
                    Win32.MouseEvent(MouseEventFlags.Wheel, deltaStep);
                    delta -= 30;
                }
                Win32.MouseEvent(MouseEventFlags.Wheel, deltaStep - delta);
            }
            
            return true;
        }

        public bool SimRandomMouseMove(bool addSleep = true)
        {
            try
            {
                int innerWidth = Setting.instance.innerWidth;
                int innerHeight = Setting.instance.innerHeight;

                int startX = innerWidth / 16;
                int startY = innerHeight / 24;
                int endX = innerWidth * 15 / 16;
                int endY = innerHeight * 23 / 24;

                Point endPos = new Point(
                                    Utils.GetRandValue(startX, endX),
                                    Utils.GetRandValue(10) > 5 ? endY - Utils.GetRandValue(startY, endY) : Utils.GetRandValue(startY, endY)
                               );
                //m_handlerWriteStatus(string.Format("from ({0}, {1}) to ({2}, {3})", _mouseCurrPos.x, _mouseCurrPos.y, endPos.x, endPos.y));
                SimMouseMoveTo(endPos);
                if(addSleep) Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus("Exception in SimRandomMouseMove : " + ex.ToString());
            }

            return true;
        }

        async public Task<bool> AdjustElementPositionWithMouse(Point elPos)
        {
            decimal deltaX = 0, deltaY = 0;
            bool viewportAdjust = false;
            // If the top of the node is less than 0
            if (elPos.y <= 120)
            {
                // deltaY always positive
                deltaY = elPos.y - Setting.instance.innerHeight / 2 - Utils.GetRandValue(120, 200);
                viewportAdjust = true;
            }
            else if (elPos.y >= Setting.instance.innerHeight)
            {
                // If the botton is beyond
                deltaY = elPos.y - Setting.instance.innerHeight / 2 + Utils.GetRandValue(50, 200);
                //deltaY = elPos.y + 30 + 30 - Setting.instance.innerHeight /2;
                viewportAdjust = true;
            }
            m_handlerWriteStatus(string.Format("width: {2}, height: {3}, deltaX : {0}, deltaY : {1}", deltaX, deltaY, Setting.instance.innerWidth, Setting.instance.innerHeight));
            if (viewportAdjust)
            {
                elPos.y = elPos.y - deltaY;
                this.SimMouseMoveTo(elPos);
                //await _page.Mouse.WheelAsync(deltaX, deltaY);
                Thread.Sleep(Utils.GetRandValue(100, 400));
            }
            return viewportAdjust;
        }

        public bool SimMouseMoveTo(Point endPos, int maxPoints = -1, int timestamp = -1, int cpDelta = 1)
        {
            Point closeToEndPos = new Point(
                                        endPos.x + Utils.GetRandValue(5, 30, true),
                                        endPos.y + Utils.GetRandValue(5, 30, true)
                                      );
            SimWinMouse(_mouseCurrPos, closeToEndPos, maxPoints, timestamp, cpDelta);

            // The last pos must correction
            MoveCursor(endPos, Utils.GetRandValue(1, 2));
            return true;
        }

        async private Task<bool> SimMouseMove(Point startPos, Point endPos, int maxPoints = -1, int timestamp = -1, int cpDelta = 1)
        {

            List<Point> points = MouseMovementTrack(
                                    startPos,
                                    endPos,
                                    maxPoints > 0 ? maxPoints : Utils.GetRandValue(45, 70),
                                    cpDelta
                                 );
            SetCursorPos(startPos);
            for (int n = 0; n < points.Count; n += 1)
            {
                Point point = points[n];
                MoveCursor(point, Utils.GetRandValue(1, 2));
                timestamp = (timestamp > 0 ? timestamp : (Utils.GetRandValue(300, 800) / points.Count));
                Thread.Sleep(timestamp);
            }
            return true;
        }

        private void MoveCursor(Point endPos, int step) 
        {
            Point sPos = new Point(_mouseCurrPos.x, _mouseCurrPos.y);
            decimal xDiff = endPos.x - sPos.x;
            decimal yDiff = endPos.y - sPos.y;
            if (xDiff > 0)
            {
                for (int i = (int)sPos.x; i <= endPos.x; i= i + step) 
                {
                    decimal x = sPos.x + (i - sPos.x);
                    decimal y = sPos.y + (i - sPos.x) * (yDiff/xDiff);
                    SetCursorPos(new Point(x, y));
                }

            }
            else if(xDiff < 0 )
            {
                for (int i = (int)sPos.x; i >= endPos.x; i = i - step)
                {
                    decimal x = sPos.x + (i - sPos.x);
                    decimal y = sPos.y + (i - sPos.x) * (yDiff / xDiff);
                    SetCursorPos(new Point(x, y));
                }
            }
            SetCursorPos(endPos);
        }

        private void SetCursorPos(Point pos)
        {
            //POINT p = pos.Win32Pos;
            //Win32.ClientToScreen(Setting.instance.WindowHandle, ref p);
            Win32.SetCursorPosition((int)pos.x + _xOffset, (int)(pos.y + (decimal)Setting.instance.heightDiff + +_yOffset));
            //Win32.SetCursorPosition((int)pos.x, (int)(pos.y));
            _mouseCurrPos = pos;
        }

        //public bool Removebet()
        //{
        //    try
        //    {
        //        string posResult = ExecuteScript("getPosActivityBtn()", true);
        //        if (posResult == "false") return false;
        //        JObject betPosition = JsonConvert.DeserializeObject<JObject>(posResult);
        //        decimal x = decimal.Parse(betPosition.SelectToken("x").ToString());
        //        decimal y = decimal.Parse(betPosition.SelectToken("y").ToString());
        //        FakeUserAction.Intance.SimMouseMoveTo(new Point(x, y));
        //        FakeUserAction.Intance.SimClick();
        //    }
        //    catch
        //    {
        //    }
        //    ExecuteScript("BetSlipLocator.betSlipManager.getBetCount() > 0 ? BetSlipLocator.betSlipManager.deleteAllBets() : 0");
        //    return true;
        //}

        private List<Point> MouseMovementTrack(Point startPos, Point endPos, int maxPoints = 30, int cpDelta = 1)
        {
            List<Point> lstPoints = new List<Point>();

            List<int> nums = new List<int>();
            int maxNum = 0;
            int moveStep = 1;
            for (int n = 0; n < maxPoints; ++n)
            {
                nums.Add(maxNum);
                if (n < maxPoints * 1 / 10)
                {
                    moveStep += Utils.GetRandValue(60, 100);
                }
                else if (n >= maxPoints * 9 / 10)
                {
                    moveStep -= Utils.GetRandValue(60, 100);
                    moveStep = Math.Max(20, moveStep);
                }

                maxNum += moveStep;
            }

            Point p1 = new Point(startPos.x, startPos.y);
            Point cp1 = new Point(
                                    (startPos.x + endPos.x) / 2 + Utils.GetRandValue(30, 100, true) * cpDelta,
                                    (startPos.y + endPos.y) / 2 + Utils.GetRandValue(30, 100, true) * cpDelta
                                 );
            Point cp2 = new Point(
                                    (startPos.x + endPos.x) / 2 + Utils.GetRandValue(30, 100, true) * cpDelta,
                                    (startPos.y + endPos.y) / 2 + Utils.GetRandValue(30, 100, true) * cpDelta
                                 );
            Point p2 = new Point(endPos.x, endPos.y);
            foreach (int num in nums)
            {
                lstPoints.Add(Utils.ThreeBezier((decimal)num / (decimal)maxNum, p1, cp1, cp2, p2));
            }
            return lstPoints;
        }

        public void startHumanThread()
        {
            if (isRunning)
            {
                m_handlerWriteStatus("FakeUserAction is already started!");
                return;
            }
            //stopHumanThread();
            Task.Run(() =>
            {
                while (!isStopped) Thread.Sleep(100);
                if (isRunning)
                {
                    m_handlerWriteStatus("FakeUserAction is already started!");
                    return;
                }
                isRunning = true;
                _workThread = new Thread(humanThreadFunc);
                _workThread.Start();
            });
        }

        public void stopHumanThread()
        {
            try
            {
                isRunning = false;
                if (_workThread != null) _workThread.Abort();
            }
            catch
            {

            }
            m_handlerWriteStatus("FakeUserAction - STOP!");
        }

        public bool taskHumanActivity()
        {
            try
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
                Win32.GetWindowRect(Setting.instance.WindowHandle, out rect);
                _xOffset = rect.X;
                _yOffset = rect.Y;

                SimRandomMouseMove();
                //int randValue = Utils.GetRandValue(10);
                if (_mouseCurrPos.y > 440 && _mouseCurrPos.y < 440) 
                {
                    SimClick();
                }
            }
            catch (Exception ex)
            {
                m_handlerWriteStatus(ex.ToString());
            }
            return true;
        }

        async public void humanThreadFunc()
        {
            m_handlerWriteStatus("FakeUserAction - START!");
            isStopped = false;
            while (isRunning)
            {
                try
                {
                    SimRandomMouseMove();
                    int randValue = Utils.GetRandValue(10);

                }
                catch (Exception ex)
                {
                    //m_handlerWriteStatus(ex.ToString());
                }
                Thread.Sleep(Utils.GetRandValue(300, 1200) * 3);
            }
            isStopped = true;
        }

    }
}
