using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using System.Data;

namespace DisconnectedVersie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        DataSet DS = new DataSet();
        bool isNieuw;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //CreateTables();
            //CreateSeedings();
            ReadXML();
            VulCmbSoorten();
            VulLstAdressen();

            grpAdressen.IsEnabled = true;
            grpGegevens.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteXML();

        }
        private void ReadXML()
        {
            string XMLMap = Directory.GetCurrentDirectory() + "/XMLBestanden"; 
            string XMLBestand = Directory.GetCurrentDirectory() + "/XMLBestanden/adressen.xml";
            if (!Directory.Exists(XMLMap)) 
                Directory.CreateDirectory(XMLMap); 
            if(!File.Exists(XMLBestand))
            {
                CreateTables();
                CreateSeedings();
            }
            else
            {
                DS.ReadXml(XMLBestand, XmlReadMode.ReadSchema);
            }
        }
        private void WriteXML()
        {
            string XMLMap = Directory.GetCurrentDirectory() + "/XMLBestanden";
            string XMLBestand = Directory.GetCurrentDirectory() + "/XMLBestanden/adressen.xml";
            if (!Directory.Exists(XMLMap))
                Directory.CreateDirectory(XMLMap);
            if (File.Exists(XMLBestand))
            {
                File.Delete(XMLBestand); 
            }
            DS.WriteXml(XMLBestand, XmlWriteMode.WriteSchema);
        }
        public void CreateTables()
        {
            // datatable Soorten aanmaken
            DataTable dtSoorten = new DataTable();
            dtSoorten.TableName = "Soorten";

            // datacolumn's aanmaken
            DataColumn dc;
            dc = new DataColumn();
            dc.ColumnName = "ID";
            dc.DataType = typeof(string);
            dtSoorten.Columns.Add(dc);
            dtSoorten.PrimaryKey = new DataColumn[] { dc };

            dc = new DataColumn();
            dc.ColumnName = "soort";
            dc.DataType = typeof(string);
            dc.Unique = true;
            dc.AllowDBNull = false;
            dtSoorten.Columns.Add(dc);

            DS.Tables.Add(dtSoorten);


            // datatable Adressen aanmaken
            DataTable dtAdressen = new DataTable();
            dtAdressen.TableName = "Adressen";

            // datacolumn's aanmaken
            dc = new DataColumn();
            dc.ColumnName = "ID";
            dc.DataType = typeof(string);
            dtAdressen.Columns.Add(dc);
            dtAdressen.PrimaryKey = new DataColumn[] { dc };

            dc = new DataColumn();
            dc.ColumnName = "naam";
            dc.DataType = typeof(string);
            dc.AllowDBNull = false;
            dtAdressen.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "adres";
            dc.DataType = typeof(string);
            dtAdressen.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "post";
            dc.DataType = typeof(string);
            dtAdressen.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "gemeente";
            dc.DataType = typeof(string);
            dtAdressen.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "land";
            dc.DataType = typeof(string);
            dtAdressen.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "soort_id";
            dc.DataType = typeof(string);
            dtAdressen.Columns.Add(dc);

            DS.Tables.Add(dtAdressen);

            DS.Relations.Add(DS.Tables["Soorten"].Columns["ID"], DS.Tables["Adressen"].Columns["soort_id"]);
        }
        public void CreateSeedings()
        {
            DataRow dr = DS.Tables["soorten"].NewRow();
            dr["ID"] = Guid.NewGuid().ToString();
            dr["soort"] = "Familie";
            DS.Tables["soorten"].Rows.Add(dr);

            dr = DS.Tables["soorten"].NewRow();
            dr["ID"] = Guid.NewGuid().ToString();
            dr["soort"] = "Vrienden";
            DS.Tables["soorten"].Rows.Add(dr);

            dr = DS.Tables["soorten"].NewRow();
            dr["ID"] = Guid.NewGuid().ToString();
            dr["soort"] = "Klanten";
            DS.Tables["soorten"].Rows.Add(dr);

        }
        private void VulCmbSoorten()
        {
            cmbSoorten.Items.Clear();
            ComboBoxItem itm;
            foreach (DataRow rw in DS.Tables["soorten"].Rows)
            {
                itm = new ComboBoxItem();
                itm.Tag = rw["id"];
                itm.Content = rw["soort"];
                cmbSoorten.Items.Add(itm);
            }
        }
        private void VulLstAdressen()
        {
            lstAdressen.Items.Clear();
            ListBoxItem itm;
            foreach (DataRow rw in DS.Tables["adressen"].Rows)
            {
                itm = new ListBoxItem();
                itm.Tag = rw["id"];
                itm.Content = rw["naam"];
                lstAdressen.Items.Add(itm);
            }

        }
        private void MaakLeeg()
        {
            txtNaam.Text = "";
            txtAdres.Text = "";
            txtPost.Text = "";
            txtGemeente.Text = "";
            txtLand.Text = "";
            cmbSoorten.SelectedIndex = -1;

        }
        private void lstAdressen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MaakLeeg();
            if (lstAdressen.SelectedIndex == -1) return;

            ListBoxItem litm = (ListBoxItem)lstAdressen.SelectedItem;
            DataRow dr = DS.Tables["adressen"].Select("id=" + litm.Tag.ToString()).FirstOrDefault();
            if (dr == null) return;

            txtNaam.Text = dr["naam"].ToString();
            txtAdres.Text = dr["adres"].ToString();
            txtPost.Text =  dr["post"].ToString();
            txtGemeente.Text = dr["gemeente"].ToString();
            txtLand.Text = dr["land"].ToString();
            string soort_id = dr["soort_id"].ToString();

            int indeks = 0;
            foreach(ComboBoxItem citm in cmbSoorten.Items)
            {
                if(citm.Tag.ToString() == soort_id)
                {
                    cmbSoorten.SelectedIndex = indeks;
                    break;
                }
                indeks++;
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            isNieuw = true;
            grpAdressen.IsEnabled = false;
            grpGegevens.IsEnabled = true;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            MaakLeeg();
            txtNaam.Focus();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            isNieuw = false;

            grpAdressen.IsEnabled = false;
            grpGegevens.IsEnabled = true;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;

            txtNaam.Focus();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem litm = (ListBoxItem)lstAdressen.SelectedItem;
            int id = int.Parse(litm.Tag.ToString());

            DataRow dr = DS.Tables["adressen"].Select("id=" + id.ToString()).FirstOrDefault();
            if (dr == null)
            {
                MessageBox.Show("ERROR : PK not found", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            dr.Delete();
            VulLstAdressen();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(txtNaam.Text.Trim() == "")
            {
                MessageBox.Show("Naam invoeren !", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNaam.Focus();
                return;
            }
            if(cmbSoorten.SelectedIndex == -1)
            {
                MessageBox.Show("Adressoort selecteren !", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                cmbSoorten.Focus();
                return;

            }
            if (isNieuw)
            {
                DataRow dr = DS.Tables["adressen"].NewRow();
                dr["ID"] = Guid.NewGuid().ToString();
                dr["naam"] = txtNaam.Text;
                dr["adres"] = txtAdres.Text;
                dr["post"] = txtPost.Text;
                dr["gemeente"] = txtGemeente.Text;
                dr["land"] = txtLand.Text;
                ComboBoxItem itm = (ComboBoxItem)cmbSoorten.SelectedItem;
                dr["soort_id"] = itm.Tag.ToString();
                try
                {
                    DS.Tables["adressen"].Rows.Add(dr);
                }
                catch (Exception fout)
                {
                    MessageBox.Show(fout.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                ListBoxItem litm = (ListBoxItem)lstAdressen.SelectedItem;
                int id = int.Parse(litm.Tag.ToString());

                DataRow dr = DS.Tables["adressen"].Select("id=" + id.ToString()).FirstOrDefault();
                if (dr == null)
                {
                    MessageBox.Show("ERROR : PK not found", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    dr["naam"] = txtNaam.Text;
                    dr["adres"] = txtAdres.Text;
                    dr["post"] = txtPost.Text;
                    dr["gemeente"] = txtGemeente.Text;
                    dr["land"] = txtLand.Text;
                    ComboBoxItem citm = (ComboBoxItem)cmbSoorten.SelectedItem;
                    dr["soort_id"] = int.Parse(citm.Tag.ToString());
                }
                catch (Exception fout)
                {
                    MessageBox.Show(fout.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            grpAdressen.IsEnabled = true;
            grpGegevens.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;


            VulLstAdressen();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            grpAdressen.IsEnabled = true;
            grpGegevens.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;
            lstAdressen_SelectionChanged(null, null);
            lstAdressen.Focus();
        }


    }
}
