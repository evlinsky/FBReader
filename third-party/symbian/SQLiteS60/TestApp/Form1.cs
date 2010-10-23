using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLiteClient;

namespace TestApp
{
    public partial class Form1 : Form
    {
        SQLiteConnection _Connection;

        public Form1()
        {
            InitializeComponent();
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
        
        private void Form1_Load(object sender, EventArgs e)
        {
            _Connection = new SQLiteConnection("Data Source=test1.db;NewDatabase=True;Synchronous=Off;Encoding=UTF8;Emulator=true");
            _Connection.Open();
            InitializeTables();

            SQLiteCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = "select id, items, customer, amount from orders";

            DataTable table = new DataTable();

            table.Columns.Add("Id");
            table.Columns.Add("Items");
            table.Columns.Add("Customer");
            table.Columns.Add("Amount");

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DataRow row = table.NewRow();
                row["Id"] = reader.GetInt32(0);
                row["Items"] = reader.GetInt32(1);
                row["Customer"] = reader.GetString(2);
                row["Amount"] = reader.GetDouble(3);
                table.Rows.Add(row);
            }

            dataGridView1.DataSource = table;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            int orderId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);

            SQLiteCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = "select id, orderid, article from orderdetails where OrderId = @OrderId";
            cmd.Parameters.Add("@OrderId", DbType.Int32).Value = orderId;

            DataTable table = new DataTable();

            table.Columns.Add("Id");
            table.Columns.Add("OrderId");
            table.Columns.Add("Article");

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DataRow row = table.NewRow();
                row["Id"] = reader.GetInt32(0);
                row["OrderId"] = reader.GetInt32(1);
                row["Article"] = reader.GetString(2);
                table.Rows.Add(row);
            }

            dataGridView2.DataSource = table;

        }
    }
}