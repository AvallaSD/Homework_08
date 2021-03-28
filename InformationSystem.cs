using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using static System.Console;

namespace Homework_08
{
    class InformationSystem
    {
        List<Worker> Workers { get; set; }

        List<Departament> Departaments { get; set; }


        public InformationSystem()
        {
            Workers = new List<Worker>();
            Departaments = new List<Departament>();
        }

        /// <summary>
        /// Запуск базы данных
        /// </summary>
        public void Run()
        {
            OpenMainMenu();
        }

        /// <summary>
        /// Запуск главного меню
        /// </summary>
        private void OpenMainMenu()
        {
            Clear();
            bool flag = true;
            while (flag)
            {
                Clear();
                WriteLine("Главное меню:");
                WriteLine("1) Добавить сотрудника \n2) Удалить сотрудника \n3) Редактировать информацию о сотруднике\n4) Печать базы\n5) Записать XML\n6) Прочитать XML\n7) Записать JSON\n8) Прочитать JSON\n9) Сортировка\n0) Выход");
                switch (ReadKey().KeyChar)
                {
                    case '1':
                        AddWorker();
                        break;
                    case '2':
                        DeleteByIndex();
                        break;
                    case '3':
                        EditByIndex();
                        break;
                    case '4':
                        Clear();
                        WriteLine(ToString());
                        ReadKey();
                        break;
                    case '5':
                        SerialiseWorker();
                        break;
                    case '6':
                        DeserialiseWorker();
                        break;
                    case '7':
                        SerialiseWorkerJSON();
                        break;
                    case '8':
                        DeserialiseWorkerJSON();
                        break;
                    case '9':
                        Sort();
                        break;
                    case '0':
                        flag = false;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Sort()
        {
            WriteLine("Выберите свойство для сортировки:");
            var propertiesToSort = new List<PropertyInfo>();
            int des = GetAndSelectWorkerProperty(out propertiesToSort);
            var property = propertiesToSort[des];

            Workers = Workers.OrderBy(x=>property.GetValue(x)).ToList();
        }

        /// <summary>
        /// Печать базы данных в консоль
        /// </summary>
        public override string ToString()
        {
            string rez = "      Id          Name        Surname  Age   Department    Salary    Project Amount\n";
            Workers.ForEach(x => rez += $"{x.Id,8}{x.Name,14}{x.Surname,15}{x.Age,5}{x.Departament,13}{x.Salary,10}{x.ProjectsAmount,18}\n");
            return rez;
        }

        /// <summary>
        /// Добавление сотрудника
        /// </summary>
        private void AddWorker()
        {
            Console.Clear();
            int id = Workers.Count;
            Console.WriteLine("Добавление сотрудника в базу:");

            Console.Write("Имя: ");
            string name = Console.ReadLine();

            Console.Write("Фамилия: ");
            string surname = Console.ReadLine();

            Console.Write("Возраст: ");
            int age = InputInt();

            Console.Write("Департамент: ");
            string departament = Console.ReadLine();
            AddWorkersDepartament(departament);

            Console.Write("Зарплата: ");
            int salary = InputInt();

            Console.Write("Количество проектов: ");
            int projectsAmount = InputInt();

            Workers.Add(new Worker { Id = id, Age = age, Departament = departament, Name = name, ProjectsAmount = projectsAmount, Salary = salary, Surname = surname });
        }

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        private void DeleteByIndex()
        {
            if (Workers.Count == 0)
            {
                WriteLine("База пуста. Нажмите любую клавишу длz возврата в меню");
                ReadKey();
                return;
            }

            Clear();
            WriteLine(ToString());

            Console.WriteLine("Удаление сотрудника из базы. Введите индекс:");

            int index = InputInt();
            while (Workers.Count <= index)
            {
                Console.WriteLine("Сотрудника с таким индексом не существует. Повторите ввод");
                index = InputInt();
            }
            DoWithDepartment(Workers[index].Name, x => x.WorkersAmount--);
            Workers.RemoveAt(index);

        }

        /// <summary>
        /// Редактирование информации о сотруднике
        /// </summary>
        private void EditByIndex()
        {

            if (Workers.Count == 0)
            {
                WriteLine("База пуста. Нажмите любую клавишу длz возврата в меню");
                ReadKey();
                return;
            }

            Clear();
            WriteLine(ToString());
            WriteLine("Редактирование информации о сотруднике. Введите индекс сотрудника");



            int index = InputInt();

            while (index < 0 || index >= Workers.Count())
            {
                WriteLine("Неверный индекс. Повторите ввод: ");
                index = InputInt();
            }

            WriteLine("Выберите свойство, которое хотите изменить:");

            var propertiesToEdit = new List<PropertyInfo>();

            int des = GetAndSelectWorkerProperty(out propertiesToEdit);

            WriteLine("Введите новое значение: ");
            var editingWorker = Workers[index];

            if (propertiesToEdit[des].PropertyType == typeof(int))
            {
                int intValue = InputInt();
                propertiesToEdit[des].SetValue(editingWorker, intValue);
            }
            else
            {
                string strValue = ReadLine();
                if (propertiesToEdit[des].Name == "Departament")
                {
                    DoWithDepartment(editingWorker.Departament, x => x.WorkersAmount--);
                    AddWorkersDepartament(strValue);
                }
                propertiesToEdit[des].SetValue(editingWorker, strValue);
            }
        }

        /// <summary>
        /// Запись базы в XML
        /// </summary>
        private void SerialiseWorker()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Worker>));
            using (Stream str = new FileStream("Workers.xml", FileMode.OpenOrCreate))
            {
                serializer.Serialize(str, Workers);
            }
        }

        /// <summary>
        /// Чтение базы из XML
        /// </summary>
        private void DeserialiseWorker()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Worker>));
            using (Stream str = new FileStream("Workers.xml", FileMode.OpenOrCreate))
            {
                Workers = serializer.Deserialize(str) as List<Worker>;
            }
        }

        /// <summary>
        /// Запись базы в JSON
        /// </summary>
        private void SerialiseWorkerJSON()
        {
            File.WriteAllText("Workers.json", JsonConvert.SerializeObject(Workers));
        }

        /// <summary>
        /// Чтение базы из JSON
        /// </summary>
        private void DeserialiseWorkerJSON()
        {
            Workers = JsonConvert.DeserializeObject<List<Worker>>(File.ReadAllText("Workers.json"));
        }

        private int InputInt()
        {
            int intValue;
            while (!int.TryParse(Console.ReadLine(), out intValue))
            {
                Console.WriteLine("Неверный формат ввода. Повторите ввод:");

            }
            return intValue;
        }

        private void DoWithDepartment(string name, Action<Departament> action)
        {
            Departaments.Where(x => x.Name == name).ToList().ForEach(action);
        }

        private void AddWorkersDepartament(string depName)
        {
            if (!Departaments.Any(x => x.Name == depName))
            {
                Departaments.Add(new Departament(depName));
            }
            else
            {
                DoWithDepartment(depName, x => x.WorkersAmount++);
            }
        }

        private int GetAndSelectWorkerProperty(out List<PropertyInfo> properties)
        {
            var propertiesProto = typeof(Worker).GetProperties().Where(x => x.Name != "Id").ToList();
            propertiesProto.ForEach(x => WriteLine($"{propertiesProto.IndexOf(x) + 1}) {x.Name}"));


            int des = InputInt() - 1;

            while (des < 0 || des >= propertiesProto.Count())
            {
                WriteLine("Неверно выбрано свойство. Повторите ввод: ");
                des = InputInt() - 1;
            }
            properties = propertiesProto;
            return des;
        }
    }
}
