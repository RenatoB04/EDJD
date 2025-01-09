namespace P01_SAD
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnAbrirInserir;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnAbrirInserir = new System.Windows.Forms.Button();

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Gestão de Clientes";

            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Size = new System.Drawing.Size(760, 350);
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.ReadOnly = true;
            this.Controls.Add(this.dataGridView1);

            this.btnAbrirInserir.Location = new System.Drawing.Point(12, 380);
            this.btnAbrirInserir.Size = new System.Drawing.Size(150, 30);
            this.btnAbrirInserir.Text = "Novo Cliente";
            this.btnAbrirInserir.Click += new System.EventHandler(this.btnAbrirInserir_Click);
            this.Controls.Add(this.btnAbrirInserir);

            this.Load += new System.EventHandler(this.Form1_Load);

            this.ResumeLayout(false);
        }

        #endregion
    }
}