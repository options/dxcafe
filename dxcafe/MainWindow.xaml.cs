using Microsoft.ProjectOxford.Face;
using OpenCvSharp;
using ServiceHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoFrameAnalyzer;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using Microsoft.ProjectOxford.Face.Contract;

namespace dxcafe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public const string faceKey = "7d63e81223db4b9ab41f064a1a8a609f";
        private FaceServiceClient faceClient = new FaceServiceClient(faceKey);
        private readonly FrameGrabber<LiveCameraResult> grabber = new FrameGrabber<LiveCameraResult>();
        private readonly CascadeClassifier localFaceDetector = new CascadeClassifier();

        private static readonly ImageEncodingParam[] jpegParams =
        {
            new ImageEncodingParam(ImwriteFlags.JpegQuality, 60)
        };

        private bool _fuseClientRemoteResults;
        private LiveCameraResult _latestResultsToDisplay = null;


        public MainWindow()
        {
            InitializeComponent();
            grabber.AnalysisFunction = FacesAnalysisFunction;
            grabber.NewFrameProvided += Grabber_NewFrameProvided;
            grabber.NewResultAvailable += Grabber_NewResultAvailable;
            grabber.TriggerAnalysisOnInterval(TimeSpan.FromMilliseconds(5000));

            localFaceDetector.Load("Data/haarcascade_frontalface_alt2.xml");

        }

        // recognited face
        private void Grabber_NewResultAvailable(object sender, FrameGrabber<LiveCameraResult>.NewResultEventArgs e)
        {
            string age = string.Empty;
            string gender = string.Empty;
            string id = string.Empty;

            age = e.Analysis.Faces.FirstOrDefault()?.FaceAttributes.Age.ToString();
            gender = e.Analysis.Faces.FirstOrDefault()?.FaceAttributes.Gender.ToString();
            id = e.Analysis.Faces.FirstOrDefault()?.FaceId.ToString();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBox.Text += $"Face Detected, Age={age}, Gender={gender}, id={id}\n";
            }));
        }

        // 
        private async Task<LiveCameraResult> FacesAnalysisFunction(VideoFrame frame)
        {
            Face[] faces = null;

            try
            {
                // Encode image. 
                var jpg = frame.Image.ToMemoryStream(".jpg", jpegParams);
                // Submit image to API. 
                var attrs = new List<FaceAttributeType> { FaceAttributeType.Age,
                FaceAttributeType.Gender, FaceAttributeType.HeadPose };
                faces = await faceClient.DetectAsync(jpg, returnFaceAttributes: attrs);

                //"personGroupId": "36fef99e-f1a6-42e2-843e-b9fcee2b56a0",
                //"name": "ISV",
                //"userData": "a61934e8-d8ec-477c-b6a5-a715ed314bd6"
                //"personid" {50eabdcb-4dd9-4101-bf7c-40f0c7863b24}"


                if (faces != null & faces.Length > 0)
                {
                    string personGroupId = "36fef99e-f1a6-42e2-843e-b9fcee2b56a0";
                    var persons = await faceClient.IdentifyAsync(personGroupId, new Guid[] { faces[0].FaceId });

                    if (persons != null && persons.Length > 0)
                    {
                        if (persons[0].Candidates != null && persons[0].Candidates.Length > 0)
                        {
                            Person person = await faceClient.GetPersonAsync(personGroupId, persons[0].Candidates[0].PersonId);
                            await Dispatcher.BeginInvoke((Action)(() =>
                            {
                                TextBox.Text += $"Person Detected, Name={person.Name}\n";
                                UpdatePerson(person.Name, (int)faces[0].FaceAttributes.Age, faces[0].FaceAttributes.Gender );
                            }));
                        }
                    }
                }
            }
            catch (Microsoft.ProjectOxford.Face.FaceAPIException ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return new LiveCameraResult { Faces = faces };
        }

        private async void UpdatePerson(string name, int age, string gender)
        {
            Team.Text = "ISV";
            Name.Text = name;
            switch (name)
            {
                case "Myung Shin Kim":
                    picture.Source = new BitmapImage(new Uri(@"/images/m.jpg", UriKind.Relative));
                    Gender.Text = "Male";
                    Age.Text = "45";
                    break;
                case "Bong Joo Kim":
                    picture.Source = new BitmapImage(new Uri(@"/images/b.jpg", UriKind.Relative));
                    Gender.Text = "Female";
                    Age.Text = "20";
                    break;

                case "Yi Hyun Kim":
                    picture.Source = new BitmapImage(new Uri(@"/images/y.jpg", UriKind.Relative));
                    Gender.Text = "Female";
                    Age.Text = "25";
                    break;
            }

            ScoredMenu sm = await MenuRecommender.GetTopScoredMenu(name, age, gender == "male" ? dxcafe.Gender.Male : dxcafe.Gender.Femail);
            SortedDictionary<string, double> list = sm.GetSortedMenuList();

            SortedList<double, KeyValuePair<string, double>> sortedList = new SortedList<double, KeyValuePair<string, double>>();
            foreach(var each in list)
            {
                sortedList.Add(each.Value, each);
            }

            /*
             *     <TextBlock x:Name="no1text" Text="변함없이 " FontSize="50"></TextBlock>
                            </Button>
                            <Button BorderThickness="0" x:Name="no2" HorizontalContentAlignment="Left" Background="Transparent">
                            <TextBlock x:Name="no2text" Text="소중한 " FontSize="45"></TextBlock>
                            </Button>
                            <Button BorderThickness="0" x:Name="no3" HorizontalContentAlignment="Left" Background="Transparent">
                            <TextBlock x:Name="no3text" Text="오늘도 " FontSize="40"></TextBlock>
                            </Button>
                            <Button BorderThickness="0" x:Name="no4" HorizontalContentAlignment="Left" Background="Transparent">
                            <TextBlock x:Name="no4text" Text="가끔은 " FontSize="35"></TextBlock>
                            </Button>
                            <Button BorderThickness="0" x:Name="no5" HorizontalContentAlignment="Left" Background="Transparent">
                            <TextBlock x:Name="no5text" Text="도전! " FontSize="30"></TextBlock>*/
            string[] prefix = 
            {
                "변함없이 ", "소중한 ", "오늘도 ", "가끔은 ", "도전! "
            };
            TextBlock[] tb = new TextBlock[]
            {
                no1text, no2text, no3text, no4text, no5text
            };
            var reverseList = sortedList.Reverse();
            for (int i = 0; i < reverseList.Count() ; i++)
            {
                tb[i].Text = prefix[i] + "\"" + reverseList.ToArray()[i].Value.Key + "\"";
            }
        }


        // new frame was arrived
        private void Grabber_NewFrameProvided(object sender, FrameGrabber<LiveCameraResult>.NewFrameEventArgs e)
        {
            var rects = localFaceDetector.DetectMultiScale(e.Frame.Image);

            Dispatcher.BeginInvoke((Action)(() =>
            {
                //if (rects != null & rects.Length >= 1)
                //{
                //    FaceRectangle.Margin = new Thickness(rects[0].Left, rects[0].Top, 0, 0);
                //    FaceRectangle.Width = rects[0].Width;
                //    FaceRectangle.Height = rects[0].Height;
                //    FaceRectangle.Visibility = Visibility.Visible;
                //}

                Image.Source = e.Frame.Image.ToBitmapSource();
            }));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Title.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Hidden;

            await grabber.StartProcessingCameraAsync();

        }

        private async void order_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("주문이 완료되었습니다.");
            await grabber.StopProcessingAsync();
            StartButton.Visibility = Visibility.Visible;
        }
    }
}
