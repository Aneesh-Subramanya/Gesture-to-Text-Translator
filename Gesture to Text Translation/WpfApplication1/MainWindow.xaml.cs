using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq; 
using System.Windows; 
using System.Windows.Media; 
using Microsoft.Kinect;
namespace WpfApplication1
{
    
    public partial class MainWindow : Window
    {
        KinectSensor sensor = KinectSensor.KinectSensors[0];



        #region "Variables"
        private const double BodyCenterThickness = 10;

        private const double ClipBoundsThickness = 10;

        private readonly Brush centerPointBrush = Brushes.Blue;

        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        private readonly Brush inferredJointBrush = Brushes.Yellow;
        
        static int raod1 = 0, raod2 = 0, raod3=0,qwe=1;
        
        static DateTime hiDate1, hiDate2, hiDate3, empty, LRWDate1, LRWDate2, RLWDate1, RLWDate2;
        
        static Boolean LRWFlag1=true, LRWFlag2=false, RLWFlag1=true, RLWFlag2=false;

        static string word = "word";
        
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        private DrawingImage imageSource;

        private const double JointThickness = 3;
        
        private DrawingGroup drawingGroup;
        
        private const float RenderWidth = 640.0f;
        
        private const float RenderHeight = 480.0f;

        string anyCommand; 
  
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;


            Unloaded += MainWindow_Unloaded;


        }




        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            sensor.Stop();

        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            //Create a Drawing Group that will be used for Drawing 
            this.drawingGroup = new DrawingGroup();

            //Create an image Source that will display our skeleton
            this.imageSource = new DrawingImage(this.drawingGroup);

            //Display the Image in our Image control
            Image.Source = imageSource;

