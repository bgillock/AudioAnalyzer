using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;

 
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

 
namespace GraphLib
{        
    public partial class PlotterDisplayEx : UserControl
    {         
        #region MEMBERS

        delegate void InvokeVoidFuncDelegate();

        PlotterGraphSelectCurvesForm GraphPropertiesForm = null;
        PrintPreviewForm printPreviewForm = null;

        //private PrecisionTimer.Timer mTimer = null;
        private float play_speed = 0.5f;
        private float play_speed_max = 10f;
        private float play_speed_min = 0.5f;

        private bool paused = false;
        private bool isRunning = false;

        #endregion

        #region CONSTRUCTOR

        public PlotterDisplayEx()
        {
            InitializeComponent();
            //mTimer = new PrecisionTimer.Timer();
           // mTimer.Period = 50;                         // 20 fps
            //mTimer.Tick += new EventHandler(OnTimerTick);
            play_speed = 0.5f; // 20x10 = 200 values per second == sample frequency     
            // mTimer.Start();
            isRunning = false;
            this.gPane.setDisplay(this);
        }
       
        void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            String text = e.ClickedItem.Text;
            foreach (DataSource s in gPane.Sources)
            {
                if (s.Name == text)
                {
                    s.Active ^= true;
                    gPane.Invalidate();
                    break;
                }
            }
        }
         
        #endregion

