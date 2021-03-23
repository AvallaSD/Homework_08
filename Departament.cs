using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_08
{
    class Departament
    {
        public Departament(string name)
        {
            Name = name;
            CreationDate = DateTime.Now;
            WorkersAmount = 0;
        }

        public string Name { get; private set; }
        public DateTime CreationDate { get; set; }
        public int WorkersAmount { get; set; }

        
    }
}
