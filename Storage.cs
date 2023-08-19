using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace AircraftProject
{
    public class Storage<T> : System.Collections.IEnumerator          //Хранилище объектов типа T
    {
        T _currentobj = default(T);         //default - значение по умолчанию текущего объекта_(currentobj)
        protected List<T> _objs;
        int _pos;                           //позиция

        public Storage()
        {
            //List<int> _obj = new List<int>() { 1, 2, 3, 45 };
            //_obj = new List<T>();                                             //_objs - массив
                                                                                //obj - элемент массива
            _objs = new List<T>();                                              //Массив хранящий все Obj находящиеся в хранилище
            _pos = -1;
           
        }
        
        public void Dispose()                   //Метод Dispose позволяет в любой момент времени вызвать освобождение связанных ресурсов
        {                                       //Глава 20. Сборка мусора, управление памятью и указатели
            _currentobj = default(T);           //Нужно ли (T)?
            _objs.Clear();
            _pos = -1;
        }

        public int AddBasic(T objAdd)
        {
            try
            {
                _objs.Add(objAdd);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine("Add aircraft = {0}", objAdd);
            }
            return _objs.Count - 1;
        }

        public void RemoveBasic(T objRem)
        {
            try
            {
                _objs.Remove(objRem);

            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex.Message);
                Console.WriteLine("Remove aircraft {0}", objRem);
            }
        }

        public void RemoveIndex(int index)
        {
            _objs.RemoveAt(index);                                       //Удаляет из списка элемент по индексу index
        }

        public object Current { get { return _currentobj; } }       //Текущий элемент в контейнере
                                                     //Свойство Current возвращает объект в последовательности, на который указывает указатель

        public bool MoveNext()                      //Перемещение на одну позицию вперед в контейнере элементов
        {                                           //Метод MoveNext() перемещает указатель на текущий элемент на следующую позицию в последов. 
            if (_pos < _objs.Count - 1)             //Если последовательность еще не закончилась, то возвращает true.
            {                                       //Если же последовательность закончилась, то возвращается false.
                _pos++;
                _currentobj = _objs[_pos];
                return true;
            }
            else
            {
                _currentobj = _objs[_pos];
                return false;
            }
        }

        public void Reset()                                       //Перемещение в начало контейнера
        {                                                         //Метод Reset() сбрасывает указатель позиции в начальное положение
            {
                _currentobj = default(T);
                _pos = -1;
            }
        }

        
        public int GetCount() { return _objs.Count; }       //посчитать массив объектов

        public bool IsContains(T selObj)      //Содержит      
        {                                     //bool - хранит значение true or false
            return _objs.Contains(selObj);
        }

        public T this[int index]              //индексатор
        {
            get
            {
                if (index >= 0 && index < _objs.Count) return (T)_objs[index];
                else throw new Exception("Out of Range");                       //"получить диапазон"
            }   //с помощью оператора /throw/ мы сами можем создать исключение и вызвать его в процессе выполнения

            set { if (index == _objs.Count) this.AddBasic(value); else _objs[index] = value; }
        }

        public IEnumerator<T> GetEnumerator()                   //Перечисление
        {
            return _objs.GetEnumerator();

        }

        public IEnumerable<T> GetOnlyType<T1>()                 //Выборка
        {
            for (int i = 0; i < _objs.Count; i++)
            {
                if (_objs[i] is T)
                {
                    yield return (T)(object)_objs[i];           //yeild - автоматически выводит список;
                }                                               //перебирает набор значений;
            }
        }
        
        }
    }
