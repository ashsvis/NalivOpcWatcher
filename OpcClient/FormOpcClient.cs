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
        OpcBridgeSupport opc = new OpcBridgeSupport();
        private BackgroundWorker worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        string[] list;

        public FormOpcClient()
        {
            InitializeComponent();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync(opc);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var opc = (OpcBridgeSupport)e.Argument;
            var w = (BackgroundWorker)sender;
            string server = "Lectus.OPC.1";
            var group ="{" + $"{Guid.NewGuid()}".ToUpper() + "}";
            var props = opc.GetProps(server)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim('"').Split(new char[] { '=' })[0]).ToArray();
            string[] values = new string[props.Length];
            while (!w.CancellationPending)
            {
                for (var i = 0; i < props.Length; i++)
                    values[i] = string.Concat(props[i], ';', opc.FetchItem(server, group, props[i]));
                w.ReportProgress(values.Length, values);
                Thread.Sleep(1000);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            list = (string[])e.UserState;
            lvValues.VirtualListSize = e.ProgressPercentage;
            lvValues.Invalidate();
        }

        private void lvValues_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem();
            var vals = list[e.ItemIndex].Split(';');
            e.Item.Text = vals[0];
            e.Item.SubItems.Add(vals[1]);
            e.Item.SubItems.Add(vals[2]);
            e.Item.SubItems.Add(vals[3]);
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
            //var servers = new HashSet<string>();
            //foreach (var item in _opc.GetServers().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Split('=')[1]))
            //    servers.Add(item);
            //cbOpcServer.Items.Clear();
            //foreach (var server in servers.OrderBy(item => item))
            //    cbOpcServer.Items.Add(server);
        }

        private void FormOpcClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            worker.CancelAsync();
            opc.FinitOpc();
            //worker.Dispose();
        }

        private void cbOpcServer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //var props = _opc.GetProps(cbOpcServer.Text).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //listBox1.Items.Clear();
            //foreach (var line in props)
            //{
            //    listBox1.Items.Add(line);
            //}
            //GC.Collect();
        }

        private void btnOpcServersRefresh_Click(object sender, EventArgs e)
        {
            ExploreOpcServers();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var item = listBox1.SelectedItem.ToString().Trim('"').Split(new char[] { '=' })[0];
            //tbItem.Text = item;
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            //opc.AddItem(cbOpcServer.Text, "group1", tbItem.Text);
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            //tbValue.Text = _opc.FetchItem(server, "group1", tbItem.Text);
        }
    }
}
