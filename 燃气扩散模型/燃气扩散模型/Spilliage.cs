using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 燃气扩散模型
{
    class Spillage
    {
        const double k = 1.3;//天然气绝热系数
        const double R = 8.31436;//气体常数
        const double C = 1.0;//泄漏系数（默认泄漏孔为圆形）
        const double M = 0.01684;//天然气相对分子质量



        public double spillage;//计算泄漏量（质量）
        public double leakspeed;//就算泄漏速度
        public double insidePressur;//管道压强
        public double outsidePressur;//环境压强
        public double temperature;//天然气温度
        public double leakDiameter;// 泄漏孔的直径（此处默认其为圆形）

        public double LeakSpeed()
        {
            leakspeed = 0.97 * Math.Sqrt(2 * k / (k - 1) * R * temperature * (1 - Math.Pow(outsidePressur / insidePressur, (k - 1) / k)));
            return leakspeed;

        }
        public double Leackage()
        {
            double leakRatio = 0; //根据泄漏的音速条件，计算相应的泄漏速度系数
            if (outsidePressur / insidePressur > Math.Pow(2 / (k + 1), k / (k - 1))) //亚音速
                leakRatio = Math.Sqrt(2 * k / (k - 1) * (Math.Pow(outsidePressur / insidePressur, 2 / k) - Math.Pow(outsidePressur / insidePressur, (k - 1) / k)));
            else if (outsidePressur / insidePressur <= Math.Pow(2 / (k + 1), k / (k - 1)))//音速
                leakRatio = Math.Sqrt(2 * k / (k + 1) * Math.Pow(2 / (k + 1), 2 / (k - 1)));

            //使用经典源强公式计算泄漏体积，感觉不对
            spillage = Math.PI * Math.Pow(leakDiameter / 2, 2) * C * insidePressur * leakRatio * Math.Sqrt(M / (R * temperature));

            //使用泄漏速度计算膨胀后的天然气体积
            //spillage =   Math.PI * LeakSpeed() * leakDiameter * leakDiameter * insidePressur / (32*outsidePressur);
            return spillage;
        }
    }
}
