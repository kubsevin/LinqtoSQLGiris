using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinqtoSQLGiris
{
    public partial class Form1 : Form
    {
        NorthwindDataContext ctx = new NorthwindDataContext();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = ctx.Products;

            comboKategori.DisplayMember = "CategoryName";
            comboKategori.ValueMember = "CategoryID";
            comboKategori.DataSource = ctx.Categories;

            comboTedarikci.DisplayMember = "CompanyName";
            comboTedarikci.ValueMember = "SupplierID";
            comboTedarikci.DataSource = ctx.Suppliers;

            //kategori ve tedarikci adının dgv'a gelmesi için şunlar eklenir:
            //products ile category tablolarını join et.
            var sonuc = from urun in ctx.Products
                        join kat in ctx.Categories
                        on urun.CategoryID equals kat.CategoryID
                        join ted in ctx.Suppliers
                        on urun.SupplierID equals ted.SupplierID
                        select new // aşağıdaki beş kolondan yeni bir entitiy tipi. (anonim)
                        {
                            urun.ProductID,
                            urun.ProductName,
                            urun.UnitPrice,
                            urun.UnitsInStock,
                            kat.CategoryName,
                            urun.CategoryID,
                            urun.SupplierID
                        };

            
            dataGridView1.DataSource = sonuc;
            dataGridView1.Columns["CategoryID"].Visible = false;
            dataGridView1.Columns["SupplierID"].Visible = false;

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Product p = new Product();
            p.ProductName = txtProductName.Text;
            p.UnitPrice = nudFiyat.Value;
            p.UnitsInStock = Convert.ToInt16(nudStok.Value);
            p.CategoryID = (int)comboKategori.SelectedValue;
            p.SupplierID = (int)comboTedarikci.SelectedValue;

            ctx.Products.InsertOnSubmit(p);
            MessageBox.Show($"SubmitChanges oncesi ProductID = {p.ProductID}");
            ctx.SubmitChanges(); // değişiklikleri ADO.net koduna çevirerek veri tabanına gönder.

            MessageBox.Show($"SubmitChanges sonrası ProductID= {p.ProductID}");

            //refresh the grid
            dataGridView1.DataSource = ctx.Products.GetNewBindingList();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            //once context'ten sil, sonra onayla (veritabanında sil.)
            int urunID = (int)dataGridView1.CurrentRow.Cells["ProductID"].Value;
            Product p = ctx.Products.SingleOrDefault(urun => urun.ProductID == urunID);
            ctx.Products.DeleteOnSubmit(p);
            ctx.SubmitChanges();

            dataGridView1.DataSource = ctx.Products.GetNewBindingList();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow r = dataGridView1.CurrentRow;
            txtProductName.Text = r.Cells["ProductName"].Value.ToString();
            nudFiyat.Value = Convert.ToDecimal(r.Cells["UnitPrice"].Value);
            nudStok.Value = Convert.ToDecimal(r.Cells["UnitsInStock"].Value);
            comboKategori.SelectedValue = r.Cells["CategoryName"].Value;
            comboTedarikci.SelectedValue = (int)r.Cells["SupplierID"].Value;
            txtProductName.Tag = r.Cells["ProductID"].Value;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            NorthwindDataContext ctx = new NorthwindDataContext();
            Product p = ctx.Products.SingleOrDefault(urun => urun.ProductID == (int)txtProductName.Tag);
            p.ProductName = txtProductName.Text;
            p.UnitPrice = nudFiyat.Value;
            p.UnitsInStock = Convert.ToInt16(nudStok.Value);
            p.CategoryID = (int)comboKategori.SelectedValue;
            p.SupplierID = (int)comboTedarikci.SelectedValue;

            ctx.SubmitChanges(); //değişiklikleri ADO.net koduna çevirerek veritabanına gönder.

            //refresh the grid

            dataGridView1.DataSource = ctx.Products;
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            NorthwindDataContext ctx = new NorthwindDataContext();
            dataGridView1.DataSource = ctx.Products.Where(x => x.ProductName.Contains(txtAra.Text));
        }
    }
}
