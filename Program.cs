using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace AircraftProject
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Constructors
            Aircraft pass = new Passenger("Boing 777", 7406, 800, 250, TypePassenger.Medium_haul);
           //pass.GetInfo();
           Console.WriteLine(pass);

           //Aircraft[] arreyAir = new Aircraft[3];
           //arreyAir[0] = new Passenger("Boing 555555", 7406, 800, 250, TypePassenger.Medium_haul);
           //Console.WriteLine(arreyAir);

           Aircraft pass1 = new Passenger("Airbas ACJ320", 7800, 900, 190, TypePassenger.Medium_haul);
           //pass1.GetInfo();
           Console.WriteLine(pass1);

           Aircraft pass2 = new Passenger("Boing 747", 12100, 917, 425, TypePassenger.Long_haul);
           //pass2.GetInfo();
           Console.WriteLine(pass2);

           Aircraft cargo = new Cargo("АН-124", 10000, 650, 120);
           //cargo.GetInfo();
           Console.WriteLine(cargo);

            Aircraft cargo1 = new Cargo("ИЛ-76", 5000, 400, 45);
            //cargo.GetInfo();
            Console.WriteLine(cargo1);

            Aircraft military = new Military("Су-34", 5000, 990, TypeMilitary.Fighter_bomber, WeaponSystem.Rockets);
           //military.GetInfo();
           Console.WriteLine(military);

            Aircraft military1 = new Military("МиГ-35", 3000, 2400, TypeMilitary.Fighter, WeaponSystem.Rockets);
            //military.GetInfo();
            Console.WriteLine(military1);
            #endregion

            #region Interface
            Console.WriteLine("-----------------------------------");
           Military imil = new Military();
           imil.Hit();
           imil.Break();
           imil.Fly();
           imil.Refuel();
           Console.WriteLine("-----------------------------------");
            #endregion

            #region Storage
           Storage<int> stor = new Storage<int>();
           stor.AddBasic(1154);
           stor.AddBasic(2235);
           stor.RemoveBasic(2235);
         
           foreach (int s in stor)
             {
              Console.WriteLine(s);
             }
           Console.WriteLine("-----------------------------------");

           Aircraft[] aircraft_storage ={  new Passenger("Boing 777", 7406, 800, 250, TypePassenger.Medium_haul),
                                           new Passenger("Airbas ACJ320", 7800, 900, 190, TypePassenger.Medium_haul),
                                           new Passenger("Boing 747", 12100, 917, 425, TypePassenger.Long_haul),
                                           new Military("Су-34", 5000, 990, TypeMilitary.Fighter_bomber, WeaponSystem.Rockets),
                                           new Military("МиГ-35", 3000, 2400, TypeMilitary.Fighter, WeaponSystem.Rockets),
                                           new Cargo("АН-124", 10000, 650, 120),
                                           new Cargo("ИЛ-76", 5000, 400, 45)
                                         };

           StorageAirport storAir = new StorageAirport();
           foreach (Aircraft a in aircraft_storage) storAir.InAirport(a);

           Console.WriteLine("All medium-haul passenger aircrafts ->");
           foreach (Aircraft med_haul in storAir.ViewTypePassenger(TypePassenger.Medium_haul))
           Console.WriteLine(med_haul);
            
           Console.WriteLine("All military fighter-bombers with rocket weapon system ->");
           foreach (Aircraft mil in storAir.ViewMilitary(TypeMilitary.Fighter_bomber, WeaponSystem.Rockets))  
           Console.WriteLine(mil);

           Console.WriteLine("All cargo aircraft with carrying more than 110 ton ->");
           var vc = storAir.ViewCargo(110);
           foreach (Aircraft cur in vc)
           Console.WriteLine(cur);

           storAir.OutAirport(aircraft_storage[1]); //???
            #endregion

            #region Semaphore
            /*
            Console.WriteLine("__________________________________________________");
            for (int i = 1; i < 3; i++)
            {
                ThreadsAircraft ta = new ThreadsAircraft();
            }
            */
            #endregion

            #region Threads
            Console.WriteLine("__________________________________________________");
           
            Console.WriteLine("Start of all threads!");
            AirportWithEvents air_with_e = new AirportWithEvents();
            air_with_e.InitDB();
            Thread[] thArray = new Thread[aircraft_storage.Length];// Массив с потоками
            ActionsAircraft[] acts = new ActionsAircraft[aircraft_storage.Length];//Массив с событиями\операциями

            for (int i = 0; i < aircraft_storage.Length; i++)           
            {
                acts[i] = new ActionsAircraft(air_with_e, aircraft_storage[i], 1000 + (i % 2) * 200);    //инициализация
                acts[i].InitEvent();
                thArray[i] = new Thread(acts[i].DoActionAirCraft);  //инициализация массива потоков и их запуск
                thArray[i].Start();
            }

            /*
            for (int i = 1; i <= 10; i++)               // тут ремонт...
            {
                Thread.Sleep(rnd.Next(1500));
                if (i % 2 == 0)
                    event_a.AddQAir(new Cargo("ИЛ-76", 5000, 400, 45));
                else
                    event_a.AddQAir(new Military("МиГ-35", 3000, 2400, TypeMilitary.Fighter, WeaponSystem.Rockets));
            }
            */

            //Ожидание завершения потоков
            bool b = true;
            while (b)
            {
                b = false;
                foreach (var thread in thArray)
                {
                    b = thread.IsAlive || b;        //статус потока
                }
            }
           Console.WriteLine("All threads end work!");
            #endregion

            for (int i = 0; i < aircraft_storage.Length; i++) // вывод на экран операции
            {
                foreach (Operation oper in air_with_e.GetOperationForAircraft(aircraft_storage[i]))
                Console.WriteLine(oper);
            }

            Console.WriteLine("_____________Journal_Operations.xml test_____________");
            air_with_e.JournalXML();

            Console.WriteLine("_____________DataBaseAircraft_____________");
            //air_with_e.DBAircraft();

            #region Reports
            air_with_e.IDCharacterReport(aircraft_storage[5]);
            air_with_e.EventReport(TypeEventAircraft.StartRepeir);
            air_with_e.TypeAircraftEventReport(aircraft_storage[0], TypeEventAircraft.Landing);

            air_with_e.ReportFromDB();
            #endregion

            air_with_e.QuitDB();

            Console.ReadKey();
        }
    }
}
