using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace P01_SAD
{
    public partial class FormInserir : Form
    {
        private DataGridView dataGridViewContactos;
        private TextBox txtNome;
        private TextBox txtEmail;
        private Button btnInserir;

        public FormInserir()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.dataGridViewContactos = new DataGridView();
            this.txtNome = new TextBox();
            this.txtEmail = new TextBox();
            this.btnInserir = new Button();

            this.SuspendLayout();

            this.dataGridViewContactos.Location = new System.Drawing.Point(12, 80);
            this.dataGridViewContactos.Size = new System.Drawing.Size(400, 200);
            this.dataGridViewContactos.Columns.Add("Telefone", "Telefone");

            this.txtNome.Location = new System.Drawing.Point(12, 12);
            this.txtNome.Size = new System.Drawing.Size(200, 20);
            this.txtNome.PlaceholderText = "Nome";

            this.txtEmail.Location = new System.Drawing.Point(12, 40);
            this.txtEmail.Size = new System.Drawing.Size(200, 20);
            this.txtEmail.PlaceholderText = "Email";

            this.btnInserir.Location = new System.Drawing.Point(12, 300);
            this.btnInserir.Size = new System.Drawing.Size(100, 30);
            this.btnInserir.Text = "Inserir";
            this.btnInserir.Click += new EventHandler(this.BtnInserir_Click);

            this.ClientSize = new System.Drawing.Size(450, 350);
            this.Controls.Add(this.dataGridViewContactos);
            this.Controls.Add(this.txtNome);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.btnInserir);
            this.Text = "Inserir Cliente e Contactos";

            this.ResumeLayout(false);
        }

        private void BtnInserir_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=LEGION;Database=P01-SAD;Trusted_Connection=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string queryCliente = "INSERT INTO cliente (Nome, Email) VALUES (@Nome, @Email); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdCliente = new SqlCommand(queryCliente, connection);
                    cmdCliente.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmdCliente.Parameters.AddWithValue("@Email", txtEmail.Text);
                    int clienteId = Convert.ToInt32(cmdCliente.ExecuteScalar());

                    foreach (DataGridViewRow row in dataGridViewContactos.Rows)
                    {
                        if (row.Cells[0].Value != null)
                        {
                            string queryContacto = "INSERT INTO contacto (ClienteId, Telefone) VALUES (@ClienteId, @Telefone);";
                            SqlCommand cmdContacto = new SqlCommand(queryContacto, connection);
                            cmdContacto.Parameters.AddWithValue("@ClienteId", clienteId);
                            cmdContacto.Parameters.AddWithValue("@Telefone", row.Cells[0].Value.ToString());
                            cmdContacto.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Cliente e contactos inseridos com sucesso!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
}