package com.example.p01_djpm

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.camera.core.ImageAnalysis
import androidx.camera.core.ImageProxy
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.p01_djpm.databinding.ActivityAddBookBinding
import com.google.mlkit.vision.barcode.common.Barcode
import com.google.mlkit.vision.barcode.BarcodeScanning
import com.google.mlkit.vision.common.InputImage
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class AddBookActivity : AppCompatActivity() {
    private lateinit var binding: ActivityAddBookBinding
    private val apiClient = ApiClient.retrofit.create(GoogleBooksApi::class.java)
    private val apiKey = "API_KEY"

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
    }

    private fun startBarcodeScanner() {
        val intent = Intent(this, BarcodeScannerActivity::class.java)
        startActivityForResult(intent, BARCODE_SCANNER_REQUEST)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == BARCODE_SCANNER_REQUEST && resultCode == RESULT_OK) {
            val barcode = data?.getStringExtra("barcode")
            if (!barcode.isNullOrEmpty()) {
                searchBooks(barcode)
            } else {
                Toast.makeText(this, "Nenhum código encontrado", Toast.LENGTH_SHORT).show()
            }
        }
    }

    private fun searchBooks(query: String) {
        apiClient.searchBooks(query, apiKey).enqueue(object : Callback<BooksResponse> {
            override fun onResponse(call: Call<BooksResponse>, response: Response<BooksResponse>) {
                if (response.isSuccessful) {
                    val books = response.body()?.items ?: emptyList()
                    setupRecyclerView(books)
                } else {
                    Toast.makeText(this@AddBookActivity, "Nenhum livro encontrado.", Toast.LENGTH_SHORT).show()
                }
            }

            override fun onFailure(call: Call<BooksResponse>, t: Throwable) {
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
            startActivity(intent)
        }
    }

    companion object {
        private const val BARCODE_SCANNER_REQUEST = 1001
    }
}