using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OpcClient
{
    public partial class FormOpcClient : Form
    {
        readonly OpcBridgeSupport opc = new OpcBridgeSupport();
        readonly BackgroundWorker worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        string[] list;
        string[] items;
        Dictionary<string, string> descriptors = new Dictionary<string, string>();

        public FormOpcClient()
        {
            InitializeComponent();
            lvValues.SetDoubleBuffered(true);
            var mif = new MemIniFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpcClient.ini"));
            items = mif.ReadSectionKeys("Items");
            items.ToList().ForEach(item => descriptors.Add(item, mif.ReadString("Items", item, "")));
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync(new Tuple<OpcBridgeSupport, string[], Dictionary<string, string>>(opc, items, descriptors));
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var arg = (Tuple<OpcBridgeSupport, string[], Dictionary<string, string>>)e.Argument;
            var opc = arg.Item1;
            var items = arg.Item2;
            var descs = arg.Item3;
            var w = (BackgroundWorker)sender;
            string server = "Lectus.OPC.1";
            var group ="{" + $"{Guid.NewGuid()}".ToUpper() + "}";
            // получаем все доступные переменные, отдаваемые OPC сервером
            var props = opc.GetProps(server)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim('"').Split(new char[] { '=' })[0]);
            // определяем список для накопления переменных, указанных в файле настроек
            var addresses = new List<string>();
            // группируем переменные по начальным значениям
            foreach (var groupProps in props.GroupBy(prop => string.Join(".", prop.Split('.'), 0, prop.Split('.').Length - 1)))
                items.ToList().ForEach(item => addresses.Add(groupProps.Key + '.' + item));
            // определяем список для накопления ответов от OPC сервера
            var values = new string[addresses.Count];
            var descriptors = new string[addresses.Count];
            for (var i = 0; i < addresses.Count; i++)
                descriptors[i] = descs[items[i]];
            while (!w.CancellationPending)
            {
                // собираем ответы
                for (var i = 0; i < addresses.Count; i++)
                {
                    var answer = opc.FetchItem(server, group, addresses[i]);
                    values[i] = string.Concat(addresses[i], ';', descriptors[i], ';', answer);
                }
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
            e.Item.SubItems.Add(vals[4]);
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
