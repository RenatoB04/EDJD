package com.example.p01_djpm

import android.os.Bundle
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
    }

    private fun searchBooks(query: String) {
        apiClient.searchBooks(query, apiKey).enqueue(object : Callback<BooksResponse> {
            override fun onResponse(call: Call<BooksResponse>, response: Response<BooksResponse>) {
                if (response.isSuccessful) {
                    val books = response.body()?.items ?: emptyList()
                    setupRecyclerView(books)
                } else {
                    Toast.makeText(this@AddBookActivity, "Erro na pesquisa", Toast.LENGTH_SHORT).show()
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
            Toast.makeText(this, "Selecionou: ${book.volumeInfo.title}", Toast.LENGTH_SHORT).show()
        }
    }
}