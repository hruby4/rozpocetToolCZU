using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;

namespace Omega
{
    public class Budget
    {
        private List<Product> products;
        private string date;
        private string custName;
        private string custSurname;
        private int id;

        public List<Product> Products { get => products; set => products = value; }
        public string Date { get => date; set => date = value; }
        public string CustName { get => custName; set => custName = value; }
        public string CustSurname { get => custSurname; set => custSurname = value; }
        public int Id { get => id; set => id = value; }

        public Budget(List<Product> products, int id, string date, string custName, string custSurname)
        {
            this.Products = products;
            this.Date = date;
            this.custName = custName;
            this.custSurname = custSurname;
            this.Id = id;
        }
        /// <summary>
        /// Method <c>getLastId</c> gets the max id from rozpocet table
        /// </summary>
        /// <returns>
        /// max id
        /// </returns>
        public static int getLastId()
        {
            MySqlConnection conn = DatabaseConnection.getConnection();
            var command = new MySqlCommand();
            command.Connection = conn;
            command.CommandText = ("select max(id) from rozpocet;");
            int id = System.Convert.ToInt32(command.ExecuteScalar().ToString());
            DatabaseConnection.closeConnection();
            
            return id;
        }
        /// <summary>
        /// Method <c>editList</c> edits the rozpocet table and the produkt table in database with parameters
        /// </summary>
        /// <returns>
        /// true if there will be no problem then false
        /// </returns>
        public static bool editList(int id,List<Product> products,string custName,string custSurname)
        {
            var upload = new MySqlCommand();
            MySqlConnection conn = DatabaseConnection.getConnection();
            upload.Connection = conn;
            try
            {
                upload.CommandText = ("update rozpocet set jmeno_zak = @cust_name, prijmeni_zak = @cust_surname where id = @rozpocet_id");
                upload.Parameters.AddWithValue("rozpocet_id", id);
                upload.Parameters.AddWithValue("cust_name", custName);
                upload.Parameters.AddWithValue("cust_surname", custSurname);
                upload.ExecuteNonQuery();

                var deleteProducts = new MySqlCommand();
                deleteProducts.Connection = conn;
                deleteProducts.CommandText = ("delete from produkt where rozpocet_id = @rozpocet_id;");
                deleteProducts.Parameters.AddWithValue("rozpocet_id", id);
                deleteProducts.ExecuteNonQuery();

                MessageBox.Show(products.Count.ToString());

                foreach (Product p in products)
                {
                    var insert_produkt = new MySqlCommand();
                    insert_produkt.Connection = conn;
                    insert_produkt.CommandText = ("insert into produkt(nazev,rozpocet_id,jednotka,pocet,cena) values(@nazev,@rozpocet_id,@jednotka,@pocet,@cena);"); ;
                    insert_produkt.Parameters.AddWithValue("nazev", p.Name);
                    insert_produkt.Parameters.AddWithValue("rozpocet_id", id);
                    insert_produkt.Parameters.AddWithValue("jednotka", p.Unit.ToString());
                    insert_produkt.Parameters.AddWithValue("pocet", p.Count);
                    insert_produkt.Parameters.AddWithValue("cena", p.Price);
                    insert_produkt.ExecuteNonQuery();

                }

            }
            catch
            {
                throw new Exception("Nelze vytvorit rozpocet.");
            }
            finally {
                DatabaseConnection.closeConnection();
            }




            return true;
        }
        /// <summary>
        /// Method <c>uploadList</c> inserts in the rozpocet table and produkt in database with parameters
        /// </summary>
        /// <returns>
        /// true if there will be no problem then false
        /// </returns>
        public static void uploadList(List<Product> products, string custName, string custSurname)
        {
            int userId = User.getIdByUserNickname();
            var upload = new MySqlCommand();
            MySqlConnection conn = DatabaseConnection.getConnection();
            upload.Connection = conn;
            try
            {
                upload.CommandText = ("INSERT INTO rozpocet (user_id, datum, jmeno_zak, prijmeni_zak) " +
                    "VALUES (@user_id, @date, @cust_name, @cust_surname); " +
                    "SELECT LAST_INSERT_ID();");
                upload.Parameters.AddWithValue("user_id", userId);
                Console.WriteLine(DateTime.Now.ToString("yyyy-m-d"));
                upload.Parameters.AddWithValue("date", DateTime.Now.ToString("yyyy-MM-dd"));
                upload.Parameters.AddWithValue("cust_name", custName);
                upload.Parameters.AddWithValue("cust_surname", custSurname);

                int budgetId = Convert.ToInt32(upload.ExecuteScalar());

                foreach (Product p in products)
                {
                    var insert = new MySqlCommand();
                    insert.Connection = conn;
                    insert.CommandText = ("insert into produkt(nazev,rozpocet_id,jednotka,pocet,cena) values(@nazev,@rozpocet_id,@jednotka,@pocet,@cena);"); ;
                    insert.Parameters.AddWithValue("nazev", p.Name);
                    insert.Parameters.AddWithValue("rozpocet_id", budgetId);
                    insert.Parameters.AddWithValue("jednotka", p.Unit.ToString());
                    insert.Parameters.AddWithValue("pocet", p.Count);
                    insert.Parameters.AddWithValue("cena", p.Price);
                    insert.ExecuteNonQuery();

                }
                DatabaseConnection.closeConnection();

            }
            catch (Exception error)
            {
                DatabaseConnection.closeConnection();
                throw new Exception("Nastal problem pri ukladani rozpoctu.\n " + error.Message);
            }
        }
        /// <summary>
        /// Method <c>GetAllByNickname</c> gets all produkt tables with the user nickname that is logged in now
        /// </summary>
        /// <returns>
        /// list made of rozpocet instances
        /// </returns>
        public static List<Budget> GetAllByNickname()
        {
            int userId = User.getIdByUserNickname();
            MySqlConnection conn = DatabaseConnection.getConnection();
            var select = new MySqlCommand();
            List<Budget> budgets = new List<Budget>();
            select.Connection = conn;
            select.CommandText = ("select id,datum,jmeno_zak,prijmeni_zak from rozpocet where user_id = @user_id");
            select.Parameters.AddWithValue("user_id",userId);
            try
            {
                MySqlDataReader reader = select.ExecuteReader();
                

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string date = reader.GetMySqlDateTime(1).ToString();
                    string custName = reader.GetString(2);
                    string custSurname = reader.GetString(3);
                    budgets.Add(new Budget(new List<Product>(), id, date, custName, custSurname));
                }

                reader.Close();
                
                foreach (var r in budgets)
                {
                    var insert = new MySqlCommand();
                    insert.Connection = conn;
                    insert.CommandText = ("select nazev,jednotka,pocet,cena from produkt where rozpocet_id = @rozpocet_id");
                    insert.Parameters.AddWithValue("rozpocet_id", r.Id);
                    MySqlDataReader reader2 = insert.ExecuteReader();
                    List<Product> productList = new List<Product>();

                    while (reader2.Read())
                    {
                        string name = reader2.GetString(0);
                        string unit = reader2.GetString(1);
                        double count = reader2.GetDouble(2);
                        double price = reader2.GetDouble(3);
                        productList.Add(new Product(name, price, unit, count));
                    }

                    reader2.Close();
                    r.Products = productList;
                }
            }
            catch (Exception error){
                DatabaseConnection.closeConnection();
                throw new Exception("Nelze ziskat rozpocty. \n " + error.Message);
            }

            DatabaseConnection.closeConnection();
            return budgets;
        }

        /// <summary>
        /// Method <c>delete</c> deletes from produkt and rozpocet tables where id is equal to parameter
        /// </summary>
        /// <returns>
        /// list made of rozpocet instances
        /// </returns>
        public static void delete(int id)
        {
            try
            {
                MySqlConnection conn = DatabaseConnection.getConnection();
                var delete = new MySqlCommand();
                delete.Connection = conn;
                delete.CommandText = ("delete from produkt where rozpocet_id = @rozpocet_id;");
                delete.Parameters.AddWithValue("rozpocet_id", id);
                delete.ExecuteNonQuery();
                var deleteRozpocet = new MySqlCommand();
                deleteRozpocet.Connection = conn;
                deleteRozpocet.CommandText = ("delete from rozpocet where id = @id;");
                deleteRozpocet.Parameters.AddWithValue("id", id);
                deleteRozpocet.ExecuteNonQuery();

                DatabaseConnection.closeConnection();
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }
            finally {
                DatabaseConnection.closeConnection();
            }
  
        }

        public override string ToString()
        {
            return "Čislo: " + Id.ToString() + " | Zákazník: " + CustName + " " + CustSurname + " | Datum: " + Date;
        }

    }
}
