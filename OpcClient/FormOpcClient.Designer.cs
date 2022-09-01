namespace OpcClient
{
    partial class FormOpcClient
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cbOpcServer = new System.Windows.Forms.ComboBox();
            this.btnOpcServersRefresh = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.tbItem = new System.Windows.Forms.TextBox();
            this.btnFetch = new System.Windows.Forms.Button();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "OPC сервер:";
            // 
            // cbOpcServer
            // 
            this.cbOpcServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOpcServer.FormattingEnabled = true;
            this.cbOpcServer.Location = new System.Drawing.Point(16, 34);
            this.cbOpcServer.Name = "cbOpcServer";
            this.cbOpcServer.Size = new System.Drawing.Size(210, 25);
            this.cbOpcServer.TabIndex = 1;
            this.cbOpcServer.SelectionChangeCommitted += new System.EventHandler(this.cbOpcServer_SelectionChangeCommitted);
            // 
            // btnOpcServersRefresh
            // 
            this.btnOpcServersRefresh.Location = new System.Drawing.Point(232, 33);
            this.btnOpcServersRefresh.Name = "btnOpcServersRefresh";
            this.btnOpcServersRefresh.Size = new System.Drawing.Size(87, 25);
            this.btnOpcServersRefresh.TabIndex = 2;
            this.btnOpcServersRefresh.Text = "Обновить";
            this.btnOpcServersRefresh.UseVisualStyleBackColor = true;
            this.btnOpcServersRefresh.Click += new System.EventHandler(this.btnOpcServersRefresh_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 17;
            this.listBox1.Location = new System.Drawing.Point(16, 65);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(303, 191);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // btnAddItem
            // 
            this.btnAddItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddItem.Location = new System.Drawing.Point(16, 274);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(75, 25);
            this.btnAddItem.TabIndex = 4;
            this.btnAddItem.Text = "Добавить";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // tbItem
            // 
            this.tbItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbItem.Location = new System.Drawing.Point(98, 275);
            this.tbItem.Name = "tbItem";
            this.tbItem.Size = new System.Drawing.Size(221, 25);
            this.tbItem.TabIndex = 5;
            // 
            // btnFetch
            // 
            this.btnFetch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFetch.Location = new System.Drawing.Point(16, 305);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(75, 25);
            this.btnFetch.TabIndex = 4;
            this.btnFetch.Text = "Опросить";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // tbValue
            // 
            this.tbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbValue.Location = new System.Drawing.Point(98, 306);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(221, 25);
            this.tbValue.TabIndex = 5;
            // 
            // FormOpcClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 343);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.btnFetch);
            this.Controls.Add(this.tbItem);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnOpcServersRefresh);
            this.Controls.Add(this.cbOpcServer);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormOpcClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OPC клиент";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOpcClient_FormClosing);
            this.Load += new System.EventHandler(this.FormOpcClient_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbOpcServer;
        private System.Windows.Forms.Button btnOpcServersRefresh;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.TextBox tbItem;
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.TextBox tbValue;
    }
}

