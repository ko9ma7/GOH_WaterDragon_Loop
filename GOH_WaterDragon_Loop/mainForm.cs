using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Util;
using System.Threading;
using System.Collections;

namespace GOH_WaterDragon_Loop
{
    public partial class mainForm : Form
    {
        String AppName = "LDPlayer";
        Bitmap searchImg;
        int X, Y;
        int Width, Height;

        private delegate void SafeCallDelegate(string text);


        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        public enum WMessages : int
        {
            WM_MOUSEMOVE = 0x200,
            WM_LBUTTONDOWN = 0x201, //Left mousebutton down
            WM_LBUTTONUP = 0x202,  //Left mousebutton up
            WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
            WM_RBUTTONDOWN = 0x204, //Right mousebutton down
            WM_RBUTTONUP = 0x205,   //Right mousebutton up
            WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick
            WM_KEYDOWN = 0x100,  //Key down
            WM_KEYUP = 0x101,   //Key up
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_CHAR = 0x102,     //char
            WM_COMMAND = 0x111
        }

        public static IntPtr NoxFind(string windows)
        {
            IntPtr hw1 = FindWindow(null, windows);
            IntPtr hw2 = FindWindowEx(hw1, IntPtr.Zero, "RenderWindow", "TheRender");
            return hw2;
        }

        public static void NoxClick(IntPtr Id, int X, int Y, bool Check = true)
        {
            //Console.WriteLine("클릭: " + Id + ", " + X + ", " + Y);
            PostMessage(Id, (int)WMessages.WM_LBUTTONDOWN, 1, new IntPtr(Y * 0x10000 + X));
            //Thread.Sleep(1000);
            PostMessage(Id, (int)WMessages.WM_LBUTTONUP, 0, new IntPtr(Y * 0x10000 + X));
        }

        public static void NoxDrag(IntPtr Id, int X, int Y, int to_X, int to_Y)
        {
            Y -= 30;
            to_Y -= 30;
            Console.WriteLine("드래그: " + Id + ", " + X + ", " + Y + ", " + to_X + ", " + to_Y);
            PostMessage(Id, (int)WMessages.WM_LBUTTONDOWN, 1, new IntPtr(Y * 0x10000 + X));
            for (int i = X; i < to_X; i++)
            {
                Console.WriteLine("X: " + i);
                PostMessage(Id, (int)WMessages.WM_LBUTTONDOWN, 1, new IntPtr(to_Y * 0x10000 + i));
                Thread.Sleep(1);
            }
            PostMessage(Id, (int)WMessages.WM_LBUTTONDOWN, 1, new IntPtr(to_Y * 0x10000 + to_X));
            PostMessage(Id, (int)WMessages.WM_LBUTTONUP, 0, new IntPtr(to_Y * 0x10000 + to_X));

        }

        public static void SendKey(IntPtr Id, int key)
        {
            PostMessage(Id, (int)WMessages.WM_KEYDOWN, key, IntPtr.Zero);
        }

