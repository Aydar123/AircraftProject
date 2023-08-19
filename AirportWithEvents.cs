using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace AircraftProject
{
    delegate void DelEventAircraft(Operation evAir);
    delegate void AirPortGo_New(bool g);

    class AirportWithEvents
    {
        //public event EventAircraft aircraft_event;    //Событие
        public event AirPortGo_New airport_go_new;      //Событие
        public List<Operation> loperation;              //Лист операций\журнал

        public SqlConnection SQLconnect;
        public SqlDataAdapter daEv, daTA, daAC, daOp;
        public SqlCommandBuilder cmdEv, cmdTA, cmdAV, cmdOp;
        public DataSet DS;
        public DataTable dtEv, dtTA, dtAC, dtOp;
        int idOp = 0;

        public AirportWithEvents() { loperation = new List<Operation>(); /*aircraft_event += OnEventAircraft;*/ } //Конструктор

        //Обработчик событий
        public virtual void OnEventAircraft(Operation handlerEv)
        {
            lock (this)     //Блокировка потоков на момент регистрации события
            {
                if (handlerEv == null) { Console.WriteLine("Event handler is null"); return; }
                if (handlerEv.Sender == null) { Console.WriteLine("Sender is null"); return; }
                try
                {
                    loperation.Add(handlerEv);
                    // Job with DB
                    Aircraft curac = handlerEv.Sender;
                    int idac = curac.IDAcr;
                    DataRow[] selac = dtAC.Select(string.Format("idAircraft={0}", idac));
                    if (selac.Length == 0)
                    {
                        DataRow dr = dtAC.NewRow();
                        dr["IDAirCraft"] = idac;
                        dr["Model"] = curac.Model;
                        dr["IDTypeAircraft"] = getID_TypeAirCraft(curac);
                        dr["Max range"] = curac.MaxRange;
                        dr["Speed"] = curac.Speed;
                        dtAC.Rows.Add(dr);
                    }
                    // Operations
                    DataRow drop = dtOp.NewRow();
                    idOp++;
                    drop["IDOperationAir"] = idOp;
                    drop["IDAircraft"] = idac;
                    drop["IDEvent"] = handlerEv.TEA;
                    drop["DateTimeEvents"] = (DateTime)handlerEv.DT;
                    drop["Message"] = handlerEv.Message;
                    drop["IDTypeAircraft"] = getID_TypeAirCraft(curac);
                    dtOp.Rows.Add(drop);

                    if (handlerEv.TEA == TypeEventAircraft.Landing || handlerEv.TEA == TypeEventAircraft.Takeoff)

                    {
                        if (airport_go_new != null) airport_go_new(false);
                        Thread.Sleep(500);
                        if (airport_go_new != null) airport_go_new(true);
                    }
                    //  Console.WriteLine(handlerEv);
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    Console.WriteLine(" Event ={0}  Aircraft={1}", handlerEv.TEA, handlerEv.Sender);
                }
            }
        }

        ////Oтчет(report) по событиям операций за интервал времени
        //public IEnumerable<Operation> GetOperationWithTimeInterval(DateTime beginDT, DateTime endDT)
        //{
        //    IEnumerable<Operation> report = from o_info in loperation
        //                                    where (o_info.DT >= beginDT && o_info.DT <= endDT)
        //                                    select o_info;
        //    return report;
        //}

        //Oтчет(report) по событиям самолета
        public IEnumerable<Operation> GetOperationForAircraft(Aircraft currentPlane)
        {
            IEnumerable<Operation> report = from o_info in loperation
                                            where (o_info.Sender == currentPlane)
                                            select o_info;
            return report;
        }

        public void JournalXML()
        {
            Console.WriteLine("Start XML");

            XmlWriterSettings settings = new XmlWriterSettings();       //Объект для записи настроек
            settings.Indent = true;                                     //Использование отступов
            settings.IndentChars = (" ");                               //Символы для использования отступов

            // Создание объекта XmlWriter с записью данных в файл ("journal_Operations.xml")
            XmlWriter writer = XmlWriter.Create("journal_Operations.xml", settings);

            // Создание начального тега корневого узла Operations 
            writer.WriteStartElement("Operations");
            foreach (var oper in loperation)
            {

                writer.WriteStartElement("Operation");

                    writer.WriteStartElement("Aircarft");
                        writer.WriteElementString("ID", oper.Sender.IDAcr.ToString());
                        writer.WriteElementString("TypeAircraft", oper.Sender.GetType().Name);
                        writer.WriteElementString("Model", oper.Sender.Model);
                    writer.WriteEndElement();

                    writer.WriteElementString("Event", oper.TEA.ToString());
                    writer.WriteElementString("Date", oper.DT.ToLongDateString());
                    writer.WriteElementString("Time", oper.DT.ToShortTimeString());

                writer.WriteEndElement();
            }
            // Создание конечного тега корневого узла Operations
            writer.WriteEndElement();

            // сброс данных из буфера в файл    
            writer.Flush();
            // Закрытие потока
            writer.Close();

            Console.WriteLine("Finish XML");
        }

        public void DBAircraft()
        {
            Console.WriteLine("Start DB");
            SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Алмаз\source\repos\AircraftProject\BDAircraft.mdf;Integrated Security=True;Connect Timeout=30");
            connect.Open();

            Console.WriteLine("______Table_Events______");
            try
            {
                SqlCommand cmd = new SqlCommand("select * from Events", connect);
                SqlDataReader readerEv = cmd.ExecuteReader();

                if (readerEv.HasRows)
                {
                    while (readerEv.Read())
                    {
                        Console.WriteLine("IDEvent {0} NameEvent {1}", readerEv[0], readerEv[1]);
                    }
                }

                readerEv.Close();
              
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("______Table_TypeAircraft______");
            try
            {
                SqlCommand cmd1 = new SqlCommand("select * from TypeAircraft", connect);
                SqlDataReader readerTA = cmd1.ExecuteReader();

                if (readerTA.HasRows)
                {
                    while (readerTA.Read())
                    {
                        Console.WriteLine("IDTypeAircraft {0} NameTypeAircraft {1}", readerTA[0], readerTA[1]);
                    }
                }

                readerTA.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            connect.Close();

            Console.WriteLine("Finish DB");
        }

        public int getID_TypeAirCraft(Aircraft ac)
        {
            if (ac is Passenger) return 1;
            if (ac is Military) return 2;
            if (ac is Cargo) return 3;
            return 0;
        }

        public void InitDB()
        {
            SQLconnect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Алмаз\source\repos\AircraftProject\BDAircraft.mdf;Integrated Security=True;Connect Timeout=30");
            SQLconnect.Open();
            CleanDB();
            DS = new DataSet("VDB");
            daEv = new SqlDataAdapter("select * from Events", SQLconnect);
            daTA = new SqlDataAdapter("select * from TypeAircraft", SQLconnect);
            daAC = new SqlDataAdapter("select * from Aircraft", SQLconnect);
            daOp = new SqlDataAdapter("select * from OperationsAir", SQLconnect);

            cmdEv = new SqlCommandBuilder(daEv);
            cmdTA = new SqlCommandBuilder(daTA);
            cmdAV = new SqlCommandBuilder(daAC);
            cmdOp = new SqlCommandBuilder(daOp);

            daEv.Fill(DS, "Events"); dtEv = DS.Tables[0];       //Tables - Возвращает коллекцию таблиц класса DataSet
            daTA.Fill(DS, "TypeAircraft"); dtTA = DS.Tables[1];  
            daAC.Fill(DS, "Aircraft"); dtAC = DS.Tables[2];
            daOp.Fill(DS, "OperationsAir"); dtOp = DS.Tables[3];
            idOp = dtOp.Rows.Count;
            ViewDS(DS);

        }
        
        public void CleanDB()
        {
            try
            {

                string sql = "Delete from OperationsAir where IDOperationAir < @id ";

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = SQLconnect;
                cmd.CommandText = sql;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = 1000;

                // Выполнить Command (Используется для delete, insert, update).
                int rowCount = cmd.ExecuteNonQuery();
                //DS.AcceptChanges();

                Console.WriteLine("Очистил таблицу OperationsAir с кол-ом операций = " + rowCount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }

        }

        public void QuitDB()
        {
            daAC.Update(DS, "Aircraft");
            daOp.Update(DS, "OperationsAir");
            SQLconnect.Close();
        }

        public void ViewDS(DataSet DS)
        {
            Console.WriteLine("DataSet is named: {0}", DS.DataSetName);
            // Вывести каждую таблицу.
            foreach (DataTable dt in DS.Tables)
            {
                ViewDataTable(dt);
            }
        }
        public void ViewDataTable(DataTable dt)
        {
            Console.WriteLine("\n----------------------------------");
            Console.WriteLine("Table =>  {0}", dt.TableName);
            // Вывести имена столбцов.
            for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
            {
                Console.Write(dt.Columns[curCol].ColumnName + "\t");
            }
            Console.WriteLine();
            // Вывести DataTable.
            for (int curRow = 0; curRow < dt.Rows.Count; curRow++)
            {
                for (int curCol = 0; curCol < dt.Columns.Count; curCol++)
                {
                    Console.Write(dt.Rows[curRow][curCol].ToString() + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n----------------------------------");

        }

        public void ReportFromDB()
        {
            Console.WriteLine("\nReport from DB\n");
            
            var query = from tj in dtOp.AsEnumerable()
                        join ta in dtAC.AsEnumerable() on tj.Field<int>("IDAircraft") equals ta.Field<int>("IDAircraft")
                        join te in dtEv.AsEnumerable() on tj.Field<int>("IDEvent") equals te.Field<int>("IDEvent")
                        join typeair in dtTA.AsEnumerable() on tj.Field<int>("IDTypeAircraft") equals typeair.Field<int>("IDTypeAircraft")
                        select new
                        {
                            model = ta.Field<string>("Model"),
                            dataevent = tj.Field<DateTime>("DateTimeEvents"),
                            nevents = te.Field<string>("NameEvent"),
                            idta = ta.Field<int>("IDTypeAircraft"),
                            mes = tj.Field<string>("Message"),
                            typename = typeair.Field<string>("NameTypeAircraft")

                        };
            foreach (var op in query)
            {
                DataRow[] tac = dtTA.Select(string.Format("IDTypeAircraft={0}", op.idta));
                Console.WriteLine("{0}  {1}  ID Type Aircraft: {2}  {3}  {4}  {5}", op.model, op.typename, tac[0]["IDTypeAircraft"], op.nevents, op.dataevent, op.mes);
            }
            
        }

        public void IDCharacterReport(Aircraft aircraftID)
        {
            Console.WriteLine("\n----------------------------------");
            var aircrafts = from air in dtAC.AsEnumerable()
                       join ta in dtTA.AsEnumerable() on air.Field<int>("IDTypeAircraft") equals ta.Field<int>("IDTypeAircraft")
                       where
                       air.Field<int>("IDAircraft") == aircraftID.IDAcr
                       select new
                       {
                           model = air.Field<string>("Model"),
                           typeac = ta.Field<string>("NameTypeAircraft"),
                           max_r = air.Field<int>("Max range"),
                           speed = air.Field<int>("Speed")
                         
                       };
            foreach (var item in aircrafts)
            {
                Console.WriteLine("Характеристика самолета по ID {0} -> Model = {1} Name type: {2} Max range: {3}[m] Speed: {4}[km/h]", aircraftID.IDAcr, item.model, item.typeac, item.max_r, item.speed);
                //Console.WriteLine("Характеристика самолета по ID -> {0}", aircraftID.IDAcr);
            }

            Console.WriteLine("\n----------------------------------");
        }

        public void EventReport(TypeEventAircraft tea)
        {
            Console.WriteLine("\n----------------------------------");
                var repair = from rep in dtOp.AsEnumerable()
                             join ac in dtAC.AsEnumerable() on rep.Field<int>("IDAircraft") equals ac.Field<int>("IDAircraft")
                             where
                             rep.Field<int>("IDEvent") == (int)tea
                            
                                select new
                                    {
                                     id = rep.Field<int>("IDAircraft"),
                                     model = ac.Field<string>("Model"),
                                     mes = rep.Field<string>("Message"),
                                     dt = rep.Field<DateTime>("DateTimeEvents")
                                    };

            if (repair.Count() == 0)
            {
                Console.WriteLine("Нет событий {0}", tea);
            }
            else
            {
                foreach (var item in repair)
                {
                    Console.WriteLine("Событие: {0} -> ID Самолета: {1} Модель: {2}  Дата/время события: {3} Состояние: {4} _Count {5}",item.mes, item.id, item.model,  item.dt, tea, repair.Count());

                }
            }
            
            Console.WriteLine("\n----------------------------------");
        }
      
        public void TypeAircraftEventReport(Aircraft currentAir, TypeEventAircraft tea) 
        {
            Console.WriteLine("\n----------------------------------");
            var taer = from t_event in dtOp.AsEnumerable()
                            join ac in dtAC.AsEnumerable() on t_event.Field<int>("IDAircraft") equals ac.Field<int>("IDAircraft")
                            join ta in dtTA.AsEnumerable() on t_event.Field<int>("IDTypeAircraft") equals ta.Field<int>("IDTypeAircraft")
                            join ev in dtEv.AsEnumerable() on t_event.Field<int>("IDEvent") equals ev.Field<int>("IDEvent")
                            where
                            /*t_event.Field<int>("IDAircraft") == currentAir.IDAcr*/
                            t_event.Field<int>("IDTypeAircraft") == getID_TypeAirCraft(currentAir) && t_event.Field<int>("IDEvent") == (int)tea
                            select new
                            {
                                model = ac.Field<string>("Model"),
                                number = t_event.Field<int>("IDOperationAir"),
                                typeac = ta.Field<string>("NameTypeAircraft"),
                                message = t_event.Field<string>("Message")
                            };

            if (taer.Count() == 0)
            {
                Console.WriteLine("Нет событий {0}", tea);
            }
            else
            {
                foreach (var item in taer)
                {
                    Console.WriteLine("Cамолет {0} готов к {1} -> Номер рейса: {2}  Тип самолета: {3}  Event: {4}", item.model, item.message, item.number, item.typeac, tea);

                }
            }
            Console.WriteLine("\n----------------------------------");
        }


        /*
        public void ReportForPassengers()
        {
            // Проецировать новый результирующий набор, содержащий
            // идентификатор/цвет для строк, в которых Color = Red.
            var passengers = from pass in dtTA.AsEnumerable()
                       where
                       (string)pass["NameTypeAircraft"] == "Пассажирский"
                       select new
                       {
                           ID = (int)pass["IDTypeAircraft"]
                           //Make = (string)pass["NameTypeAircraft"]
                       };
            //Console.WriteLine("Here are the red cars we have in stock:");
            foreach (var item in passengers)
            {
                Console.WriteLine("-> CarlD = {0}", item.ID);
            }
        }
        */

    }
}
