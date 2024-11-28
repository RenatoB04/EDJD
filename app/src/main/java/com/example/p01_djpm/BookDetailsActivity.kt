package com.example.p01_djpm

import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.p01_djpm.databinding.ActivityBookDetailsBinding
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.firestore.FirebaseFirestore

class BookDetailsActivity : AppCompatActivity() {
    private lateinit var binding: ActivityBookDetailsBinding
    private val db = FirebaseFirestore.getInstance()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityBookDetailsBinding.inflate(layoutInflater)
        setContentView(binding.root)

        val title = intent.getStringExtra("title")
        val author = intent.getStringExtra("author")
        val description = intent.getStringExtra("description")
        val thumbnail = intent.getStringExtra("thumbnail")

        binding.bookTitleTextView.text = title
        binding.bookAuthorTextView.text = author ?: "Autor desconhecido"
        binding.bookDescriptionTextView.text = description ?: "Sem descrição disponível"

        binding.readButton.setOnClickListener {
            saveBookToFirestore(title, author, description, "Lido")
        }

        binding.wishlistButton.setOnClickListener {
            saveBookToFirestore(title, author, description, "Lista de Desejos")
        }

        binding.readingButton.setOnClickListener {
            saveBookToFirestore(title, author, description, "Em Leitura")
        }
    }

    private fun saveBookToFirestore(title: String?, author: String?, description: String?, status: String) {
        val userId = FirebaseAuth.getInstance().currentUser?.uid

        if (userId == null) {
            Toast.makeText(this, "Utilizador não autenticado.", Toast.LENGTH_SHORT).show()
            return
        }

        val book = hashMapOf(
            "title" to title,
            "author" to author,
            "description" to description,
            "status" to status,
            "userId" to userId
        )

        db.collection("books")
            .add(book)
            .addOnSuccessListener {
                Toast.makeText(this, "Livro adicionado como '$status'.", Toast.LENGTH_SHORT).show()
                finish()
            }
            .addOnFailureListener { e ->
                Toast.makeText(this, "Erro ao guardar livro: ${e.message}", Toast.LENGTH_SHORT).show()
            }
    }
}