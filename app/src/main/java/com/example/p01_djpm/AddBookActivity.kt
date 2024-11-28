package com.example.p01_djpm

import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.p01_djpm.databinding.ActivityAddBookBinding
import com.google.firebase.firestore.FirebaseFirestore
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.Timestamp

class AddBookActivity : AppCompatActivity() {
    private lateinit var binding: ActivityAddBookBinding
    private val db = FirebaseFirestore.getInstance()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityAddBookBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.saveBookButton.setOnClickListener {
            val title = binding.titleEditText.text.toString()
            val author = binding.authorEditText.text.toString()
            val userId = FirebaseAuth.getInstance().currentUser?.uid

            if (title.isBlank() || author.isBlank()) {
                Toast.makeText(this, "Preencha todos os campos!", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            val book = hashMapOf(
                "title" to title,
                "author" to author,
                "date" to Timestamp.now(),
                "userId" to userId
            )

            db.collection("books")
                .add(book)
                .addOnSuccessListener {
                    Toast.makeText(this, "Livro adicionado com sucesso!", Toast.LENGTH_SHORT).show()
                    finish()
                }
                .addOnFailureListener { e ->
                    Toast.makeText(this, "Erro ao adicionar livro: ${e.message}", Toast.LENGTH_SHORT).show()
                }
        }
    }
}