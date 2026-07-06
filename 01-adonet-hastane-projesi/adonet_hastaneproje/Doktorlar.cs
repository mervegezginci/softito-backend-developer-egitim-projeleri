using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace adonet_hastaneproje
{
    public partial class Doktorlar : Form
    {
        public Doktorlar()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=Merve;Database=softHastane;Integrated Security=true");

        // listele fonksiyonu
        public void Listele()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DoktorListele";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dr = new DataTable();
            adapter.Fill(dr);
            dataGridView1.DataSource = dr;

            dataGridView1.Visible = true;

        }

        // poliklinikleri getir fonksiyonu
        public void PoliklinikleriGetir()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PoliklinikListele";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataRow row = dt.NewRow();
            row["PolNo"] = 0;
            row["PolAdi"] = "Seçiniz";
            dt.Rows.InsertAt(row, 0);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "PolAdi";
            comboBox1.ValueMember = "PolNo";

            comboBox1.SelectedIndex = 0;
        }



        private void Doktorlar_Load(object sender, EventArgs e)
        {
            PoliklinikleriGetir();
            dataGridView1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PoliklinikEkle";

            cmd.Parameters.AddWithValue("PolAdi", textBox1.Text);

            cmd.ExecuteNonQuery();
            conn.Close();
            Listele();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DoktorGuncelle";

            cmd.Parameters.AddWithValue("@DoktorNo", Convert.ToInt32(textBox1.Tag));
            cmd.Parameters.AddWithValue("@AdSoyad", textBox1.Text);
            cmd.Parameters.AddWithValue("@Maas", Convert.ToDecimal(textBox2.Text));
            cmd.Parameters.AddWithValue("@PolNo", comboBox1.SelectedValue);

            cmd.ExecuteNonQuery();
            conn.Close();
            Listele();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DoktorSil";
            cmd.Parameters.AddWithValue("DoktorNo", Convert.ToInt32(textBox1.Tag));
            cmd.ExecuteNonQuery();
            conn.Close();
            Listele();

            textBox1.Clear();
            textBox2.Clear();
            textBox5.Clear();

            textBox1.Tag = "";

            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int satir = dataGridView1.SelectedCells[0].RowIndex;
            textBox1.Tag = dataGridView1.Rows[satir].Cells[0].Value.ToString();
            textBox1.Text = dataGridView1.Rows[satir].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[satir].Cells[2].Value.ToString();
            comboBox1.SelectedValue = dataGridView1.Rows[satir].Cells[3].Value;
        }

        // doktorlar menü butonu
        private void doktorlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Doktorlar frm = new Doktorlar();
            frm.Show();
            this.Hide();
        }

        // hastalar menü butonu
        private void hastalarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hastalar frm = new Hastalar();
            frm.Show();
            this.Close();
        }

        // poliklinik menü butonu
        private void polikliniklerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Poliklinikler frm = new Poliklinikler();
            frm.Show();
            this.Close();
        }

        // kayıtlar menü butonu
        private void kayıtlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kayitlar frm = new Kayitlar();
            frm.Show();
            this.Close();
        }

        // raporlar menü butonu
        private void raporlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rapor frm = new Rapor();
            frm.Show();
            this.Close();
        }

        // arama butonu
        private void button5_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DoktorAra";

            cmd.Parameters.AddWithValue("@AdSoyad", textBox5.Text);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
        }
    }
}
