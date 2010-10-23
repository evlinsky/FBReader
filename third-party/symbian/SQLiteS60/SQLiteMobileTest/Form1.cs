#region Using directives

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Data.SQLiteClient;
using System.Data.SQLiteClient.Native;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

#endregion

namespace SQLiteTest
{
    /// <summary>
    /// Summary description for form.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        private MainMenu mainMenu1;
        private MenuItem menuItem1;
        private MenuItem menuItem2;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private MenuItem menuItem3;
        private MenuItem menuItem4;
        private MenuItem menuItem5;
        SQLiteConnection _Connection;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.MenuItems.Add(this.menuItem3);
            this.menuItem2.MenuItems.Add(this.menuItem4);
            this.menuItem2.MenuItems.Add(this.menuItem5);
            this.menuItem2.Text = "Tests";
            // 
            // menuItem3
            // 
            this.menuItem3.Text = "SQLite Version";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Text = "Fill Database";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Text = "Show Orders Table";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.Add(this.columnHeader1);
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Size = new System.Drawing.Size(205, 174);
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Orders Table";
            this.columnHeader1.Width = 200;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(176, 180);
            this.Controls.Add(this.listView1);
            this.Menu = this.mainMenu1;
            this.Text = "SQLite Test";
            this.Load += new System.EventHandler(this.Form1_Load);

        }

        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Application.Run(new Form1());
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            _Connection.Close();
            Close();
        }

        private void AddOrder(SQLiteTransaction trans, int id, int items, string customer, double amount)
        {
            SQLiteCommand cmd = _Connection.CreateCommand("insert into Orders (Id, Items, Customer, Amount) values (@Id, @Items, @Customer, @Amount)");
            cmd.Transaction = trans;

            cmd.Parameters.Add("@Id", DbType.Int32).Value = id;
            cmd.Parameters.Add("@Items", DbType.Int32).Value = items;
            cmd.Parameters.Add("@Customer", DbType.String).Value = customer;
            cmd.Parameters.Add("@Amount", DbType.Single).Value = amount;

            cmd.ExecuteNonQuery();
        }

        private void AddOrderDetail(SQLiteTransaction trans, int id, int orderId, string article)
        {
            SQLiteCommand cmd = _Connection.CreateCommand("insert into OrderDetails (Id, OrderId, Article) values (@Id, @OrderId, @Article)");
            cmd.Transaction = trans;

            cmd.Parameters.Add("@Id", DbType.Int32).Value = id;
            cmd.Parameters.Add("@OrderId", DbType.Int32).Value = orderId;
            cmd.Parameters.Add("@Article", DbType.String).Value = article;

            cmd.ExecuteNonQuery();
        }

        private void InitializeTables()
        {
            SQLiteCommand cmd = _Connection.CreateCommand();

            cmd.CommandText = "create table Orders (Id int, items int, Customer varchar(50), amount float)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "create table OrderDetails (Id int, OrderId int, article varchar(50))";
            cmd.ExecuteNonQuery();

            SQLiteTransaction trans = _Connection.BeginTransaction();

            AddOrder(trans, 1, 3, "Peter Parker", 250.30);
            AddOrder(trans, 2, 2, "Superman", 504);
            AddOrder(trans, 3, 1, "Indiana Jones", 96.34);
            AddOrder(trans, 4, 4, "Dark Vader", 1111.11);

            AddOrderDetail(trans, 1, 1, "costume");
            AddOrderDetail(trans, 2, 1, "snacks");
            AddOrderDetail(trans, 3, 1, "shoes");

            AddOrderDetail(trans, 4, 2, "costume");
            AddOrderDetail(trans, 5, 2, "hairgel");

            AddOrderDetail(trans, 6, 3, "whip");

            AddOrderDetail(trans, 7, 4, "helmet");
            AddOrderDetail(trans, 8, 4, "sword");
            AddOrderDetail(trans, 9, 4, "cape");
            AddOrderDetail(trans, 10, 4, "The force");

            trans.Commit();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("SQLite Version: {0}", _Connection.SQLiteVersion));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _Connection = new SQLiteConnection("Data Source=c:\\data\\redfivelabs\\temp\\SQLiteTest.sqlite;NewDatabase=True;Synchronous=Off;Encoding=UTF8;Emulator=false");
            _Connection.Open();
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeTables();
                MessageBox.Show("Tables filled.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            try
            {
                listView1.Items.Clear();

                SQLiteCommand cmd = _Connection.CreateCommand("select id, items, customer, amount from orders");
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //ListViewItem item = listView1.Items.Add(new ListViewItem(reader.GetInt32(0).ToString()));
                    //item.SubItems.Add(reader.GetInt32(1).ToString());
                    //item.SubItems.Add(reader.GetString(2));
                    //item.SubItems.Add(reader.GetInt32(3).ToString());
                    string line = string.Format("Id: {0}, Items: {1}, Customer: {2}, Amount: {3}", reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3));
                    listView1.Items.Add(new ListViewItem(line));
                }
                reader.Close();

                listView1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


    }
}

