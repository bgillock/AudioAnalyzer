using System;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

/* Copyright (c) 2008-2014 DI Zimmermann Stephan (stefan.zimmermann@tele2.at)
 *   
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

namespace GraficDisplay
{
    using GraphLib;

    public partial class MainForm : Form
    {

        private int NumGraphs = 2;
        private bool DrawDots = true;
        private String CurExample = "NORMAL";
        private String CurColorSchema = "BLACK";
        //private PrecisionTimer.Timer mTimer = null;
        private DateTime lastTimerTick = DateTime.Now;
        private String ReferenceFileName = @"C:\Users\bgill\Desktop\Sound\Music\Parachutes - CDRip [master].flac";
        private String CompareFileName = @"C:\Users\bgill\Desktop\Sound\Music\Parachutes [S_LMS;P_ROC;1].wav";

        public MainForm()
        {           
            InitializeComponent();
            
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.None;
            
            CalcDataGraphs();
             
            display.Refresh();

            UpdateGraphCountMenu();

            UpdateColorSchemaMenu();

            

            //mTimer = new PrecisionTimer.Timer();
            //mTimer.Period = 40;                         // 20 fps
            //mTimer.Tick += new EventHandler(OnTimerTick);
            lastTimerTick = DateTime.Now;
            //mTimer.Start();             
        }

        protected override void OnClosed(EventArgs e)
        {
            /*
            if (mTimer == null) return;
            
            mTimer.Stop();
            mTimer.Dispose();
            base.OnClosed(e);
            */
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (CurExample == "ANIMATED_AUTO" )
            {
                    try
                    {
                        TimeSpan dt = DateTime.Now - lastTimerTick;

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            
                            CalcSinusFunction_3(display.DataSources[j], j, (float)dt.TotalMilliseconds);
                            
                        }
                   
                        this.Invoke(new MethodInvoker(RefreshGraph));
                    }
                    catch (ObjectDisposedException ex)
                    {
                        // we get this on closing of form
                    }
                    catch (Exception ex)
                    {
                        Console.Write("exception invoking refreshgraph(): " + ex.Message);
                    }
                 
                
            }
        }
        private void RefreshGraph()
        {                             
            display.Refresh();             
        }

        protected void CalcSinusFunction_0(DataSource src, int idx)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src.Samples[i].x = i;
                src.Samples[i].y = (float)(((float)200 * Math.Sin((idx + 1) *(i + 1.0) * 48 / src.Length)));
            }            
        }

        protected void CalcSinusFunction_1(DataSource src, int idx)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src.Samples[i].x = i;
                
                src.Samples[i].y = (float)(((float)20 *
                                            Math.Sin(20 * (idx + 1) * (i + 1) * Math.PI / src.Length)) *
                                            Math.Sin(40 * (idx + 1) * (i + 1) * Math.PI / src.Length)) +
                                            (float)(((float)200 *
                                            Math.Sin(200 * (idx + 1) * (i + 1) * Math.PI / src.Length)));
            }
            src.OnRenderYAxisLabel = RenderYLabel;
        }

        protected void CalcSinusFunction_2(DataSource src, int idx)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src.Samples[i].x = i;

                src.Samples[i].y = (float)(((float)20 *
                                            Math.Sin(40 * (idx + 1) * (i + 1) * Math.PI / src.Length)) *
                                            Math.Sin(160 * (idx + 1) * (i + 1) * Math.PI / src.Length)) +
                                            (float)(((float)200 *
                                            Math.Sin(4 * (idx + 1) * (i + 1) * Math.PI / src.Length)));
            }
            src.OnRenderYAxisLabel = RenderYLabel;
        }

        protected void CalcSinusFunction_3(DataSource ds, int idx,float time)
        {
            cPoint[] src = ds.Samples;
            for (int i = 0; i < src.Length; i++)
            {
                src[i].x = i;
                src[i].y = 200 + (float)((200 * Math.Sin((idx + 1) * (time + i * 100) / 8000.0)))+
                                +(float)((40  * Math.Sin((idx + 1) * (time + i * 200) / 2000.0)));
                /**
                            (float)( 4* Math.Sin( ((time + (i+8) * 100) / 900.0)))+
                            (float)(28 * Math.Sin(((time + (i + 8) * 100) / 290.0))); */
            }
            
        }


        private void ApplyColorSchema()
        {            
            switch (CurColorSchema)
            {
                case "DARK_GREEN":
                    {
                        Color[] cols = { Color.FromArgb(0,255,0), 
                                         Color.FromArgb(0,255,0),
                                         Color.FromArgb(0,255,0), 
                                         Color.FromArgb(0,255,0), 
                                         Color.FromArgb(0,255,0) ,
                                         Color.FromArgb(0,255,0),                              
                                         Color.FromArgb(0,255,0) };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.FromArgb(0, 64, 0);
                        display.BackgroundColorBot = Color.FromArgb(0, 64, 0);
                        display.SolidGridColor = Color.FromArgb(0, 128, 0);
                        display.DashedGridColor = Color.FromArgb(0, 128, 0);
                    }
                    break;
                case "WHITE":
                    {
                        Color[] cols = { Color.DarkRed, 
                                         Color.DarkSlateGray,
                                         Color.DarkCyan, 
                                         Color.DarkGreen, 
                                         Color.DarkBlue ,
                                         Color.DarkMagenta,                              
                                         Color.DeepPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j%7];
                        }

                        display.BackgroundColorTop = Color.White;
                        display.BackgroundColorBot = Color.White;
                        display.SolidGridColor = Color.LightGray;
                        display.DashedGridColor = Color.LightGray;
                    }
                    break;

                case "BLUE":
                    {
                        Color[] cols = { Color.Red, 
                                         Color.Orange,
                                         Color.Yellow, 
                                         Color.LightGreen, 
                                         Color.Blue ,
                                         Color.DarkSalmon,                              
                                         Color.LightPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j%7];
                        }

                        display.BackgroundColorTop = Color.Navy;
                        display.BackgroundColorBot = Color.FromArgb(0, 0, 64);
                        display.SolidGridColor = Color.Blue;
                        display.DashedGridColor = Color.Blue;
                    }
                    break;

                case "GRAY":
                    {
                        Color[] cols = { Color.DarkRed, 
                                         Color.DarkSlateGray,
                                         Color.DarkCyan, 
                                         Color.DarkGreen, 
                                         Color.DarkBlue ,
                                         Color.DarkMagenta,                              
                                         Color.DeepPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.White;
                        display.BackgroundColorBot = Color.LightGray;
                        display.SolidGridColor = Color.LightGray;
                        display.DashedGridColor = Color.LightGray;
                    }
                    break;

                case "RED":
                    {
                        Color[] cols = { Color.DarkCyan, 
                                         Color.Yellow,
                                         Color.DarkCyan, 
                                         Color.DarkGreen, 
                                         Color.DarkBlue ,
                                         Color.DarkMagenta,                              
                                         Color.DeepPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.DarkRed;
                        display.BackgroundColorBot = Color.Black;
                        display.SolidGridColor = Color.Red;
                        display.DashedGridColor = Color.Red;
                    }
                    break;

                case "LIGHT_BLUE":
                    {
                        Color[] cols = { Color.DarkRed, 
                                         Color.DarkSlateGray,
                                         Color.DarkCyan, 
                                         Color.DarkGreen, 
                                         Color.DarkBlue ,
                                         Color.DarkMagenta,                              
                                         Color.DeepPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.White;
                        display.BackgroundColorBot = Color.FromArgb(183,183,255);
                        display.SolidGridColor = Color.Blue;
                        display.DashedGridColor = Color.Blue;
                    }
                    break;

                case "BLACK":
                    {
                        Color[] cols = { Color.FromArgb(255,0,0), 
                                         Color.FromArgb(0,255,0),
                                         Color.FromArgb(255,255,0), 
                                         Color.FromArgb(64,64,255), 
                                         Color.FromArgb(0,255,255) ,
                                         Color.FromArgb(255,0,255),                              
                                         Color.FromArgb(255,128,0) };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.Black;
                        display.BackgroundColorBot = Color.Black;
                        display.SolidGridColor = Color.DarkGray;
                        display.DashedGridColor = Color.DarkGray;
                    }
                    break;
            }

        }

        protected void CalcDataGraphs( )
        {

            this.SuspendLayout();
           
            display.DataSources.Clear();
            display.SetDisplayRangeX(0, 4);

            // Compare mode?
            if (this.ReferenceFileName != "" && this.CompareFileName != "")
            {
                display.DataSources.Add(createFileDataSource(this.ReferenceFileName,WaveDump.WaveReader.Channel.LEFT));
                display.DataSources.Add(createFileDataSource(this.CompareFileName,WaveDump.WaveReader.Channel.LEFT));

                double xmin = double.MaxValue;
                double xmax = double.MinValue;
                this.SuspendLayout();

                for (int j = 0; j < display.DataSources.Count; j++)
                {
                    xmin = Math.Min(xmin, display.DataSources[j].XMin);
                    xmax = Math.Max(xmax, display.DataSources[j].XMax);
                }
                
                display.SetFullRangeX(xmin, xmax);
                display.SetDisplayRangeX(xmin, xmax);
                display.UpdateYScale();
                display.UpdateScrollBar();

                ApplyColorSchema();
                
                this.ResumeLayout();
                display.Refresh();
                return;
            }

            // Not compare mode, use default modes
            for (int j = 0; j < NumGraphs; j++)
            {
                display.DataSources.Add(new DataSource());
                display.DataSources[j].Name = "Graph " + (j + 1);                
                display.DataSources[j].OnRenderXAxisLabel += RenderXLabel;
              
                switch (CurExample)
                {
                    case  "NORMAL":
                        this.Text = "Normal Graph";
                        display.DataSources[j].Length = 5800;
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.NORMAL;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-300, 300);
                        display.DataSources[j].SetGridDistanceY(100);
                        display.DataSources[j].OnRenderYAxisLabel = RenderYLabel;
                        CalcSinusFunction_0(display.DataSources[j], j);                        
                        break;

                    case "NORMAL_AUTO":
                        this.Text = "Normal Graph Autoscaled";
                        display.DataSources[j].Length = 5800;
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.NORMAL;
                        display.DataSources[j].AutoScaleY = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 300);
                        display.DataSources[j].SetGridDistanceY(100);
                        display.DataSources[j].OnRenderYAxisLabel = RenderYLabel;
                        CalcSinusFunction_0(display.DataSources[j], j);
                        break;

                    case "STACKED":
                        this.Text = "Stacked Graph";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.STACKED;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-250, 250);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcSinusFunction_1(display.DataSources[j], j);
                        break;

                    case "VERTICAL_ALIGNED":
                        this.Text = "Vertical aligned Graph";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.VERTICAL_ARRANGED;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-300, 300);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcSinusFunction_2(display.DataSources[j], j);    
                        break;

                    case "VERTICAL_ALIGNED_AUTO":
                        this.Text = "Vertical aligned Graph autoscaled";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.VERTICAL_ARRANGED;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 300);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcSinusFunction_2(display.DataSources[j], j);
                        break;

                    case "TILED_VERTICAL":
                        this.Text = "Tiled Graphs (vertical prefered)";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_VER;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-300, 600);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcSinusFunction_2(display.DataSources[j], j);    
                        break;

                    case "TILED_VERTICAL_AUTO":
                        this.Text = "Tiled Graphs (vertical prefered) autoscaled";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_VER;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 600);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcSinusFunction_2(display.DataSources[j], j);
                        break;

                    case "TILED_HORIZONTAL":
                        this.Text = "Tiled Graphs (horizontal prefered)";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-300, 600);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcSinusFunction_2(display.DataSources[j], j);    
                        break;

                    case "TILED_HORIZONTAL_AUTO":
                        this.Text = "Tiled Graphs (horizontal prefered) autoscaled";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 600);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcSinusFunction_2(display.DataSources[j], j);
                        break;

                    case "ANIMATED_AUTO":
                       
                        this.Text = "Animated graphs fixed x range";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
                        display.DataSources[j].Length = 402;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].AutoScaleX = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 500);
                        display.DataSources[j].SetGridDistanceY(100);
                        display.DataSources[j].XAutoScaleOffset = 50;
                        CalcSinusFunction_3(display.DataSources[j], j, 0);
                        display.DataSources[j].OnRenderYAxisLabel = RenderYLabel;
                        break;
                }             
            }
            
            ApplyColorSchema();

            this.ResumeLayout();
            display.Refresh();
           
        }

        private int SampleRate = 1;
        private String RenderXLabel(DataSource s, double value)
        {
            if (s.AutoScaleX)
            {
                {
                    int Value = (int)(value);
                    return "" + value.ToString();
                }
            }
            else
            {
                int minute = (int)(value / 60);
                double second = (double)(value - (minute * 60));
                String Label = minute.ToString() + ":" + second.ToString();
                return Label;
            }
        }

        private String RenderYLabel(DataSource s, float  value)
        {             
            return String.Format("{0:0.0}", value);
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            display.Dispose();

            base.OnClosing(e);
        }

        private void stackedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.NORMAL;
        }

        private void verticalALignedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.VERTICAL_ARRANGED;
        }

        private void tiledVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_VER;
        }

        private void tiledHorizontalyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
        }
                
        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.None;
        }

        private void antiAliasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        private void highSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        private void highQualityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "NORMAL";
            CalcDataGraphs();
        }

        private void normalAutoscaledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "NORMAL_AUTO";
            CalcDataGraphs();
        }

        private void stackedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            CurExample = "STACKED";
            CalcDataGraphs();
        }

        private void verticallyAlignedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "VERTICAL_ALIGNED";
            CalcDataGraphs();
        }
        private void verticallyAlignedAutoscaledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "VERTICAL_ALIGNED_AUTO";
            CalcDataGraphs();
        }

        private void tiledVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "TILED_VERTICAL";
            CalcDataGraphs();
        }
        private void tiledVerticalAutoscaledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "TILED_VERTICAL_AUTO";
            CalcDataGraphs();
        }

        private void tiledHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "TILED_HORIZONTAL";
            CalcDataGraphs();
        }

        private void tiledHorizontalAutoscaledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "TILED_HORIZONTAL_AUTO";
            CalcDataGraphs();
        }

        private void animatedGraphDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "ANIMATED_AUTO";
            CalcDataGraphs();
        }
   

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "BLUE";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "WHITE";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "GRAY";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

         private void lightBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "LIGHT_BLUE";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
            
        }

        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "BLACK";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "RED";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

          private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "DARK_GREEN";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
           
        }

        private void UpdateColorSchemaMenu()
        {
            blueToolStripMenuItem.Checked = false;
            whiteToolStripMenuItem.Checked = false;
            grayToolStripMenuItem.Checked = false;
            lightBlueToolStripMenuItem.Checked = false;
            blackToolStripMenuItem.Checked = false;
            redToolStripMenuItem.Checked = false;

            if (CurColorSchema == "WHITE") whiteToolStripMenuItem.Checked = true;
            if (CurColorSchema == "BLUE") blueToolStripMenuItem.Checked = true;
            if (CurColorSchema == "GRAY") grayToolStripMenuItem.Checked = true;
            if (CurColorSchema == "LIGHT_BLUE") lightBlueToolStripMenuItem.Checked = true;
            if (CurColorSchema == "BLACK") blackToolStripMenuItem.Checked = true;
            if (CurColorSchema == "RED") redToolStripMenuItem.Checked = true;
            if (CurColorSchema == "DARK_GREEN") greenToolStripMenuItem.Checked = true;
        }

        private void UpdateGraphCountMenu()
        {
            toolStripMenuItem2.Checked = false;
            toolStripMenuItem3.Checked = false;
            toolStripMenuItem4.Checked = false;
            toolStripMenuItem5.Checked = false;
            toolStripMenuItem6.Checked = false;
            toolStripMenuItem7.Checked = false;

            switch (NumGraphs)
            {
                case 1: toolStripMenuItem2.Checked = true; break;
                case 2: toolStripMenuItem3.Checked = true; break;
                case 3: toolStripMenuItem4.Checked = true; break;
                case 4: toolStripMenuItem5.Checked = true; break;
                case 5: toolStripMenuItem6.Checked = true; break;
                case 6: toolStripMenuItem7.Checked = true; break;
                
            }
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            NumGraphs = 1;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            NumGraphs = 2;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            NumGraphs = 3;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            NumGraphs = 4;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            NumGraphs = 5;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            NumGraphs = 6;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private DataSource createFileDataSource(String filename, WaveDump.WaveReader.Channel channel)
        {
            DataSource ds = new DataSource();
            ds.Name = Path.GetFileNameWithoutExtension(filename);
            ds.OnRenderXAxisLabel += RenderXLabel;
            string ext = Path.GetExtension(filename);
            WaveDump.AudioReader wr0 = null;
            if (ext == ".flac")
            {
                wr0 = new WaveDump.FlacReader(filename);
            }
            else
            {
                wr0 = new WaveDump.WaveReader(filename);
            }

            ds.Length = wr0.nSamples;
            ds.SampleRate = wr0.sampleRate;
            ds.AutoScaleY = false;

            // Set start and end sample of non-zero data.
            for (ds.StartSample = 0;
                (ds.StartSample < wr0.nSamples) &&
                ((channel == WaveDump.WaveReader.Channel.LEFT ? wr0.left[ds.StartSample] : wr0.right[ds.StartSample]) == 0);
                ds.StartSample++) ;
            for (ds.EndSample = wr0.nSamples-1;
                (ds.EndSample >=0 ) &&
                ((channel == WaveDump.WaveReader.Channel.LEFT ? wr0.left[ds.EndSample] : wr0.right[ds.EndSample]) == 0);
                ds.EndSample--) ;

            ds.OnRenderYAxisLabel = RenderYLabel;
            float max = float.MinValue;
            double sum = 0.0;

            if (channel != WaveDump.WaveReader.Channel.HISTOGRAM)
            {
                for (int i = 0; i < ds.Length; i++)
                {
                    ds.Samples[i].x = (double)i / (double)ds.SampleRate;
                    float y = channel == WaveDump.WaveReader.Channel.LEFT ? wr0.left[i] : wr0.right[i];

                    ds.Samples[i].y = y;

                    sum += Math.Abs(y);
                    if (Math.Abs(y) > max) max = Math.Abs(y);
                }
            }
            else
            {
                for (int i = 0; i < wr0.histogramLeft.Length; i++)
                {
                    ds.Samples[i].x = (double)i / (double)ds.SampleRate;
                    float y = wr0.histogramLeft[i];

                    ds.Samples[i].y = y;

                    sum += Math.Abs(y);
                    if (Math.Abs(y) > max) max = Math.Abs(y);
                }
            }

            double avg = sum / (double)ds.Samples.Length;
            //max = (float)avg * 15f;
            ds.SetDisplayRangeY(-max, max);
            ds.SetGridDistanceY(max/10);

            wr0.Dump();
            return ds;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"E:\Music\Pink Floyd\Wish You Were Here\",
                Title = "Browse WAV Files",

                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true,
                DefaultExt = "wav",
                Filter = "wav files (*.wav)|*.wav|flac files (*.flac)|*.flac|All Files (*.*)|*.*",
                FilterIndex = 3,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                double xmin = double.MaxValue;
                double xmax = double.MinValue;
                this.SuspendLayout();

                display.DataSources.Clear();
                // display.SetDisplayRangeX(0, 10000);
                NumGraphs = openFileDialog1.FileNames.Length;
                
                for (int j = 0; j < openFileDialog1.FileNames.Length; j++)
                {
                    if (j == 0) display.DataSources.Add(createFileDataSource(openFileDialog1.FileNames[j], WaveDump.WaveReader.Channel.LEFT));
                    else display.DataSources.Add(createFileDataSource(openFileDialog1.FileNames[j], WaveDump.WaveReader.Channel.LEFT));
                    xmin = Math.Min(xmin, display.DataSources[j].XMin);
                    xmax = Math.Max(xmax, display.DataSources[j].XMax);               
                }

                if (display.DataSources.Count < 2) return;

                ApplyColorSchema();
                
                display.SetDisplayRangeX(xmin,xmax);
                display.setPhaseDataSource(display.DataSources[1], false);
                display.setStartTime(xmin);
                display.UpdateYScale();
                this.ResumeLayout();
                display.Refresh();
            }   
        }

        private void display_MouseMove(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("In mouse wheel");
        }
    }
}