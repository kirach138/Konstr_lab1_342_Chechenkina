using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace grocerystore
{
    public partial class Form1 : Form
    {
        private string connectionString;
        //"Data Source=LAPTOP-T5OQM7VD;Initial Catalog=Chechenkina342;Integrated Security=True";
        private DataTable resultTable = new DataTable();
        private DataTable resultTable2 = new DataTable();
        
        private DataTable resultTable4 = new DataTable();
        private DataTable resultTable5 = new DataTable();
        private SqlDataAdapter Adapter1;
        private SqlDataAdapter Adapter2;
        private SqlDataAdapter Adapter3;
        private SqlDataAdapter Adapter4;

        private SqlCommandBuilder Builder1 = new SqlCommandBuilder();
        private SqlCommandBuilder Builder2 = new SqlCommandBuilder();
        private SqlCommandBuilder Builder4 = new SqlCommandBuilder();

        private DataSet dataSet = new DataSet();

        public Form1()
        {
            InitializeComponent();
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TestConnection"].ConnectionString;
            Adapter1 = new SqlDataAdapter("Select * from Thing", connectionString);
            Adapter2 = new SqlDataAdapter("Select * from Office", connectionString);
            Adapter3 = new SqlDataAdapter("Select * from Cabinet_type", connectionString);
            Adapter4 = new SqlDataAdapter("Select * from Division", connectionString);

            Builder1 = new SqlCommandBuilder(Adapter1);
            Builder2 = new SqlCommandBuilder(Adapter2);
            Builder4 = new SqlCommandBuilder(Adapter4);
            Adapter1.Fill(dataSet, "Thing");
            Adapter2.Fill(dataSet, "Office");
            Adapter3.Fill(dataSet, "Cabinet_type");
            Adapter4.Fill(dataSet, "Division");
            Adapter1.Fill(resultTable2);
            Adapter4.Fill(resultTable5);

            results2.DataSource = dataSet.Tables["Thing"];
            results3.DataSource = dataSet.Tables["Office"];
            result4.DataSource = dataSet.Tables["Cabinet_type"];
            res5.DataSource = dataSet.Tables["Division"];
            FillCombobox1();
        }

        private void FillCombobox1()
        {
            ((DataGridViewComboBoxColumn)results3.Columns["Type_of"]).DataSource =
                dataSet.Tables["Cabinet_type"];
            ((DataGridViewComboBoxColumn)results3.Columns["Type_of"]).DisplayMember =
                "Name";
            ((DataGridViewComboBoxColumn)results3.Columns["Type_of"]).ValueMember =
                "ID_of_cabinet_type";
        }
        private void Sel2()
        {
            DataRow[] foundRows = resultTable2.Select($"ID_of_thing_type".ToString()+$"= {txt2.Text}");
            DataTable resultTable21=new DataTable();
            resultTable21.Columns.Add("ID_of_thing");
            resultTable21.Columns.Add("ID_of_thing_type");
            resultTable21.Columns.Add("Cost");
            resultTable21.Columns.Add("Start_date");
            resultTable21.Columns.Add("End_date");
            foreach (DataRow row in foundRows)
            {
                DataRow r = resultTable21.NewRow();
                r["ID_of_thing"] = row["ID_of_thing"];
                r["ID_of_thing_type"] = row["ID_of_thing_type"];
                r["Cost"] = row["Cost"];
                r["Start_date"] = row["Start_date"];
                r["End_date"] = row["End_date"];
                resultTable21.Rows.Add(r);
            }
            results2.DataSource = resultTable21;
            
        }
        private void Add4()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
//                DECLARE @max_value INT;
//                Select @max_value = MAX(ID_of_cabinet_type)
//FROM Cabinet_type;
//                INSERT INTO Cabinet_type(ID_of_cabinet_type, Name) VALUES(@max_value + 1, 'aaaaa')
                //INSERT INTO Cabinet_type(ID_of_cabinet_type, Name) VALUES (5, 'aaaaa')
                //DELETE FROM Cabinet_type WHERE ID_of_cabinet_type=5
                SqlCommand sqlCommand =
                    new SqlCommand("DECLARE @max_value INT;\r\nSelect @max_value = MAX(ID_of_cabinet_type)\r\nFROM Cabinet_type;\r\nINSERT INTO Cabinet_type(ID_of_cabinet_type, Name) VALUES (@max_value+1, '" + Name4.Text +"')"
                                  , sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                sqlDataReader.Close();
                Adapter3.Update(dataSet, "Cabinet_type");
                resultTable4.Clear();
                Adapter3.Fill(resultTable4);
                result4.DataSource = resultTable4;
            }
        }

        private void Del4()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                
                    sqlConnection.Open();
                    SqlCommand sqlCommand =
                        new SqlCommand("DELETE FROM Cabinet_type WHERE Name='"+ Name4.Text+"'"
                                      , sqlConnection);
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    sqlDataReader.Close();
                    Adapter3.Update(dataSet, "Cabinet_type");
                    resultTable4.Clear();
                    Adapter3.Fill(resultTable4);
                    result4.DataSource = resultTable4;
                    
                
                
            }
        }
        //private void buttonReport2_Click(object sender, EventArgs e)
        //{
        //    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        //    {
        //        SqlDataAdapter sqlAdapter = new SqlDataAdapter("Orders_getByMonth", sqlConnection);
        //        sqlAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

        //        sqlAdapter.SelectCommand.Parameters.Add(new SqlParameter("@month", SqlDbType.Int));
        //        sqlAdapter.SelectCommand.Parameters["@month"].Value = numericUpDown1.Value;

        //        DataSet dataSet = new DataSet();
        //        sqlAdapter.Fill(dataSet, "report2");

        //        dataGridViewReport2.DataSource = dataSet.Tables["report2"];
        //    }
        //}

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            SqlConnection connection = null;
            try
            {
                if (!int.TryParse(txtYear.Text, out int year))
                {
                    MessageBox.Show("Введите корректный код вида аудиторий.");
                    return;
                }
                
                string sql = @"
                SELECT
                    a.ID_of_office, a.Number, a.Square, a.Capacity, a.The_method_of_protection, a.Equipment_cost, a.ID_of_division, a.ID_of_building
                FROM
                Office a, Cabinet_type b
                WHERE
                a.ID_of_cabinet_type = b.ID_of_cabinet_type and b.ID_of_cabinet_type = @cod";

                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@cod", year);

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                connection.Open();
                resultTable.Clear();
                adapter.Fill(resultTable);
                dgvResults.DataSource = resultTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void txtYear_Enter(object sender, EventArgs e)
        {
            if (txtYear.Text == "Введите код")
            {
                txtYear.Text = "";
                txtYear.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtYear_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtYear.Text))
            {
                txtYear.Text = "Введите код";
                txtYear.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void but4_Click(object sender, EventArgs e)
        {
            Add4();
        }

        private void butdel4_Click(object sender, EventArgs e)
        {
            Del4();
        }

        private void but2_Click(object sender, EventArgs e)
        {
            Sel2();
        }

        private void proc()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                //sqlCommand.CommandText = "DECLARE @max_value INT;\r\nSelect @max_value = MAX(ID_of_division)\r\nFROM Division;";
                //int iddd = 1 + ((int)sqlCommand.ExecuteScalar());
                int iddd=0;

                DataRow[] foundRows = resultTable5.Select($"ID_of_division> {iddd}");
                
                foreach (DataRow row in foundRows)
                {
                    if (row.Field<int>("ID_of_division") > iddd) iddd = row.Field<int>("ID_of_division");
                }
                iddd = iddd + 1;
                Adapter4 = new SqlDataAdapter("Select * from Division", sqlConnection);
                Builder4 = new SqlCommandBuilder(Adapter4);
                Adapter4.InsertCommand = new SqlCommand("cr", sqlConnection);
                Adapter4.InsertCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter p1 = new SqlParameter
                {
                    ParameterName = "@name",
                    Value=txt5.Text
                };
                
                Adapter4.InsertCommand.Parameters.Add(p1);
                SqlParameter p2 = new SqlParameter
                {
                    ParameterName = "@ID",
                    Value = iddd
                };
                


                Adapter4.InsertCommand.Parameters.Add(p2);
                Adapter4.InsertCommand.ExecuteNonQuery();
                //SqlParameter parameter = Adapter2.InsertCommand.Parameters.Add("")
                Adapter4.Update(dataSet, "Division");
                resultTable5.Clear();
                Adapter4.Fill(resultTable5);
                res5.DataSource = resultTable5;
            }
        }

        private void bsave_Click(object sender, EventArgs e)
        {
            Adapter2.Update(dataSet, "Office");
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            proc();
        }
    }
}