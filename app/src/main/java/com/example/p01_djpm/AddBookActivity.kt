package com.example.p01_djpm

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.p01_djpm.databinding.ActivityAddBookBinding
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class AddBookActivity : AppCompatActivity() {
    private lateinit var binding: ActivityAddBookBinding
    private val apiClient = ApiClient.retrofit.create(GoogleBooksApi::class.java)
    private val apiKey = "API_KEY"
    private var isSearching = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityAddBookBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.searchButton.setOnClickListener {
            val query = binding.searchEditText.text.toString()
            if (query.isNotBlank()) {
                searchBooks(query)
            } else {
                Toast.makeText(this, "Digite um termo de pesquisa", Toast.LENGTH_SHORT).show()
            }
        }

        binding.scanButton.setOnClickListener {
            startBarcodeScanner()
        }

        handleIncomingISBN()
    }

    private fun handleIncomingISBN() {
        if (isSearching) {
            Log.d("AddBookActivity", "Pesquisa em andamento. Ignorando novo ISBN.")
            return
        }

        val scannedIsbn = intent.getStringExtra("isbn")
        scannedIsbn?.let {
            Log.d("AddBookActivity", "ISBN Recebido: $it")
            Toast.makeText(this, "ISBN Detetado: $it", Toast.LENGTH_SHORT).show()
            searchBooks(it)
        }
    }

    private fun startBarcodeScanner() {
        val intent = Intent(this, BarcodeScannerActivity::class.java)
        try {
            startActivityForResult(intent, BARCODE_SCANNER_REQUEST)
        } catch (e: Exception) {
            Log.e("AddBookActivity", "Erro ao iniciar BarcodeScannerActivity: ${e.message}")
            Toast.makeText(this, "Erro ao abrir scanner", Toast.LENGTH_SHORT).show()
        }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == BARCODE_SCANNER_REQUEST && resultCode == RESULT_OK) {
            val barcode = data?.getStringExtra("barcode")
            if (!barcode.isNullOrEmpty() && !isSearching) {
                Log.d("AddBookActivity", "ISBN do scanner: $barcode")
                searchBooks(barcode)
            } else {
                Log.d("AddBookActivity", "Nenhum código de barras retornado ou pesquisa em andamento.")
                Toast.makeText(this, "Nenhum código encontrado", Toast.LENGTH_SHORT).show()
            }
        } else {
            Log.d("AddBookActivity", "Scanner cancelado ou falhou.")
        }
    }

    private fun searchBooks(query: String) {
        if (isSearching) {
            Log.d("AddBookActivity", "Já há uma pesquisa em andamento. A ignorar nova pesquisa.")
            return
        }

        isSearching = true
        Log.d("AddBookActivity", "Iniciando pesquisa para: $query")

        apiClient.searchBooks(query, apiKey).enqueue(object : Callback<BooksResponse> {
            override fun onResponse(call: Call<BooksResponse>, response: Response<BooksResponse>) {
                isSearching = false
                if (response.isSuccessful) {
                    val books = response.body()?.items ?: emptyList()
                    if (books.isNotEmpty()) {
                        Log.d("AddBookActivity", "Livros encontrados: ${books.size}")
                        setupRecyclerView(books)
                    } else {
                        Log.d("AddBookActivity", "Nenhum livro encontrado para: $query")
                        Toast.makeText(this@AddBookActivity, "Nenhum livro encontrado.", Toast.LENGTH_SHORT).show()
                    }
                } else {
                    Log.e("AddBookActivity", "Erro na resposta da API: ${response.errorBody()?.string()}")
                    Toast.makeText(this@AddBookActivity, "Erro ao procurar livros.", Toast.LENGTH_SHORT).show()
                }
            }

            override fun onFailure(call: Call<BooksResponse>, t: Throwable) {
                isSearching = false
                Log.e("AddBookActivity", "Erro: ${t.message}")
                Toast.makeText(this@AddBookActivity, "Erro: ${t.message}", Toast.LENGTH_SHORT).show()
            }
        })
    }

    private fun setupRecyclerView(books: List<BookItem>) {
        binding.booksRecyclerView.layoutManager = LinearLayoutManager(this)
        binding.booksRecyclerView.adapter = BooksAdapter(books) { book ->
            val intent = Intent(this, BookDetailsActivity::class.java).apply {
                putExtra("title", book.volumeInfo.title)
                putExtra("author", book.volumeInfo.authors?.joinToString(", "))
                putExtra("description", book.volumeInfo.description)
                putExtra("thumbnail", book.volumeInfo.imageLinks?.thumbnail)
            }
            try {
                startActivity(intent)
                Log.d("AddBookActivity", "Livro selecionado: ${book.volumeInfo.title}")
            } catch (e: Exception) {
                Log.e("AddBookActivity", "Erro ao abrir detalhes do livro: ${e.message}")
                Toast.makeText(this, "Erro ao abrir detalhes do livro", Toast.LENGTH_SHORT).show()
            }
        }
    }

    companion object {
        private const val BARCODE_SCANNER_REQUEST = 1001
    }
}