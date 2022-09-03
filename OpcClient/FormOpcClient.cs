using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace OpcClient
{
    public partial class FormOpcClient : Form
    {
        readonly OpcBridgeSupport opc = new OpcBridgeSupport();
        readonly BackgroundWorker worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        OpcItemAnswer[] listValues;
        OpcItemChange[] listChanges;
        readonly Dictionary<string, string> descriptors = new Dictionary<string, string>();
        public static ConcurrentDictionary<string, OpcItemAnswer> OpcItemAnswers = new ConcurrentDictionary<string, OpcItemAnswer>();
        public static ConcurrentDictionary<string, OpcItemChange> OpcItemChanges = new ConcurrentDictionary<string, OpcItemChange>();

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
            foreach (var riser in risers)
                items.ToList().ForEach(item => OpcItemAnswers.TryAdd(riser + '.' + item, new OpcItemAnswer(riser + '.' + item, "", descriptors[item])));

            // настраиваем и запускаем на выполнение фоновый поток для опроса значений переменных
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
            // определяем список для накопления ответов от OPC сервера
            while (!w.CancellationPending)
            {
                var values = new List<OpcItemAnswer>();
                var changes = new List<OpcItemChange>();
                // собираем ответы
                foreach (var address in OpcItemAnswers.Keys)
                {
                    if (OpcItemAnswers.TryGetValue(address, out var valueItem))
                    {
                        string content = opc.FetchItem(server, group, address);
                        var answerItem = new OpcItemAnswer(address, content, valueItem.Descriptor);
                        if (OpcItemAnswers.TryUpdate(address, answerItem, valueItem))
                        {
                            values.Add(answerItem);
                            if (valueItem.Value != answerItem.Value)
                            {
                                var newItem = new OpcItemChange
                                {
                                    SnapTime = answerItem.SnapTime,
                                    Address = address,
                                    Descriptor = answerItem.Descriptor,
                                    ValueOld = "",
                                    ValueNew = answerItem.Value.ToString()
                                };
                                if (OpcItemChanges.TryGetValue(address, out var oldItem))
                                {
                                    newItem.ValueOld = oldItem.ValueNew;
                                    if (OpcItemChanges.TryUpdate(address, newItem, oldItem))
                                        changes.Add(newItem);
                                }
                                else
                                    if (OpcItemChanges.TryAdd(address, newItem))
                                        changes.Add(newItem);
                            }
                        }
                    }
                }
                w.ReportProgress(0, values.ToArray());
                w.ReportProgress(1, changes.ToArray());
                Thread.Sleep(1000);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:
                    listValues = (OpcItemAnswer[])e.UserState;
                    lvValues.VirtualListSize = listValues.Length;
                    lvValues.Invalidate();
                    break;
                case 1:
                    listChanges = (OpcItemChange[])e.UserState;
                    lvChanges.VirtualListSize = listChanges.Length;
                    lvChanges.Invalidate();
                    break;
            }
        }

        private void lvValues_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem();
            var answerItem = listValues[e.ItemIndex];
            e.Item.Text = answerItem.Address;
            e.Item.SubItems.Add(answerItem.Descriptor);
            e.Item.SubItems.Add(answerItem.Value.ToString());
            e.Item.SubItems.Add(answerItem.Quality);
            e.Item.SubItems.Add(answerItem.SnapTime.ToString());
        }

        private void lvChanges_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem();
            var changeItem = listChanges[e.ItemIndex];
            e.Item.Text = changeItem.SnapTime.ToString();
            e.Item.SubItems.Add(changeItem.Address);
            e.Item.SubItems.Add(changeItem.Descriptor);
            e.Item.SubItems.Add(changeItem.ValueOld);
            e.Item.SubItems.Add(changeItem.ValueNew);
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