            try
            {
                //Check if the Sensor is Connected
                if (sensor.Status == KinectStatus.Connected)
                {
                    //Start the Sensor
                    sensor.Start();
                    //Tell Kinect Sensor to use the Default Mode(Human Skeleton Standing) || Seated(Human Skeleton Sitting Down)
                    sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                    //Subscribe to te  Sensor's SkeletonFrameready event to track the joins and create the joins to display on our image control
                    sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
                    //nice message with Colors to alert you if your sensor is working or not
                    Message.Text = "Kinect Ready";
                    Message.Background = new SolidColorBrush(Colors.Green);
                    Message.Foreground = new SolidColorBrush(Colors.White);

                    // Turn on the skeleton stream to receive skeleton frames
                    this.sensor.SkeletonStream.Enable();
                }
                else if (sensor.Status == KinectStatus.Disconnected)
                {
                    Message.Text = "Kinect Sensor is not Connected";
                    Message.Background = new SolidColorBrush(Colors.Orange);
                    Message.Foreground = new SolidColorBrush(Colors.Black);

                }
                else if (sensor.Status == KinectStatus.NotPowered)
                {   Message.Text = "Kinect Sensor is not Powered";
                    Message.Background = new SolidColorBrush(Colors.Red);
                    Message.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[1];

            using (SkeletonFrame skeletonframe = e.OpenSkeletonFrame())
            {
                if (skeletonframe != null)
                {

                    skeletons = new Skeleton[skeletonframe.SkeletonArrayLength];

                    skeletonframe.CopySkeletonDataTo(skeletons);

                    if (sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Default)
                    {
                        DrawStandingSkeletons(skeletons);
                    }
                    else if (sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated)
                    {
                        DrawSeatedSkeletons(skeletons);
                    }
                }

            }


        }



        private void DrawStandingSkeletons(Skeleton[] skeletons)
        {

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                //Draw a Transparent background to set the render size or our Canvas
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);


                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(this.centerPointBrush,
                                           null,
                                           this.SkeletonPointToScreen(skel.Position), BodyCenterThickness, BodyCenterThickness);

                        }

                    }


                }

                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));

            }
        }


        private void DrawSeatedSkeletons(Skeleton[] skeletons)
        {

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                //Draw a Transparent background to set the render size 
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);


                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(this.centerPointBrush,
                                           null,
                                           this.SkeletonPointToScreen(skel.Position), BodyCenterThickness, BodyCenterThickness);

                        }

                    }


                }

                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));

            }
        }



        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>'
        /// 

        private void swipeleft(Skeleton skeleton, JointType joint0, JointType joint1)
        {
            Joint HandLeft = skeleton.Joints[joint0];
            Joint SholderRight = skeleton.Joints[joint1];
            if (HandLeft.TrackingState == JointTrackingState.NotTracked || SholderRight.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            if (HandLeft.TrackingState == JointTrackingState.Inferred && SholderRight.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }
            if (HandLeft.Position.X > SholderRight.Position.X)
            {
                Label1.Content = "Swipe left detected";
            }

        }


        private bool findCross(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3)
        {
            Joint HandRight = skeleton.Joints[joint0];
            Joint HandLeft = skeleton.Joints[joint1];
            Joint ElbowLeft = skeleton.Joints[joint2];
            Joint ElbowRight = skeleton.Joints[joint3];

        

            if (HandRight.TrackingState == JointTrackingState.NotTracked || HandLeft.TrackingState == JointTrackingState.NotTracked || ElbowLeft.TrackingState == JointTrackingState.NotTracked || ElbowRight.TrackingState == JointTrackingState.NotTracked)
            {
                return false;
            }

            if (HandRight.TrackingState == JointTrackingState.Inferred && HandLeft.TrackingState == JointTrackingState.Inferred || ElbowRight.TrackingState == JointTrackingState.Inferred && ElbowLeft.TrackingState == JointTrackingState.Inferred)
            {
                return false;
            }

            if ((!word.Contains("I") && HandRight.Position.X < HandLeft.Position.X) && (ElbowRight.Position.Y < HandLeft.Position.Y) && (ElbowLeft.Position.Y < HandLeft.Position.Y) && (ElbowRight.Position.Y < HandRight.Position.Y) && (ElbowLeft.Position.Y < HandRight.Position.Y) && (ElbowLeft.Position.X < ElbowRight.Position.X))
            {
                Label1.Content += " I";
                word = "I";
            }
            return true;
        }

        private bool findDeadBall(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3)
        {
            Joint HandRight = skeleton.Joints[joint0];
            Joint HandLeft = skeleton.Joints[joint1];
            Joint ElbowLeft = skeleton.Joints[joint2];
            Joint ElbowRight = skeleton.Joints[joint3];



            if (HandRight.TrackingState == JointTrackingState.NotTracked || HandLeft.TrackingState == JointTrackingState.NotTracked || ElbowLeft.TrackingState == JointTrackingState.NotTracked || ElbowRight.TrackingState == JointTrackingState.NotTracked)
            {
                return false;
            }

            if (HandRight.TrackingState == JointTrackingState.Inferred && HandLeft.TrackingState == JointTrackingState.Inferred || ElbowRight.TrackingState == JointTrackingState.Inferred && ElbowLeft.TrackingState == JointTrackingState.Inferred)
            {
                return false;
            }

            if ((!word.Contains("you") && HandRight.Position.X < HandLeft.Position.X) && (ElbowRight.Position.Y > HandLeft.Position.Y) && (ElbowLeft.Position.Y > HandLeft.Position.Y) && (ElbowRight.Position.Y > HandRight.Position.Y) && (ElbowLeft.Position.Y > HandRight.Position.Y) && (ElbowLeft.Position.X < ElbowRight.Position.X))
            {
                Label1.Content += " you";
                word = "you";
            }

            return true;
        }

        private void findclap(Skeleton skeleton, JointType joint0, JointType joint1) // not working
        {
            Joint HandLeft = skeleton.Joints[joint0];
            Joint HandRight = skeleton.Joints[joint1];
            if (HandLeft.TrackingState == JointTrackingState.NotTracked || HandRight.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            if (HandLeft.TrackingState == JointTrackingState.Inferred && HandRight.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            if (Math.Abs(HandLeft.Position.X - HandRight.Position.X)<=0.05 && Math.Abs(HandLeft.Position.Y - HandRight.Position.Y)<=0.05)
            {
                Label1.Content = "Clap Recognized";
            }
        }


        private bool findhorizontalhand(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3, JointType joint4, JointType joint5, JointType joint6)
        {
            Joint ShoulderCenter = skeleton.Joints[joint0];
            Joint ShoulderLeft = skeleton.Joints[joint1];
            Joint ElbowLeft = skeleton.Joints[joint2];
            Joint HandLeft = skeleton.Joints[joint3];
            Joint ShoulderRight = skeleton.Joints[joint4];
            Joint ElbowRight = skeleton.Joints[joint5];
            Joint HandRight = skeleton.Joints[joint6];
            bool left=false, right=false;
            
            //var proc1 = new ProcessStartInfo();
            //proc1.UseShellExecute = true;
            //proc1.WorkingDirectory = @"D:\AS\Academic\Information Technology\3rd Year\6th Semester\Projects\HCI\Working";
            //proc1.FileName = @"D:\AS\Academic\Information Technology\3rd Year\6th Semester\Projects\HCI\Working\cmd.exe";
            //proc1.Verb = "runas";
            

            if (ShoulderCenter.TrackingState == JointTrackingState.NotTracked || ShoulderLeft.TrackingState == JointTrackingState.NotTracked || ElbowLeft.TrackingState == JointTrackingState.NotTracked || HandLeft.TrackingState == JointTrackingState.NotTracked || ShoulderRight.TrackingState == JointTrackingState.NotTracked || ElbowRight.TrackingState == JointTrackingState.NotTracked || HandRight.TrackingState == JointTrackingState.NotTracked)
            {
                return false;
            }

            
            if (ShoulderCenter.TrackingState == JointTrackingState.Inferred && ShoulderLeft.TrackingState == JointTrackingState.Inferred && ElbowLeft.TrackingState == JointTrackingState.Inferred && HandLeft.TrackingState == JointTrackingState.Inferred && ShoulderRight.TrackingState == JointTrackingState.Inferred && ElbowRight.TrackingState == JointTrackingState.Inferred  && HandRight.TrackingState == JointTrackingState.NotTracked)
            {
                return false;
            }
            
            if (Math.Abs(ShoulderLeft.Position.Y - HandLeft.Position.Y)<=0.1 && HandLeft.Position.X< ShoulderLeft.Position.X-0.4&& Math.Abs(HandLeft.Position.Z-ShoulderLeft.Position.Z)<0.1)
            {
                left = true;
                //Label1.Content = "Left arm streching detected";
                //Label1.Content = "" + ShoulderLeft.Position.Y + " "+ HandLeft.Position.Y;
            }
            if (Math.Abs(ShoulderRight.Position.Y - HandRight.Position.Y) <= 0.1 && HandRight.Position.X > ShoulderRight.Position.X + 0.4 && Math.Abs(HandRight.Position.Z - ShoulderRight.Position.Z) < 0.1)
            {
                right = true;
                //Label1.Content = "Right arm streching detected";
            }

            //if(left && right)
            //{
            //    Label1.Content = "Wide";
            //}
            if (!word.Contains("are")&& left)
            {
                Label1.Content += " are";
                word = "are";
                //if(qwe==1)
                //{
                //    anyCommand = ".\as.mp3";
                //    proc1.Arguments = "/c " + anyCommand;
                //    proc1.WindowStyle = ProcessWindowStyle.Hidden;
                //    Process.Start(proc1);
                //    qwe+=1;
                //}

            }
            else if(!word.Contains("am")&& right)
            {
                Label1.Content += " am";
                word = "am";
            }
            else if(Label1.Content.ToString().Contains("am") || Label1.Content.ToString().Contains("are"))
            {
                

            }
            return true;
        }





        private void findPunch(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3)
        {
            Joint ShoulderRight = skeleton.Joints[joint0];
            Joint HandRight = skeleton.Joints[joint1];
            Joint ShoulderLeft = skeleton.Joints[joint2];
            Joint HandLeft = skeleton.Joints[joint3];

            if (ShoulderRight.TrackingState == JointTrackingState.NotTracked || HandRight.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            
            if (ShoulderRight.TrackingState == JointTrackingState.Inferred && HandRight.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            
            
            if ((Math.Abs(ShoulderLeft.Position.Y - HandLeft.Position.Y) <= 0.0500) && (Math.Abs(ShoulderLeft.Position.X - HandLeft.Position.X) <= 0.0500))
            {
                Label1.Content = "Left Punch";

            }
            else if (Math.Abs(ShoulderRight.Position.Y - HandRight.Position.Y)<=0.0500 && Math.Abs(ShoulderRight.Position.X - HandRight.Position.X)<=0.0500)
            {
                Label1.Content = "Right Punch";

            }

        }

        private bool findRaise(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3)
        {
            Joint ShoulderLeft = skeleton.Joints[joint0]; //CHANGED to head
            Joint ShoulderRight = skeleton.Joints[joint1];
            Joint HandLeft = skeleton.Joints[joint2];
            Joint HandRight = skeleton.Joints[joint3];

            bool Left = false, Right = false;

            if (ShoulderLeft.TrackingState == JointTrackingState.NotTracked || ShoulderRight.TrackingState == JointTrackingState.NotTracked || HandLeft.TrackingState == JointTrackingState.NotTracked || HandRight.TrackingState == JointTrackingState.NotTracked)
            {
                return false;
            }

            if (ShoulderLeft.TrackingState == JointTrackingState.Inferred && ShoulderRight.TrackingState == JointTrackingState.Inferred && HandLeft.TrackingState == JointTrackingState.Inferred && HandRight.TrackingState == JointTrackingState.Inferred)
            {
                return false;
            }

            if (ShoulderLeft.Position.Y + 0.4 < HandLeft.Position.Y && Math.Abs(ShoulderLeft.Position.X-HandLeft.Position.X )<0.2 && Math.Abs(ShoulderLeft.Position.Z-HandLeft.Position.Z )<0.2)
            {
                Left = true;
                //Label1.Content = "Left arm above head ..";
                //Label1.Content = Head.Position.Y + " " + HandLeft.Position.Y;
                //return true;
            }

            if (ShoulderRight.Position.Y + 0.4 < HandRight.Position.Y && Math.Abs(ShoulderRight.Position.X-HandRight.Position.X )<0.2 && Math.Abs(ShoulderRight.Position.Z-HandRight.Position.Z )<0.2)
            {
                Right = true;
                //Label1.Content = "Right arm above head ..";
                //Label1.Content = Head.Position.Y + " " + HandRight.Position.Y;
                //return true;
            }

            //if (Left==true && Right==true)
            //{
            //    Label1.Content = "Six";
            //}

            if(Left==true && !word.Contains("doing"))
            {
                Label1.Content += " doing";
                word = "doing";
            }

            else if(Right==true && !word.Contains("how"))
            {
                Label1.Content += " how";
                word = "how";
            }

            return true;

        }

        private bool findWaveLeftRight(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3)
        {
            Joint ShoulderLeft = skeleton.Joints[joint0];
            Joint ShoulderRight = skeleton.Joints[joint1];
            Joint HandLeft = skeleton.Joints[joint2];
            Joint ShoulderCenter = skeleton.Joints[joint3];

            if(HandLeft.Position.X+0.4<ShoulderLeft.Position.X && Math.Abs(HandLeft.Position.Y-ShoulderLeft.Position.Y)<0.20 && Math.Abs(HandLeft.Position.Z-ShoulderLeft.Position.Z)<0.15)
            {
                LRWFlag1 = true;
                LRWDate1 = DateTime.Now;

            }

            if(ShoulderLeft.Position.Z-HandLeft.Position.Z > 0.4 && Math.Abs(HandLeft.Position.X-ShoulderLeft.Position.X)<0.20 && Math.Abs(HandLeft.Position.Y-ShoulderLeft.Position.Y)<0.15 && LRWFlag1 == true && (DateTime.Now-LRWDate1).TotalSeconds < 2)
            {
                LRWFlag2 = true;
                LRWDate2 = DateTime.Now;

            }

            if (HandLeft.Position.X - ShoulderRight.Position.X > 0.1 && Math.Abs(HandLeft.Position.Y - ShoulderRight.Position.Y) < 0.20 && Math.Abs(HandLeft.Position.Z - ShoulderRight.Position.Z) < 0.15 && LRWFlag1 == true && LRWFlag2 == true && (DateTime.Now - LRWDate2).TotalSeconds < 2)
            {
                //Label1.Content = "Four";
                LRWFlag1 = LRWFlag2 = false;
                LRWDate1 = LRWDate2 = empty;

            }
            
            else if(LRWFlag1 && LRWFlag2 && (DateTime.Now-LRWDate2).TotalSeconds>2)
            {
                LRWFlag1 = LRWFlag2 = false;
                LRWDate1 = LRWDate2 = empty;
            }
            
            if(LRWFlag1 && !LRWFlag2 && LRWDate1!=empty && (DateTime.Now-LRWDate1).TotalSeconds>2)
            {
                LRWFlag1 = false;
                LRWDate1 = LRWDate2 = empty;
            }

            if (Label1.Content == "Four" && ((HandLeft.Position.X - ShoulderRight.Position.X < 0.1 && Math.Abs(HandLeft.Position.Y - ShoulderRight.Position.Y) > 0.20 && Math.Abs(HandLeft.Position.Z - ShoulderRight.Position.Z) > 0.15)))
            {
                LRWFlag1 = LRWFlag2 = false;
                LRWDate1 = LRWDate2 = empty;
                //Label1.Content = "";
            }
            return false;
        }

        private bool findWaveRightLeft(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3)
        {
            Joint ShoulderRight = skeleton.Joints[joint0];
            Joint ShoulderLeft = skeleton.Joints[joint1];
            Joint HandRight = skeleton.Joints[joint2];
            Joint ShoulderCenter = skeleton.Joints[joint3];

            if (HandRight.Position.X - 0.4 > ShoulderRight.Position.X && Math.Abs(HandRight.Position.Y - ShoulderRight.Position.Y) < 0.20 && Math.Abs(HandRight.Position.Z - ShoulderRight.Position.Z) < 0.15)
            {
                RLWFlag1 = true;
                RLWDate1 = DateTime.Now;

            }

            if (ShoulderRight.Position.Z - HandRight.Position.Z > 0.4 && Math.Abs(HandRight.Position.X - ShoulderRight.Position.X) < 0.15 && Math.Abs(HandRight.Position.Y - ShoulderRight.Position.Y) < 0.20 && RLWFlag1 == true && (DateTime.Now - RLWDate1).TotalSeconds < 2)
            {
                RLWFlag2 = true;
                RLWDate2 = DateTime.Now;

            }

            if (HandRight.Position.X - ShoulderLeft.Position.X < -0.1 && Math.Abs(HandRight.Position.Y - ShoulderLeft.Position.Y) < 0.20 && Math.Abs(HandRight.Position.Z - ShoulderLeft.Position.Z) < 0.15 && RLWFlag1 == true && RLWFlag2 == true && (DateTime.Now - RLWDate2).TotalSeconds < 2)
            {
                //Label1.Content = "Four";
                RLWFlag1 = RLWFlag2 = false;
                RLWDate1 = RLWDate2 = empty;

            }

            else if (RLWFlag1 && RLWFlag2 && (DateTime.Now - RLWDate2).TotalSeconds > 2)
            {
                RLWFlag1 = RLWFlag2 = false;
                RLWDate1 = RLWDate2 = empty;
            }

            if (RLWFlag1 && !RLWFlag2 && RLWDate1 != empty && (DateTime.Now - RLWDate1).TotalSeconds > 2)
            {
                RLWFlag1 = false;
                RLWDate1 = RLWDate2 = empty;
            }

            if (Label1.Content == "Four" && ((HandRight.Position.X - ShoulderLeft.Position.X > -0.1 && Math.Abs(HandRight.Position.Y - ShoulderLeft.Position.Y) > 0.2 && Math.Abs(HandRight.Position.Z - ShoulderLeft.Position.Z) > 0.15)))
            {
                RLWFlag1 = RLWFlag2 = false;
                RLWDate1 = RLWDate2 = empty;
                //Label1.Content = "";
            }
            return false;
        }

        private bool findWaveLeft(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2)
        {
            Joint ShoulderLeft = skeleton.Joints[joint0];
            Joint HandRight = skeleton.Joints[joint1];
            Joint ShoulderCenter = skeleton.Joints[joint2];

            

            if (ShoulderLeft.TrackingState == JointTrackingState.NotTracked || HandRight.TrackingState == JointTrackingState.NotTracked || ShoulderCenter.TrackingState == JointTrackingState.NotTracked)
            {
                return false;
            }

            
            if (ShoulderLeft.TrackingState == JointTrackingState.Inferred && HandRight.TrackingState == JointTrackingState.Inferred && ShoulderCenter.TrackingState == JointTrackingState.Inferred)
            {
                return false;
            }

            
            if (ShoulderLeft.Position.X > HandRight.Position.X && ShoulderCenter.Position.Y > HandRight.Position.Y)
            {
                Label1.Content = "Right Wave Successful";
                return true;
            }
            return false;
        }

        /*
        private bool findHi(Skeleton skeleton, JointType joint0, JointType joint1)
        {
            Joint ElbowLeft = skeleton.Joints[joint0];
            Joint HandLeft = skeleton.Joints[joint1];
            //Label1.Content = "" + ElbowLeft.Position.X + " " + HandLeft.Position.X;
            int[] arr;
            //Label1.Content = "" + DateTime.Now.TimeOfDay;
            arr=new int[2];
            if (HandLeft.Position.X < ElbowLeft.Position.X)
            {
                raod1=1;
                if (hiDate1 == empty)
                {
                    hiDate1 = DateTime.Now;
                    //Label1.Content="sdsdfsdgdgdg "+d1;
                }
            }
            if (HandLeft.Position.X > ElbowLeft.Position.X && raod1==1)
            {
                raod2=1;
                if (hiDate2 == empty)
                {
                    hiDate2 = DateTime.Now;
                    //Label1.Content="sdsdfsdgdgdg "+i+" "+j;
                }
            }
            if (HandLeft.Position.X < ElbowLeft.Position.X && raod1==1 && raod2==1)
            {
                raod3 = 1;
                if (hiDate3 == empty)
                {
                    hiDate3 = DateTime.Now;
                    //Label2.Content=""+d1+" "+d2+" "+d3;
                    //Label3.Content = "" + ((d2 - d1).TotalSeconds<=3) + " " + ((d3 - d2).TotalSeconds<=3);
                    if ((hiDate2 - hiDate1).TotalSeconds <= 3 && (hiDate3 - hiDate2).TotalSeconds <= 3)
                        Label1.Content = "Hi!";
                    raod1 = raod2 = raod3 = 0;
                    hiDate1=hiDate2=hiDate3=empty;
                    //Label1.Content="sdsdfsdgdgdg "+i+" "+j;
                }
            }
            return false;
        }
         */

        private bool findHi(Skeleton skeleton, JointType joint0, JointType joint1)
        {
            Joint ElbowLeft = skeleton.Joints[joint0];
            Joint HandLeft = skeleton.Joints[joint1];
            //Label1.Content = "" + ElbowLeft.Position.X + " " + HandLeft.Position.X;
            int[] arr;
            //Label1.Content = "" + DateTime.Now.TimeOfDay;
            arr = new int[2];
            if (HandLeft.Position.X < ElbowLeft.Position.X && HandLeft.Position.Y > ElbowLeft.Position.Y && HandLeft.Position.Z <= ElbowLeft.Position.Z)
            {
                raod1 = 1;
                if (hiDate1 == empty)
                {
                    hiDate1 = DateTime.Now;
                    //Label1.Content="sdsdfsdgdgdg "+d1;
                }
            }
            if (HandLeft.Position.X > ElbowLeft.Position.X && HandLeft.Position.Y > ElbowLeft.Position.Y && HandLeft.Position.Z <= ElbowLeft.Position.Z && raod1 == 1)
            {
                raod2 = 1;
                if (hiDate2 == empty)
                {
                    hiDate2 = DateTime.Now;
                    //Label1.Content="sdsdfsdgdgdg "+i+" "+j;
                }
            }
            if (HandLeft.Position.X < ElbowLeft.Position.X && HandLeft.Position.Y > ElbowLeft.Position.Y && HandLeft.Position.Z <= ElbowLeft.Position.Z && raod1 == 1 && raod2 == 1)
            {
                raod3 = 1;
                if (hiDate3 == empty)
                {
                    hiDate3 = DateTime.Now;
                    //Label2.Content=""+d1+" "+d2+" "+d3;
                    //Label3.Content = "" + ((d2 - d1).TotalSeconds<=3) + " " + ((d3 - d2).TotalSeconds<=3);
                    if (!word.Contains("hi") && (hiDate2 - hiDate1).TotalSeconds <= 3 && (hiDate3 - hiDate2).TotalSeconds <= 3)
                    {
                        Label1.Content += " Hi!";
                        word = "hi";
                    }
                    raod1 = raod2 = raod3 = 0;
                    hiDate1 = hiDate2 = hiDate3 = empty;
                    //Label1.Content="sdsdfsdgdgdg "+i+" "+j;
                }
            }

            if (Label1.Content == "Hi!" && (HandLeft.Position.X > ElbowLeft.Position.X || HandLeft.Position.Y < ElbowLeft.Position.Y || HandLeft.Position.Z > ElbowLeft.Position.Z))
            {
                //Label1.Content = "";
                raod1 = raod2 = raod3 = 0;
                hiDate1 = hiDate2 = hiDate3 = empty;
            }
            return false;
        }

        private bool findLegByeLeft(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3)
        {
            Joint HipLeft = skeleton.Joints[joint0];
            Joint KneeLeft = skeleton.Joints[joint1];
            Joint AnkleLeft = skeleton.Joints[joint2];
            Joint HandLeft = skeleton.Joints[joint3];

            if(!word.Contains("today") && Math.Abs(HipLeft.Position.X-KneeLeft.Position.X)<0.35 && Math.Abs(HipLeft.Position.Y-KneeLeft.Position.Y)<0.2 && HipLeft.Position.Z > KneeLeft.Position.Z && Math.Abs(HandLeft.Position.X-KneeLeft.Position.X)<0.15 && Math.Abs(HandLeft.Position.Y-KneeLeft.Position.Y)<0.15 && (HipLeft.Position.Z>=HandLeft.Position.Z && HandLeft.Position.Z>=KneeLeft.Position.Z))
            {
                Label1.Content += " today";
                word = "today";
                return true;
            }

            else if (Label1.Content.ToString().Contains("today"))
            {
                
                return false;
            }
            return false;
        }

        private bool findLegByeRight(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2, JointType joint3)
        {
            Joint HipRight = skeleton.Joints[joint0];
            Joint KneeRight = skeleton.Joints[joint1];
            Joint AnkleRight = skeleton.Joints[joint2];
            Joint HandRight = skeleton.Joints[joint3];

            if (!word.Contains("alright") && Math.Abs(HipRight.Position.X - KneeRight.Position.X) < 0.35 && Math.Abs(HipRight.Position.Y - KneeRight.Position.Y) < 0.2 && HipRight.Position.Z > KneeRight.Position.Z && Math.Abs(HandRight.Position.X - KneeRight.Position.X) < 0.15 && Math.Abs(HandRight.Position.Y - KneeRight.Position.Y) < 0.15 && (HipRight.Position.Z >= HandRight.Position.Z && HandRight.Position.Z >= KneeRight.Position.Z))
            {
                Label1.Content += " alright";
                word = "alright";
                return true;
            }

            else if (Label1.Content.ToString().Contains("Bye"))
            {
                
                return false;
            }
            return false;
        }

        private bool findShortRunLeft(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2)
        {
            Joint ShoulderLeft = skeleton.Joints[joint0];
            Joint ElbowLeft = skeleton.Joints[joint1];
            Joint HandLeft = skeleton.Joints[joint2];

            if(Math.Abs(ElbowLeft.Position.Y-ShoulderLeft.Position.Y)<0.20 && Math.Abs(HandLeft.Position.X-ShoulderLeft.Position.X)<0.1 && HandLeft.Position.Y-ShoulderLeft.Position.Y>0 && HandLeft.Position.Y-ShoulderLeft.Position.Y<0.2 && Math.Abs(HandLeft.Position.Z-ShoulderLeft.Position.Z)<0.15)
            {
                Label1.Content = "Short Run";
                return true;
                
            }

            else if(Label1.Content=="Short Run")
            {
                Label1.Content = "";
            }
            return false;
        }

        private bool findShortRunRight(Skeleton skeleton, JointType joint0, JointType joint1, JointType joint2)
        {
            Joint ShoulderRight = skeleton.Joints[joint0];
            Joint ElbowRight = skeleton.Joints[joint1];
            Joint HandRight = skeleton.Joints[joint2];

            if (Math.Abs(ElbowRight.Position.Y - ShoulderRight.Position.Y) < 0.20 && Math.Abs(HandRight.Position.X - ShoulderRight.Position.X) < 0.1 && HandRight.Position.Y - ShoulderRight.Position.Y > 0 && HandRight.Position.Y - ShoulderRight.Position.Y < 0.2 && Math.Abs(HandRight.Position.Z - ShoulderRight.Position.Z) < 0.15)
            {
                Label1.Content = "Short Run";
                return true;

            }

            else if (Label1.Content == "Short Run")
            {
                Label1.Content = "";
            }
            return false;
        }


        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {

            // Render Torso


            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);
            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            //Gestures
            findRaise(skeleton, JointType.ShoulderLeft, JointType.ShoulderRight, JointType.HandLeft, JointType.HandRight);
            findCross(skeleton, JointType.HandRight, JointType.HandLeft, JointType.ElbowLeft, JointType.ElbowRight);
            findhorizontalhand(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.HandLeft, JointType.ShoulderRight, JointType.ElbowRight, JointType.HandRight);
            findWaveLeftRight(skeleton, JointType.ShoulderLeft, JointType.ShoulderRight, JointType.HandLeft, JointType.ShoulderCenter);
            findWaveRightLeft(skeleton, JointType.ShoulderRight, JointType.ShoulderLeft, JointType.HandRight, JointType.ShoulderCenter);
            //findPunch(skeleton, JointType.ShoulderRight, JointType.HandRight, JointType.ShoulderLeft, JointType.HandLeft);
            //findclap(skeleton, JointType.HandLeft, JointType.HandRight);
            findHi(skeleton, JointType.ElbowLeft, JointType.HandLeft);
            findLegByeLeft(skeleton, JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.HandLeft);
            findLegByeRight(skeleton, JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.HandRight);
            findShortRunLeft(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.HandLeft);
            findShortRunRight(skeleton, JointType.ShoulderRight, JointType.ElbowRight, JointType.HandRight);
            findDeadBall(skeleton, JointType.HandRight, JointType.HandLeft, JointType.ElbowLeft, JointType.ElbowRight);
            //Gestures
            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }

        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked || joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            
            if (joint0.TrackingState == JointTrackingState.Inferred && joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;

            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }


        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

    }
}