using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace Omega
{
    public partial class ShowBudget : Form
    {
        List<Product> products = new List<Product>();
        int id;
        Budget budget = null;
        double total = 0;
        public ShowBudget(int id)
        {
            this.FormClosed += Program.MyClosedHandler;
            this.id = id;
            InitializeComponent();
            try
            {
                foreach (Budget r in Budget.GetAllByNickname())
                {
                    if (r.Id == id)
                    {
                        budget = r;
                        break;
                    }
                }
                this.custName_textbox.Text = budget.CustName;
                this.CustSurname_textbox.Text = budget.CustSurname;
                foreach (Product p in budget.Products)
                {
                    this.product_list.Items.Add(p);
                    this.products.Add(p);
                    this.total += (p.Price * p.Count);
                }
                this.total_label.Text = this.total.ToString() + ",-";
            }
            catch (Exception error) { MessageBox.Show(error.Message); }

        }

        /// <summary>
        /// Method <c>add_button_Click</c> gets texts from textBoxes and appends them to productList and product_list listBox
        /// </summary>
        private void add_button_Click(object sender, EventArgs e)
        {
            try
            {
                string name = this.name_textbox.Text;
                string unit = this.jednotka_select.Text;
                double count = System.Convert.ToDouble(this.count_textbox.Text);
                double price = System.Convert.ToDouble(this.price_textbox.Text);
                if (count > 0)
                {
                    this.total += (count * price);
                    this.total_label.Text = this.total.ToString() + ",-";
                    products.Add(new Product(name, price, unit, count));
                    this.product_list.Items.Add(new Product(name, price, unit, count));
                    this.name_textbox.Text = null;
                    this.jednotka_select.Text = null;
                    this.price_textbox.Text = null;
                    this.count_textbox.Text = null;
                }
                else
                {
                    MessageBox.Show("Počet musí být číslo vetší než nula");
                }

            }
            catch
            {
                MessageBox.Show("Špatně zadané hodnoty");
            }
        }
        /// <summary>
        /// Method <c>remove_button_Click</c> removes items from productList and product_list listBox but only if some item was selected
        /// </summary>
        private void remove_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.product_list.Items.Contains(this.product_list.SelectedItem))
                {
                    this.product_list.Items.Add(this.product_list.SelectedItem);
                    this.product_list.Items.Remove(this.product_list.SelectedItem);

                }
                else
                {
                    foreach (var x in this.products)
                    {
                        if (((Product)this.product_list.SelectedItem).Name == x.Name)
                        {
                            this.total -= (x.Count * x.Price);
                            this.total_label.Text = this.total.ToString() + ",-"; ;
                            this.products.Remove(x);
                            break;
                        }

                    }
                    this.product_list.Items.Remove(this.product_list.SelectedItem);


                }

            }
            catch
            {
                MessageBox.Show("Nebylo vybráno nic k odebrání");
            }
        }
        /// <summary>
        /// Method <c>back_button_Click</c> hides current form and shows RozpocetList form
        /// </summary>
        private void back_button_Click(object sender, EventArgs e)
        {
            this.Hide();
            var rozpocetList = new ListBudgets();
            rozpocetList.Show();
        }
        /// <summary>
        /// Method <c>save_button_Click</c> validates if textBoxes texts are in required patterns and then it will pass them to editList method
        /// </summary>
        private void save_button_Click(object sender, EventArgs e)
        {
            if (RegexValidator.one_nonDigit_Word(this.CustSurname_textbox.Text) && RegexValidator.one_nonDigit_Word(this.custName_textbox.Text))
            {
                if (this.product_list.Items.Count == 0)
                {
                    MessageBox.Show("Nezadali jste žádné položky do seznamu.");
                }
                else
                {
                    try { Budget.editList(this.id, this.products, this.custName_textbox.Text, this.CustSurname_textbox.Text); }
                    catch (Exception error) { MessageBox.Show(error.Message); }
                    this.budget.CustName = this.custName_textbox.Text;
                    this.budget.CustSurname = this.CustSurname_textbox.Text;
                    this.budget.Products = this.products;
                    MessageBox.Show("Upraveno");
                    this.Hide();
                    var menu = new Menu();
                    menu.Show();
                }
            }
            else
            {
                MessageBox.Show("Špatně zadané hodnoty jména nebo příjmení zákazníka");
            }
        }


    }
}
