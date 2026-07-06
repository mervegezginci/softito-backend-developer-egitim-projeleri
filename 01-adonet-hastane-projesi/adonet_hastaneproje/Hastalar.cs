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
    public partial class Hastalar : Form
    {
        public Hastalar()
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
            cmd.CommandText = "HastaListele";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dr = new DataTable();
            adapter.Fill(dr);
            dataGridView1.DataSource = dr;

            dataGridView1.Visible = true;
        }

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

        // arama butonu
        private void button5_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaAra";

            cmd.Parameters.AddWithValue("@AdSoyad", textBox5.Text);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
        }


        // listele butonu
        private void button1_Click(object sender, EventArgs e)
        {
            Listele();
        }

        // kaydet butonu
        private void button2_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(comboBox1.SelectedValue) == 0)
            {
                MessageBox.Show("Lütfen poliklinik seçiniz!");
                return;
            }
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaEkle";

            cmd.Parameters.AddWithValue("AdSoyad", textBox1.Text);
            cmd.Parameters.AddWithValue("Yas", textBox2.Text);
            cmd.Parameters.AddWithValue("Telefon", textBox3.Text);
            cmd.Parameters.AddWithValue("Adres", textBox4.Text);
            cmd.Parameters.AddWithValue("@PolNo", comboBox1.SelectedValue);

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
            cmd.CommandText = "HastaGuncelle";
            cmd.Parameters.AddWithValue("@HastaNo", Convert.ToInt32(textBox1.Tag));
            cmd.Parameters.AddWithValue("AdSoyad", textBox1.Text);
            cmd.Parameters.AddWithValue("Yas", textBox2.Text);
            cmd.Parameters.AddWithValue("Telefon", textBox3.Text);
            cmd.Parameters.AddWithValue("Adres", textBox4.Text);
            cmd.Parameters.AddWithValue("@PolNo", comboBox1.SelectedValue);

            cmd.ExecuteNonQuery();
            conn.Close();
            Listele();
        }

        // sil butonu
        private void button4_Click(object sender, EventArgs e)
        {
            conn.Open();

            SqlCommand kontrol = new SqlCommand("select count(*) from HastaBilgi where HastaNo=@HastaNo", conn);
            kontrol.Parameters.AddWithValue("@HastaNo", Convert.ToInt32(textBox1.Tag));
            int sayi = (int)kontrol.ExecuteScalar();

            if (sayi > 0)
            {
                MessageBox.Show(
                    "Bu hastaya ait kayıtlar var. Önce Hasta Bilgilerini siliniz!",
                    "Silme Engellendi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                conn.Close();
                return;
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HastaSil";
            cmd.Parameters.AddWithValue("HastaNo", Convert.ToInt32(textBox1.Tag));
            cmd.ExecuteNonQuery();
            conn.Close();
            Listele();

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();

            textBox1.Tag = "";

            comboBox1.SelectedIndex = 0;
        }


        // datagrid ekranı 
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int satir = dataGridView1.SelectedCells[0].RowIndex;
            textBox1.Tag = dataGridView1.Rows[satir].Cells[0].Value.ToString();
            textBox1.Text = dataGridView1.Rows[satir].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[satir].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.Rows[satir].Cells[3].Value.ToString();
            textBox4.Text = dataGridView1.Rows[satir].Cells[4].Value.ToString();
            comboBox1.SelectedValue = dataGridView1.Rows[satir].Cells[5].Value;

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

        private void Hastalar_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            PoliklinikleriGetir();
        }

    }
}
