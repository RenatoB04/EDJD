package com.example.p01_djpm

import android.content.Intent
import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.example.p01_djpm.databinding.ItemBookBinding

class BooksAdapter(
    private val books: List<BookItem>,
    private val onBookClick: (BookItem) -> Unit
) : RecyclerView.Adapter<BooksAdapter.BookViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): BookViewHolder {
        val binding = ItemBookBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return BookViewHolder(binding)
    }

    override fun onBindViewHolder(holder: BookViewHolder, position: Int) {
        holder.bind(books[position], onBookClick)
    }

    override fun getItemCount(): Int = books.size

    class BookViewHolder(private val binding: ItemBookBinding) : RecyclerView.ViewHolder(binding.root) {
        fun bind(book: BookItem, onClick: (BookItem) -> Unit) {
            binding.titleTextView.text = book.volumeInfo.title
            binding.authorTextView.text = book.volumeInfo.authors?.joinToString(", ") ?: "Autor desconhecido"

            binding.root.setOnClickListener { onClick(book) }
        }
    }
}