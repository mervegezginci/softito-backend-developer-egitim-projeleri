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

namespace adonetPastaneProje
{
    public partial class Urunler : Form
    {
        public Urunler()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=Merve;Database=softPastane;Integrated Security=true");

        // listele
        public void Listele(string listele)
        {
            SqlDataAdapter dr = new SqlDataAdapter(listele, conn);
            DataSet ds = new DataSet();
            dr.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
        }

        // listele butonu
        private void button1_Click(object sender, EventArgs e)
        {
            Listele("Select * From Urun");
        }

        // kaydet butonu
        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand komut = new SqlCommand("insert into Urun(UrunAdi,Fiyat,Stok) values (@UrunAdi,@Fiyat,@Stok)", conn);

            komut.Parameters.AddWithValue("@UrunAdi", textBox2.Text);
            komut.Parameters.AddWithValue("@Fiyat", textBox3.Text);
            komut.Parameters.AddWithValue("@Stok", textBox4.Text);

            komut.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Ürün başarıyla kaydedildi!");
            Listele("Select * From Urun");

            // Temizlik işlemi
            textBox2.Tag = null;
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // sil butonu
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.Tag == null)
            {
                MessageBox.Show("Lütfen önce tablodan silinecek bir ürün seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            conn.Open();
            SqlCommand komut = new SqlCommand("delete from Urun where UrunNo=@UrunNo", conn);
            komut.Parameters.AddWithValue("@UrunNo", textBox2.Tag.ToString());

            komut.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Ürün başarıyla silindi!");
            Listele("Select * From Urun");

            textBox2.Tag = null;
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // güncelle butonu
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Tag == null)
            {
                MessageBox.Show("Lütfen önce tablodan güncellenecek bir ürün seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            conn.Open();
            SqlCommand komut = new SqlCommand("update Urun set UrunAdi=@UrunAdi, Fiyat=@Fiyat, Stok=@Stok where UrunNo=@UrunNo", conn);

            komut.Parameters.AddWithValue("@UrunAdi", textBox2.Text);
            komut.Parameters.AddWithValue("@Fiyat", Convert.ToDecimal(textBox3.Text));
            komut.Parameters.AddWithValue("@Stok", Convert.ToInt32(textBox4.Text));
            komut.Parameters.AddWithValue("@UrunNo", Convert.ToInt32(textBox2.Tag));

            komut.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("Ürün bilgileri başarıyla güncellendi!");
            Listele("Select * From Urun");

            textBox2.Tag = null;
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            string sorgu = "SELECT * FROM Urun WHERE UrunAdi LIKE @p";

            SqlDataAdapter da = new SqlDataAdapter(sorgu, conn);
            da.SelectCommand.Parameters.AddWithValue("@p", "%" + textBox1.Text + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow satir = dataGridView1.Rows[e.RowIndex];

                textBox2.Tag = satir.Cells["UrunNo"].Value.ToString();
                textBox2.Text = satir.Cells["UrunAdi"].Value.ToString();
                textBox3.Text = satir.Cells["Fiyat"].Value.ToString();
                textBox4.Text = satir.Cells["Stok"].Value.ToString();
            }
        }

        private void ürünlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Urunler frm = new Urunler();
            frm.Show();
            this.Close();
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

        private void raporlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rapor frm = new Rapor();
            frm.Show();
            this.Close();
        }
    }
}
