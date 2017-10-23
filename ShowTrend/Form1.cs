using ExtractValueData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ShowTrend
{
    public partial class Form1 : Form
    {
        private string _urlHistory = "http://fund.eastmoney.com/f10/F10DataApi.aspx?type=lsjz";
        private string _code = "180031";

        void Style()
        {
            //线条颜色
            chart1.Series[0].Color = Color.Green;
            //线条粗细
            chart1.Series[0].BorderWidth = 1;
            //标记点边框颜色      
            chart1.Series[0].MarkerBorderColor = Color.Black;
            //标记点边框大小
            chart1.Series[0].MarkerBorderWidth = 1;
            //标记点中心颜色
            chart1.Series[0].MarkerColor = Color.Red;
            //标记点大小
            chart1.Series[0].MarkerSize = 2;
            //标记点类型     
            chart1.Series[0].MarkerStyle = MarkerStyle.Circle;
            //将文字移到外侧
            chart1.Series[0]["PieLabelStyle"] = "Outside";
            //绘制黑色的连线
            chart1.Series[0]["PieLineColor"] = "Black";
        }

        void InitData()
        {
            var queryUri = "&code={code}&page={page}&per={per}&sdate={sdate}&edate={edate}&rt=0." + DateTime.Now.Second + "2038" + DateTime.Now.Millisecond + "498954" + DateTime.Now.Second;

            var sdate = DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd");
            var edate = DateTime.Now.ToString("yyyy-MM-dd");

            var daysData = new ExtractDayData(_urlHistory, queryUri, _code).GetData(sdate, edate);

            var xData = new List<string>();

            xData.AddRange(daysData.Select(x => x.Date).Reverse());

            var yData = new List<double>();
            yData.AddRange(daysData.Select(x=>x.Val).Reverse());
           //hartArea1.AxisX.Interval = 100;
            chart1.Series[0].Points.DataBindXY(xData, yData);

            //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Days;
            //chart1.ChartAreas[0].AxisX.Interval = 1;
            //chart1.ChartAreas[0].AxisX.Maximum = DateTime.Now.ToOADate();
            //chart1.ChartAreas[0].AxisX.Minimum = DateTime.Now.AddMonths(-6).ToOADate();
            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "MM-dd\nHH:mm";//时间格式。

        }
        private void SetAxisY()
        {
            Axis AxisY = new Axis()
            {
                LineColor = Color.FromArgb(64, 64, 64, 64),
                // Y轴标签字体大小
                LabelAutoFitMinFontSize = 10,
                // Y轴标签显示样式
                LabelStyle = new LabelStyle() { Format = "{#}.0%" },
                // 网格Y轴设置
                MajorGrid = new Grid() { LineColor = Color.FromArgb(64, 64, 64, 64) },
                // Y轴的最大值
                Maximum = 4,
                // Y轴的最小值
                Minimum = 0,
                // Y轴标签间距
                Interval = 0.01,
                // 最小刻度
               // MajorTickMark = new TickMark() { TickMarkStyle = TickMarkStyle.OutsideArea, Size = 500 },
            };

            //System.Windows.Forms.DataVisualization.Charting.Chart c = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chart1.ChartAreas[0].AxisY = AxisY;
        }
        public Form1()
        {
            InitializeComponent();
           
            //SetAxisY();
            Style();
            InitData();
        }
    }
}
