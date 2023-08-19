using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftProject
{
 
     abstract class Aircraft : IBaseInterface
    {

        public string Model;       //Модель самолета
        public int MaxRange;       //Максимальная дальность полета
        public int Speed;          //Скорость
        public int IDAcr { get; }
        public static int ID = 0;


        public Aircraft() : base()          //пустой конструктор
        {
            Model = "No Model";             //Значение по умолчанию
            MaxRange = 0;                   //Значение по умолчанию
            Speed = 0;                      //Значение по умолчанию
            ID++; IDAcr = ID;
            
        }      
        
        public Aircraft(string mod, int maxR, int speed)     //Создаем конструктор
        {
            Model = mod;
            MaxRange = maxR;
            Speed = speed;
            ID++; IDAcr = ID;
        }

        public virtual void Fly()                           //Реализация интерфейсов
        {
            Console.WriteLine($"The plain is flying");
        }

        public virtual void Break()                         //Реализация интерфейсов
        {
            Console.WriteLine($"The plain broak down");             
        }

        public virtual void Refuel()                         //Реализация интерфейсов
        {
            Console.WriteLine($"The plain is refueling");
        }

        public virtual void GetInfo()
        {
            Console.WriteLine($"ID: {IDAcr} Model: {Model} Max range of flight: {MaxRange} Speed: {Speed}");
        }
        //public abstract void GetInfo(); //используя abstract

        public override string ToString()
        {
            return $"ID: {IDAcr} Model: {Model} Max range of flight: {MaxRange} Speed: {Speed}";
        }

        public bool Active { get; internal set; }   //Для StorageAirport...

    }
}
