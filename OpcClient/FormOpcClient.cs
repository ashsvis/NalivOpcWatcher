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
        OpcItemAnswer[] list;
        readonly Dictionary<string, string> descriptors = new Dictionary<string, string>();

        public FormOpcClient()
        {
            InitializeComponent();
            lvValues.SetDoubleBuffered(true);
            var mif = new MemIniFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpcClient.ini"));
            // перечень адресов стояков налива
            var risers = new List<string>();
            var prefix = mif.ReadString("Risers", "Prefix", "");
            var first = mif.ReadInteger("Risers", "First", 0);
            var last = mif.ReadInteger("Risers", "Last", 0);
            var step = mif.ReadInteger("Risers", "Step", 1);
            for (var i = first; i <= last; i += step)
                risers.Add($"{prefix}.Стояк {i}");

            // перечень имён переменных опроса
            var items = mif.ReadSectionKeys("Items");
            // заполняем словарь дескрипторов для имён переменных опроса
            items.ToList().ForEach(item => descriptors.Add(item, mif.ReadString("Items", item, "")));
            // заполняем список адресов переменных опроса
            var addresses = new List<Tuple<string, string>>();
            foreach (var riser in risers)
                items.ToList().ForEach(item => addresses.Add(new Tuple<string, string>(riser + '.' + item, descriptors[item])));

            // настраиваем и запускаем на выполнение фоновый поток для опроса значений переменных
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync(new Tuple<OpcBridgeSupport, Tuple<string, string>[]>(opc, addresses.ToArray()));
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var arg = (Tuple<OpcBridgeSupport, Tuple<string, string>[]>)e.Argument;
            var opc = arg.Item1;
            var addresses = arg.Item2;
            var w = (BackgroundWorker)sender;
            string server = "Lectus.OPC.1";
            var group ="{" + $"{Guid.NewGuid()}".ToUpper() + "}";
            // определяем список для накопления ответов от OPC сервера
            while (!w.CancellationPending)
            {
                var values = new List<OpcItemAnswer>();
                // собираем ответы
                for (var i = 0; i < addresses.Length; i++)
                {
                    var answer = opc.FetchItem(server, group, addresses[i].Item1);
                    values.Add(new OpcItemAnswer(addresses[i].Item1, answer, addresses[i].Item2));
                }
                w.ReportProgress(values.Count, values.ToArray());
                Thread.Sleep(1000);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            list = (OpcItemAnswer[])e.UserState;
            lvValues.VirtualListSize = e.ProgressPercentage;
            lvValues.Invalidate();
        }

        private void lvValues_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem();
            var answerItem = list[e.ItemIndex];
            e.Item.Text = answerItem.Address;
            e.Item.SubItems.Add(answerItem.Descriptor);
            e.Item.SubItems.Add(answerItem.Value.ToString());
            e.Item.SubItems.Add(answerItem.Quality);
            e.Item.SubItems.Add(answerItem.SnapTime.ToString());
        }

        private void FormOpcClient_Load(object sender, EventArgs e)
        {
        }

        private void FormOpcClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            worker.CancelAsync();
            opc.FinitOpc();
        }
    }
}
