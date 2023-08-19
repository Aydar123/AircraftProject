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
    delegate void DelEventAircraft_1(Operation evAir);
    delegate void AirPortGo(bool g);

    class EventsWithAircraft : StorageAirport   //EVENTS WITH AIRPORT!
    {
        public Storage<Aircraft> currentStor { get; set; }  //Ссылка на хранилищe
        public bool Resolution { get; set; }                //Разрешение на действие
        public int CountBoxs { get; set; }                  //Количество ангаров
        int FreeBoxs;                                       //Кол-во свободных ангаров
        public int timeRefueling;                           //Время заправки самолета
        public Aircraft currentAir;
        Queue<Aircraft> Qair;
        //Qair = new Queue<Aircraft>();

        public EventsWithAircraft(int count, int timeRef, Storage<Aircraft> cS, Aircraft cA)
        {
            this.CountBoxs = count; this.FreeBoxs = this.CountBoxs; Resolution = false; this.timeRefueling = timeRef;
            currentStor = cS;
            currentAir = cA;
            Qair = new Queue<Aircraft>();
        }

        public event DelEventAircraft_1 aircraft_event;       //Событие
        public event AirPortGo airport_go;                  //Событие
        public List<Operation> loperation;                  //Лист операций\журнал

        public EventsWithAircraft()                         //Конструктор + регистрация обработчика событий
        {
            loperation = new List<Operation>();
            aircraft_event += OnEventAircraft;
            airport_go += OnGo; 
        } 

        //public void InitEvent()
        //{
        //    aircraft_event += OnEventAircraft;
        //    airport_go += OnGo;
        //}

        //Обработчик событий
        public virtual void OnEventAircraft(Operation handlerEv)
        {
            Console.WriteLine(handlerEv);
            lock (this)     //Блокировка потоков на момент регистрации события
            {
                if (handlerEv == null) { Console.WriteLine("Event handler is null"); return; }
                if (handlerEv.Sender == null) { Console.WriteLine("Sender is null"); return; }
                try
                {
                    loperation.Add(handlerEv);
                    if (handlerEv.TEA == TypeEventAircraft.Landing || handlerEv.TEA == TypeEventAircraft.Takeoff)
                    {
                        if (airport_go != null) airport_go(false);
                        Thread.Sleep(500);
                        if (airport_go != null) airport_go(true);
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    Console.WriteLine(" Event ={0}  Aircraft={1}", handlerEv.TEA, handlerEv.Sender);
                }
            }
        }

        //public void InitEvent()
        //{
        //    if (EWA != null) aircraft_event += EWA.OnEventAircraft;
        //    if (EWA != null) EWA.airport_go += OnGo;
        //}

        public void OnGo(bool g)
        {
            Resolution = g;
        }
        

        //Посадака самолета
        public void Landing(Aircraft currentPlane)
        {
            if (currentPlane == null) { return; }
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.Landing;
            oper_info.Sender = currentPlane;

            if (aircraft_event != null) aircraft_event(oper_info);

        }

        // Взлет самолета
        public void Takeoff(Aircraft currentPlane)
        {
            if (currentPlane == null) { return; }
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.Takeoff;
            oper_info.Sender = currentPlane;

            if (aircraft_event != null) aircraft_event(oper_info);
        }

        // Добавление самолетов в очередь
        public void AddQAir(Aircraft a)
        {
            Operation oper_info = new Operation();
           // Qair.Enqueue(a); //внесение в очередь //починить
            if (aircraft_event != null) aircraft_event(oper_info);
        }

        //Заправка самолета
        public void Refueling(Aircraft currentPlane, int timeRefueling)
        {
            if (currentPlane == null) { return; }

            Operation oper_info = new Operation();

            while (Resolution)
            {
                if (FreeBoxs > 0 && Qair.Count > 0)
                {
                    Aircraft aircraft_ref = Qair.Dequeue();
                    if (aircraft_event != null) aircraft_event(oper_info);  //генерация

                    oper_info.TEA = TypeEventAircraft.StartRefueling;
                    oper_info.Sender = currentPlane;

                    Thread.Sleep(timeRefueling);  //Приостанавливает текущий поток на заданное время(в данном примере - на миллисекунды)

                    oper_info = new Operation();
                    oper_info.TEA = TypeEventAircraft.FinishRefueling;
                    oper_info.Sender = currentPlane;

                    if (aircraft_event != null) aircraft_event(oper_info);  //генерация

                    Task.Run(() =>
                    {
                        FreeBoxs--;
                        Thread.Sleep(timeRefueling);
                        FreeBoxs++;
                        if (aircraft_event != null) aircraft_event(oper_info);  //генерация
                    });

                }
            }
            //if (washed != null) washed("AutoWashig end job");
        }

        //Ремонт самолета
        public void Repair(Aircraft currentPlane, int intervalRepair)
        {
            if (currentPlane == null) { return; }

            //Формирование информации о событии
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.StartRepeir;
            oper_info.Sender = currentPlane;

            Thread.Sleep(intervalRepair);  //Приостанавливает текущий поток на заданное время(в данном примере - на миллисекунды)

            oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.FinishRepair;
            oper_info.Sender = currentPlane;

            // генерация события
            if (aircraft_event != null) aircraft_event(oper_info);     

        }

        //Oтчет(report) по событиям самолета
        public IEnumerable<Operation> GetOperationForAircraft1(Aircraft currentPlane)
        {
            IEnumerable<Operation> report = from o_info in loperation
                                            where (o_info.Sender == currentPlane)
                                            select o_info;
            return report;
        }

        public void JournalXML()
        {
            // Создание объекта класса XmlWriterSettings для использования его при создании
            // объекта XmlWriter
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;                                     //Использование отступов
            settings.IndentChars = (" ");                               //Символы для использования отступов

            // Создание объекта XmlWriter с записью данных в файл ("journal.xml")
            XmlWriter writer = XmlWriter.Create("journal.xml", settings);
            int id = 1;
            // Write XML data.
            // Создание начального тега корневого узла Aircraft 
            writer.WriteStartElement("Operations");
            foreach (var oper in loperation)
            {

                // Создание начального тега первого узла Passenger_aircraft
               // writer.WriteStartElement("Passenger_aircraft");
                
                writer.WriteStartElement("Operation");
              //  writer.WriteAttributeString("ID", id.ToString());
                writer.WriteElementString("Type", oper.TEA.ToString());
            //    writer.WriteEndElement();
             //   writer.WriteStartElement("DateTime");
                writer.WriteElementString("DateTime", oper.DT.ToLongDateString());
            //    writer.WriteElementString("Time", oper.DT.ToShortTimeString());
          //      writer.WriteEndElement();
           //     writer.WriteStartElement("Passenger");
                writer.WriteElementString("Model", oper.Sender.ToString());
                writer.WriteEndElement();
                id++;

                //writer.WriteElementString("Name", "Телевизор"); // Запись тега ...

                /*
                writer.WriteStartElement("Model", );
                writer.WriteElementString("TypeOperation", oper.TEA.ToString);
                writer.WriteAttributeString("Max_range", "7406"); // Запись атрибута ...
                writer.WriteAttributeString("Max_speed", "800");  // Запись атрибута ...
                writer.WriteAttributeString("Number_places", "250");
                writer.WriteAttributeString("Type_passenger", "Medium haul");
                writer.WriteEndElement();

                writer.WriteStartElement("Airbas_ACJ320");
                writer.WriteAttributeString("Max_range", "7800");
                writer.WriteAttributeString("Max_speed", "900");
                writer.WriteAttributeString("Number_places", "190");
                writer.WriteAttributeString("Type_passenger", "Medium haul");
                writer.WriteEndElement();

                writer.WriteStartElement("Boing_747");
                writer.WriteAttributeString("Max_range", "12100");
                writer.WriteAttributeString("Max_speed", "917");
                writer.WriteAttributeString("Number_places", "425");
                writer.WriteAttributeString("Type_passenger", "Long haul");
                writer.WriteEndElement();
                */

                //Создание конечного тега первого узла Passenger_aircraft
            //  writer.WriteEndElement();
                

                

                /*
                // Создание второго узла Cargo_aircraft
                writer.WriteStartElement("Cargo_aircraft");

                writer.WriteStartElement("АН-124");
                writer.WriteAttributeString("Max_range", "10000");
                writer.WriteAttributeString("Max_speed", "650");
                writer.WriteAttributeString("Carrying", "120");
                writer.WriteEndElement();

                writer.WriteEndElement();

                // Создание третьего узла Military_aircraft
                writer.WriteStartElement("Military_aircraft");

                writer.WriteStartElement("Су-34");
                writer.WriteAttributeString("Max_range", "5000");
                writer.WriteAttributeString("Max_speed", "990");
                writer.WriteAttributeString("Type_military", "Fighter bomber");
                writer.WriteAttributeString("Weapon_system", "Rockets");
                writer.WriteEndElement();
                

                writer.WriteEndElement();
                */
            }
                // Создание конечного тега корневого узла Operations
                writer.WriteEndElement();
              
            // сброс данных из буфера в файл    
            writer.Flush();
            // Закрытие потока
            writer.Close();
        }

    }
}
