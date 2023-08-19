using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftProject
{
    class Cargo : Aircraft
    {
        //public int Volume;        //Объем грузового отсека
        //public float Dimensions;  //Габариты грузового отсека
        public int Сarrying;      //Грузоподъемность

        public Cargo() : base()
        {
            //Volume = 0;
            //Dimensions = 0F;
            this.Сarrying = 0;              //через this потому что не предлагает выбрать carrying  
        }

        public Cargo(string mod, int maxR, int speed, int carry) : base(mod, maxR, speed)
        {
            //Volume = vol;
            //Dimensions = dimens;
            Сarrying = carry;
        }

        public override void GetInfo()
        {
            base.GetInfo();
            Console.WriteLine($"Carrying: {Сarrying}tone");
        }

        public override string ToString()
        {
            return "Cargo - " + base.ToString() + " Carrying: " + Сarrying.ToString();
        }

    }
}
