namespace P01_SAD
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnAbrirInserir;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnPesquisar;
        private System.Windows.Forms.TextBox txtNIFPesquisar;

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
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnPesquisar = new System.Windows.Forms.Button();
            this.txtNIFPesquisar = new System.Windows.Forms.TextBox();

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

            this.btnEliminar.Location = new System.Drawing.Point(180, 380);
            this.btnEliminar.Size = new System.Drawing.Size(150, 30);
            this.btnEliminar.Text = "Eliminar Cliente";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            this.Controls.Add(this.btnEliminar);

            this.btnEditar.Location = new System.Drawing.Point(350, 380);
            this.btnEditar.Size = new System.Drawing.Size(150, 30);
            this.btnEditar.Text = "Editar Cliente";
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            this.Controls.Add(this.btnEditar);

            this.txtNIFPesquisar.Location = new System.Drawing.Point(520, 385);
            this.txtNIFPesquisar.Size = new System.Drawing.Size(120, 22);
            this.txtNIFPesquisar.PlaceholderText = "Digite o NIF";
            this.Controls.Add(this.txtNIFPesquisar);

            this.btnPesquisar.Location = new System.Drawing.Point(650, 380);
            this.btnPesquisar.Size = new System.Drawing.Size(120, 30);
            this.btnPesquisar.Text = "Pesquisar Cliente";
            this.btnPesquisar.Click += new System.EventHandler(this.btnPesquisar_Click);
            this.Controls.Add(this.btnPesquisar);

            this.Load += new System.EventHandler(this.Form1_Load);

            this.ResumeLayout(false);
        }

        #endregion
    }
}