package com.example.p01_djpm

import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.appcompat.app.AlertDialog
import com.example.p01_djpm.databinding.ActivityHomeBinding
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.firestore.FirebaseFirestore

class HomeActivity : AppCompatActivity() {
    private lateinit var binding: ActivityHomeBinding
    private val db = FirebaseFirestore.getInstance()
    private val currentUser = FirebaseAuth.getInstance().currentUser

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.booksRecyclerView.layoutManager = LinearLayoutManager(this)

        binding.addBookButton.setOnClickListener {
            val intent = Intent(this, AddBookActivity::class.java)
            startActivity(intent)
        }

        fetchUserLibrary()
    }

    // Busca os livros do Firestore
    private fun fetchUserLibrary() {
        if (currentUser == null) {
            Toast.makeText(this, "Utilizador não autenticado.", Toast.LENGTH_SHORT).show()
            return
        }

        db.collection("books")
            .whereEqualTo("userId", currentUser.uid)
            .get()
            .addOnSuccessListener { documents ->
                val books = documents.map { document ->
                    UserBookItem(
                        id = document.id,
                        volumeInfo = VolumeInfo(
                            title = document.getString("title") ?: "Sem título",
                            authors = document.getString("author")?.let { listOf(it) }
                                ?: listOf("Autor desconhecido"),
                            description = document.getString("description") ?: "Sem descrição",
                            imageLinks = ImageLinks(
                                thumbnail = document.getString("thumbnail") ?: ""
                            )
                        ),
                        status = document.getString("status") ?: "Desconhecido"
                    )
                }
                setupRecyclerView(books)
            }
            .addOnFailureListener { exception ->
                Toast.makeText(
                    this,
                    "Erro ao carregar a biblioteca: ${exception.message}",
                    Toast.LENGTH_SHORT
                ).show()
            }
    }

    private fun setupRecyclerView(books: List<UserBookItem>) {
        val adapter = BooksAdapter(
            books,
            { book ->
                val intent = Intent(this, BookDetailsActivity::class.java).apply {
                    putExtra("title", book.volumeInfo.title)
                    putExtra("author", book.volumeInfo.authors?.joinToString(", ") ?: "Desconhecido")
                    putExtra("description", book.volumeInfo.description ?: "Sem descrição")
                    putExtra("thumbnail", book.volumeInfo.imageLinks?.thumbnail ?: "")
                }
                startActivity(intent)
            },
            { bookId ->
                confirmDelete(bookId)
            }
        )
        binding.booksRecyclerView.adapter = adapter
    }

    private fun confirmDelete(bookId: String) {
        AlertDialog.Builder(this)
            .setTitle("Remover Livro")
            .setMessage("Tem a certeza que deseja remover este livro da biblioteca?")
            .setPositiveButton("Sim") { _, _ ->
                deleteBook(bookId)
            }
            .setNegativeButton("Cancelar", null)
            .show()
    }

    private fun deleteBook(bookId: String) {
        db.collection("books").document(bookId)
            .delete()
            .addOnSuccessListener {
                Toast.makeText(this, "Livro removido com sucesso.", Toast.LENGTH_SHORT).show()
                fetchUserLibrary()
            }
            .addOnFailureListener { e ->
                Toast.makeText(
                    this,
                    "Erro ao remover o livro: ${e.message}",
                    Toast.LENGTH_SHORT
                ).show()
            }
    }
}