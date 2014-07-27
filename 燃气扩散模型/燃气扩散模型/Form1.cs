using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace 燃气扩散模型
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<double> paraList = new List<double>(5);
        //XmlDocument paraxml = new XmlDocument();
        Spillage sp = new Spillage();
        GasConcentration gasCon = new GasConcentration();

        static int cellSize = 10;//网格大小 10m
        /// <summary>
        /// 打开并加载参数xml文档
        /// 将参数值存储到参数list中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                XmlDocument paraxml = new XmlDocument();
                OpenFileDialog fd = new OpenFileDialog();
                fd.Filter = "(*.xml;)|*.xml";
                fd.InitialDirectory = Application.StartupPath + @"";
                fd.ShowReadOnly = true;
                DialogResult result = fd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    paraxml.Load(fd.FileName);
                }
                openFileButton.Enabled = false;

                NameTable xmt = new NameTable();
                XmlNamespaceManager xnm = new XmlNamespaceManager(xmt);
                xnm.AddNamespace(string.Empty, "");
                XmlNodeList xnlist = paraxml.SelectNodes("/ParameterML/ParameterList/Parameter/value");
                foreach (XmlNode xn in xnlist)
                {
                    paraList.Add(Convert.ToDouble(xn.InnerText));
                }
                calculateButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            sp.insidePressur = paraList[0];
            sp.outsidePressur =paraList[1];
            sp.temperature =paraList[2];
            sp.leakDiameter =paraList[3];
            gasCon.t = paraList[4];
            gasCon.u = paraList[6];
            gasCon.angle = paraList[7];

            txtProduct();
            calculateButton.Enabled = false;
            clearButton.Enabled = true;
          
        }
        string[,] Concentration_2d()
        {


            int i = 0;
            int j = 0;
            string[,] gasConcentrationCollection = new string[100, 100];
            for (i = 0; i < 100; i++)
            {
                for (j = 0; j < 100; j++)
                {
                    gasCon.x = j * cellSize;
                    gasCon.y = i * cellSize;
                    gasCon.z = paraList[5];
                    gasCon.ConcentrationCal(sp);
                    gasConcentrationCollection[i, j] = ((double)gasCon.ConcentrationCal(sp)).ToString("#0.00000");


                }
            }
            return gasConcentrationCollection;


        }
        private void txtProduct()
        {
            string fileName = "gasConcentration2DArray.txt";
            string path = @"" + fileName;
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            //输出文件头，说明参数
            sw.WriteLine("ncols              100");
            sw.WriteLine("nrows             100");
            sw.WriteLine("xllcorner        {0}", paraList[8]);
            sw.WriteLine("yllcorner        {0}", paraList[9]);
            sw.WriteLine("insidePressure:        {0} Pa ", paraList[0]);
            sw.WriteLine("outsidePressure:      {0} Pa", paraList[1]);
            sw.WriteLine("temparature:            {0} K", paraList[2]);
            sw.WriteLine("leakDiameter:          {0}  m", paraList[3]);
            sw.WriteLine("time:               {0}  s", paraList[4]);
            sw.WriteLine("windSpeed:              {0}  m/s", gasCon.u = paraList[6]);
            sw.WriteLine("windDirection:        {0} degree",gasCon.angle=paraList[7]);
            sw.WriteLine("生成时间：      {0} ", DateTime.Now);
            sw.Flush();
            //输出坐标对应的浓度数据
            string[,] temp = Concentration_2d();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    gasCon.x = j * cellSize;
                    gasCon.y = i * cellSize;
                    string element = temp[i, j];
                    if (element == "非数字")
                        sw.Write("({0},{1})99999.9 ", GetTargateCor().Lon.ToString("#0.00000"), GetTargateCor().Lat.ToString("#0.00000"));
                    else
                        sw.Write("({0},{1})" + element + " ", GetTargateCor().Lon.ToString("#0.00000"), GetTargateCor().Lat.ToString("#0.00000"));
                }
                sw.WriteLine();
            }
            sw.Close();
        }
        /// <summary>
        /// 根据泄漏点经纬度，距离，方位角计算目标点经纬度
        /// </summary>
        /// <returns></returns>
        private Corordinate GetTargateCor()
        {
            double lon = paraList[8];
            double lat = paraList[9];
            Corordinate leakCor = new Corordinate(lon, lat);

            double distance = Math.Sqrt(gasCon.x * gasCon.x + gasCon.y * gasCon.y);//距离
            double direction = gasCon.angle * Math.PI / 180 - Math.Atan(gasCon.y / gasCon.x);//弧度 
            double dx = distance * Math.Sin(direction);
            double dy = distance * Math.Cos(direction);
            double targateLon;
            double targateLat;
            if (gasCon.x == 0 && gasCon.y == 0)
            {
                targateLon = lon;
                targateLat = lat;
            }
            else
            {
                targateLon = (dx / leakCor.Ed + leakCor.RadLon) * 180 / Math.PI;
                targateLat = (dy / leakCor.Ec + leakCor.RadLat) * 180 / Math.PI;
            }
            Corordinate targate = new Corordinate(targateLon, targateLat);
            return targate;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            paraList.Clear();
            
            openFileButton.Enabled = true;
            
        }

    }
}
 
