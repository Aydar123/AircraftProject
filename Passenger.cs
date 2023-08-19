using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftProject
{
    public enum TypePassenger    //Тип пассажирского самолета
    {
        Long_haul = 1,            //Дальнемагистральный
        Medium_haul,              //Среднемагистральный 
        Short_haul                //Ближнемагистральный
    }

    class Passenger : Aircraft
    {   
        public int NumberPlaces;    //Число пассажирских мест
        public TypePassenger TP;

        public Passenger() : base()         //Пустой конструктор
        {
            NumberPlaces = 0;
            TP = TypePassenger.Long_haul;
        }

        public Passenger(string mod, int maxR, int speed, int num, TypePassenger typeP) : base(mod, maxR, speed)
        {
            
            NumberPlaces = num;
            TP = typeP;
        }
      
        public override void GetInfo()
        {
            base.GetInfo();
            Console.WriteLine($"Number of places: {NumberPlaces}");
        }

        public override string ToString()
        {
            return "Passenger - " + base.ToString() + " Number places: " + NumberPlaces.ToString() + " Type passenger aircraft: " + TP.ToString();
        }

    }
}