        private int ImageSearch_Multi(string Path)
        {
            int i = 0;
            double accuracy;
            ArrayList rectList = new ArrayList(0);
            IntPtr handle = NoxFind(AppName);
            if (handle != IntPtr.Zero)
            {
                //Console.WriteLine("핸들 : " + handle);
                //찾은 플레이어를 바탕으로 Graphics 정보를 가져옵니다.
                Graphics Graphicsdata = Graphics.FromHwnd(handle);

                //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                System.Drawing.Rectangle rect = System.Drawing.Rectangle.Round(Graphicsdata.VisibleClipBounds);

                //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(handle, hdc, 0x2);
                    g.ReleaseHdc(hdc);
                }

                pictureBox1.Image = bmp;
                //Cv2.ImShow("test", OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp));

                // 원본 이미지
                Mat mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
                //OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp)

                // 찾을 이미지 (고양이 얼굴)
                using (Mat temp = new Mat(@"img\" + Path + ".PNG"))
                using (Mat result = new Mat())
                {
                    mat = mat.CvtColor(ColorConversionCodes.RGBA2RGB);

                    // 템플릿 매칭
                    Cv2.MatchTemplate(mat, temp, result, TemplateMatchModes.CCoeffNormed);

                    // 매칭 범위 지정
                    Cv2.Threshold(result, result, 0.8, 1.0, ThresholdTypes.Tozero);

                    while (true)
                    {
                        // 이미지 매칭 범위
                        OpenCvSharp.Point minloc, maxloc;
                        double minval, maxval;
                        Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);

                        var threshold = 0.9;
                        if (maxval >= threshold)
                        {
                            // 검색된 부분 빨간 테두리
                            //Rect rect2 = new Rect(maxloc.X, maxloc.Y, temp.Width, temp.Height);
                            //Cv2.Rectangle(mat, rect2, new OpenCvSharp.Scalar(0, 0, 255), 2);

                            rectList.Add(new Rectangle(maxloc.X, maxloc.Y, temp.Width, temp.Height));

                            //Console.WriteLine("위치 - X = " + maxloc.X + ", Y = " + maxloc.Y + ", Width = " + temp.Width + ", Height = " + temp.Height);

                            // 
                            Rect outRect;
                            Cv2.FloodFill(result, maxloc, new OpenCvSharp.Scalar(0), out outRect, new OpenCvSharp.Scalar(0.1), new OpenCvSharp.Scalar(1.0), FloodFillFlags.Link4);

                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //Cv2.ImShow("template2_show", mat);
                }
                Graphics grp = Graphics.FromImage((System.Drawing.Image)pictureBox1.Image);

                foreach (Rectangle rect3 in rectList)
                {
                    grp.DrawRectangle(new Pen(Color.Red, 2), rect3);
                }

                //Console.WriteLine(i + "개");
            }
            return i;
        }

        private int ImageSearch_(string Path)
        {
            int i = 0;
            double accuracy;
            IntPtr handle = NoxFind(AppName);
            if (handle != IntPtr.Zero)
            {
                //Console.WriteLine("핸들 : " + handle);
                //찾은 플레이어를 바탕으로 Graphics 정보를 가져옵니다.
                Graphics Graphicsdata = Graphics.FromHwnd(handle);

                //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                System.Drawing.Rectangle rect = System.Drawing.Rectangle.Round(Graphicsdata.VisibleClipBounds);

                //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(handle, hdc, 0x2);
                    g.ReleaseHdc(hdc);
                }
                pictureBox1.Image = bmp;
                //Cv2.ImShow("test", OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp));

                // 원본 이미지
                Mat mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
                //OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp)

                // 찾을 이미지 (고양이 얼굴)
                using (Mat temp = new Mat(@"img\" + Path + ".PNG"))
                using (Mat result = new Mat())
                {
                    mat = mat.CvtColor(ColorConversionCodes.RGBA2RGB);

                    // 템플릿 매칭
                    Cv2.MatchTemplate(mat, temp, result, TemplateMatchModes.CCoeffNormed);

                    // 매칭 범위 지정
                    Cv2.Threshold(result, result, 0.8, 1.0, ThresholdTypes.Tozero);

                    //while (true)
                    //{
                    // 이미지 매칭 범위
                    OpenCvSharp.Point minloc, maxloc;
                    double minval, maxval;
                    Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);

                    var threshold = 0.9;
                    if (maxval >= threshold)
                    {
                        // 검색된 부분 빨간 테두리
                        //Rect rect2 = new Rect(maxloc.X, maxloc.Y, temp.Width, temp.Height);
                        //Cv2.Rectangle(mat, rect2, new OpenCvSharp.Scalar(0, 0, 255), 2);

                        //Console.WriteLine("위치 - X = " + maxloc.X + ", Y = " + maxloc.Y + ", Width = " + temp.Width + ", Height = " + temp.Height);

                        X = maxloc.X;
                        Y = maxloc.Y;
                        Width = temp.Width;
                        Height = temp.Height;

                        // 
                        Rect outRect;
                        Cv2.FloodFill(result, maxloc, new OpenCvSharp.Scalar(0), out outRect, new OpenCvSharp.Scalar(0.1), new OpenCvSharp.Scalar(1.0), FloodFillFlags.Link4);

                        i++;
                    }
                    else
                    {
                        //break;
                    }
                    //}

                    //Cv2.ImShow("template2_show", mat);
                }
                
                //Console.WriteLine(i + "개");
            }
            //Rectangle rect3 = new Rectangle(X, Y, Width, Height);
            //Graphics grp = Graphics.FromImage(pictureBox1.Image);
            //grp.DrawRectangle(new Pen(Color.Red, 2), rect3);

            return i;
        }

        private void ImageSearch_Click(string Path)
        {
            if (ImageSearch_(Path) == 1)
            {
                NoxClick(NoxFind(AppName), X + Width / 2, Y + Height / 2);
            }
        }

        Thread thread;

        private void btn_searchWater_Click(object sender, EventArgs e)
        {
            thread = new Thread(new ThreadStart(WaterDragon_Loop));
            thread.Start();
        }

        private void btn_stopThread_Click(object sender, EventArgs e)
        {
            thread.Abort();
        }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //NoxClick(NoxFind(AppName), 1166, 164);
            ImageSearch_Click("테스트");
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            //폼 숨기기
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Opacity = 0;

            Console.Title = "GOH WaterDragon Loop";
            Console.SetWindowPosition(0, 0);
            Console.SetWindowSize(65, 30);

            //Console.WriteLine(ImageSearch_Multi("결과_물용_3성"));

            Console.Write("=-= GOH WaterDragon Loop =-=\n\nPress Any Key.");
            Console.ReadKey();

            WaterDragon_Loop();
        }

        private bool Loop_Search(string Path)
        {
            Console.WriteLine(Path + " 확인중...");
            while (true)
            {
                //Path 있으면
                if (ImageSearch_(Path) != 0) break;
                else Thread.Sleep(2000);
            }
            Console.WriteLine(Path + " 확인 완료.");
            return true;
        }

        private void Loop_Click(string Path)
        {
            if(Loop_Search(Path) == true)
            {
                Thread.Sleep(1000);
                ImageSearch_Click(Path);
                Console.WriteLine(Path + " 클릭.");
            }
        }

        private void Loop_Skip(string Path)
        {
            Console.WriteLine(Path + " 확인중...");
            while (true)
            {
                //Path 있으면
                if (ImageSearch_(Path) != 0) break;
                else
                {
                    NoxClick(NoxFind(AppName), 215, 358);
                    Thread.Sleep(1000);
                }
            }
            Console.WriteLine(Path + " 확인 완료.");
        }

        public static void SendChar(IntPtr Id, char key)
        {
            PostMessage(Id, (int)WMessages.WM_CHAR, key, IntPtr.Zero);
        }
        public static void SendString(IntPtr Id, String str)
        {
            foreach (char i in str)
            {
                SendChar(Id, i);
            }
        }

        public void Gold_(int Count)
        {
            //뽑기_골드_뽑기_이동 클릭
            Loop_Click("뽑기_골드_뽑기_이동");

            for (int i = 0; i < Count; i++)
            {
                //뽑기_골드_뽑기 클릭
                Loop_Click("뽑기_골드_뽑기");

                //뽑기_10+1_연속_뽑기 클릭
                Loop_Click("뽑기_10+1_연속_뽑기");

                //뽑기 스킵
                Loop_Skip("뽑기_닫기");

                //뽑기_닫기 클릭
                Loop_Click("뽑기_닫기");

                int Count_ = i + 1;
                Console.WriteLine("골드_뽑기 " + Count_ + "번 완료");
            }
        }

        private void WaterDragon_Loop()
        {
            for(; ; )
            {
                Console.WriteLine("프로그램 시작.");
                //메뉴_월드맵 클릭
                Loop_Click("메뉴_월드맵");

                //메뉴_월드맵 인지 확인
                Loop_Search("월드맵_확인");

                //월드맵_메인스토리 클릭
                Loop_Click("월드맵_메인스토리");

                int Season = 0;

                Console.WriteLine("현재 시즌 확인.");
                //시즌 확인
                while (true)
                {
                    for(int i = 1; i < 6; i++)
                    {
                        //현재 시즌이 i 면
                        if (ImageSearch_("시즌_" + i) != 0)
                        {
                            Console.WriteLine("현재 시즌 " + i);
                            Season = i;
                            break;
                        }
                    }
                    break;
                }

                switch (Season)
                {
                    //시즌 1이면
                    case 1:
                        ImageSearch_Click("시즌_1_시즌_2");
                        break;

                    case 2:
                        break;

                    //나머지
                    case 3:
                    case 4:
                    case 5:
                        ImageSearch_Click("시즌_3_시즌_4_시즌_2");
                        break;
                }
                Console.WriteLine("시즌 2로 이동.");
                Thread.Sleep(2000);

                //16_2 클릭
                /*Loop_Click("시즌_2_16_2");

                //스테이지_3 클릭
                Loop_Click("시즌2_16_2_스테이지_3");

                Loop_Click("시즌2_16_2_스테이지_3_입장");

                //물용_3성_15마리 얻기 루프

                int WaterDragon_3Star = 0;

                for(int i = 1; ; i ++)
                {
                    //친구_없이_도전 클릭
                    Loop_Click("친구_없이_도전");

                    //시즌2_16_2_스테이지_3_월드맵_확인 확인
                    Loop_Search("시즌2_16_2_스테이지_3_월드맵_확인");

                    //게임 시작
                    Loop_Click("시즌2_16_2_스테이지_3_시작");

                    //재시도 까지 스킵
                    Loop_Skip("시즌2_16_2_스테이지_3_재시도");

                    //물용_3성 확인
                    WaterDragon_3Star += ImageSearch_Multi("결과_물용_3성");
                    Console.WriteLine("시즌2_16_2_스테이지_3 " + i + "번째 시도중... 물용_3성 " + WaterDragon_3Star + "마리 획득.");

                    //물용_3성이 15마리 이상 먹으면
                    if(WaterDragon_3Star >= 1)
                    {
                        //중지
                        break;
                    }
                    else
                    {
                        //재시도 클릭
                        Loop_Click("시즌2_16_2_스테이지_3_재시도");
                    }
                }

                //월드맵 클릭
                Loop_Click("시즌2_16_2_스테이지_3_월드맵");
                */

                //메뉴_뽑기 클릭
                Loop_Click("시즌_2_메뉴_뽑기");

                //골드 뽑기 / 임시로 1번 / 원래 5번
                //Gold_(1);

                //메뉴_캐릭터로 이동
                Loop_Click("뽑기_메뉴_캐릭터");

                //메뉴_캐릭터_확인
                Loop_Search("메뉴_캐릭터_확인");

                //강화 클릭
                Loop_Click("강화_확인");

                //15번 반복
                for(; ; )
                {
                    //강화_정렬_및_필터 클릭
                    Loop_Click("강화_정렬_및_필터");

                    //모두_해제 / 모두_체크 찾기
                    while (true)
                    {
                        //강화_모두_해제 찾기
                        //강화_모두_해제가 있으면
                        if (ImageSearch_("강화_모두_해제") == 1)
                        {
                            Thread.Sleep(1000);
                            break;
                        }
                        //강화_모두_체크 찾기
                        //강화_모두_체크가 있으면
                        if (ImageSearch_("강화_모두_체크") == 1)
                        {
                            Thread.Sleep(1000);
                            ImageSearch_Click("강화_모두_체크");
                            Console.WriteLine("강화_모두_체크 클릭.");
                            break;
                        }
                        Thread.Sleep(2000);
                    }

                    //모두 해제 클릭
                    Loop_Click("강화_모두_해제");

                    //물용_3성 기타 - 지원형 - 물속성 - 3성
                    //기타 클릭
                    Loop_Click("강화_기타");

                    //지원형 클릭
                    Loop_Click("강화_지원형");

                    //물속성 클릭
                    Loop_Click("강화_물속성");

                    //필터 아래로 내리기
                    Loop_Click("강화_필터_내리기");

                    //3등급 클릭
                    Loop_Click("강화_3등급");

                    //필터 적용 클릭
                    Loop_Click("강화_필터_적용");

                    //물용 첫슬롯 클릭
                    Loop_Click("강화_물용_1번_슬롯");

                    //강화 재료 선택
                    Loop_Click("강화_재료_선택");

                    //강화_정렬_및_필터 클릭
                    Loop_Click("강화_정렬_및_필터");

                    //모두_해제 / 모두_체크 찾기
                    while (true)
                    {
                        //강화_모두_해제 찾기
                        //강화_모두_해제가 있으면
                        if (ImageSearch_("강화_모두_해제") == 1)
                        {
                            Thread.Sleep(1000);
                            break;
                        }
                        //강화_모두_체크 찾기
                        //강화_모두_체크가 있으면
                        if (ImageSearch_("강화_모두_체크") == 1)
                        {
                            Thread.Sleep(1000);
                            ImageSearch_Click("강화_모두_체크");
                            Console.WriteLine("강화_모두_체크 클릭.");
                            break;
                        }
                        Thread.Sleep(2000);
                    }

                    //모두 해제 클릭
                    Loop_Click("강화_모두_해제");

                    //필터 위로 올리기
                    Loop_Click("강화_필터_올리기");

                    //재료 - 일반/기타 1/2등급
                    //일반 클릭
                    Loop_Click("강화_일반");

                    //기타 클릭
                    Loop_Click("강화_기타");

                    //필터 아래로 내리기
                    Loop_Click("강화_필터_내리기");

                    //1등급 클릭
                    Loop_Click("강화_1등급");

                    //2등급 클릭
                    Loop_Click("강화_2등급");

                    //필터 적용 클릭
                    Loop_Click("강화_필터_적용");

                    //강화_목록 확인
                    Loop_Search("강화_목록");

                    //강화 재료가 없을때
                    Thread.Sleep(1000);
                    if(ImageSearch_("강화_재료_없음") == 1)
                    {
                        //골드 뽑기 다시
                        //강화_재료_뒤로가기 클릭
                        Loop_Click("강화_재료_뒤로가기");

                        //강화_뒤로가기 클릭
                        Loop_Click("강화_뒤로가기");

                        //강화_목록_뒤로가기 클릭
                        Loop_Click("강화_재료_뒤로가기");

                        //메뉴_뽑기 로 이동
                        Loop_Click("캐릭터_메뉴_뽑기");

                        //골드 뽑기 / 임시로 1번 / 원래 3번
                        Gold_(1);

                        //메뉴_캐릭터로 이동
                        Loop_Click("뽑기_메뉴_캐릭터");

                        //메뉴_캐릭터_확인
                        Loop_Search("메뉴_캐릭터_확인");

                        //강화 클릭
                        Loop_Click("강화_확인");

                        //다시 for문 이어서 continue
                        continue;
                    }
                    //강화 재료가 있을때
                    else
                    {
                        for(int i = 0; i < 5; i++)
                        {
                            Loop_Click("강화_재료");
                        }

                        //강화_재료_확인 클릭
                        Loop_Click("강화_재료_확인");

                        //강화_강화하기 클릭
                        Loop_Click("강화_강화하기");

                        //강화_확인하기 클릭
                        Loop_Click("강화_확인하기");

                        //딜레이
                        Thread.Sleep(2000);

                        //강화 스킵
                        Loop_Skip("강화_확인하기");

                        //강화_맥스 3번 확인
                        for(int i = 0; i < 3; i++)
                        {
                            //강화_맥스 확인
                            //강화_맥스가 있음
                            if(ImageSearch_("강화_맥스") == 1)
                            {
                                //강화_확인하기 클릭
                                Loop_Click("강화_확인하기");

                                //강화 뒤로가기 클릭
                                Loop_Click("강화_뒤로가기");

                                break;
                            }
                            else
                            {
                                continue;
                            }

                            break;
                        }
                        //강화_확인하기 클릭
                        Loop_Click("강화_확인하기");

                    }
                }


                break; //임시 종료
            }
        }

        //NoxDrag(NoxFind(AppName), 25, 877, 537, 877);

        public mainForm()
        {
            InitializeComponent();
        }


    }
}
