using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace adonetPastaneProje
{
    public partial class Musteriler : Form
    {
        public Musteriler()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=Merve;Database=softPastane;Integrated Security=true");

        public void Listele(string listele)
        {
            SqlDataAdapter dr = new SqlDataAdapter(listele, conn);
            DataSet ds = new DataSet();
            dr.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Listele("Select * From Musteriler");
        }

        // kaydet butonu
        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand komut = new SqlCommand("insert into musteriler(AdSoyad, Telefon) values (@AdSoyad, @Telefon)", conn);

            komut.Parameters.AddWithValue("@AdSoyad", textBox1.Text);
            komut.Parameters.AddWithValue("@Telefon", textBox2.Text);

            komut.ExecuteNonQuery();
            conn.Close();
            Listele("Select * From musteriler");
        }

        // güncelle butonu
        private void button3_Click(object sender, EventArgs e)
        {
            conn.Open();
            // musterNo'nun textBox1.Tag içinde saklandığını varsayıyoruz
            SqlCommand komut = new SqlCommand("update musteriler set AdSoyad=@AdSoyad, Telefon=@Telefon where musteriNo=@musteriNo", conn);

            komut.Parameters.AddWithValue("@AdSoyad", textBox1.Text);
            komut.Parameters.AddWithValue("@Telefon", textBox2.Text);
            komut.Parameters.AddWithValue("@musteriNo", textBox1.Tag);

            komut.ExecuteNonQuery();
            conn.Close();

            Listele("Select * From musteriler");
        }

        // sil butonu
        private void button4_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand komut = new SqlCommand("delete from musteriler where musteriNo=@musteriNo", conn);
            komut.Parameters.AddWithValue("@musteriNo", textBox1.Tag);

            komut.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Müşteri başarıyla silindi!");
            Listele("Select * From musteriler");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string aramaSorgusu = "Select * From Musteriler Where AdSoyad Like '%" + textBox3.Text + "%'";
            Listele(aramaSorgusu);
        }

        private void müşterilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Musteriler frm = new Musteriler();
            frm.Show();
            this.Close();
        }

        private void siparişlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Siparisler frm = new Siparisler();
            frm.Show();
            this.Close();
        }

        private void ürünlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Urunler frm = new Urunler();
            frm.Show();
            this.Close();
        }

        private void raporlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rapor frm = new Rapor();
            frm.Show();
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            textBox1.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString(); 
            textBox2.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();

            textBox1.Tag = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
        }
    }
}