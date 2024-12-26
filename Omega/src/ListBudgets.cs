﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Omega
{
    public partial class ListBudgets : Form
    {
        public ListBudgets()
        {
            this.FormClosed += Program.MyClosedHandler;
            InitializeComponent();

            
        }

        /// <summary>
        /// Method <c>back_button_Click</c> hides current form and shows Menu form
        /// </summary>
        private void back_button_Click(object sender, EventArgs e)
        {
            this.Hide();
            var menu = new Menu();
            menu.Show();
        }

        /// <summary>
        /// Method <c>RozpocetList_Load</c> loads all the Rozpocet in Rozpocet.GetAllByNickname and fills it into lst_listbox
        /// </summary>
        private void ListBudgets_Load(object sender, EventArgs e)
        {
            foreach (Budget r in Budget.GetAllByNickname())
            {
                this.list_listBox.Items.Add(r);
            }
            this.list_listBox.MouseDoubleClick += new MouseEventHandler(list_listBox_MouseDoubleClick);
        }
        /// <summary>
        /// Method <c>list_listBox_MouseDoubleClick</c> gets the index of double-clicked item from the list_listbox and closes current form and opens the RozpocetShow and passes it the id from the selected item
        /// </summary>
        private void list_listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.list_listBox.IndexFromPoint(e.Location);

            if (index != System.Windows.Forms.ListBox.NoMatches)

            {
                this.Hide();
                int id = ((Budget)this.list_listBox.Items[index]).Id;
                var RozpocetShow = new ShowBudget(id);
                RozpocetShow.Show();


            }
        }
        /// <summary>
        /// Method <c>open_button_Click</c> hides current form and shows RozpocetShow form and passes it the id from selected item from the list_listbox
        /// </summary>
        private void open_button_Click(object sender, EventArgs e)
        {
            if (this.list_listBox.SelectedItem != null) {
                this.Hide();
                int id = ((Budget)this.list_listBox.SelectedItem).Id;
                MessageBox.Show(id.ToString());
                var RozpocetShow = new ShowBudget(id);
                RozpocetShow.Show();
            }
        }
        /// <summary>
        /// Method <c>remove_button_Click</c> removes items from productList and product_list listBox but only if some item was selected
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.list_listBox.Items.Contains(this.list_listBox.SelectedItem))
                {
                    this.list_listBox.Items.Add(this.list_listBox.SelectedItem);
                    this.list_listBox.Items.Remove(this.list_listBox.SelectedItem);
                    Budget.delete(((Budget)this.list_listBox.SelectedItem).Id);


                }
                else
                {
                    Budget.delete(((Budget)this.list_listBox.SelectedItem).Id);
                    this.list_listBox.Items.Remove(this.list_listBox.SelectedItem);
                   


                }

            }
            catch
            {
                MessageBox.Show("Nebylo vybráno nic k odebrání");
            }

        }
        /// <summary>
        /// Method <c>vytisknout_button_Click</c> opens the save dialog if some item was selected
        /// </summary>
        private void vytisknout_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.list_listBox.Items.Contains(this.list_listBox.SelectedItem))
                {
                    MessageBox.Show("Nebylo vybráno nic k exportu");
                }
                else
                {
                    this.save_file_dialog.ShowDialog();
                }
            }
            catch {
                MessageBox.Show("Nebylo vybráno nic k exportu");
            }
        }
        /// <summary>
        /// Method <c>vytisknout_button_Click</c> creates a pdf from selected rozpocet and saves it into the selected path
        /// </summary>
        private void save_file_dialog_FileOk(object sender, CancelEventArgs e)
        {

            try
            {
                if (this.list_listBox.Items.Contains(this.list_listBox.SelectedItem))
                {
                    try
                    {
                        Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                        Budget selectedRozpocet = (Budget)this.list_listBox.SelectedItem;
                        PdfWriter.GetInstance(document, new FileStream(this.save_file_dialog.FileName, FileMode.Create));

                        document.Open();


                        BaseFont font = BaseFont.CreateFont(@"C:\Windows\Fonts\dejavusans.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        iTextSharp.text.Font normalFont = new iTextSharp.text.Font(font, 12, iTextSharp.text.Font.NORMAL);
                        document.Add(new Paragraph(selectedRozpocet.CustName + " " + selectedRozpocet.CustSurname, normalFont));
                        PdfPTable table = new PdfPTable(1);
                        Phrase phrase = new Phrase("Rozpočet", normalFont);
                        phrase.Font.Size = 20;
                        PdfPCell cell = new PdfPCell(phrase);
                        cell.Border = 0;
                        cell.Rowspan = 3;

                        cell.HorizontalAlignment = 1;
                        table.PaddingTop = 25;
                        table.AddCell(cell);

                        double total = 0;
                        foreach (Product p in selectedRozpocet.Products)
                        {
                            Paragraph paragraph = new Paragraph(p.Name + " | " + p.Count.ToString() + " " + p.Unit + " | " + p.Price.ToString() + ",- /" + p.Unit, normalFont);
                            paragraph.Font.Size = 8;
                            table.AddCell(paragraph);
                            total += (p.Count * p.Price);
                        }
                        document.Add(table);
                        
                        document.Add(new Paragraph("Celkem: " + total.ToString() + ",-",normalFont));
                        document.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Nelze uložit pdf");
                    }

                }
                else
                {
                    MessageBox.Show("Nebylo vybráno nic k exportu");


                }

            }
            catch
            {
                MessageBox.Show("Nebylo vybráno nic k exportu");
            }
            

            
        }
    }
}