        #region PROPERTIES

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(System.ComponentModel.Design.CollectionEditor),
               typeof(System.Drawing.Design.UITypeEditor))]
        public List<DataSource> DataSources
        {
            get { return gPane.Sources; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(System.ComponentModel.Design.CollectionEditor),
               typeof(System.Drawing.Design.UITypeEditor))]
        public PlotterGraphPaneEx.LayoutMode PanelLayout
        {
            get { return gPane.layout; }
            set { gPane.layout = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(System.ComponentModel.Design.CollectionEditor),
               typeof(System.Drawing.Design.UITypeEditor))]
        public SmoothingMode Smoothing
        {
            get { return gPane.smoothing; }
            set { gPane.smoothing = value; }
        }

        [Category("Playback")]
        [DefaultValue(typeof(float), "2")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float PlaySpeed
        {
            get { return play_speed; }
            set { play_speed = value; }
        }

        [Category("Channel")]
        [DefaultValue(typeof(string), "Left")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Channel
        {
            get { return gPane.channel; }
            set { gPane.channel = value; }
        }

        [Category("Playback")]
        [DefaultValue(typeof(bool), "true")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowMovingGrid
        {
            get { return gPane.hasMovingGrid; }
            set { gPane.hasMovingGrid = value; }
        }


        [Category("Properties")]   
        [DefaultValue(typeof(Color), "")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundColorTop
        {
            get { return gPane.BgndColorTop; }
            set { gPane.BgndColorTop = value; }
        }

        [Category("Properties")]  
        [DefaultValue(typeof(Color), "")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundColorBot
        {
            get { return gPane.BgndColorBot; }
            set { gPane.BgndColorBot = value; }
        }

        [Category("Properties")]  
        [DefaultValue(typeof(Color), "")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DashedGridColor
        {
            get { return gPane.MinorGridColor; }
            set { gPane.MinorGridColor = value; }
        }

        [Category("Properties")]
        [DefaultValue(typeof(Color), "")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SolidGridColor
        {
            get { return gPane.MajorGridColor; }
            set { gPane.MajorGridColor = value; }
        }
                
      
        public bool DoubleBuffering
        {
            get { return gPane.useDoubleBuffer; }
            set { gPane.useDoubleBuffer = value; }
        }

        #endregion

        #region PUBLIC METHODS

        public void setPhaseDataSource(DataSource ds, bool phase)
        {
            ds.PhaseShift = phase;
            this.checkBox1.Checked = phase;
        }
        public void setStartTime(double st)
        {
            this.textBox3.Text = st.ToString();
        }
        public void SetDisplayRangeX(double x_start, double x_end )
        {
            gPane.XD0 = (float)x_start;
            gPane.XD1 = (float)x_end;
            gPane.CurXD0 = gPane.XD0;
            gPane.CurXD1 = gPane.XD1;
        }
        public void SetFullRangeX(double x_start, double x_end)
        {
            gPane.XMin = x_start;
            gPane.XMax = x_end;
        }
        public void SetGridDistanceX(float grid_dist_x_samples)
        {
            gPane.grid_distance_x = grid_dist_x_samples;           
        }
      
        public void SetGridOriginX(float off_x)
        {
            gPane.grid_off_x = off_x;          
        }

        
        #endregion

        #region PRIVATE METHODS

        protected override void Dispose(bool disposing)
        {
            paused = true;
            /*
            if (mTimer.IsRunning)
            {
                mTimer.Stop();
                mTimer.Dispose();
            }
            */          
            base.Dispose(disposing);
        }

       
        private void UpdateControl()
        {
            try
            {
                bool AllAutoscaled = true;

                foreach (DataSource s in gPane.Sources)
                {
                    AllAutoscaled &= s.AutoScaleX;
                }

           
                if (AllAutoscaled == true)
                {
                    if (panel1.Visible == true)
                    {
                     //   this.Invoke(new MethodInvoker(SetPlayPanelInvisible));
                    }
                }
                else
                {
                    if (panel1.Visible == false)
                    {
                     //   this.Invoke(new MethodInvoker(SetPlayPanelVisible));
                    }
                }
            }
            catch
            {
            }
        }

        private void UpdatePlayback()
        {
            if (!paused && isRunning == true)
            {
                try
                {
                    // gPane.starting_idx += play_speed;
                    UpdateScrollBar();
                    gPane.Invalidate();
                }
                catch { }
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateControl();

            UpdatePlayback();
        }
        
        public void UpdateScrollBar()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateScrollBar));
            }
            else
            {
                if (gPane.Sources.Count > 0)
                {
                    hScrollBar1.Minimum = (int)(gPane.XMin * 10000f);
                    hScrollBar1.Maximum = (int)(gPane.XMax * 10000f);
                    hScrollBar1.Value = (int)(gPane.XD0 * 10000f);
                    hScrollBar1.LargeChange = (int)((gPane.XD1 - gPane.XD0) * 10000f);
 
                    float maxGraphWidth = -1.0f;
                    for (int i = 0; i < gPane.Sources.Count; i++)
                    {
                        if (gPane.Sources[i].CurGraphWidth > maxGraphWidth) maxGraphWidth = gPane.Sources[i].CurGraphWidth;
                    }

                    hScrollBar1.SmallChange = (int)(((gPane.XD1 - gPane.XD0) / maxGraphWidth) * 10000f);
                }
                else
                {
                    hScrollBar1.Value = 0;
                }
            }
        }
        public void UpdateShift()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateShift));
            }
            else
            {
                textBox1.Text = gPane.Sources[1].StartTime.ToString();
                textBox3.Text = gPane.XD0.ToString();
            }
        }


        public void UpdateYScale()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateYScale));
            }
            else
            {
                textBox2.Text = gPane.Sources[1].YD1.ToString();
            }
        }
        private void OnScrollbarScroll(object sender, ScrollEventArgs e)
        {
            if (gPane.Sources.Count > 0)
            {
                float val = (float)hScrollBar1.Value / 10000f;
                float size = (float)hScrollBar1.LargeChange / 10000f; 
                gPane.XD0 = val;
                gPane.XD1 = val + size;
                gPane.Invalidate();
                UpdateShift();
            }
        }
        #endregion

        private void ShowPrintPreview()
        {
            if (printPreviewForm == null)
            {
                printPreviewForm = new PrintPreviewForm();
            }

            printPreviewForm.GraphPanel = this.gPane;
            printPreviewForm.Show();
            printPreviewForm.TopMost = true;
            printPreviewForm.Invalidate();
        }

        private void selectGraphsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GraphPropertiesForm == null)
            {
                GraphPropertiesForm = new PlotterGraphSelectCurvesForm();              
            }
                      
            GraphPropertiesForm.GraphPanel = this.gPane;            
            GraphPropertiesForm.Show();            
          //  GraphPropertiesForm.BringToFront();

        }

        private void gPane_Scroll(object sender, ScrollEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("In Pane Scroll event");
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            int test = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string scaleStr = comboBox1.Text;
            if (scaleStr == "<All>")
            {
                SetDisplayRangeX(gPane.XMin, gPane.XMax);
            }
            else
            {
                float pixelsPerSample = float.Parse(scaleStr);
                float minSecondsPerSample = 1.0f;
                float maxGraphWidth = -1.0f;
                for (int i=0; i<gPane.Sources.Count; i++)
                {
                    if ((1.0f / gPane.Sources[i].SampleRate) < minSecondsPerSample) minSecondsPerSample = 1.0f / gPane.Sources[i].SampleRate;
                    if (gPane.Sources[i].CurGraphWidth > maxGraphWidth) maxGraphWidth = gPane.Sources[i].CurGraphWidth;
                }
                float newDX = maxGraphWidth * (minSecondsPerSample / pixelsPerSample);
                SetDisplayRangeX(gPane.XD0, gPane.XD0 + newDX);
            }
            
            UpdateScrollBar();
            gPane.Invalidate();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            switch ((int)e.KeyCode)
            {
                case 13: // Enter
                    if (gPane.Sources.Count > 1)
                    {
                        gPane.Sources[1].StartTime = double.Parse(textBox1.Text);
                    }
                    gPane.Invalidate();
                    break;
            } 
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            switch ((int)e.KeyCode)
            {
                case 13: // Enter
                    if (gPane.Sources.Count > 1)
                    {
                        float scaleY = float.Parse(textBox2.Text);
                        gPane.Sources[1].SetDisplayRangeY(-scaleY, scaleY);
                        gPane.Sources[1].SetGridDistanceY(scaleY / 10);
                    }
                    gPane.Invalidate();
                    break;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (gPane.Sources.Count > 1)
            {
                setPhaseDataSource(gPane.Sources[1], checkBox1.Checked);
                Refresh();
            }
        }

        private void textBox3_KeyUp_1(object sender, KeyEventArgs e)
        {
            switch ((int)e.KeyCode)
            {
                case 13: // Enter
                    if (gPane.Sources.Count > 1)
                    {
                        double newStartTime = double.Parse(textBox3.Text);
                        double sizeX = gPane.XD1 - gPane.XD0;
                        SetDisplayRangeX(newStartTime, newStartTime + sizeX);
                        Refresh();
                    }
                    break;
            }
        }
    }
}
