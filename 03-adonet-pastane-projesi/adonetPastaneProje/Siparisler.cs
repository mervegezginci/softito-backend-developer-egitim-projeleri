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
    public partial class Siparisler : Form
    {
        public Siparisler()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=Merve;Database=softPastane;Integrated Security=true");

        // listele metotu
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
            Listele("SELECT * FROM Siparis");
        }

        // kaydet butonu
        private void button2_Click(object sender, EventArgs e)
        {
            decimal toplamFiyat = 0;
            int toplamAdet = 0; // Toplam adeti tutacak değişken

            foreach (FlowLayoutPanel panel in flowLayoutPanel1.Controls)
            {
                CheckBox cb = panel.Controls[0] as CheckBox;
                NumericUpDown num = panel.Controls[1] as NumericUpDown;

                if (cb.Checked)
                {
                    toplamAdet += (int)num.Value; // Adetleri topla

                    int urunNo = Convert.ToInt32(cb.Tag);
                    SqlCommand cmd = new SqlCommand("SELECT Fiyat FROM Urun WHERE UrunNo=@p", conn);
                    cmd.Parameters.AddWithValue("@p", urunNo);

                    if (conn.State == ConnectionState.Closed) conn.Open();
                    decimal fiyat = Convert.ToDecimal(cmd.ExecuteScalar());
                    conn.Close();

                    toplamFiyat += fiyat * num.Value;
                }
            }

            conn.Open();
            string siparisSorgu = "INSERT INTO Siparis (MusteriNo, SiparisTarihi, Adet, ToplamFiyat) VALUES (@MusteriNo, @SiparisTarihi, @Adet, @ToplamFiyat); SELECT SCOPE_IDENTITY();";
            SqlCommand cmdSiparis = new SqlCommand(siparisSorgu, conn);
            cmdSiparis.Parameters.AddWithValue("@MusteriNo", Convert.ToInt32(comboBox1.SelectedValue));
            cmdSiparis.Parameters.AddWithValue("@SiparisTarihi", dateTimePicker1.Value);
            cmdSiparis.Parameters.AddWithValue("@Adet", toplamAdet);
            cmdSiparis.Parameters.AddWithValue("@ToplamFiyat", toplamFiyat);

            // Yeni oluşan SiparisNo'yu alıyoruz
            int yeniSiparisNo = Convert.ToInt32(cmdSiparis.ExecuteScalar());
            conn.Close();

            // seçilen ürünleri SiparisDetay tablosuna ekle
            foreach (FlowLayoutPanel panel in flowLayoutPanel1.Controls)
            {
                CheckBox cb = panel.Controls[0] as CheckBox;
                NumericUpDown num = panel.Controls[1] as NumericUpDown; // Adeti al

                if (cb.Checked)
                {
                    SqlCommand cmdDetay = new SqlCommand("INSERT INTO SiparisDetay (SiparisNo, UrunNo, Adet) VALUES (@sn, @un, @adet)", conn);
                    cmdDetay.Parameters.AddWithValue("@sn", yeniSiparisNo);
                    cmdDetay.Parameters.AddWithValue("@un", cb.Tag);
                    cmdDetay.Parameters.AddWithValue("@adet", (int)num.Value); 

                    conn.Open();
                    cmdDetay.ExecuteNonQuery();
                    conn.Close();
                }
            }
            MessageBox.Show("Sipariş kaydedildi.");
            Listele("SELECT * FROM Siparis");
        }

        // güncelle butonu
        private void button3_Click(object sender, EventArgs e)
        {
            int secilenSiparisNo = Convert.ToInt32(dateTimePicker1.Tag);
            decimal toplamFiyat = 0;
            int toplamAdet = 0;

            // hesaplamalar
            foreach (FlowLayoutPanel panel in flowLayoutPanel1.Controls)
            {
                CheckBox cb = panel.Controls[0] as CheckBox;
                NumericUpDown num = panel.Controls[1] as NumericUpDown;
                if (cb.Checked)
                {
                    int urunNo = Convert.ToInt32(cb.Tag);
                    SqlCommand cmd = new SqlCommand("SELECT Fiyat FROM Urun WHERE UrunNo=@p", conn);
                    cmd.Parameters.AddWithValue("@p", urunNo);
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    decimal fiyat = Convert.ToDecimal(cmd.ExecuteScalar());
                    conn.Close();
                    toplamFiyat += fiyat * num.Value;
                    toplamAdet += (int)num.Value;
                }
            }

            // ana tabloyu güncelle
            conn.Open();
            SqlCommand cmdSiparis = new SqlCommand("UPDATE Siparis SET MusteriNo=@MusteriNo, SiparisTarihi=@SiparisTarihi, Adet=@Adet, ToplamFiyat=@ToplamFiyat WHERE SiparisNo=@SiparisNo", conn);
            cmdSiparis.Parameters.AddWithValue("@MusteriNo", comboBox1.SelectedValue);
            cmdSiparis.Parameters.AddWithValue("@SiparisTarihi", dateTimePicker1.Value);
            cmdSiparis.Parameters.AddWithValue("@Adet", toplamAdet);
            cmdSiparis.Parameters.AddWithValue("@ToplamFiyat", toplamFiyat);
            cmdSiparis.Parameters.AddWithValue("@SiparisNo", secilenSiparisNo);
            cmdSiparis.ExecuteNonQuery();

            // eski detayları sil ve yenileri ekle
            SqlCommand cmdSil = new SqlCommand("DELETE FROM SiparisDetay WHERE SiparisNo=@sn", conn);
            cmdSil.Parameters.AddWithValue("@sn", secilenSiparisNo);
            cmdSil.ExecuteNonQuery();

            foreach (FlowLayoutPanel panel in flowLayoutPanel1.Controls)
            {
                CheckBox cb = panel.Controls[0] as CheckBox;
                NumericUpDown num = panel.Controls[1] as NumericUpDown;
                if (cb.Checked)
                {
                    SqlCommand cmdEkle = new SqlCommand("INSERT INTO SiparisDetay (SiparisNo, UrunNo, Adet) VALUES (@sn, @un, @adet)", conn);
                    cmdEkle.Parameters.AddWithValue("@sn", secilenSiparisNo);
                    cmdEkle.Parameters.AddWithValue("@un", cb.Tag);
                    cmdEkle.Parameters.AddWithValue("@adet", (int)num.Value);
                    cmdEkle.ExecuteNonQuery();
                }
            }
            conn.Close();
            MessageBox.Show("Sipariş güncellendi!");
            Listele("SELECT * FROM Siparis");
        }

        // sil butonu
        private void button4_Click(object sender, EventArgs e)
        {
            conn.Open();
            int secilenSiparisNo = Convert.ToInt32(dateTimePicker1.Tag);

            // Önce detayları sil (İlişkili veriler)
            SqlCommand cmdDetaySil = new SqlCommand("DELETE FROM SiparisDetay WHERE SiparisNo=@SiparisNo", conn);
            cmdDetaySil.Parameters.AddWithValue("@SiparisNo", secilenSiparisNo);
            cmdDetaySil.ExecuteNonQuery();

            // Sonra ana siparişi sil
            SqlCommand cmdSiparis = new SqlCommand("DELETE FROM Siparis WHERE SiparisNo=@SiparisNo", conn);
            cmdSiparis.Parameters.AddWithValue("@SiparisNo", secilenSiparisNo);
            cmdSiparis.ExecuteNonQuery();

            conn.Close();
            MessageBox.Show("Sipariş başarıyla silindi!");
            Listele("SELECT * FROM Siparis");
            richTextBox1.Clear();
        }

        private void Siparisler_Load(object sender, EventArgs e)
        {
            conn.Open();

            // müşteri ComboBox doldur
            SqlCommand komutMusteri = new SqlCommand("SELECT MusteriNo, AdSoyad FROM Musteriler", conn);
            SqlDataAdapter daMusteri = new SqlDataAdapter(komutMusteri);
            DataTable dtMusteri = new DataTable();
            daMusteri.Fill(dtMusteri);

            DataRow row = dtMusteri.NewRow();
            row["MusteriNo"] = 0;
            row["AdSoyad"] = "Seçiniz";

            dtMusteri.Rows.InsertAt(row, 0);

            comboBox1.DisplayMember = "AdSoyad";
            comboBox1.ValueMember = "MusteriNo";
            comboBox1.DataSource = dtMusteri;

            flowLayoutPanel1.Controls.Clear();

            // urunleri çek
            SqlCommand cmd = new SqlCommand("SELECT UrunNo, UrunAdi FROM Urun", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            da.Fill(dt);
            conn.Close();

            // ürün sayısı kadar döngü kur
            foreach (DataRow r in dt.Rows)
            {
                // Panel oluştur
                FlowLayoutPanel panel = new FlowLayoutPanel();
                panel.Width = 250;
                panel.Height = 35; 

                // CheckBox oluştur
                CheckBox cb = new CheckBox();
                cb.Text = r["UrunAdi"].ToString();
                cb.Tag = r["UrunNo"]; // Veritabanındaki ID

                // NumericUpDown oluştur
                NumericUpDown num = new NumericUpDown();
                num.Minimum = 1;
                num.Maximum = 100;
                num.Width = 50;

                // Panele ekle
                panel.Controls.Add(cb);
                panel.Controls.Add(num);

                // Formdaki FlowLayoutPanel'e ekle
                flowLayoutPanel1.Controls.Add(panel);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sorgu = "SELECT * FROM Siparis WHERE MusteriNo IN (SELECT MusteriNo FROM Musteriler WHERE AdSoyad LIKE @p)";
            SqlDataAdapter da = new SqlDataAdapter(sorgu, conn);
            da.SelectCommand.Parameters.AddWithValue("@p", "%" + textBox1.Text + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Long;

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
            if (e.RowIndex < 0) return;

            // müşteri ve tarihi textboxlara aktar
            comboBox1.SelectedValue = dataGridView1.Rows[e.RowIndex].Cells["MusteriNo"].Value;
            dateTimePicker1.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["SiparisTarihi"].Value);
            dateTimePicker1.Tag = dataGridView1.Rows[e.RowIndex].Cells["SiparisNo"].Value;

            string siparisNo = dataGridView1.Rows[e.RowIndex].Cells["SiparisNo"].Value.ToString();
            richTextBox1.Clear();

            // sipariş detayları
            DataTable dtDetay = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("SELECT UrunNo, Adet FROM SiparisDetay WHERE SiparisNo = " + siparisNo, conn);
            da.Fill(dtDetay);

            // paneli temizle
            foreach (FlowLayoutPanel panel in flowLayoutPanel1.Controls)
            {
                CheckBox cb = panel.Controls[0] as CheckBox;
                NumericUpDown num = panel.Controls[1] as NumericUpDown;
                cb.Checked = false;
                num.Value = 1;
            }

            // Detayları RichTextBox'a yaz ve Panelde işaretle
            foreach (DataRow row in dtDetay.Rows) 
            {
                int uNo = Convert.ToInt32(row["UrunNo"]);
                int adet = Convert.ToInt32(row["Adet"]);

                SqlCommand cmdUrun = new SqlCommand("SELECT UrunAdi, Fiyat FROM Urun WHERE UrunNo = " + uNo, conn);
                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlDataReader dr = cmdUrun.ExecuteReader();
                if (dr.Read())
                {
                    richTextBox1.AppendText($"{dr["UrunAdi"]} | {adet} Adet | {Convert.ToDecimal(dr["Fiyat"]) * adet} TL\n");
                }
                dr.Close();
                conn.Close();

                foreach (FlowLayoutPanel panel in flowLayoutPanel1.Controls)
                {
                    CheckBox cb = panel.Controls[0] as CheckBox;
                    NumericUpDown num = panel.Controls[1] as NumericUpDown;

                    if (Convert.ToInt32(cb.Tag) == uNo)
                    {
                        cb.Checked = true;
                        num.Value = adet;
                    }
                }
            }
        }
    }
}
