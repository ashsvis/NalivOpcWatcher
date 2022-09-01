using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OpcClient
{
    public partial class FormOpcClient : Form
    {
        private OpcBridgeSupport _opc;
        private string server;
        private BackgroundWorker worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

        public FormOpcClient()
        {
            InitializeComponent();
            //_opc = new OpcBridgeSupport();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var opc = new OpcBridgeSupport();
            var w = (BackgroundWorker)sender;
            string server = "Lectus.OPC.1";
            while (!w.CancellationPending)
            {
                string item = "ПНВЦ.Эстакада 4.Путь 12.Бензин.Стояк 11.HRADC1VAL";
                string value = opc.FetchItem(server, "group1", item);
                w.ReportProgress(0, value);
                Thread.Sleep(1000);
            }
            opc.FinitOpc();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tbValue.Text = $"{e.UserState}";
        }

        private void FormOpcClient_Load(object sender, EventArgs e)
        {
            //ExploreOpcServers();

            //server = "Lectus.OPC.1";
            //cbOpcServer.Text = server;

            //var props = _opc.GetProps(server).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //listBox1.Items.Clear();
            //foreach (var line in props)
            //{
            //    listBox1.Items.Add(line);
            //}
            //GC.Collect();

            //var item = "ПНВЦ.Эстакада 4.Путь 12.Бензин.Стояк 11.HRADC1VAL";
            //tbItem.Text = item;
            ////_opc.AddItem(cbOpcServer.Text, "group1", item);
            //tbValue.Text = _opc.FetchItem(server, "group1", item);
        }

        private void ExploreOpcServers()
        {
            var servers = new HashSet<string>();
            foreach (var item in _opc.GetServers().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Split('=')[1]))
                servers.Add(item);
            cbOpcServer.Items.Clear();
            foreach (var server in servers.OrderBy(item => item))
                cbOpcServer.Items.Add(server);
        }

        private void FormOpcClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            //_opc.FinitOpc();
            worker.CancelAsync();
        }

        private void cbOpcServer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var props = _opc.GetProps(cbOpcServer.Text).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            listBox1.Items.Clear();
            foreach (var line in props)
            {
                listBox1.Items.Add(line);
            }
            GC.Collect();
        }

        private void btnOpcServersRefresh_Click(object sender, EventArgs e)
        {
            ExploreOpcServers();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listBox1.SelectedItem.ToString().Trim('"').Split(new char[] { '=' })[0];
            tbItem.Text = item;
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            _opc.AddItem(cbOpcServer.Text, "group1", tbItem.Text);
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            tbValue.Text = _opc.FetchItem(server, "group1", tbItem.Text);
        }
    }
}
