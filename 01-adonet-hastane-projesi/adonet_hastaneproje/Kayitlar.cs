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
    public partial class Kayitlar : Form
    {
        public Kayitlar()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=Merve;Database=softHastane;Integrated Security=true");


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

        private void Kayitlar_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            HastaGetir();

        }

        // listele fonksiyonu
        public void Listele()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaBilgiListele";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Visible = true;

        }

        // hasta getir fonksiyonu
        public void HastaGetir()
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaListele";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DataRow bosSatir = dt.NewRow();

            bosSatir["AdSoyad"] = "Hasta Seçiniz";

            dt.Rows.InsertAt(bosSatir, 0);

            comboBox1.DisplayMember = "AdSoyad";
            comboBox1.ValueMember = "HastaNo";
            comboBox1.DataSource = dt;

            comboBox1.SelectedIndex = 0;

            conn.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int satir = dataGridView1.SelectedCells[0].RowIndex;
            textBox1.Tag = dataGridView1.Rows[satir].Cells[0].Value.ToString();
            comboBox1.SelectedValue = Convert.ToInt32(dataGridView1.Rows[satir].Cells[1].Value);
            textBox1.Text = dataGridView1.Rows[satir].Cells[2].Value.ToString();
            textBox2.Text = dataGridView1.Rows[satir].Cells[3].Value.ToString();
            textBox3.Text = dataGridView1.Rows[satir].Cells[4].Value.ToString();
            textBox4.Text = dataGridView1.Rows[satir].Cells[5].Value.ToString();
            textBox5.Text = dataGridView1.Rows[satir].Cells[6].Value.ToString();
            textBox6.Text = dataGridView1.Rows[satir].Cells[7].Value.ToString();
        }

        // kaydet butonu
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                MessageBox.Show("Lütfen hasta seçiniz");
                return;
            }
            conn.Open();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaBilgiEkle";

            cmd.Parameters.AddWithValue("@HastaNo", comboBox1.SelectedValue);
            cmd.Parameters.AddWithValue("@Kilo", textBox1.Text);
            cmd.Parameters.AddWithValue("@Boy", textBox2.Text);
            cmd.Parameters.AddWithValue("@Tansiyon", textBox3.Text);
            cmd.Parameters.AddWithValue("@Nabiz", textBox4.Text);
            cmd.Parameters.AddWithValue("@Seker", textBox5.Text);
            cmd.Parameters.AddWithValue("@Teshis", textBox6.Text);

            cmd.ExecuteNonQuery();

            conn.Close();

            Listele();
        }

        // güncelle butonu
        private void button3_Click(object sender, EventArgs e)
        {

            conn.Open();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaBilgiGuncelle";
            cmd.Parameters.AddWithValue("@BilgiNo", textBox1.Tag);
            cmd.Parameters.AddWithValue("@HastaNo", comboBox1.SelectedValue);
            cmd.Parameters.AddWithValue("@Kilo", textBox1.Text);
            cmd.Parameters.AddWithValue("@Boy", textBox2.Text);
            cmd.Parameters.AddWithValue("@Tansiyon", textBox3.Text);
            cmd.Parameters.AddWithValue("@Nabiz", textBox4.Text);
            cmd.Parameters.AddWithValue("@Seker", textBox5.Text);
            cmd.Parameters.AddWithValue("@Teshis", textBox6.Text);

            cmd.ExecuteNonQuery();

            conn.Close();

            Listele();
        }

        // sil butonu
        private void button4_Click(object sender, EventArgs e)
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaBilgiSil";

            cmd.Parameters.AddWithValue("@BilgiNo", textBox1.Tag);

            cmd.ExecuteNonQuery();

            conn.Close();

            Listele();

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();

            textBox1.Tag = "";

            comboBox1.SelectedIndex = 0;
        }

        // listele butonu
        private void button1_Click(object sender, EventArgs e)
        {
            Listele();
        }

        // arama butonu
        private void button5_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaBilgiAra";

            cmd.Parameters.AddWithValue("@AdSoyad", textBox7.Text);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;
        }
    }
}
