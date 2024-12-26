using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    public class Product:IComparable<Product>
    {
        private string name;
        private double price;
        private string unit;
        private double count;

        public string Name { get => name; set => name = value; }
        public double Price { get => price; set => price = value; }
        public string Unit { get => unit; set => unit = value; }
        public double Count { get => count; set => count = value; }

        public Product(string name,double price,string unit, double count) {
            this.Name = name;
            this.Price = price;
            this.unit = unit;
            this.count = count;
        }

        public override string ToString()
        {
            if (Count != 0)
            {
                return Name + " | " + Count.ToString() + Unit + " | cena za jednotku : " + Price.ToString() + ",-";
            }
            else {
                return Name;
            }
        }

        public int CompareTo(Product other)
        {
            if (other == null)
                return 1;

            else
                return this.Name.CompareTo(other.Name);
        }
    }
}
