using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftProject
{
    public enum TypeMilitary    //типы военных самолетов
    {
        Fighter = 1,    //Истребитель
        Bomber,         //Бомбардировщик
        Interceptor,    //Перехватчик
        Stormtrooper,   //Штурмовик
        Fighter_bomber  //Истребитель-бомбардировщик
    }

    public enum WeaponSystem     //Система вооружения
    {
        Rockets,        //Ракеты
        Cannons         //Пушки
    }


    class Military : Aircraft, Imilitary
    {
        public TypeMilitary TM;
        public WeaponSystem WS;

        public Military() : base()              //Пустой конструктор
        {
            TM = TypeMilitary.Fighter;        //Значение по умолчанию
            WS = WeaponSystem.Rockets;        //Значение по умолчанию
        }

        public Military(string mod, int maxR, int speed, TypeMilitary typeM, WeaponSystem weaponS) : base(mod, maxR, speed)
        {
            TM = typeM;
            WS = weaponS;
        }

        public override void GetInfo()
        {
            base.GetInfo();
        }

        public void Hit()                                    //Реализация интерфейсов
        {
            Console.WriteLine($"Military aircraft hits the target");
        }
        
        public override string ToString()
        {
            return "Military - " + base.ToString() + " Type military: " + TM.ToString() + " Weapon system: " + WS.ToString();
        }
    }
}
