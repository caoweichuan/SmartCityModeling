using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 燃气扩散模型
{
    class GasConcentration
    {
        //下风向距离x,垂直风向距离y，高度z，风速u，泄露时间t，风向angle
        public double x, y, z, u, t,angle;
         public double gasConCentration;
        

        public double ConcentrationCal(Spillage sp)
        {
            double x_Factor = 0.16 * x / Math.Sqrt(1 + 0.0004 * x);//x方向上的扩散系数，需要根据当地大气稳定度确定，此处使用D级大气稳定度条件
            double y_Factor = 0.16 * y / Math.Sqrt(1 + 0.0004 * y);
            double z_Factor = 0.14 * z / Math.Sqrt(1 + 0.0003 * z);
            //double y_turbulence = u * y_Factor * y_Factor / (2 * x * t);//y方向上的湍流系数，用于定源强模型
            //double z_turbulence = u * z_Factor * z_Factor / (2 * x * t);

            
            //烟羽模型
            gasConCentration = sp.Leackage() / (Math.PI * u * y_Factor * z_Factor)
                * Math.Exp(-1 / 2 * (z * z / (2 * z_Factor * z_Factor) + y * y / (2 * y_Factor * y_Factor)));
            //定源强模型
            //gasConCentration = sp.Leackage() / (4 * Math.PI * Math.Pow(y_turbulence * z_turbulence, 1 / 2))
            //    * Math.Exp(-u / (4 * x) * (y * y / y_turbulence + z * z / z_turbulence));

            return gasConCentration;
        }
    }
}
