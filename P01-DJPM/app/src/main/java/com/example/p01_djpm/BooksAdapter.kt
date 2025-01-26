package com.example.p01_djpm

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.bumptech.glide.load.engine.DiskCacheStrategy
import com.example.p01_djpm.databinding.ItemBookBinding

class BooksAdapter<T>(
    private val books: List<T>,
    private val onBookClick: (T) -> Unit,
    private val onBookLongClick: (String) -> Unit
) : RecyclerView.Adapter<BooksAdapter.BookViewHolder<T>>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): BookViewHolder<T> {
        val binding = ItemBookBinding.inflate(LayoutInflater.from(parent.context), parent, false)
        return BookViewHolder(binding)
    }

    override fun onBindViewHolder(holder: BookViewHolder<T>, position: Int) {
        holder.bind(books[position], onBookClick, onBookLongClick)
    }

    override fun getItemCount(): Int = books.size

    class BookViewHolder<T>(private val binding: ItemBookBinding) :
        RecyclerView.ViewHolder(binding.root) {

        fun bind(
            book: T,
            onClick: (T) -> Unit,
            onLongClick: (String) -> Unit
        ) {
            if (book is BookItem) {
                binding.titleTextView.text = book.volumeInfo.title
                binding.authorTextView.text = book.volumeInfo.authors?.joinToString(", ") ?: "Autor desconhecido"
                loadImage(book.volumeInfo.imageLinks?.thumbnail)
            } else if (book is UserBookItem) {
                binding.titleTextView.text = book.volumeInfo.title
                binding.authorTextView.text = book.volumeInfo.authors?.joinToString(", ") ?: "Autor desconhecido"
                binding.statusTextView.text = "Estado: ${book.status}"
                loadImage(book.volumeInfo.imageLinks?.thumbnail)

                binding.root.setOnLongClickListener {
                    onLongClick(book.id)
                    true
                }
            }

            binding.root.setOnClickListener {
                onClick(book)
            }
        }

        private fun loadImage(thumbnailUrl: String?) {
            if (!thumbnailUrl.isNullOrEmpty()) {
                Glide.with(binding.root.context)
                    .load(thumbnailUrl)
                    .placeholder(R.drawable.placeholder_image)
                    .error(R.drawable.placeholder_image)
                    .diskCacheStrategy(DiskCacheStrategy.ALL)
                    .into(binding.bookCoverImageView)
            } else {
                binding.bookCoverImageView.setImageResource(R.drawable.placeholder_image)
            }
        }
    }
}